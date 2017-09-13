module App.View

open Shared
open State
open Fable.Core
open Fable.Core.JsInterop
open Fable.Import.Browser

type CanvasInfo =
    { Context: CanvasRenderingContext2D
      Zoom: float
      Width: float
      Height: float }

let initCanvas() =
    let canvas = document.getElementsByTagName_canvas().[0]
    canvas.width <- window.innerWidth  * window.devicePixelRatio
    canvas.height <- window.innerHeight * window.devicePixelRatio
    let zoom =
        let zoom = canvas.height / Init.spaceHeight
        if canvas.width / Init.spaceWidth < zoom
        then canvas.width / Init.spaceWidth
        else zoom
    let ctx = canvas.getContext_2d()
    ctx.lineWidth <- 2. / zoom
    ctx.fillStyle <- !^Init.fillStyle
    ctx.strokeStyle <- !^Init.strokeStyle
    { Context = ctx
      Zoom = zoom
      Width = canvas.width
      Height = canvas.height }

let drawShip (ctx: CanvasRenderingContext2D) (model: Model) =
    // if not hideShip then
    let x = model.Ship.X
    let y = model.Ship.Y
    let angle = model.Ship.Angle
    let radius = Init.shipSize
    ctx.save()
    // Translate to the ship center
    ctx.translate(x, y)
    // Rotate to ship orientation
    ctx.rotate(angle)
    ctx.beginPath()
    ctx.moveTo(-radius * 0.6, -radius)
    ctx.lineTo(0., radius)
    ctx.lineTo(radius * 0.6, -radius)
    ctx.moveTo(-radius * 0.5, -radius * 0.5)
    ctx.lineTo(radius * 0.5, -radius * 0.5)
    ctx.closePath()
    ctx.stroke()
    ctx.restore()
    
let drawAsteroids (ctx: CanvasRenderingContext2D) (model: Model) =
    let radius = Init.calculateRadius(model.Level)
    for asteroid in model.Asteroids do
        ctx.save()
        ctx.translate(asteroid.X, asteroid.Y)  // Translate to the center
        ctx.rotate(asteroid.Angle)
        ctx.beginPath()
        for j = 0 to asteroid.Vertices.Length - 1 do
            let v = asteroid.Vertices.[j]
            if j = 0 then
                ctx.moveTo(fst v, snd v)
            else
                ctx.lineTo(fst v, snd v)
        ctx.closePath()
        ctx.stroke()
        ctx.restore()
    
let drawBounds (ctx: CanvasRenderingContext2D) =
    ctx.beginPath()
    ctx.moveTo(-Init.spaceWidth / 2., -Init.spaceHeight / 2.)
    ctx.lineTo(-Init.spaceWidth / 2.,  Init.spaceHeight / 2.)
    ctx.lineTo( Init.spaceWidth / 2.,  Init.spaceHeight / 2.)
    ctx.lineTo( Init.spaceWidth / 2., -Init.spaceHeight / 2.)
    ctx.lineTo(-Init.spaceWidth / 2., -Init.spaceHeight / 2.)
    ctx.closePath()
    ctx.stroke()

let render(info: CanvasInfo) (model: Model) (dispatch: Msg->unit) =
    if model.Initialized then
        let ctx = info.Context
        // Clear the canvas
        ctx.clearRect(0., 0., info.Width, info.Height)
        // Transform the canvas
        // Note that we need to flip the y axis since Canvas pixel coordinates
        // goes from top to bottom, while physics does the opposite.
        ctx.save()
        // Translate to the center
        ctx.translate(info.Width / 2., info.Height / 2.)
        // Zoom in and flip y axis
        ctx.scale(info.Zoom, -info.Zoom)

        drawShip ctx model
        drawAsteroids ctx model
        drawBounds ctx

        // Restore transform
        ctx.restore()
