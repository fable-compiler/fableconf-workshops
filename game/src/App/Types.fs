module App.Types

type IBox =
    abstract X: float
    abstract Y: float
    abstract Angle: float
    abstract Width: float
    abstract Height: float

type IModel =
    abstract Boxes: IBox seq
    abstract Initialized: bool

type Msg =
    | Physics of float[]
