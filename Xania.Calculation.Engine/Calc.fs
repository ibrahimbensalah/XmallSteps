namespace Xania.Calculation.Engine

module Tree =

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
        let mapTuples = mapTree >> mapTuple >> List.map
        match tree with
        | Const c -> Const c
        | Leaf a -> Leaf (f >> a)
        | Node tuples -> Node ( mapTuples f tuples )

    let rec apply tree x =
        let applySub tree = apply tree x
        match tree with
        | Const c -> c
        | Leaf f -> f x
        | Node tuples -> List.map (mapTuple applySub) tuples |> Complex

    let rec resultToJson result =
        let tupleToJson (n, v) = "\"" + n + "\" : " + (resultToJson v) + ""
        match result with
            | Text c -> "\"" + c + "\""
            | Failure message -> "\"Failure: " + message + "\""
            | Number n -> n.ToString()
            | Amount a -> a.ToString()
            | Array arr -> "[ " + (String.concat ", " (List.map resultToJson arr)) + " ]"
            | Complex tuples -> "{ " + (String.concat ", " (List.map tupleToJson tuples)) + " }"

    let treeToJson tree input = apply tree input |> resultToJson

    //// helpers
    let toText x = x.ToString() |> Text

    let bindInput f input = (input, f input)
    let bindInputs f parent = f parent |> List.map (fun input -> (parent, input))

    let branchMany name path f = (name , path >> List.map f >> Array |> Leaf )
    let branch name path f = (name , path >> f |> Leaf )

    let bindResults f = List.map f >> Array
    let bindTrees f = bindResults (apply f)

module Designer =

    open System.Windows.Forms
    open Xania.Calculation.Designer
    open Xania.Calculation.Designer.Components

    let private eval = Microsoft.FSharp.Linq.RuntimeHelpers.LeafExpressionConverter.EvaluateQuotation
    type CalculationEngine () = 
        interface ICalculationEngine with
            member this.Run (comp: ITreeComponent) = 
                match comp with
                | :? LeafComponent as leaf -> 
                    let o = eval <@@ leaf.Fun @@>
                    o.ToString()
                | _ -> "TBD"

    [<EntryPoint>]
    [<System.STAThread>]
    let main argv = 
        Application.EnableVisualStyles()
        Application.SetCompatibleTextRenderingDefault(false)
        let form = new FormCalculationEngine( CalculationEngine () )
        form
        Application.Run(form)
        0 // return an integer exit code