/** @jsx React.DOM */
/// <reference path="https://cdnjs.cloudflare.com/ajax/libs/react/0.13.3/react.js" />
/// <reference path="https://cdnjs.cloudflare.com/ajax/libs/react/0.13.3/JSXTransformer.js" />

var Space = React.createClass({
    render: function () {
        var textStyle = {display: "inline-block", verticalAlign: "middle", lineHeight: "70px"};
        if (this.props.space.Image !== null) {
            var content = (<div>
                            <div style={{width: "100%", height: "100%", position: "absolute", backgroundImage: "url(" + this.props.space.Image + ")", backgroundSize: "cover", backgroundRepeat: "no-repeat"}}></div>
                            <div style={textStyle}>{this.props.space.Name}</div>
                          </div>);
        }
        else {
            var content = <div className={this.props.space.Icon ? this.props.space.Icon.Image + " mif-2x" : null} style={textStyle} />;
        }
        return (
            <div className={"tile-small" + (this.props.space.BackgroundColor ? " bg-" + this.props.space.BackgroundColor.Name : " bg-taupe") + (this.props.space.TextColor ? " fg-" + this.props.space.TextColor.Name : " fg-white")} onClick={this.props.onClick} style={this.props.style}>
                <div className="tile-content" style={{textAlign: "center"}}>{content}</div>
            </div>
        );
    }
});