using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace Haven
{
    public class Board : IDeletable
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public int ImageId { get; set; }

        public bool Active { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public int MessageAreaWidth { get; set; }

        public int MessageAreaHeight { get; set; }

        public int MessageAreaX { get; set; }

        public int MessageAreaY { get; set; }

        public int StatusAreaWidth { get; set; }

        public int StatusAreaHeight { get; set; }

        public int StatusAreaX { get; set; }

        public int StatusAreaY { get; set; }

        public IEnumerable<Space> Spaces
        {
            get
            {
                return Persistence.Connection.Table<Space>().Where(x => x.BoardId == this.Id);
            }
        }

        public IEnumerable<NameCard> NameCards
        {
            get
            {
                return Persistence.Connection.Query<NameCard>(
                @"select NameCard.* from Board
                  join Space on Board.Id=Space.BoardId
                  join NameCard on Space.NameCardId=NameCard.Id
                  where Board.Id=?", this.Id);
            }
        }

        public IEnumerable<SafeHavenCard> SafeHavenCards
        {
            get
            {
                return Persistence.Connection.Query<SafeHavenCard>(
                @"select SafeHavenCard.* from Board
                  join Space on Board.Id=Space.BoardId
                  join SafeHavenCard on Space.SafeHavenCardId=SafeHavenCard.Id
                  where Board.Id=?", this.Id);
            }
        }

        public IEnumerable<Challenge> Challenges
        {
            get
            {
                return Persistence.Connection.Table<Challenge>().Where(x => x.BoardId == this.Id);
            }
        }

        public Image Image
        {
            get
            {
                return this.ImageId == 0 ? null : Persistence.Connection.Get<Image>(this.ImageId);
            }
        }

        public Space GetNewSpace(int startSpaceId, int spacesToMove, bool direction)
        {
            var spaces = this.Spaces.OrderBy(x => x.Order).ToList();
            int directionMultiplier = direction ? 1 : -1;
            int movement = spacesToMove * directionMultiplier;
            int startLocation = spaces.Select(x => x.Id).ToList().IndexOf(startSpaceId);
            int endLocation = (((startLocation + movement) % spaces.Count) + spaces.Count) % spaces.Count;
            return spaces[endLocation];
        }

        public void MovePlayer(Game game, Player player, int spaceId)
        {
            player.SpaceId = spaceId;
            Persistence.Connection.Update(player);
            var space = Persistence.Connection.Get<Space>(player.SpaceId);
            space.OnLand(player);
        }

        public void Delete()
        {
            // delete spaces
            foreach (Space space in this.Spaces)
            {
                space.Delete();
            }

            // delete challenges
            foreach (Challenge challenge in this.Challenges)
            {
                challenge.Delete();
            }

            // delete board
            Persistence.Connection.Delete(this);
        }

        public BoardValidation Validate()
        {
            var spaces = this.Spaces;
            var challenges = this.Challenges;
            var validation = new BoardValidation();

            // validate board
            if (this.Name == null)
            {
                validation.Errors.Add("No name");
            }
            if (this.Description == null)
            {
                validation.Warnings.Add("No description");
            }
            if (this.Image == null)
            {
                validation.Warnings.Add("No icon");
            }

            // validate spaces
            // make sure there are enough spaces
            //int requiredSpaces = (this.Width * 2) + ((this.Height - 1) * 2);
            //if (spaces.Count() < requiredSpaces)
            //{
            //    validation.Errors.Add(string.Format("{0}/{1} required spaces", spaces.Count(), requiredSpaces));
            //}
            // make sure the board has spaces
            if (spaces.Count() < 1)
            {
                validation.Errors.Add("No spaces");
            }
            // make sure the board has enough spaces
            int recommendedNumberOfSpaces = 7;
            if (spaces.Count() < recommendedNumberOfSpaces)
            {
                validation.Warnings.Add(string.Format("{0}/{1} recommended minimum number of spaces", spaces.Count(), recommendedNumberOfSpaces));
            }
            // make sure all spaces have a type
            var typed = spaces.Where(x => x.Type != SpaceType.None);
            if (typed.Count() < spaces.Count())
            {
                validation.Errors.Add(string.Format("{0}/{1} spaces with a type", typed.Count(), spaces.Count()));
            }
            // make sure any challenge space has a corresponding card
            var challengeSpaces = spaces.Where(x => x.Type == SpaceType.Challenge);
            var noChallengeCard = challengeSpaces.Where(x => x.NameCardId == 0);
            if (noChallengeCard.Count() > 0)
            {
                validation.Errors.Add(string.Format("{0}/{1} challenge spaces with a corresponding card", noChallengeCard.Count(), challengeSpaces.Count()));
            }
            // make sure any safe haven space has a corresponding card
            var safeHavenSpaces = spaces.Where(x => x.Type == SpaceType.SafeHaven);
            var noSafeHavenCard = safeHavenSpaces.Where(x => x.SafeHavenCardId == 0);
            if (noSafeHavenCard.Count() > 0)
            {
                validation.Errors.Add(string.Format("{0}/{1} safe haven spaces with a corresponding card", noSafeHavenCard.Count(), safeHavenSpaces.Count()));
            }
            // make sure any recall space has a corresponding verse
            var recallSpaces = spaces.Where(x => x.Type == SpaceType.Recall);
            var noRecall = recallSpaces.Where(x => x.BibleVerseId == 0);
            if (noRecall.Count() > 0)
            {
                validation.Errors.Add(string.Format("{0}/{1} recall spaces with a corresponding card", noRecall.Count(), recallSpaces.Count()));
            }
            // make sure there are no collisions in order
            var sameOrder = spaces.Select(x => x.Order).GroupBy(x => x).SelectMany(x => x.Skip(1));
            if (sameOrder.Count() > 0)
            {
                validation.Errors.Add(string.Format("Multiple spaces with the same order: {0}", sameOrder.Select(x => x.ToString()).Aggregate((x, y) => x + ", " + y)));
            }

            // validate dimensions
            // make sure all dimenions are set
            // make sure there are no overlapping spaces or areas

            // validate challenges
            // make sure every challenge has at least one correct answer
            var challengesWithoutCorrectAnswer = challenges.Where(x => x.Answers.Where(y => y.Correct).Count() < 1);
            if (challengesWithoutCorrectAnswer.Count() > 0)
            {
                validation.Errors.Add(string.Format("{0}/{1} challenges with a correct anwser", challenges.Count() - challengesWithoutCorrectAnswer.Count(), challenges.Count()));
            }
            // warn if there aren't very many challenges
            var recommendedChallenges = 10 + (challengeSpaces.Count() * 5);
            if (challenges.Count() < recommendedChallenges)
            {
                validation.Warnings.Add(string.Format("{0}/{1} recommended minimum number of challenges", challenges.Count(), recommendedChallenges));
            }

            // validate name cards


            // validate safe haven cards


            return validation;
        }
    }


    public class BoardValidation
    {
        public List<string> Errors;

        public List<string> Warnings;

        public BoardValidation()
        {
            this.Errors = new List<string>();
            this.Warnings = new List<string>();
        }
    }
}
