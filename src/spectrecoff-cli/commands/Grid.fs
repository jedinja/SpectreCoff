﻿namespace SpectreCoff.Cli.Commands

open Spectre.Console
open Spectre.Console.Cli
open SpectreCoff

type GridSettings()  =
    inherit CommandSettings()

type GridExample() =
    inherit Command<GridSettings>()
    interface ICommandLimiter<GridSettings>

    override _.Execute(_context, _settings) =
        let numbersGrid = grid [
            Numbers [1; 2]
            Strings ["One"; "Two"; "Three"]
        ]
        Many [
            C "The grid will have as many columns as are needed to accomodate the longest row:"
            numbersGrid.toOutputPayload
        ] |> toConsole

        (Numbers [3; 4; 5]) |> numbersGrid.addRow
        Many [
            C "Rows can later be added to an existing grid. Keep in mind that the number of elements per row must not exceed the number of columns:"
            numbersGrid.toOutputPayload
        ] |> toConsole

        let renderableGrid = grid [
            Payloads [numbersGrid.toOutputPayload; numbersGrid.toOutputPayload]
            Payloads [Emoji Emoji.Known.SmilingFace; Emoji Emoji.Known.SmilingFace]
        ]
        Many [
            C "Aside from strings and numbers, grids can also contain OutputPayloads:"
            renderableGrid.toOutputPayload
        ] |> toConsole
        0

open SpectreCoff.Cli.Documentation

type GridDocumentation() =
    inherit Command<GridSettings>()
    interface ICommandLimiter<GridSettings>

    override _.Execute(_context, _settings) =
        setDocumentationStyle
        Many [
            docSynopsis "Grid module" "This module provides functionality from the grid widget of Spectre.Console" "widgets/grid"
            BL
            C "The grid can be used by the grid function:"
            BI [
                P "grid: Row list -> Grid"
            ]
            BL
            C "Each row is a DU consisting in one of the following union types:"
            BI [
                Many [P "Payloads"; C "of"; P "OutputPayload list"]
                Many [P "Strings"; C "of"; P "string list"]
                Many [P "Numbers"; C "of"; P "int list"]
            ]
            BL
            C "The toOutputPayload() extension method on the"; P "Grid"; C "can be used to create a corresponding OutputPayload."
            C "Pipe it into toConsole to easily write the grid to the console:"
            BI [
                P "grid.toOutputPayload |> toConsole"
            ]
            BL
            C "The number of columns is automatically set to the number of elements in the longest row."
            C "Rows can be added later to the created grid, but the number of their elements must not exceed the number of columns of the grid."
        ] |> toConsole
        0