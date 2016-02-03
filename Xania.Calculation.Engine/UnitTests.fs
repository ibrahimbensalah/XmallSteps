namespace Xania.Calculation.Engine

    open NUnit.Framework    
    open Expression

    module UnitTests =

        [<TestFixture>]
        type MyClass() =
            [<Test>]
            member this.`` parsing constant value ``() =
                let expr = parseExpression "1"
                let result = Option.map (evalExpression id) expr
                printfn "result %A" result
                Assert.AreEqual (Some 1, result)

            [<Test>]
            member this.`` parsing + operator ``() =
                let expr = parseExpression "1+2"
                let result = Option.map (evalExpression id) expr
                printfn "result %A" result
                Assert.AreEqual (Some 3, result)