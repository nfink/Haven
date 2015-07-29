using SQLite;

namespace Haven
{
    public class NameCard : ICloneable<NameCard>
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
            var nameCard = new NameCard() { Name = this.Name, Details = this.Details, ImageId = this.ImageId };
            Persistence.Connection.Insert(nameCard);
            return nameCard;
        }
    }
}
