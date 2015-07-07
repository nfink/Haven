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
        None = 0,
        Recall,
        Challenge,
        ExchangePlaces,
        OptionalTurnAround,
        RollToGo,
        SafeHaven,
        TurnAround,
        War,
    }

    public partial class Space : IDeletable
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public int BoardId { get; set; }

        public int Order { get; set; }

        public string Name
        {
            get
            {
                switch (this.Type)
                {
                    case SpaceType.Recall:
                        return this.BibleVerse.ToString();
                    case SpaceType.Challenge:
                        return this.NameCard.Name;
                    case SpaceType.ExchangePlaces:
                        return "Exchange Places";
                    case SpaceType.OptionalTurnAround:
                        return "Turn Around?";
                    case SpaceType.RollToGo:
                        return "Roll Again";
                    case SpaceType.SafeHaven:
                        return this.SafeHavenCard.Name;
                    case SpaceType.TurnAround:
                        return "Turn Around";
                    case SpaceType.War:
                        return "War!";
                    default:
                        return this.Type.ToString();
                }
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

        public void OnLand(Player player)
        {
            switch (this.Type)
            {
                case (SpaceType.Recall):
                    OnLandRecall(player);
                    break;
                case (SpaceType.Challenge):
                    OnLandChallege(player);
                    break;
                case (SpaceType.ExchangePlaces):
                    OnLandExchangePlaces(player);
                    break;
                case (SpaceType.OptionalTurnAround):
                    OnLandOptionalTurnAround(player);
                    break;
                case (SpaceType.RollToGo):
                    OnLandRollToGo(player);
                    break;
                case (SpaceType.SafeHaven):
                    OnLandSafeHaven(player);
                    break;
                case (SpaceType.TurnAround):
                    OnLandTurnAround(player);
                    break;
                case (SpaceType.War):
                    OnLandWar(player);
                    break;
                default:
                    throw new Exception("Space has no Type");
            }
        }

        public void Delete()
        {
            // delete any dependent records
            if (this.Id != 0)
            {
                if (this.BibleVerseId != 0)
                {
                    Persistence.Connection.Execute("delete from BibleVerse where Id=?", this.BibleVerseId);
                }
                if (this.NameCardId != 0)
                {
                    Persistence.Connection.Execute("delete from NameCard where Id=?", this.NameCardId);
                }
                if (this.SafeHavenCardId != 0)
                {
                    Persistence.Connection.Execute("delete from SafeHavenCard where Id=?", this.SafeHavenCardId);
                }
            }

            // delete space
            Persistence.Connection.Delete(this);
        }
    }
}
