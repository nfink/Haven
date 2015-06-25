using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Haven
{
    public partial class Action
    {
        private void ReadBibleVerseAction(Object input)
        {
            Persistence.Connection.Execute("delete from Action where (Type=? or Type=?) and OwnerId=?", ActionType.ReadBibleVerse, ActionType.ReciteBibleVerse, this.OwnerId);

            // display bible verse


            Game.EndTurn(this.OwnerId);

            Persistence.Connection.Insert(new Message() { PlayerId = this.OwnerId, Text = "Read bible verse." });
        }
    }
}
