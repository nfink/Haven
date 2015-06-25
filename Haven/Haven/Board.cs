using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace Haven
{
    public class Board
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Icon { get; set; }

        public int MessageAreaWidth { get; set; }

        public int MessageAreaHeight { get; set; }

        public int MessageAreaX { get; set; }

        public int MessageAreaY { get; set; }

        public int StatusAreaWidth { get; set; }

        public int StatusAreaHeight { get; set; }

        public int StatusAreaX { get; set; }

        public int StatusAreaY { get; set; }

        public IEnumerable<Space> Spaces
        {
            get
            {
                return Persistence.Connection.Table<Space>().Where(x => x.BoardId == this.Id).OrderBy(x => x.Order).ToList();
            }
        }

        public IEnumerable<NameCard> NameCards
        {
            get
            {
                return Persistence.Connection.Query<NameCard>(
                @"select NameCard.* from Board
                  join Space on Board.Id=Space.BoardId
                  join NameCard on Space.NameCardId=NameCard.Id
                  where Board.Id=?", this.Id);
            }
        }

        public IEnumerable<SafeHavenCard> SafeHavenCards
        {
            get
            {
                return Persistence.Connection.Query<SafeHavenCard>(
                @"select SafeHavenCard.* from Board
                  join Space on Board.Id=Space.BoardId
                  join SafeHavenCard on Space.SafeHavenCardId=SafeHavenCard.Id
                  where Board.Id=?", this.Id);
            }
        }

        public Space GetNewSpace(int startSpaceId, int spacesToMove, bool direction)
        {
            var spaces = Persistence.Connection.Table<Space>().Where(x => x.BoardId == this.Id).OrderBy(x => x.Order).ToList();
            int directionMultiplier = direction ? 1 : -1;
            int movement = spacesToMove * directionMultiplier;
            int startLocation = spaces.Select(x => x.Id).ToList().IndexOf(startSpaceId);
            int endLocation = (((startLocation + movement) % spaces.Count) + spaces.Count) % spaces.Count;
            return spaces[endLocation];
        }

        public void MovePlayer(Game game, Player player, int spaceId)
        {
            player.SpaceId = spaceId;
            Persistence.Connection.Update(player);
            var space = Persistence.Connection.Get<Space>(player.SpaceId);
            space.OnLand(player);
        }
    }
}
