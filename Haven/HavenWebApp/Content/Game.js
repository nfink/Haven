/// <reference path="https://code.createjs.com/easeljs-0.8.1.min.js" />
/// <reference path="https://code.createjs.com/tweenjs-0.6.1.min.js" />
/// <reference path="https://code.jquery.com/jquery-1.11.3.min.js" />
/// <reference path="Content/Space.js" />
/// <reference path="Content/Action.js" />
/// <reference path="Content/Player.js" />

var stage;
var game;
var spaces;
var players;
var actionsPanel;


function init() {
    canvasInit();
    boardInit();  
}

function canvasInit() {
    var canvas = document.getElementById('canvasElementId').getContext("2d");
    canvas.canvas.width = window.innerWidth;
    canvas.canvas.height = window.innerHeight;
}

function boardInit() {
    stage = new createjs.Stage("canvasElementId");
    
    $(window).bind('resize', function () {
        stage.update();
    });

    //var background = new createjs.Bitmap("Content/white_marble.jpg");
    //stage.addChild(background);

    // create a new game
    $.get("NewGame", function (data) {
        game = JSON.parse(data);
        spaces = loadSpaces(game);
        players = loadPlayers(game);
        loadActions(game.Players);
        createjs.Ticker.setFPS(60);
        createjs.Ticker.addEventListener("tick", stage);
        //alert($.grep(game.Board.Spaces, function (space) { return space.Id == 4 })[0].BibleVerseId);
    });
}

function performAction(actionId, input) {
    $.post("PerformAction/" + actionId, input, function (data) {
        alert(data);
        // update entire game state?
        $.get("Game/" + game.Id + "/Players", function (data) {
            //alert(data);
            var playerData = JSON.parse(data);
            updateActions(playerData);
            updatePlayers(playerData);
        })
    });
}

function loadSpaces(game) {
    var spaces = [];
    for (i = 0; i < game.Board.Spaces.length; ++i) {
        var space = new Space(game.Board.Spaces[i]);
        spaces[space.Space.Id] = space;
        stage.addChild(space);
    }
    return spaces;
}

function loadPlayers(game) {
    var players = [];
    for (i = 0; i < game.Players.length; ++i) {
        var player = new Player(game.Players[i]);
        players[player.Player.Id] = player;
        var startingLocation = spaces[player.Player.SpaceId].getCenter();
        player.x = startingLocation.x;
        player.y = startingLocation.y;
        stage.addChild(player);
        //moveToSpace(player, spaces[player.Player.SpaceId]);
    }
    return players;
}

function loadActions(players) {
    actionsPanel = new createjs.Container();
    var xValues = $.map(spaces, function (space) { return (space === undefined ? 0 : space.x + space.getBounds().width); });
    var yValues = $.map(spaces, function (space) { return (space === undefined ? 9999 : space.y); });
    actionsPanel.x = Math.max.apply(Math, xValues) + 10;
    actionsPanel.y = Math.min.apply(Math, yValues);

    for (i = 0; i < players.length; ++i) {
        for (j = 0; j < players[i].Actions.length; ++j) {
            var action = new Action(players[i].Actions[j]);
            addActionToPanel(action);
        }
    }

    stage.addChild(actionsPanel);
}

function test() {
    circle = new createjs.Shape();
    circle.graphics.beginFill("blue").drawCircle(0, 0, 20);
    circle.x = 50;
    circle.y = 100
    stage.addChild(circle);

    //space1 = newSpace(100, 100, "green", "Abraham");
    //space2 = newSpace(100, 100, "green", "Babebraham");
    //space3 = newSpace(100, 100, "green", "Lincoln");
    //space1.x = 50;
    //space1.y = 100;
    //space2.x = 200;
    //space2.y = 100;
    //space3.x = 350;
    //space3.y = 100;
    //stage.addChild(space1);
    //stage.addChild(space2);
    //stage.addChild(space3);
    //stage.addChild(circle);
    //stage.update();

    //createjs.Tween.get(circle)
    //.to({ x: 400 }, 1000, createjs.Ease.getPowInOut(4))
    //.to({ alpha: 0, y: 175 }, 500, createjs.Ease.getPowInOut(2))
    //.to({ alpha: 0, y: 225 }, 100)
    //.to({ alpha: 1, y: 200 }, 500, createjs.Ease.getPowInOut(2))
    //.to({ x: 100 }, 800, createjs.Ease.getPowInOut(2));

    //moveToSpace(circle, space3);
    
    createjs.Ticker.setFPS(60);
    createjs.Ticker.addEventListener("tick", stage);
}

function updatePlayers(playerList) {
    for (i = 0; i < playerList.length; ++i) {
        var player = playerList[i];
        var playerContainer = players[player.Id];
        if (playerContainer.Player.SpaceId !== player.SpaceId) {
            playerContainer.moveToSpace(spaces[player.SpaceId]);
        }
        playerContainer.Player = player;
    }
}

function addActionToPanel(action) {
    action.x = 0;
    action.y = (action.getBounds().height) * actionsPanel.children.length;
    actionsPanel.addChild(action);
}

function clearActionsPanel() {
    for (i = 0; i < actionsPanel.children.length; ++i) {
        actionsPanel.children[i].removeAllEventListeners();
    }
    actionsPanel.removeAllChildren();
}

function updateActions(playerList) {
    clearActionsPanel();
    for (i = 0; i < playerList.length; ++i) {
        for (j = 0; j < playerList[i].Actions.length; ++j) {
            var action = new Action(playerList[i].Actions[j]);
            addActionToPanel(action);
        }
    }
}

function handleSpaceClick(event) {
    moveToSpace(circle, this);
}
