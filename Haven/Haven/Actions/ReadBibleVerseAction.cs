using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Haven
{
    public partial class Action
    {
        private Message ReadBibleVerseAction(Object input)
        {
            Persistence.Connection.Execute("delete from Action where (Type=? or Type=?) and OwnerId=?", ActionType.ReadBibleVerse, ActionType.ReciteBibleVerse, this.OwnerId);

            // display bible verse


            Game.EndTurn(this.OwnerId);

            return new Message("Read bible verse.");
        }
    }
}
