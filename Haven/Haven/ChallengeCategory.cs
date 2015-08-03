using SQLite;

namespace Haven
{
    public class ChallengeCategory : IDeletable, ICloneable<ChallengeCategory>
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string Name { get; set; }

        public int OwnerId { get; set; }

        public void Delete()
        {
            // delete category
            Persistence.Connection.Delete<ChallengeCategory>(this.Id);

            // delete challenges in the category
            Persistence.Connection.Execute("delete from Challenge where ChallengeCategoryId=?", this.Id);

            // remove links from any spaces
            Persistence.Connection.Execute("delete from SpaceChallengeCategory where ChallengeCategoryId=?", this.Id);

            // remove links from boards
            Persistence.Connection.Execute("delete from BoardChallengeCategory where ChallengeCategoryId=?", this.Id);
        }

        public ChallengeCategory Clone()
        {
            var category = new ChallengeCategory() { Name = this.Name, OwnerId = this.OwnerId };
            Persistence.Connection.Insert(category);
            return category;
        }
    }
}
