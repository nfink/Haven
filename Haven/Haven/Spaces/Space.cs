using SQLite;
using System;

namespace Haven
{
    public partial class Space : IDeletable, ICloneable<Space>
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

        public int BackgroundColorId { get; set; }

        public int TextColorId { get; set; }

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
                    case SpaceType.SafeHaven:
                        return this.SafeHavenCard.Name;
                    default:
                        return this.Type.GetName();
                }
            }
        }

        public string Description
        {
            get
            {
                return this.Type.GetDescription();
            }
        }

        public int ImageId { get; set; }

        public string Image
        {
            get
            {
                if (this.ImageId == 0)
                {
                    Image image;
                    switch (this.Type)
                    {
                        case SpaceType.Challenge:
                            image = this.NameCard.Image;
                            return image != null ? image.Filepath : null;
                        case SpaceType.SafeHaven:
                            image = this.SafeHavenCard.Image;
                            return image != null ? image.Filepath : null;
                        default:
                            return null;
                    }
                }
                else
                {
                    return Persistence.Connection.Get<Image>(this.ImageId).Filepath;
                }
            }
        }

        public string Icon
        {
            get
            {
                return this.Type.GetIcon();
            }
        }

        public Color BackgroundColor
        {
            get
            {
                return this.BackgroundColorId != 0 ? Persistence.Connection.Get<Color>(this.BackgroundColorId) : null;
            }
        }

        public Color TextColor
        {
            get
            {
                return this.TextColorId != 0 ? Persistence.Connection.Get<Color>(this.TextColorId) : null;
            }
        }

        //[Ignore]
        //public Space PreviousSpace { get; set; }

        //[Ignore]
        //public Space NextSpace { get; set; }

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

        public Space Clone()
        {
            // use same attributes and Image record
            var space = new Space() { BoardId = this.BoardId, Order = this.Order, Type = this.Type, Width = this.Width,
                Height = this.Height, X = this.X, Y = this.Y, BackgroundColorId = this.BackgroundColorId,
                TextColorId = this.TextColorId, ImageId = this.ImageId };

            // copy any subrecords
            if (this.BibleVerseId != 0)
            {
                space.BibleVerseId = this.BibleVerse.Clone().Id;
            }
            if (this.NameCardId != 0)
            {
                space.NameCardId = this.NameCard.Clone().Id;
            }
            if (this.SafeHavenCardId != 0)
            {
                space.SafeHavenCardId = this.SafeHavenCard.Clone().Id;
            }

            Persistence.Connection.Insert(space);
            return space;
        }
    }
}
