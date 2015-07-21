/** @jsx React.DOM */
/// <reference path="https://cdnjs.cloudflare.com/ajax/libs/react/0.13.3/react.js" />
/// <reference path="https://cdnjs.cloudflare.com/ajax/libs/react/0.13.3/JSXTransformer.js" />

var EditBoard = React.createClass({
    render: function () {
        if (this.state.board === null) {
            return (
                <div className="padding20" data-role="preloader" data-type="metro" data-style="dark"></div>
            );
        }
        else {
            return (
                <div style={{display: "flex", flexWrap: "wrap"}}>
                    <div style={{width: 780, height: 780}}>
                        {this.dummySpaces()}
                        {this.state.board.Spaces.map(function(item, index){
                            return <Board.Space space={item} key={index} />;
                        }, this)}
                    </div>
                    <div className="padding5" style={{flexGrow: 1}}>
                        <div className="accordion" data-role="accordion">
                            <div className="frame">
                                <div className="heading">Board<span className="icon mif-widgets"></span></div>
                                <div className="content">
                                    <form onSubmit={this.save}>
                                        <ImageSelector ref="imageSelector" image={(this.state.board.Image ? this.state.board.Image.Filepath : null)} />
                                        <br />
                                        <label htmlFor="boardName">Name:</label>
                                        <br />
                                        <div className="input-control text">
                                            <input id="boardName" type="text" value={this.state.name} onChange={this.handleNameChange} />
                                        </div>
                                        <br />
                                        <label htmlFor="boardDescription">Description:</label>
                                        <br />
                                        <div className="input-control textarea">
                                            <textarea id="boardDescription" value={this.state.description} onChange={this.handleDescriptionChange} />
                                        </div>
                                        <br />
                                        <LoadingButton text="Save" ref="saveButton" />
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
                        <div className="padding10"></div>
                        <div id="editPane"></div>
                    </div>
                </div>
            );
        }
    },
    getInitialState: function () {
        return {board: null, validation: null, name: null, description: null};
    },
    componentDidMount: function () {
        $.get("Boards/" + this.props.id, function (data) {
            var board = JSON.parse(data);
            this.setState({board: board, name: board.Name, description: board.Description});
            this.validate();
        }.bind(this));
    },
    dummySpaces: function () {
        var dummySpaces = [];
        for (i = 1; i <= 10; i++)
        {
            for (j = 1; j <= 10; j++)
            {
                dummySpaces.push(<EditBoard.DummySpace x={i} y={j} key={i + "," + j} />);
            }
        }
        return <div>{dummySpaces}</div>;
    },
    validate: function () {
        $.get("Boards/" + this.props.id + "/Validation", function (data) {
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
    handleNameChange: function (event) {
        this.setState({name: event.target.value});
    },
    handleDescriptionChange: function (event) {
        this.setState({description: event.target.value});
    },
    save: function (event) {
        event.preventDefault();
        this.refs.saveButton.showLoading();
        var formData = new FormData();
        formData.append("Id", this.props.id);
        formData.append("Name", this.state.name);
        formData.append("Description", this.state.description);
        var file = this.refs.imageSelector.file();
        if (file) {
            formData.append("Image", file, this.refs.imageSelector.filename());
        }
        $.ajax({
            url: "/Boards/" + this.props.id + "/Edit",
            type: 'POST',
            data: formData,
            contentType: false,
            processData: false
        })
        .done(function () {
            this.refs.saveButton.hideLoading();
        }.bind(this));
    }
});

EditBoard.Space = React.createClass({
    render: function () {
        // convert coordinates to pixel offsets
        var left = AdjustCoordinate(this.props.space.X, 70);
        var top = AdjustCoordinate(this.props.space.Y, 70);
        return (
            <Space name={this.props.space.Name} image={this.props.space.Image} icon={this.props.space.Icon} style={{position: "absolute", left: left, top: top}} />
        );
    }
});

EditBoard.DummySpace = React.createClass({
    render: function () {
        // convert coordinates to pixel offsets
        var left = AdjustCoordinate(this.props.x, 70);
        var top = AdjustCoordinate(this.props.y, 70);
        return (
            <div className="tile-small bg-white fg-black bd-black" style={{position: "absolute", left: left, top: top, borderWidth: 1, borderStyle: "solid"}}>
                <div className="padding5">Add...</div>
            </div>
        );
    }
});