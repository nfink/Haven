/** @jsx React.DOM */
/// <reference path="https://cdnjs.cloudflare.com/ajax/libs/react/0.13.3/react.js" />
/// <reference path="https://cdnjs.cloudflare.com/ajax/libs/react/0.13.3/JSXTransformer.js" />

var ComboBox = React.createClass({
    render: function () {
        return (
            <div style={this.props.style}>
                {this.props.label ? <label htmlFor={this.props.id ? this.props.id : "combobox"}>{this.props.label}</label> : null}
                <div className="input-control text" style={{marginLeft: 5}}>
                    <input type="text" id={this.props.id ? this.props.id : "combobox"} value={this.state.value} placeholder={this.props.placeholder} onChange={this.handleValueChange} onClick={this.handleFocus} onFocus={this.handleFocus} onBlur={this.handleBlur} />
                    <div style={{border: "1px solid #787878", position: "absolute", left: 0, top: 41, width: 209, backgroundColor: "white", zIndex: 9999, display: (this.state.listDisplayed ? "block" : "none")}}>
                        {this.props.options.map(function(item, index){
                            return (
				                <div className="comboBoxItem" style={{padding: 5}} onClickCapture={this.handleSelect} key={index}>{item}</div>
			                );
		                }, this)}
                    </div>
                </div>
            </div>
        );
    },
    getInitialState: function () {
        return {value: this.props.value ? this.props.value : "", listDisplayed: false};
    },
    handleValueChange: function (event) {
        this.setState({value: event.target.value, listDisplayed: false});
    },
    handleSelect: function (event) {
        this.setState({value: $(event.target).text()});
    },
    handleFocus: function () {
        this.setState({listDisplayed: true});
    },
    handleBlur: function () {
        setTimeout(function () {
            this.setState({listDisplayed: false});
        }.bind(this), 100);
    },
    value: function () {
        return this.state.value;
    }
});