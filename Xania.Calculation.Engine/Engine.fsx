// Learn more about F# at http://fsharp.net. See the 'F# Tutorial' project
// for more guidance on F# programming.

// #load "Library1.fs"
// open Xania.Calculation.Engine

// Define your library scripting code here

type Result =
    | Amount of decimal
    | Number of int
    | Text of string
    | Array of Result list
    | Object of (string * Result) list
    | Failure of string

type 'a Tree = 
    | Const of Result
    | Leaf of ('a -> Result)
    | Node of ( string * 'a Tree ) list

let mapTuple f (s, t) = (s, f t)

let toString x = x.ToString() |> Text

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
        tuples |> List.map bindTuple |> Object

let rec resultToJson result =
    let tupleToJson (n, v) = "\"" + n + "\" : " + (resultToJson v) + ""
    match result with
        | Text c -> "\"" + c + "\""
        | Failure message -> "\"Failure: " + message + "\""
        | Number n -> n.ToString()
        | Amount a -> a.ToString()
        | Array arr -> "[ " + (String.concat ", " (List.map resultToJson arr)) + " ]"
        | Object tuples -> "{ " + (String.concat ", " (List.map tupleToJson tuples)) + " }"

let rec treeToJson tree input =
    let subTreeToJson subtree = treeToJson subtree input
    // let leafToJson value = "\"" + (value.ToString()) + "\""
    // let eval = Microsoft.FSharp.Linq.RuntimeHelpers.LeafExpressionConverter.EvaluateQuotation
    match tree with
    | Const c -> c.ToString()
    | Leaf f -> f input |> resultToJson
    | Node branches -> 
        let branchToResult (n, st) = (n, applyTree st input) // "\"" + n + "\" : " + (subTreeToJson st)
        List.map branchToResult branches |> Object |> resultToJson
        // "{ " + (List.map executeBranche branches |> String.concat ", ") + " }"
    // | Expr expr -> expr input |> eval |> leafToJson

////////////////////////////////////////////////////////////////////////////////////////////
////// DOMAIN //////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////

// let add x y  = x + y
let add4 y = 4 + y
let duplicate (x:float) = x * 2.

type Person = { FirstName : string ; Age: int; Grades: int list; }

let personAge p = p.Age
let personGrades p = p.Grades
let lookupFun db y = 1

// let toObject x = x :> obj
let yellowTree = Leaf toString


let masterdata name = "MD(" + name + ")"

let simpleNode = 
    Node [
        ("branch 1", masterdata "CreditFee" |> Text |> Const )
    ]

let blueTree = 
    Node [ 
        // Branch ("leaf 1", add 1m >> yellowTree); 
        ("leaf 2", personAge >> ((+) 4) >> Number |> Leaf ) 
        ("yellow", mapTree personAge yellowTree ) 
    ]

let brownTree = 
    Node [ 
        ("leaf toString", Leaf toString)
        ("leaf +4", Leaf ((+) 4 >> Number))
        ("failure", Failure "some message" |> Const )
    ]

let mainTree = 
    Node [ 
        ("simple", simpleNode) 
        ("blue", blueTree) 
        ("brown list", personGrades >> List.map (applyTree brownTree) >> Array |> Leaf )
    ]

let ibrahim = { FirstName = "ibrahim" ; Age = 35 ; Grades = [9 ; 8 ; 10] }

