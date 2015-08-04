using System;
using System.Linq;

namespace Haven
{
    public partial class Action
    {
        private void DeclareWarAction(Object input)
        {
            // remove all war actions
            Persistence.Connection.Execute("delete from Action where (Type=? or Type=?) and OwnerId=?", ActionType.DeclareWar, ActionType.DeclineWar, this.OwnerId);

            // add challenge to the challenger
            var game = Persistence.Connection.Query<Game>("select * from Game where Id=(select GameId from Player where Id=?)", this.OwnerId).First();
            var player = Persistence.Connection.Get<Player>(this.OwnerId);
            var challenge = game.GetNextChallenge(player.SpaceId);
            Persistence.Connection.Insert(new Action() { Type = ActionType.AnswerWarChallenge, OwnerId = this.OwnerId, Challenger = true, PlayerId = this.PlayerId, ChallengeId = challenge.Id });
            var challengedPlayer = Persistence.Connection.Get<Player>(this.PlayerId);
            Persistence.Connection.Insert(new Message() { PlayerId = this.OwnerId, Text = string.Format("Declared war against {0}!", challengedPlayer.Name) });
        }
    }
}
