namespace Haven
{
    public partial class Space
    {
        private void OnLandOptionalTurnAround(Player player)
        {
            this.Repository.Add(new Action() { Type = ActionType.TurnAround, OwnerId = player.Id });
            this.Repository.Add(new Action() { Type = ActionType.EndTurn, OwnerId = player.Id });
            this.Repository.Add(new Message() { PlayerId = player.Id, Text = "You may choose to turn around." });
        }
    }
}
