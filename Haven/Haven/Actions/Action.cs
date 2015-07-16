using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace Haven
{
    public enum ActionType
    {
        None = 0,
        AnswerChallenge,
        AnswerWarChallenge,
        DeclareWar,
        DeclineWar,
        EndTurn,
        EnterName,
        EnterPassword,
        ExchangePlaces,
        ReadBibleVerse,
        ReciteBibleVerse,
        Roll,
        RollToGo,
        SelectPiece,
        TurnAround,
    }

    public partial class Action
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public int OwnerId { get; set; }

        public ActionType Type { get; set; }

        public string Name
        {
            get
            {
                return this.ToString();
            }
        }

        public int AnswerId { get; set; }

        public int BibleVerseId { get; set; }

        public bool Challenger { get; set; }

        public int NameCardId { get; set; }

        public int PlayerId { get; set; }

        public bool RequiresInput
        {
            get
            {
                return (this.Type == ActionType.EnterName) || 
                    (this.Type == ActionType.ReciteBibleVerse) || 
                    (this.Type == ActionType.EnterPassword) ||
                    (this.Type == ActionType.SelectPiece);
            }
        }

        public ChallengeAnswer Answer
        {
            get
            {
                return this.AnswerId == 0 ? null : Persistence.Connection.Get<ChallengeAnswer>(this.AnswerId);
            }
        }

        public BibleVerse BibleVerse
        {
            get
            {
                return this.BibleVerseId == 0 ? null : Persistence.Connection.Get<BibleVerse>(this.BibleVerseId);
            }
        }

        public NameCard NameCard
        {
            get
            {
                return this.NameCardId == 0 ? null : Persistence.Connection.Get<NameCard>(this.NameCardId);
            }
        }

        public Player Player
        {
            get
            {
                return this.PlayerId == 0 ? null : Persistence.Connection.Get<Player>(this.PlayerId);
            }
        }

        public string Text
        {
            get
            {
                switch (this.Type)
                {
                    case ActionType.AnswerChallenge:
                        return this.Answer.Answer;
                    case ActionType.AnswerWarChallenge:
                        return this.Answer.Answer;
                    case ActionType.DeclareWar:
                        return "Declare War";
                    case ActionType.DeclineWar:
                        return "No War";
                    case ActionType.EndTurn:
                        return "End Turn";
                    case ActionType.EnterName:
                        return "Your name";
                    case ActionType.EnterPassword:
                        return "Your password";
                    case ActionType.ExchangePlaces:
                        return "Exchange Places";
                    case ActionType.ReadBibleVerse:
                        return "Read";
                    case ActionType.ReciteBibleVerse:
                        return "Enter bible verse";
                    case ActionType.Roll:
                        return "Roll";
                    case ActionType.RollToGo:
                        return "Roll To Go";
                    case ActionType.SelectPiece:
                        return "Select Piece";
                    case ActionType.TurnAround:
                        return "Turn Around";
                    default:
                        return null;
                }
            }
        }

        public string Icon
        {
            get
            {
                switch (this.Type)
                {
                    case ActionType.AnswerChallenge:
                        return "mif-question";
                    case ActionType.AnswerWarChallenge:
                        return "mif-question";
                    case ActionType.DeclareWar:
                        return this.Player.Piece.Image;
                    case ActionType.DeclineWar:
                        return "mif-florist";
                    case ActionType.EndTurn:
                        return "mif-blocked";
                    case ActionType.ExchangePlaces:
                        return this.Player.Piece.Image;
                    case ActionType.ReadBibleVerse:
                        return "mif-books";
                    case ActionType.Roll:
                        return "mif-dice";
                    case ActionType.RollToGo:
                        return "mif-dice";
                    case ActionType.TurnAround:
                        return "mif-undo";
                    default:
                        return null;
                }
            }
        }

        public string Color
        {
            get
            {
                switch (this.Type)
                {
                    case ActionType.DeclareWar:
                        return this.Player.Color.Name;
                    case ActionType.ExchangePlaces:
                        return this.Player.Color.Name;
                    default:
                        return "mauve";
                }
            }
        }

        public void PerformAction(Object input)
        {
            switch (this.Type)
            {
                case (ActionType.AnswerChallenge):
                    AnswerChallengeAction(input);
                    break;
                case (ActionType.AnswerWarChallenge):
                    AnswerWarChallengeAction(input);
                    break;
                case (ActionType.DeclareWar):
                    DeclareWarAction(input);
                    break;
                case (ActionType.DeclineWar):
                    DeclineWarAction(input);
                    break;
                case (ActionType.EndTurn):
                    EndTurnAction(input);
                    break;
                case (ActionType.EnterName):
                    EnterNameAction(input);
                    break;
                case (ActionType.EnterPassword):
                    EnterPasswordAction(input);
                    break;
                case (ActionType.ExchangePlaces):
                    ExchangePlacesAction(input);
                    break;
                case (ActionType.ReadBibleVerse):
                    ReadRecallAction(input);
                    break;
                case (ActionType.ReciteBibleVerse):
                    ReciteRecallAction(input);
                    break;
                case (ActionType.Roll):
                    RollAction(input);
                    break;
                case (ActionType.RollToGo):
                    RollToGoAction(input);
                    break;
                case (ActionType.SelectPiece):
                    SelectPieceAction(input);
                    break;
                case (ActionType.TurnAround):
                    TurnAroundAction(input);
                    break;
                default:
                    throw new Exception("Action has no Type");
            }
        }

        public override string ToString()
        {
            return this.Type.ToString();
        }
    }
}
