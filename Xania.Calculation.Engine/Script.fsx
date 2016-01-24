// Learn more about F# at http://fsharp.net. See the 'F# Tutorial' project
// for more guidance on F# programming.

// #load "Library1.fs"
// open Xania.Calculation.Engine

// Define your library scripting code here

type 'a Branch =
    | Branch of string * 'a

type 'a Tree = 
    | Leaf of ('a -> obj)
    | Text of ('a -> string)
    | Amount of ('a -> decimal)
    | Perc of ('a -> float)
    | Number of ('a -> int)
    | Node of 'a Tree Branch list
    | List of ('a -> 'a Tree list)

let mapBranch (f:'a -> 'b) b =
    match b with
    | Branch(s, t) -> Branch(s, f t)

let mapList = List.map
let toString x = x.ToString()

let rec mapTree (f:'a -> 'b) tree = 
    let mapBranches = mapList << mapBranch << mapTree
    let mapTrees = mapList << mapTree
    match tree with
    | Leaf a -> Leaf (f >> a)
    | Text a -> Text (f >> a)
    | Amount a -> Amount (f >> a)
    | Perc a -> Perc (f >> a)
    | Number a -> Number (f >> a)
    | Node branches -> Node ( mapBranches f branches )
    | List a -> List (f >> a >> mapList (mapTree f))

let bindTree tree x = mapTree (fun _ -> x) tree
let bindTrees tree = List.map (bindTree tree)
let treeList path tree = List ( path >> bindTrees tree )
let rec treeToJson tree input =
    let subTreeToJson subtree = treeToJson subtree input
    match tree with
    | Leaf f -> f input |> toString
    | Text f -> f input |> toString
    | Amount f -> f input |> toString
    | Perc f -> f input |> toString
    | Number f -> f input |> toString
    | Node branches -> 
        let executeBranche (Branch(n, st)) = "'" + n + "':" + (subTreeToJson st)
        "{ " + (mapList executeBranche branches |> String.concat ", ") + " }"
    | List l -> 
        "[ " + (mapList subTreeToJson (l input) |> String.concat ", ") + " ]"

////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////

let add x y  = x + y
let add4 y = 4 + y
let duplicate (x:float) = x * 2.

type Person = { FirstName : string ; Age: int; Grades: int list; }

let personAge p = p.Age
let personGrades p = p.Grades

let yellowTree = Text toString

let blueTree = 
    Node [ 
        // Branch ("leaf 1", add 1m >> yellowTree); 
        Branch ("leaf 2", personAge >> add4 |> Number ) 
        Branch ("leaf 3", (fun p -> p.Age) >> add4 |> Number ) 
        Branch ("yellow", mapTree personAge yellowTree ) 
    ]

let brownTree = 
    Node [ 
        Branch ("leaf 3", Text toString) 
        Branch ("leaf 4", Number add4) 
    ]

let mainTree = 
    Node [ 
        Branch ("blue", blueTree) 
        Branch ("brown", treeList personGrades brownTree )
    ]

let ibrahim = { FirstName = "ibrahim" ; Age = 35 ; Grades = [9 ; 8 ; 10] }

let eval = Microsoft.FSharp.Linq.RuntimeHelpers.LeafExpressionConverter.EvaluateQuotation
