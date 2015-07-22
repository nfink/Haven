using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Haven;
using Nancy;
using Nancy.Security;
using Nancy.Authentication.Forms;
using Nancy.ModelBinding;
using Nancy.Responses;
using Newtonsoft.Json;

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

            Get["/Boards/{id}/Validation"] = parameters =>
            {
                var userId = int.Parse(this.Context.CurrentUser.UserName);
                var boardId = (int)parameters.id;
                var board = Persistence.Connection.Table<Board>().Where(x => (x.Id == boardId) && (x.OwnerId == userId)).FirstOrDefault();
                if (board != null)
                {
                    return JsonConvert.SerializeObject(board.Validate());
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
                    challenge.Name = (string)this.Request.Form.Name;
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





            Get["/{username}/Boards/{id}"] = parameters =>
            {
                var board = Persistence.Connection.Get<Board>((int)parameters.id);
                return View["Views/Admin/BoardDialog.cshtml", board];
            };

            Get["/{username}/Boards/{id}/Edit"] = parameters =>
            {
                var boardId = (int)this.Request.Query.BoardId;
                var board = Persistence.Connection.Get<Board>(boardId);
                return View["Views/Admin/EditBoard.cshtml", board];
            };

            Post["/{username}/Boards/{id}/Edit"] = parameters =>
            {
                var boardId = (int)this.Request.Form.Id;
                var board = Persistence.Connection.Get<Board>(boardId);
                board.Name = (string)this.Request.Form.Name;
                board.Description = (string)this.Request.Form.Description;
                var imageFile = this.Request.Files.FirstOrDefault();
                var image = this.UpdateImage(pathProvider, board.Image, imageFile);
                if (image != null)
                {
                    board.ImageId = image.Id;
                }

                Persistence.Connection.Update(board);
                return View["Views/Admin/EditBoardDetails.cshtml", board];
            };

            Post["/{username}/Boards"] = parameters =>
            {
                var board = new Board() { Active = false };
                return View["Views/Admin/EditBoard.cshtml", board];
            };

            Delete["/{username}/Boards/{id}"] = parameters =>
            {
                var boardId = (int)this.Request.Query.BoardId;
                var board = Persistence.Connection.Get<Board>(boardId);
                board.Delete();
                return new HtmlResponse(HttpStatusCode.OK);
            };

            Get["/{username}/Spaces/{id}/Edit"] = parameters =>
            {
                var spaceId = (int)this.Request.Query.SpaceId;
                Space space;
                if (spaceId == 0)
                {
                    var boardId = (int)this.Request.Query.BoardId;
                    var x = (int)this.Request.Query.X;
                    var y = (int)this.Request.Query.Y;
                    space = new Space() { BoardId = boardId, X = x, Y = y };
                }
                else
                {
                    space = Persistence.Connection.Get<Space>(spaceId);
                }

                return View["Views/Admin/EditSpace.cshtml", space];
            };

            Post["/{username}/Spaces/{id}/Edit"] = parameters =>
            {
                var space = this.Bind<Space>();

                var imageFile = this.Request.Files.FirstOrDefault();
                Image image = null;

                // clean up any dependent records
                if (space.Id != 0)
                {
                    var existingSpace = Persistence.Connection.Get<Space>(space.Id);
                    if (existingSpace.BibleVerseId != 0)
                    {
                        Persistence.Connection.Delete<BibleVerse>(existingSpace.BibleVerseId);
                    }
                    if (existingSpace.NameCardId != 0)
                    {
                        image = existingSpace.NameCard.Image;
                        Persistence.Connection.Delete<NameCard>(existingSpace.NameCardId);
                    }
                    if (existingSpace.SafeHavenCardId != 0)
                    {
                        image = existingSpace.SafeHavenCard.Image;
                        Persistence.Connection.Delete<SafeHavenCard>(existingSpace.SafeHavenCardId);
                    }
                }

                // add any dependent records
                if (space.Type == SpaceType.Challenge)
                {
                    var nameCard = new NameCard() { Name = (string)this.Request.Form.NameCardName, Details = (string)this.Request.Form.NameCardDetails };
                    image = this.UpdateImage(pathProvider, image, imageFile);
                    if (image != null)
                    {
                        nameCard.ImageId = image.Id;
                    }
                    Persistence.Connection.Insert(nameCard);
                    space.NameCardId = nameCard.Id;
                }
                else if (space.Type == SpaceType.SafeHaven)
                {
                    var safeHavenCard = new SafeHavenCard() { Name = (string)this.Request.Form.SafeHavenCardName, Details = (string)this.Request.Form.SafeHavenCardDetails };
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
                    if (space.Type == SpaceType.Recall)
                    {
                        var recall = new BibleVerse() { Text = (string)this.Request.Form.RecallText };
                        Persistence.Connection.Insert(recall);
                        space.BibleVerseId = recall.Id;

                    }

                    // delete unused image
                    if (image != null)
                    {
                        image.DeleteImage(pathProvider.GetRootPath());
                        Persistence.Connection.Delete(image);
                    }
                }

                // add/update the space
                if (space.Id == 0)
                {
                    Persistence.Connection.Insert(space);
                }
                else
                {
                    Persistence.Connection.Update(space); 
                }

                return View["Views/Space.cshtml", space];
            };

            Delete["/{username}/Spaces/{id}"] = parameters =>
            {
                var space = Persistence.Connection.Get<Space>((int)parameters.id);
                space.Delete();
                return new HtmlResponse(HttpStatusCode.OK);
            };

            Get["/{username}/Challenges/{id}/Edit"] = parameters =>
            {
                var challengeId = (int)this.Request.Query.ChallengeId;
                Challenge challenge;
                if (challengeId == 0)
                {
                    var boardId = (int)this.Request.Query.BoardId;
                    challenge = new Challenge() { BoardId = boardId };
                }
                else
                {
                    challenge = Persistence.Connection.Get<Challenge>(challengeId);
                    
                }

                return View["Views/Admin/EditChallenge.cshtml", challenge];
            };

            Post["/{username}/Challenges/{id}/Edit"] = parameters =>
            {
                var challenge = this.Bind<Challenge>();
                if (challenge.Id == 0)
                {
                    Persistence.Connection.Insert(challenge);
                }
                else
                {
                    Persistence.Connection.Update(challenge);
                    Persistence.Connection.Execute("delete from ChallengeAnswer where ChallengeId=?", challenge.Id);
                }

                var answers = JsonConvert.DeserializeObject<IEnumerable<ChallengeAnswer>>((string)this.Request.Form.Answers);

                foreach(ChallengeAnswer answer in answers)
                {
                    Persistence.Connection.Insert(new ChallengeAnswer() { ChallengeId = challenge.Id, Answer = answer.Answer, Correct = answer.Correct });
                }

                return View["Views/Admin/Challenge.cshtml", challenge];
            };

            Delete["/{username}/Challenges/{id}"] = parameters =>
            {
                var challenge = Persistence.Connection.Get<Challenge>((int)parameters.id);
                challenge.Delete();
                return new HtmlResponse(HttpStatusCode.OK);
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
                    image.DeleteImage(pathProvider.GetRootPath());
                    Persistence.Connection.Delete(image);
                }

                // add the new image
                image = new Image() { Filename = newImage.Name };
                Persistence.Connection.Insert(image);
                image.SaveImage(pathProvider.GetRootPath(), "Uploads", newImage.Value);
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
    }
}