using SQLite;
using System;

namespace Haven
{
    public class NameCard : ICloneable<NameCard>, IEquatable<NameCard>
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string Name { get; set; }

        public string Details { get; set; }

        public int ImageId { get; set; }

        public Image Image
        {
            get
            {
                return this.ImageId == 0 ? null : Persistence.Connection.Get<Image>(this.ImageId);
            }
        }

        public NameCard Clone()
        {
            int imageId = 0;
            if (this.ImageId != 0)
            {
                imageId = this.Image.Clone().Id;
            }

            var nameCard = new NameCard() { Name = this.Name, Details = this.Details, ImageId = imageId };
            Persistence.Connection.Insert(nameCard);
            return nameCard;
        }

        public bool Equals(NameCard other)
        {
            return this.Id == other.Id;
        }
    }
}
