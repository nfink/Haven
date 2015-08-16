using System;

namespace Haven
{
    public partial class Action
    {
        private void EnterNameAction(Object input)
        {
            this.Repository.Remove(this);
            var player = this.Owner;
            player.Name = (string)input;
            this.Repository.Update(player);
            var game = this.Repository.Get<Game>(player.GameId);
            game.StartGame();
            this.Repository.Add(new Message() { PlayerId = player.Id, Text = string.Format("Welcome {0}!", player.Name) });
        }
    }
}
