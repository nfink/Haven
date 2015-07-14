/** @jsx React.DOM */
/// <reference path="https://cdnjs.cloudflare.com/ajax/libs/react/0.13.3/react.js" />
/// <reference path="https://cdnjs.cloudflare.com/ajax/libs/react/0.13.3/JSXTransformer.js" />

var Game = React.createClass({
    render: function () {
        if (this.state.game === null) {
            return (
                <div className="padding20" data-role="preloader" data-type="metro" data-style="dark"></div>
            );
        }
        else {
            return (
                <div>
                    <div id="board" style={{position: "absolute", left: "80px", width: "720px"}}>
                        <Board board={this.state.game.Board} />
                    </div>
                    <div id="pieces">
                        @Html.Partial("Pieces", @Model)
                    </div>
                    <div id="actions" class="padding5" style="position:absolute;left: 820px;">
                        @Html.Partial("Actions", @Model)
                    </div>
                    <div id="cards" style="display: none;">
                        @Html.Partial("Cards", @Model)
                    </div>
                    <div id="messages" style="display: none;">
                        @Html.Partial("Messages", @Model)
                    </div>
                </div>
            );
        }
    },
    getInitialState: function () {
        return {game: null};
    },
    componentDidMount: function () {
        $.get("Games/" + this.props.id, function (data) {
            this.setState({game: JSON.parse(data)});
            SetupGame(data);
        }.bind(this));
    },
});