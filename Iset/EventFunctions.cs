using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Iset
{
    class EventFunctions
    {
        public static string pickRandomPlayer(string players)
        {
            List<string> playersToChooseFrom = new List<string>();
            foreach (string player in players.Split(' '))
            {
                if (!string.IsNullOrEmpty(player))
                {
                    playersToChooseFrom.Add(player);
                }
            }
            return playersToChooseFrom.Count().ToString();
        }
    }
}
