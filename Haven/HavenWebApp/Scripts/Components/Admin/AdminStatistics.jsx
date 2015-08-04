/** @jsx React.DOM */
/// <reference path="https://cdnjs.cloudflare.com/ajax/libs/react/0.13.3/react.js" />

var AdminStatistics = React.createClass({
    render: function () {
        if (this.state.games === null) {
            return (
                <div className="padding20" data-role="preloader" data-type="metro" data-style="dark"></div>
            );
        }
        else if (this.state.games.length < 1) {
            return (
                <div className="container page-content padding10 bg-white">
                    <div>No completed games</div>
                </div>
            );
        }
        else {
            return (
                <div className="container page-content padding10 bg-white">
                    {this.state.games.map(function (item, index) {
                        return (
                            <div key={item.Id}>
                                <div>{item.Name}</div>
                                <ul>
                                    {item.Winners.map(function (item, index) {
                                        return <li key={item.Id}>{item.Player + " won on turn " + item.Turn}</li>;
                                    })}
                                </ul>
                            </div>
                        );
                    })}
                </div>
            );
        }
    },
    getInitialState: function () {
        return { games: null };
    },
    componentDidMount: function () {
        $.get("/Games/Completed", function (data) {
            this.setState({ games: JSON.parse(data) });
        }.bind(this));
    },
});