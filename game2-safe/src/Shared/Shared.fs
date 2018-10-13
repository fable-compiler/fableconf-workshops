namespace Shared

module Route =
    let builder typeName methodName =
        sprintf "/api/%s/%s" typeName methodName

type IGameApi =
    { getHighScores : unit -> Async<(string * int) list>
      submitHighScore : string * int -> Async<(string * int) list> }
