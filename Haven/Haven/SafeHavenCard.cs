using SQLite;

namespace Haven
{
    public class SafeHavenCard : ICloneable<SafeHavenCard>, IEntity
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Ignore]
        public IRepository Repository { private get; set; }

        public string Name { get; set; }

        public string Details { get; set; }

        public int ImageId { get; set; }

        public Image Image
        {
            get
            {
                return this.ImageId == 0 ? null : this.Repository.Get<Image>(this.ImageId);
            }
        }

        public SafeHavenCard Clone()
        {
            int imageId = 0;
            if (this.ImageId != 0)
            {
                imageId = this.Image.Clone().Id;
            }

            var safeHavenCard = new SafeHavenCard() { Name = this.Name, Details = this.Details, ImageId = imageId };
            this.Repository.Add<SafeHavenCard>(safeHavenCard);
            return safeHavenCard;
        }
    }
}
