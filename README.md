# Arkanoid
Semestral program for C# programming in 2020/2021 

![alt text](https://github.com/KatDan/Arkanoid/blob/master/Arkanoid/image.png?raw=true)

## User documentation

The goal of this game is to survive all the levels. Using a paddle, the player ejects a ball or reflects it 
to destroy all the bricks above in current level.
Each type of brick needs to be hit a certain amount of times before it is destroyed.
If there are no bricks left, the level is completed and player proceeds to a more difficult level.
The game currently contains 3 levels.

At the start of the game, the player has 3 lives.
If a ball falls down, the player loses 1 life.
If there are no lives left, the game is over.

### Bricks

There are 5 types of bricks that appear in the game:
 - pink brick - it is destroyed after 1 hit of the ball
 - gray brick - it is destroyed after 2 hits of the ball
 - golden brick - it is destroyed after 3 hits of the ball
 - dark red brick - it is destroyed after 4 hits of the ball
 - cyan brick - it is destroyed after 5 hits of the ball

### Power ups

Power ups are activated when the paddle catches them when falling down.
There are 4 different power ups that can appear in the game:
 - slow ball - slows down all the balls that exist
 - fast ball - speeds up all the balls that exist
 - superball - enlarges all the balls that exist
 - triple ball - adds another balls to the game so the player plays with 3 balls at once 

### Controls
 - right and left arrowkeys move the paddle
 - space ejects the ball if the paddle holds it. It also restarts the game if the game is over.
 - 'p' pauses and unpauses the game at any moment



## Developer documentation
The project is divided into 3 logical parts:
 - the form - visualization and player input processing
 - the objects' GUI - each main object in the game has its own GUI manager class
 - the game logic - movement of the objects, resolution of objects' collisions etc.

### GameForm
This class is in _Form1.cs_.
It represents the GUI of the game as a whole
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

### The GUI of the objects

For more organized management of the objects visualization, an abstract class `ObjectGUI` is implemented.
Each class that inherits from this class contains PictureBox of the object and method `updatePosition()`, that moves pictureBox accordingly to the new position of the object it manages. Each one of inheriting classes contains an instance of the corresponding object that is visualized.

Specialized classes:
 - `BallGUI` - manages visualization of the ball
 - `PaddleGUI` - manages visualization of the paddle
 - `PowerUpGUI` - manages visualization of a power up
 - `BrickMapGUI` - manages visualization of level's brick map

### The game logic

Each one of the main objects has its own class.

#### Paddle
This class manages the behavior of the paddle. It can be resized or moved horizontally.
To avoid repetitive angles of reflection, the ball does not reflect on the surface of the paddle as it were a regular rectangle. To create more interesting reflections, the surface of the paddle is considered to be an arc of a bigger circle which has center under the center of the paddle.

#### Ball
This class represents a ball in the game. There can be multiple balls at once during the game, each one of them will have its own `Ball` instance.
The behavior of the ball is defined by its position, radius, angle and velocity.
The ball's collisions with other objects are processed in class `Collider`.

#### Brick 
This class represents a single brick in the level brick map.
It is defined by position of its center, size, thickness (how many hits are needed before the brick is destroyed), and a score that the player achieves by hitting it. If the brick is destroyed, its flag `bool isAlive` is set to `false`.

#### PowerUp
This class represents a power up. It is defined by its radius, position of the center, speed of falling down, visibility state and the type.

Visibility state describes whether the power up is visible and twinkling without moving, or whether it is falling down.

Type of a power up is one of the values from `enum PowerUpType`, that determines what special behavior will be activated after paddle catching the power up.

#### Game
This class connects all the logical components of the game, including instances of `Paddle`, `Ball`, `PowerUp`, 2D array of `Brick`s (brick map) of the level, score of the player, number of the player's remaining lives, etc.

Important component of this class is an instance of class `Collider`, that detects the collisions of the objects (usually the ball and another object). When the collision is detected, this class changes the angle of the ball so it bounces nicely.

For communication between the GUI and the internal logic of the game is class `EventManager`. If the game needs to be stopped (level up, life down, game over), it enables `GameForm` to react.

It contains `bool brickHit` that is set to `true` if any brick was hit in the last gameTick.

It also contains information about the state of a power up (whether it exits, twinkles, falls down or is caught).

The most impotant part of this class is method `gameTick()`.
Each tick the objects' positions and properties are updated important information is passed to the GUI via `EventManager` instance that is common for both `Game` and `GameForm` classes.



