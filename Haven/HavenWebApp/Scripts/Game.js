/// <reference path="https://code.jquery.com/jquery-1.11.3.min.js" />

function SetupGame(game) {
    $("#game").append(game);

    AdjustBoardDimensions($(".space, #messagearea, #statusarea"), $("#messagearea, #statusarea"));

    // set up links between spaces for piece movement
    var spaceOrders = [];
    $(".space").each(function () {
        spaceOrders.push(Number($(this).attr("order")));
    });
    spaceOrders.sort(function (x, y) { return x - y; });
    var numberOfSpaces = spaceOrders.length;
    for (i = 0; i < spaceOrders.length; i++) {
        var nextSpaceOrder = spaceOrders[(i + 1) % numberOfSpaces];
        var previousSpaceOrder = i > 0 ? spaceOrders[(i - 1) % (numberOfSpaces - 1)] : spaceOrders[numberOfSpaces - 1];
        var space = $(".space[order=" + spaceOrders[i] + "]");
        var nextSpaceId = $(".space[order=" + nextSpaceOrder + "]").attr("spaceid");
        var previousSpaceId = $(".space[order=" + previousSpaceOrder + "]").attr("spaceId");
        space.attr("nextspaceid", nextSpaceId);
        space.attr("previousspaceid", previousSpaceId);
    }

    // click handler to show details for challenge and safe haven spaces
    $(".space[details!='']").click(function (e) {
        var statusarea = $("#statusarea");
        var spaceClass = "spaceDetails" + $(this).attr("spaceid");
        if (statusarea.hasClass(spaceClass)) {
            statusarea.removeClass(spaceClass);
            RestoreCards();
        }
        else {
            RemoveSpaceClasses()
            statusarea.addClass(spaceClass);
            var details = $(this).attr("details");
            $("#statusarea").empty();
            $("#statusarea").text(details);
            $("#statusarea").click(RestoreCards);
        }
    });

    PlayerNameSetup();
    UpdateActionPasswords();
}

function AdjustBoardDimensions(spaces, areas) {
    // adjust position of board elements
    spaces.offset(function (index) {
        var left = $(this).attr("x");
        var top = $(this).attr("y");
        var newLeft = 10 + (left - 1) * $(this).width() * 1.1;
        var newTop = 10 + (top - 1) * $(this).height() * 1.1;
        return { left: newLeft, top: newTop };
    });

    // adjust size of status and message areas
    areas.width(function (index, width) {
        return (($(this).attr("width") - 1) * (width * 1.1)) + width;
    });
    areas.height(function (index, height) {
        return (($(this).attr("height") - 1) * (height * 1.1)) + height;
    });
}

function PerformAction(actionForm) {
    if (!FieldValidation(actionForm)) {
        return false;
    }

    $.post("PerformAction", $(actionForm).serialize())
        .done(function (data) {
            var elements = $(data);

            // update player details and remove used actions before moving pieces
            var actions = elements.filter("#actions").children();
            UpdatePlayerDetails(actions);
            RemoveOldActions(actions);

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
                AddNewActions(actions);

                // update displayed details and messages
                UpdateMessagesAndStatus(selectedPlayerId);
            });
        })
        .fail(function () {
            $(".action").show();
            alert("invalid attempt to perform action");
        });

    return false;
}

function UpdatePlayerDetails(actions) {
    $.each(actions, function (index, value) {
        var newActionContainer = $(value);
        var playerId = newActionContainer.attr("playerid");
        var actionContainer = $(".actionContainer[playerid=" + playerId + "]");

        // add/remove passwordNotSet class as needed
        if (newActionContainer.hasClass("passwordNotSet")) {
            if (!actionContainer.hasClass("passwordNotSet")) {
                actionContainer.addClass("passwordNotSet").addClass("bg-olive").removeClass("ribbed-olive");
            }
        }
        else {
            if (actionContainer.hasClass("passwordNotSet")) {
                actionContainer.removeClass("passwordNotSet").addClass("ribbed-olive").removeClass("bg-olive");
            }
        }

        // update name/icon if necessary
        var currentName = actionContainer.find(".playerNameLabel").text();
        var newName = newActionContainer.find(".playerNameLabel").text();
        var currentIcon = actionContainer.find(".icon").attr("class");
        var newIcon = newActionContainer.find(".icon").attr("class");
        if ((currentName != newName) || (currentIcon != newIcon)) {
            actionContainer.find(".playerName").parents("form").remove();
            actionContainer.prepend(newActionContainer.find(".playerName").parents("form"));
        }
    });

    UpdateActionPasswords();
}

function RemoveOldActions(actions) {
    $.each(actions, function (index, value) {
        var newActionContainer = $(value);
        var playerId = newActionContainer.attr("playerid");
        var actionContainer = $(".actionContainer[playerid=" + playerId + "]");

        // remove actions that no longer exist
        $.each(actionContainer.find(".action"), function (j, action) {
            var actionId = $(action).find("[name=Id]").val();
            if (newActionContainer.find(".action").find("[name=Id][value=" + actionId + "]").length < 1) {
                $(action).remove();
            }
        });
    });

    UpdateActionPasswords();
}

function AddNewActions(actions) {
    $.each(actions, function (index, value) {
        var newActionContainer = $(value);
        var playerId = newActionContainer.attr("playerid");
        var actionContainer = $(".actionContainer[playerid=" + playerId + "]");

        // add new actions
        $.each($(value).find(".action"), function (j, action) {
            var actionId = $(action).find("[name=Id]").val();
            if (actionContainer.find(".action").find("[name=Id][value=" + actionId + "]").length < 1) {
                actionContainer.append(action);
            }
        });
    });

    UpdateActionPasswords();
}

function UpdateActionPasswords()
{
    // set passwords
    $.each($(".playerPassword"), function (index, value) {
        var playerId = $(value).attr("playerid");
        var password = $(value).attr("password");
        var actionContainer = $(".actionContainer[playerid=" + playerId + "]");
        actionContainer.find(".action").find("[name=Password]").val(password);
        actionContainer.find(".playerName").find("input").hide();
        actionContainer.find(".playerNameButton").hide();
        // set background to indicate player has logged in
        actionContainer.attr("class", actionContainer.attr("class").replace("ribbed-", "bg-"));
    });

    // hide actions without a password
    $(".actionContainer:not(.passwordNotSet)").find(".action").find("[name=Password]:not([value])").parents(".action").hide();

    // show actions with a password
    $(".actionContainer:not(.passwordNotSet)").find(".action").find("[name=Password][value]").parents(".action").show();
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
                piece.attr("direction", $(value).attr("direction"));
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
    UpdateMessagesAndStatus(playerId);
}

function UpdateMessagesAndStatus(playerId) {
    $("#messagearea").empty();
    $("#messagearea").append($("#messages").find("[playerid=" + playerId + "]").clone());
    $("#statusarea").attr("playerid", playerId);
    RestoreCards();
}

function RestoreCards() {
    RemoveSpaceClasses();
    var playerId = $("#statusarea").attr("playerid");
    $("#statusarea").empty();
    $("#statusarea").append($("#cards").find("[playerid=" + playerId + "]").clone());
    $("#statusarea").attr("playerid", playerId);

    // remove click handler while cards are displayed
    $("#statusarea").off("click");

    // add click handler to expand a card
    $(".namecard, .safehavencard").click(function (e) {
        e.stopPropagation();

        var text = $(this).attr("details");
        $("#statusarea").empty();
        $("#statusarea").text(text);

        // add click handler to restore the cards view
        $("#statusarea").click(RestoreCards);
    });
}

function RemoveSpaceClasses() {
    // remove and spaceDetails classes from the statusarea
    $("#statusarea").removeClass(function (index, className) {
        var classesToRemove = "";
        $.each($(".space[details!='']"), function (index, value) {
            classesToRemove += ("spaceDetails" + $(value).attr("spaceid") + " ");
        });
        return classesToRemove;
    });
}

function EnterPasswordAction(actionForm) {
    if (!FieldValidation(actionForm)) {
        return false;
    }

    AddPassword($(actionForm).attr("playerid"), $(actionForm).find("[name=Input]").val());

    // continue with normal action
    return PerformAction(actionForm);
}

function EnterPassword(form) {
    var playerId = $(form).find("[name=PlayerId]").val();
    var password = $(form).find("[name=Password]").val();
    $.post("Authenticate", $(form).serialize())
        .done(function (data) {
            AddPassword(playerId, password);
            //$("#actions").find(".actionContainer[playerId=" + playerId + "]").append($(data).filter(".actions"));
            UpdateActionPasswords();
            $(form).find("[name=Password]").blur();
        })
        .fail(function () {
            $(form).find(".playerName").addClass("error");
            $(form).find(".playerNameInformer").text("Incorrect password");
            $(form).find("[name=Password]").focus();
        })
        .always(function () {
            $(form).find("[name=Password]").val("");
        });

    return false;
}

function AddPassword(playerId, password) {
    // add password element to the game if it doesn't exist already
    if ($(".playerPassword[playerid=" + playerId + "]").length < 1) {
        var passwordElement = $("<input/>", {
            type: "hidden",
            class: "playerPassword",
            playerid: playerId,
            password: password,
        });
        $("#game").append(passwordElement);
    }
}

function PlayerNameSetup() {
    $.each($(".playerName"), function (index, value) {
        var input = $(value).find("input");
        var button = $(value).find(".playerNameButton");

        input.blur(function () {
            setTimeout( function() {
                button.hide();
            },
            100);
        });

        input.focus(function () {
            button.show();
        });
    });
}

function SelectIcon(iconKey) {
    var image = $(iconKey).attr("image");
    var pieceId = $(iconKey).attr("pieceId");
    var form = $(iconKey).parents("form");
    form.find(".imageValue").val(pieceId);
    form.find(".imageSelect").attr("class", "imageSelect mif-2x " + image);
    form.find('.imagePad').toggle();
    SetPieceInputValue(form);
}

function SelectColor(colorKey) {
    var color = $(colorKey).attr("name");
    var colorId = $(colorKey).attr("colorId");
    var form = $(colorKey).parents("form");
    form.find(".colorValue").val(colorId);
    form.find(".colorSelect").attr("class", "colorSelect bg-" + color);
    form.find('.colorPad').toggle();
    SetPieceInputValue(form);
}

function SetPieceInputValue(form) {
    var pieceId = form.find(".imageValue").val();
    var colorId = form.find(".colorValue").val();
    var inputValue = form.find("[name=Input]").val(JSON.stringify({ PieceId: pieceId, ColorId: colorId }));
    if (pieceId && colorId)
    {
        form.find("button").prop("disabled", false);
    }
}