/** @jsx React.DOM */
/// <reference path="https://cdnjs.cloudflare.com/ajax/libs/react/0.13.3/react.js" />
/// <reference path="https://cdnjs.cloudflare.com/ajax/libs/react/0.13.3/JSXTransformer.js" />

var WarningPanel = React.createClass({
    render: function () {
        return (
            <div className="panel warning">
                <div className="heading">
                    <span className="title">{this.props.title ? this.props.title : "Warning"}</span>
                </div>
                <div className="content">
                    <ul className="simple-list">
                        {this.props.warnings.map(function(item, index){
                            return <li key={index}>{item}</li>
                        }, this)}
                    </ul>
                </div>
            </div>
        );
    }
});