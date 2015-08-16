using System;
using System.Linq;

namespace Haven
{
    public partial class Action
    {
        private void AnswerWarChallengeAction(Object input)
        {
            // remove all answer war challenge actions
            this.RemoveActions(ActionType.AnswerWarChallenge);

            var enemy = this.Player;
            var game = this.Repository.Get<Game>(enemy.GameId);

            if (this.Challenge.CorrectAnswer((string)input))
            {
                // add a challenge to the other player in the war
                Challenge newChallenge;
                if (this.Challenger)
                {
                    var owner = this.Repository.Get<Player>(this.OwnerId);
                    newChallenge = game.GetNextChallenge(owner.SpaceId);
                }
                else
                {
                    newChallenge = game.GetNextChallenge(enemy.SpaceId);
                }
                this.Repository.Add(new Action() { Type = ActionType.AnswerWarChallenge, OwnerId = enemy.Id, Challenger = !this.Challenger, PlayerId = this.OwnerId, ChallengeId = newChallenge.Id });
                this.Repository.Add(new Message() { PlayerId = this.OwnerId, Text = string.Format("Correct! Now {0} must answer a challenge.", enemy.Name) });
            }
            else
            {
                // lose war
                if (this.Challenger)
                {
                    // give enemy all name cards that they do not have
                    var cardsToAdd = this.Owner.NameCards.Except(enemy.NameCards);

                    foreach(NameCard nameCard in cardsToAdd)
                    {
                        this.Repository.Add(new PlayerNameCard() { PlayerId = enemy.Id, NameCardId = nameCard.Id });
                        foreach (PlayerNameCard card in this.Repository.Find<PlayerNameCard>(x => x.PlayerId == this.OwnerId && x.NameCardId == nameCard.Id))
                        {
                            this.Repository.Remove(card);
                        }
                    }

                    if (cardsToAdd.Count() > 0)
                    {
                        this.Repository.Add(new Message() { PlayerId = this.OwnerId, Text = string.Format("Incorrect! {0} has taken the following cards: {1}.", enemy.Name, cardsToAdd.Select(x => x.Name).Aggregate((x, y) => x + ", " + y)) });
                    }
                    else
                    {
                        this.Repository.Add(new Message() { PlayerId = this.OwnerId, Text = string.Format("Incorrect! {0} has won the war but you have no cards they can take.", enemy.Name) });
                    }

                    game.EndTurn(this.OwnerId);
                }
                else
                {
                    // give all name cards to the enemy
                    var cardsToAdd = this.Owner.NameCards.Except(enemy.NameCards);

                    foreach (NameCard card in cardsToAdd)
                    {
                        this.Repository.Add(new PlayerNameCard() { PlayerId = enemy.Id, NameCardId = card.Id });
                    }

                    foreach (PlayerNameCard card in this.Repository.Find<PlayerNameCard>(x => x.PlayerId == this.OwnerId))
                    {
                        this.Repository.Remove(card);
                    }
                    
                    if (cardsToAdd.Count() > 0)
                    {
                        this.Repository.Add(new Message() { PlayerId = this.OwnerId, Text = string.Format("Incorrect! {0} has taken all of your cards.", enemy.Name) });
                    }
                    else
                    {
                        this.Repository.Add(new Message() { PlayerId = this.OwnerId, Text = string.Format("Incorrect! {0} has won the war but you have no cards they can take.", enemy.Name) });
                    }

                    game.EndTurn(enemy.Id);
                }
            }
        }
    }
}
