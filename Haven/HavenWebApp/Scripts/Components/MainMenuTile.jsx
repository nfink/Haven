/** @jsx React.DOM */
/// <reference path="https://cdnjs.cloudflare.com/ajax/libs/react/0.13.3/react.js" />
/// <reference path="https://cdnjs.cloudflare.com/ajax/libs/react/0.13.3/JSXTransformer.js" />

var MainMenuTile = React.createClass({
    render: function () {
        return (
            <div className="tile-large" data-role="tile" onClick={this.props.onClick}>
                <div className="tile-content">
                    <div className="image-container">
                        <div className="frame">
                            <img src={this.props.image} />
                        </div>
                        <div className="image-overlay op-cyan">
                            <h2>{this.props.description}</h2>
                            <p></p>
                        </div>
                    </div>
                    <div className="tile-label header fg-white">{this.props.title}</div>
                </div>
            </div>
        );
    },
});