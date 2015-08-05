using System;
using System.Linq;

namespace Haven
{
    public partial class Action
    {
        private void AnswerChallengeAction(Object input)
        {
            // remove all answer challenge actions
            Persistence.Connection.Execute("delete from Action where Type=? and OwnerId=?", ActionType.AnswerChallenge, this.OwnerId);

            if (this.Challenge.CorrectAnswer((string)input))
            {
                // add the card for the space if the player doesn't have it, otherwise add a random card they don't have
                var game = Game.GetGame(this.OwnerId);
                var missingNameCards = game.Board.NameCards.Except(this.Owner.NameCards);

                if (missingNameCards.Where(x => x.Id == this.NameCardId).Count() > 0)
                {
                    var nameCard = this.NameCard;
                    Persistence.Connection.Insert(new PlayerNameCard() { PlayerId = this.OwnerId, NameCardId = this.NameCardId });
                    Persistence.Connection.Insert(new Message() { PlayerId = this.OwnerId, Text = string.Format("Correct! Earned {0} card.", nameCard.Name) });
                    game.EndTurn(this.OwnerId);
                }
                else if (missingNameCards.Count() > 0)
                {
                    var randomCard = missingNameCards.OrderBy(x => Dice.RollDice(1, missingNameCards.Count()).Sum).First();
                    Persistence.Connection.Insert(new PlayerNameCard() { PlayerId = this.OwnerId, NameCardId = randomCard.Id });
                    Persistence.Connection.Insert(new Message() { PlayerId = this.OwnerId, Text = string.Format("Correct! Earned {0} card.", randomCard.Name) });
                    game.EndTurn(this.OwnerId);
                }
                else
                {
                    Persistence.Connection.Insert(new Action() { Type = ActionType.Roll, OwnerId = this.OwnerId });
                    Persistence.Connection.Insert(new Message() { PlayerId = this.OwnerId, Text = "You have all the challenge cards. Roll again!" });
                }
            }
            else
            {
                Persistence.Connection.Insert(new Message() { PlayerId = this.OwnerId, Text = "Incorrect!" });
                Game.GetGame(this.OwnerId).EndTurn(this.OwnerId);
            }
        }
    }
}
