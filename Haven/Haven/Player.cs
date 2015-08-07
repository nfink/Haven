using Newtonsoft.Json;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Haven
{
    public class Player : IDeletable, IEquatable<Player>, IEntity
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public IRepository Repository { private get; set; }

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
                return this.PieceId == 0 ? null : this.Repository.Get<Piece>(this.PieceId);
            }
        }

        public Color Color
        {
            get
            {
                return this.ColorId == 0 ? null : this.Repository.Get<Color>(this.ColorId);
            }
        }

        public Space Space
        {
            get
            {
                return this.Repository.Get<Space>(this.SpaceId);
            }
        }

        public IEnumerable<Action> Actions
        {
            get
            {
                return this.Repository.Find<Action>(x => x.OwnerId == this.Id);
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
            this.Repository.Update(this);
            var space = this.Repository.Get<Space>(this.SpaceId);
            space.OnLand(this);
        }

        public void SetPassword(string password)
        {
            string savedPasswordHash = Haven.Password.HashPassword(password);
            this.Password = savedPasswordHash;
            this.Repository.Update(this);
        }

        public bool VerifyPassword(string password)
        {
            return Haven.Password.VerifyPassword(this.Password, password);
        }

        public IEnumerable<Message> RecentMessages(int number)
        {
            return this.Repository.Find<Message>(x => x.PlayerId == this.Id).OrderByDescending(x => x.Id).Take(number).OrderBy(x => x.Id);
        }

        public void Delete()
        {
            // delete actions
            foreach (Action action in this.Actions.ToList())
            {
                this.Repository.Remove(action);
            }

            // delete cards
            foreach (NameCard nameCard in this.NameCards.ToList())
            {
                this.Repository.Remove(nameCard);
            }
            foreach (SafeHavenCard safeHavenCard in this.SafeHavenCards.ToList())
            {
                this.Repository.Remove(safeHavenCard);
            }

            // delete messages
            foreach (Message message in this.Repository.Find<Message>(x => x.PlayerId == this.Id))
            {
                this.Repository.Remove(message);
            }

            // delete player
            this.Repository.Remove(this);
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
