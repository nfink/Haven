using System.Linq;

namespace Haven
{
    public partial class Space
    {
        private void OnLandWar(Player player)
        {
            var game = player.Game;
            var playersOnWarSpace = game.Players.Where(x => x.SpaceId == player.SpaceId);

            if (playersOnWarSpace.Count() < 2)
            {
                // if player is the only player on this space, add a declaration action for each player who is not on a safe haven and add a decline action
                var eligiblePlayers = game.Players.Where(x => x.Id != player.Id && x.Space.Type != SpaceType.SafeHaven);

                if (eligiblePlayers.Count() > 0)
                {
                    this.Repository.Add(new Action() { Type = ActionType.DeclineWar, OwnerId = player.Id });
                    foreach (Player p in eligiblePlayers)
                    {
                        this.Repository.Add(new Action() { Type = ActionType.DeclareWar, OwnerId = player.Id, PlayerId = p.Id });
                    }

                    this.Repository.Add(new Message() { PlayerId = player.Id, Text = "Select a player to challenge to war." });
                }
                else
                {
                    game.EndTurn(player.Id);
                    this.Repository.Add(new Message() { PlayerId = player.Id, Text = "All other players are on a safe haven and cannot be challenged to war." });
                }
            }
            else if (playersOnWarSpace.Count() == 2)
            {
                // if a single other player is on this space, player automatically goes to war as the challenger (add challenge actions)
                var challenged = playersOnWarSpace.Where(x => x != player).Single();
                var challenge = game.GetNextChallenge(this.Id);
                this.Repository.Add(new Action() { Type = ActionType.AnswerWarChallenge, OwnerId = player.Id, ChallengeId = challenge.Id, PlayerId = challenged.Id, Challenger = true });
                this.Repository.Add(new Message() { PlayerId = player.Id, Text = string.Format("You have challenged {0} to war!", challenged.Name) });
            }
            else
            {
                // if multiple other players are on this space, add a declaration action for each
                foreach (Player p in playersOnWarSpace.Where(x => x != player))
                {
                    this.Repository.Add(new Action() { Type = ActionType.DeclareWar, OwnerId = player.Id, PlayerId = p.Id });
                }

                this.Repository.Add(new Message() { PlayerId = player.Id, Text = "Select a player to challenge to war." });
            }
        }
    }
}
