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
        public AdminModule(IRootPathProvider pathProvider) : base("/Admin")
        {

            var formsAuthConfiguration =
                new FormsAuthenticationConfiguration()
                {
                    RedirectUrl = "~/Admin/Login",
                    UserMapper = new UserMapper(),
                };

            FormsAuthentication.Enable(this, formsAuthConfiguration);

            Get["/"] = parameters =>
            {
                return View["Views/Admin/Home.cshtml", Persistence.Connection.Table<Board>()];
            };

            Get["/Board/{id}"] = parameters =>
            {
                var board = Persistence.Connection.Get<Board>((int)parameters.id);
                return View["Views/Admin/BoardDialog.cshtml", board];
            };

            Get["/Board/Edit"] = parameters =>
            {
                var boardId = (int)this.Request.Query.BoardId;
                var board = Persistence.Connection.Get<Board>(boardId);
                return View["Views/Admin/EditBoard.cshtml", board];
            };

            Post["/Board/Edit"] = parameters =>
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

            Post["/Board/New"] = parameters =>
            {
                var board = new Board() { Active = false };
                return View["Views/Admin/EditBoard.cshtml", board];
            };

            Delete["/Board"] = parameters =>
            {
                var boardId = (int)this.Request.Query.BoardId;
                var board = Persistence.Connection.Get<Board>(boardId);
                board.Delete();
                return new HtmlResponse(HttpStatusCode.OK);
            };

            Get["/Space/Edit"] = parameters =>
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

            Post["/Space/Edit"] = parameters =>
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

            Delete["/Space/{id}"] = parameters =>
            {
                var space = Persistence.Connection.Get<Space>((int)parameters.id);
                space.Delete();
                return new HtmlResponse(HttpStatusCode.OK);
            };

            Get["/Challenge/Edit"] = parameters =>
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

            Post["/Challenge/Edit"] = parameters =>
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

            Delete["/Challenge/{id}"] = parameters =>
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
                image.DeleteImage(pathProvider.GetRootPath());
                Persistence.Connection.Delete(image);

                // add the new image
                image = new Image() { Filename = newImage.Name };
                Persistence.Connection.Insert(image);
                image.SaveImage(pathProvider.GetRootPath(), "Uploads", newImage.Value);
                Persistence.Connection.Update(image);
            }

            return image;
        }
    }
}