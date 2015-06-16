using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace Haven
{
    public class Player
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string Name { get; set; }

        public int PieceId { get; set; }

        public int SpaceId { get; set; }

        public int GameId { get; set; }

        // true = clockwise
        // false = counter-clockwise
        public bool MovementDirection { get; set; }

        public int NextPlayerId { get; set; }

        public IEnumerable<Action> Actions
        {
            get
            {
                return Persistence.Connection.Table<Action>().Where(x => x.OwnerId == this.Id);
            }
        }

        public Player()
        {
            this.MovementDirection = true;
        }

        public static IEnumerable<Action> GetAvailableAction(int playerId)
        {
            return Persistence.Connection.Table<Action>().Where(x => x.OwnerId == playerId);
        }
    }
}
