using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Haven;

namespace HavenTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var dl = new Haven.Data.DataLoad();
            dl.LoadTables();

            var gameId = Game.NewGame(1, 2).Id;
            Console.WriteLine(gameId);

            string command;
            do
            {
                ListActions(gameId);
                Console.Write("Action: ");
                command = Console.ReadLine();
                int commandNumber;
                if (Int32.TryParse(command, out commandNumber))
                {
                    PerformAction(gameId, commandNumber);
                }

            } while (command != "exit");

            Console.WriteLine("done");
            Console.ReadLine();
        }

        public static void ListActions(int gameId)
        {
            var game = Persistence.Connection.Get<Game>(gameId);
            var currentPlayer = Persistence.Connection.Get<Player>(game.CurrentPlayerId);
            Console.WriteLine(string.IsNullOrWhiteSpace(currentPlayer.Name) ? string.Format("Player {0}:", currentPlayer.Id) : currentPlayer.Name + ": ");
            Console.WriteLine(string.Format("On {0}", currentPlayer.SpaceId));
            int i = 0;
            foreach (Haven.Action a in Player.GetAvailableAction(game.CurrentPlayerId))
            {
                Console.WriteLine(i + ") " + a);
                i++;
            }
        }

        public static void PerformAction(int gameId, int number)
        {
            Console.Write("Input: ");
            var input = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(input))
            {
                input = null;
            }

            var game = Persistence.Connection.Get<Game>(gameId);
            int i = 0;
            foreach (Haven.Action a in Player.GetAvailableAction(game.CurrentPlayerId))
            {
                if (i == number)
                {
                    var message = a.PerformAction(input);
                    Console.WriteLine(message.Text);
                    return;
                }
                i++;
            }

            if (Player.GetAvailableAction(game.CurrentPlayerId).Count() < 1)
            {
                Console.WriteLine("------ No actions available ------");
                Game.NextPlayer(game.CurrentPlayerId);
            }
        }
    }
}
