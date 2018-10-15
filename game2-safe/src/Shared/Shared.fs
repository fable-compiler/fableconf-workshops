namespace Shared

module Route =
    let builder typeName methodName =
        sprintf "/api/%s/%s" typeName methodName

type Score = string * int
type HighScores = Score list

type IGameApi =
    { getHighScores : unit -> Async<HighScores>
      submitHighScore : Score -> Async<HighScores> }
