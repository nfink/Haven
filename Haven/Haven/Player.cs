﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace Haven
{
    public class Player : IDeletable
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string Guid { get; set; }

        public string Name { get; set; }

        public string Password { get; set; }

        public int PieceId { get; set; }

        public int ColorId { get; set; }

        public int SpaceId { get; set; }

        public int GameId { get; set; }

        // true = clockwise
        // false = counter-clockwise
        public bool MovementDirection { get; set; }

        public int NextPlayerId { get; set; }

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

        public IEnumerable<Message> Messages
        {
            get
            {
                return Persistence.Connection.Table<Message>().Where(x => x.PlayerId == this.Id);
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

        public static IEnumerable<Action> GetAvailableAction(int playerId)
        {
            return Persistence.Connection.Table<Action>().Where(x => x.OwnerId == playerId);
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

            // update NextPlayerId for other players
            Persistence.Connection.Execute("update Player set NextPlayerId=? where NextPlayerId=?", this.NextPlayerId, this.Id);

            // delete player
            Persistence.Connection.Delete(this);
        }
    }
}
