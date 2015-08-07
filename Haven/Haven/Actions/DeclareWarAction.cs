using System;

namespace Haven
{
    public partial class Action
    {
        private void DeclareWarAction(Object input)
        {
            // remove all war actions
            this.RemoveActions(ActionType.DeclareWar);
            this.RemoveActions(ActionType.DeclineWar);

            // add challenge to the challenger
            var game = Game.GetGame(this.OwnerId);
            var challenge = game.GetNextChallenge(this.Owner.SpaceId);
            this.Repository.Add(new Action() { Type = ActionType.AnswerWarChallenge, OwnerId = this.OwnerId, Challenger = true, PlayerId = this.PlayerId, ChallengeId = challenge.Id });
            this.Repository.Add(new Message() { PlayerId = this.OwnerId, Text = string.Format("Declared war against {0}!", this.Player.Name) });
        }
    }
}
