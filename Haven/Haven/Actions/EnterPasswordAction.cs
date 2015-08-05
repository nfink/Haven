using System;

namespace Haven
{
    public partial class Action
    {
        private void EnterPasswordAction(Object input)
        {
            Persistence.Connection.Delete(this);
            var player = this.Owner;
            player.SetPassword((string)input);
            var game = Persistence.Connection.Get<Game>(player.GameId);
            game.StartGame();
            Persistence.Connection.Insert(new Message() { PlayerId = player.Id, Text = "Password saved." });
        }
    }
}
