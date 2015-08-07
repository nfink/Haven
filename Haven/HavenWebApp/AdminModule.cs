﻿using Haven;
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
        public AdminModule(IRootPathProvider pathProvider, IRepository repository)
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

            Get["/Games/Active"] = parameters =>
            {
                var userId = int.Parse(this.Context.CurrentUser.UserName);
                return JsonConvert.SerializeObject(Persistence.Connection.Query<Game>("select * from Game where OwnerId=? and Ended=0", userId));
            };

            Get["/Games/Completed"] = parameters =>
            {
                var userId = int.Parse(this.Context.CurrentUser.UserName);
                return JsonConvert.SerializeObject(Persistence.Connection.Query<Game>("select * from Game where OwnerId=? and Ended<>0", userId));
            };

            Post["/Games"] = parameters =>
            {
                using (repository)
                {
                    var game = new Game { Name = (string)this.Request.Form.Name };
                    game.Create((int)this.Request.Form.BoardId, (int)this.Request.Form.NumberOfPlayers);
                    return JsonConvert.SerializeObject(game);
                }
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
                    board.TurnsToEnd = (int)this.Request.Form.TurnsToEnd;
                    board.NameCardsToEnd = (int)this.Request.Form.NameCardsToEnd;
                    board.SafeHavenCardsToEnd = (int)this.Request.Form.SafeHavenCardsToEnd;
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

            Post["/Boards/{id}/ChallengeCategories"] = parameters =>
            {
                var userId = int.Parse(this.Context.CurrentUser.UserName);
                var boardId = (int)parameters.id;
                var board = Persistence.Connection.Table<Board>().Where(x => (x.Id == boardId) && (x.OwnerId == userId)).FirstOrDefault();
                if (board != null)
                {
                    Persistence.Connection.Execute("delete from BoardChallengeCategory where BoardId=?", board.Id);

                    var categoryIds= JsonConvert.DeserializeObject<IEnumerable<int>>((string)this.Request.Form.ChallengeCategories);

                    foreach (int categoryId in categoryIds)
                    {
                        Persistence.Connection.Insert(new BoardChallengeCategory() { BoardId = board.Id, ChallengeCategoryId = categoryId });
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
                    challenge.OpenEnded = (bool)this.Request.Form.OpenEnded;
                    challenge.OwnerId = userId;
                    challenge.ChallengeCategoryId = (int)this.Request.Form.ChallengeCategoryId;
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

                Persistence.Connection.Insert(space);

                // add challenge categories
                if ((space.Type == Haven.SpaceType.Challenge) || (space.Type == Haven.SpaceType.War))
                {
                    var categoryIds = JsonConvert.DeserializeObject<IEnumerable<int>>((string)this.Request.Form.ChallengeCategories);
                    foreach (int categoryId in categoryIds)
                    {
                        Persistence.Connection.Insert(new SpaceChallengeCategory() { ChallengeCategoryId = categoryId, SpaceId = space.Id });
                    }
                }

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
                    }

                    // update challenge categories
                    Persistence.Connection.Execute("delete from SpaceChallengeCategory where SpaceId=?", space.Id);
                    if ((space.Type == Haven.SpaceType.Challenge) || (space.Type == Haven.SpaceType.War))
                    {
                        var categoryIds = JsonConvert.DeserializeObject<IEnumerable<int>>((string)this.Request.Form.ChallengeCategories);
                        foreach (int categoryId in categoryIds)
                        {
                            Persistence.Connection.Insert(new SpaceChallengeCategory() { ChallengeCategoryId = categoryId, SpaceId = space.Id });
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