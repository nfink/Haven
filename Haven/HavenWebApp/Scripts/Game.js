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
}

function PerformAction(actionForm) {
    if (!FieldValidation(actionForm)) {
        return false;
    }

    // hide actions so the player can't try to perform multiple actions
    $(".action").hide();

    $.post("PerformAction", $(actionForm).serialize())
        .done(function (data) {
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
                PlayerNameSetup();
                UpdateActionPasswords();

                // retain player selection
                $(".actionContainer[playerid=" + selectedPlayerId + "]").trigger("click");
            });
        })
        .fail(function () {
            $(".action").show();
            alert("invalid attempt to perform action");
        });

    return false;
}

function UpdateActionPasswords()
{
    // set passwords
    $.each($(".playerPassword"), function (index, value) {
        var playerId = $(value).attr("playerid");
        var password = $(value).attr("password");
        $(".actionContainer[playerid=" + playerId + "]").find(".action").find("[name=Password]").val(password);
        $(".actionContainer[playerid=" + playerId + "]").find(".playerName").find("input").hide();
        $(".actionContainer[playerid=" + playerId + "]").find(".playerNameButton").hide();
    });

    //// hide actions without a password
    //$.each($(".action"), function (index, value) {
    //    if (!($(value).find("[name=Password]").val())) {
    //        $(value).hide();
    //    }
    //    else
    //    {
    //        $(value).show();
    //    }
    //});
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
            $("#actions").find(".actionContainer[playerId=" + playerId + "]").append($(data).filter(".actions"));
            UpdateActionPasswords();
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
            name: "playerId" + playerId,
            value: password,
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