﻿/** @jsx React.DOM */
/// <reference path="https://cdnjs.cloudflare.com/ajax/libs/react/0.13.3/react.js" />

function GetButtonClass() {
    return "button small-button bg-mauve bd-mauve bg-active-steel fg-white";
}

var Action = React.createClass({
    render: function () {
        return (
            <div className="padding5" style={{display: (this.props.password === null ? "none" : "block")}}>
                <form onSubmit={this.performAction}>
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
            else if (this.props.action.Challenge !== null) {
                if (this.props.action.Challenge.OpenEnded) {
                    // input field for open ended question
                    return (
                        <div className="fg-white padding5">
                            <div>{this.props.action.Challenge.Question}</div>
                            <div className="input-control text" data-role="input" style={{width: "100%"}}>
                                <input className={this.state.error ? "error" : ""}
                                       style={{paddingRight: "62px", height: "80%"}}
                                       value={this.state.input}
                                       onChange={this.handleInput}
                                       placeholder={this.state.error ? "Answer is required" : this.props.action.Text}
                                       type="text" />
                                <button className={buttonClass} type="submit" disabled={!this.state.input}>
                                    <span className="mif-play"></span>
                                </button>
                                <br />
                            </div>
                        </div>
                    );
                }
                else {
                    // buttons for multiple choice question
                    return (
                        <div>
                            <div className="fg-white padding5">{this.props.action.Challenge.Question}</div>
                            {this.props.action.Challenge.Answers.map(function (item, index) {
                                return (
                                    <button type="button" onClick={function () { this.handleMultipleChoiceClick(item.Answer); }.bind(this)} className={buttonClass + " image-button"} style={{width: "100%"}} key={item.Id}>{item.Answer}<span className={"icon " + this.props.action.Icon + " bg-" + this.props.action.Color}/></button>
                                );
                            }, this)}
                        </div>
                    );
                }
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
    performAction: function (event) {
        if (event) {
            event.preventDefault();
        }

        // validate input if it is required
        var input = this.state.input;
        if (this.props.action.RequiresInput) {
            if (this.props.action.Name === "SelectPiece") {
                var pieceId = this.refs.iconSelector.getSelectedId();
                var colorId = this.refs.colorSelector.getSelectedId();
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

        $.post("/PerformAction",
            {
                Id: this.props.action.Id,
                Password: this.props.password,
                Input: input
            })
            .done(function (data) {
                // callback handles returned data
                this.props.onUpdate(this.props.action, this.state.input, data);
            }.bind(this))
            .fail(function () {
                alert("Invalid attempt to perform action");
            });
    },
    handleMultipleChoiceClick: function (answer) {
        this.state.input = answer;
        this.performAction();
    }
});