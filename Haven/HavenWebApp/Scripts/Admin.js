/// <reference path="https://code.jquery.com/jquery-1.11.3.min.js" />
/// <reference path=""~/Scripts/doT.js"" />

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

//$(document).ready(function () {
//    var stuff = { "Id": 1, "Question": "test question", Answers: [{ Id: 1, "Answer": "a" }, { Id: 2, "Answer": "b" }, { Id: 3, "Answer": "c" }] };
//    $.get("/Views/Admin/TestView1.html").done(function (data) {
//        var template = doT.template(data);
//        $("#test1").append(template(stuff));
//    });
//});

//angular.module("Admin", ["ngRoute"])

//.config(function ($routeProvider) {
//    $routeProvider
//      .when("/", {
//          controller: "PageController",
//          templateUrl: "/Views/Admin/Login.cshtml",
//      })
//      .when("/test2", {
//          controller: "PageController",
//          templateUrl: "/Views/Admin/Home4.cshtml"
//      })
//      .otherwise({
//          redirectTo: '/'
//      });
//})

//.controller('PageController', function () {

//});
