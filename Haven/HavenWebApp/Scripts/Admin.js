/// <reference path="https://code.jquery.com/jquery-1.11.3.min.js" />

$(document).ready(function () {
    // set up click handlers
    $("#active").find(".board").click(function () {
        //BoardDialog($(this));
        var boardId = $(this).attr("boardId");
        $("#boardDialog").load("/Admin/Board/" + boardId, function () {
            $("#boardDialog").data("dialog").open();
        });
    });
});
