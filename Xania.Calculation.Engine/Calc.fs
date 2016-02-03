namespace Xania.Calculation.Engine

    module Tree =

        type Result =
            | Amount of decimal
            | Number of int
            | Text of string
            | Array of Result list
            | Complex of (string * Result) list
            | Failure of string

        type 'a Tree = 
            | Const of Result
            | Leaf of ('a -> Result)
            | Node of ( string * 'a Tree ) list

        let mapTuple f (s, t) = (s, f t)

        let rec mapTree (f:'a -> 'b) tree = 
            let mapTuples = mapTree >> mapTuple >> List.map
            match tree with
            | Const c -> Const c
            | Leaf a -> Leaf (f >> a)
            | Node tuples -> Node ( mapTuples f tuples )

        let rec apply tree x =
            let applySub tree = apply tree x
            match tree with
            | Const c -> c
            | Leaf f -> f x
            | Node tuples -> List.map (mapTuple applySub) tuples |> Complex

        let rec resultToJson result =
            let tupleToJson (n, v) = "\"" + n + "\" : " + (resultToJson v) + ""
            match result with
                | Text c -> "\"" + c + "\""
                | Failure message -> "\"Failure: " + message + "\""
                | Number n -> n.ToString()
                | Amount a -> a.ToString()
                | Array arr -> "[ " + (String.concat ", " (List.map resultToJson arr)) + " ]"
                | Complex tuples -> "{ " + (String.concat ", " (List.map tupleToJson tuples)) + " }"

        let treeToJson tree input = apply tree input |> resultToJson

        //// helpers
        let toText x = x.ToString() |> Text

        let bindInput f input = (input, f input)
        let bindInputs f parent = f parent |> List.map (fun input -> (parent, input))

        let branchMany name path f = (name , path >> List.map f >> Array |> Leaf )
        let branch name path f = (name , path >> f |> Leaf )

        let bindResults f = List.map f >> Array
        let bindTrees f = bindResults (apply f)

    module Expression =

        open System
        open Microsoft.FSharp.Compiler.SourceCodeServices

        let checker = FSharpChecker.Create()

        /// Get untyped tree for a specified input
        let getUntypedTree (file, input) = 
          // Get compiler options for the 'project' implied by a single script file
          let projOptions = 
              checker.GetProjectOptionsFromScript(file, input)
              |> Async.RunSynchronously

          // Run the first phase (untyped parsing) of the compiler
          let parseFileResults = 
              checker.ParseFileInProject(file, input, projOptions) 
              |> Async.RunSynchronously

          match parseFileResults.ParseTree with
          | Some tree -> tree
          | None -> failwith "Something went wrong during parsing!"

        open Microsoft.FSharp.Compiler.Ast

        let rec visitPattern = function
          | SynPat.Wild(_) -> 
              printfn "  .. underscore pattern"
          | SynPat.Named(pat, name, _, _, _) ->
              visitPattern pat
              printfn "  .. named as '%s'" name.idText
          | SynPat.LongIdent(LongIdentWithDots(ident, _), _, _, _, _, _) ->
              let names = String.concat "." [ for i in ident -> i.idText ]
              printfn "  .. identifier: %s" names
          | pat -> printfn "  .. other pattern: %A" pat

        let rec visitExpression = function
          | SynExpr.Ident op -> 
              printfn "ID (%A)" op.idText
          | SynExpr.Const (a, _) -> 
              printf "Const ( "
              match a with
              | SynConst.Int32 x -> printf "Int 32 (%A)" x
              | _ -> printf "not supported %A" a
              printfn ")"
          | SynExpr.Paren (x, _, _, _) -> 
              printfn "("
              visitExpression x
              printfn ")"
          | SynExpr.App(_, _, x, y, _) -> 
              printfn "App"
              visitExpression x
              visitExpression y
          | expr -> failwith " - not supported expression: %A" expr

        let visitDeclarations decls = 
          let getExpr decl =
            match decl with
            | SynModuleDecl.DoExpr (_, e, _) -> Some e
            | _ -> None
          List.map getExpr decls

        let visitModulesAndNamespaces modulesOrNss =
          let getExpr moduleOrNs =
            let (SynModuleOrNamespace(lid, isMod, decls, xml, attrs, _, m)) = moduleOrNs
            visitDeclarations decls
          modulesOrNss |> List.collect getExpr |> List.filter Option.isSome

        let parseExpression input =
            let tree = getUntypedTree (@"c:\input.fs", input)
            match tree with
            | ParsedInput.ImplFile(implFile) ->
                // Extract declarations and walk over them
                let (ParsedImplFileInput(fn, script, name, _, _, modules, _)) = implFile
                visitModulesAndNamespaces modules |> List.filter Option.isSome |> List.head
            | _ -> failwith "F# Interface file (*.fsi) not supported."

        type Operator =
        | Const of int
        | Unary of (int -> int)
        | Binary of (int -> int -> int)

        let rec evalExpression expr = 
            let rec evalApp x = 
                match x with
                | SynExpr.App (_, _, o, b, _) -> 
                    let op = evalApp o
                    match op with 
                    | Binary f -> Unary (f (evalExpression b))
                    | Unary f -> Const (f (evalExpression b))
                    | _ -> failwith (" - unexpected opeator : " + op.ToString())
                | SynExpr.Ident o -> Binary (+)
                | _ -> failwith (" - not supported expression: " + x.ToString())
            match expr with
            | SynExpr.Const (a, _) -> 
                match a with
                | SynConst.Int32 x -> x
                | _ -> 0
            //          | SynExpr.Paren (x, _, _, _) -> 
            //              printfn "("
            //              visitExpression x
            //              printfn ")"
            | SynExpr.App (_, _, o, right, _) -> 
                let app = evalApp o
                evalExpression right |> evalApp o
//                null |> evalApp args
//                match op with
//                | SynExpr.Ident _ -> failwith (" - not supported expression: " + op.ToString())
//                | _ -> 
//                printfn "%A" expr
//                failwith (" - not supported expression: " + op.ToString())
            | expr -> failwith (" - not supported expression: " + expr.ToString())

