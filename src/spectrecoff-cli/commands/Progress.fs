namespace SpectreCoff.Cli.Commands

open System.Threading.Tasks
open Spectre.Console
open Spectre.Console.Cli
open SpectreCoff

type ProgressSettings()  =
    inherit CommandSettings()

type ProgressExample() =
    inherit Command<ProgressSettings>()
    interface ICommandLimiter<ProgressSettings>

    override _.Execute(_context, _settings) =
        let rec operation (context: ProgressContext) =
            task {
                let task1 =
                    "Turtle"
                    |> HotPercentageTask
                    |> realizeIn context
                let task2 =
                    (60.0, "Rabbit")
                    |> ColdCustomTask
                    |> realizeIn context
                while not context.IsFinished do
                    task1 |> incrementBy 5 |> ignore
                    if task1.Value > 50 then
                        task2.StartTask()
                    if task2.IsStarted then
                        task2 |> incrementBy 7 |> ignore
                    do! Task.Delay(300)
            }
        let template =
            emptyTemplate
            |> withDescriptionColumn
            |> withSpinnerColumn
            |> withRemainingTimeColumn
            |> withProgressBarColumn
        (operation |> Progress.startCustom template).Wait()
        0

type ProgressDocumentation() =
    inherit Command<ProgressSettings>()
    interface ICommandLimiter<ProgressSettings>

    override _.Execute(_context, _settings) =
        Cli.Theme.setDocumentationStyle

        BL |> toConsole
        pumped "Progress module"
        |> alignedRule Left
        |> toConsole

        Many [
            C "This module provides functionality from the progress widget of Spectre.Console ("
            Link "https://spectreconsole.net/widgets/progress"
            C ")"
        ] |> toConsole
        0