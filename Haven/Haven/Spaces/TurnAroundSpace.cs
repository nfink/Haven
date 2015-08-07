namespace Haven
{
    public partial class Space
    {
        private void OnLandTurnAround(Player player)
        {
            // change player movement direction
            player.MovementDirection = !player.MovementDirection;

            Game.GetGame(player.Id).EndTurn(player.Id);

            this.Repository.Add(new Message() { PlayerId = player.Id, Text = "Turned around." });
        }
    }
}
