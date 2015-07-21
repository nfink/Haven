/** @jsx React.DOM */
/// <reference path="https://cdnjs.cloudflare.com/ajax/libs/react/0.13.3/react.js" />
/// <reference path="https://cdnjs.cloudflare.com/ajax/libs/react/0.13.3/JSXTransformer.js" />

var EditBoardDetails = React.createClass({
    render: function () {
        return (
            <div className="accordion" data-role="accordion">
                <div className="frame">
                    <div className="heading">Board<span className="icon mif-widgets"></span></div>
                    <div className="content">
                        <form onSubmit={this.editBoardAttributes}>
                            <div className="grid">
                                <div className="row cells2">
                                    <div className="cell">
                                        <label htmlFor="boardName">Name:</label>
                                        <br />
                                        <div className="input-control text">
                                            <input type="text" name="Name" value="@Model.Name" />
                                        </div>
                                        <br />
                                        <label htmlFor="boardIcon">Icon:<span id="boardIconDisplay" style={{float: "right", width: 70, height: 70, backgroundImage: "url('@Model.Image.Filepath')", backgroundSize: "cover", backgroundRepeat: "no-repeat", borderRadius: 0}} /></label>
                                        <br />
                                        <div id="iconFileInput" style={{width: 350}}>
                                            <input id="boardIcon" name="Icon" value="@Model.Image.Filename" tabIndex="-1" style={{zIndex: 0, display: "none"}} type="file" onchange="$('#iconSelectOverlay').val($(this).val()); PreviewImage(this, $('#boardIconDisplay'));" />
                                            <div className="input-control text" data-role="input">
                                                <input id="iconSelectOverlay" style={{paddingRight: 62}} type="text" readOnly="" value="@Model.Image.Filename" onclick="$('#boardIcon').trigger('click');" />
                                            </div>
                                        </div>
                                    </div>
                                    <div className="cell">
                                        <label htmlFor="boardDescription">Description:</label>
                                        <br />
                                        <div className="input-control textarea">
                                            <textarea id="boardDescription" name="Description">@Model.Description</textarea>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <button className="button primary" type="submit">Save</button>
                        </form>
                    </div>
                </div>
                <div className="frame">
                    <div className="heading">Challenges<span className="icon mif-question"></span></div>
                    <div className="content">Frame content</div>
                </div>
                <div className="frame">
                    <div className="heading">Errors & Warnings<span className="icon mif-warning"></span></div>
                    <div className="content">{this.validations()}</div>
                </div>
            </div>
        );
    },
    getInitialState: function () {
        return {validation: null, name: "", description: "", image: ""};
    },
    validate: function () {
        $.get("Boards/" + this.props.board.Id + "/Validation", function (data) {
            this.setState({validation: JSON.parse(data)});
        }.bind(this));
    },
    validations: function () {
        if (this.state.validation) {
            var errors = "";
            var warnings = "";
            if (this.state.validation.Errors.length > 0) {
                var errors = <ErrorPanel errors={this.state.validation.Errors} />
            }
            if (this.state.validation.Warnings.length > 0) {
                warnings = <WarningPanel warnings={this.state.validation.Warnings} />
            }
            return (
                <div>{errors}{warnings}</div>  
            );
        }
        return "None";
    },
    editBoardAttributes: function () {

    }
});