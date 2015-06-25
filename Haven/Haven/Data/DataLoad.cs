using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using System.IO;

namespace Haven.Data
{
    public class DataLoad
    {
        public SQLiteConnection Connection;

        public DataLoad()
        {
            // var path = Path.Combine(Environment., "Data/SafeHavenGame.sqlite");
            this.Connection = new SQLiteConnection("C:\\Users\\Nolan\\OneDrive\\Code\\Haven\\Haven\\Haven\\bin\\Debug\\Data\\SafeHavenGame.sqlite");
        }

        public void LoadTables()
        {
            this.LoadPiece();
            this.LoadNameCard();
            this.LoadSafeHavenCard();
            this.LoadBibleVerse();
            this.LoadChallenge();
            this.LoadChallengeAnswer();
            this.LoadSpace();
            this.LoadBoard();
            this.LoadMessage();
            this.LoadPlayer();
            this.LoadGame();
            this.Connection.CreateTable<Action>();
            this.Connection.CreateTable<UsedChallenge>();
            this.Connection.CreateTable<PlayerNameCard>();
            this.Connection.CreateTable<PlayerSafeHavenCard>();
            this.Connection.Close();
        }

        public void LoadBibleVerse()
        {
            this.Connection.CreateTable<BibleVerse>();
            this.Connection.Insert(new BibleVerse() { Id = 1, Book = "Genesis", Chapter = 1, Verse = 1, Text = "a" });
            this.Connection.Insert(new BibleVerse() { Id = 2, Book = "Genesis", Chapter = 4, Verse = 9, Text = "b" });
            this.Connection.Insert(new BibleVerse() { Id = 3, Book = "Genesis", Chapter = 6, Verse = 8, Text = "c" });
            this.Connection.Insert(new BibleVerse() { Id = 4, Book = "Genesis", Chapter = 50, Verse = 25, Text = "d" });
        }

        public void LoadBoard()
        {
            this.Connection.CreateTable<Board>();
            this.Connection.Insert(new Board() { Id = 1, Name = "Genesis", Description = "Covers the book of Genesis", Icon = "Content/Rembrandt_Harmensz_square.jpg",
                MessageAreaWidth = 7, MessageAreaHeight = 3, MessageAreaX = 2, MessageAreaY = 2,
                StatusAreaWidth = 7, StatusAreaHeight = 4, StatusAreaX = 2, StatusAreaY = 5});
        }

        public void LoadChallenge()
        {
            this.Connection.CreateTable<Challenge>();
            this.Connection.Insert(new Challenge() { Id = 1, Question = "Test Question 1", BoardId = 1 });
            this.Connection.Insert(new Challenge() { Id = 2, Question = "Test Question 2", BoardId = 1 });
            this.Connection.Insert(new Challenge() { Id = 3, Question = "Test Question 3", BoardId = 1 });
        }

        public void LoadChallengeAnswer()
        {
            this.Connection.CreateTable<ChallengeAnswer>();
            this.Connection.Insert(new ChallengeAnswer() { Answer = "Test Answer 1a", Correct = false, ChallengeId = 1 });
            this.Connection.Insert(new ChallengeAnswer() { Answer = "Test Answer 1b", Correct = false, ChallengeId = 1 });
            this.Connection.Insert(new ChallengeAnswer() { Answer = "Test Answer 1c", Correct = false, ChallengeId = 1 });
            this.Connection.Insert(new ChallengeAnswer() { Answer = "Test Answer 1d", Correct = true, ChallengeId = 1 });
            this.Connection.Insert(new ChallengeAnswer() { Answer = "Test Answer 2a", Correct = false, ChallengeId = 2 });
            this.Connection.Insert(new ChallengeAnswer() { Answer = "Test Answer 2b", Correct = false, ChallengeId = 2 });
            this.Connection.Insert(new ChallengeAnswer() { Answer = "Test Answer 2c", Correct = true, ChallengeId = 2 });
            this.Connection.Insert(new ChallengeAnswer() { Answer = "Test Answer 2d", Correct = false, ChallengeId = 2 });
            this.Connection.Insert(new ChallengeAnswer() { Answer = "Test Answer 3a", Correct = false, ChallengeId = 3 });
            this.Connection.Insert(new ChallengeAnswer() { Answer = "Test Answer 3b", Correct = true, ChallengeId = 3 });
            this.Connection.Insert(new ChallengeAnswer() { Answer = "Test Answer 3c", Correct = false, ChallengeId = 3 });
            this.Connection.Insert(new ChallengeAnswer() { Answer = "Test Answer 3d", Correct = false, ChallengeId = 3 });
        }

        public void LoadGame()
        {
            this.Connection.CreateTable<Game>();
        }

        public void LoadNameCard()
        {
            this.Connection.CreateTable<NameCard>();
            this.Connection.Insert(new NameCard() { Id = 1, Name = "Judah" });
            this.Connection.Insert(new NameCard() { Id = 2, Name = "Issachar" });
            this.Connection.Insert(new NameCard() { Id = 3, Name = "Zebulun" });
            this.Connection.Insert(new NameCard() { Id = 4, Name = "Reuben" });
            this.Connection.Insert(new NameCard() { Id = 5, Name = "Simeon" });
            this.Connection.Insert(new NameCard() { Id = 6, Name = "Gad" });
            this.Connection.Insert(new NameCard() { Id = 7, Name = "Joseph" });
            this.Connection.Insert(new NameCard() { Id = 8, Name = "Levi" });
            this.Connection.Insert(new NameCard() { Id = 9, Name = "Benjamin" });
            this.Connection.Insert(new NameCard() { Id = 10, Name = "Dan" });
            this.Connection.Insert(new NameCard() { Id = 11, Name = "Asher" });
            this.Connection.Insert(new NameCard() { Id = 12, Name = "Naphtali" });
        }

        public void LoadMessage()
        {
            this.Connection.CreateTable<Message>();
        }

        public void LoadPiece()
        {
            this.Connection.CreateTable<Piece>();
            this.Connection.Insert(new Piece() { Name = "Square", BoardId = 1, Image = "bug", Color = "cobalt" });
            this.Connection.Insert(new Piece() { Name = "Circle", BoardId = 1, Image = "rocket", Color = "amber" });
            this.Connection.Insert(new Piece() { Name = "Triangle", BoardId = 1, Image = "truck", Color = "magenta" });
            this.Connection.Insert(new Piece() { Name = "Pentagon", BoardId = 1, Image = "local-airport", Color = "emerald" });
        }

        public void LoadPlayer()
        {
            this.Connection.CreateTable<Player>();
        }

        public void LoadSafeHavenCard()
        {
            this.Connection.CreateTable<SafeHavenCard>();
            this.Connection.Insert(new SafeHavenCard() { Id = 1, Name = "God", Details = "The Father, creator of the universe." });
            this.Connection.Insert(new SafeHavenCard() { Id = 2, Name = "Abraham", Details = "The founding father of the Jewish nation of Israel, he was a man of great faith and obedience to the will of God." });
            this.Connection.Insert(new SafeHavenCard() { Id = 3, Name = "Isaac", Details = "Only child of Abraham and Sarah. Miraculously born after Sarah healed of being barren." });
            this.Connection.Insert(new SafeHavenCard() { Id = 4, Name = "Jacob", Details = "Son of Isaac, father of the twelve sons who form the tribes of Israel." });
        }

        public void LoadSpace()
        {
            this.Connection.CreateTable<Space>();
            this.Connection.Insert(new Space() { BoardId = 1, Order = 1, Type = SpaceType.BibleVerse, BibleVerseId = 1, Width = 100, Height = 100, X = 150, Y = 50, BackgroundColor = "#E1CFC5", BorderColor="#997F70" });
            this.Connection.Insert(new Space() { BoardId = 1, Order = 9, Type = SpaceType.BibleVerse, BibleVerseId = 2, Width = 100, Height = 100, X = 850, Y = 150, BackgroundColor = "#E1CFC5", BorderColor="#997F70" });
            this.Connection.Insert(new Space() { BoardId = 1, Order = 17, Type = SpaceType.BibleVerse, BibleVerseId = 3, Width = 100, Height = 100, X = 750, Y = 850, BackgroundColor = "#E1CFC5", BorderColor="#997F70" });
            this.Connection.Insert(new Space() { BoardId = 1, Order = 25, Type = SpaceType.BibleVerse, BibleVerseId = 4, Width = 100, Height = 100, X = 50, Y = 750, BackgroundColor = "#E1CFC5", BorderColor="#997F70" });

            this.Connection.Insert(new Space() { BoardId = 1, Order = 6, Type = SpaceType.ExchangePlaces, Width = 100, Height = 100, X = 650, Y = 50, BackgroundColor = "#E1CFC5", BorderColor="#997F70" });
            this.Connection.Insert(new Space() { BoardId = 1, Order = 14, Type = SpaceType.ExchangePlaces, Width = 100, Height = 100, X = 850, Y = 650, BackgroundColor = "#E1CFC5", BorderColor="#997F70" });

            this.Connection.Insert(new Space() { BoardId = 1, Order = 3, Type = SpaceType.OptionalTurnAround, Width = 100, Height = 100, X = 350, Y = 50, BackgroundColor = "#E1CFC5", BorderColor="#997F70" });
            this.Connection.Insert(new Space() { BoardId = 1, Order = 19, Type = SpaceType.OptionalTurnAround, Width = 100, Height = 100, X = 550, Y = 850, BackgroundColor = "#E1CFC5", BorderColor="#997F70" });

            this.Connection.Insert(new Space() { BoardId = 1, Order = 11, Type = SpaceType.RollToGo, Width = 100, Height = 100, X = 850, Y = 350, BackgroundColor = "#E1CFC5", BorderColor="#997F70" });
            this.Connection.Insert(new Space() { BoardId = 1, Order = 27, Type = SpaceType.RollToGo, Width = 100, Height = 100, X = 50, Y = 550, BackgroundColor = "#E1CFC5", BorderColor="#997F70" });

            this.Connection.Insert(new Space() { BoardId = 1, Order = 0, Type = SpaceType.SafeHaven, SafeHavenCardId = 1, Width = 100, Height = 100, X = 50, Y = 50, BackgroundColor = "#E1CFC5", BorderColor="#997F70" });
            this.Connection.Insert(new Space() { BoardId = 1, Order = 8, Type = SpaceType.SafeHaven, SafeHavenCardId = 2, Width = 100, Height = 100, X = 850, Y = 50, BackgroundColor = "#E1CFC5", BorderColor="#997F70" });
            this.Connection.Insert(new Space() { BoardId = 1, Order = 16, Type = SpaceType.SafeHaven, SafeHavenCardId = 3, Width = 100, Height = 100, X = 850, Y = 850, BackgroundColor = "#E1CFC5", BorderColor="#997F70" });
            this.Connection.Insert(new Space() { BoardId = 1, Order = 24, Type = SpaceType.SafeHaven, SafeHavenCardId = 4, Width = 100, Height = 100, X = 50, Y = 850, BackgroundColor = "#E1CFC5", BorderColor="#997F70" });

            this.Connection.Insert(new Space() { BoardId = 1, Order = 2, Type = SpaceType.Challenge, NameCardId = 1, Width = 100, Height = 100, X = 250, Y = 50, BackgroundColor = "#E1CFC5", BorderColor="#997F70" });
            this.Connection.Insert(new Space() { BoardId = 1, Order = 4, Type = SpaceType.Challenge, NameCardId = 2, Width = 100, Height = 100, X = 450, Y = 50, BackgroundColor = "#E1CFC5", BorderColor="#997F70" });
            this.Connection.Insert(new Space() { BoardId = 1, Order = 5, Type = SpaceType.Challenge, NameCardId = 3, Width = 100, Height = 100, X = 550, Y = 50, BackgroundColor = "#E1CFC5", BorderColor="#997F70" });
            this.Connection.Insert(new Space() { BoardId = 1, Order = 10, Type = SpaceType.Challenge, NameCardId = 4, Width = 100, Height = 100, X = 850, Y = 250, BackgroundColor = "#E1CFC5", BorderColor="#997F70" });
            this.Connection.Insert(new Space() { BoardId = 1, Order = 12, Type = SpaceType.Challenge, NameCardId = 5, Width = 100, Height = 100, X = 850, Y = 450, BackgroundColor = "#E1CFC5", BorderColor="#997F70" });
            this.Connection.Insert(new Space() { BoardId = 1, Order = 13, Type = SpaceType.Challenge, NameCardId = 6, Width = 100, Height = 100, X = 850, Y = 550, BackgroundColor = "#E1CFC5", BorderColor="#997F70" });
            this.Connection.Insert(new Space() { BoardId = 1, Order = 18, Type = SpaceType.Challenge, NameCardId = 7, Width = 100, Height = 100, X = 650, Y = 850, BackgroundColor = "#E1CFC5", BorderColor="#997F70" });
            this.Connection.Insert(new Space() { BoardId = 1, Order = 20, Type = SpaceType.Challenge, NameCardId = 8, Width = 100, Height = 100, X = 450, Y = 850, BackgroundColor = "#E1CFC5", BorderColor="#997F70" });
            this.Connection.Insert(new Space() { BoardId = 1, Order = 21, Type = SpaceType.Challenge, NameCardId = 9, Width = 100, Height = 100, X = 350, Y = 850, BackgroundColor = "#E1CFC5", BorderColor="#997F70" });
            this.Connection.Insert(new Space() { BoardId = 1, Order = 26, Type = SpaceType.Challenge, NameCardId = 10, Width = 100, Height = 100, X = 50, Y = 650, BackgroundColor = "#E1CFC5", BorderColor="#997F70" });
            this.Connection.Insert(new Space() { BoardId = 1, Order = 28, Type = SpaceType.Challenge, NameCardId = 11, Width = 100, Height = 100, X = 50, Y = 450, BackgroundColor = "#E1CFC5", BorderColor="#997F70" });
            this.Connection.Insert(new Space() { BoardId = 1, Order = 29, Type = SpaceType.Challenge, NameCardId = 12, Width = 100, Height = 100, X = 50, Y = 350, BackgroundColor = "#E1CFC5", BorderColor="#997F70" });

            this.Connection.Insert(new Space() { BoardId = 1, Order = 22, Type = SpaceType.TurnAround, Width = 100, Height = 100, X = 250, Y = 850, BackgroundColor = "#E1CFC5", BorderColor="#997F70" });
            this.Connection.Insert(new Space() { BoardId = 1, Order = 30, Type = SpaceType.TurnAround, Width = 100, Height = 100, X = 50, Y = 250, BackgroundColor = "#E1CFC5", BorderColor="#997F70" });

            this.Connection.Insert(new Space() { BoardId = 1, Order = 7, Type = SpaceType.War, Width = 100, Height = 100, X = 750, Y = 50, BackgroundColor = "#E1CFC5", BorderColor="#997F70" });
            this.Connection.Insert(new Space() { BoardId = 1, Order = 15, Type = SpaceType.War, Width = 100, Height = 100, X = 850, Y = 750, BackgroundColor = "#E1CFC5", BorderColor="#997F70" });
            this.Connection.Insert(new Space() { BoardId = 1, Order = 23, Type = SpaceType.War, Width = 100, Height = 100, X = 150, Y = 850, BackgroundColor = "#E1CFC5", BorderColor="#997F70" });
            this.Connection.Insert(new Space() { BoardId = 1, Order = 31, Type = SpaceType.War, Width = 100, Height = 100, X = 50, Y = 150, BackgroundColor = "#E1CFC5", BorderColor="#997F70" });

            //this.Connection.Execute("update Space set Width=70, Height=70, X=(X - (X / 4)), Y=(Y - (Y / 4))");
            this.Connection.Execute("update Space set X=(((X - 50) / 100) + 1), Y=(((Y - 50) / 100) + 1)");
        }
    }
}
