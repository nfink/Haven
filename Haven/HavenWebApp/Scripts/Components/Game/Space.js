/** @jsx React.DOM */
/// <reference path="https://cdnjs.cloudflare.com/ajax/libs/react/0.13.3/react.js" />
/// <reference path="https://cdnjs.cloudflare.com/ajax/libs/react/0.13.3/JSXTransformer.js" />

var Space = React.createClass({
    render: function () {
        var textStyle = {display: "inline-block", verticalAlign: "middle", lineHeight: "70px"};
        if (this.props.image !== null) {
            var content = (<div>
                            <div style={{width: "100%", height: "100%", position: "absolute", backgroundImage: "url('" + this.props.image + "')", backgroundSize: "cover", backgroundRepeat: "no-repeat"}}></div>
                            <div style={textStyle}>{this.props.name}</div>
                          </div>);
        }
        else {
            var content = <div className={this.props.icon + " mif-2x"} style={textStyle} />;
        }
        return (
            <div className="tile-small bg-taupe fg-white" onClick={this.props.onClick} style={this.props.style}>
                <div className="tile-content" style={{textAlign: "center"}}>{content}</div>
            </div>
        );
    }
});