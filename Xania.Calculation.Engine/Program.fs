namespace Xania.Calculation.Engine

open System.Windows.Forms
open Xania.Calculation.Designer
open Xania.Calculation.Designer.Components

module Program =

    let private eval = Microsoft.FSharp.Linq.RuntimeHelpers.LeafExpressionConverter.EvaluateQuotation
    type CalculationEngine () = 
        interface ICalculationEngine with
            member this.Run (comp: ITreeComponent) = 
                match comp with
                | :? LeafComponent as leaf -> 
                    let o = eval <@@ leaf.Fun @@>
                    o.ToString()
                | _ -> "TBD"

    // [<EntryPoint>]
    [<System.STAThread>]
    let main argv = 
        Application.EnableVisualStyles()
        Application.SetCompatibleTextRenderingDefault(false)
        let form = new FormCalculationEngine( CalculationEngine () )
        Application.Run(form)
        0 // return an integer exit code