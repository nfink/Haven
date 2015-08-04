/** @jsx React.DOM */
/// <reference path="https://cdnjs.cloudflare.com/ajax/libs/react/0.13.3/react.js" />
/// <reference path="https://cdnjs.cloudflare.com/ajax/libs/react/0.13.3/JSXTransformer.js" />

var AdminProfile = React.createClass({
    render: function () {
        if (this.state.user === null) {
            return (
                <div className="padding20" data-role="preloader" data-type="metro" data-style="dark"></div>
            );
        }
        else {
            return (
                <div className="container page-content">
                    <div className="bg-white padding10">
                        <form onSubmit={this.save}>
                            <label htmlFor="usernameEdit">Username:</label>
                            <div className="input-control text" style={{marginLeft: 5}}>
				                <input id="usernameEdit" type="text" value={this.state.user.Username} placeholder="Enter username..." onChange={this.handleUsernameChange} />
			                </div>
                            <br />
                            <LoadingButton text="Save" ref="saveButton" />
                        </form>
                    </div>
                </div>
            );
        }
    },
    getInitialState: function () {
        return {user: null};
    },
    componentDidMount: function () {
        $.get("/User", function (data) {
            var user = JSON.parse(data);
            this.setState({user: user});
        }.bind(this));
    },
    handleUsernameChange: function (event) {
        this.state.user.Username = event.target.value;
        this.setState({user: this.state.user});
    },
    save: function () {
        this.refs.saveButton.showLoading();
        $.ajax({
            url: "/User",
            type: "PUT",
            data: {Username: this.state.user.Username}
        })
        .done(function (data) {
            this.refs.saveButton.hideLoading();
            this.setState({ user: JSON.parse(data) });
        }.bind(this));
    }
});