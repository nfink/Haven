using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Haven
{
    public partial class Space
    {
        private void OnLandRollToGo(Player player)
        {
            Persistence.Connection.Insert(new Action() { Type = ActionType.RollToGo, OwnerId = player.Id });
            Persistence.Connection.Insert(new Message() { PlayerId = player.Id, Text = "Roll again. If even, go forward, if odd, go backward." });
        }
    }
}
