/** @jsx React.DOM */
/// <reference path="https://cdnjs.cloudflare.com/ajax/libs/react/0.13.3/react.js" />
/// <reference path="https://cdnjs.cloudflare.com/ajax/libs/react/0.13.3/JSXTransformer.js" />

var ConfirmationDialog = React.createClass({
    render: function () {
        return (
            <div style={{width: "auto", height: "auto"}} data-role="dialog" className="padding20 dialog" data-close-button="true" data-overlay="true" data-overlay-color="op-dark">
                <p>{this.props.text ? this.props.text : "Are you sure?"}</p>
                <div>
                    <button className="button" type="button" onClick={this.confirm}>Yes</button>
                    <button className="button" type="button" onClick={this.close}>No</button>
                </div>
            </div>
        );
    },
    open: function () {
        OpenDialog(this);
    },
    close: function () {
        CloseDialog(this);
    },
    confirm: function () {
        this.close();
        this.props.action();
    },
});

var DeleteDialog = React.createClass({
    render: function () {
        return (
            <ConfirmationDialog text="Are you sure you want to delete?" action={this.props.action} />
        );
    },
    open: function () {
        OpenDialog(this);
    },
    close: function () {
        CloseDialog(this);
    },
});