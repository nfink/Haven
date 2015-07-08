/// <reference path="https://code.jquery.com/jquery-1.11.3.min.js" />

$(document).ready(function () {
    AdjustBoardDimensions($(".space, #messagearea, #statusarea"), $("#messagearea, #statusarea"));
    // add borders to dummy spaces (do this after adjusting dimensions to avoid throwing off size calculations)
    $(".dummySpace, .dummyPiece, .challenge, .dummyChallenge").css({ borderStyle: "solid", borderWidth: "1px" });

    $(".space").click(function () {
        SelectSpace(this);
    });
});

function EditBoard(form) {
    var formData = new FormData(form);
    $.ajax({
        url: "/Admin/Board/Edit",
        type: 'POST',
        data: formData,
        contentType: false,
        processData: false
    });

    return false;
}

function SelectChallenge(challenge) {
    $.get("/Admin/Challenge/Edit",
        {
            ChallengeId: $(challenge).attr("challengeid"),
            BoardId: $("#BoardId").val(),
        },
        function (data) {
            $("#editPane").html(data);
        });
}

function EditChallenge(form) {
    var challengeForm = $(form);
    var answers = [];
    $(".answer").each(function () {
        var answer = $(this).find("[name=Answer]").val();
        var correct = $(this).find("[name=Correct]").prop("checked");
        answers.push({ Answer: answer, Correct: correct });
    });

    $.post("/Admin/Challenge/Edit",
        {
            Id: challengeForm.find("[name=Id]").val(),
            BoardId: challengeForm.find("[name=BoardId]").val(),
            Question: challengeForm.find("[name=Question]").val(),
            Answers: JSON.stringify(answers),
        },
        function (data) {
            // update the displayed challenge or add a new one if needed
            var challengeId = challengeForm.find("[name=Id]").val();
            if (challengeId == 0) {
                $("#challenges").append(data);
            }
            else {
                var piece = $("#challenges").find("[challengeid=" + challengeId + "]");
                piece.replaceWith(data);
        }
    });

    return false;
}

function DeleteChallenge(id) {
    $.ajax({
        url: "/Admin/Challenge/" + id,
        method: "DELETE",
        success: function () {
            // clear the edit pane
            $('#editPane').empty();

            // remove from the challenges area
            $(".challenge[challengeid=" + id + "]").remove();
        }
    });
}

function AddAnswer(element) {
    element.append('<div class="answer"><button class="button small mini-button danger rounded" type="button" onclick="$(this).parent().remove();"><span class="mif-minus"></span></button><div class="input-control text"><input name="Answer" type="text" placeholder="Enter answer here..." /></div><label class="input-control checkbox"><input type="checkbox" name="Correct"><span class="check"></span><span class="caption">Correct</span></label></div>');
}

function SelectSpace(space) {
    $.get("/Admin/Space/Edit",
        {
            SpaceId: $(space).attr("spaceid"),
            BoardId: $("#BoardId").val(),
            X: $(space).attr("x"),
            Y: $(space).attr("y"),
        },
        function (data) {
            $("#editPane").html(data);
        });
}

function EditSpace(form) {
    var formData = new FormData(form);
    $.ajax({
        url: "/Admin/Space/Edit",
        type: 'POST',
        data: formData,
        contentType: false,
        processData: false,
    }).done(function (data) {
            // update the displayed space or add a new one if needed
            var spaceId = $(form).find("[name=Id]").val();
            if (spaceId == 0) {
                $("#spaces").append(data);
            }
            else {
                var space = $("#board").find("[spaceid=" + spaceId + "]");
                space.replaceWith(data);
            }
            spaceId = $(data).attr("spaceId");
            // update position of space
            AdjustBoardDimensions($("[spaceid=" + spaceId + "]"), $(null));
            // add click handler
            $("[spaceid=" + spaceId + "]").click(function () {
                SelectSpace(this);
            });
        });

    return false;
}

function DeleteSpace(id) {
    $.ajax({
        url: "/Admin/Space/" + id,
        method: "DELETE",
        success: function () {
            // clear the edit pane
            $('#editPane').empty();

            // remove from the spaces grid
            $(".space[spaceid=" + id + "]").remove();
        }
    });
}

// from http://stackoverflow.com/questions/4459379/preview-an-image-before-it-is-uploaded
function PreviewImage(input, imageElement) {
    if (input.files && input.files[0]) {
        var reader = new FileReader();

        reader.onload = function (e) {
            imageElement.css("background-image", "url(" + e.target.result + ")");
        }

        reader.readAsDataURL(input.files[0]);
    }
}

function ExpandToggle(toggleButton, elementToToggle) {
    elementToToggle.toggle();
    var button = $(toggleButton);
    if (button.find('span').hasClass('mif-expand-less')) {
        button.find('span').removeClass('mif-expand-less');
        button.find('span').addClass('mif-expand-more');
    }
    else {
        button.find('span').removeClass('mif-expand-more');
        button.find('span').addClass('mif-expand-less');
    }
}

function ConfirmDelete(deleteFunction) {
    $("#confirmDelete").off("click");
    $("#confirmDelete").click(function () {
        deleteFunction();
        $('#deleteDialog').data('dialog').close();
    });
    $("#deleteDialog").data("dialog").open();
}