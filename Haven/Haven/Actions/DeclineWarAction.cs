using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Haven
{
    public partial class Action
    {
        private Message DeclineWarAction(Object input)
        {
            // remove all war actions
            Persistence.Connection.Execute("delete from Action where (Type=? or Type=?) and OwnerId=?", ActionType.DeclareWar, ActionType.DeclineWar, this.OwnerId);

            Game.EndTurn(this.OwnerId);

            return new Message("Declined to declare war.");
        }
    }
}
