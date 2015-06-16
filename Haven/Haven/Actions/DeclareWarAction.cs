using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Haven
{
    public partial class Action
    {
        private Message DeclareWarAction(Object input)
        {
            // remove all war actions
            Persistence.Connection.Execute("delete from Action where (Type=? or Type=?) and OwnerId=?", ActionType.DeclareWar, ActionType.DeclineWar, this.OwnerId);

            // add challenge to the challenger
            var game = Persistence.Connection.Query<Game>("select * from Game where Id=(select GameId from Player where Id=?)", this.OwnerId).First();
            var challenge = game.GetNextChallenge();
            foreach (ChallengeAnswer ca in challenge.Answers)
            {
                Persistence.Connection.Insert(new Action() { Type = ActionType.AnswerWarChallenge, OwnerId = this.PlayerId, Challenger = true, PlayerId = this.PlayerId, AnswerId = ca.Id });
            }

            var challengedPlayer = Persistence.Connection.Get<Player>(this.PlayerId);
            return new Message(string.Format("Declared war against {0}!", challengedPlayer.Name));
        }
    }
}
