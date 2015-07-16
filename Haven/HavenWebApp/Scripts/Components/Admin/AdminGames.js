/** @jsx React.DOM */
/// <reference path="https://cdnjs.cloudflare.com/ajax/libs/react/0.13.3/react.js" />
/// <reference path="https://cdnjs.cloudflare.com/ajax/libs/react/0.13.3/JSXTransformer.js" />

var AdminGames = React.createClass({
    render: function () {
        return (
            <div className="container page-content">
                <div className="header">Games in progress</div>
                <GamesMenu />
                <div className="header">Create new game</div>
                <NewGameMenu />
            </div>
        );
    }
});