using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Haven
{
    public partial class Space
    {
        private void OnLandSafeHaven(Player player)
        {
            // add the safe haven card if the player has all name cards
            var safeHaven = Persistence.Connection.Get<SafeHavenCard>(this.SafeHavenCardId);
            var missingNameCards = Persistence.Connection.Query<NameCard>(
                @"select NameCard.* from NameCard where Id not in (
                    select NameCard.Id from NameCard
                    join PlayerNameCard on NameCard.Id=PlayerNameCard.NameCardId
                    join Player on PlayerNameCard.PlayerId=Player.Id
                    where Player.Id=?)", player.Id);

            if ((missingNameCards.Count < 1) && (Persistence.Connection.Query<PlayerSafeHavenCard>("select PlayerSafeHavenCard.* from PlayerSafeHavenCard where PlayerId=? and SafeHavenCardId=?", player.Id, this.SafeHavenCardId).Count() < 1))
            {
                Persistence.Connection.Insert(new PlayerSafeHavenCard() { PlayerId = player.Id, SafeHavenCardId = this.SafeHavenCardId });

                // if player has all safe haven cards, win the game
                var missingSafeHavenCards = Persistence.Connection.Query<NameCard>(
                @"select SafeHavenCard.* from SafeHavenCard where Id not in (
                    select SafeHavenCard.Id from SafeHavenCard
                    join PlayerSafeHavenCard on SafeHavenCard.Id=PlayerSafeHavenCard.SafeHavenCardId
                    join Player on PlayerSafeHavenCard.PlayerId=Player.Id
                    where Player.Id=?)", player.Id);

                if (missingSafeHavenCards.Count < 1)
                {
                    Persistence.Connection.Insert(new Message() { PlayerId = player.Id, Text = string.Format("{0} gives you an award. You have collected all awards and win the game!", safeHaven.Name) });
                }
                else
                {
                    Persistence.Connection.Insert(new Message() { PlayerId = player.Id, Text = string.Format("{0} protects you this turn and gives you an award", safeHaven.Name) });
                }
            }
            else
            {
                Persistence.Connection.Insert(new Message() { PlayerId = player.Id, Text = string.Format("{0} protects you this turn.", safeHaven.Name) });
            }

            Game.EndTurn(player.Id);
        }
    }
}
