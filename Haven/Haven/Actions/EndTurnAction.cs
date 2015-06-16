using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Haven
{
    public partial class Action
    {
        private Message EndTurnAction(Object input)
        {
            Persistence.Connection.Execute("delete from Action where OwnerId=?", this.OwnerId);

            Game.EndTurn(this.OwnerId);

            return new Message("Ended turn.");
        }
    }
}
