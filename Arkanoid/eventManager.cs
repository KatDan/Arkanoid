using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arkanoid
{
    /// <summary>
    /// A singleton class that is used for communication between the GUI and the internal mechanisms of the game.
    /// </summary>
    public class EventManager
    {
        /// <summary>
        /// Contains every possible reason for the game timer to stop, and NONE.
        /// </summary>
        public enum GameStopper
        {
            NONE = 0,
            LEVELUP = 1,
            LIFEDOWN = 2,
            GAMEOVER = 3,
            WIN = 4
        };

        /// <summary>
        /// Current GameStopper in the internal tick of the game.
        /// </summary>
        internal GameStopper gameStopper = GameStopper.NONE;

        /// <summary>
        /// A flag whether the brick was hit in the game tick or not.
        /// </summary>
        internal bool brickHit = false;

        /// <summary>
        /// All possible states of the power up's life.
        /// </summary>
        public enum PowerUpState
        {
            NONE,
            EXISTS,
            FALLING,
            TAKEN
        };

        /// <summary>
        /// Current PowerUpState in the internal tick of the game.
        /// </summary>
        internal PowerUpState powerupState = PowerUpState.NONE;

        /// <summary>
        /// An empty constructor for the event manager.
        /// </summary>
        public EventManager() { }

        /// <summary>
        /// Resets the attributes of the instance.
        /// </summary>
        public void reset()
        {
            gameStopper = GameStopper.NONE;
            brickHit = false;
            powerupState = PowerUpState.NONE;
        }
    }

    /// <summary>
    /// A class that holds the coordinates of a brick in the brick map.
    /// </summary>
    public class Coords
    {
        /// <summary>
        /// Row of a brick in the brick map. For an empty instance set to -1. 
        /// </summary>
        internal int row = -1;

        /// <summary>
        /// column of a brick in the brick map. For an empty instance set to -1. 
        /// </summary>
        internal int column = -1;

        /// <summary>
        /// An empty constructor for the Coords instance.
        /// </summary>
        public Coords() { }

        /// <summary>
        /// Sets the new values of the coordinates.
        /// </summary>
        /// <param name="row">int new row.</param>
        /// <param name="column">int new column.</param>
        public void update(int row, int column)
        {
            this.row = row;
            this.column = column;
        }
    }
}
