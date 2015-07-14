/** @jsx React.DOM */
/// <reference path="https://cdnjs.cloudflare.com/ajax/libs/react/0.13.3/react.js" />
/// <reference path="https://cdnjs.cloudflare.com/ajax/libs/react/0.13.3/JSXTransformer.js" />

var GameMainMenu = React.createClass({
    render: function () {
        return (
            <div className="tile-container padding10" style={{width: "680px", margin: "auto"}}>
                <MainMenuTile title="New Game"
                          description="Start a new game"
                          image="/Content/Images/board_new_square.jpg"
                          onClick={function() {page('/Boards');}} />
                <MainMenuTile title="Continue Game"
                          description="Continue an unfinished game"
                          image="/Content/Images/board_continue_square.jpg"
                          onClick={function() {page('/Games');}} />
            </div>
        );
    }
});