/** @jsx React.DOM */
/// <reference path="https://cdnjs.cloudflare.com/ajax/libs/react/0.13.3/react.js" />
/// <reference path="https://cdnjs.cloudflare.com/ajax/libs/react/0.13.3/JSXTransformer.js" />

var DeleteDialog = React.createClass({
    render: function () {
        return (
            <div style={{width: "auto", height: "auto"}} data-role="dialog" className="padding20 dialog" data-close-button="true" data-overlay="true" data-overlay-color="op-dark">
                <p>Are you sure you want to delete?</p>
                <div>
                    <button className="button" type="button">Yes</button>
                    <button className="button" type="button" onClick={this.close}>No</button>
                </div>
            </div>
        );
    },
    getInitialState: function () {
        return {validation: null};
    },
    componentDidMount: function () {
        $.get("Boards/" + this.props.board.Id + "/Validation", function (data) {
            this.setState({validation: JSON.parse(data)});
            this.open();
        }.bind(this));
    },
    open: function () {
        OpenDialog(this);
    },
    close: function () {
        CloseDialog(this);
    },
    editBoard: function (event) {
        event.preventDefault();
        this.close();
        $.get("Boards/" + this.props.board.Id, function (data) {
            page("/Boards/" + JSON.parse(data).Id);
        });
    },
});