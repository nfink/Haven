using SQLite;

namespace Haven
{
    public class SafeHavenCard : ICloneable<SafeHavenCard>
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

        public SafeHavenCard Clone()
        {
            var safeHavenCard = new SafeHavenCard() { Name = this.Name, Details = this.Details, ImageId = this.ImageId };
            Persistence.Connection.Insert(safeHavenCard);
            return safeHavenCard;
        }
    }
}
