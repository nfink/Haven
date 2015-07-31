namespace Haven
{
    public partial class Space
    {
        private void OnLandChallege(Player player)
        {
            var game = Persistence.Connection.Get<Game>(player.GameId);
            var challenge = game.GetNextChallenge();
            Persistence.Connection.Insert(new Action() { Type = ActionType.AnswerChallenge, OwnerId = player.Id, NameCardId = this.NameCardId, ChallengeId = challenge.Id });
            Persistence.Connection.Insert(new Message() { PlayerId = player.Id, Text = challenge.Question });
        }
    }
}
