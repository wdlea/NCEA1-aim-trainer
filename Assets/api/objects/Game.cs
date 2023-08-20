using System;
using System.Collections.Generic;

namespace api.objects
{
    [Serializable]
    public struct Game
    {
        public List<Player> Players;
        public int State;
        public string Name;
    }

}

