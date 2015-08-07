using SQLite;
using System.Linq;

namespace Haven
{
    public class ChallengeCategory : IEntity, IDeletable, ICloneable<ChallengeCategory>
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public IRepository Repository { private get; set; }

        public string Name { get; set; }

        public int OwnerId { get; set; }

        public void Delete()
        {
            // delete challenges in the category
            foreach (Challenge challenge in this.Repository.Find<Challenge>(x => x.ChallengeCategoryId == this.Id).ToList())
            {
                this.Repository.Remove(challenge);
            }

            // remove links from any spaces
            foreach (SpaceChallengeCategory link in this.Repository.Find<SpaceChallengeCategory>(x => x.ChallengeCategoryId == this.Id).ToList())
            {
                this.Repository.Remove(link);
            }

            // remove links from boards
            foreach (BoardChallengeCategory link in this.Repository.Find<BoardChallengeCategory>(x => x.ChallengeCategoryId == this.Id).ToList())
            {
                this.Repository.Remove(link);
            }

            // delete category
            this.Repository.Remove(this);
        }

        public ChallengeCategory Clone()
        {
            var category = new ChallengeCategory() { Name = this.Name, OwnerId = this.OwnerId };
            this.Repository.Add(category);
            return category;
        }
    }
}
