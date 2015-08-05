namespace Haven
{
    public partial class Space
    {
        private void OnLandChallege(Player player)
        {
            var challenge = Game.GetGame(player.Id).GetNextChallenge(this.Id);
            Persistence.Connection.Insert(new Action() { Type = ActionType.AnswerChallenge, OwnerId = player.Id, NameCardId = this.NameCardId, ChallengeId = challenge.Id });
            Persistence.Connection.Insert(new Message() { PlayerId = player.Id, Text = challenge.Question });
        }
    }
}
