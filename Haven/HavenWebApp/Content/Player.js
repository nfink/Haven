
function Player(player) {
    var color = "blue";
    var name = "[" + player.Name + "]";
    var radius = 20;
    var playerContainer = new createjs.Container();
    var circle = new createjs.Shape();
    circle.graphics.beginFill(color).drawCircle(0, 0, radius);
    var label = new createjs.Text(name, "10px Arial");
    label.textBaseline = "alphabetic";
    var bounds = label.getBounds();
    label.x = 0 - (bounds.width / 2);
    label.y = (radius / 2) - (bounds.height / 2);
    playerContainer.addChild(circle);
    playerContainer.addChild(label);
    playerContainer.Player = player;

    playerContainer.moveToSpace = function (space) {
        var center = space.getCenter();
        createjs.Tween.get(this)
        .to({ x: center.x, y: center.y }, 1000, createjs.Ease.getPowInOut(4))
    }

    return playerContainer;
}
