/** @jsx React.DOM */
/// <reference path="https://cdnjs.cloudflare.com/ajax/libs/react/0.13.3/react.js" />
/// <reference path="https://cdnjs.cloudflare.com/ajax/libs/react/0.13.3/JSXTransformer.js" />

var Space = React.createClass({
    render: function () {
        return (
            <div className="space tile-small bg-taupe fg-white" style={{position: "absolute"}}>
                <div className="tile-content" style={{text-align: "center"}}>
                    @if (!string.IsNullOrWhiteSpace(image))
                    {
                        <div style={{width: "100%", height: "100%", position: "absolute", backgroundImage: "url('" + @image + "')", backgroundSize: "cover", backgroundRepeat: "no-repeat"}}></div>
                    }
                <div className="@additionalClass" style={{display: "inline-block", verticalAlign: "middle", lineHeight: "70px"}}>{this.props.space.Name}</div>
                </div>
            </div>
        );
    }
});