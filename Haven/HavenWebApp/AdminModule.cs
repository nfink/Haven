using Haven;
using Nancy;
using Nancy.Authentication.Forms;
using Nancy.ModelBinding;
using Nancy.Responses;
using Nancy.TinyIoc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HavenWebApp
{
    public class AdminModule : NancyModule
    {
        public AdminModule(IRootPathProvider pathProvider, TinyIoCContainer container)
        {
            var formsAuthConfiguration =
                new FormsAuthenticationConfiguration()
                {
                    RedirectUrl = "~/Login",
                    UserMapper = new UserMapper(container.Resolve<IRepository>()),
                };

            FormsAuthentication.Enable(this, formsAuthConfiguration);

            Get["/"] = parameters =>
            {
                return View["Views/Haven.cshtml"];
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
                using (var repository = container.Resolve<IRepository>())
                {
                    var userId = int.Parse(this.Context.CurrentUser.UserName);
                    var user = repository.Get<User>(userId);
                    return JsonConvert.SerializeObject(user);
                }
            };

            Put["/User"] = parameters =>
            {
                using (var repository = container.Resolve<IRepository>())
                {
                    var userId = int.Parse(this.Context.CurrentUser.UserName);
                    var user = repository.Get<User>(userId);
                    user.Username = (string)this.Request.Form.Username;
                    repository.Update(user);
                    return JsonConvert.SerializeObject(user);
                }
            };

            Get["/Games"] = parameters =>
            {
                using (var repository = container.Resolve<IRepository>())
                {
                    var userId = int.Parse(this.Context.CurrentUser.UserName);
                    return JsonConvert.SerializeObject(repository.Find<Game>(x => x.OwnerId == userId));
                }
            };

            Get["/Games/Active"] = parameters =>
            {
                using (var repository = container.Resolve<IRepository>())
                {
                    var userId = int.Parse(this.Context.CurrentUser.UserName);
                    return JsonConvert.SerializeObject(repository.Find<Game>(x => x.OwnerId == userId).Where(x => x.Ended));
                }
            };

            Get["/Games/Completed"] = parameters =>
            {
                using (var repository = container.Resolve<IRepository>())
                {
                    var userId = int.Parse(this.Context.CurrentUser.UserName);
                    return JsonConvert.SerializeObject(repository.Find<Game>(x => x.OwnerId == userId).Where(x => !x.Ended));
                }
            };

            Post["/Games"] = parameters =>
            {
                using (var repository = container.Resolve<IRepository>())
                {
                    var game = new Game { Name = (string)this.Request.Form.Name };
                    game.Repository = repository;
                    game.Create((int)this.Request.Form.BoardId, (int)this.Request.Form.NumberOfPlayers);
                    return JsonConvert.SerializeObject(game);
                }
            };

            Get["/Boards"] = parameters =>
            {
                using (var repository = container.Resolve<IRepository>())
                {
                    var userId = int.Parse(this.Context.CurrentUser.UserName);
                    return JsonConvert.SerializeObject(repository.Find<Board>(x => x.OwnerId == userId));
                }
            };

            Post["/Boards"] = parameters =>
            {
                using (var repository = container.Resolve<IRepository>())
                {
                    var board = new Board();
                    var userId = int.Parse(this.Context.CurrentUser.UserName);
                    board.OwnerId = userId;
                    repository.Add(board);
                    return JsonConvert.SerializeObject(board);
                }
            };

            Get["/Boards/{id}"] = parameters =>
            {
                using (var repository = container.Resolve<IRepository>())
                {
                    var userId = int.Parse(this.Context.CurrentUser.UserName);
                    var boardId = (int)parameters.id;
                    var board = repository.Find<Board>(x => (x.Id == boardId) && (x.OwnerId == userId)).FirstOrDefault();
                    if (board != null)
                    {
                        return JsonConvert.SerializeObject(board);
                    }
                    else
                    {
                        return new HtmlResponse(HttpStatusCode.NotFound);
                    }
                }
            };

            Put["/Boards/{id}"] = parameters =>
            {
                using (var repository = container.Resolve<IRepository>())
                {
                    var userId = int.Parse(this.Context.CurrentUser.UserName);
                    var boardId = (int)parameters.id;
                    var board = repository.Find<Board>(x => (x.Id == boardId) && (x.OwnerId == userId)).FirstOrDefault();
                    if (board != null)
                    {
                        board.Name = (string)this.Request.Form.Name;
                        board.Description = (string)this.Request.Form.Description;
                        board.TurnsToEnd = (int)this.Request.Form.TurnsToEnd;
                        board.NameCardsToEnd = (int)this.Request.Form.NameCardsToEnd;
                        board.SafeHavenCardsToEnd = (int)this.Request.Form.SafeHavenCardsToEnd;
                        var imageFile = this.Request.Files.FirstOrDefault();
                        var image = this.UpdateImage(pathProvider, repository, board.Image, imageFile);
                        if (image != null)
                        {
                            board.ImageId = image.Id;
                        }
                        repository.Update(board);

                        return JsonConvert.SerializeObject(board);
                    }
                    else
                    {
                        return new HtmlResponse(HttpStatusCode.NotFound);
                    }
                }
            };

            Delete["/Boards/{id}"] = parameters =>
            {
                using (var repository = container.Resolve<IRepository>())
                {
                    var boardId = (int)parameters.id;
                    var userId = int.Parse(this.Context.CurrentUser.UserName);
                    var board = repository.Find<Board>(x => (x.Id == boardId) && (x.OwnerId == userId)).FirstOrDefault();
                    if (board != null)
                    {
                        board.Delete();
                        return new HtmlResponse(HttpStatusCode.OK);
                    }
                    else
                    {
                        return new HtmlResponse(HttpStatusCode.NotFound);
                    }
                }
            };

            Post["/Boards/{id}/ChallengeCategories"] = parameters =>
            {
                using (var repository = container.Resolve<IRepository>())
                {
                    var userId = int.Parse(this.Context.CurrentUser.UserName);
                    var boardId = (int)parameters.id;
                    var board = repository.Find<Board>(x => (x.Id == boardId) && (x.OwnerId == userId)).SingleOrDefault();
                    if (board != null)
                    {
                        foreach (BoardChallengeCategory category in board.ChallengeCategories.ToList())
                        {
                            repository.Remove(category);
                        }

                        var categoryIds = JsonConvert.DeserializeObject<IEnumerable<int>>((string)this.Request.Form.ChallengeCategories);

                        foreach (int categoryId in categoryIds)
                        {
                            repository.Add(new BoardChallengeCategory() { BoardId = board.Id, ChallengeCategoryId = categoryId });
                        }

                        return JsonConvert.SerializeObject(board.Challenges);
                    }
                    else
                    {
                        return new HtmlResponse(HttpStatusCode.NotFound);
                    }
                }
            };

            Post["/Boards/{id}/Validate"] = parameters =>
            {
                using (var repository = container.Resolve<IRepository>())
                {
                    var userId = int.Parse(this.Context.CurrentUser.UserName);
                    var boardId = (int)parameters.id;
                    var board = repository.Find<Board>(x => (x.Id == boardId) && (x.OwnerId == userId)).SingleOrDefault();
                    if (board != null)
                    {
                        var validations = board.Validate();
                        var valid = (validations.Errors.Count < 1);
                        if (board.Active != valid)
                        {
                            board.Active = valid;
                            repository.Update(board);
                        }
                        return JsonConvert.SerializeObject(validations);
                    }
                    else
                    {
                        return new HtmlResponse(HttpStatusCode.NotFound);
                    }
                }
            };

            Post["/Boards/{id}/Copy"] = parameters =>
            {
                using (var repository = container.Resolve<IRepository>())
                {
                    var userId = int.Parse(this.Context.CurrentUser.UserName);
                    var boardId = (int)parameters.id;
                    var board = repository.Find<Board>(x => (x.Id == boardId) && (x.OwnerId == userId)).SingleOrDefault();
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
                }
            };

            Get["/Challenges"] = parameters =>
            {
                using (var repository = container.Resolve<IRepository>())
                {
                    var userId = int.Parse(this.Context.CurrentUser.UserName);
                    return JsonConvert.SerializeObject(repository.Find<Challenge>(x => x.OwnerId == userId));
                }
            };

            Post["/Challenges"] = parameters =>
            {
                using (var repository = container.Resolve<IRepository>())
                {
                    var challenge = this.Bind<Challenge>();
                    challenge.OwnerId = int.Parse(this.Context.CurrentUser.UserName);
                    repository.Add(challenge);

                    var answers = JsonConvert.DeserializeObject<IEnumerable<ChallengeAnswer>>((string)this.Request.Form.Answers);

                    foreach (ChallengeAnswer answer in answers)
                    {
                        repository.Add(new ChallengeAnswer() { ChallengeId = challenge.Id, Answer = answer.Answer, Correct = answer.Correct });
                    }

                    return JsonConvert.SerializeObject(challenge);
                }
            };

            Put["/Challenges/{id}"] = parameters =>
            {
                using (var repository = container.Resolve<IRepository>())
                {
                    var challengeId = (int)parameters.id;
                    var userId = int.Parse(this.Context.CurrentUser.UserName);
                    var challenge = repository.Find<Challenge>(x => (x.Id == challengeId) && (x.OwnerId == userId)).SingleOrDefault();
                    if (challenge != null)
                    {
                        challenge.Question = (string)this.Request.Form.Question;
                        challenge.OpenEnded = (bool)this.Request.Form.OpenEnded;
                        challenge.OwnerId = userId;
                        challenge.ChallengeCategoryId = (int)this.Request.Form.ChallengeCategoryId;
                        repository.Update(challenge);
                        foreach (ChallengeAnswer answer in challenge.Answers.ToList())
                        {
                            repository.Remove(answer);
                        }

                        var answers = JsonConvert.DeserializeObject<IEnumerable<ChallengeAnswer>>((string)this.Request.Form.Answers);

                        foreach (ChallengeAnswer answer in answers)
                        {
                            repository.Add(new ChallengeAnswer() { ChallengeId = challenge.Id, Answer = answer.Answer, Correct = answer.Correct });
                        }

                        return JsonConvert.SerializeObject(challenge);
                    }
                    else
                    {
                        return new HtmlResponse(HttpStatusCode.NotFound);
                    }
                }
            };

            Delete["/Challenges/{id}"] = parameters =>
            {
                using (var repository = container.Resolve<IRepository>())
                {
                    var challengeId = (int)parameters.id;
                    var userId = int.Parse(this.Context.CurrentUser.UserName);
                    var challenge = repository.Find<Challenge>(x => (x.Id == challengeId) && (x.OwnerId == userId)).SingleOrDefault();
                    if (challenge != null)
                    {
                        challenge.Delete();
                        return new HtmlResponse(HttpStatusCode.OK);
                    }
                    else
                    {
                        return new HtmlResponse(HttpStatusCode.NotFound);
                    }
                }
            };

            Get["/ChallengeCategories"] = parameters =>
            {
                using (var repository = container.Resolve<IRepository>())
                {
                    var userId = int.Parse(this.Context.CurrentUser.UserName);
                    return JsonConvert.SerializeObject(repository.Find<ChallengeCategory>(x => x.OwnerId == userId));
                }
            };

            Post["/ChallengeCategories"] = parameters =>
            {
                using (var repository = container.Resolve<IRepository>())
                {
                    var challengeCategory = this.Bind<ChallengeCategory>();
                    challengeCategory.OwnerId = int.Parse(this.Context.CurrentUser.UserName);
                    repository.Add(challengeCategory);
                    return JsonConvert.SerializeObject(challengeCategory);
                }
            };

            Put["/ChallengeCategories/{id}"] = parameters =>
            {
                using (var repository = container.Resolve<IRepository>())
                {
                    var challengeCategoryId = (int)parameters.id;
                    var userId = int.Parse(this.Context.CurrentUser.UserName);
                    var challengeCategory = repository.Find<ChallengeCategory>(x => (x.Id == challengeCategoryId) && (x.OwnerId == userId)).SingleOrDefault();
                    if (challengeCategory != null)
                    {
                        challengeCategory.Name = (string)this.Request.Form.Name;
                        repository.Update(challengeCategory);
                        return JsonConvert.SerializeObject(challengeCategory);
                    }
                    else
                    {
                        return new HtmlResponse(HttpStatusCode.NotFound);
                    }
                }
            };

            Delete["/ChallengeCategories/{id}"] = parameters =>
            {
                using (var repository = container.Resolve<IRepository>())
                {
                    var challengeCategoryId = (int)parameters.id;
                    var userId = int.Parse(this.Context.CurrentUser.UserName);
                    var challengeCategory = repository.Find<ChallengeCategory>(x => (x.Id == challengeCategoryId) && (x.OwnerId == userId)).SingleOrDefault();
                    if (challengeCategory != null)
                    {
                        challengeCategory.Delete();
                        return new HtmlResponse(HttpStatusCode.OK);
                    }
                    else
                    {
                        return new HtmlResponse(HttpStatusCode.NotFound);
                    }
                }
            };


            Post["/Spaces"] = parameters =>
            {
                using (var repository = container.Resolve<IRepository>())
                {
                    var space = this.Bind<Space>();

                    var imageFile = this.Request.Files.FirstOrDefault();
                    Image image = null;

                    // add any dependent records
                    if (space.Type == Haven.SpaceType.Challenge)
                    {
                        var nameCard = new NameCard() { Name = (string)this.Request.Form.CardName, Details = (string)this.Request.Form.CardDetails };
                        image = this.UpdateImage(pathProvider, repository, image, imageFile);
                        if (image != null)
                        {
                            nameCard.ImageId = image.Id;
                        }
                        repository.Add(nameCard);
                        space.NameCardId = nameCard.Id;
                    }
                    else if (space.Type == Haven.SpaceType.SafeHaven)
                    {
                        var safeHavenCard = new SafeHavenCard() { Name = (string)this.Request.Form.CardName, Details = (string)this.Request.Form.CardDetails };
                        image = this.UpdateImage(pathProvider, repository, image, imageFile);
                        if (image != null)
                        {
                            safeHavenCard.ImageId = image.Id;
                        }
                        repository.Add(safeHavenCard);
                        space.SafeHavenCardId = safeHavenCard.Id;
                    }

                    repository.Add(space);

                    // add challenge categories
                    if ((space.Type == Haven.SpaceType.Challenge) || (space.Type == Haven.SpaceType.War))
                    {
                        var categoryIds = JsonConvert.DeserializeObject<IEnumerable<int>>((string)this.Request.Form.ChallengeCategories);
                        foreach (int categoryId in categoryIds)
                        {
                            repository.Add(new SpaceChallengeCategory() { ChallengeCategoryId = categoryId, SpaceId = space.Id });
                        }
                    }

                    return JsonConvert.SerializeObject(space);
                }
            };

            Put["/Spaces/{id}"] = parameters =>
            {
                using (var repository = container.Resolve<IRepository>())
                {
                    var spaceId = (int)parameters.id;
                    var userId = int.Parse(this.Context.CurrentUser.UserName);
                    var space = repository.Find<Space>(x => x.Id == spaceId).Where(x => repository.Find<Board>(y => y.OwnerId == userId && y.Id == x.BoardId).Count() > 0).SingleOrDefault();
                    if (space != null)
                    {
                        // update fields
                        space.Type = (Haven.SpaceType)((int)this.Request.Form.Type);
                        space.Order = (int)this.Request.Form.Order;
                        space.IconId = (int)this.Request.Form.IconId;
                        space.BackgroundColorId = (int)this.Request.Form.BackgroundColorId;
                        space.TextColorId = (int)this.Request.Form.TextColorId;

                        var imageFile = this.Request.Files.FirstOrDefault();

                        // add/update any dependent records
                        if (space.Type == Haven.SpaceType.Challenge)
                        {
                            // delete other records
                            if (space.SafeHavenCardId != 0)
                            {
                                repository.Remove(space.SafeHavenCard);
                                space.SafeHavenCardId = 0;
                            }

                            // add/update name card record
                            NameCard nameCard = space.NameCard;
                            if (nameCard == null)
                            {
                                nameCard = new NameCard() { Name = (string)this.Request.Form.CardName, Details = (string)this.Request.Form.CardDetails };
                                repository.Add(nameCard);
                            }
                            else
                            {
                                nameCard.Name = (string)this.Request.Form.CardName;
                                nameCard.Details = (string)this.Request.Form.CardDetails;
                            }

                            var image = nameCard.Image;
                            image = this.UpdateImage(pathProvider, repository, image, imageFile);
                            if (image != null)
                            {
                                nameCard.ImageId = image.Id;
                            }

                            repository.Update(nameCard);
                            space.NameCardId = nameCard.Id;
                        }
                        else if (space.Type == Haven.SpaceType.SafeHaven)
                        {
                            // delete other records
                            if (space.NameCardId != 0)
                            {
                                repository.Remove(space.NameCard);
                                space.NameCardId = 0;
                            }

                            // add/update safe haven card record
                            SafeHavenCard safeHavenCard = space.SafeHavenCard;
                            if (safeHavenCard == null)
                            {
                                safeHavenCard = new SafeHavenCard() { Name = (string)this.Request.Form.CardName, Details = (string)this.Request.Form.CardDetails };
                                repository.Add(safeHavenCard);
                            }
                            else
                            {
                                safeHavenCard.Name = (string)this.Request.Form.CardName;
                                safeHavenCard.Details = (string)this.Request.Form.CardDetails;
                            }

                            var image = safeHavenCard.Image;
                            image = this.UpdateImage(pathProvider, repository, image, imageFile);
                            if (image != null)
                            {
                                safeHavenCard.ImageId = image.Id;
                            }

                            repository.Update(safeHavenCard);
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
                                repository.Remove(space.NameCard);
                                space.NameCardId = 0;
                            }
                            if (space.SafeHavenCardId != 0)
                            {
                                if (space.SafeHavenCard.Image != null)
                                {
                                    space.SafeHavenCard.Image.Delete();
                                }
                                repository.Remove(space.SafeHavenCard);
                                space.SafeHavenCardId = 0;
                            }
                        }

                        // update challenge categories
                        foreach (SpaceChallengeCategory category in space.ChallengeCategories.ToList())
                        {
                            repository.Remove(category);
                        }

                        if ((space.Type == Haven.SpaceType.Challenge) || (space.Type == Haven.SpaceType.War))
                        {
                            var categoryIds = JsonConvert.DeserializeObject<IEnumerable<int>>((string)this.Request.Form.ChallengeCategories);
                            foreach (int categoryId in categoryIds)
                            {
                                repository.Add(new SpaceChallengeCategory() { ChallengeCategoryId = categoryId, SpaceId = space.Id });
                            }
                        }

                        repository.Update(space);

                        return JsonConvert.SerializeObject(space);
                    }
                    else
                    {
                        return new HtmlResponse(HttpStatusCode.NotFound);
                    }
                }
            };

            Delete["/Spaces/{id}"] = parameters =>
            {
                using (var repository = container.Resolve<IRepository>())
                {
                    var spaceId = (int)parameters.id;
                    var userId = int.Parse(this.Context.CurrentUser.UserName);
                    var space = repository.Find<Space>(x => x.Id == spaceId).Where(x => repository.Find<Board>(y => y.OwnerId == userId && y.Id == x.BoardId).Count() > 0).SingleOrDefault();
                    if (space != null)
                    {
                        space.Delete();
                        return new HtmlResponse(HttpStatusCode.OK);
                    }
                    else
                    {
                        return new HtmlResponse(HttpStatusCode.NotFound);
                    }
                }
            };
        }

        private bool HasNewImage(Image image, HttpFile newImage)
        {
            return ((newImage != null) &&
                (newImage.ContentType.StartsWith("image") &&
                ((image == null) || (image.Filename != newImage.Name))));
        }

        private Image UpdateImage(IRootPathProvider pathProvider, IRepository repository, Image image, HttpFile newImage)
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
                repository.Add(image);
                image.SaveImage(newImage.Value);
                repository.Update(image);
            }

            return image;
        }

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