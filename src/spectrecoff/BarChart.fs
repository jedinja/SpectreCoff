﻿module SpectreCoff.BarChart

open Spectre.Console
open SpectreCoff.Layout

let mutable width = 60
let mutable alignment = Alignment.Center

let mutable colors = [
    Color.Blue
    Color.Green
    Color.Red
    Color.Yellow
    Color.Aqua
    Color.Lime
]

type ChartItem =
    | ChartItem of string * float
    | ChartItemWithColor of string * float * Color

let createBasicChart label =
    let chart = BarChart()
    chart.Width <- width
    chart.Label <- label

    chart.LabelAlignment <-
        match alignment with
        | Left -> Justify.Left
        | Center -> Justify.Center
        | Right -> Justify.Right

    chart

let barChart label (items: ChartItem list) =
    let chart = createBasicChart label
    items
    |> List.indexed
    |> List.map (fun item ->
        match snd item with
        | ChartItem (label, value) -> BarChartItem(label, value, colors[fst item % colors.Length])
        | ChartItemWithColor (label, value, color) -> BarChartItem(label, value, color))
    |> List.iter (fun item -> chart.AddItem(item) |> ignore)

    chart

let toConsole (barChart: BarChart) =
    barChart |> AnsiConsole.Write
