using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace Haven
{
    public enum SpaceType
    {
        None = 0,
        Challenge,
        ExchangePlaces,
        OptionalTurnAround,
        RollToGo,
        SafeHaven,
        TurnAround,
        War,
    }

    public static class SpaceTypeExtensions
    {
        public static string GetName(this SpaceType type)
        {
            switch (type)
            {
                case SpaceType.Challenge:
                    return "Challenge";
                case SpaceType.ExchangePlaces:
                    return "Exchange Places";
                case SpaceType.OptionalTurnAround:
                    return "Turn Around?";
                case SpaceType.RollToGo:
                    return "Roll Again";
                case SpaceType.SafeHaven:
                    return "Safe Haven";
                case SpaceType.TurnAround:
                    return "Turn Around";
                case SpaceType.War:
                    return "War";
                default:
                    return type.ToString();
            }
        }

        public static string GetDescription(this SpaceType type)
        {
            switch (type)
            {
                case SpaceType.Challenge:
                    return "Player landing here must answer one of the challenge questions. If correct, the player is rewarded with a challenge card.";
                case SpaceType.ExchangePlaces:
                    return "Player landing here must exchange places with another player of their choice.";
                case SpaceType.OptionalTurnAround:
                    return "Player landing here may choose to change direction, moving around the board in the opposite direction.";
                case SpaceType.RollToGo:
                    return "Player landing here must roll again. An even number and the player moves forward that number of spaces. An odd number and the player moves backward that number of spaces.";
                case SpaceType.SafeHaven:
                    return "Player landing here is safe from war. If the player has collected all of the challenge cards, they are rewarded with a safe haven card.";
                case SpaceType.TurnAround:
                    return "Player landing here must change direction, moving around the board in the opposite direction.";
                case SpaceType.War:
                    return "Player landing here may choose to challenge another player to war, or decline and remain neutral. If another player is on the War space, the player may not remain neutral. Players on a safe haven may not be challenged.";
                default:
                    return null;
            }
        }

        public static string GetIcon(this SpaceType type)
        {
            switch (type)
            {
                case SpaceType.Challenge:
                    return "mif-question";
                case SpaceType.ExchangePlaces:
                    return "mif-loop2";
                case SpaceType.OptionalTurnAround:
                    return "mif-tab";
                case SpaceType.RollToGo:
                    return "mif-dice";
                case SpaceType.SafeHaven:
                    return "mif-home";
                case SpaceType.TurnAround:
                    return "mif-undo";
                case SpaceType.War:
                    return "mif-fire";
                default:
                    return null;
            }
        }
    }
}
