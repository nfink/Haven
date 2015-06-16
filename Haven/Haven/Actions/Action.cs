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
        AnswerChallenge = 1,
        AnswerWarChallenge,
        DeclareWar,
        DeclineWar,
        EndTurn,
        EnterName,
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

        public int PieceId { get; set; }

        public int PlayerId { get; set; }

        public bool RequiresInput
        {
            get
            {
                return (this.Type == ActionType.EnterName) || (this.Type == ActionType.ReciteBibleVerse);
            }
        }

        public Message PerformAction(Object input)
        {
            switch (this.Type)
            {
                case (ActionType.AnswerChallenge):
                    return AnswerChallengeAction(input);
                case (ActionType.AnswerWarChallenge):
                    return AnswerWarChallengeAction(input);
                case (ActionType.DeclareWar):
                    return DeclareWarAction(input);
                case (ActionType.DeclineWar):
                    return DeclineWarAction(input);
                case (ActionType.EndTurn):
                    return EndTurnAction(input);
                case (ActionType.EnterName):
                    return EnterNameAction(input);
                case (ActionType.ExchangePlaces):
                    return ExchangePlacesAction(input);
                case (ActionType.ReadBibleVerse):
                    return ReadBibleVerseAction(input);
                case (ActionType.ReciteBibleVerse):
                    return ReciteBibleVerseAction(input);
                case (ActionType.Roll):
                    return RollAction(input);
                case (ActionType.RollToGo):
                    return RollToGoAction(input);
                case (ActionType.SelectPiece):
                    return SelectPieceAction(input);
                case (ActionType.TurnAround):
                    return TurnAroundAction(input);
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
