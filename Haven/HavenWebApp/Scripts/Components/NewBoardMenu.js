/** @jsx React.DOM */
/// <reference path="https://cdnjs.cloudflare.com/ajax/libs/react/0.13.3/react.js" />
/// <reference path="https://cdnjs.cloudflare.com/ajax/libs/react/0.13.3/JSXTransformer.js" />

var NewBoardMenu = React.createClass({
    render: function () {
        return (
            <div>
                <div className="header">Create new board</div>
                <div className="tile-container padding10">
                    <SubMenuTile title="New" description="Start from scratch with an empty board" image="/Content/Images/Go_Board_Empty_Square.jpg" onClick={this.newBoard} />
                    <SubMenuTile title="Copy" description="Start from a copy of an existing board" image="/Content/Images/Go_Board_Full_Square.jpg" onClick={this.copyBoard} />
                </div>
                <div id="copyBoardDialog"></div>
            </div>
        );
    },
    newBoard: function () {

    },
    copyBoard: function () {

    }
});

//NewBoardMenu.Tile = React.createClass({
//    render: function () {
//        return (
//            <SubMenuTile title={this.props.board.Name} description={this.props.board.Description} image={this.props.board.Image.Filepath} disabled={!this.props.board.Active} onClick={this.newGameDialog} />
//        );
//    },
//    newGameDialog: function () {
//        var dialog = React.render(<NewGameDialog title={this.props.board.Name} description={this.props.board.Description} image={this.props.board.Image.Filepath} boardId={this.props.board.Id} />, document.getElementById("newGameDialog"));
//        dialog.open();
//    },
//});