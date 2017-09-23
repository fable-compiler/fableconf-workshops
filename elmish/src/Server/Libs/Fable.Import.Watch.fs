namespace Fable.Import

open System
open System.Text.RegularExpressions
open Fable.Core
open Fable.Import.JS
open Fable.Import.Node

module Watch =

    type FileOrFiles = U2<Fs.Stats, obj>

    and [<StringEnum>] EventType =
        | Created
        | Removed

    and [<AllowNullLiteral>] Monitor =
        inherit Events.EventEmitter
        abstract files: obj with get, set
        abstract on: ``event``: EventType * listener: Func<FileOrFiles, Fs.Stats, unit> -> obj
        [<Emit("$0.on('created', $1...")>] abstract on_created: ``event``: Func<FileOrFiles, Fs.Stats, unit> -> obj
        [<Emit("$0.on('removed', $1...")>] abstract on_removed: ``event``: Func<FileOrFiles, Fs.Stats, unit> -> obj
        [<Emit("$0.on('changed',$1...)")>] abstract on_changed: listener: Func<FileOrFiles, Fs.Stats, Fs.Stats, unit> -> obj
        abstract on: ``event``: U2<string, Symbol> * listener: Func<obj, unit> -> obj
        abstract stop: unit -> unit

    and [<AllowNullLiteral>] Options =
        abstract ignoreDotFiles: bool option with get, set
        abstract interval: float option with get, set
        abstract ignoreUnreadableDir: bool option with get, set
        abstract ignoreNotPermitted: bool option with get, set
        abstract ignoreDirectoryPattern: bool option with get, set
        abstract filter: (string -> Fs.Stats -> bool) option with get, set

    [<Import("*", "watch")>]
    type Exports =
        class end
        static member watchTree(root: string, cb: FileOrFiles -> Fs.Stats -> Fs.Stats -> unit): unit = jsNative
        static member watchTree(root: string, options: Options, cb: FileOrFiles -> Fs.Stats -> Fs.Stats -> unit): unit = jsNative
        static member unwatchTree(root: string) : unit = jsNative
        static member createMonitor(root: string, cb: Monitor -> unit): unit = jsNative
        static member createMonitor(root: string, options: Options, cb: Monitor -> unit): unit = jsNative
