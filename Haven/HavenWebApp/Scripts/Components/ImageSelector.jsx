/** @jsx React.DOM */
/// <reference path="https://cdnjs.cloudflare.com/ajax/libs/react/0.13.3/react.js" />
/// <reference path="https://cdnjs.cloudflare.com/ajax/libs/react/0.13.3/JSXTransformer.js" />

var ImageSelector = React.createClass({
    render: function () {
        return (
            <div className="tile-container">
                <input ref="imageInput" tabIndex="-1" style={{zIndex: 0, display: "none"}} type="file" onChange={this.handleImageChange} />
                <div className="tile bg-white fg-black bd-black" data-role="tile" onClick={this.selectImage} style={{borderWidth: 1, borderStyle: "solid"}}>
                    {this.state.image ? null : (<div className="padding5">{this.props.description ? this.props.description : "Select an image..."}</div>)}
                    <div className="tile-content">
                        <div style={{width: "100%"}} className="image-container image-format-square">
                            <div className="frame">
                                <div ref="imageDisplay" style={{width: "100%", height: "150px", backgroundImage: "url('" + (this.state.image ? this.state.image : "") + "')", backgroundSize: "cover", backgroundRepeat: "no-repeat", borderRadius: "0px"}}></div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        );
    },
    getInitialState: function () {
        return {image: this.props.image};
    },
    handleImageChange: function (event) {
        var input = event.target;
        var imageDisplay = React.findDOMNode(this.refs.imageDisplay);
        this.setState({image: input.value});
        if (input.files && input.files[0]) {
            var reader = new FileReader();

            reader.onload = function (e) {
                $(imageDisplay).css("background-image", "url(" + e.target.result + ")");
            }

            reader.readAsDataURL(input.files[0]);
        }

        if (typeof this.props.onSelect === "function") {
            this.props.onSelect(input.files && input.files[0]);
        }
    },
    selectImage: function (event) {
        React.findDOMNode(this.refs.imageInput).click();
    },
    file: function () {
        var input = React.findDOMNode(this.refs.imageInput);
        return (input.files && input.files[0]);
    }
});