/** @jsx React.DOM */
/// <reference path="https://code.jquery.com/jquery-1.11.3.min.js" />
/// <reference path="https://cdn.rawgit.com/visionmedia/page.js/master/page.js" />
/// <reference path="https://cdnjs.cloudflare.com/ajax/libs/react/0.13.3/react.js" />
/// <reference path="https://cdnjs.cloudflare.com/ajax/libs/react/0.13.3/JSXTransformer.js" />

$(function () {
    // set up routing
    // main menu
    page('/', function () {
        $("#backButton").hide();
        React.render(<GameMainMenu />, document.body);
    });
    page.exit("/", function (ctx, next) {
        React.unmountComponentAtNode(document.body);
        next();
    });

    // new game menu
    page("/Boards", function () {
        $("#backButton").show();
        React.render(<BoardsMenu />, document.body);
    });
    page.exit("/Boards", function (ctx, next) {
        React.unmountComponentAtNode(document.body);
        next();
    });

    // continue game menu
    page("/Games", function () {
        $("#backButton").show();
        React.render(<GamesMenu />, document.body)
    });
    page.exit("/Games", function (ctx, next) {
        React.unmountComponentAtNode(document.body);
        next();
    });

    // game
    page("/Games/:id", function (ctx, next) {
        $("#backButton").show();
        React.render(<Game id={ctx.params.id} />, document.body;)
    });
    page.exit("/Games/:id", function (ctx, next) {
        React.unmountComponentAtNode(document.body);
        next();
    });

    // fallback
    page("*", function () {
        alert("nothing found");
    });

    page({
        hashbang: true
    });
});

function FieldValidation(form) {
    var failures = $(form).find(".requiredField").filter(function (index, element) {
            return !($(this).val());
        })
        .addClass("error")
        .attr("placeholder", function (index, attr) {
            return $(this).attr("name") + " is required";
        }).length;
    return failures == 0;
}
