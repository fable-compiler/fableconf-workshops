namespace Shared

module Route =
    let builder typeName methodName =
        sprintf "/api/%s/%s" typeName methodName

type Score = string * int
type Scores = Score list

module HighScores =
    let limit = 10

type IGameApi =
    { getHighScores : unit -> Async<Scores>
      submitHighScore : Score -> Async<Scores> }
