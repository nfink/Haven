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
        else if (this.state.boards.length < 1 ) {
            return (
                <div className="header">No existing boards</div>
            );
        }
        else {
            return (
                <div>
                    <div className="header">Ready for play</div>
                    <div className="tile-container padding10">
                        {this.state.boards.filter(function (value) {return value.Active}, this).map(function(item, index){
                            return <BoardsMenu.Tile board={item} key={item.Id} />;
                        }, this)}
                    </div>
                    <div className="header">Incomplete</div>
                    <div className="tile-container padding10">
                        {this.state.boards.filter(function (value) {return !value.Active}, this).map(function(item, index){
                            return <BoardsMenu.Tile board={item} key={item.Id} />;
                        }, this)}
                    </div>
                    <div id="editBoardDialog"></div>
                </div>
            );
        }
    },
    getInitialState: function () {
        return {boards: null};
    },
    componentDidMount: function () {
        $.get("/Boards", function (data) {
            this.setState({boards: JSON.parse(data)});
        }.bind(this));
    },
});

BoardsMenu.Tile = React.createClass({
    render: function () {
        return (
            <SubMenuTile title={this.props.board.Name} description={this.props.board.Description} image={this.props.board.Image? this.props.board.Image.Filepath : null} onClick={this.editBoard} />
        );
    },
    editBoard: function () {
        $.get("/Boards/" + this.props.board.Id, function (data) {
            page("/Boards/" + JSON.parse(data).Id);
        });
    },
    editBoardDialog: function () {
        var dialog = React.render(<OpenBoardDialog board={this.props.board} />, document.getElementById("editBoardDialog"));
        dialog.open();
    }
});