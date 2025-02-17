﻿namespace SpectreCoff.Cli.Commands

open Spectre.Console
open Spectre.Console.Cli
open SpectreCoff

type CanvasSettings() =
    inherit CommandSettings()

type CanvasExample() =
    inherit Command<CanvasSettings>()
    interface ICommandLimiter<CanvasSettings>

    override _.Execute(_context, _settings) =
        canvas (Width 12) (Height 12)
        |> withPixels (Rectangle (0,0,11,11)) Color.Yellow
        |> withPixels (Rectangle (2,2,4,3)) Color.Purple
        |> withPixels (Rectangle (7,2,9,3)) Color.Purple
        |> withPixels (ColumnSegment (ColumnIndex 8, StartIndex 4, EndIndex 7)) Color.Blue
        |> withPixels (RowSegment (RowIndex 9, StartIndex 3, EndIndex 8)) Color.Purple
        |> withPixels (MultiplePixels [(3,10); (8,10)]) Color.Purple
        |> toOutputPayload
        |> toConsole
        0

open SpectreCoff.Cli.Documentation

type CanvasDocumentation() =
    inherit Command<CanvasSettings>()
    interface ICommandLimiter<CanvasSettings>

    override _.Execute(_context, _settings) =
        setDocumentationStyle
        Many [
            docSynopsis "Canvas module" "This module provides functionality from the canvas widget of Spectre.Console" "widgets/canvas"
            docMissing
        ] |> toConsole
        0
