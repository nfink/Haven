using Newtonsoft.Json;
using SQLite;
using System;
using System.Collections.Generic;

namespace Haven
{
    public class Player : IDeletable, IEquatable<Player>
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string Guid { get; set; }

        public string Name { get; set; }

        [JsonIgnore]
        public string Password { get; set; }

        public bool PasswordSet
        {
            get
            {
                return this.Password != null;
            }
        }

        public int PieceId { get; set; }

        public int ColorId { get; set; }

        public int SpaceId { get; set; }

        public int GameId { get; set; }

        // true = clockwise
        // false = counter-clockwise
        public bool MovementDirection { get; set; }

        public int TurnOrder { get; set; }

        public Piece Piece
        {
            get
            {
                return this.PieceId == 0 ? null : Persistence.Connection.Get<Piece>(this.PieceId);
            }
        }

        public Color Color
        {
            get
            {
                return this.ColorId == 0 ? null : Persistence.Connection.Get<Color>(this.ColorId);
            }
        }

        public Space Space
        {
            get
            {
                return Persistence.Connection.Get<Space>(this.SpaceId);
            }
        }

        public IEnumerable<Action> Actions
        {
            get
            {
                return Persistence.Connection.Table<Action>().Where(x => x.OwnerId == this.Id);
            }
        }

        //[JsonIgnore]
        //public IEnumerable<Message> Messages
        //{
        //    get
        //    {
        //        return Persistence.Connection.Table<Message>().Where(x => x.PlayerId == this.Id);
        //    }
        //}

        public IEnumerable<Message> Messages
        {
            get
            {
                return this.RecentMessages(10);
            }
        }

        public IEnumerable<NameCard> NameCards
        {
            get
            {
                return Persistence.Connection.Query<NameCard>("select NameCard.* from NameCard join PlayerNameCard on NameCard.Id=PlayerNameCard.NameCardId join Player on PlayerNameCard.PlayerId=Player.Id where Player.Id=?", this.Id);
            }
        }

        public IEnumerable<SafeHavenCard> SafeHavenCards
        {
            get
            {
                return Persistence.Connection.Query<SafeHavenCard>("select SafeHavenCard.* from SafeHavenCard join PlayerSafeHavenCard on SafeHavenCard.Id=PlayerSafeHavenCard.SafeHavenCardId join Player on PlayerSafeHavenCard.PlayerId=Player.Id where Player.Id=?", this.Id);
            }
        }

        public Player()
        {
            this.MovementDirection = true;
        }

        public void Move(int spaceId)
        {
            this.SpaceId = spaceId;
            Persistence.Connection.Update(this);
            var space = Persistence.Connection.Get<Space>(this.SpaceId);
            space.OnLand(this);
        }

        public void SetPassword(string password)
        {
            string savedPasswordHash = Haven.Password.HashPassword(password);
            this.Password = savedPasswordHash;
            Persistence.Connection.Update(this);
        }

        public bool VerifyPassword(string password)
        {
            return Haven.Password.VerifyPassword(this.Password, password);
        }

        public IEnumerable<Message> RecentMessages(int number)
        {
            return Persistence.Connection.Table<Message>().Where(x => x.PlayerId == this.Id).OrderByDescending(x => x.Id).Take(number).OrderBy(x => x.Id);
        }

        public void Delete()
        {
            // delete actions
            Persistence.Connection.Execute("delete from Action where OwnerId=?", this.Id);

            // delete cards
            Persistence.Connection.Execute("delete from PlayerNameCard where PlayerId=?", this.Id);
            Persistence.Connection.Execute("delete from PlayerSafeHavenCard where PlayerId=?", this.Id);

            // delete messages
            Persistence.Connection.Execute("delete from Message where PlayerId=?", this.Id);

            // delete player
            Persistence.Connection.Delete(this);
        }

        public bool Equals(Player other)
        {
            return this.Id == other.Id;
        }

        public override int GetHashCode()
        {
            return this.Id;
        }
    }
}
