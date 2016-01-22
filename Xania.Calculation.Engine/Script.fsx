// Learn more about F# at http://fsharp.net. See the 'F# Tutorial' project
// for more guidance on F# programming.

#load "Library1.fs"
open Xania.Calculation.Engine

// Define your library scripting code here


type 'a Branch =
    | Branch of string * 'a
    // | BranchMany of string * 'a list

//type Tree = 
//    | Node of Branch<Tree> list
//    | Amount of decimal
//    | Number of int
//    | Text of string

type 'a Tree = 
    | Node of 'a Tree Branch list
    | List of 'a Tree list
    | Amount of ('a -> decimal)
    | Perc of ('a -> float)
    | Number of ('a -> int)
    | Text of ('a -> string)

let mapBranch (f:'a -> 'b) b =
    match b with
    | Branch(s, a) -> Branch(s, f a)
    // | BranchMany(s, a) -> BranchMany(s, List.map f a)

let mapList = List.map

let rec mapTree (f:'a -> 'b) tree = 
    let mapBranches = mapList << mapBranch << mapTree
    let mapTrees = mapList << mapTree
    match tree with
    | Text a -> Text (f >> a)
    | Amount a -> Amount (f >> a)
    | Perc a -> Perc (f >> a)
    | Number a -> Number (f >> a)
    | Node branches -> Node ( mapBranches f branches )
    | List trees -> List ( mapTrees f trees )

let mapTrees f = mapList (fun g -> mapTree (f g))

let toString x = x.ToString()
let add x y  = x + y
let add4 y = 4 + y
let duplicate (x:float) = x * 2.

type Person = { FirstName : string ; LastName : string; Age: int; Grades: int list; }

let personAge p = p.Age
let personGrades (p:Person) = p.Grades

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

let pers f p =  mapTrees f brownTree]

let mainTree = 
    Node [ 
        Branch ("blue", blueTree) 
        Branch ("brown", List ( (mapList << personGrades ) mapTree brownTree))
    ]

let rec execute a tree =
    match tree with
    | Text f -> f a |> toString
    | Amount f -> f a |> toString
    | Perc f -> f a |> toString
    | Number f -> f a |> toString
    | List l -> mapList (execute a) l |> toString
    | Node (Branch(n, st)::tail) -> n + "=" + (execute a st) + ";" + (execute a (Node tail))
    | Node [] -> ""

[<EntryPoint>]
let main argv = 
    printfn "%A" argv
    0 // return an integer exit code
