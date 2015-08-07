namespace Haven
{
    public partial class Space
    {
        private void OnLandRollToGo(Player player)
        {
            this.Repository.Add(new Action() { Type = ActionType.RollToGo, OwnerId = player.Id });
            this.Repository.Add(new Message() { PlayerId = player.Id, Text = "Roll again. If even, go forward, if odd, go backward." });
        }
    }
}
