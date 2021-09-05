using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arkanoid
{
    public class EventManager
    {
        public enum GameStopper
        {
            NONE = 0,
            LEVELUP = 1,
            LIFEDOWN = 2,
            GAMEOVER = 3,
            WIN = 4
        };

        internal GameStopper gameStopper = GameStopper.NONE;

        internal bool brickHit = false;

        public enum PowerUpState
        {
            NONE,
            EXISTS,
            FALLING,
            TAKEN
        };

        internal PowerUpState powerupState = PowerUpState.NONE;

        public EventManager() { }

        public void reset()
        {
            gameStopper = GameStopper.NONE;
            brickHit = false;
            powerupState = PowerUpState.NONE;
        }
    }

    public class Coords
    {
        internal int row = -1;
        internal int column = -1;

        public Coords() { }

        public void update(int row, int column)
        {
            this.row = row;
            this.column = column;
        }
    }
}
