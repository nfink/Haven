/** @jsx React.DOM */
/// <reference path="https://cdnjs.cloudflare.com/ajax/libs/react/0.13.3/react.js" />
/// <reference path="https://cdnjs.cloudflare.com/ajax/libs/react/0.13.3/JSXTransformer.js" />

var EditBoard = React.createClass({
    render: function () {
        if (this.state.board === null) {
            return (
                <div className="padding20" data-role="preloader" data-type="metro" data-style="dark"></div>
            );
        }
        else {
            return (
                <div>
                    <Board board={this.state.board} />

                </div>
            );
        }
    },
    getInitialState: function () {
        return {board: null};
    },
    componentDidMount: function () {
        $.get("Boards/" + this.props.id, function (data) {
            var board = JSON.parse(data);
            this.setState({board: board});
        }.bind(this));
    },
});