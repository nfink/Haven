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
                            return <EditBoard.Space space={item} key={index} />;
                        }, this)}
                    </div>
                    <div className="padding5" style={{flexGrow: 1}}>
                        <div className="accordion" data-role="accordion">
                            <div className="frame">
                                <div className="heading">Board<span className="icon mif-widgets"></span></div>
                                <div className="content">
                                    <form onSubmit={this.saveBoard}>
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
                                        <LoadingButton text="Save" ref="saveBoardButton" />
                                        <LoadingButton text="Delete" className="button danger" type="button" onClick={this.handleDeleteBoard} ref="deleteBoardButton" style={{marginLeft: 5}}/>
                                    </form>
                                </div>
                            </div>
                            <div className="frame">
                                <div className="heading">Challenges<span className="icon mif-question"></span></div>
                                <div className="content">
                                    {(this.state.challenges !== null && this.state.challengeCategories !== null) ? this.challengesList() : <div data-role="preloader" data-type="metro" data-style="dark"></div>}
                                </div>
                            </div>
                            <div className="frame">
                                <div className="heading">Errors & Warnings<span className="icon mif-warning"></span></div>
                                <div className="content">{this.state.validation !== null ? this.validations() : null}</div>
                            </div>
                        </div>
                        <div className="padding10"></div>
                        <div id="editPane"></div>
                        <div ref="dialog"></div>
                    </div>
                </div>
            );
        }
    },
    getInitialState: function () {
        return {board: null, validation: null, name: null, description: null, challenges: null, challengeCategories: null, boardChallenges: null};
    },
    componentDidMount: function () {
        $.get("Boards/" + this.props.id, function (data) {
            var board = JSON.parse(data);
            var boardChallenges = {};
            board.Challenges.forEach(function(item, index) {
                boardChallenges[item.Id] = item;
            });
            this.setState({board: board, name: board.Name, description: board.Description, boardChallenges: boardChallenges});
            this.validate();
        }.bind(this));
        $.get("Challenges", function (data) {
            var challenges = JSON.parse(data);
            this.setState({challenges: challenges});
        }.bind(this));
        $.get("ChallengeCategories", function (data) {
            var categories = JSON.parse(data);
            this.setState({challengeCategories: categories});
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
    challengesList: function () {
        var uncategorized = this.questionsInCategory(0);
        return (
            <form onSubmit={this.saveChallenges}>
                <div className="treeview" data-role="treeview" ref="challengesTree">
                    <ul>
                        {uncategorized.length > 0 ?
                            <li className="node collapsed" data-mode="checkbox" key="0">
                                <span className="leaf">Uncategorized</span>
                                <span className="node-toggle"></span>
                                <ul>
                                    {uncategorized}
                                </ul>
                            </li>
                            :
                            null}
                        {this.state.challengeCategories.map(function(item, index){
                            return (
                                <li className="node collapsed" data-mode="checkbox" key={item.Id}>
                                    <span className="leaf">{item.Name}</span>
                                    <span className="node-toggle"></span>
                                    <ul>
                                        {this.questionsInCategory(item.Id)}
                                    </ul>
                                </li>
                                );
                        }, this)
                        }
                    </ul>
                </div>
                <LoadingButton text="Save" ref="saveChallengesButton" />
            </form>
        );
    },
    questionsInCategory (categoryId) {
	    return this.state.challenges.filter(function (value) {
		    return value.ChallengeCategoryId === categoryId;
	    })
	    .map(function (item, index) {
		    return (
			    <li data-mode="checkbox" data-name={"challenge" + item.Id} data-checked={item.Id in this.state.boardChallenges} key={item.Id}>
                    <span className="leaf">{item.Question}</span>
                </li>
		    );
	    }, this);
    },
    getSelectedChallenges: function () {
        var challengesTree = $(React.findDOMNode(this.refs.challengesTree));
        var challengeCheckboxes = challengesTree.find("[name^='challenge']");
        var selectedChallengeIds = [];
        $.each(challengeCheckboxes, function (index, value) {
            if ($(value).prop("checked")) {
                selectedChallengeIds.push($(value).attr("name").replace("challenge", ""));
            }
        });
        var challenges = {};
        this.state.challenges.forEach(function(item, index) {
            challenges[item.Id] = item;
        });
        var selectedChallenges = selectedChallengeIds.map(function (item, index) {
            return challenges[item];
        }, this);
        return selectedChallenges;
    },
    validate: function () {
        $.get("Boards/" + this.props.id + "/Validation", function (data) {
            this.setState({validation: JSON.parse(data)});
        }.bind(this));
    },
    validations: function () {
        var errors = "";
        var warnings = "";
        if (this.state.validation.Errors.length > 0) {
            errors = <ErrorPanel errors={this.state.validation.Errors} />;
        }
        if (this.state.validation.Warnings.length > 0) {
            warnings = <WarningPanel warnings={this.state.validation.Warnings} />;
        }
        return (
            <div>{errors}{warnings}</div>  
        );
    },
    handleNameChange: function (event) {
        this.setState({name: event.target.value});
    },
    handleDescriptionChange: function (event) {
        this.setState({description: event.target.value});
    },
    saveBoard: function (event) {
        event.preventDefault();
        this.refs.saveBoardButton.showLoading();
        var formData = new FormData();
        formData.append("Id", this.props.id);
        formData.append("Name", this.state.name);
        formData.append("Description", this.state.description);
        var file = this.refs.imageSelector.file();
        if (file) {
            formData.append("Image", file, this.refs.imageSelector.filename());
        }
        $.ajax({
            url: "/Boards/" + this.props.id,
            type: "PUT",
            data: formData,
            contentType: false,
            processData: false
        })
        .done(function () {
            this.refs.saveBoardButton.hideLoading();
            this.validate();
        }.bind(this));
    },
    saveChallenges: function (event) {
        event.preventDefault();
        this.refs.saveChallengesButton.showLoading();
        var challenges = {};
        $.post("/Boards/" + this.props.id + "/Challenges",
            {
                Challenges: JSON.stringify(this.getSelectedChallenges())
            },
            function (data) {
                this.refs.saveChallengesButton.hideLoading();
                this.validate();
            }.bind(this));
    },
    handleDeleteBoard: function () {
        var dialog = React.render(<DeleteDialog action={this.deleteBoard} />, React.findDOMNode(this.refs.dialog));
        dialog.open();
    },
    deleteBoard: function () {
        React.unmountComponentAtNode(React.findDOMNode(this.refs.dialog));
        this.refs.deleteBoardButton.showLoading();
        $.ajax({
            url: "/Boards/" + this.props.id,
            method: "DELETE",
        })
        .done(function () {
            this.refs.saveBoardButton.hideLoading();
            page("/Boards");
        }.bind(this));
    }
});

EditBoard.Space = React.createClass({
    render: function () {
        // convert coordinates to pixel offsets
        var left = AdjustCoordinate(this.props.space.X, 70);
        var top = AdjustCoordinate(this.props.space.Y, 70);
        return (
            <Space onClick={function() {alert("not implemented")}} name={this.props.space.Name} image={this.props.space.Image} icon={this.props.space.Icon} style={{position: "absolute", left: left, top: top}} />
        );
    }
});

EditBoard.DummySpace = React.createClass({
    render: function () {
        // convert coordinates to pixel offsets
        var left = AdjustCoordinate(this.props.x, 70);
        var top = AdjustCoordinate(this.props.y, 70);
        return (
            <div onClick={function() {alert("not implemented")}} className="tile-small bg-white fg-black bd-black" style={{position: "absolute", left: left, top: top, borderWidth: 1, borderStyle: "solid"}}>
                <div className="padding5">Add...</div>
            </div>
        );
    }
});