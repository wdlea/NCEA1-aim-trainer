using System;
using System.Collections.Generic;

namespace api.objects
{
    /// <summary>
    /// This is used to unmarshal the JSON representation of Game into, this resembles the game struct on game.go
    /// </summary>
    [Serializable]
    public struct Game
    {
        public List<Player> Players;
        public int State;
        public string Name;
    }

}

