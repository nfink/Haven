﻿/** @jsx React.DOM */
/// <reference path="https://cdnjs.cloudflare.com/ajax/libs/react/0.13.3/react.js" />

var Game = React.createClass({
    render: function () {
        if (this.state.game === null) {
            return (
                <div className="padding20" data-role="preloader" data-type="metro" data-style="dark"></div>
            );
        }
        else {
            var selectedPlayer = this.state.players.filter(function (value) { return ((value.Id !== null) && (value.Id === this.state.selectedPlayerId)); }, this)[0];
            var boardWidth = AdjustCoordinate(Math.max.apply(null, this.state.game.Board.Spaces.map(function (item, index) { return item.X; })), 70);
            return (
                <div>
                    <div id="board" style={{position: "absolute", left: 20, width: boardWidth}}>
                        <Board board={this.state.game.Board} />
                        <div id="pieces">
                            {this.state.players.map(function (item, index) {
                                return <Piece player={item} ref={"piece" + item.Id} key={item.Id} />;
                            }, this)}
                        </div>
                    </div>
                    <div id="actions" className="padding5" style={{display: "flex", flexWrap: "wrap", position: "absolute", left: boardWidth + 110}}>
                        {this.state.players.map(function (item, index) {
                            return <PlayerPanel player={item} ref={"player" + item.Id} onUpdate={this.update} onClick={this.selectPlayer} selected={this.state.selectedPlayerId === item.Id} key={item.Id} />;
                        }, this)}
                    </div>
                </div>
            );
        }
    },
    getInitialState: function () {
        return {game: null, players: [], selectedPlayerId: null};
    },
    componentDidMount: function () {
        $.get("/Games/" + this.props.id, function (data) {
            var gameState = JSON.parse(data);
            this.setState({game: gameState, players: gameState.Players});
        }.bind(this));
    },
    selectPlayer: function(player) {
        // highlight player panel
        this.setState({selectedPlayerId: player.props.player.Id});

        // display messages and cards

    },
    update: function (newGameState) {

        var newGameState = JSON.parse(newGameState);

        // update pieces

        newGameState.map(function (item, index) {
            this.refs["piece" + item.Id].moveTo(item.Space);
        }, this);


        // update player details and actions after pieces have moved
        //$(".piece").promise().done(function () {
        //    this.state.game.Players = newGameState.Players;
        //});

        this.setState({players: newGameState});

        //// update player details and remove used actions before moving pieces
        //var actions = elements.filter("#actions").children();
        //UpdatePlayerDetails(actions);
        //RemoveOldActions(actions);

        //UpdatePieces(elements.filter("#pieces").children());

        //// update after pieces have moved
        
        //    // current player selection
        //    var selectedPlayerId = $(".actionContainer.element-selected").attr("playerid");

        //    // update messages
        //    $("#messages").empty();
        //    $("#messages").append(elements.filter("#messages").children());

        //    // update cards
        //    $("#cards").empty();
        //    $("#cards").append(elements.filter("#cards").children());

        //    // update actions
        //    AddNewActions(actions);

        //    // update displayed details and messages
        //    UpdateMessagesAndStatus(selectedPlayerId);
        //});
    }
});