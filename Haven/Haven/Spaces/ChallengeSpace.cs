using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Haven
{
    public partial class Space
    {
        private void OnLandChallege(Player player)
        {
            var game = Persistence.Connection.Get<Game>(player.GameId);
            var challenge = game.GetNextChallenge();

            // add answers
            foreach (ChallengeAnswer ca in challenge.Answers)
            {
                Persistence.Connection.Insert(new Action() { Type = ActionType.AnswerChallenge, OwnerId = player.Id, NameCardId = this.NameCardId, AnswerId = ca.Id });
            }

            Persistence.Connection.Insert(new Message() { PlayerId = player.Id, Text = challenge.Question });
        }
    }
}
