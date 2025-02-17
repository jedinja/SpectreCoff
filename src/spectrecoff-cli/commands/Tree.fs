namespace SpectreCoff.Cli.Commands

open Spectre.Console
open Spectre.Console.Cli
open SpectreCoff

type TreeSettings()  =
    inherit CommandSettings()

type TreeExample() =
    inherit Command<TreeSettings>()
    interface ICommandLimiter<TreeSettings>

    override _.Execute(_context, _) =

        let subNode value = 
            node (panel "" (P value)) []

        let nodes = 
            [ for i in 1 .. 16 -> (i, node (C $"{i}") []) ] 
            |> List.map (fun (index, node) -> 
                match index with
                | i when i % 15 = 0 -> attach [subNode "FizzBuzz"] node
                | i when i % 5 = 0 -> attach [subNode "Buzz"] node
                | i when i % 3 = 0 -> attach [subNode "Fizz"] node
                | _ -> node)
        
        tree (P "FizzBuzz-Tree!") nodes 
        |> toConsole

        customTree 
            { defaultTreeLayout with Look = { Color = Some Color.Green; BackgroundColor = Some Color.Grey; Decorations = [Decoration.Bold] } }
            (P "Custom-Tree!") 
            [ for i in 1 .. 3 -> node (C $"Node {i}!") [] ] 
        |> toConsole
        0

open SpectreCoff.Cli.Documentation

type TreeDocumentation() =
    inherit Command<TreeSettings>()
    interface ICommandLimiter<TreeSettings>

    override _.Execute(_context, _) =
        setDocumentationStyle
        Many[
            docSynopsis "Tree module" "This module provides functionality from the tree widget of Spectre.Console" "widgets/tree"
            docMissing
        ] |> toConsole
        0
