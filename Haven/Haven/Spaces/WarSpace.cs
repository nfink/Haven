using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Haven
{
    public partial class Space
    {
        private Message OnLandWar(Player player)
        {
            var game = Persistence.Connection.Get<Game>(player.GameId);
            var playersOnWarSpace = game.Players.Where(x => x.SpaceId == player.SpaceId);

            if (playersOnWarSpace.Count() < 2)
            {
                // if player is the only player on this space, add a declaration action for each player who is not on a safe haven and add a decline action
                var eligiblePlayers = Persistence.Connection.Query<Player>(
                    @"select Player.* from Player
                      join Space on Player.SpaceId=Space.Id
                      where Space.Type<>? and Player.Id<>?", SpaceType.SafeHaven, player.Id);

                if (eligiblePlayers.Count() > 0)
                {
                    Persistence.Connection.Insert(new Action() { Type = ActionType.DeclineWar, OwnerId = player.Id });
                    foreach (Player p in eligiblePlayers)
                    {
                        Persistence.Connection.Insert(new Action() { Type = ActionType.DeclareWar, OwnerId = player.Id, PlayerId = p.Id });
                    }

                    return new Message("Select a player to challenge to war.");
                }
                else
                {
                    Game.EndTurn(player.Id);
                    return new Message("All other players are on a safe haven and cannot be challenged to war.");
                }
            }
            else if (playersOnWarSpace.Count() == 2)
            {
                // if a single other player is on this space, player automatically goes to war as the challenger (add challenge actions)
                var challenged = playersOnWarSpace.Where(x => x != player).First();
                var challenge = game.GetNextChallenge();
                foreach (ChallengeAnswer ca in challenge.Answers)
                {
                    Persistence.Connection.Insert(new Action() { Type = ActionType.AnswerWarChallenge, OwnerId = player.Id, AnswerId = ca.Id, PlayerId = challenged.Id, Challenger = true });
                }

                return new Message(string.Format("You have challenged {0} to war!", challenged.Name));
            }
            else
            {
                // if multiple other players are on this space, add a declaration action for each
                foreach (Player p in playersOnWarSpace.Where(x => x != player))
                {
                    Persistence.Connection.Insert(new Action() { Type = ActionType.DeclareWar, OwnerId = player.Id, PlayerId = p.Id });
                }

                return new Message("Select a player to challenge to war.");
            }
        }
    }
}
