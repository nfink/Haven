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
    var formData = new FormData($(form)[0]);
    $.ajax({
        url: "/Admin/Board/Edit",
        type: 'POST',
        xhr: function () {
            return $.ajaxSettings.xhr();
        },
        data: formData,
        cache: false,
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

function AddAnswer(element) {
    element.append('<div class="answer"><input name="Answer" value="" /><input name="Correct" type="checkbox" /><button class="button" type="button" onclick="$(this).parent().remove();">Remove</button></div>');
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
    var spaceForm = $(form);

    $.post("/Admin/Space/Edit",
        {
            Id: spaceForm.find("[name=Id]").val(),
            BoardId: spaceForm.find("[name=BoardId]").val(),
            Type: spaceForm.find("[name=Type]").val(),
            Order: spaceForm.find("[name=Order]").val(),
            X: spaceForm.find("[name=X]").val(),
            Y: spaceForm.find("[name=Y]").val(),
            Recall: JSON.stringify({
                Text: spaceForm.find("[name=RecallText]").val(),
            }),
            NameCard: JSON.stringify({
                Name: spaceForm.find("[name=NameCardName]").val(),
                Details: spaceForm.find("[name=NameCardDetails]").val(),
                Image: spaceForm.find("[name=NameCardImage]").val(),
            }),
            SafeHavenCard: JSON.stringify({
                Name: spaceForm.find("[name=SafeHavenCardName]").val(),
                Details: spaceForm.find("[name=SafeHavenCardDetails]").val(),
                Image: spaceForm.find("[name=SafeHavenCardImage]").val(),
            }),
        },
        function (data) {
            // update the displayed space or add a new one if needed
            var spaceId = spaceForm.find("[name=Id]").val();
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