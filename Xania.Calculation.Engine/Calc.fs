namespace Xania.Calculation.Engine

module Calc =

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
        let mapTuples = List.map << mapTuple << mapTree
        match tree with
        | Const c -> Const c
        | Leaf a -> Leaf (f >> a)
        | Node tuples -> Node ( mapTuples f tuples )

    let rec applyTree tree x =
        let applySubTree tree = applyTree tree x
        match tree with
        | Const c -> c
        | Leaf f -> f x
        | Node tuples -> 
            let bindTuple = mapTuple applySubTree
            tuples |> List.map bindTuple |> Complex

    let rec resultToJson result =
        let tupleToJson (n, v) = "\"" + n + "\" : " + (resultToJson v) + ""
        match result with
            | Text c -> "\"" + c + "\""
            | Failure message -> "\"Failure: " + message + "\""
            | Number n -> n.ToString()
            | Amount a -> a.ToString()
            | Array arr -> "[ " + (String.concat ", " (List.map resultToJson arr)) + " ]"
            | Complex tuples -> "{ " + (String.concat ", " (List.map tupleToJson tuples)) + " }"

    let complexToJson l = l |> Complex |> resultToJson

    let rec treeToJson tree input =
        let subTreeToJson subtree = treeToJson subtree input
        // let leafToJson value = "\"" + (value.ToString()) + "\""
        // let eval = Microsoft.FSharp.Linq.RuntimeHelpers.LeafExpressionConverter.EvaluateQuotation
        match tree with
        | Const c -> c.ToString()
        | Leaf f -> f input |> resultToJson
        | Node branches -> 
            let branchToResult (n, st) = (n, applyTree st input) // "\"" + n + "\" : " + (subTreeToJson st)
            List.map branchToResult branches |> complexToJson

    //// helpers
    let toText x = x.ToString() |> Text

    let bindInput f input = (input, f input)
    let bindInputs f parent = f parent |> List.map (fun input -> (parent, input))

    let branchMany name path f = (name , path >> List.map f >> Array |> Leaf )
    let branch name path f = (name , path >> f |> Leaf )

    let bindResults f = List.map f >> Array
    let bindTrees f = bindResults (applyTree f)

