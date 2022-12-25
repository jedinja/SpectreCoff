namespace SpectreCoff.Cli.Commands

open Spectre.Console.Cli
open SpectreCoff

type CalendarSettings()  =
    inherit CommandSettings()

type CalendarExample() =
    inherit Command<CalendarSettings>()
    interface ICommandLimiter<CalendarSettings>

    override _.Execute(_context, _) =
        0

type CalendarDocumentation() =
    inherit Command<CalendarSettings>()
    interface ICommandLimiter<CalendarSettings>

    override _.Execute(_context, _) =
        Cli.Theme.setDocumentationStyle
        Edgy "Sorry, this documentation is not available yet." |> toConsole    
        0