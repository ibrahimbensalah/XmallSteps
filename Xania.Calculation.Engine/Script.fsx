// Learn more about F# at http://fsharp.net. See the 'F# Tutorial' project
// for more guidance on F# programming.

// #load "Library1.fs"
// open Xania.Calculation.Engine

// Define your library scripting code here

type 'a Branch =
    | Branch of string * 'a

type 'a Tree = 
    | Node of 'a Tree Branch list
    | List of ('a -> 'a Tree list)
    | List2 of 'a Tree list
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
    let mapTrees = mapList << mapTree
    match tree with
    | Text a -> Text (f >> a)
    | Amount a -> Amount (f >> a)
    | Perc a -> Perc (f >> a)
    | Number a -> Number (f >> a)
    | Node branches -> Node ( mapBranches f branches )
    | List2 trees -> 
        List2 ( mapTrees f trees )
    | List a -> 
        List (f >> a >> mapList (mapTree f))

let bindTree (tree:'a Tree) x:'a = mapTree (fun _ -> x) tree
let bindTrees tree = List.map (bindTree tree)
// let bindTree tree = List.map (fun x -> mapTree (fun _ -> x) tree)
let mapTrees g tree = g >> List.map (bindTree tree) 

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

// let mapTrees : f:('a -> 'b list) -> tree:'b Tree -> 'c = f

// let mapBrown f = mapTrees2 f [brownTree]

let mainTree = 
    Node [ 
        Branch ("blue", blueTree) 
        Branch ("brown", 
            // let f = List. brownTree
            List ( mapTrees personGrades brownTree ) )
        Branch ("brown2", 
            List ( personGrades >> bindTrees brownTree ) )
    ]

let rec execute a tree =
    match tree with
    | Text f -> f a |> toString
    | Amount f -> f a |> toString
    | Perc f -> f a |> toString
    | Number f -> f a |> toString
    | List2 l -> mapList (execute a) l |> toString
    | List l -> mapList (execute a) (l a) |> toString
    | Node (Branch(n, st)::tail) -> "[" + n + "]=" + (execute a st) + ";" + (execute a (Node tail))
    | Node [] -> ""

let ibrahim = { FirstName = "ibrahim" ; Age = 35 ; Grades = [9 ; 8 ; 10] }
