# FableConf Workshop: Create a game with Elmish

This example shows how to create a 60FPS game with physics calculation. We will use the same model-update-view structure we know, but we'll have to deal with a bit of mutability and also split the responsibility of the state update with the worker.

The `Program.withPhysicsWorker` helper will make this much easier, taking care of the synchronization between animation frames and worker messages for us. Thanks to Elmish, we will also be able to use the Redux Devtools to do some time-travel debugging.

## Building and running the app

> In the commands below, yarn is the tool of choice. If you want to use npm, just replace `yarn` by `npm` in the commands.

* Install JS dependencies: `yarn install`
* Install F# dependencies: `dotnet restore`
* **Move to `src` folder**: `cd src`
* Start Fable daemon and [Webpack](https://webpack.js.org/) dev server: `dotnet fable yarn-start`
* In your browser, open: http://localhost:8080/

If you are using VS Code + [Ionide](http://ionide.io/), you can also use the key combination: Ctrl+Shift+B (Cmd+Shift+B on macOS) instead of typing the `dotnet fable yarn-start` command. This also has the advantage that Fable-specific errors will be highlighted in the editor along with other F# errors.

Any modification you do to the F# code will be reflected in the web page after saving. When you want to output the JS code to disk, run `dotnet fable yarn-build` and you'll get a minified JS bundle in the `public` folder.
