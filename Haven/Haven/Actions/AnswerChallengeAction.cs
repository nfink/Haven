using System;
using System.Linq;

namespace Haven
{
    public partial class Action
    {
        private void AnswerChallengeAction(Object input)
        {
            // remove all answer challenge actions
            this.RemoveActions(ActionType.AnswerChallenge);

            var game = Player.Game;

            if (this.Challenge.CorrectAnswer((string)input))
            {
                // add the card for the space if the player doesn't have it, otherwise add a random card they don't have
                var missingNameCards = game.Board.NameCards.Except(this.Owner.NameCards);

                if (missingNameCards.Where(x => x.Id == this.NameCardId).Count() > 0)
                {
                    var nameCard = this.NameCard;
                    this.Repository.Add(new PlayerNameCard() { PlayerId = this.OwnerId, NameCardId = this.NameCardId });
                    this.Repository.Add(new Message() { PlayerId = this.OwnerId, Text = string.Format("Correct! Earned {0} card.", nameCard.Name) });
                    game.EndTurn(this.OwnerId);
                }
                else if (missingNameCards.Count() > 0)
                {
                    var randomCard = missingNameCards.OrderBy(x => Dice.RollDice(1, missingNameCards.Count()).Sum).First();
                    this.Repository.Add(new PlayerNameCard() { PlayerId = this.OwnerId, NameCardId = randomCard.Id });
                    this.Repository.Add(new Message() { PlayerId = this.OwnerId, Text = string.Format("Correct! Earned {0} card.", randomCard.Name) });
                    game.EndTurn(this.OwnerId);
                }
                else
                {
                    this.Repository.Add(new Action() { Type = ActionType.Roll, OwnerId = this.OwnerId });
                    this.Repository.Add(new Message() { PlayerId = this.OwnerId, Text = "You have all the challenge cards. Roll again!" });
                }
            }
            else
            {
                this.Repository.Add(new Message() { PlayerId = this.OwnerId, Text = "Incorrect!" });
                game.EndTurn(this.OwnerId);
            }
        }
    }
}
