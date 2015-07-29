/** @jsx React.DOM */
/// <reference path="https://cdnjs.cloudflare.com/ajax/libs/react/0.13.3/react.js" />
/// <reference path="https://cdnjs.cloudflare.com/ajax/libs/react/0.13.3/JSXTransformer.js" />

var AdminBoards = React.createClass({
    render: function () {
        return (
            <div className="container page-content bg-white padding10">
                <NewBoardMenu />
                <BoardsMenu />
            </div>
        );
    }
});