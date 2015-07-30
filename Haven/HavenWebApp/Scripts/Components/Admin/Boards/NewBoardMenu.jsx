/** @jsx React.DOM */
/// <reference path="https://cdnjs.cloudflare.com/ajax/libs/react/0.13.3/react.js" />

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
        $.post("Boards", function (data) {
            page("/Boards/" + JSON.parse(data).Id);
        });
    },
    copyBoard: function () {
        $.get("Boards", function (data) {
            var dialog = React.render(<NewBoardMenu.CopyBoardDialog boards={JSON.parse(data)} />, document.getElementById("copyBoardDialog"));
            dialog.open();
        }.bind(this));
    }
});

NewBoardMenu.CopyBoardDialog = React.createClass({
    render: function () {
        if ((this.props.boards === null) || (this.props.boards.length < 1 )) {
            var content = <div className="header">No existing boards</div>;
        }
        else {
            var content = (
                <div>
                    <div className="header">Select a board to copy</div>
                    <div className="tile-container padding10" style={{margin: 10}}>
                        {this.props.boards.map(function(item, index){
                            return <SubMenuTile title={item.Name} description={item.Description} image={item.Image? item.Image.Filepath : null} key={item.Id} onClick={function () { this.copyBoard(item); }.bind(this)} />;
                        }, this)}
                    </div>
                </div>
            );
        }
        return (
            <div className="dialog" data-overlay="true" data-overlay-color="op-dark" data-close-button="true" data-role="dialog">
                <div className="padding20">
                    {content}
                    <button className="button" type="button" onClick={this.close}>Cancel</button>
                </div>
            </div>
        );
    },
    open: function () {
        OpenDialog(this);
    },
    close: function () {
        CloseDialog(this);
        React.unmountComponentAtNode(document.getElementById("copyBoardDialog"));
    },
    copyBoard: function (board) {
        $.post("Boards/" + board.Id + "/Copy", function (data) {
            this.close();
            page("/Boards/" + JSON.parse(data).Id);
        }.bind(this));
    }
});