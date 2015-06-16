
function Action(action) {
    var color = "red";
    var text = action.Name;
    var width = 150;
    var height = 50;
    var actionContainer = new createjs.Container();
    var outer = new createjs.Shape();
    outer.graphics.beginFill(color);
    outer.graphics.drawRoundRect(0, 0, width - 1, height - 1, 20);
    var inner = new createjs.Shape();
    inner.graphics.beginFill("white");
    inner.graphics.drawRoundRect(5, 5, width - 11, height - 11, 16);
    var label = new createjs.Text(text, "10px Arial");
    label.textBaseline = "alphabetic";
    var bounds = label.getBounds();
    label.x = ((width - 1) / 2) - (bounds.width / 2);
    label.y = ((height - 1) / 2) + 10 - (bounds.height / 2);
    actionContainer.addChild(outer);
    actionContainer.addChild(inner);
    actionContainer.addChild(label);
    actionContainer.setBounds(0, 0, width, height);
    actionContainer.Action = action;

    actionContainer.on("click", function(event) {
        performAction(this.Action.Id, null);
    }, actionContainer);

    return actionContainer;
}