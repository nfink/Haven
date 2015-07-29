/** @jsx React.DOM */
/// <reference path="https://cdnjs.cloudflare.com/ajax/libs/react/0.13.3/react.js" />
/// <reference path="https://cdnjs.cloudflare.com/ajax/libs/react/0.13.3/JSXTransformer.js" />

var ColorSelector = React.createClass({
    render: function () {
        return (
            <div style={this.props.style} title="Select a color">
                <div className={"colorSelect bg-" + (this.state.selectedColor ? this.state.selectedColor : "white") + (this.state.error ? " error" : "")} onClick={this.toggleColors}></div>
                <div className="colorPad keypad keypad-dropdown" style={{position: "absolute", zIndex: "1000", width: "242px", display: (this.state.colorsDisplayed ? "block" : "none")}}>
                    {this.state.colors.map(function (item, index) {
                        return <div className={"key bg-" + item.Name} id={item.Id} value={item.Name} onClick={this.selectColor} key={index} />;
                    }, this)}
                </div>
            </div>
        );
    },
    getInitialState: function () {
        return {selectedColor: (this.props.selectedColor ? this.props.selectedColor.Name : null), selectedColorId: (this.props.selectedColor ? this.props.selectedColor.Id : null), colorsDisplayed: false, colors: [], error: false};
    },
    toggleColors: function() {
        this.setState({colorsDisplayed: !this.state.colorsDisplayed});
    },
    selectColor: function (event) {
        this.setState({colorsDisplayed: false, selectedColor: event.target.getAttribute("value"), selectedColorId: event.target.id, error: false});
        if (typeof this.props.onSelect === "function") {
            this.props.onSelect({Id: event.target.id, Name: event.target.getAttribute("value")});
        }
    },
    getSelectedId: function () {
        return this.state.selectedColorId;
    },
    getSelectedName: function () {
        return this.state.selectedColor;
    },
    showError: function () {
        this.setState({error: true});
    },
    componentDidMount: function () {
        $.get("Colors", function (data) {
            this.setState({colors: JSON.parse(data)});
        }.bind(this));
    },
});