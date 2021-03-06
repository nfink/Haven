﻿using Newtonsoft.Json;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Haven
{
    public class Board : IEntity, IDeletable, ICloneable<Board>
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Ignore]
        public IRepository Repository { private get; set; }

        public int OwnerId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public int ImageId { get; set; }

        public bool Active { get; set; }

        public int TurnsToEnd { get; set; }

        public int NameCardsToEnd { get; set; }

        public int SafeHavenCardsToEnd { get; set; }

        public IEnumerable<Space> Spaces
        {
            get
            {
                return this.Repository.Find<Space>(x => x.BoardId == this.Id);
            }
        }

        //public IEnumerable<Space> Spaces
        //{
        //    get
        //    {
        //        var spaces = Persistence.Connection.Table<Space>().Where(x => x.BoardId == this.Id);
        //        var orderedSpaces = spaces.OrderBy(x => x.Order).ToList();
        //        for (int i = 0; i < orderedSpaces.Count; i++)
        //        {
        //            orderedSpaces[i].NextSpace = orderedSpaces[(i + 1) % orderedSpaces.Count];
        //            orderedSpaces[i].PreviousSpace = (i > 0 ? orderedSpaces[(i - 1) % (orderedSpaces.Count - 1)] : orderedSpaces[orderedSpaces.Count - 1]);
                    
        //        }
        //        return orderedSpaces;
        //    }
        //}

        [JsonIgnore]
        public IEnumerable<NameCard> NameCards
        {
            get
            {
                return this.Spaces.Where(x => x.NameCardId != 0).Select(x => x.NameCard);
            }
        }

        [JsonIgnore]
        public IEnumerable<SafeHavenCard> SafeHavenCards
        {
            get
            {
                return this.Spaces.Where(x => x.SafeHavenCardId != 0).Select(x => x.SafeHavenCard);
            }
        }

        [JsonIgnore]
        public IEnumerable<Challenge> Challenges
        {
            get
            {
                return this.ChallengeCategories.SelectMany(x => this.Repository.Find<Challenge>(y => y.ChallengeCategoryId == x.ChallengeCategoryId));
            }
        }

        public IEnumerable<BoardChallengeCategory> ChallengeCategories
        {
            get
            {
                return this.Repository.Find<BoardChallengeCategory>(x => x.BoardId == this.Id);
            }
        }

        public Image Image
        {
            get
            {
                return this.ImageId == 0 ? null : this.Repository.Get<Image>(this.ImageId);
            }
        }

        [JsonIgnore]
        public Space StartingSpace
        {
            get
            {
                var spaces = this.Spaces;
                if (spaces.Count() < 1)
                {
                    return null;
                }
                else
                {
                    return spaces.Aggregate((x, y) => y.Order < x.Order ? y : x);
                }
            }
        }

        public Board()
        {
            this.TurnsToEnd = -1;
            this.NameCardsToEnd = -1;
            this.SafeHavenCardsToEnd = -1;
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

        //public IEnumerable<Space> GetMovementSequence(int startSpaceId, int spacesToMove, bool direction)
        //{
        //    var spaceSequence = new List<Space>();
        //    var spaces = this.Spaces;
        //    var space = spaces.Where(x => x.Id == startSpaceId).First();

        //    for (int i = 0; i < spacesToMove; i++ )
        //    {
        //        if (direction)
        //        {
        //            space = space.NextSpace;
        //        }
        //        else
        //        {
        //            space = space.PreviousSpace;
        //        }
        //        spaceSequence.Add(space);
        //    }

        //    return spaceSequence;
        //}

        public void Delete()
        {
            // delete spaces
            foreach (Space space in this.Spaces.ToList())
            {
                space.Delete();
            }

            // delete challenge links
            foreach (BoardChallengeCategory category in this.ChallengeCategories.ToList())
            {
                this.Repository.Remove(category);
            }

            // delete image
            if (this.ImageId != 0)
            {
                this.Repository.Remove(this.Image);
            }

            // delete board
            this.Repository.Remove(this);
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
                validation.Warnings.Add("No image");
            }

            // validate spaces
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
            // make sure there are no collisions in order
            var sameOrder = spaces.Select(x => x.Order).GroupBy(x => x).SelectMany(x => x.Skip(1));
            if (sameOrder.Count() > 0)
            {
                validation.Errors.Add(string.Format("Multiple spaces with the same order: {0}", sameOrder.Select(x => x.ToString()).Aggregate((x, y) => x + ", " + y)));
            }
            // make sure there are no overlapping spaces
            var sameLocation = spaces.Select(x => new Tuple<int, int>(x.X, x.Y)).GroupBy(x => x).SelectMany(x => x.Skip(1));
            if (sameLocation.Count() > 0)
            {
                validation.Errors.Add(string.Format("Multiple spaces at the same location: {0}", sameLocation.Select(x => string.Format("({0}, {1})", x.Item1, x.Item2)).Aggregate((x, y) => x + ", " + y)));
            }

            // validate challenges
            // make sure every challenge has at least one correct answer
            var challengesWithoutCorrectAnswer = challenges.Where(x => x.Answers.Where(y => y.Correct).Count() < 1);
            if (challengesWithoutCorrectAnswer.Count() > 0)
            {
                validation.Errors.Add(string.Format("{0}/{1} challenges with a correct answer", challenges.Count() - challengesWithoutCorrectAnswer.Count(), challenges.Count()));
            }
            // warn if there aren't very many challenges
            var recommendedChallenges = 10 + (challengeSpaces.Count() * 5);
            if (challenges.Count() < recommendedChallenges)
            {
                validation.Warnings.Add(string.Format("{0}/{1} recommended minimum number of challenges", challenges.Count(), recommendedChallenges));
            }

            // validate name cards
            var nameCards = this.NameCards;

            // warn if there are name cards without a name
            var nameCardsWithName = nameCards.Where(x => !string.IsNullOrWhiteSpace(x.Name));
            if (nameCardsWithName.Count() < nameCards.Count())
            {
                validation.Warnings.Add(string.Format("{0}/{1} challenge cards with a name", nameCardsWithName.Count(), nameCards.Count()));
            }

            // warn if there are name cards without a description
            var nameCardsWithDescription = nameCards.Where(x => !string.IsNullOrWhiteSpace(x.Details));
            if (nameCardsWithDescription.Count() < nameCards.Count())
            {
                validation.Warnings.Add(string.Format("{0}/{1} challenge cards with a description", nameCardsWithDescription.Count(), nameCards.Count()));
            }

            // warn if there are name cards without an image
            var nameCardsWithImage = nameCards.Where(x => x.ImageId != 0);
            if (nameCardsWithImage.Count() < nameCards.Count())
            {
                validation.Warnings.Add(string.Format("{0}/{1} challenge cards with an image", nameCardsWithImage.Count(), nameCards.Count()));
            }

            // validate safe haven cards
            var safeHavenCards = this.SafeHavenCards;

            // warn if there are name cards without a name
            var safeHavenCardsWithName = safeHavenCards.Where(x => !string.IsNullOrWhiteSpace(x.Name));
            if (safeHavenCardsWithName.Count() < safeHavenCards.Count())
            {
                validation.Warnings.Add(string.Format("{0}/{1} safe haven cards with a name", safeHavenCardsWithName.Count(), safeHavenCards.Count()));
            }

            // warn if there are name cards without a description
            var safeHavenCardsWithDescription = safeHavenCards.Where(x => !string.IsNullOrWhiteSpace(x.Details));
            if (safeHavenCardsWithDescription.Count() < safeHavenCards.Count())
            {
                validation.Warnings.Add(string.Format("{0}/{1} safe haven cards with a description", safeHavenCardsWithDescription.Count(), safeHavenCards.Count()));
            }

            // warn if there are name cards without an image
            var safeHavenCardsWithImage = safeHavenCards.Where(x => x.ImageId != 0);
            if (safeHavenCardsWithImage.Count() < safeHavenCards.Count())
            {
                validation.Warnings.Add(string.Format("{0}/{1} safe haven cards with an image", safeHavenCardsWithImage.Count(), safeHavenCards.Count()));
            }

            return validation;
        }

        public Board Clone()
        {
            // use same attributes
            var board = new Board() { Name = this.Name, Description = this.Description, Active = this.Active, OwnerId = this.OwnerId, TurnsToEnd = this.TurnsToEnd, NameCardsToEnd = this.NameCardsToEnd, SafeHavenCardsToEnd = this.SafeHavenCardsToEnd };
            this.Repository.Add(board);

            // clone image
            if (this.ImageId != 0)
            {
                board.ImageId = this.Image.Clone().Id;
            }

            // clone challenge categories
            var categories = new List<int>();
            var boardCategories = this.Challenges.Select(x => x.ChallengeCategoryId);
            categories.AddRange(boardCategories);
            categories.AddRange(this.Spaces.SelectMany(x => x.ChallengeCategories.Select(y => y.ChallengeCategoryId)));

            var categoryIdMap = new Dictionary<int, int>();
            foreach (int categoryId in categories.Distinct())
            {
                var category = this.Repository.Get<ChallengeCategory>(categoryId);
                category.OwnerId = 0;
                var clonedCategory = category.Clone();
                categoryIdMap.Add(categoryId, clonedCategory.Id);
            }

            // clone board category links
            foreach (int categoryId in boardCategories.Distinct().ToList())
            {
                this.Repository.Add(new BoardChallengeCategory() { BoardId = board.Id, ChallengeCategoryId = categoryIdMap[categoryId] });
            }

            // clone spaces
            foreach (Space space in this.Spaces.ToList())
            {
                space.BoardId = board.Id;
                var clonedSpace = space.Clone();
                foreach (SpaceChallengeCategory category in clonedSpace.ChallengeCategories)
                {
                    category.ChallengeCategoryId = categoryIdMap[category.ChallengeCategoryId];
                    this.Repository.Update(category);
                }
            }

            // clone challenges
            foreach (int categoryId in categories.Distinct())
            {
                foreach (Challenge challenge in this.Repository.Find<Challenge>(x => x.ChallengeCategoryId == categoryId).ToList())
                {
                    challenge.OwnerId = 0;
                    challenge.ChallengeCategoryId = categoryIdMap[challenge.ChallengeCategoryId];
                    challenge.Clone();
                }
            }

            this.Repository.Update(board);
            return board;
        }

        /// <summary>
        /// Returns a copy of the board, with copied spaces and challenge links. Does not copy challenges or categories.
        /// </summary>
        /// <returns></returns>
        public Board Copy()
        {
            // use same attributes and Image record
            var board = new Board() { Name = this.Name, Description = this.Description, Active = this.Active, ImageId = this.ImageId, OwnerId = this.OwnerId, TurnsToEnd = this.TurnsToEnd, NameCardsToEnd = this.NameCardsToEnd, SafeHavenCardsToEnd = this.SafeHavenCardsToEnd };
            this.Repository.Add(board);

            // clone spaces
            foreach (Space space in this.Spaces.ToList())
            {
                space.BoardId = board.Id;
                space.Clone();
            }

            // clone challenge links
            foreach (BoardChallengeCategory category in this.ChallengeCategories.ToList())
            {
                this.Repository.Add(new BoardChallengeCategory() { BoardId = board.Id, ChallengeCategoryId = category.ChallengeCategoryId });
            }

            this.Repository.Update(board);
            return board;
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
