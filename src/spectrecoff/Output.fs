[<AutoOpen>]
module SpectreCoff.Output

open Spectre.Console.Rendering
open SpectreCoff.Layout
open Spectre.Console
open System

// Styles
let mutable calmStyle = None
let mutable calmColor = Color.SteelBlue

let mutable pumpedStyle = Italic
let mutable pumpedColor = Color.DeepSkyBlue3_1

let mutable edgyStyle = Bold
let mutable edgyColor = Color.DarkTurquoise

let mutable linkColor = pumpedColor

// Special strings
let mutable bulletItemPrefix = " + "

// Basic output
let private markupWithColor color content =
    $"[{color}]{Markup.Escape content}[/]"

let private markupWithStyle style content =
    $"[{style}]{Markup.Escape content}[/]"

let private markupWithColorAndStyle color (style: string) content =
    $"[{color} {style}]{Markup.Escape content}[/]"

let private padEmoji (emoji: string) =
    match emoji.StartsWith ":" with
    | true -> emoji
    | false -> $":{emoji}:"

let markupString (color: Color option) (style: Layout.Style option) content =
    match style with
    | None ->
        match color with
        | None -> content
        | Some c -> markupWithColor c content
    | Some s ->
        match color with
        | None -> markupWithStyle s content
        | Some c -> markupWithColorAndStyle c (stringifyStyle s) content

let pumped content = markupString (Some pumpedColor) (Some pumpedStyle) content
let edgy content = markupString (Some edgyColor) (Some edgyStyle) content
let calm content = markupString (Some calmColor) calmStyle content

let printMarkedUpInline content = AnsiConsole.Markup $"{content}"
let printMarkedUp content = AnsiConsole.Markup $"{content}{Environment.NewLine}"

let rec private joinSeparatedBy (separator: string) (strings: string list) =
    match strings with
    | [] -> ""
    | [s] -> s
    | head::tail -> head + separator + joinSeparatedBy separator tail

let rec private joinSeparatedByNewline =
    joinSeparatedBy Environment.NewLine

let appendNewline content =
    content + Environment.NewLine

type OutputPayload =
    | NewLine
    | MarkupCS of Color * Layout.Style * string
    | MarkupC of Color * string
    | MarkupS of Layout.Style * string
    | Link of string
    | LinkWithLabel of string*string
    | Emoji of string
    | Calm of string
    | Pumped of string
    | Edgy of string
    | Vanilla of string
    | BulletItems of OutputPayload list
    | Many of OutputPayload list
    | Renderable of IRenderable

// Short aliases
let MCS = MarkupCS
let MC = MarkupC
let MS = MarkupS
let C = Calm
let P = Pumped
let E = Edgy
let V = Vanilla
let BI = BulletItems
let NL = NewLine

let toOutputPayload value =
    value
    :> IRenderable
    |> Renderable

let rec toMarkedUpString (payload: OutputPayload) =
    match payload with
    | Calm content -> content |> calm
    | Pumped content -> content |> pumped
    | Edgy content -> content |> edgy
    | Vanilla content -> content
    | MarkupCS (color, style, content) -> content |> markupString (Some color) (Some style)
    | MarkupC (color, content) -> content |> markupString (Some color) None
    | MarkupS (style, content) -> content |> markupString None (Some style)
    | Link link -> link |> markupWithColorAndStyle linkColor "link"
    | LinkWithLabel (label, link) -> label |> markupWithColorAndStyle linkColor $"link={link}"
    | Emoji emoji -> emoji |> padEmoji
    | NewLine -> Environment.NewLine
    | BulletItems items ->
        items
        |> List.map (fun item ->
            match item with
            | Renderable _ -> failwith "Renderables can't be used within bullet items."
            | BulletItems _ -> failwith "Bullet items can't be used within bullet items."
            | _ -> Many [C bulletItemPrefix; item])
        |> List.map toMarkedUpString
        |> joinSeparatedByNewline
    | Many payloads ->
        payloads
        |> List.map toMarkedUpString
        |> joinSeparatedBy " "
    | Renderable _ -> failwith "The payload type 'Renderable' is not stringifyable."

let stringifyable payload =
    match payload with
    | Vanilla _
    | Calm _
    | Pumped _
    | Edgy _
    | Emoji _
    | Link _
    | LinkWithLabel _
    | MarkupC _
    | MarkupS _
    | MarkupCS _ -> true
    | _ -> false

let combineStringifyables item1 item2 =
    Vanilla (item1 + " " + item2)

let rec reduceRenderables (items: OutputPayload list) =
    match items with
    | head :: tail ->
        match tail with
        | head2 :: tail2 ->
            match (stringifyable head, stringifyable head2) with
            | true, true -> reduceRenderables([combineStringifyables (toMarkedUpString head) (toMarkedUpString head2)]@tail2)
            | false, true -> [head]@(reduceRenderables tail)
            | _ -> [head; head2]@(reduceRenderables tail2)
        | _ -> [head]
    | _ -> []

let rec payloadToRenderable (payload: OutputPayload) =
    match payload with
    | Renderable renderable -> renderable
    | Many payloads ->
        payloads
        |> reduceRenderables
        |> List.map payloadToRenderable
        |> Rows
        :> IRenderable
    | _ ->
        payload
        |> toMarkedUpString
        |> Markup
        :> IRenderable

let toConsole (payload: OutputPayload) =
    payload
    |> payloadToRenderable
    |> AnsiConsole.Write
    AnsiConsole.WriteLine ""

type OutputPayload with
    member self.toMarkedUpString = toMarkedUpString self
    member self.toRenderable = payloadToRenderable self
    member self.toConsole = toConsole self
