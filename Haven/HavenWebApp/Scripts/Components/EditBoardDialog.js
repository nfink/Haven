/** @jsx React.DOM */
/// <reference path="https://cdnjs.cloudflare.com/ajax/libs/react/0.13.3/react.js" />
/// <reference path="https://cdnjs.cloudflare.com/ajax/libs/react/0.13.3/JSXTransformer.js" />

var OpenBoardDialog = React.createClass({
    render: function () {
        return (
            <div className="dialog" data-overlay="true" data-overlay-color="op-dark" data-close-button="true" data-role="dialog">
                <div style={{backgroundImage: "url('" + this.props.board.Image.Filepath + "')", width: "100%", height: "100%", position: "absolute", backgroundSize: "cover", backgroundRepeat: "no-repeat", opacity: 0.25}}></div>
                <div className="padding20">
                    <h2>{this.props.board.Name}</h2>
                    <p>{this.props.board.Description}</p>
                    <form onSubmit={this.editBoard} method="get" action="Admin/Board/Edit">
                        <div className="padding10">
                            {this.errors()}
                            {this.warnings()}
                        </div>
                        <div>
                            <button className="button" type="submit">Edit</button>
                            <button className="button" type="button" onClick={this.close}>Cancel</button>
                        </div>
                    </form>
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
    errors: function () {
        if ((this.state.validation != null) && (this.state.validation.Errors.length > 0)) {
            return (
                <div className="panel alert">
                    <div className="heading">
                        <span className="title">Error</span>
                    </div>
                    <div className="content">
                        <ul className="simple-list">
                            {this.state.validation.Errors.map(function(item, index){
                                return <li key={index}>{item}</li>
                            }, this)}
                        </ul>
                    </div>
                </div>
            );
        }
        else {
            return "";
        }
    },
    warnings: function () {
        if ((this.state.validation != null) && (this.state.validation.Warnings.length > 0)) {
            return (
                <div className="panel warning">
                    <div className="heading">
                        <span className="title">Warning</span>
                    </div>
                    <div className="content">
                        <ul className="simple-list">
                            {this.state.validation.Warnings.map(function(item, index){
                                return <li key={index}>{item}</li>
                            }, this)}
                        </ul>
                    </div>
                </div>
            );
        }
        else {
            return "";
        }
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