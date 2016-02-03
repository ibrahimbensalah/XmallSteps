#r "bin/Debug/FSharp.Compiler.Service.dll"

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
  for declaration in decls do
    match declaration with
    | SynModuleDecl.DoExpr (_, e, _) -> visitExpression e
    | _ -> failwith " - not supported declaration: %A" declaration

let visitModulesAndNamespaces modulesOrNss =
  for moduleOrNs in modulesOrNss do
    let (SynModuleOrNamespace(lid, isMod, decls, xml, attrs, _, m)) = moduleOrNs
    visitDeclarations decls

let parseExpression input =
    let tree = getUntypedTree (@"c:\input.fs", input)
    match tree with
    | ParsedInput.ImplFile(implFile) ->
        // Extract declarations and walk over them
        let (ParsedImplFileInput(fn, script, name, _, _, modules, _)) = implFile
        visitModulesAndNamespaces modules
    | _ -> failwith "F# Interface file (*.fsi) not supported."
