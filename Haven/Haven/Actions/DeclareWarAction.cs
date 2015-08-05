using System;

namespace Haven
{
    public partial class Action
    {
        private void DeclareWarAction(Object input)
        {
            // remove all war actions
            Persistence.Connection.Execute("delete from Action where (Type=? or Type=?) and OwnerId=?", ActionType.DeclareWar, ActionType.DeclineWar, this.OwnerId);

            // add challenge to the challenger
            var game = Game.GetGame(this.OwnerId);
            var challenge = game.GetNextChallenge(this.Owner.SpaceId);
            Persistence.Connection.Insert(new Action() { Type = ActionType.AnswerWarChallenge, OwnerId = this.OwnerId, Challenger = true, PlayerId = this.PlayerId, ChallengeId = challenge.Id });
            Persistence.Connection.Insert(new Message() { PlayerId = this.OwnerId, Text = string.Format("Declared war against {0}!", this.Player.Name) });
        }
    }
}
