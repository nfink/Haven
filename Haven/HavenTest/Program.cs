using Haven;
using System;
using System.Linq;

namespace HavenTest
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var repository = new Repository())
            {
                var dl = new Haven.Data.DataLoad();
                dl.LoadTables();

                var game = new Game();
                game.Create(1, 2);
                Console.WriteLine(game.Id);

                string command;
                do
                {
                    ListActions(repository, game.Id);
                    Console.Write("Action: ");
                    command = Console.ReadLine();
                    int actionId;
                    if (Int32.TryParse(command, out actionId))
                    {
                        PerformAction(repository, game.Id, actionId);
                    }

                } while (command != "exit");

                Console.WriteLine("done");
                Console.ReadLine();
            }
        }

        public static void ListActions(IRepository repository, int gameId)
        {
            var game = repository.Get<Game>(gameId);
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

        public static void PerformAction(IRepository repository, int gameId, int actionId)
        {
            Console.Write("Input: ");
            var input = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(input))
            {
                input = null;
            }

            var action = repository.Get<Haven.Action>(actionId);
            action.PerformAction(input);
            var latestMessage = action.Owner.Messages.First();
            Console.WriteLine(latestMessage.Text);
        }
    }
}
