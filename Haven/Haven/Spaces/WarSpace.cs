using System.Linq;

namespace Haven
{
    public partial class Space
    {
        private void OnLandWar(Player player)
        {
            var game = Persistence.Connection.Get<Game>(player.GameId);
            var playersOnWarSpace = game.Players.Where(x => x.SpaceId == player.SpaceId);

            if (playersOnWarSpace.Count() < 2)
            {
                // if player is the only player on this space, add a declaration action for each player who is not on a safe haven and add a decline action
                var eligiblePlayers = Persistence.Connection.Query<Player>(
                    @"select Player.* from Player
                      join Space on Player.SpaceId=Space.Id
                      where Space.Type<>? and Player.Id<>? and Player.GameId=?", SpaceType.SafeHaven, player.Id, player.GameId);

                if (eligiblePlayers.Count() > 0)
                {
                    Persistence.Connection.Insert(new Action() { Type = ActionType.DeclineWar, OwnerId = player.Id });
                    foreach (Player p in eligiblePlayers)
                    {
                        Persistence.Connection.Insert(new Action() { Type = ActionType.DeclareWar, OwnerId = player.Id, PlayerId = p.Id });
                    }

                    Persistence.Connection.Insert(new Message() { PlayerId = player.Id, Text = "Select a player to challenge to war." });
                }
                else
                {
                    Game.EndTurn(player.Id);
                    Persistence.Connection.Insert(new Message() { PlayerId = player.Id, Text = "All other players are on a safe haven and cannot be challenged to war." });
                }
            }
            else if (playersOnWarSpace.Count() == 2)
            {
                // if a single other player is on this space, player automatically goes to war as the challenger (add challenge actions)
                var challenged = playersOnWarSpace.Where(x => x != player).First();
                var challenge = game.GetNextChallenge(this.Id);
                Persistence.Connection.Insert(new Action() { Type = ActionType.AnswerWarChallenge, OwnerId = player.Id, ChallengeId = challenge.Id, PlayerId = challenged.Id, Challenger = true });
                Persistence.Connection.Insert(new Message() { PlayerId = player.Id, Text = string.Format("You have challenged {0} to war!", challenged.Name) });
            }
            else
            {
                // if multiple other players are on this space, add a declaration action for each
                foreach (Player p in playersOnWarSpace.Where(x => x != player))
                {
                    Persistence.Connection.Insert(new Action() { Type = ActionType.DeclareWar, OwnerId = player.Id, PlayerId = p.Id });
                }

                Persistence.Connection.Insert(new Message() { PlayerId = player.Id, Text = "Select a player to challenge to war." });
            }
        }
    }
}
