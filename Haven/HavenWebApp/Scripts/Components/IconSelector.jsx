/** @jsx React.DOM */
/// <reference path="https://cdnjs.cloudflare.com/ajax/libs/react/0.13.3/react.js" />
/// <reference path="https://cdnjs.cloudflare.com/ajax/libs/react/0.13.3/JSXTransformer.js" />

var IconSelector = React.createClass({
    render: function () {
        return (
            <div style={this.props.style} title="Select an icon">
                <div className={"imageSelect mif-2x " + (this.state.selectedIcon ? this.state.selectedIcon : "mif-question") + (this.state.error ? " error" : "")} onClick={this.toggleIcons}></div>
                <div className="imagePad keypad keypad-dropdown" style={{position: "absolute", zIndex: "1000", width: "242px", display: (this.state.iconsDisplayed ? "block" : "none")}}>
                    {this.state.pieces.map(function (item, index) {
                        return <div className={"key " + item.Image} id={item.Id} value={item.Image} onClick={this.selectIcon} key={index} />;
                    }, this)}
                </div>
            </div>
        );
    },
    getInitialState: function () {
        return {selectedIcon: (this.props.selectedIcon ? this.props.selectedIcon.Image : null), selectedIconId: (this.props.selectedIcon ? this.props.selectedIcon.Id :  null), iconsDisplayed: false, pieces: [], error: false};
    },
    toggleIcons: function() {
        this.setState({iconsDisplayed: !this.state.iconsDisplayed});
    },
    selectIcon: function (event) {
        this.setState({iconsDisplayed: false, selectedIcon: event.target.getAttribute("value"), selectedIconId: event.target.id, error: false});
        if (typeof this.props.onSelect === "function") {
            this.props.onSelect({Id: event.target.id, Image: event.target.getAttribute("value")});
        }
    },
    getSelectedId: function () {
        return this.state.selectedIconId;
    },
    getSelectedName: function () {
        return this.state.selectedIcon;
    },
    showError: function () {
        this.setState({error: true});
    },
    componentDidMount: function () {
        $.get("Pieces", function (data) {
            this.setState({pieces: JSON.parse(data)});
        }.bind(this));
    },
});