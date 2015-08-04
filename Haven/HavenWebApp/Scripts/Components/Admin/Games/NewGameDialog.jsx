/** @jsx React.DOM */
/// <reference path="https://cdnjs.cloudflare.com/ajax/libs/react/0.13.3/react.js" />
/// <reference path="https://cdnjs.cloudflare.com/ajax/libs/react/0.13.3/JSXTransformer.js" />

var NewGameDialog = React.createClass({
    render: function () {
        return (
            <div className="dialog" data-overlay="true" data-overlay-color="op-dark" data-close-button="true" data-role="dialog">
                <div style={{backgroundImage: "url('" + this.props.image + "')", width: "100%", height: "100%", position: "absolute", backgroundSize: "cover", backgroundRepeat: "no-repeat", opacity: 0.25}}></div>
                <div className="padding20">
                    <h2>{this.props.title}</h2>
                    <p>{this.props.description}</p>
                    <form onSubmit={this.newGame}>
                        <input name="BoardId" value={this.props.boardId} type="hidden" />
                        <input name="NumberOfPlayers" value={this.state.numberOfPlayers} type="hidden" />
                        <div>
                            <label>Game name:</label>
                            <div className="padding5 no-padding-left">
                                <input name="Name" type="text" value={this.state.name} className={"input-control text requiredField" + (this.state.error ? " error" : "")} placeholder={this.state.error ? "Name is required" : ""} onChange={this.handleNameChange} />
                            </div>
                        </div>
                        <div>
                            <label>Number of players:</label>
                            <div className="padding5 no-padding-left" data-role="group" data-group-type="one-state">
                                {this.state.numberOfPlayerOptions.map(function(item, index){
                                    return <button className={item == this.state.numberOfPlayers ? "button active" : "button"} type="button" onClick={this.selectPlayerCount} key={index}>{item}</button>;
                                }, this)}
                            </div>
                        </div>
                        <div>
                            <button className="button" type="submit">Start Game</button>
                            <button className="button" type="button" onClick={this.close}>Cancel</button>
                        </div>
                    </form>
                </div>
            </div>
        );
    },
    getInitialState: function () {
        return {numberOfPlayers: 2, numberOfPlayerOptions: [2, 3, 4, 5, 6], name: "", error: false};
    },
    open: function () {
        OpenDialog(this);
    },
    close: function () {
        CloseDialog(this);
        this.setState(this.getInitialState());
        React.unmountComponentAtNode(document.getElementById("newGameDialog"));
    },
    selectPlayerCount: function (event) {
        this.setState({numberOfPlayers: $(event.target).text()});
    },
    handleNameChange: function(event) {
        this.setState({name: event.target.value});
    },
    newGame: function (event) {
        event.preventDefault();
        if (this.state.name.length === 0 || !this.state.name.trim()) {
            this.setState({error: true});
        }
        else {
            this.close();
            $.post("/Games",
                {
                    BoardId: this.props.boardId,
                    NumberOfPlayers: this.state.numberOfPlayers,
                    Name: this.state.name
                },
                function (data) {
                    page("/Games/" + JSON.parse(data).Id);
                });
        }
    },
});