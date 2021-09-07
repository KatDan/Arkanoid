# Arkanoid
Semestral program for C# programming in 2020/2021 

## User documentation

The goal of this game is to survive all the levels. Using a paddle, the player ejects a ball or reflects it 
to destroy all the bricks above in current level.
Each type of brick needs to be hit a certain amount of times before it is destroyed.
If there are no bricks left, the level is completed and player proceeds to a more difficult level.
The game currently contains 3 levels.

At the start of the game, the player has 3 lives.
If a ball falls down, the player loses 1 life.
If there are no lives left, the game is over.

### Controls
 - right and left arrowkeys move the paddle
 - space ejects the ball if the paddle holds it. It also restarts the game if the game is over.
 - 'p' pauses and unpauses the game at any moment



## Developer documentation
The project is divided into 3 logical parts:
 - the form - visualization and player input processing
 - the objects' UI - each main object in the game has its own UI manager class
 - the game logic - movement of the objects, resolution of objects' collisions etc.

### GameForm
This class is in _Form1.cs_.
It represents the UI of the game as a whole
and handles keyboard inputs.

Important methods:
 - updating methods:
   - `updateScoreLabel()`
   - `updateLevelLabel()`
   - `updateHearts()`
 - `tick()` - tick of a timer. It calls Game.gameTick(), processes its results and updates the UI accordingly
 - input processing methods:
   - `keyIsDown(object sender, KeyEventArgs e)`
   - `keyIsPressed(object sender, KeyEventArgs e)`
