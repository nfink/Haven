/** @jsx React.DOM */
/// <reference path="https://cdnjs.cloudflare.com/ajax/libs/react/0.13.3/react.js" />
/// <reference path="https://cdnjs.cloudflare.com/ajax/libs/react/0.13.3/JSXTransformer.js" />

var PlayerPanel = React.createClass({
    render: function () {
        var name = this.props.player.Name ? this.props.player.Name : "New Player";
        var icon = this.props.player.Piece ? this.props.player.Piece.Image : "mif-user";
        var iconColor = this.props.player.Color ? this.props.player.Color.Name : "white";

        return (
            <div className={"padding5 margin5" + (this.state.passwordEntered ? " bg-olive" : " ribbed-olive") + (this.props.selected ? " element-selected" : "")} onClick={this.selectPlayer} style={{width: 280, height: "100%", boxShadow: "0px 0px 1px #FFC inset"}}>
                <form onSubmit={this.enterPassword}>
                    <div className={"playerName password iconic" + (this.state.error ? " error" : "")} data-role="input">
                        <input type="password" ref="passwordField" value={this.state.passwordEntered ? "" : this.state.password} onChange={this.handlePasswordChange} style={{display: (this.state.passwordEntered ? "none" : "block" )}} />
                        <span className="playerNameLabel">{name}</span>
                        <span className="playerNameInformer">{this.state.informer}</span>
                        <span className="playerNamePlaceholder" style={{zIndex: 0}}>{name}</span>
                        <span className={"icon " + icon + " fg-" + iconColor}></span>
                        <button className={"playerNameButton " + GetButtonClass()} type="submit" style={{display: (this.state.passwordEntered ? "none" : "block" )}}>
                            <span className="mif-play fg-white"></span>
                        </button>
                    </div>
                </form>
                <div style={{display: "flex", marginLeft: 5, marginRight: 5, marginBottom: 5}}>
                    <div className="fg-white bg-transparent text-small" style={{height: "4.5em", overflow: "hidden", flexGrow: 1}}>
                        {this.props.player.Messages.map(function(item, index){
                            return (<div style={{paddingLeft: 5}} key={item.Id}>{item.Text}<br /></div>);
                        }, this)}
                    </div>
                    <div>
                        <PlayerPanel.CardsViewer player={this.props.player} style={{width: 52, height: 52, margin: 0}} />
                    </div>
                </div>
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

    }
});

PlayerPanel.CardsViewer = React.createClass({
    render: function () {
        var cardCount = this.props.player.SafeHavenCards.length + this.props.player.NameCards.length;
        var width = (cardCount * 60 > 180 ? 180 : cardCount * 60);
        return (
            <div style={{display: (cardCount > 0 ? "block" : "none")}}>
                <div className="tile-small bg-taupe fg-white" style={this.props.style} title="View cards" onClick={this.toggleCards}>
                    <div className="tile-content iconic">
                        <span className="icon mif-stack"></span>
                    </div>
                </div>
                <div className="bg-white" style={{position: "absolute", width: width, left: -width, border: "1px solid #EEE", display: ((this.state.cardsDisplayed && cardCount > 0) ? "block" : "none")}}>
                    {this.props.player.SafeHavenCards.map(function (item, index) {
                        return this.renderCard(item, item.Id);
                    }, this)}
                    {this.props.player.NameCards.map(function (item, index) {
                        return this.renderCard(item, item.Id);
                    }, this)}
                </div>
            </div>
        );
    },
    getInitialState: function () {
        return {cardsDisplayed: false};
    },
    toggleCards: function () {
        if ((this.props.player.NameCards.length > 0) || (this.props.player.SafeHavenCards.length > 0)) {
            this.setState({ cardsDisplayed: !this.state.cardsDisplayed });
        }
    },
    renderCard: function (card, key) {
        return (
            <div className="tile-small bg-taupe fg-white" style={{width: 52, height: 52, margin: "1px 5px 1px 1px"}} key={key} data-role="popover" data-popover-mode="click" data-popover-position="bottom" data-popover-text={card.Details} data-popover-background="bg-cyan" data-popover-color="fg-white">
                <div className="tile-content" style={{textAlign: "center"}}>
                    {card.Image !== null ? <div style={{width: "100%", height: "100%", position: "absolute", backgroundImage: "url(" + card.Image.Filepath + ")", backgroundSize: "cover", backgroundRepeat: "no-repeat"}}></div> : null}
                    <div style={{display: "inline-block", verticalAlign: "middle", lineHeight: "52px"}}>{card.Name}</div>
                </div>
            </div>
        );
    }
});