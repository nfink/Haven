/** @jsx React.DOM */
/// <reference path="https://cdnjs.cloudflare.com/ajax/libs/react/0.13.3/react.js" />
/// <reference path="https://cdnjs.cloudflare.com/ajax/libs/react/0.13.3/JSXTransformer.js" />

var PlayerPanel = React.createClass({
    render: function () {
        var name = this.props.player.Name ? this.props.player.Name : "New Player";
        var icon = this.props.player.Piece ? this.props.player.Piece.Image : "mif-user";
        var iconColor = this.props.player.Color ? this.props.player.Color.Name : "white";

        return (
            <div className={"tile padding5" + (this.state.passwordEntered ? " bg-olive" : " ribbed-olive") + (this.props.selected ? " element-selected" : "")} onClick={this.selectPlayer} style={{width: "auto", height: "auto"}}>
                <form onSubmit={this.enterPassword} method="post" action="Authenticate">
                    <div className={"playerName password iconic" + (this.state.error ? " error" : "")} data-role="input">
                        <input type="password" ref="passwordField" value={this.state.passwordEntered ? "" : this.state.password} onChange={this.handlePasswordChange} style={{display: (this.state.passwordEntered ? "none" : "block" )}} />
                        <span className="playerNameLabel">{name}</span>
                        <span className="playerNameInformer">{this.state.informer}</span>
                        <span className="playerNamePlaceholder">{name}</span>
                        <span className={"icon " + icon + " fg-" + iconColor}></span>
                        <button className={"playerNameButton " + GetButtonClass()} type="submit" style={{display: (this.state.passwordEntered ? "none" : "block" )}}>
                            <span className="mif-play fg-white"></span>
                        </button>
                    </div>
                </form>
                {this.props.player.Actions.map(function (item, index) {
                    return <Action action={item} password={this.state.passwordEntered ? this.state.password : null} onUpdate={this.props.onUpdate} key={item.Id} />;
                }, this)}
            </div>
        );
    },
    getInitialState: function () {
        return {
            password: "",
            passwordEntered: (this.props.player.PasswordSet ? false : true),
            error: false,
            informer: "Please enter your password",
        };
    },
    handlePasswordChange: function (event) {
        this.setState({password: event.target.value});
    },
    enterPassword: function (event) {
        event.preventDefault();
        // verify password
        $.post("Authenticate",
            {
                PlayerId: this.props.player.Id,
                Password: this.state.password,
            })
            .done(function (data) {
                this.setState({passwordEntered: true});
                React.findDOMNode(this.refs.passwordField).blur();
            }.bind(this))
            .fail(function () {
                this.setState({error: true, informer: "Incorrect password", password: ""});
                React.findDOMNode(this.refs.passwordField).focus();
            }.bind(this));
    },
    selectPlayer: function () {
        this.props.onClick(this);
    }
});