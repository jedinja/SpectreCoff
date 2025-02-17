﻿[<AutoOpen>]
module SpectreCoff.LiveDisplay

open System.Threading.Tasks
open Spectre.Console

type Configuration =
    { AutoClear: bool
      Overflow: VerticalOverflow Option
      Cropping: VerticalOverflowCropping Option }

let defaultConfiguration =
    { AutoClear = true
      Overflow = None
      Cropping = None }

let start renderable (operation: LiveDisplayContext -> Task<unit>) =
    task { return! AnsiConsole.Live(renderable).StartAsync(operation) }

let startWithCustomConfig config renderable (operation: LiveDisplayContext -> Task<unit>) =
    task {
        let live = AnsiConsole.Live(renderable)
        live.AutoClear <- config.AutoClear
        live.Overflow <-
            match config.Overflow with
            | Some overflow -> overflow
            | None -> live.Overflow
        live.Cropping <-
            match config.Cropping with
            | Some cropping -> cropping
            | None -> live.Cropping
        return! live.StartAsync(operation)
    }