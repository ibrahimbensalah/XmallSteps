﻿// Learn more about F# at http://fsharp.net. See the 'F# Tutorial' project
// for more guidance on F# programming.

// #r @"bin\Debug\Xania.Calculation.Designer.exe"
#load "Calc.fs"
open Xania.Calculation.Engine
open Tree

// let add x y  = x + y
let add4 y = 4 + y
let duplicate (x:float) = x * 2.

type Person = { FirstName : string ; Age: int; Grades: int list; } with
    static member personAge p = p.Age
    static member firstName p = p.FirstName
    static member personGrades p = p.Grades

type Organisation = { Name : string ; Employees: Person list; } with
    static member name o = o.Name
    static member employees o = o.Employees

let yellowTree = Leaf toText

let masterdata name = "MD(" + name + ")" |> Text

let simpleNode = 
    Node [
        ("branch 1", masterdata "CreditFee" |> Const )
    ]

let blueTree = 
    Node [
        // Branch ("leaf 1", add 1m >> yellowTree); 
        ("leaf 2", Person.personAge >> ((+) 4) >> Number |> Leaf ) 
        ("yellow", mapTree Person.personAge yellowTree ) 
    ]

let brownTree = 
    Node [
        ("leaf toString", Leaf toText)
        ("leaf +4", Leaf ((+) 4 >> Number))
        ("failure", Failure "some message" |> Const )
    ]

let mainTree = 
    Node [
        ("simple", simpleNode) 
        ("blue", blueTree) 
        ("brown list", Person.personGrades >> List.map (apply brownTree) >> Array |> Leaf )
    ]

let ibrahim = { FirstName = "ibrahim" ; Age = 35 ; Grades = [9 ; 8 ; 10] }
let xania = { Name = "Xania" ; Employees = [ ibrahim ; ibrahim ] }

// let branchMany l f input = List.map (fun i -> f (input, i) ) (l input) |> Array

let reportEmployee (parent, input) = 
    "Employee " + input.FirstName + " works at " + parent.Name |> Text

let orgTree = 
    Node [ 
        ("Name", Organisation.name >> Text |> Leaf )
        branch "Name2" id (Organisation.name >> Text)
        ("Employees", (bindInputs Organisation.employees) >> bindResults reportEmployee |> Leaf )
        branchMany "Employees2" (bindInputs Organisation.employees) reportEmployee
        ("Employee3", Organisation.employees >> bindTrees blueTree |> Leaf )
    ]

printfn "%s" (treeToJson mainTree ibrahim)

let f ((a, b), c) = b, (c, a)

(f ((1, 2), 3)).ToString() |> printfn "%s"  

//open System
// open Microsoft.FSharp.Compiler.SourceCodeServices
//
//    [<Test>]
//    let sometest () =
//        let result = parseExpression "x + 1"
//        Assert.AreEqual (1, 1)