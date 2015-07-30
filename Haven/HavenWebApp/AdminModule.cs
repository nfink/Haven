using Haven;
using Nancy;
using Nancy.Authentication.Forms;
using Nancy.ModelBinding;
using Nancy.Responses;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HavenWebApp
{
    public class AdminModule : NancyModule
    {
        public AdminModule(IRootPathProvider pathProvider)
        {

            var formsAuthConfiguration =
                new FormsAuthenticationConfiguration()
                {
                    RedirectUrl = "~/Login",
                    UserMapper = new UserMapper(),
                };

            FormsAuthentication.Enable(this, formsAuthConfiguration);

            Get["/"] = parameters =>
            {
                return View["Views/Haven.cshtml"];
            };

            Get["/Pieces"] = parameters =>
            {
                return JsonConvert.SerializeObject(Piece.Pieces);
            };

            Get["/Colors"] = parameters =>
            {
                return JsonConvert.SerializeObject(Color.Colors);
            };

            Get["/SpaceTypes"] = parameters =>
            {
                var types = new List<AdminModule.SpaceType>();
                foreach (Haven.SpaceType type in Enum.GetValues(typeof(Haven.SpaceType)))
                {
                    types.Add(new AdminModule.SpaceType(type));
                }

                return JsonConvert.SerializeObject(types);
            };

            Get["/User"] = parameters =>
            {
                var userId = int.Parse(this.Context.CurrentUser.UserName);
                var user = Persistence.Connection.Get<User>(userId);
                return JsonConvert.SerializeObject(user);
            };

            Put["/User"] = parameters =>
            {
                var userId = int.Parse(this.Context.CurrentUser.UserName);
                var user = Persistence.Connection.Get<User>(userId);
                user.Username = (string)this.Request.Form.Username;
                Persistence.Connection.Update(user);
                return JsonConvert.SerializeObject(user);
            };

            Get["/Games"] = parameters =>
            {
                var userId = int.Parse(this.Context.CurrentUser.UserName);
                return JsonConvert.SerializeObject(Persistence.Connection.Table<Game>().Where(x => x.OwnerId == userId));
            };

            Post["/Games"] = parameters =>
            {
                var game = Game.NewGame((int)this.Request.Form.BoardId, (int)this.Request.Form.NumberOfPlayers);
                game.Name = (string)this.Request.Form.Name;
                Persistence.Connection.Update(game);
                return JsonConvert.SerializeObject(game);
            };

            Get["/Boards"] = parameters =>
            {
                var userId = int.Parse(this.Context.CurrentUser.UserName);
                return JsonConvert.SerializeObject(Persistence.Connection.Table<Board>().Where(x => x.OwnerId == userId));
            };

            Post["/Boards"] = parameters =>
            {
                var board = new Board();
                var userId = int.Parse(this.Context.CurrentUser.UserName);
                board.OwnerId = userId;
                Persistence.Connection.Insert(board);
                return JsonConvert.SerializeObject(board);
            };

            Get["/Boards/{id}"] = parameters =>
            {
                var userId = int.Parse(this.Context.CurrentUser.UserName);
                var boardId = (int)parameters.id;
                var board = Persistence.Connection.Table<Board>().Where(x => (x.Id == boardId) && (x.OwnerId == userId)).FirstOrDefault();
                if (board != null)
                {
                    return JsonConvert.SerializeObject(board);
                }
                else
                {
                    return new HtmlResponse(HttpStatusCode.NotFound);
                }
            };

            Put["/Boards/{id}"] = parameters =>
            {
                var userId = int.Parse(this.Context.CurrentUser.UserName);
                var boardId = (int)parameters.id;
                var board = Persistence.Connection.Table<Board>().Where(x => (x.Id == boardId) && (x.OwnerId == userId)).FirstOrDefault();
                if (board != null)
                {
                    board.Name = (string)this.Request.Form.Name;
                    board.Description = (string)this.Request.Form.Description;
                    var imageFile = this.Request.Files.FirstOrDefault();
                    var image = this.UpdateImage(pathProvider, board.Image, imageFile);
                    if (image != null)
                    {
                        board.ImageId = image.Id;
                    }
                    Persistence.Connection.Update(board);

                    return JsonConvert.SerializeObject(board);
                }
                else
                {
                    return new HtmlResponse(HttpStatusCode.NotFound);
                }
            };

            Delete["/Boards/{id}"] = parameters =>
            {
                var boardId = (int)parameters.id;
                var userId = int.Parse(this.Context.CurrentUser.UserName);
                var board = Persistence.Connection.Table<Board>().Where(x => (x.Id == boardId) && (x.OwnerId == userId)).FirstOrDefault();
                if (board != null)
                {
                    board.Delete();
                    return new HtmlResponse(HttpStatusCode.OK);
                }
                else
                {
                    return new HtmlResponse(HttpStatusCode.NotFound);
                }
            };

            Post["/Boards/{id}/Challenges"] = parameters =>
            {
                var userId = int.Parse(this.Context.CurrentUser.UserName);
                var boardId = (int)parameters.id;
                var board = Persistence.Connection.Table<Board>().Where(x => (x.Id == boardId) && (x.OwnerId == userId)).FirstOrDefault();
                if (board != null)
                {
                    Persistence.Connection.Execute("delete from BoardChallenge where BoardId=?", board.Id);

                    var challenges = JsonConvert.DeserializeObject<IEnumerable<Challenge>>((string)this.Request.Form.Challenges);

                    foreach (Challenge challenge in challenges)
                    {
                        Persistence.Connection.Insert(new BoardChallenge() { BoardId = board.Id, ChallengeId = challenge.Id });
                    }

                    return JsonConvert.SerializeObject(board.Challenges);
                }
                else
                {
                    return new HtmlResponse(HttpStatusCode.NotFound);
                }
            };

            Post["/Boards/{id}/Validate"] = parameters =>
            {
                var userId = int.Parse(this.Context.CurrentUser.UserName);
                var boardId = (int)parameters.id;
                var board = Persistence.Connection.Table<Board>().Where(x => (x.Id == boardId) && (x.OwnerId == userId)).FirstOrDefault();
                if (board != null)
                {
                    var validations = board.Validate();
                    var valid = (validations.Errors.Count < 1);
                    if (board.Active != valid)
                    {
                        board.Active = valid;
                        Persistence.Connection.Update(board);
                    }
                    return JsonConvert.SerializeObject(validations);
                }
                else
                {
                    return new HtmlResponse(HttpStatusCode.NotFound);
                }
            };

            Post["/Boards/{id}/Copy"] = parameters =>
            {
                var userId = int.Parse(this.Context.CurrentUser.UserName);
                var boardId = (int)parameters.id;
                var board = Persistence.Connection.Table<Board>().Where(x => (x.Id == boardId) && (x.OwnerId == userId)).FirstOrDefault();
                if (board != null)
                {
                    board.Name = board.Name + " (copy)";
                    var copiedBoard = board.Copy();
                    return JsonConvert.SerializeObject(copiedBoard);
                }
                else
                {
                    return new HtmlResponse(HttpStatusCode.NotFound);
                }
            };

            Get["/Challenges"] = parameters =>
            {
                var userId = int.Parse(this.Context.CurrentUser.UserName);
                return JsonConvert.SerializeObject(Persistence.Connection.Table<Challenge>().Where(x => x.OwnerId == userId));
            };

            Post["/Challenges"] = parameters =>
            {
                var challenge = this.Bind<Challenge>();
                challenge.OwnerId = int.Parse(this.Context.CurrentUser.UserName);
                challenge.ChallengeCategoryId = this.GetCategoryId((string)this.Request.Form.Category);
                Persistence.Connection.Insert(challenge);

                var answers = JsonConvert.DeserializeObject<IEnumerable<ChallengeAnswer>>((string)this.Request.Form.Answers);

                foreach (ChallengeAnswer answer in answers)
                {
                    Persistence.Connection.Insert(new ChallengeAnswer() { ChallengeId = challenge.Id, Answer = answer.Answer, Correct = answer.Correct });
                }

                return JsonConvert.SerializeObject(challenge);
            };

            Put["/Challenges/{id}"] = parameters =>
            {
                var challengeId = (int)parameters.id;
                var userId = int.Parse(this.Context.CurrentUser.UserName);
                var challenge = Persistence.Connection.Table<Challenge>().Where(x => (x.Id == challengeId) && (x.OwnerId == userId)).FirstOrDefault();
                if (challenge != null)
                {
                    challenge.Question = (string)this.Request.Form.Question;
                    challenge.OwnerId = userId;
                    challenge.ChallengeCategoryId = this.GetCategoryId((string)this.Request.Form.Category);
                    Persistence.Connection.Update(challenge);
                    Persistence.Connection.Execute("delete from ChallengeAnswer where ChallengeId=?", challenge.Id);

                    var answers = JsonConvert.DeserializeObject<IEnumerable<ChallengeAnswer>>((string)this.Request.Form.Answers);

                    foreach (ChallengeAnswer answer in answers)
                    {
                        Persistence.Connection.Insert(new ChallengeAnswer() { ChallengeId = challenge.Id, Answer = answer.Answer, Correct = answer.Correct });
                    }

                    return JsonConvert.SerializeObject(challenge);
                }
                else
                {
                    return new HtmlResponse(HttpStatusCode.NotFound);
                }
            };

            Delete["/Challenges/{id}"] = parameters =>
            {
                var challengeId = (int)parameters.id;
                var userId = int.Parse(this.Context.CurrentUser.UserName);
                var challenge = Persistence.Connection.Table<Challenge>().Where(x => (x.Id == challengeId) && (x.OwnerId == userId)).FirstOrDefault();
                if (challenge != null)
                {
                    challenge.Delete();
                    return new HtmlResponse(HttpStatusCode.OK);
                }
                else
                {
                    return new HtmlResponse(HttpStatusCode.NotFound);
                }
            };

            Get["/ChallengeCategories"] = parameters =>
            {
                var userId = int.Parse(this.Context.CurrentUser.UserName);
                return JsonConvert.SerializeObject(Persistence.Connection.Table<ChallengeCategory>().Where(x => x.OwnerId == userId));
            };

            Post["/ChallengeCategories"] = parameters =>
            {
                var challengeCategory = this.Bind<ChallengeCategory>();
                challengeCategory.OwnerId = int.Parse(this.Context.CurrentUser.UserName);
                Persistence.Connection.Insert(challengeCategory);
                return JsonConvert.SerializeObject(challengeCategory);
            };

            Put["/ChallengeCategories/{id}"] = parameters =>
            {
                var challengeCategoryId = (int)parameters.id;
                var userId = int.Parse(this.Context.CurrentUser.UserName);
                var challengeCategory = Persistence.Connection.Table<ChallengeCategory>().Where(x => (x.Id == challengeCategoryId) && (x.OwnerId == userId)).FirstOrDefault();
                if (challengeCategory != null)
                {
                    challengeCategory.Name = (string)this.Request.Form.Name;
                    Persistence.Connection.Update(challengeCategory);
                    return JsonConvert.SerializeObject(challengeCategory);
                }
                else
                {
                    return new HtmlResponse(HttpStatusCode.NotFound);
                }
            };

            Delete["/ChallengeCategories/{id}"] = parameters =>
            {
                var challengeCategoryId = (int)parameters.id;
                var userId = int.Parse(this.Context.CurrentUser.UserName);
                var challengeCategory = Persistence.Connection.Table<ChallengeCategory>().Where(x => (x.Id == challengeCategoryId) && (x.OwnerId == userId)).FirstOrDefault();
                if (challengeCategory != null)
                {
                    challengeCategory.Delete();
                    return new HtmlResponse(HttpStatusCode.OK);
                }
                else
                {
                    return new HtmlResponse(HttpStatusCode.NotFound);
                }
            };


            Post["/Spaces"] = parameters =>
            {
                var space = this.Bind<Space>();

                var imageFile = this.Request.Files.FirstOrDefault();
                Image image = null;

                // add any dependent records
                if (space.Type == Haven.SpaceType.Challenge)
                {
                    var nameCard = new NameCard() { Name = (string)this.Request.Form.CardName, Details = (string)this.Request.Form.CardDetails };
                    image = this.UpdateImage(pathProvider, image, imageFile);
                    if (image != null)
                    {
                        nameCard.ImageId = image.Id;
                    }
                    Persistence.Connection.Insert(nameCard);
                    space.NameCardId = nameCard.Id;
                }
                else if (space.Type == Haven.SpaceType.SafeHaven)
                {
                    var safeHavenCard = new SafeHavenCard() { Name = (string)this.Request.Form.CardName, Details = (string)this.Request.Form.CardDetails };
                    image = this.UpdateImage(pathProvider, image, imageFile);
                    if (image != null)
                    {
                        safeHavenCard.ImageId = image.Id;
                    }
                    Persistence.Connection.Insert(safeHavenCard);
                    space.SafeHavenCardId = safeHavenCard.Id;
                }
                else
                {
                    if (space.Type == Haven.SpaceType.Recall)
                    {
                        var recall = new BibleVerse() { Text = (string)this.Request.Form.Text };
                        Persistence.Connection.Insert(recall);
                        space.BibleVerseId = recall.Id;

                    }
                }

                Persistence.Connection.Insert(space);

                return JsonConvert.SerializeObject(space);
            };

            Put["/Spaces/{id}"] = parameters =>
            {
                var spaceId = (int)parameters.id;
                var userId = int.Parse(this.Context.CurrentUser.UserName);
                var space = Persistence.Connection.Query<Space>("select Space.* from Space join Board on Space.BoardId=Board.Id where Space.Id=? and Board.OwnerId=?", spaceId, userId).FirstOrDefault();
                if (space != null)
                {
                    // update fields
                    space.Type = (Haven.SpaceType)((int)this.Request.Form.Type);
                    space.Order = (int)this.Request.Form.Order;
                    space.BackgroundColorId = (int)this.Request.Form.BackgroundColorId;
                    space.TextColorId = (int)this.Request.Form.TextColorId;

                    var imageFile = this.Request.Files.FirstOrDefault();

                    // add/update any dependent records
                    if (space.Type == Haven.SpaceType.Challenge)
                    {
                        // delete other records
                        if (space.BibleVerseId != 0)
                        {
                            Persistence.Connection.Delete<BibleVerse>(space.BibleVerseId);
                            space.BibleVerseId = 0;
                        }
                        if (space.SafeHavenCardId != 0)
                        {
                            Persistence.Connection.Delete<SafeHavenCard>(space.SafeHavenCardId);
                            space.SafeHavenCardId = 0;
                        }

                        // add/update name card record
                        NameCard nameCard = space.NameCard;
                        if (nameCard == null)
                        {
                            nameCard = new NameCard() { Name = (string)this.Request.Form.CardName, Details = (string)this.Request.Form.CardDetails };
                            Persistence.Connection.Insert(nameCard);
                        }
                        else
                        {
                            nameCard.Name = (string)this.Request.Form.CardName;
                            nameCard.Details = (string)this.Request.Form.CardDetails;
                        }

                        var image = nameCard.Image;
                        image = this.UpdateImage(pathProvider, image, imageFile);
                        if (image != null)
                        {
                            nameCard.ImageId = image.Id;
                        }

                        Persistence.Connection.Update(nameCard);
                        space.NameCardId = nameCard.Id;
                    }
                    else if (space.Type == Haven.SpaceType.SafeHaven)
                    {
                        // delete other records
                        if (space.BibleVerseId != 0)
                        {
                            Persistence.Connection.Delete<BibleVerse>(space.BibleVerseId);
                            space.BibleVerseId = 0;
                        }
                        if (space.NameCardId != 0)
                        {
                            Persistence.Connection.Delete<NameCard>(space.NameCardId);
                            space.NameCardId = 0;
                        }

                        // add/update safe haven card record
                        SafeHavenCard safeHavenCard = space.SafeHavenCard;
                        if (safeHavenCard == null)
                        {
                            safeHavenCard = new SafeHavenCard() { Name = (string)this.Request.Form.CardName, Details = (string)this.Request.Form.CardDetails };
                            Persistence.Connection.Insert(safeHavenCard);
                        }
                        else
                        {
                            safeHavenCard.Name = (string)this.Request.Form.CardName;
                            safeHavenCard.Details = (string)this.Request.Form.CardDetails;
                        }

                        var image = safeHavenCard.Image;
                        image = this.UpdateImage(pathProvider, image, imageFile);
                        if (image != null)
                        {
                            safeHavenCard.ImageId = image.Id;
                        }

                        Persistence.Connection.Update(safeHavenCard);
                        space.SafeHavenCardId = safeHavenCard.Id;
                    }
                    else
                    {
                        // delete unused records
                        if (space.NameCardId != 0)
                        {
                            if (space.NameCard.Image != null)
                            {
                                space.NameCard.Image.Delete();
                            }
                            Persistence.Connection.Delete<NameCard>(space.NameCardId);
                            space.NameCardId = 0;
                        }
                        if (space.SafeHavenCardId != 0)
                        {
                            if (space.SafeHavenCard.Image != null)
                            {
                                space.SafeHavenCard.Image.Delete();
                            }
                            Persistence.Connection.Delete<SafeHavenCard>(space.SafeHavenCardId);
                            space.SafeHavenCardId = 0;
                        }
                        if (space.BibleVerseId != 0)
                        {
                            Persistence.Connection.Delete<BibleVerse>(space.BibleVerseId);
                            space.BibleVerseId = 0;
                        }

                        // re-create recall record for each edit
                        if (space.Type == Haven.SpaceType.Recall)
                        {
                            var recall = new BibleVerse() { Text = (string)this.Request.Form.RecallText };
                            Persistence.Connection.Insert(recall);
                            space.BibleVerseId = recall.Id;

                        }
                    }

                    Persistence.Connection.Update(space);

                    return JsonConvert.SerializeObject(space);
                }
                else
                {
                    return new HtmlResponse(HttpStatusCode.NotFound);
                }
            };

            Delete["/Spaces/{id}"] = parameters =>
            {
                var spaceId = (int)parameters.id;
                var userId = int.Parse(this.Context.CurrentUser.UserName);
                var space = Persistence.Connection.Query<Space>("select Space.* from Space join Board on Space.BoardId=Board.Id where Space.Id=? and Board.OwnerId=?", spaceId, userId).FirstOrDefault();
                if (space != null)
                {
                    space.Delete();
                    return new HtmlResponse(HttpStatusCode.OK);
                }
                else
                {
                    return new HtmlResponse(HttpStatusCode.NotFound);
                }
            };
        }

        private bool HasNewImage(Image image, HttpFile newImage)
        {
            return ((newImage != null) &&
                (newImage.ContentType.StartsWith("image") &&
                ((image == null) || (image.Filename != newImage.Name))));
        }

        private Image UpdateImage(IRootPathProvider pathProvider, Image image, HttpFile newImage)
        {
            if (this.HasNewImage(image, newImage))
            {
                // delete the image
                if (image != null)
                {
                    image.Delete();
                }

                // add the new image
                image = new Image() { Filename = newImage.Name };
                Persistence.Connection.Insert(image);
                image.SaveImage(newImage.Value);
                Persistence.Connection.Update(image);
            }

            return image;
        }

        private int GetCategoryId(string categoryName)
        {
            if (string.IsNullOrWhiteSpace(categoryName))
            {
                return 0;
            }
            else
            {
                var userId = int.Parse(this.Context.CurrentUser.UserName);
                var category = Persistence.Connection.Table<ChallengeCategory>().Where(x => (x.OwnerId == userId) && (x.Name == categoryName)).FirstOrDefault();
                if (category == null)
                {
                    category = new ChallengeCategory() { Name = categoryName, OwnerId = userId };
                    Persistence.Connection.Insert(category);
                    return category.Id;
                }
                else
                {
                    return category.Id;
                }
            }
        }

        //private T GetObject<T>(int id)
        //{
        //    var userId = int.Parse(this.Context.CurrentUser.UserName);
        //    var matchingObject = Persistence.Connection.Table<T>().Where(x => (x.Id == id) && (x.OwnerId == userId)).FirstOrDefault();
        //    return matchingObject;
        //    //    if (matchingObject != null)
        //    //    {
        //    //        return matchingObject;
        //    //    }
        //    //    else
        //    //    {
        //    //        return new HtmlResponse(HttpStatusCode.NotFound);
        //    //    }
        //}

        private class SpaceType
        {
            public int Id { get; set; }

            public string Name { get; set; }

            public string Description { get; set; }

            public string Icon { get; set; }

            public SpaceType(Haven.SpaceType type)
            {
                this.Id = (int)type;
                this.Name = type.GetName();
                this.Description = type.GetDescription();
                this.Icon = type.GetIcon();
            }
        }
    }
}