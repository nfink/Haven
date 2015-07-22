/** @jsx React.DOM */
/// <reference path="https://cdnjs.cloudflare.com/ajax/libs/react/0.13.3/react.js" />
/// <reference path="https://cdnjs.cloudflare.com/ajax/libs/react/0.13.3/JSXTransformer.js" />

var LoadingButton = React.createClass({
    render: function () {
        return (
            <button className={(this.props.className ? this.props.className : "button primary") + (this.state.loading ? " loading-cube" : "")} type={this.props.type ? this.props.type : "submit"} onClick={this.props.onClick} style={this.props.style}>{this.props.text}</button>
        );
    },
    getInitialState: function () {
        return {loading: false};
    },
    showLoading: function () {
        this.setState({loading: true});
    },
    hideLoading: function () {
        this.setState({loading: false});
    }
});


