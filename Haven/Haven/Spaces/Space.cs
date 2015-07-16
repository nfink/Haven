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

        public string Description
        {
            get
            {
                switch (this.Type)
                {
                    case SpaceType.Recall:
                        return "Player landing here may read the text, or attempt to recite it. Reciting it accurately is rewarded with a bonus turn.";
                    case SpaceType.Challenge:
                        return "Player landing here must answer one of the challenge questions. If correct, the player is rewarded with a challenge card:\n\n" + this.NameCard.Name + "\n" + this.NameCard.Details;
                    case SpaceType.ExchangePlaces:
                        return "Player landing here must exchange places with another player of their choice.";
                    case SpaceType.OptionalTurnAround:
                        return "Player landing here may choose to change direction, moving around the board in the opposite direction.";
                    case SpaceType.RollToGo:
                        return "Player landing here must roll again. An even number and the player moves forward that number of spaces. An odd number and the player moves backward that number of spaces.";
                    case SpaceType.SafeHaven:
                        return "Player landing here is safe from war. If the player has collected all of the challenge cards, they are rewarded with a safe haven card:\n\n" + this.SafeHavenCard.Name + "\n" + this.SafeHavenCard.Details;
                    case SpaceType.TurnAround:
                        return "Player landing here must change direction, moving around the board in the opposite direction.";
                    case SpaceType.War:
                        return "Player landing here may choose to challenge another player to war, or decline and remain neutral. If another player is on the War space, the player may not remain neutral. Players on a safe haven may not be challenged.";
                    default:
                        return "No description.";
                }
            }
        }

        public string Image
        {
            get
            {
                switch (this.Type)
                {
                    case SpaceType.Challenge:
                        return this.NameCard.Image.Filepath;
                    case SpaceType.SafeHaven:
                        return this.SafeHavenCard.Image.Filepath;
                    default:
                        return null;
                }
            }
        }

        public string Icon
        {
            get
            {
                switch (this.Type)
                {
                    case SpaceType.Recall:
                        return "mif-books";
                    case SpaceType.Challenge:
                        return "mif-question";
                    case SpaceType.ExchangePlaces:
                        return "mif-loop2";
                    case SpaceType.OptionalTurnAround:
                        return "mif-tab";
                    case SpaceType.RollToGo:
                        return "mif-dice";
                    case SpaceType.SafeHaven:
                        return "mif-home";
                    case SpaceType.TurnAround:
                        return "mif-undo";
                    case SpaceType.War:
                        return "mif-fire";
                    default:
                        return null;
                }
            }
        }

        [Ignore]
        public Space PreviousSpace { get; set; }

        [Ignore]
        public Space NextSpace { get; set; }

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
