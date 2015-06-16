
function Space(space) {
    var name;
    var color;
    switch (space.Type) {
        case 1:
            name = space.BibleVerse.Book + " " + space.BibleVerse.Chapter + ":" + space.BibleVerse.Verse;
            color = "pink";
            break;
        case 2:
            name = space.NameCard.Name;
            color = "green";
            break;
        case 6:
            name = space.SafeHavenCard.Name;
            color = "yellow";
            break;
        default:
            name = space.Name;
            color = "black";
            break;
    }
    var spaceContainer = new createjs.Container();
    var outer = new createjs.Shape();
    outer.graphics.beginFill(color);
    outer.graphics.drawRoundRect(0, 0, space.Width - 1, space.Height - 1, 20);
    var inner = new createjs.Shape();
    inner.graphics.beginFill("white");
    inner.graphics.drawRoundRect(5, 5, space.Width - 11, space.Height - 11, 16);
    var label = new createjs.Text(name, "10px Arial");
    label.textBaseline = "alphabetic";
    var bounds = label.getBounds();
    label.x = ((space.Width - 1) / 2) - (bounds.width / 2);
    label.y = ((space.Height - 1) / 2) + 10 - (bounds.height / 2);
    spaceContainer.addChild(outer);
    spaceContainer.addChild(inner);
    spaceContainer.addChild(label);
    spaceContainer.setBounds(0, 0, space.Width, space.Height);
    spaceContainer.x = space.X;
    spaceContainer.y = space.Y
    spaceContainer.Space = space;

    spaceContainer.getCenter = function () {
        var bounds = this.getBounds();
        var x = this.x + (bounds.width / 2);
        var y = this.y + (bounds.height / 2);
        return { x: x, y: y };
    }

    return spaceContainer;
}
