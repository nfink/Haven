/** @jsx React.DOM */
/// <reference path="https://cdnjs.cloudflare.com/ajax/libs/react/0.13.3/react.js" />

var EditBoard = React.createClass({
    render: function () {
        if (this.state.board === null) {
            return (
                <div className="padding20" data-role="preloader" data-type="metro" data-style="dark"></div>
            );
        }
        else {
            var nameCardSpaces = this.state.board.Spaces.filter(function (item) {
                    return item.NameCard !== null;
                });
            return (
                <div style={{display: "flex", flexWrap: "wrap"}}>
                    <div>
                        <div className=""></div>
                        <div style={{width: 770, height: 775}}>
                            {this.dummySpaces()}
                            {this.state.board.Spaces.map(function(item, index){
                                return <EditBoard.Space space={item} categories={this.state.challengeCategories} updateCallback={this.loadBoard} key={item.Id} />;
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
                                        <div>The game ends when...</div>
                                        <div style={{display: ((!this.state.endWithCards && !this.state.endWithTurns) ? "block" : "none"), marginLeft: "1em", marginTop: 5}}>a player has collected all cards.</div>
                                        <div style={{display: "flex", marginLeft: "1em"}}>
                                            <label className="input-control checkbox" style={{marginRight: 5}}>
                                                <input checked={this.state.endWithTurns} onChange={this.handleEndWithTurnsChange} type="checkbox" />
                                                <span className="check"></span>
                                            </label>
                                            <div className={this.state.endWithTurns ? "" : "fg-grayLight"}><div className="input-control text" style={{width: "3.0rem"}}><input value={this.state.turnsToEnd} disabled={!this.state.endWithTurns} onChange={this.handleTurnsToEndChange} /></div> turns have passed</div>
                                        </div>
                                        <div style={{display: "flex", marginLeft: "1em"}}>
                                            <label className="input-control checkbox">
                                                <input checked={this.state.endWithCards} onChange={this.handleEndWithCardsChange} type="checkbox" />
                                                <span className="check"></span>
                                                <span className={this.state.endWithCards ? "" : "fg-grayLight"} style={{marginLeft: 5}}>{this.state.endWithTurns ? "or " : ""}a player has collected:</span>
                                            </label>
                                        </div>
                                        <div style={{marginLeft: "3em"}}>
                                            <div className="input-control select">
				                                <select value={this.state.nameCardsToEnd} disabled={!this.state.endWithCards} onChange={this.handleNameCardsToEndChange}>
                                                    <option value="-1">All challenge cards</option>
                                                    <option value="0">0 challenge cards</option>
				                                    {nameCardSpaces.map(function (item, index) {
				                                            return <option value={index + 1} key={index}>{index + 1 + " challenge cards"}</option>;
				                                        }, this)
                                                    }
				                                </select>
			                                </div>
                                            <div style={{display: ((this.state.nameCardsToEnd < 0 || this.state.nameCardsToEnd >= nameCardSpaces.length) ? "block" : "none")}}>
                                                <div className={this.state.endWithCards ? "" : "fg-grayLight"}>and</div>
                                                <div className="input-control select">
				                                    <select value={this.state.safeHavenCardsToEnd} disabled={!this.state.endWithCards} onChange={this.handleSafeHavenCardsToEndChange}>
                                                        <option value="-1">All safe haven cards</option>
                                                        <option value="0">0 haven cards</option>
				                                        {this.state.board.Spaces.filter(function (item) {
                                                                return item.SafeHavenCard !== null;
                                                            }).map(function (item, index) {
				                                                return <option value={index + 1} key={index}>{index + 1 + " safe haven cards"}</option>;
				                                            }, this)
                                                        }
				                                    </select>
			                                    </div>
                                            </div>
                                        </div>
                                        <br />
                                        <LoadingButton text="Save" ref="saveBoardButton" />
                                        <LoadingButton text="Delete" className="button danger" type="button" onClick={this.handleDeleteBoard} ref="deleteBoardButton" style={{marginLeft: 5}} />
                                    </form>
                                </div>
                            </div>
                            <div className="frame">
                                <div className="heading">Questions<span className="icon mif-question"></span></div>
                                <div className="content">
                                    <div>Select question categories that will be used by Challenge and War spaces:</div>
                                    {this.state.challengeCategories !== null ? this.categoriesList() : <div data-role="preloader" data-type="metro" data-style="dark"></div>}
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
        return {board: null, validation: null, name: null, description: null, challengeCategories: null, turnsToEnd: -1, nameCardsToEnd: -1, safeHavenCardsToEnd: -1, endWithTurns: false, endWithCards: true };
    },
    componentDidMount: function () {
        this.loadBoard();
        $.get("/ChallengeCategories", function (data) {
            var categories = JSON.parse(data);
            this.setState({challengeCategories: categories});
        }.bind(this));
    },
    loadBoard: function () {
        $.get("/Boards/" + this.props.id, function (data) {
            var board = JSON.parse(data);
            this.setState({ board: board, name: board.Name, description: board.Description, turnsToEnd: board.TurnsToEnd, nameCardsToEnd: board.NameCardsToEnd, safeHavenCardsToEnd: board.SafeHavenCardsToEnd, endWithTurns: (board.TurnsToEnd > -1), endWithCards: (board.NameCardsToEnd !== 0 || board.SafeHavenCardsToEnd !== 0) });
            this.validate();
        }.bind(this));
    },
    dummySpaces: function () {
        var dummySpaces = [];
        for (i = 1; i <= 10; i++)
        {
            for (j = 1; j <= 10; j++)
            {
                dummySpaces.push(<EditBoard.DummySpace boardId={this.state.board.Id} x={i} y={j} categories={this.state.challengeCategories} updateCallback={this.loadBoard} key={i + "," + j} />);
            }
        }
        return <div>{dummySpaces}</div>;
    },
    categoriesList: function () {
        var categories = this.state.board.ChallengeCategories.map(function (item) { return item.ChallengeCategoryId });
        return (
            <form onSubmit={this.saveCategories}>
                <div className="treeview" data-role="treeview" ref="categoriesTree">
                    <ul>
                        {this.state.challengeCategories.map(function (item, index) {
                            return (
                                <li data-mode="checkbox" data-name={"category" + item.Id} data-checked={$.inArray(item.Id, categories) !== -1} key={item.Id}>
                                    <span className="leaf">{item.Name}</span>
                                </li>
                                );
                        }, this)
                        }
                    </ul>
                </div>
                <LoadingButton text="Save" ref="saveCategoriesButton" />
            </form>
        );
    },
    validate: function () {
        $.post("/Boards/" + this.props.id + "/Validate", function (data) {
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
    handleEndWithTurnsChange: function (event) {
        this.setState({endWithTurns: !this.state.endWithTurns});
    },
    handleTurnsToEndChange: function (event) {
        this.setState({ turnsToEnd: event.target.value });
    },
    handleEndWithCardsChange: function (event) {
        this.setState({endWithCards: !this.state.endWithCards});
    },
    handleNameCardsToEndChange: function (event) {
        this.setState({ nameCardsToEnd: event.target.value });
    },
    handleSafeHavenCardsToEndChange: function (event) {
        this.setState({ safeHavenCardsToEnd: event.target.value });
    },
    saveBoard: function (event) {
        event.preventDefault();
        this.refs.saveBoardButton.showLoading();
        var formData = new FormData();
        formData.append("Name", this.state.name);
        formData.append("Description", this.state.description);
        formData.append("TurnsToEnd", (this.state.endWithTurns ? this.state.turnsToEnd : -1));
        formData.append("NameCardsToEnd", (this.state.endWithCards ? this.state.nameCardsToEnd : -1));
        var nameCardCount = this.state.board.Spaces.filter(function (item) { return item.NameCard !== null; }).length;
        formData.append("SafeHavenCardsToEnd", ((this.state.endWithCards && ((this.state.nameCardsToEnd < 0) || (this.state.nameCardsToEnd >= nameCardCount))) ? this.state.safeHavenCardsToEnd : -1));
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
    saveCategories: function (event) {
        event.preventDefault();
        this.refs.saveCategoriesButton.showLoading();
        $.post("/Boards/" + this.props.id + "/ChallengeCategories",
            {
                ChallengeCategories: JSON.stringify(GetSelectedCategories(this.refs.categoriesTree))
            },
            function (data) {
                this.refs.saveCategoriesButton.hideLoading();
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
        React.render(<EditBoard.EditSpace space={this.props.space} categories={this.props.categories} updateCallback={this.props.updateCallback} />, document.getElementById("editPane"));
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
        var space = { BoardId: this.props.boardId, Type: 0, Order: 0, IconId: 0, Image: null, X: this.props.x, Y: this.props.y, BackgroundColorId: 0, TextColorId: 0, ChallengeCategories: [] };
        React.render(<EditBoard.EditSpace space={space} categories={this.props.categories} updateCallback={this.props.updateCallback} />, document.getElementById("editPane"));
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
                                    <div style={{marginRight: 5}}>Icon:</div>
                                    <IconSelector selectedIcon={this.state.space.Icon} onSelect={this.handleIconChange} />
                                </div>
                                <div style={{display: "flex", alignItems: "center"}}>
                                    <div style={{marginRight: 5}}>Background color:</div>
                                    <ColorSelector selectedColor={this.state.space.BackgroundColor} onSelect={this.handleBackgroundColorChange} />
                                </div>
                                <div style={{display: "flex", alignItems: "center"}}>
                                    <div style={{marginRight: 5}}>Text color:</div>
                                    <ColorSelector selectedColor={this.state.space.TextColor} onSelect={this.handleTextColorChange} />
                                </div>
                                {this.challengeSelection()}
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
        $.get("/SpaceTypes", function (data) {
            this.setState({spaceTypes: JSON.parse(data)});
        }.bind(this));
    },
    challengeSelection: function () {
        if (!this.state.space || (this.state.spaceTypes.length < 1)) {
            return null;
        }

        var spaceType = this.state.spaceTypes[this.state.space.Type].Name;
        if ((spaceType === "Challenge") || (spaceType === "War"))  {
            var categories = this.state.space.ChallengeCategories.map(function (item) { return item.ChallengeCategoryId; });
            return (
                <div>
                    <div>Question categories:</div>
                    <div className="treeview" data-role="treeview" ref="spaceNameCardCategoriesTree">
                        <ul>
                            {this.props.categories.map(function (item, index) {
                                return (
                                    <li data-mode="checkbox" data-name={"category" + item.Id} data-checked={$.inArray(item.Id, categories) !== -1}  key={item.Id}>
                                        <span className="leaf">{item.Name}</span>
                                    </li>
                                );
                            }, this)
                            }
                        </ul>
                    </div>
                </div>
            );
        }
        else {
            return null;
        }
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
        formData.append("IconId", this.state.space.IconId);
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
        if ((spaceType === "Challenge") || (spaceType === "War")) {
            formData.append("ChallengeCategories", JSON.stringify(GetSelectedCategories(this.refs.spaceNameCardCategoriesTree)));
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
    handleIconChange: function (selectedIcon) {
        this.state.space.IconId = selectedIcon.Id;
        this.state.space.Icon = selectedIcon;
        this.setState({ space: this.state.space });
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

function GetSelectedCategories (tree) {
    var categoriesTree = $(React.findDOMNode(tree));
    var categoryCheckboxes = categoriesTree.find("[name^='category']");
    var selectedCategoryIds = [];
    $.each(categoryCheckboxes, function (index, value) {
        if ($(value).prop("checked")) {
            selectedCategoryIds.push($(value).attr("name").replace("category", ""));
        }
    });
    return selectedCategoryIds;
}