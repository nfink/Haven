/** @jsx React.DOM */
/// <reference path="https://cdnjs.cloudflare.com/ajax/libs/react/0.13.3/react.js" />
/// <reference path="https://cdnjs.cloudflare.com/ajax/libs/react/0.13.3/JSXTransformer.js" />

var Board = React.createClass({
    render: function () {
        return (
            <div>
                <div id="spaces">
                    {this.props.board.Spaces.map(function(item, index){
                        return <Space space={item} key={index} />;
                    }, this)}
                </div>
                <div id="messagearea"
                     className="tile-small bg-taupe fg-white"
                     width={this.props.board.MessageAreaWidth}
                     height={this.props.board.MessageAreaHeight}
                     x={this.props.board.MessageAreaX}
                     y={this.props.board.MessageAreaY}
                     style={{position: "absolute"}}></div>
                <div id="statusarea"
                     className="tile-small tile-container bg-taupe fg-white"
                     width={this.props.board.StatusAreaWidth}
                     height={this.props.board.StatusAreaHeight}
                     x={this.props.board.StatusAreaX}
                     y={this.props.board.StatusAreaY}
                     style={{position: "absolute"}}></div>
            </div>
        );
    }
});