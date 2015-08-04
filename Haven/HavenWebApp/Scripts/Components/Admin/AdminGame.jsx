/** @jsx React.DOM */
/// <reference path="https://cdnjs.cloudflare.com/ajax/libs/react/0.13.3/react.js" />

var AdminGame = React.createClass({
    render: function () {
        return (
            <div>
                <div className="container page-content">
                    <div className="bg-white padding10">
                        <div>Game link: <a href={"/Play/" + this.props.id}>{window.location.origin + "/Play/" + this.props.id}</a></div>
                    </div>
                </div>
                <div className="padding10"></div>
                <Game id={this.props.id} />
            </div>
        );
    },
});