using SQLite;
using System;

namespace Haven.Data
{
    public class DataLoad
    {
        public SQLiteConnection Connection;

        public DataLoad()
        {
            this.Connection = new SQLiteConnection(System.Configuration.ConfigurationManager.AppSettings["databasePath"]);
        }

        public void LoadTables()
        {
            this.LoadPiece();
            this.LoadNameCard();
            this.LoadSafeHavenCard();
            this.LoadChallengeCategory();
            this.LoadBoardChallengeCategory();
            this.LoadSpaceChallengeCategory();
            this.LoadChallenge();
            this.LoadChallengeAnswer();
            this.LoadColor();
            this.LoadImage();
            this.LoadSpaceType();
            this.LoadSpace();
            this.LoadBoard();
            this.LoadMessage();
            this.LoadPlayer();
            this.LoadGame();
            this.LoadGameWinner();
            this.LoadUser();
            this.Connection.CreateTable<Action>();
            this.Connection.CreateTable<UsedChallenge>();
            this.Connection.CreateTable<PlayerNameCard>();
            this.Connection.CreateTable<PlayerSafeHavenCard>();
            this.Connection.Close();
        }

        public void LoadBoard()
        {
            this.Connection.CreateTable<Board>();
            this.Connection.Insert(new Board() { Id = 1, OwnerId = 1, Name = "Genesis", Description = "Covers the book of Genesis", ImageId = 17, Active = true });
        }

        public void LoadBoardChallengeCategory()
        {
            this.Connection.CreateTable<BoardChallengeCategory>();
            this.Connection.Insert(new BoardChallengeCategory() { BoardId = 1, ChallengeCategoryId = 1 });
            this.Connection.Insert(new BoardChallengeCategory() { BoardId = 1, ChallengeCategoryId = 2 });
        }

        public void LoadChallenge()
        {
            this.Connection.CreateTable<Challenge>();
            this.Connection.Insert(new Challenge() { Id = 1, Question = "Test Multiple Choice 1?", ChallengeCategoryId = 1, OwnerId = 1 });
            this.Connection.Insert(new Challenge() { Id = 2, Question = "Test Multiple Choice 2?", ChallengeCategoryId = 1, OwnerId = 1 });
            this.Connection.Insert(new Challenge() { Id = 3, Question = "Test Multiple Choice 3?", ChallengeCategoryId = 2, OwnerId = 1 });
            this.Connection.Insert(new Challenge() { Id = 4, Question = "Test Open Ended 1?", ChallengeCategoryId = 3, OpenEnded = true, OwnerId = 1 });
            this.Connection.Insert(new Challenge() { Id = 5, Question = "Test Open Ended 2?", ChallengeCategoryId = 3, OpenEnded = true, OwnerId = 1 });
        }

        public void LoadChallengeAnswer()
        {
            this.Connection.CreateTable<ChallengeAnswer>();
            this.Connection.Insert(new ChallengeAnswer() { Answer = "Test Answer 1a", Correct = false, ChallengeId = 1 });
            this.Connection.Insert(new ChallengeAnswer() { Answer = "Test Answer 1b", Correct = false, ChallengeId = 1 });
            this.Connection.Insert(new ChallengeAnswer() { Answer = "Test Answer 1c", Correct = false, ChallengeId = 1 });
            this.Connection.Insert(new ChallengeAnswer() { Answer = "Test Answer 1d (correct)", Correct = true, ChallengeId = 1 });
            this.Connection.Insert(new ChallengeAnswer() { Answer = "Test Answer 2a", Correct = false, ChallengeId = 2 });
            this.Connection.Insert(new ChallengeAnswer() { Answer = "Test Answer 2b", Correct = false, ChallengeId = 2 });
            this.Connection.Insert(new ChallengeAnswer() { Answer = "Test Answer 2c (correct)", Correct = true, ChallengeId = 2 });
            this.Connection.Insert(new ChallengeAnswer() { Answer = "Test Answer 2d", Correct = false, ChallengeId = 2 });
            this.Connection.Insert(new ChallengeAnswer() { Answer = "Test Answer 3a", Correct = false, ChallengeId = 3 });
            this.Connection.Insert(new ChallengeAnswer() { Answer = "Test Answer 3b (correct)", Correct = true, ChallengeId = 3 });
            this.Connection.Insert(new ChallengeAnswer() { Answer = "Test Answer 3c", Correct = false, ChallengeId = 3 });
            this.Connection.Insert(new ChallengeAnswer() { Answer = "Test Answer 3d", Correct = false, ChallengeId = 3 });
            this.Connection.Insert(new ChallengeAnswer() { Answer = "test1", Correct = true, ChallengeId = 4 });
            this.Connection.Insert(new ChallengeAnswer() { Answer = "test2", Correct = true, ChallengeId = 5 });
        }

        public void LoadChallengeCategory()
        {
            this.Connection.CreateTable<ChallengeCategory>();
            this.Connection.Insert(new ChallengeCategory() { Id = 1, Name = "Multiple Choice 1", OwnerId = 1 });
            this.Connection.Insert(new ChallengeCategory() { Id = 2, Name = "Multiple Choice 2", OwnerId = 1 });
            this.Connection.Insert(new ChallengeCategory() { Id = 3, Name = "Open Ended", OwnerId = 1 });
        }

        public void LoadColor()
        {
            this.Connection.CreateTable<Color>();
            this.Connection.Insert(new Color() { Name = "lightTeal" });
            this.Connection.Insert(new Color() { Name = "teal" });
            this.Connection.Insert(new Color() { Name = "darkTeal" });
            this.Connection.Insert(new Color() { Name = "lighterBlue" });
            this.Connection.Insert(new Color() { Name = "lightBlue" });
            this.Connection.Insert(new Color() { Name = "blue" });
            this.Connection.Insert(new Color() { Name = "darkBlue" });
            this.Connection.Insert(new Color() { Name = "cyan" });
            this.Connection.Insert(new Color() { Name = "darkCyan" });
            this.Connection.Insert(new Color() { Name = "cobalt" });
            this.Connection.Insert(new Color() { Name = "darkCobalt" });
            this.Connection.Insert(new Color() { Name = "indigo" });
            this.Connection.Insert(new Color() { Name = "darkIndigo" });
            this.Connection.Insert(new Color() { Name = "violet" });
            this.Connection.Insert(new Color() { Name = "darkViolet" });
            this.Connection.Insert(new Color() { Name = "mauve" });
            this.Connection.Insert(new Color() { Name = "lightPink" });
            this.Connection.Insert(new Color() { Name = "pink" });
            this.Connection.Insert(new Color() { Name = "darkPink" });
            this.Connection.Insert(new Color() { Name = "magenta" });
            this.Connection.Insert(new Color() { Name = "darkMagenta" });
            this.Connection.Insert(new Color() { Name = "lightRed" });
            this.Connection.Insert(new Color() { Name = "red" });
            this.Connection.Insert(new Color() { Name = "darkRed" });
            this.Connection.Insert(new Color() { Name = "crimson" });
            this.Connection.Insert(new Color() { Name = "darkCrimson" });
            this.Connection.Insert(new Color() { Name = "lightOrange" });
            this.Connection.Insert(new Color() { Name = "orange" });
            this.Connection.Insert(new Color() { Name = "darkOrange" });
            this.Connection.Insert(new Color() { Name = "amber" });
            this.Connection.Insert(new Color() { Name = "yellow" });
            this.Connection.Insert(new Color() { Name = "lime" });
            this.Connection.Insert(new Color() { Name = "lightGreen" });
            this.Connection.Insert(new Color() { Name = "green" });
            this.Connection.Insert(new Color() { Name = "darkGreen" });
            this.Connection.Insert(new Color() { Name = "emerald" });
            this.Connection.Insert(new Color() { Name = "darkEmerald" });
            this.Connection.Insert(new Color() { Name = "lightOlive" });
            this.Connection.Insert(new Color() { Name = "olive" });
            this.Connection.Insert(new Color() { Name = "taupe" });
            this.Connection.Insert(new Color() { Name = "brown" });
            this.Connection.Insert(new Color() { Name = "darkBrown" });
            this.Connection.Insert(new Color() { Name = "black" });
            this.Connection.Insert(new Color() { Name = "grayDarker" });
            this.Connection.Insert(new Color() { Name = "grayDark" });
            this.Connection.Insert(new Color() { Name = "gray" });
            this.Connection.Insert(new Color() { Name = "grayLight" });
            this.Connection.Insert(new Color() { Name = "grayLighter" });
            this.Connection.Insert(new Color() { Name = "white" });
        }

        public void LoadGame()
        {
            this.Connection.CreateTable<Game>();
            this.Connection.Insert(new Game() { Id = 1, OwnerId = 1, Name = "Test Game 1", Ended = true });
            this.Connection.Insert(new Game() { Id = 2, OwnerId = 1, Name = "Test Game 2", Ended = true });
        }

        public void LoadGameWinner()
        {
            this.Connection.CreateTable<GameWinner>();
            this.Connection.Insert(new GameWinner() { GameId = 1, Player = "Test Player 1", Turn = 50 });
            this.Connection.Insert(new GameWinner() { GameId = 2, Player = "Test Player 2", Turn = 60 });
            this.Connection.Insert(new GameWinner() { GameId = 2, Player = "Test Player 3", Turn = 60 });
        }

        public void LoadImage()
        {
            this.Connection.CreateTable<Image>();
            this.Connection.Insert(new Image() { Id = 1, Filename = "Judah.jpg", Filepath = "/Uploads/Judah.jpg" });
            this.Connection.Insert(new Image() { Id = 2, Filename = "Issachar.jpg", Filepath = "/Uploads/Issachar.jpg" });
            this.Connection.Insert(new Image() { Id = 3, Filename = "Zebulun.jpg", Filepath = "/Uploads/Zebulun.jpg" });
            this.Connection.Insert(new Image() { Id = 4, Filename = "Reuben.jpg", Filepath = "/Uploads/Reuben.jpg" });
            this.Connection.Insert(new Image() { Id = 5, Filename = "Simeon.jpg", Filepath = "/Uploads/Simeon.jpg" });
            this.Connection.Insert(new Image() { Id = 6, Filename = "Gad.jpg", Filepath = "/Uploads/Gad.jpg" });
            this.Connection.Insert(new Image() { Id = 7, Filename = "Joseph.jpg", Filepath = "/Uploads/Joseph.jpg" });
            this.Connection.Insert(new Image() { Id = 8, Filename = "Levi.jpg", Filepath = "/Uploads/Levi.jpg" });
            this.Connection.Insert(new Image() { Id = 9, Filename = "Benjamin.jpg", Filepath = "/Uploads/Benjamin.jpg" });
            this.Connection.Insert(new Image() { Id = 10, Filename = "Dan.jpg", Filepath = "/Uploads/Dan.jpg" });
            this.Connection.Insert(new Image() { Id = 11, Filename = "Asher.jpg", Filepath = "/Uploads/Asher.jpg" });
            this.Connection.Insert(new Image() { Id = 12, Filename = "Naphtali.jpg", Filepath = "/Uploads/Naphtali.jpg" });
            this.Connection.Insert(new Image() { Id = 13, Filename = "God_the_Father.jpg", Filepath = "/Uploads/God_the_Father.jpg" });
            this.Connection.Insert(new Image() { Id = 14, Filename = "Abraham.jpg", Filepath = "/Uploads/Abraham.jpg" });
            this.Connection.Insert(new Image() { Id = 15, Filename = "Isaac.jpeg", Filepath = "/Uploads/Isaac.jpeg" });
            this.Connection.Insert(new Image() { Id = 16, Filename = "Jacob.jpg", Filepath = "/Uploads/Jacob.jpg" });
            this.Connection.Insert(new Image() { Id = 17, Filename = "Rembrandt_Harmensz_square.jpg", Filepath = "/Uploads/Rembrandt_Harmensz_square.jpg" });
        }

        public void LoadNameCard()
        {
            this.Connection.CreateTable<NameCard>();
            this.Connection.Insert(new NameCard() { Id = 1, Name = "Judah", ImageId = 1 });
            this.Connection.Insert(new NameCard() { Id = 2, Name = "Issachar", ImageId = 2 });
            this.Connection.Insert(new NameCard() { Id = 3, Name = "Zebulun", ImageId = 3 });
            this.Connection.Insert(new NameCard() { Id = 4, Name = "Reuben", ImageId = 4 });
            this.Connection.Insert(new NameCard() { Id = 5, Name = "Simeon", ImageId = 5 });
            this.Connection.Insert(new NameCard() { Id = 6, Name = "Gad", ImageId = 6 });
            this.Connection.Insert(new NameCard() { Id = 7, Name = "Joseph", ImageId = 7 });
            this.Connection.Insert(new NameCard() { Id = 8, Name = "Levi", ImageId = 8 });
            this.Connection.Insert(new NameCard() { Id = 9, Name = "Benjamin", ImageId = 9 });
            this.Connection.Insert(new NameCard() { Id = 10, Name = "Dan", ImageId = 10 });
            this.Connection.Insert(new NameCard() { Id = 11, Name = "Asher", ImageId = 11 });
            this.Connection.Insert(new NameCard() { Id = 12, Name = "Naphtali", ImageId = 12 });
            this.Connection.Insert(new NameCard() { Id = 13, Name = "Genesis 1:1" });
            this.Connection.Insert(new NameCard() { Id = 14, Name = "Genesis 4:9" });
            this.Connection.Insert(new NameCard() { Id = 15, Name = "Genesis 6:8" });
            this.Connection.Insert(new NameCard() { Id = 16, Name = "Genesis 50:25" });
        }

        public void LoadMessage()
        {
            this.Connection.CreateTable<Message>();
        }

        public void LoadPiece()
        {
            this.Connection.CreateTable<Piece>();
            this.Connection.Insert(new Piece() { Name = "Microscope", Image = "mif-microscope" });
            this.Connection.Insert(new Piece() { Name = "Lamp", Image = "mif-lamp" });
            this.Connection.Insert(new Piece() { Name = "Stethoscope", Image = "mif-stethoscope" });
            this.Connection.Insert(new Piece() { Name = "Airplane", Image = "mif-local-airport" });
            this.Connection.Insert(new Piece() { Name = "Cart", Image = "mif-cart" });
            this.Connection.Insert(new Piece() { Name = "Bug", Image = "mif-bug" });
            this.Connection.Insert(new Piece() { Name = "Rocket", Image = "mif-rocket" });
            this.Connection.Insert(new Piece() { Name = "Truck", Image = "mif-truck" });
            this.Connection.Insert(new Piece() { Name = "Ambulance", Image = "mif-ambulance" });
            this.Connection.Insert(new Piece() { Name = "Bicycle", Image = "mif-directions-bike" });
            this.Connection.Insert(new Piece() { Name = "Paint Roller", Image = "mif-paint" });
            this.Connection.Insert(new Piece() { Name = "Wrench", Image = "mif-wrench" });
            this.Connection.Insert(new Piece() { Name = "Hammer", Image = "mif-hammer" });
            this.Connection.Insert(new Piece() { Name = "Paper Airplane", Image = "mif-paper-plane" });
            this.Connection.Insert(new Piece() { Name = "Satellite", Image = "mif-satellite" });
            this.Connection.Insert(new Piece() { Name = "Scales", Image = "mif-justice" });
            this.Connection.Insert(new Piece() { Name = "Paw", Image = "mif-paw" });
            this.Connection.Insert(new Piece() { Name = "Barbell", Image = "mif-barbell" });
            this.Connection.Insert(new Piece() { Name = "Car", Image = "mif-drive-eta" });
            this.Connection.Insert(new Piece() { Name = "Gift", Image = "mif-gift" });
            this.Connection.Insert(new Piece() { Name = "Silverware", Image = "mif-spoon-fork" });
            this.Connection.Insert(new Piece() { Name = "Flask", Image = "mif-lab" });
            this.Connection.Insert(new Piece() { Name = "Suitcase", Image = "mif-suitcase" });
            this.Connection.Insert(new Piece() { Name = "Money", Image = "mif-dollar" });
            this.Connection.Insert(new Piece() { Name = "Tools", Image = "mif-tools" });
            this.Connection.Insert(new Piece() { Name = "Anchor", Image = "mif-anchor" });
            this.Connection.Insert(new Piece() { Name = "Gamepad", Image = "mif-gamepad" });
            this.Connection.Insert(new Piece() { Name = "Piano", Image = "mif-piano" });
            this.Connection.Insert(new Piece() { Name = "Phone", Image = "mif-phone" });
            this.Connection.Insert(new Piece() { Name = "Computer", Image = "mif-display" });
            this.Connection.Insert(new Piece() { Name = "Medical Kit", Image = "mif-medkit" });
            this.Connection.Insert(new Piece() { Name = "Gas Pump", Image = "mif-gas-station" });
            this.Connection.Insert(new Piece() { Name = "Magic Wand", Image = "mif-magic-wand" });
            this.Connection.Insert(new Piece() { Name = "Eye", Image = "mif-eye" });
            this.Connection.Insert(new Piece() { Name = "Shopping Basket", Image = "mif-shopping-basket2" });
        }

        public void LoadPlayer()
        {
            this.Connection.CreateTable<Player>();
        }

        public void LoadSafeHavenCard()
        {
            this.Connection.CreateTable<SafeHavenCard>();
            this.Connection.Insert(new SafeHavenCard() { Id = 1, Name = "God", ImageId = 13, Details = "The Father, creator of the universe." });
            this.Connection.Insert(new SafeHavenCard() { Id = 2, Name = "Abraham", ImageId = 14, Details = "The founding father of the Jewish nation of Israel, he was a man of great faith and obedience to the will of God." });
            this.Connection.Insert(new SafeHavenCard() { Id = 3, Name = "Isaac", ImageId = 15, Details = "Only child of Abraham and Sarah. Miraculously born after Sarah healed of being barren." });
            this.Connection.Insert(new SafeHavenCard() { Id = 4, Name = "Jacob", ImageId = 16, Details = "Son of Isaac, father of the twelve sons who form the tribes of Israel." });
        }

        public void LoadSpaceType()
        {

        }

        public void LoadSpaceChallengeCategory()
        {
            this.Connection.CreateTable<SpaceChallengeCategory>();
            this.Connection.Insert(new SpaceChallengeCategory() { SpaceId = 1, ChallengeCategoryId = 3 });
            this.Connection.Insert(new SpaceChallengeCategory() { SpaceId = 2, ChallengeCategoryId = 3 });
            this.Connection.Insert(new SpaceChallengeCategory() { SpaceId = 3, ChallengeCategoryId = 3 });
            this.Connection.Insert(new SpaceChallengeCategory() { SpaceId = 4, ChallengeCategoryId = 3 });
        }

        public void LoadSpace()
        {
            this.Connection.CreateTable<Space>();
            this.Connection.Insert(new Space() { Id = 1, BoardId = 1, Order = 1, Type = SpaceType.Challenge, NameCardId = 13, Width = 100, Height = 100, X = 150, Y = 50,});// BackgroundColor = "#E1CFC5", BorderColor="#997F70" });
            this.Connection.Insert(new Space() { Id = 2, BoardId = 1, Order = 9, Type = SpaceType.Challenge, NameCardId = 14, Width = 100, Height = 100, X = 850, Y = 150,});// BackgroundColor = "#E1CFC5", BorderColor="#997F70" });
            this.Connection.Insert(new Space() { Id = 3, BoardId = 1, Order = 17, Type = SpaceType.Challenge, NameCardId = 15, Width = 100, Height = 100, X = 750, Y = 850,});// BackgroundColor = "#E1CFC5", BorderColor="#997F70" });
            this.Connection.Insert(new Space() { Id = 4, BoardId = 1, Order = 25, Type = SpaceType.Challenge, NameCardId = 16, Width = 100, Height = 100, X = 50, Y = 750,});// BackgroundColor = "#E1CFC5", BorderColor="#997F70" });

            this.Connection.Insert(new Space() { BoardId = 1, Order = 6, Type = SpaceType.ExchangePlaces, Width = 100, Height = 100, X = 650, Y = 50,});// BackgroundColor = "#E1CFC5", BorderColor="#997F70" });
            this.Connection.Insert(new Space() { BoardId = 1, Order = 14, Type = SpaceType.ExchangePlaces, Width = 100, Height = 100, X = 850, Y = 650,});// BackgroundColor = "#E1CFC5", BorderColor="#997F70" });

            this.Connection.Insert(new Space() { BoardId = 1, Order = 3, Type = SpaceType.OptionalTurnAround, Width = 100, Height = 100, X = 350, Y = 50,});// BackgroundColor = "#E1CFC5", BorderColor="#997F70" });
            this.Connection.Insert(new Space() { BoardId = 1, Order = 19, Type = SpaceType.OptionalTurnAround, Width = 100, Height = 100, X = 550, Y = 850,});// BackgroundColor = "#E1CFC5", BorderColor="#997F70" });

            this.Connection.Insert(new Space() { BoardId = 1, Order = 11, Type = SpaceType.RollToGo, Width = 100, Height = 100, X = 850, Y = 350,});// BackgroundColor = "#E1CFC5", BorderColor="#997F70" });
            this.Connection.Insert(new Space() { BoardId = 1, Order = 27, Type = SpaceType.RollToGo, Width = 100, Height = 100, X = 50, Y = 550,});// BackgroundColor = "#E1CFC5", BorderColor="#997F70" });

            this.Connection.Insert(new Space() { BoardId = 1, Order = 0, Type = SpaceType.SafeHaven, SafeHavenCardId = 1, Width = 100, Height = 100, X = 50, Y = 50,});// BackgroundColor = "#E1CFC5", BorderColor="#997F70" });
            this.Connection.Insert(new Space() { BoardId = 1, Order = 8, Type = SpaceType.SafeHaven, SafeHavenCardId = 2, Width = 100, Height = 100, X = 850, Y = 50,});// BackgroundColor = "#E1CFC5", BorderColor="#997F70" });
            this.Connection.Insert(new Space() { BoardId = 1, Order = 16, Type = SpaceType.SafeHaven, SafeHavenCardId = 3, Width = 100, Height = 100, X = 850, Y = 850,});// BackgroundColor = "#E1CFC5", BorderColor="#997F70" });
            this.Connection.Insert(new Space() { BoardId = 1, Order = 24, Type = SpaceType.SafeHaven, SafeHavenCardId = 4, Width = 100, Height = 100, X = 50, Y = 850,});// BackgroundColor = "#E1CFC5", BorderColor="#997F70" });

            this.Connection.Insert(new Space() { BoardId = 1, Order = 2, Type = SpaceType.Challenge, NameCardId = 1, Width = 100, Height = 100, X = 250, Y = 50,});// BackgroundColor = "#E1CFC5", BorderColor="#997F70" });
            this.Connection.Insert(new Space() { BoardId = 1, Order = 4, Type = SpaceType.Challenge, NameCardId = 2, Width = 100, Height = 100, X = 450, Y = 50,});// BackgroundColor = "#E1CFC5", BorderColor="#997F70" });
            this.Connection.Insert(new Space() { BoardId = 1, Order = 5, Type = SpaceType.Challenge, NameCardId = 3, Width = 100, Height = 100, X = 550, Y = 50,});// BackgroundColor = "#E1CFC5", BorderColor="#997F70" });
            this.Connection.Insert(new Space() { BoardId = 1, Order = 10, Type = SpaceType.Challenge, NameCardId = 4, Width = 100, Height = 100, X = 850, Y = 250,});// BackgroundColor = "#E1CFC5", BorderColor="#997F70" });
            this.Connection.Insert(new Space() { BoardId = 1, Order = 12, Type = SpaceType.Challenge, NameCardId = 5, Width = 100, Height = 100, X = 850, Y = 450,});// BackgroundColor = "#E1CFC5", BorderColor="#997F70" });
            this.Connection.Insert(new Space() { BoardId = 1, Order = 13, Type = SpaceType.Challenge, NameCardId = 6, Width = 100, Height = 100, X = 850, Y = 550,});// BackgroundColor = "#E1CFC5", BorderColor="#997F70" });
            this.Connection.Insert(new Space() { BoardId = 1, Order = 18, Type = SpaceType.Challenge, NameCardId = 7, Width = 100, Height = 100, X = 650, Y = 850,});// BackgroundColor = "#E1CFC5", BorderColor="#997F70" });
            this.Connection.Insert(new Space() { BoardId = 1, Order = 20, Type = SpaceType.Challenge, NameCardId = 8, Width = 100, Height = 100, X = 450, Y = 850,});// BackgroundColor = "#E1CFC5", BorderColor="#997F70" });
            this.Connection.Insert(new Space() { BoardId = 1, Order = 21, Type = SpaceType.Challenge, NameCardId = 9, Width = 100, Height = 100, X = 350, Y = 850,});// BackgroundColor = "#E1CFC5", BorderColor="#997F70" });
            this.Connection.Insert(new Space() { BoardId = 1, Order = 26, Type = SpaceType.Challenge, NameCardId = 10, Width = 100, Height = 100, X = 50, Y = 650,});// BackgroundColor = "#E1CFC5", BorderColor="#997F70" });
            this.Connection.Insert(new Space() { BoardId = 1, Order = 28, Type = SpaceType.Challenge, NameCardId = 11, Width = 100, Height = 100, X = 50, Y = 450,});// BackgroundColor = "#E1CFC5", BorderColor="#997F70" });
            this.Connection.Insert(new Space() { BoardId = 1, Order = 29, Type = SpaceType.Challenge, NameCardId = 12, Width = 100, Height = 100, X = 50, Y = 350,});// BackgroundColor = "#E1CFC5", BorderColor="#997F70" });

            this.Connection.Insert(new Space() { BoardId = 1, Order = 22, Type = SpaceType.TurnAround, Width = 100, Height = 100, X = 250, Y = 850,});// BackgroundColor = "#E1CFC5", BorderColor="#997F70" });
            this.Connection.Insert(new Space() { BoardId = 1, Order = 30, Type = SpaceType.TurnAround, Width = 100, Height = 100, X = 50, Y = 250,});// BackgroundColor = "#E1CFC5", BorderColor="#997F70" });

            this.Connection.Insert(new Space() { BoardId = 1, Order = 7, Type = SpaceType.War, Width = 100, Height = 100, X = 750, Y = 50,});// BackgroundColor = "#E1CFC5", BorderColor="#997F70" });
            this.Connection.Insert(new Space() { BoardId = 1, Order = 15, Type = SpaceType.War, Width = 100, Height = 100, X = 850, Y = 750,});// BackgroundColor = "#E1CFC5", BorderColor="#997F70" });
            this.Connection.Insert(new Space() { BoardId = 1, Order = 23, Type = SpaceType.War, Width = 100, Height = 100, X = 150, Y = 850,});// BackgroundColor = "#E1CFC5", BorderColor="#997F70" });
            this.Connection.Insert(new Space() { BoardId = 1, Order = 31, Type = SpaceType.War, Width = 100, Height = 100, X = 50, Y = 150,});// BackgroundColor = "#E1CFC5", BorderColor="#997F70" });

            //this.Connection.Execute("update Space set Width=70, Height=70, X=(X - (X / 4)), Y=(Y - (Y / 4))");
            this.Connection.Execute("update Space set X=(((X - 50) / 100) + 1), Y=(((Y - 50) / 100) + 1)");
        }

        public void LoadUser()
        {
            this.Connection.CreateTable<User>();
            var testUser = new User() { Id = 1, Username = "test", Guid = Guid.NewGuid().ToString(), Password = "iJahIbXk+C1Rh1cTsEFJZRBAoL+XVIcrXVOIgzToC9stvzdg" };
            this.Connection.Insert(testUser);
        }
    }
}
