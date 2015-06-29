using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using System.Security.Cryptography;

namespace Haven
{
    public class Player
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string Name { get; set; }

        public string Password { get; set; }

        public int PieceId { get; set; }

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
            // from http://stackoverflow.com/questions/4181198/how-to-hash-a-password
            byte[] salt;
            new RNGCryptoServiceProvider().GetBytes(salt = new byte[16]);
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 1000);
            byte[] hash = pbkdf2.GetBytes(20);
            byte[] hashBytes = new byte[36];
            Array.Copy(salt, 0, hashBytes, 0, 16);
            Array.Copy(hash, 0, hashBytes, 16, 20);
            string savedPasswordHash = Convert.ToBase64String(hashBytes);
            this.Password = savedPasswordHash;
            Persistence.Connection.Update(this);
        }

        public bool VerifyPassword(string password)
        {
            if (password == null || this.Password == null)
            {
                return false;
            }

            // from http://stackoverflow.com/questions/4181198/how-to-hash-a-password
            byte[] hashBytes = Convert.FromBase64String(this.Password);
            byte[] salt = new byte[16];
            Array.Copy(hashBytes, 0, salt, 0, 16);
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 1000);
            byte[] hash = pbkdf2.GetBytes(20);
            return !(hash.Where((x, i) => x != hashBytes[i + 16]).Any());
        }

        public IEnumerable<Message> RecentMessages(int number)
        {
            return Persistence.Connection.Table<Message>().Where(x => x.PlayerId == this.Id).OrderByDescending(x => x.Id).Take(number).OrderBy(x => x.Id);
        }
    }
}
