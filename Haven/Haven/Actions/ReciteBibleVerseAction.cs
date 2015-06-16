using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Haven
{
    public partial class Action
    {
        private Message ReciteBibleVerseAction(Object input)
        {
            Persistence.Connection.Execute("delete from Action where (Type=? or Type=?) and OwnerId=?", ActionType.ReadBibleVerse, ActionType.ReciteBibleVerse, this.OwnerId);

            // if correct, add a roll action
            var verse = Persistence.Connection.Get<BibleVerse>(this.BibleVerseId);
            if ((string)input == verse.Text)
            {
                Persistence.Connection.Insert(new Action() { Type = ActionType.Roll, OwnerId = this.OwnerId });
                return new Message("Correct! Roll again.");
            }
            else
            {
                Game.EndTurn(this.OwnerId);
                return new Message("Incorrect!");
            }
        }
    }
}
