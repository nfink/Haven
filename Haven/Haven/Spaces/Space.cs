using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Haven
{
    public partial class Space : IEntity, IDeletable, ICloneable<Space>
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Ignore]
        public IRepository Repository { private get; set; }

        public int BoardId { get; set; }

        public int Order { get; set; }

        public SpaceType Type { get; set; }

        public int NameCardId { get; set; }

        public NameCard NameCard
        {
            get
            {
                return this.NameCardId == 0 ? null : this.Repository.Get<NameCard>(this.NameCardId);
            }
        }

        public int SafeHavenCardId { get; set; }

        public SafeHavenCard SafeHavenCard
        {
            get
            {
                return this.SafeHavenCardId == 0 ? null : this.Repository.Get<SafeHavenCard>(this.SafeHavenCardId);
            }
        }

        public int Width { get; set; }

        public int Height { get; set; }

        public int X { get; set; }

        public int Y { get; set; }

        public int BackgroundColorId { get; set; }

        public int TextColorId { get; set; }

        public int IconId { get; set; }

        public string Name
        {
            get
            {
                switch (this.Type)
                {
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
                    return this.Repository.Get<Image>(this.ImageId).Filepath;
                }
            }
        }

        public Piece Icon
        {
            get
            {
                if (this.IconId != 0)
                {
                    return this.Repository.Get<Piece>(this.IconId);
                }
                else
                {
                    return new Piece() { Image = this.Type.GetIcon() };
                }
            }
        }

        public Color BackgroundColor
        {
            get
            {
                return this.BackgroundColorId == 0 ? null : this.Repository.Get<Color>(this.BackgroundColorId);
            }
        }

        public Color TextColor
        {
            get
            {
                return this.TextColorId == 0 ? null : this.Repository.Get<Color>(this.TextColorId);
            }
        }

        public IEnumerable<SpaceChallengeCategory> ChallengeCategories
        {
            get
            {
                return this.Repository.Find<SpaceChallengeCategory>(x => x.SpaceId == this.Id);
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
            if (this.NameCardId != 0)
            {
                this.Repository.Remove(this.NameCard);
            }

            if (this.SafeHavenCardId != 0)
            {
                this.Repository.Remove(this.SafeHavenCard);
            }

            foreach (SpaceChallengeCategory category in this.ChallengeCategories.ToList())
            {
                this.Repository.Remove(category);
            }

            // delete space
            this.Repository.Remove(this);
        }

        public Space Clone()
        {
            // use same attributes and Image record
            var space = new Space() { BoardId = this.BoardId, Order = this.Order, Type = this.Type, Width = this.Width,
                Height = this.Height, X = this.X, Y = this.Y, BackgroundColorId = this.BackgroundColorId,
                TextColorId = this.TextColorId, ImageId = this.ImageId };

            // copy any subrecords
            if (this.NameCardId != 0)
            {
                space.NameCardId = this.NameCard.Clone().Id;
            }
            if (this.SafeHavenCardId != 0)
            {
                space.SafeHavenCardId = this.SafeHavenCard.Clone().Id;
            }

            this.Repository.Add(space);
            
            // add categories
            foreach (SpaceChallengeCategory category in this.ChallengeCategories.ToList())
            {
                this.Repository.Add(new SpaceChallengeCategory() { SpaceId = space.Id, ChallengeCategoryId = category.ChallengeCategoryId });
            }

            return space;
        }
    }
}
