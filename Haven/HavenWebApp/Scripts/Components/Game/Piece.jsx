/** @jsx React.DOM */
/// <reference path="https://cdnjs.cloudflare.com/ajax/libs/react/0.13.3/react.js" />
/// <reference path="https://cdnjs.cloudflare.com/ajax/libs/react/0.13.3/JSXTransformer.js" />

var Piece = React.createClass({
    render: function () {
        var left = 35 + AdjustCoordinate(this.state.left, 70);
        var top = 35 + AdjustCoordinate(this.state.top, 70);
        var icon = (this.props.player.Piece !== null ? " mif-lg " + this.props.player.Piece.Image : "" );
        var color = (this.props.player.Color !== null ? " bg-" + this.props.player.Color.Name : "");

        return (
            <div className={"piece" + color}
                 style={{display: (this.props.player.Piece === null ? "none" : "block"),
                         position: "absolute",
                         width: 40,
                         height: 40,
                         left: left,
                         top: top,
                         borderRadius: "50%",
                         transform: "translate(-10px, -10px)",
                         boxShadow: "1px 1px 5px black",
                         zIndex: 999}}>
                <span className={"icon" + icon + color}
                      style={{position: "relative", top: "50%", left: "50%", transform: "translate(-50%, -50%)"}} />
            </div>
        );
    },
    getInitialState: function () {
        return {left: this.props.player.Space.X, top: this.props.player.Space.Y};
    },
    moveTo: function (space) {
        $(React.findDOMNode(this)).animate({left: 35 + AdjustCoordinate(space.X, 70), top: 35 + AdjustCoordinate(space.Y, 70)}, 250, function () {this.setState({left: space.X, top: space.Y});}.bind(this));
        //piece.animate({ left: nextSpacePosition.left + (nextSpace.width() / 2), top: nextSpacePosition.top + (nextSpace.height() / 2) }, 250, MovePiece);
    }
});