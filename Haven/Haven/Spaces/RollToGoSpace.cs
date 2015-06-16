using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Haven
{
    public partial class Space
    {
        private Message OnLandRollToGo(Player player)
        {
            Persistence.Connection.Insert(new Action() { Type = ActionType.RollToGo, OwnerId = player.Id });
            return new Message("Roll again. If even, go forward, if odd, go backward.");
        }
    }
}
