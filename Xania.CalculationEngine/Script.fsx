// Learn more about F# at http://fsharp.net. See the 'F# Tutorial' project
// for more guidance on F# programming.

// #load "Library1.fs"
// open Xania.CalculationEngine

// Define your library scripting code here

type 'a Branch =
    | Branch of string * 'a

type 'a Tree = 
    | Node of 'a Tree Branch list
    // | List of 'a Tree list
    | List of ('a -> 'a Tree list)
    | Amount of ('a -> decimal)
    | Perc of ('a -> float)
    | Number of ('a -> int)
    | Text of ('a -> string)

let mapBranch (f:'a -> 'b) b =
    match b with
    | Branch(s, a) -> Branch(s, f a)
    // | BranchMany(s, a) -> BranchMany(s, List.map f a)

let mapList = List.map
let toString x = x.ToString()

let rec mapTree (f:'a -> 'b) tree = 
    let mapBranches = mapList << mapBranch << mapTree
    // let mapTrees = mapList << mapTree
    match tree with
    | Text a -> Text (f >> a)
    | Amount a -> Amount (f >> a)
    | Perc a -> Perc (f >> a)
    | Number a -> Number (f >> a)
    | Node branches -> Node ( mapBranches f branches )
    // | List trees -> List ( mapTrees f trees )
    | List a -> 
        let h = f >> a
        let g = f >> a >> mapList (mapTree f)
        List g

let add x y  = x + y
let add4 y = 4 + y
let duplicate (x:float) = x * 2.

type Person = { FirstName : string ; LastName : string; Age: int; Grades: int list; }

let personAge p = p.Age
let personGrades p = p.Grades

let yellowTree = Text toString

let blueTree = 
    Node [ 
        // Branch ("leaf 1", add 1m >> yellowTree); 
        Branch ("leaf 1", toString |> Text ) 
        Branch ("leaf 2", personAge >> add4 |> Number ) 
        Branch ("leaf 3", (fun p -> p.Age) >> add4 |> Number ) 
        Branch ("node 2", mapTree personAge yellowTree )
    ]

let brownTree = 
    Node [ 
        Branch ("leaf 3", Text toString) 
        Branch ("leaf 4", Number add4) 
    ]

let holaTree f l = List.map l f

// let collectTree : f:('a -> 'b list) -> tree:'b Tree -> 'c = f

// let mapBrown f = mapTrees2 f [brownTree]

let mainTree = 
    Node [ 
        Branch ("blue", blueTree) 
        // Branch ("brown", List ( (mapList << personGrades ) mapTree brownTree))
    ]

let rec execute a tree =
    match tree with
    | Text f -> f a |> toString
    | Amount f -> f a |> toString
    | Perc f -> f a |> toString
    | Number f -> f a |> toString
    // | List l -> mapList (execute a) l |> toString
    | List l -> mapList (execute a) (l a) |> toString
    | Node (Branch(n, st)::tail) -> n + "=" + (execute a st) + ";" + (execute a (Node tail))
    | Node [] -> ""

// let Tree a = Leaf f