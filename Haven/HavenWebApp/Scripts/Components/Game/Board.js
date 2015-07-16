/** @jsx React.DOM */
/// <reference path="https://cdnjs.cloudflare.com/ajax/libs/react/0.13.3/react.js" />
/// <reference path="https://cdnjs.cloudflare.com/ajax/libs/react/0.13.3/JSXTransformer.js" />

var Board = React.createClass({
    render: function () {
        return (
            <div>
                <div id="spaces">
                    {this.props.board.Spaces.map(function(item, index){
                        return <Board.Space space={item} key={index} />;
                    }, this)}
                </div>
            </div>
        );
    }
});

Board.Space = React.createClass({
    render: function () {
        // convert coordinates to pixel offsets
        var left = AdjustCoordinate(this.props.space.X, 70);
        var top = AdjustCoordinate(this.props.space.Y, 70);
        return (
            <Space name={this.props.space.Name} image={this.props.space.Image} icon={this.props.space.Icon} style={{position: "absolute", left: left, top: top}} />
        );
    }
});