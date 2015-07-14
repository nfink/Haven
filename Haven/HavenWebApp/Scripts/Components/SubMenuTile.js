/** @jsx React.DOM */
/// <reference path="https://cdnjs.cloudflare.com/ajax/libs/react/0.13.3/react.js" />
/// <reference path="https://cdnjs.cloudflare.com/ajax/libs/react/0.13.3/JSXTransformer.js" />

var SubMenuTile = React.createClass({
    render: function () {
        return (
            <div className="tile fg-white bg-black" data-role="tile" onClick={this.props.onClick}>
                <div className="tile-content slide-down">
                    <div className="slide">
                        <div style={{width: "100%"}} className="image-container image-format-square">
                            <div className="frame">
                                <div style={{width: "100%", height: "150px", backgroundImage: "url('" + this.props.image + "')", backgroundSize: "cover", backgroundRepeat: "no-repeat", borderRadius: "0px"}}>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div className="slide-over op-cyan text-small padding10">{this.props.description}</div>
                    <div className="tile-label">{this.props.title}</div>
                </div>
            </div>
        );
    },
});