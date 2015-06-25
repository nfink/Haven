using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Haven
{
    public partial class Action
    {
        private void EndTurnAction(Object input)
        {
            Persistence.Connection.Execute("delete from Action where OwnerId=?", this.OwnerId);

            Game.EndTurn(this.OwnerId);

            Persistence.Connection.Insert(new Message() { PlayerId = this.OwnerId, Text = "Ended turn." });
        }
    }
}
