namespace Poncho.Cli.Commands

module EmptyJournal =
    open FsToolkit.ErrorHandling
    open Poncho.Domain
    open Poncho.Local
    open Spectre.Console.Cli
    open System
    open System.ComponentModel
    open Poncho.Cli.Console.Format
    open Spectre.Console

    type Settings(dir: string Option) =
        inherit CommandSettings()

        [<Description("directory")>]
        [<CommandOption("-d|--dir")>]
        member val dir: string Option = dir

    type Handler() =
        inherit Command<Settings>()

        override _.Execute(_, settings) =
            let dir =
                settings.dir
                |> Option.bind (fun providedDir -> if providedDir.Length = 0 then None else Some(providedDir))
                |> Option.defaultWith (fun () -> Environment.CurrentDirectory)

            let journal = Journal.emptyJournal |> Journal.initialize <| { doingsPerDay = 3 }

            LocalJournal.saveJournal journal dir
            |> Result.map (fun _ -> 0)
            |> Result.mapError (fun failure -> AnsiConsole.Markup(formatError failure))
            |> Result.defaultWith (fun _ -> 1)
