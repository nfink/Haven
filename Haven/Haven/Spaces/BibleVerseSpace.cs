using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Haven
{
    public partial class Space
    {
        private void OnLandBibleVerse(Player player)
        {
            // player can either read the verse or recite it
            Persistence.Connection.Insert(new Action() { Type = ActionType.ReadBibleVerse, OwnerId = player.Id, BibleVerseId = this.BibleVerseId });
            Persistence.Connection.Insert(new Action() { Type = ActionType.ReciteBibleVerse, OwnerId = player.Id, BibleVerseId = this.BibleVerseId });

            var verse = Persistence.Connection.Get<BibleVerse>(this.BibleVerseId);

            Persistence.Connection.Insert(new Message() { PlayerId = player.Id, Text = verse.ToString() });
        }
    }
}
