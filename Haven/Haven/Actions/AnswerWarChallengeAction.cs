using System;
using System.Linq;

namespace Haven
{
    public partial class Action
    {
        private void AnswerWarChallengeAction(Object input)
        {
            // remove all answer war challenge actions
            Persistence.Connection.Execute("delete from Action where Type=? and OwnerId=?", ActionType.AnswerWarChallenge, this.OwnerId);

            var challenge = Persistence.Connection.Get<Challenge>(this.ChallengeId);
            var enemy = Persistence.Connection.Get<Player>(this.PlayerId);

            if (challenge.CorrectAnswer((string)input))
            {
                // add a challenge to the other player in the war
                var game = Persistence.Connection.Get<Game>(enemy.GameId);
                Challenge newChallenge;
                if (this.Challenger)
                {
                    var owner = Persistence.Connection.Get<Player>(this.OwnerId);
                    newChallenge = game.GetNextChallenge(owner.SpaceId);
                }
                else
                {
                    newChallenge = game.GetNextChallenge(enemy.SpaceId);
                }
                Persistence.Connection.Insert(new Action() { Type = ActionType.AnswerWarChallenge, OwnerId = enemy.Id, Challenger = !this.Challenger, PlayerId = this.OwnerId, ChallengeId = newChallenge.Id });
                Persistence.Connection.Insert(new Message() { PlayerId = this.OwnerId, Text = string.Format("Correct! Now {0} must answer a challenge.", enemy.Name) });
            }
            else
            {
                // lose war
                if (this.Challenger)
                {
                    // give enemy all name cards that they do not have
                    var cardsToAdd = Persistence.Connection.Query<NameCard>(
                        @"select NameCard.* from NameCard
                            join PlayerNameCard on NameCard.Id=PlayerNameCard.NameCardId
                            where PlayerNameCard.PlayerId=? and PlayerNameCard.NameCardId not in
                            (select NameCardId from PlayerNameCard where PlayerId=?)", this.OwnerId, enemy.Id);

                    foreach(NameCard card in cardsToAdd)
                    {
                        Persistence.Connection.Insert(new PlayerNameCard() { PlayerId = enemy.Id, NameCardId = card.Id });
                        Persistence.Connection.Execute("delete from PlayerNameCard where PlayerId=? and NameCardId=?", this.OwnerId, card.Id);
                    }

                    Game.EndTurn(this.OwnerId);

                    if (cardsToAdd.Count > 0)
                    {
                        Persistence.Connection.Insert(new Message() { PlayerId = this.OwnerId, Text = string.Format("Incorrect! {0} has taken the following cards: {1}.", enemy.Name, cardsToAdd.Select(x => x.Name).Aggregate((x, y) => x + ", " + y)) });
                    }
                    else
                    {
                        Persistence.Connection.Insert(new Message() { PlayerId = this.OwnerId, Text = string.Format("Incorrect! {0} has won the war but you have no cards to take.", enemy.Name) });
                    }
                }
                else
                {
                    // give all name cards to the enemy
                    var cardsToAdd = Persistence.Connection.Query<NameCard>(
                        @"select NameCard.* from NameCard
                            join PlayerNameCard on NameCard.Id=PlayerNameCard.NameCardId
                            where PlayerNameCard.PlayerId=? and PlayerNameCard.NameCardId not in
                            (select NameCardId from PlayerNameCard where PlayerId=?)", this.OwnerId, enemy.Id);

                    foreach (NameCard card in cardsToAdd)
                    {
                        Persistence.Connection.Insert(new PlayerNameCard() { PlayerId = enemy.Id, NameCardId = card.Id });
                    }

                    Persistence.Connection.Execute("delete from PlayerNameCard where PlayerId=?", this.OwnerId);
                    Game.EndTurn(enemy.Id);

                    if (cardsToAdd.Count > 0)
                    {
                        Persistence.Connection.Insert(new Message() { PlayerId = this.OwnerId, Text = string.Format("Incorrect! {0} has taken all of your cards.", enemy.Name) });
                    }
                    else
                    {
                        Persistence.Connection.Insert(new Message() { PlayerId = this.OwnerId, Text = string.Format("Incorrect! {0} has won the war but you have no cards to take.", enemy.Name) });
                    }
                }
            }
        }
    }
}
