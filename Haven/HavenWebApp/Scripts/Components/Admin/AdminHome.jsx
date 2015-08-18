/** @jsx React.DOM */
/// <reference path="https://cdnjs.cloudflare.com/ajax/libs/react/0.13.3/react.js" />

var AdminHome = React.createClass({
    render: function () {
        return (
            <div className="container page-content">
                <div className="bg-white padding10">
                    <div>Welcome to Safe Haven, the easy board game creator!</div>
                    <br />
                    <div>Safe Haven lets you create dice-rolling trivia games, with your own questions, board designs, and play options.</div>
                    <br />
                    <div>Click on the Questions link to start creating your questions. They can be multiple choice or open-ended. Once you have created a question, it can be reused in any number of your board designs.</div>
                    <br />
                    <div>Click on the Boards link to start creating your game boards. For each board you can set up how it looks, select which questions to use, and choose from a variety of play options.</div>
                    <br />
                    <div>Click on the Games link to create a new game using one of your boards, or to monitor any games in progress.</div>
                    <br />
                    <div>Click on the Statistics link to see a history of completed games, as well as data on how your players have been answering your questions.</div>
                </div>
            </div>
        );
    },
});