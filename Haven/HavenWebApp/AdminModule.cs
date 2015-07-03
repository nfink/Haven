using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Haven;
using Nancy;
using Nancy.Security;
using Nancy.Authentication.Forms;
using Nancy.ModelBinding;
using Newtonsoft.Json;

namespace HavenWebApp
{
    public class AdminModule : NancyModule
    {
        public AdminModule() : base("/Admin")
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

                // only set image if it is valid and has changed
                var imageFile = this.Request.Files.FirstOrDefault();
                
                if ((imageFile != null) && (imageFile.ContentType.StartsWith("image") && ((board.Image == null) || (board.Image.Filename != imageFile.Name))))
                {
                    // delete the image
                    board.Image.DeleteImage();
                    Persistence.Connection.Delete(board.Image);

                    // add the new image
                    var image = new Image() { Filename = imageFile.Name };
                    Persistence.Connection.Insert(image);
                    image.SaveImage("/Uploads", imageFile.Value);
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

                // clean up any dependent records
                if (space.Id != 0)
                {
                    var existingSpace = Persistence.Connection.Get<Space>(space.Id);
                    if (existingSpace.BibleVerseId != 0)
                    {
                        Persistence.Connection.Execute("delete from BibleVerse where Id=?", existingSpace.BibleVerseId);
                    }
                    if (existingSpace.NameCardId != 0)
                    {
                        Persistence.Connection.Execute("delete from NameCard where Id=?", existingSpace.NameCardId);
                    }
                    if (existingSpace.SafeHavenCardId != 0)
                    {
                        Persistence.Connection.Execute("delete from SafeHavenCard where Id=?", existingSpace.SafeHavenCardId);
                    }
                }

                // add any dependent records
                if (space.Type == SpaceType.Recall)
                {
                    var recall = JsonConvert.DeserializeObject<BibleVerse>((string)this.Request.Form.Recall);
                    Persistence.Connection.Insert(recall);
                    space.BibleVerseId = recall.Id;

                }
                else if (space.Type == SpaceType.Challenge)
                {
                    var nameCard = JsonConvert.DeserializeObject<NameCard>((string)this.Request.Form.NameCard);
                    Persistence.Connection.Insert(nameCard);
                    space.NameCardId = nameCard.Id;
                }
                else if (space.Type == SpaceType.SafeHaven)
                {
                    var safeHavenCard = JsonConvert.DeserializeObject<SafeHavenCard>((string)this.Request.Form.SafeHavenCard);
                    Persistence.Connection.Insert(safeHavenCard);
                    space.SafeHavenCardId = safeHavenCard.Id;
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
        }
    }
}