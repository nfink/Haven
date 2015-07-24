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
                    <div>
                        <div className=""></div>
                        <div style={{width: 770, height: 775}}>
                            {this.dummySpaces()}
                            {this.state.board.Spaces.map(function(item, index){
                                return <EditBoard.Space space={item} updateCallback={this.loadBoard} key={item.Id} />;
                            }, this)}
                        </div>
                    </div>
                    <div className="padding5" style={{flexGrow: 1}}>
                        <div className="accordion bg-white padding5" data-role="accordion">
                            <div className="frame">
                                <div className="heading">Board<span className="icon mif-widgets"></span></div>
                                <div className="content">
                                    <form onSubmit={this.saveBoard}>
                                        <label>Image:</label>
                                        <br />
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
                        <div className="padding5"></div>
                        <div id="editPane" className="bg-white"></div>
                        <div ref="dialog" id="dialog"></div>
                    </div>
                </div>
            );
        }
    },
    getInitialState: function () {
        return {board: null, validation: null, name: null, description: null, challenges: null, challengeCategories: null, boardChallenges: null};
    },
    componentDidMount: function () {
        this.loadBoard();
        $.get("Challenges", function (data) {
            var challenges = JSON.parse(data);
            this.setState({challenges: challenges});
        }.bind(this));
        $.get("ChallengeCategories", function (data) {
            var categories = JSON.parse(data);
            this.setState({challengeCategories: categories});
        }.bind(this));
    },
    loadBoard: function () {
        $.get("Boards/" + this.props.id, function (data) {
            var board = JSON.parse(data);
            var boardChallenges = {};
            board.Challenges.forEach(function (item, index) {
                boardChallenges[item.Id] = item;
            });
            this.setState({ board: board, name: board.Name, description: board.Description, boardChallenges: boardChallenges });
            this.validate();
        }.bind(this));
    },
    dummySpaces: function () {
        var dummySpaces = [];
        for (i = 1; i <= 10; i++)
        {
            for (j = 1; j <= 10; j++)
            {
                dummySpaces.push(<EditBoard.DummySpace boardId={this.state.board.Id} x={i} y={j} updateCallback={this.loadBoard} key={i + "," + j} />);
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
            this.setState({ validation: JSON.parse(data) });
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
        formData.append("Name", this.state.name);
        formData.append("Description", this.state.description);
        var file = this.refs.imageSelector.file();
        if (file) {
            formData.append("Image", file, file.name);
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
            <Space onClick={this.editSpace} space={this.props.space} style={{position: "absolute", left: left, top: top}} />
        );
    },
    editSpace: function () {
        React.unmountComponentAtNode(document.getElementById("editPane"));
        React.render(<EditBoard.EditSpace space={this.props.space} updateCallback={this.props.updateCallback} />, document.getElementById("editPane"));
    }
});

EditBoard.DummySpace = React.createClass({
    render: function () {
        // convert coordinates to pixel offsets
        var left = AdjustCoordinate(this.props.x, 70);
        var top = AdjustCoordinate(this.props.y, 70);
        return (
            <div onClick={this.editSpace} className="tile-small bg-white fg-black bd-black" style={{position: "absolute", left: left, top: top, borderWidth: 1, borderStyle: "solid"}}>
                <div className="padding5">Add...</div>
            </div>
        );
    },
    editSpace: function () {
        React.unmountComponentAtNode(document.getElementById("editPane"));
        var space = { BoardId: this.props.boardId, Type: 0, Order: 0, Image: null, X: this.props.x, Y: this.props.y, BackgroundColorId: 0, TextColorId: 0 };
        React.render(<EditBoard.EditSpace space={space} updateCallback={this.props.updateCallback} />, document.getElementById("editPane"));
    }
});

EditBoard.EditSpace = React.createClass({
    render: function () {
        return (
            <form onSubmit={this.editSpace} className="padding5">
                <div className="header">Edit Space</div>
                <div>
                    <div>
                        <br />
                        <div style={{display: "flex"}}>
                            <div>
                                <Space space={this.state.space} style={{float: "none"}} />
                                <div style={{opacity: 0.0, position: "absolute", width: 70, height: 70, left: 5, top: 5, zIndex: 1001}}></div>
                            </div>
                            <div style={{marginTop: 5, marginLeft: 5, width: 300}}>{this.state.spaceTypes.length > 0 ? this.state.spaceTypes[this.state.space.Type].Description : ""}</div>
                        </div>
                        <div style={{display: "flex"}}>
                            <div>
                                <br />
                                <label htmlFor="spaceType">Type:</label>
                                <br />
			                    <div id="spaceType" className="input-control select">
				                    <select value={this.state.space.Type} onChange={this.handleTypeChange}>
				                        {this.state.spaceTypes.map(function(item, index){
				                            return <option value={item.Id} key={item.Id}>{item.Name}</option>;
				                        }, this)}
				                    </select>
			                    </div>
                                <br />
                                <label htmlFor="spaceOrder">Order:</label>
			                    <br />
			                    <div className="input-control text">
				                    <input id="spaceOrder" type="text" value={this.state.space.Order} placeholder="Enter number..." onChange={this.handleOrderChange} />
			                    </div>
                                <div style={{display: "flex", alignItems: "center"}}>
                                    <div style={{marginRight: 5}}>Background color:</div>
                                    <ColorSelector ref="backgroundColor" selectedColor={this.state.space.BackgroundColor} onSelect={this.handleBackgroundColorChange} />
                                </div>
                                <div style={{display: "flex", alignItems: "center"}}>
                                    <div style={{marginRight: 5}}>Text color:</div>
                                    <ColorSelector ref="textColor" selectedColor={this.state.space.TextColor} onSelect={this.handleTextColorChange} />
                                </div>
                            </div>
                            {this.additionalEditFields()}
                        </div>
                    </div>
                </div>
                <LoadingButton text="Save" ref="saveButton" />
                {this.state.space.Id ? <LoadingButton text="Delete" ref="deleteButton" type="button" onClick={this.handleDelete} style={{marginLeft: 5}} /> : null}
                <button className="button primary" type="button" onClick={this.close} style={{marginLeft: 5}}>Close</button>
            </form>
        );
    },
    getInitialState: function () {
        // create a copy of the space so we aren't modifying the parent
        var space = JSON.parse(JSON.stringify(this.props.space));
        return {space: space, spaceTypes: []};
    },
    componentDidMount: function () {
        $.get("SpaceTypes", function (data) {
            this.setState({spaceTypes: JSON.parse(data)});
        }.bind(this));
    },
    additionalEditFields: function () {
        if (!this.state.space || (this.state.spaceTypes.length < 1)) {
            return null;
        }

        var spaceType = this.state.spaceTypes[this.state.space.Type].Name;
        this.initNameCard();
        this.initSafeHavenCard();

        return (
            <div style={{marginLeft: 20}}>
                <div style={{display: (spaceType === "Challenge" ? "block" : "none")}}>
                    <br />
                    <label>Card image:</label>
                    <br />
                    <ImageSelector ref="spaceNameCardImageSelector" image={this.state.space.NameCard.Image ? this.state.space.NameCard.Image.Filepath : null} onSelect={this.handleNameCardImageChange} />
                    <label htmlFor="spaceNameCardName">Card name:</label>
			        <br />
			        <div className="input-control text">
				        <input id="spaceNameCardName" type="text" value={this.state.space.NameCard.Name} placeholder="Enter card name..." onChange={this.handleNameCardNameChange} />
			        </div>
                    <br />
                    <label htmlFor="spaceNameCardDescription">Card description:</label>
			        <br />
			        <div className="input-control textarea">
				        <textarea id="spaceNameCardDescription" value={this.state.space.NameCard.Details} placeholder="Enter card description..." onChange={this.handleNameCardDescriptionChange} />
			        </div>
                </div>
                <div style={{display: (spaceType === "Safe Haven" ? "block" : "none")}}>
                    <br />
                    <label>Card image:</label>
                    <br />
                    <ImageSelector ref="spaceSafeHavenCardImageSelector" image={this.state.space.SafeHavenCard.Image ? this.state.space.SafeHavenCard.Image.Filepath : null} onSelect={this.handleSafeHavenCardImageChange} />
                    <label htmlFor="spaceSafeHavenCardName">Card name:</label>
			        <br />
			        <div className="input-control text">
				        <input id="spaceSafeHavenCardName" type="text" value={this.state.space.SafeHavenCard.Name} placeholder="Enter card name..." onChange={this.handleSafeHavenCardNameChange} />
			        </div>
                    <br />
                    <label htmlFor="spaceSafeHavenCardDescription">Card description:</label>
			        <br />
			        <div className="input-control textarea">
				        <textarea id="spaceSafeHavenCardDescription" value={this.state.space.SafeHavenCard.Details} placeholder="Enter card description..." onChange={this.handleSafeHavenCardDescriptionChange} />
			        </div>
                </div>
            </div>
        );
    },
    editSpace: function (event) {
        event.preventDefault();
        this.refs.saveButton.showLoading();
        var formData = new FormData();
        formData.append("BoardId", this.state.space.BoardId);
        formData.append("Type", this.state.space.Type);
        formData.append("Order", this.state.space.Order);
        formData.append("BackgroundColorId", this.state.space.BackgroundColorId);
        formData.append("TextColorId", this.state.space.TextColorId);
        formData.append("X", this.state.space.X);
        formData.append("Y", this.state.space.Y);
        var spaceType = this.state.spaceTypes[this.state.space.Type].Name;
        if (spaceType === "Challenge") {
            formData.append("CardName", this.state.space.NameCard.Name);
            formData.append("CardDetails", this.state.space.NameCard.Details);
            var file = this.refs.spaceNameCardImageSelector.file();
            if (file) {
                formData.append("Image", file, file.name);
            }
        }
        if (spaceType === "Safe Haven") {
            formData.append("CardName", this.state.space.SafeHavenCard.Name);
            formData.append("CardDetails", this.state.space.SafeHavenCard.Details);
            var file = this.refs.spaceSafeHavenCardImageSelector.file();
            if (file) {
                formData.append("Image", file, file.name);
            }
        }

        if (this.state.space.Id) {
            $.ajax({
                url: "/Spaces/" + this.state.space.Id,
                type: "PUT",
                data: formData,
                contentType: false,
                processData: false
            })
            .done(function (data) {
                this.refs.saveButton.hideLoading();
                this.setState({ space: JSON.parse(data) });
                this.props.updateCallback();
            }.bind(this));
        }
        else {
            $.ajax({
                url: "/Spaces",
                type: "POST",
                data: formData,
                contentType: false,
                processData: false
            })
            .done(function (data) {
                this.refs.saveButton.hideLoading();
                this.setState({ space: JSON.parse(data) });
                this.props.updateCallback();
            }.bind(this));
        }
    },
    close: function () {
        React.unmountComponentAtNode(document.getElementById("editPane"));
    },
    handleDelete: function () {
        var dialog = React.render(<DeleteDialog action={this.deleteSpace} />, document.getElementById("dialog"));
        dialog.open();
    },
    deleteSpace: function () {
        React.unmountComponentAtNode(document.getElementById("dialog"));
        this.refs.deleteButton.showLoading();
        $.ajax({
            url: "/Spaces/" + this.state.space.Id,
            method: "DELETE",
            success: function (data) {
                this.refs.deleteButton.hideLoading();
                React.unmountComponentAtNode(document.getElementById("editPane"));
                this.props.updateCallback();
            }.bind(this)
        })
        .fail(function (data) {
            alert("failed to delete");
            this.refs.deleteButton.hideLoading();
        });
    },
    handleTypeChange: function (event) {
        this.state.space.Type = event.target.value;
        var spaceType = this.state.spaceTypes[this.state.space.Type]
        this.state.space.Icon = spaceType.Icon;
        if (spaceType.Name === "Challenge") {
            this.state.space.Name = this.state.space.NameCard.Name;
            this.state.space.Image = (this.state.space.NameCard.Image ? this.state.space.NameCard.Image.Filepath : null);
        }
        else if (spaceType.Name === "Safe Haven") {
            this.state.space.Name = this.state.space.SafeHavenCard.Name;
            this.state.space.Image = (this.state.space.SafeHavenCard.Image ? this.state.space.SafeHavenCard.Image.Filepath : null);
        }
        else {
            this.state.space.Image = null;
        }
        this.setState({space: this.state.space});
    },
    handleOrderChange: function (event) {
        this.state.space.Order = event.target.value;
        this.setState({space: this.state.space});
    },
    handleBackgroundColorChange: function(selectedColor) {
        this.state.space.BackgroundColorId = selectedColor.Id;
        this.state.space.BackgroundColor = selectedColor;
        this.setState({space: this.state.space});
    },
    handleTextColorChange: function(selectedColor) {
        this.state.space.TextColorId = selectedColor.Id;
        this.state.space.TextColor = selectedColor;
        this.setState({space: this.state.space});
    },
    initNameCard: function () {
        if (!this.state.space.NameCard) {
            this.state.space.NameCard = { Name: null, Details: null };
        }
    },
    initSafeHavenCard: function () {
        if (!this.state.space.SafeHavenCard) {
            this.state.space.SafeHavenCard = { Name: null, Details: null };
        }
    },
    handleNameCardNameChange: function(event) {
        this.state.space.NameCard.Name = event.target.value;
        this.state.space.Name = event.target.value;
        this.setState({ space: this.state.space });
    },
    handleSafeHavenCardNameChange: function (event) {
        this.state.space.SafeHavenCard.Name = event.target.value;
        this.state.space.Name = event.target.value;
        this.setState({ space: this.state.space });
    },
    handleNameCardDescriptionChange: function (event) {
        this.state.space.NameCard.Details = event.target.value;
        this.setState({ space: this.state.space });
    },
    handleSafeHavenCardDescriptionChange: function (event) {
        this.state.space.SafeHavenCard.Details = event.target.value;
        this.setState({ space: this.state.space });
    },
    handleNameCardImageChange: function (file) {
        var reader = new FileReader();
        reader.onload = function (e) {
            this.state.space.NameCard.Image = {};
            this.state.space.NameCard.Image.Filename = file.name;
            this.state.space.NameCard.Image.Filepath = e.target.result;
            this.state.space.Image = e.target.result;
            this.setState({ space: this.state.space });
        }.bind(this);
        reader.readAsDataURL(file);
    },
    handleSafeHavenCardImageChange: function (file) {
        var reader = new FileReader();
        reader.onload = function (e) {
            this.state.space.SafeHavenCard.Image = {};
            this.state.space.SafeHavenCard.Image.Filename = file.name;
            this.state.space.SafeHavenCard.Image.Filepath = e.target.result;
            this.state.space.Image = e.target.result;
            this.setState({ space: this.state.space });
        }.bind(this);
        reader.readAsDataURL(file);
    },
});