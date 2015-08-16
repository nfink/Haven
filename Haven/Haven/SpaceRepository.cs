using System.Linq;

namespace Haven
{
    public class SpaceRepository
    {
        private IRepository Repository;

        public SpaceRepository(IRepository repository)
        {
            this.Repository = repository;
        }

        public void Create(Space space, NameCard nameCard = null, SafeHavenCard safeHavenCard = null, Image image = null)
        {
            int imageId = 0;
            if (image != null)
            {
                this.Repository.Add<Image>(image);
                imageId = image.Id;
            }

            // add any dependent items
            if (space.Type == Haven.SpaceType.Challenge)
            {
                if (nameCard != null)
                {
                    // add name card
                    nameCard.ImageId = image.Id;
                    this.Repository.Add<NameCard>(nameCard);
                    space.NameCardId = nameCard.Id;
                }
                else
                {
                    // set space image
                    space.ImageId = image.Id;
                }
            }
            else if (space.Type == Haven.SpaceType.SafeHaven)
            {
                if (safeHavenCard != null)
                {
                    // add safe haven card
                    safeHavenCard.ImageId = image.Id;
                    this.Repository.Add<SafeHavenCard>(safeHavenCard);
                    space.SafeHavenCardId = nameCard.Id;
                }
                else
                {
                    // set space image
                    space.ImageId = image.Id;
                }
            }
            else
            {
                space.ImageId = image.Id;
            }

            this.Repository.Add<Space>(space);

            //// add challenge categories
            //if ((space.Type == Haven.SpaceType.Challenge) || (space.Type == Haven.SpaceType.War))
            //{
            //    foreach (SpaceChallengeCategory category in space.ChallengeCategories)
            //    {
            //        category.SpaceId = space.Id;
            //        category.ChallengeCategoryId = category.ChallengeCategory.Id;
            //        this.Repository.Add<SpaceChallengeCategory>(category);
            //    }
            //}
        }

        public void Delete(Space space)
        {
            // remove any dependent records
            if (space.NameCardId != 0)
            {
                this.Repository.Remove<NameCard>(new NameCard { Id = space.NameCardId });
            }
            if (space.SafeHavenCardId != 0)
            {
                this.Repository.Remove<SafeHavenCard>(new SafeHavenCard { Id = space.SafeHavenCardId });
            }
            foreach (SpaceChallengeCategory category in this.Repository.Find<SpaceChallengeCategory>(x => x.SpaceId == space.Id))
            {
                this.Repository.Remove<SpaceChallengeCategory>(category);
            }

            // remove space
            this.Repository.Remove<Space>(space);
        }

        public void Update(Space space)
        {
            var originalSpace = this.Repository.Find<Space>(x => x.Id == space.Id).SingleOrDefault();
            if (originalSpace != null)
            {
                // add/update any dependent records



            }

            //    // add/update any dependent records
            //    if (space.Type == Haven.SpaceType.Challenge)
            //    {
            //        // delete other records
            //        if (originalSpace.SafeHavenCard != null)
            //        {
            //            if (originalSpace.SafeHavenCard.Image != null)
            //            {
            //                originalSpace.SafeHavenCard.Image.Delete();
            //                this.Repository.Remove<Image>(originalSpace.SafeHavenCard.Image);
            //            }

            //            this.Repository.Remove<SafeHavenCard>(originalSpace.SafeHavenCard);
            //        }

            //        // add/update name card record
            //        if (originalSpace.NameCard == null)
            //        {
            //            // add image and name card
            //            this.Repository.Add<Image>(space.NameCard.Image);
            //            space.NameCard.ImageId = space.NameCard.Image.Id;
            //            this.Repository.Add<NameCard>(space.NameCard);
            //        }
            //        else
            //        {
            //            // if new image != old image, remove old image and add new image
            //            if (originalSpace.NameCard.ImageId != space.NameCard.ImageId)
            //            {

            //            }

            //            this.Repository.Update<NameCard>(space.NameCard);
            //        }

            //        var image = nameCard.Image;
            //        image = this.UpdateImage(pathProvider, image, imageFile);
            //        if (space.NameCard.Image != null)
            //        {

            //            nameCard.ImageId = image.Id;
            //        }

            //        this.Repository.Update(nameCard);
            //        space.NameCardId = nameCard.Id;
            //    }
            //    else if (space.Type == Haven.SpaceType.SafeHaven)
            //    {
            //        // delete other records
            //        if (space.NameCardId != 0)
            //        {
            //            this.Repository.Remove<NameCard>(space.NameCardId);
            //            space.NameCardId = 0;
            //        }

            //        // add/update safe haven card record
            //        SafeHavenCard safeHavenCard = space.SafeHavenCard;
            //        if (safeHavenCard == null)
            //        {
            //            safeHavenCard = new SafeHavenCard() { Name = (string)this.Request.Form.CardName, Details = (string)this.Request.Form.CardDetails };
            //            this.Repository.Add(safeHavenCard);
            //        }
            //        else
            //        {
            //            safeHavenCard.Name = (string)this.Request.Form.CardName;
            //            safeHavenCard.Details = (string)this.Request.Form.CardDetails;
            //        }

            //        var image = safeHavenCard.Image;
            //        image = this.UpdateImage(pathProvider, image, imageFile);
            //        if (image != null)
            //        {
            //            safeHavenCard.ImageId = image.Id;
            //        }

            //        this.Repository.Update(safeHavenCard);
            //        space.SafeHavenCardId = safeHavenCard.Id;
            //    }
            //    else
            //    {
            //        // delete unused records
            //        if (space.NameCardId != 0)
            //        {
            //            if (space.NameCard.Image != null)
            //            {
            //                space.NameCard.Image.Delete();
            //            }
            //            this.Repository.Remove<NameCard>(space.NameCardId);
            //            space.NameCardId = 0;
            //        }
            //        if (space.SafeHavenCardId != 0)
            //        {
            //            if (space.SafeHavenCard.Image != null)
            //            {
            //                space.SafeHavenCard.Image.Delete();
            //            }
            //            this.Repository.Remove<SafeHavenCard>(space.SafeHavenCardId);
            //            space.SafeHavenCardId = 0;
            //        }
            //    }

            //    // update challenge categories
            //    Persistence.Connection.Execute("delete from SpaceChallengeCategory where SpaceId=?", space.Id);
            //    if ((space.Type == Haven.SpaceType.Challenge) || (space.Type == Haven.SpaceType.War))
            //    {
            //        var categoryIds = JsonConvert.DeserializeObject<IEnumerable<int>>((string)this.Request.Form.ChallengeCategories);
            //        foreach (int categoryId in categoryIds)
            //        {
            //            this.Repository.Add(new SpaceChallengeCategory() { ChallengeCategoryId = categoryId, SpaceId = space.Id });
            //        }
            //    }

            //    this.Repository.Update(space);

            //    return JsonConvert.SerializeObject(space);
        }

        public Space Clone(Space space)
        {
            //// use same attributes and Image record
            //var space = new Space()
            //{
            //    Board = this.Board,
            //    Order = this.Order,
            //    Type = this.Type,
            //    Width = this.Width,
            //    Height = this.Height,
            //    X = this.X,
            //    Y = this.Y,
            //    BackgroundColor = this.BackgroundColor,
            //    TextColor = this.TextColor,
            //    Picture = this.Picture
            //};

            //// copy any subrecords
            //if (this.NameCard != null)
            //{
            //    space.NameCard = this.NameCard.Clone();
            //}
            //if (this.SafeHavenCard != null)
            //{
            //    space.SafeHavenCard = this.SafeHavenCard.Clone();
            //}

            ////this.Repository.Add(space);

            //// add categories
            //var categories = new List<SpaceChallengeCategory>();
            //foreach (SpaceChallengeCategory category in this.ChallengeCategories)
            //{
            //    categories.Add(new SpaceChallengeCategory() { Space = space, ChallengeCategory = category.ChallengeCategory });
            //    //this.Repository.Add(new SpaceChallengeCategory() { SpaceId = space.Id, ChallengeCategoryId = category.ChallengeCategoryId });
            //}

            //return space;


        //            public NameCard Clone()
        //{
        //    int imageId = 0;
        //    if (this.ImageId != 0)
        //    {
        //        imageId = this.Image.Clone().Id;
        //    }

        //    var nameCard = new NameCard() { Name = this.Name, Details = this.Details, ImageId = imageId };
        //    TinyIoCContainer.Current.Resolve<IRepository>().Add<NameCard>(nameCard);
        //    //var nameCard = new NameCard() { Name = this.Name, Details = this.Details, Image = (this.Image != null ? this.Image.Clone() : null) };
        //    return nameCard;
        //}

            return null;
        }
    }
}
