module App.View

open Shared
open State
open Fable.Import.Browser

let initCanvas() =
    let canvas = document.getElementsByTagName_canvas().[0]
    let ctx = canvas.getContext_2d()
    ctx.lineWidth <- Init.canvasLineWidth
    ctx, canvas.width, canvas.height

let drawBodies(ctx: CanvasRenderingContext2D, model: Model) =
    // Draw all bodies. Skip the first one, it's the ground plane
    for box in Seq.skip 1 model.Boxes do
        ctx.beginPath()
        ctx.save()
        // Translate to the center of the box
        ctx.translate(box.X, box.Y)
        // Rotate to the box body frame
        ctx.rotate(box.Angle)
        ctx.rect(-Init.boxWidth/2., -Init.boxHeight/2., Init.boxWidth, Init.boxHeight)
        ctx.stroke()
        ctx.restore()

let render(ctx: CanvasRenderingContext2D, canvasWidth: float, canvasHeight: float) (model: Model) (dispatch: Msg->unit) =
    if model.Initialized then
        // Clear the canvas
        ctx.clearRect(0., 0., canvasWidth, canvasHeight)
        // Transform the canvas
        // Note that we need to flip the y axis since Canvas pixel coordinates
        // goes from top to bottom, while physics does the opposite.
        ctx.save()
        // Translate to the center
        ctx.translate(canvasWidth / 2., canvasHeight / 2.)
        // Zoom in and flip y axis
        ctx.scale(Init.canvasZoom, -Init.canvasZoom)
        // Draw all bodies
        drawBodies(ctx, model)
        // Restore transform
        ctx.restore()
