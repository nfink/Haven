using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace Haven
{
    public enum SpaceType
    {
        BibleVerse = 1,
        Challenge,
        ExchangePlaces,
        OptionalTurnAround,
        RollToGo,
        SafeHaven,
        TurnAround,
        War,
    }

    public partial class Space
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public int BoardId { get; set; }

        public int Order { get; set; }

        public string Name
        {
            get
            {
                return this.Type.ToString();
            }
        }

        public SpaceType Type { get; set; }

        public int BibleVerseId { get; set; }

        public BibleVerse BibleVerse
        {
            get
            {
                return (this.BibleVerseId != 0 ? Persistence.Connection.Get<BibleVerse>(this.BibleVerseId) : null);
            }
        }

        public int NameCardId { get; set; }

        public NameCard NameCard
        {
            get
            {
                return (this.NameCardId != 0 ? Persistence.Connection.Get<NameCard>(this.NameCardId) : null);
            }
        }

        public int SafeHavenCardId { get; set; }

        public SafeHavenCard SafeHavenCard
        {
            get
            {
                return (this.SafeHavenCardId != 0 ? Persistence.Connection.Get<SafeHavenCard>(this.SafeHavenCardId) : null);
            }
        }

        public int Width { get; set; }

        public int Height { get; set; }

        public int X { get; set; }

        public int Y { get; set; }

        public string BorderColor { get; set; }

        public string BackgroundColor { get; set; }

        public string TextColor { get; set; }

        public Message OnLand(Player player)
        {
            switch (this.Type)
            {
                case (SpaceType.BibleVerse):
                    return OnLandBibleVerse(player);
                case (SpaceType.Challenge):
                    return OnLandChallege(player);
                case (SpaceType.ExchangePlaces):
                    return OnLandExchangePlaces(player);
                case (SpaceType.OptionalTurnAround):
                    return OnLandOptionalTurnAround(player);
                case (SpaceType.RollToGo):
                    return OnLandRollToGo(player);
                case (SpaceType.SafeHaven):
                    return OnLandSafeHaven(player);
                case (SpaceType.TurnAround):
                    return OnLandTurnAround(player);
                case (SpaceType.War):
                    return OnLandWar(player);
                default:
                    throw new Exception("Space has no Type");
            }
        }
    }
}
