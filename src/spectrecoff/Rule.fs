﻿module SpectreCoff.Rule
open SpectreCoff.Layout
open SpectreCoff.Output

open Spectre.Console

let mutable defaultAlignment = Center

let emptyRule =
    Rule()
    :> Rendering.IRenderable
    |> Renderable

let alignedRule alignment content =
    let rule = 
        match alignment with
        | Left -> Rule(content).LeftAligned()
        | Center -> Rule(content)
        | Right -> Rule(content).RightAligned()

    rule
    :> Rendering.IRenderable
    |> Renderable

let rule =
    alignedRule defaultAlignment
    
