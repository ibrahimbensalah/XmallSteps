// Learn more about F# at http://fsharp.net. See the 'F# Tutorial' project
// for more guidance on F# programming.

// #load "Library1.fs"
// open Xania.CalculationEngine

// Define your library scripting code here

type 'a Branch =
    | Branch of string * 'a

//type Tree = 
//    | Node of Branch<Tree> list
//    | Amount of decimal
//    | Number of int
//    | Text of string

type 'a Tree = 
    | Node of 'a Tree Branch list
    | Amount of ('a -> decimal)
    | Perc of ('a -> float)
    | Number of ('a -> int)
    | Text of ('a -> string)

let mapBranch (f:'a -> 'b) b =
    match b with
    | Branch(s,a) -> Branch(s, f a)

let rec map (f:'a -> 'b) tree = 
    match tree with
    | Text a -> Text (f >> a)
    | Amount a -> Amount (f >> a)
    | Perc a -> Perc (f >> a)
    | Number a -> Number (f >> a)
    | Node branches -> Node (branches |> List.map (mapBranch (map f)))

let toString x = x.ToString()
let add x y  = x + y
let add4 y = 4 + y
let duplicate (x:float) = x * 2.

type Person = { FirstName : string ; LastName : string; Age: int; BirthDate: System.DateTime; }

let personAge p = p.Age

let yellowTree = Text toString

let blueTree = 
    Node [ 
        // Branch ("leaf 1", add 1m >> yellowTree); 
        Branch ("leaf 1", toString |> Text ) 
        Branch ("leaf 2", personAge >> add4 |> Number ) 
        Branch ("node 2", map personAge yellowTree )
    ]

let brownTree = 
    Node [ 
        Branch ("leaf 3", Text toString) 
        Branch ("leaf 4", Number add4) 
    ]
let mainTree = 
    Node [ 
        Branch ("blue", blueTree) 
        Branch("brown", map personAge brownTree) 
    ]

let a = mainTree

// let Tree a = Leaf f
