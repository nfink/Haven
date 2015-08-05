namespace Haven
{
    public partial class Space
    {
        private void OnLandOptionalTurnAround(Player player)
        {
            Persistence.Connection.Insert(new Action() { Type = ActionType.TurnAround, OwnerId = player.Id });
            Persistence.Connection.Insert(new Action() { Type = ActionType.EndTurn, OwnerId = player.Id });
            Persistence.Connection.Insert(new Message() { PlayerId = player.Id, Text = "You may choose to turn around." });
        }
    }
}
