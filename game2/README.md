# Pang in Atari fashion

Remake of [Pang!](https://en.wikipedia.org/wiki/Pang_(video_game)) game with Fable and a Canvas manager imitating Elmish.

[MainLoop.js](https://github.com/IceCreamYou/MainLoop.js) is used to manage the game loop and [Matter.js](http://brm.io/matter-js/) to calculate the physics.

## Development

- Run `npm install` to install JS and F# dependencies
- Run `npm start` to start Fable daemon and webpack-dev-server on localhost:8080

## Game rules

- You can only move to the sides (ARROWS)
- If a ball touches you, game is over
- There are three ball levels: 1, 2 & 4 (ball radius is BALL_RADIUS / level)
- You can only shoot a harpoon (SPACE) at a time
- When the harpoon string touches a ball, it will split into two balls of the next level, or disappear (if level is 4)
- If all balls are gone, you win
