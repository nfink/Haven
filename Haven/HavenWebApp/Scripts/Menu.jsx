/** @jsx React.DOM */
/// <reference path="https://code.jquery.com/jquery-1.11.3.min.js" />
/// <reference path="https://cdn.rawgit.com/visionmedia/page.js/master/page.js" />
/// <reference path="https://cdnjs.cloudflare.com/ajax/libs/react/0.13.3/react.js" />

$(function () {
    // set up routing
    // main menu
    page('/', function () {
        React.render(<AdminHome />, document.getElementById("content"));
    });
    page.exit("/", function (ctx, next) {
        React.unmountComponentAtNode(document.getElementById("content"));
        next();
    });

    // games menu
    page("/Games", function () {
        React.render(<AdminGames />, document.getElementById("content"))
    });
    page.exit("/Games", function (ctx, next) {
        React.unmountComponentAtNode(document.getElementById("content"));
        next();
    });

    // game
    page("/Games/:id", function (ctx, next) {
        React.render(<Game id={ctx.params.id} />, document.getElementById("content"));
    });
    page.exit("/Games/:id", function (ctx, next) {
        React.unmountComponentAtNode(document.getElementById("content"));
        next();
    });

    // statistics page
    page("/Statistics", function () {
        React.render(<AdminStatistics />, document.getElementById("content"));
    });
    page.exit("/Statistics", function (ctx, next) {
        React.unmountComponentAtNode(document.getElementById("content"));
        next();
    });

    // boards menu
    page("/Boards", function () {
        React.render(<AdminBoards />, document.getElementById("content"));
    });
    page.exit("/Boards", function (ctx, next) {
        React.unmountComponentAtNode(document.getElementById("content"));
        next();
    });

    // board editor
    page("/Boards/:id", function (ctx, next) {
        React.render(<EditBoard id={ctx.params.id} />, document.getElementById("content"));
    });
    page.exit("/Boards/:id", function (ctx, next) {
        React.unmountComponentAtNode(document.getElementById("content"));
        next();
    });

    // questions menu
    page("/Questions", function () {
        React.render(<AdminQuestions />, document.getElementById("content"));
    });
    page.exit("/Questions", function (ctx, next) {
        React.unmountComponentAtNode(document.getElementById("content"));
        next();
    });

    // profile page
    page("/Profile", function () {
        React.render(<AdminProfile />, document.getElementById("content"));
    });
    page.exit("/Profile", function (ctx, next) {
        React.unmountComponentAtNode(document.getElementById("content"));
        next();
    });

    // fallback
    page("*", function () {
        alert("nothing found");
    });

    page({
        hashbang: true
    });
    //page();
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

function AdjustCoordinate(coordinate, size) {
    return (coordinate - 1) * size * 1.1;
}

function AdjustDimension(dimension, size) {
    return ((dimension - 1) * (size * 1.1)) + size;
}