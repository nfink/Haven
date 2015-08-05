using System;

namespace Haven
{
    public partial class Action
    {
        private void EnterNameAction(Object input)
        {
            Persistence.Connection.Delete(this);
            var player = this.Owner;
            player.Name = (string)input;
            Persistence.Connection.Update(player);
            var game = Persistence.Connection.Get<Game>(player.GameId);
            game.StartGame();
            Persistence.Connection.Insert(new Message() { PlayerId = player.Id, Text = string.Format("Welcome {0}!", player.Name) });
        }
    }
}
