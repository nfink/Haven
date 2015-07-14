/** @jsx React.DOM */
/// <reference path="https://cdnjs.cloudflare.com/ajax/libs/react/0.13.3/react.js" />
/// <reference path="https://cdnjs.cloudflare.com/ajax/libs/react/0.13.3/JSXTransformer.js" />

var BoardsMenu = React.createClass({
    render: function () {
        if (this.state.boards === null) {
            return (
                <div className="padding20" data-role="preloader" data-type="metro" data-style="dark"></div>
            );
        }
        else {
            return (
                <div>
                    <div className="tile-container padding10" style={{width: "680px", margin: "auto"}}>
                        {this.state.boards.map(function(item, index){
                            return <BoardsMenu.Tile board={item} key={index} />;
                        }, this)}
                    </div>
                    <div id="newGameDialog"></div>
                </div>
            );
        }
    },
    getInitialState: function () {
        return {boards: null};
    },
    componentDidMount: function () {
        $.get("Boards", function (data) {
            this.setState({boards: JSON.parse(data)});
        }.bind(this));
    },
});

BoardsMenu.Tile = React.createClass({
    render: function () {
        return (
            <SubMenuTile title={this.props.board.Name} description={this.props.board.Description} image={this.props.board.Image.Filepath} onClick={this.newGameDialog} />
        );
    },
    newGameDialog: function () {
        var dialog = React.render(<NewGameDialog title={this.props.board.Name} description={this.props.board.Description} image={this.props.board.Image.Filepath} boardId={this.props.board.Id} />, document.getElementById("newGameDialog"));
    dialog.open();
    },
});