module Dashboard.Types

type Model =
    { Messages : string list }

type Msg =
| ChangeStr of string
