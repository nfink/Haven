using System;

namespace Haven
{
    public partial class Action
    {
        private void EnterPasswordAction(Object input)
        {
            this.Repository.Remove(this);
            var player = this.Owner;
            player.SetPassword((string)input);
            var game = this.Repository.Get<Game>(player.GameId);
            game.StartGame();
            this.Repository.Add(new Message() { PlayerId = player.Id, Text = "Password saved." });
        }
    }
}
