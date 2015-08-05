using Haven;
using System;
using System.Linq;

namespace HavenTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var dl = new Haven.Data.DataLoad();
            dl.LoadTables();

            var game = Game.NewGame(1, 2);
            Console.WriteLine(game.Id);

            string command;
            do
            {
                ListActions(game.Id);
                Console.Write("Action: ");
                command = Console.ReadLine();
                int actionId;
                if (Int32.TryParse(command, out actionId))
                {
                    PerformAction(game.Id, actionId);
                }

            } while (command != "exit");

            Console.WriteLine("done");
            Console.ReadLine();
        }

        public static void ListActions(int gameId)
        {
            var game = Persistence.Connection.Get<Game>(gameId);
            foreach (Player player in game.Players)
            {
                Console.WriteLine(string.IsNullOrWhiteSpace(player.Name) ? string.Format("Player {0}:", player.Id) : player.Name + ": ");
                Console.WriteLine(string.Format("On {0}", player.SpaceId));
                foreach (Haven.Action a in player.Actions)
                {
                    Console.WriteLine(a.Id + ") " + a);
                }
            }
        }

        public static void PerformAction(int gameId, int actionId)
        {
            Console.Write("Input: ");
            var input = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(input))
            {
                input = null;
            }

            var action = Persistence.Connection.Get<Haven.Action>(actionId);
            action.PerformAction(input);
            var latestMessage = Persistence.Connection.Query<Message>("select Message.* from Message where Id=(select max(Message.Id) from Message join Player on Message.PlayerId=Player.Id where Player.Id=?)", action.OwnerId).First();
            Console.WriteLine(latestMessage.Text);
        }
    }
}
