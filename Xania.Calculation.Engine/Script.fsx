// Learn more about F# at http://fsharp.net. See the 'F# Tutorial' project
// for more guidance on F# programming.

// #load "Library1.fs"
// open Xania.Calculation.Engine

// Define your library scripting code here

type 'a Branch =
    | Branch of string * 'a

type 'a Tree = 
    | Text of ('a -> string)
    | Amount of ('a -> decimal)
    | Perc of ('a -> float)
    | Number of ('a -> int)
    | Node of 'a Tree Branch list
    | List of ('a -> 'a Tree list)
    | Const of obj

let mapBranch (f:'a -> 'b) b =
    match b with
    | Branch(s, t) -> Branch(s, f t)

let mapList = List.map
let toString x = x.ToString()

let toText x = x |> Text

let rec mapTree (f:'a -> 'b) tree = 
    let mapBranches = mapList << mapBranch << mapTree
    let mapTrees = mapList << mapTree
    match tree with
    | Const o -> Const o
    | Text a -> Text (f >> a)
    | Amount a -> Amount (f >> a)
    | Perc a -> Perc (f >> a)
    | Number a -> Number (f >> a)
    | Node branches -> Node ( mapBranches f branches )
    | List a -> List (f >> a >> mapList (mapTree f))

let bindTree tree x = mapTree (fun _ -> x) tree
let bindTrees tree = List.map (bindTree tree)
let mapTrees path tree = List ( path >> bindTrees tree )
let rec treeToJson tree input =
    let subTreeToJson subtree = treeToJson subtree input
    let leafToJson value = "\"" + (value.ToString()) + "\""
    // let eval = Microsoft.FSharp.Linq.RuntimeHelpers.LeafExpressionConverter.EvaluateQuotation
    match tree with
    | Const o -> o |> leafToJson
    | Text f -> f input |> leafToJson
    | Amount f -> f input |> leafToJson
    | Perc f -> f input |> leafToJson
    | Number f -> f input |> leafToJson
    | Node branches -> 
        let executeBranche (Branch(n, st)) = "\"" + n + "\" : " + (subTreeToJson st)
        "{ " + (mapList executeBranche branches |> String.concat ", ") + " }"
    | List l -> 
        "[ " + (mapList subTreeToJson (l input) |> String.concat ", ") + " ]"
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
let yellowTree = Text toString

let masterdata name = "MD(" + name + ")" :> obj |> Const

let simpleNode = 
    Node [
        Branch ("branch 1", masterdata "CreditFee" )
    ]

let blueTree = 
    Node [ 
        // Branch ("leaf 1", add 1m >> yellowTree); 
        Branch ("leaf 2", personAge >> ((+) 4) |> Number ) 
        Branch ("yellow", mapTree personAge yellowTree ) 
    ]

let brownTree = 
    Node [ 
        Branch ("leaf 3", Text toString)
        Branch ("leaf 4", Number ((+) 4))
        Branch ("leaf *2", Number ((*) 2))
    ]

let mainTree = 
    Node [ 
        Branch ("simple", simpleNode) 
        Branch ("blue", blueTree) 
        Branch ("brown list", mapTrees personGrades brownTree )
    ]

let ibrahim = { FirstName = "ibrahim" ; Age = 35 ; Grades = [9 ; 8 ; 10] }


