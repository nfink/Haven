namespace Haven
{
    public partial class Space
    {
        private void OnLandChallege(Player player)
        {
            var challenge = player.Game.GetNextChallenge(this.Id);
            this.Repository.Add(new Action() { Type = ActionType.AnswerChallenge, OwnerId = player.Id, NameCardId = this.NameCardId, ChallengeId = challenge.Id });
            this.Repository.Add(new Message() { PlayerId = player.Id, Text = challenge.Question });
        }
    }
}
