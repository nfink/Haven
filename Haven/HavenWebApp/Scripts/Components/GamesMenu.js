/** @jsx React.DOM */
/// <reference path="https://cdnjs.cloudflare.com/ajax/libs/react/0.13.3/react.js" />
/// <reference path="https://cdnjs.cloudflare.com/ajax/libs/react/0.13.3/JSXTransformer.js" />

var GamesMenu = React.createClass({
    render: function () {
        if (this.state.games === null) {
            return (
                <div className="padding20" data-role="preloader" data-type="metro" data-style="dark"></div>
            );
        }
        else {
            return (
                <div className="tile-container padding10">
                    {this.state.games.length < 1 ?
                        <div className="header">No games in progress</div>
                        :
                        this.state.games.map(function(item, index){
                            return <GamesMenu.Tile game={item} key={item.Id} />;
                        }, this)
                    }
                </div>
            );
        }
    },
    getInitialState: function () {
        return {games: null};
    },
    componentDidMount: function () {
        $.get("Games", function (data) {
            this.setState({games: JSON.parse(data)});
        }.bind(this));
    },
});

GamesMenu.Tile = React.createClass({
    render: function () {
        return (
            <SubMenuTile title={this.props.game.Name} description={this.props.game.Players.map(function (player, index) { return <div key={index}>{player.Name === null ? "(New Player)" : player.Name}<br /></div>})} image={this.props.game.Board.Image.Filepath} onClick={this.openGame} />
        );
    },
    openGame: function () {
        page('/Games/' + this.props.game.Id);
    },
});