﻿/// <reference path="https://code.jquery.com/jquery-1.11.3.min.js" />

var previousScreen;

function Init() {

}

function Back() {
    previousScreen();
}

function ShowMainMenu() {
    $("#backButton").hide();
    $("#boardMenu").hide().empty();
    $("#continueMenu").hide().empty();
    $("#mainMenu").show();
}

function ShowBoardMenu() {
    $("#mainMenu").hide();
    $("#game").hide().empty();
    $("#backButton").show();
    $("#loading").show();

    $.get("Boards", function (data) {
        previousScreen = ShowMainMenu;
        $("#loading").hide();
        $("#boardMenu").append(data).show();
    });
}

function ShowContinueMenu() {
    $("#mainMenu").hide();
    $("#game").hide().empty();
    $("#backButton").show();
    $("#loading").show();
    
    $.get("Games", function (data) {
        previousScreen = ShowMainMenu;
        $("#loading").hide();
        $("#continueMenu").append(data).show();
    });
}

function NewGameDialog(boardTile) {
    // set up dialog
    $("#newGameDialogBoardId").val(boardTile.attr("boardId"));
    $("#newGameDialogHeader").text(boardTile.attr("boardName"));
    $("#newGameDialogIcon").attr("src", boardTile.attr("boardIcon"));
    $("#newGameDialogDescription").text(boardTile.attr("boardDescription"));
    $("#newGameDialogName").val("");
    $(".playerSelection").removeClass("active");
    $(".playerSelectionDefault").trigger("click");

    $("#newGameDialog").data("dialog").open();
}

function NewGame(newGameForm) {
    $("#newGameDialog").data("dialog").close();
    $("#mainMenu").hide();
    $("#boardMenu").hide().empty();
    $("#continueMenu").hide().empty();
    $("#loading").show();

    $.get("NewGame", $(newGameForm).serialize(), function (data) {
        previousScreen = ShowContinueMenu;
        SetupGame(data);
        $("#loading").hide();
        $("#game").show();
    });

    return false;
}

function LoadGame(gameId) {
    $("#mainMenu").hide();
    $("#boardMenu").hide().empty();
    $("#continueMenu").hide().empty();
    $("#loading").show();

    $.get("Game/" + gameId, function (data) {
        previousScreen = ShowContinueMenu;
        SetupGame(data);
        $("#loading").hide();
        $("#game").show();
    });
}