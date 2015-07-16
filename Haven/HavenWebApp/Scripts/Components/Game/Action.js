/** @jsx React.DOM */
/// <reference path="https://cdnjs.cloudflare.com/ajax/libs/react/0.13.3/react.js" />
/// <reference path="https://cdnjs.cloudflare.com/ajax/libs/react/0.13.3/JSXTransformer.js" />

function GetButtonClass() {
    return "button small-button bg-mauve bd-mauve bg-active-steel fg-white";
}

var Action = React.createClass({
    render: function () {
        return (
            <div className="padding5" style={{display: (this.props.password === null ? "none" : "block")}}>
                <form onSubmit={this.performAction} method="post" action="PerformAction">
                    {this.renderInput()}
                </form>
            </div>
        );
    },
    renderInput: function () {
        // styling for the button
        var buttonClass = GetButtonClass();

        if (this.props.action.RequiresInput) {
            if (this.props.action.Name === "SelectPiece") {
                // button with icon selector and color selector
                return (
                    <div>
                        <IconSelector ref="iconSelector" style={{float: "left", paddingRight: "5px"}} />
                        <ColorSelector ref="colorSelector" style={{float: "left", paddingRight: "5px"}} />
                        <button type="submit" className={buttonClass} disabled={this.state.input} style={{height: "100%"}}>{this.props.action.Text}</button>
                    </div>
                );
            }
            else {
                // input field with submit button
                return (
                    <div className="input-control text" data-role="input" style={{width: "100%"}}>
                        <input className={this.state.error ? "error" : ""}
                               style={{paddingRight: "62px", height: "80%"}}
                               value={this.state.input}
                               onChange={this.handleInput}
                               placeholder={this.state.error ? "Input is required" : this.props.action.Text}
                               type={this.props.action.Text == "Your password" ? "password" : "text"} />
                        <button className={buttonClass} type="submit" disabled={!this.state.input}>
                            <span className="mif-play"></span>
                        </button>
                        <br />
                    </div>
                );
            }
        }
        else {
            // button with icon
            return (
                <div>
                    <button type="submit" className={buttonClass + " image-button"} style={{width: "100%"}}>{this.props.action.Text}<span className={"icon " + this.props.action.Icon + " bg-" + this.props.action.Color} /></button>
                </div>
            );
        }
    },
    getInitialState: function () {
        return {input: null, error: false};
    },
    handleInput: function(event) {
        this.setState({input: event.target.value});
    },
    performAction: function(event) {
        event.preventDefault();

        // validate input if it is required
        var input = this.state.input;
        if (this.props.action.RequiresInput) {
            if (this.props.action.Name === "SelectPiece") {
                var pieceId = this.refs.iconSelector.value();
                var colorId = this.refs.colorSelector.value();
                if (pieceId && colorId) {
                    input = JSON.stringify({PieceId: pieceId, ColorId: colorId});
                }
                else {
                    if (!pieceId) {
                        this.refs.iconSelector.showError();
                    }
                    if (!colorId) {
                        this.refs.colorSelector.showError();
                    }
                    return;
                }
            }
            else {
                if (!input || input.length === 0 || !input.trim()) {
                    this.setState({error: true});
                    return;
                }
            }
        }

        $.post("PerformAction",
            {
                Id: this.props.action.Id,
                Password: this.props.password,
                Input: input
            })
            .done(function (data) {
                // callback handles returned data
                this.props.onUpdate(data);
            }.bind(this))
            .fail(function () {
                alert("Invalid attempt to perform action");
            });
    }
});