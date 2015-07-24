/** @jsx React.DOM */
/// <reference path="https://cdnjs.cloudflare.com/ajax/libs/react/0.13.3/react.js" />
/// <reference path="https://cdnjs.cloudflare.com/ajax/libs/react/0.13.3/JSXTransformer.js" />

var MessageArea = React.createClass({
    render: function () {
        var left = AdjustCoordinate(this.props.x, 70);
        var top = AdjustCoordinate(this.props.y, 70);
        var width = AdjustDimension(this.props.width, 70);
        var height = AdjustDimension(this.props.height, 70);

        return (
            <div className="tile-small bg-taupe fg-white"
                style={{position: "absolute", left: left, top: top, width: width, height: height}}>{this.formattedMessages()}</div>
        );
    },
    formattedMessages: function () {
        return (<div>
                    {this.props.messages.map(function(item, index){
                        return (<div key={index}>{item.Text}<br /></div>);
                    }, this)}
               </div>);
    }
});