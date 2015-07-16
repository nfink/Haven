/** @jsx React.DOM */
/// <reference path="https://cdnjs.cloudflare.com/ajax/libs/react/0.13.3/react.js" />
/// <reference path="https://cdnjs.cloudflare.com/ajax/libs/react/0.13.3/JSXTransformer.js" />

var SubMenuTile = React.createClass({
    render: function () {
        return (
            <div>
                <div style={{display: (this.props.disabled ? "block" : "none"), opacity: 0.7, backgroundColor: "gray", position: "absolute", width: 150, height: 150, left: 5, top: 5, zIndex: 9998}} />
                <div className="fg-white padding5" style={{display: (this.props.disabled ? "block" : "none"), position: "absolute", zIndex: 9999, left: 5, top: 5}}>(Incomplete)</div>
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
            </div>
        );
    },
});