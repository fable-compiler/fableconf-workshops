namespace Fable.Import
open System
open System.Text.RegularExpressions
open Fable.Core
open Fable.Import.JS
open Fable.Core.JsInterop

module morgan =

    type [<AllowNullLiteral>] FormatFn =
        inherit Function
        [<Emit("$0($1...)")>] abstract Invoke: tokens: TokenIndexer * req: express.Request * res: express.Response -> string

    and [<AllowNullLiteral>] TokenCallbackFn =
        inherit Function
        [<Emit("$0($1...)")>] abstract Invoke: req: express.Request * res: express.Response * ?arg: U3<string, float, bool> -> string

    and [<AllowNullLiteral>] TokenIndexer =
        [<Emit("$0[$1]{{=$2}}")>] abstract Item: tokenName: string -> TokenCallbackFn with get, set

    and [<AllowNullLiteral>] Morgan =
        [<Emit("$0($1...)")>] abstract Invoke: format: FormatEnum * ?options: Options -> express.RequestHandler
        [<Emit("$0($1...)")>] abstract Invoke: format: FormatFn * ?options: Options -> express.RequestHandler
        abstract token: name: string * callback: TokenCallbackFn -> Morgan
        abstract format: name: string * fmt: string -> Morgan
        abstract format: name: string * fmt: FormatFn -> Morgan
        abstract compile: format: string -> FormatFn

    and [<AllowNullLiteral>] StreamOptions =
        abstract write: Func<string, unit> with get, set

    and [<AllowNullLiteral>] Options =
        abstract buffer: bool option with get, set
        abstract immediate: bool option with get, set
        abstract skip: Func<express.Request, express.Response, bool> option with get, set
        abstract stream: StreamOptions option with get, set

    and [<StringEnum>] FormatEnum =
        | [<CompiledName("combined")>] Combined
        | [<CompiledName("common")>] Common
        | [<CompiledName("dev")>] Dev
        | [<CompiledName("short")>] Short
        | [<CompiledName("tiny")>] Tiny
        | [<Erase>] Custom of string

    type [<Import("*","morgan")>] Globals =
        static member token(name: string, callback: TokenCallbackFn): Morgan = jsNative
        static member format(name: FormatEnum, fmt: string): Morgan = jsNative
        static member format(name: FormatEnum, fmt: FormatFn): Morgan = jsNative
        static member compile(format: string): FormatFn = jsNative

    module Exports =

        [<Import("*", "morgan")>]
        let Morgan: Morgan = jsNative
