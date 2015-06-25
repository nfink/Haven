/// <reference path="https://code.jquery.com/jquery-1.11.3.min.js" />

function SetupGame(game) {
    $("#game").append(game);

    // adjust position of board elements
    $(".space, #messagearea, #statusarea").offset(function (index) {
        var left = $(this).css("left");
        left = left.replace("px", "");
        var top = $(this).css("top");
        top = top.replace("px", "");
        var newLeft = 10 + (left - 1) * $(this).width() * 1.1;
        var newTop = 10 + (top - 1) * $(this).height() * 1.1;
        return { left: newLeft, top: newTop };
    });

    // adjust size of status and message areas
    $("#messagearea, #statusarea").width(function (index, width) {
        return (($(this).attr("width") - 1) * ((width * 1.1) + 1)) + width;
    });
    $("#messagearea, #statusarea").height(function (index, height) {
        return (($(this).attr("height") - 1) * ((height * 1.1) + 1)) + height;
    });
}

function PerformAction(actionForm) {
    // hide actions so the player can't try to perform multiple actions
    $(".action").hide();

    $.post("PerformAction", $(actionForm).serialize(), function (data) {
        var elements = $(data);

        UpdatePieces(elements.filter("#pieces").children());

        // update after pieces have moved
        $(".piece").promise().done(function () {
            // current player selection
            var selectedPlayerId = $(".actionContainer.element-selected").attr("playerid");

            // update messages
            $("#messages").empty();
            $("#messages").append(elements.filter("#messages").children());

            // update cards
            $("#cards").empty();
            $("#cards").append(elements.filter("#cards").children());

            // update actions
            $("#actions").empty();
            $("#actions").append(elements.filter("#actions").children());

            // retain player selection
            $(".actionContainer[playerid=" + selectedPlayerId + "]").trigger("click");
        });
    });

    return false;
}

function UpdatePieces(pieces) {
    $.each(pieces, function (index, value) {
        var playerId = $(value).attr("playerid");
        if ($(".piece[playerid=" + playerId + "]").length < 1) {
            $("#pieces").append(value);
        }
        else {
            var spaceId = $(value).attr("spaceid")
            var piece = $(".piece[playerId=" + playerId + "]");
            if (piece.attr("spaceId") !== spaceId) {
                piece.attr("destinationSpaceId", spaceId);
                piece.attr("direction", piece.attr("direction"));
                piece.MovePiece = MovePiece;
                piece.MovePiece();
            }
        }
    });
}

function MovePiece() {
    var piece = $(this);
    var currentSpaceId = piece.attr("spaceId");
    var destinationSpaceId = piece.attr("destinationSpaceId");
    if (destinationSpaceId !== currentSpaceId) {
        var currentSpace = $(".space[spaceId=" + currentSpaceId + "]");
        var direction = piece.attr("direction");
        var nextSpaceId;
        if (direction == "True") {
            nextSpaceId = currentSpace.attr("nextSpaceId");
        }
        else {
            nextSpaceId = currentSpace.attr("previousSpaceId");
        }
        var nextSpace = $(".space[spaceId=" + nextSpaceId + "]");
        var nextSpacePosition = nextSpace.offset();
        piece.attr("spaceId", nextSpaceId);
        // adjust position for other pieces
        $(".piece[spaceId=" + nextSpaceId + "]").each(function (index) {
            var translation = -1 * (10 * (index + 1));
            $(this).css({ transform: "translate(" + translation + "px," + translation + "px)" });
        });
        piece.animate({ left: nextSpacePosition.left + (nextSpace.width() / 2), top: nextSpacePosition.top + (nextSpace.height() / 2) }, 250, MovePiece);
    }
}

function SelectPlayer(actionsDiv) {
    // mark as selected
    $(".actionContainer").removeClass("element-selected");
    $(actionsDiv).addClass("element-selected");

    // show cards and messages
    var playerId = $(actionsDiv).attr("playerid");
    $("#messagearea").empty();
    $("#messagearea").append($("#messages").find("[playerid=" + playerId + "]").clone());
    $("#statusarea").empty();
    $("#statusarea").append($("#cards").find("[playerid=" + playerId + "]").clone());
}


function SetupGame2(game) {
    GameId = game.Id;
    // $("#spaces").empty();
    // $("#pieces").empty();
    // $("#actions").empty();

    // load spaces
    for (i = 0; i < game.Board.Spaces.length; ++i) {
        var space = game.Board.Spaces[i];

        //<div class="tile">
        //    <div class="tile-content slide-up2">
        //        <div class="slide">
        //            ... Main slide content ...
        //        </div>
        //        <div class="slide-over">
        //            ... Over slide content here ...
        //        </div>
        //    </div>
        //</div>

        // tile containers
        var box = $("<div/>", {
            class: "space tile-small bg-taupe fg-white",
            spaceId: space.Id,
            nextSpaceId: game.Board.Spaces[(i + 1) % (game.Board.Spaces.length - 1)].Id,
            previousSpaceId: i > 0 ? game.Board.Spaces[(i - 1) % (game.Board.Spaces.length - 1)].Id : game.Board.Spaces[game.Board.Spaces.length - 1].Id,
        })
            .css({
                position: "absolute",
                //width: space.Width,
                //height: space.Height,
                left: space.X,
                top: space.Y,
                //left: (space.X - 1) * 80 + 10,
                //top: (space.Y - 1) * 80 + 10,
                //backgroundColor: space.BackgroundColor,
                //borderColor: space.BorderColor,
                //lineHeight: space.Height + "px",
                //lineHeight: "70px",
                //textAlign: "center",
            });
        //box.css("left", 10 + (space.X - 1) * box.width() * 1.1);
        //box.css("top", 10 + (space.Y - 1) * box.height() * 1.1);

        var tile = $("<div/>", {
            class: "tile-content slide-up-2",
        });

        // main content
        var label = $("<div/>", {
            class: "slide spaceLabel" + space.Type,
        })
            .text(space.Name)
            .css({
                textAlign: "center",
                display: "inline-block",
                verticalAlign: "middle",
                lineHeight: "70px",
                //lineHeight: "normal",
            });

        tile.append(label);

        if ((space.Type == 1) || (space.Type == 2) || (space.Type == 6)) {
            // slide over
            var slideOver = $("<div/>", {
                class: "slide-over bg-orange text-small",
            })
                .text(space.Type == 6 ? space.SafeHavenCard.Details : "test");

            tile.append(slideOver);
        }

        box.append(tile);
        $("#spaces").append(box);
    }

    // add styling for spaces with icons instead of text
    $(".spaceLabel1").addClass("mif-books mif-2x");
    $(".spaceLabel1").empty();
    $(".spaceLabel3").addClass("mif-loop2 mif-2x");
    $(".spaceLabel3").empty();
    $(".spaceLabel4").addClass("mif-undo mif-2x");
    $(".spaceLabel4").text("?");
    $(".spaceLabel5").addClass("mif-dice mif-2x");
    $(".spaceLabel5").empty();
    $(".spaceLabel7").addClass("mif-undo mif-2x");
    $(".spaceLabel7").empty();
    $(".spaceLabel8").addClass("mif-fire mif-2x");
    $(".spaceLabel8").empty();

    // adjust position of spaces
    $(".space").offset(function (index) {
        var left = $(this).css("left");
        left = left.replace("px", "");
        var top = $(this).css("top");
        top = top.replace("px", "");
        var newLeft = 10 + (left - 1) * $(this).width() * 1.1;
        var newTop = 10 + (top - 1) * $(this).height() * 1.1;
        return { left: newLeft, top: newTop };
    });

    // load players
    for (i = 0; i < game.Players.length; ++i) {
        var player = game.Players[i];
        var piece = $("<div/>", {
            class: "piece ",//mif-" + player.Piece.Image + " bg-" + player.Piece.Color,
            playerId: player.Id,
            spaceId: player.SpaceId,
        })
            .css({
                position: "absolute",
                width: "40px",
                height: "40px",
                borderRadius: "50%",
                backgroundColor: "blue",
                transform: "translate(-10px,-10px)",
            });
        $("#pieces").append(piece);

        //players[player.Player.Id] = player;
        //var startingLocation = spaces[player.Player.SpaceId].getCenter();
        //player.x = startingLocation.x;
        //player.y = startingLocation.y;
    }

    LoadActions(game.Players);
    UpdatePieces(game.Players);
}

function LoadActions(players)
{
    for (i = 0; i < players.length; ++i) {
        var player = players[i];

        var div = $("<div/>", {
            class: "bg-lightOlive padding5",
        });

        var nameDiv = $("<div/>", {
            class: "padding5",
        });

        var nameLabel = $("<div/>", {
            class: "padding10 header bg-lightOlive",
        })
            .css({
                minWidth: "200px",
                display: "table-cell",
            })
            .text(player.Name == null ? " New Player" : " " + player.Name);

        var nameIcon = $("<div/>", {
            class: "icon header padding5 mif-" + (player.Piece == null ? "user" : player.Piece.Image) + " bg-" + (player.Piece == null ? "lightOlive" : player.Piece.Color),
        })
            .css({
                display: "table-cell",
                minWidth: "56px",
                textAlign: "center",
                //borderRadius: "50%",
            });

        nameDiv.append(nameIcon);
        nameDiv.append(nameLabel);
        div.append(nameDiv);

        for (j = 0; j < player.Actions.length; ++j) {
            var action = player.Actions[j];

            var form = $("<form/>", {
                action: "PerformAction",
                method: "post",
                onsubmit: "return PerformAction(this);",
            });
                //.css({
                //    width: "100px",
                //});

            var id = $("<input/>", {
                name: "Id",
                value: action.Id,
                type: "hidden",
            });

            var input = $("<input/>", {
                name: "Input",
                type: action.RequiresInput ? "text" : "hidden",
            });

            var button = $("<button/>", {
                type: "submit",
                class: "action button bg-lightBlue bd-lightBlue bg-active-blue fg-white " + action.Name,
            })
                .text(action.Name);

            if (action.Type == 12) {
                button.addClass("image-button");
                var buttonIcon = $("<span/>", {
                    class: "icon mif-" + action.Piece.Image + " bg-" + action.Piece.Color,
                });
                button.append(buttonIcon);
            }

            form.append(id)

            if (action.RequiresInput) {
                input.attr("placeholder", action.Name);
                button.empty();

                var inputDiv = $("<div/>", {
                    class: "input-control text",
                    "data-role": "input",
                })
                    .css("width", "100%");

                var icon = $("<span/>", {
                    class: "mif-play",
                })

                inputDiv.append(input);
                button.append(icon);
                inputDiv.append(button);
                form.append(inputDiv);
                form.append($("<br/>"));
            }
            else {
                button.css("width", "100%");
                form.append(input);
                form.append(button);
            }

            div.append($("<div/>", { class: "padding5" }).append(form));
        }

        $("#actions").append(div);
        $("#actions").append($("<div/>", {class: "padding10"}));
    }
}

function UpdatePieces2(players) {
    for (i = 0; i < players.length; ++i) {
        var player = players[i];
        var piece = $(".piece[playerId=" + player.Id + "]");

        if (piece.attr("spaceId") !== player.SpaceId) {
            var piece = $(".piece[playerId=" + player.Id + "]");
            piece.attr("destinationSpaceId", player.SpaceId);
            piece.attr("direction", player.MovementDirection);
            piece.MovePiece = MovePiece;
            piece.MovePiece();
        }
    }
}