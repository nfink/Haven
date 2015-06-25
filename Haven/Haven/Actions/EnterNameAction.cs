﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Haven
{
    public partial class Action
    {
        private void EnterNameAction(Object input)
        {
            Persistence.Connection.Delete(this);
            var player = Persistence.Connection.Get<Player>(this.OwnerId);
            player.Name = (string)input;
            Persistence.Connection.Update(player);
            var game = Persistence.Connection.Get<Game>(player.GameId);
            game.StartGame();
            Persistence.Connection.Insert(new Message() { PlayerId = player.Id, Text = string.Format("Welcome {0}!", player.Name) });
        }
    }
}
