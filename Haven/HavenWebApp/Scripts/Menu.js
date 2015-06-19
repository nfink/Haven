/// <reference path="https://code.jquery.com/jquery-1.11.3.min.js" />

var previousScreen;

function Back() {
    previousScreen();
}

function ShowMainMenu() {
    $("#backButton").hide();
    $("#board").hide();
    $("#actions").hide();
    $("#boardMenu").hide().empty();
    $("#continueMenu").hide().empty();
    $("#mainMenu").show();
}

function ShowBoardMenu() {
    $.get("Boards", function (data) {
        previousScreen = ShowMainMenu;
        $("#mainMenu").hide();
        $("#board").hide();
        $("#actions").hide();
        $("#backButton").show();
        $("#boardMenu").append(data).show();
    });
}

function ShowContinueMenu() {
    $.get("Games", function (data) {
        previousScreen = ShowMainMenu;
        $("#mainMenu").hide();
        $("#board").hide();
        $("#actions").hide();
        $("#backButton").show();
        $("#continueMenu").append(data).show();
    });
}

function NewGame(boardId) {
    $.get("NewGame",
        {
            BoardId: boardId,
            NumberOfPlayers: 2,
            Name: "Test Game",
        },
        function (data) {
            previousScreen = ShowBoardMenu;
            ShowGame();
            var game = JSON.parse(data);
            SetupGame(game);
        });
}

function LoadGame(gameId) {
    $.get("Game/" + gameId, function (data) {
        previousScreen = ShowContinueMenu;
        ShowGame();
        var game = JSON.parse(data);
        SetupGame(game);
    });
}

function ShowGame() {
    $("#mainMenu").hide();
    $("#boardMenu").hide().empty();
    $("#continueMenu").hide().empty();
    $("#board").show();
    $("#actions").show();
}