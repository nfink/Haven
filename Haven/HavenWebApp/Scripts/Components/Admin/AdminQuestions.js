/** @jsx React.DOM */
/// <reference path="https://cdnjs.cloudflare.com/ajax/libs/react/0.13.3/react.js" />
/// <reference path="https://cdnjs.cloudflare.com/ajax/libs/react/0.13.3/JSXTransformer.js" />

var AdminQuestions = React.createClass({
    render: function () {
        if ((this.state.categories === null) || (this.state.challenges === null)) {
            return (
                <div className="padding20" data-role="preloader" data-type="metro" data-style="dark"></div>
            );
        }
        else {
            return (
                <div className="container page-content">
                    <button className="image-button primary" onClick={this.addCategory}>Add new category<span className="icon mif-folder-plus bg-darkCobalt"></span></button>
                    <div className="grid padding10 bg-white">
                        <div className="row cells2">
                            <div className="cell">
                                <div className="listview-outlook" data-role="listview">
                                    {this.uncategorizedQuestions()}
                                    {this.state.categories.map(function(item, index){
                                        return (
                                            <div className="list-group" key={item.Id}>
                                                <div className="list-group-toggle" >
                                                    <span >{item.Name}</span>
                                                </div>
                                                <button onClick={function () {this.handleEditCategory(item);}.bind(this)} className="cycle-button mini-button" type="button" style={{position: "absolute", width: "1.2rem", height: "1.2rem", top: -1, right: 10}}><span className="mif-pencil"></span></button>
                                                <button onClick={function () {this.addQuestion(item.Id);}.bind(this)} className="cycle-button mini-button" type="button" style={{position: "absolute", width: "1.2rem", height: "1.2rem", top: -1, right: 34}}><span className="mif-plus"></span></button>
                                                <div className="list-group-content">
			                                        {this.questionsInCategory(item.Id)}
		                                        </div>
                                            </div>
                                        );
                                    }, this)}
                                </div>
                            </div>
                            <div className="cell">
                                <div id="editChallenge"></div>
                            </div>
                        </div>
                    </div>
                    <div id="deleteDialog"></div>
                </div>
            );
        }
    },
    getInitialState: function () {
        return {categories: null, challenges: null};
    },
    componentDidMount: function () {
        $.get("Challenges", function (data) {
            var challenges = JSON.parse(data);
            this.setState({challenges: challenges});
        }.bind(this));
        $.get("ChallengeCategories", function (data) {
            var categories = JSON.parse(data);
            this.setState({categories: categories});
        }.bind(this));
    },
    questionsInCategory (categoryId) {
        return this.state.challenges.filter(function (value) {
            return value.ChallengeCategoryId === categoryId;
        })
        .map(function (item, index) {
            return (
                <AdminQuestions.QuestionListItem challenge={item} categories={this.state.categories} updateCallback={this.componentDidMount.bind(this)} key={item.Id} />
            );
        }, this);
    },
    uncategorizedQuestions () {
        var uncategorized = this.questionsInCategory(0);
        if (uncategorized.length > 0) {
            return (
                <div className="list-group">
                    <span className="list-group-toggle">Uncategorized</span>
                    <button onClick={function () {this.addQuestion(0);}.bind(this)} className="cycle-button mini-button" type="button" style={{position: "absolute", width: "1.2rem", height: "1.2rem", top: -1, right: 10}}><span className="mif-plus"></span></button>
                    <div className="list-group-content">
		                {uncategorized}
	                </div>
                </div>
            );
        }
        else {
            return null;
        }
    },
    handleEditCategory: function (category) {
        React.unmountComponentAtNode(document.getElementById("editChallenge"));
        React.render(<AdminQuestions.EditCategory category={category} updateCallback={this.componentDidMount.bind(this)} />, document.getElementById("editChallenge"));
    },
    addQuestion: function (categoryId) {
        React.unmountComponentAtNode(document.getElementById("editChallenge"));
        var challenge = { Id: 0, Name: "", ChallengeCategoryId: categoryId, Question: "", Answers: []}
        React.render(<AdminQuestions.EditChallenge challenge={challenge} categories={this.state.categories} updateCallback={this.componentDidMount.bind(this)} />, document.getElementById("editChallenge"));
    },
    addCategory: function () {
        React.unmountComponentAtNode(document.getElementById("editChallenge"));
        var category = { Id: 0, Name: "" };
        React.render(<AdminQuestions.EditCategory category={category} updateCallback={this.componentDidMount.bind(this)} />, document.getElementById("editChallenge"));
    }
});


AdminQuestions.QuestionListItem = React.createClass({
    render: function () {
        return (
            <div className="list" onClick={this.select}>
                <div className="list-content">
                    <span className="list-title">{this.props.challenge.Question}</span>
                    <span className="list-subtitle"></span>
                    {this.props.challenge.Answers.map(function(item, index){
                        return (<span className="list-remark" key={item.Id}>{"• " + item.Answer}</span>);
                    })}
                </div>
		    </div>
        );
    },
    select: function () {
        React.unmountComponentAtNode(document.getElementById("editChallenge"));
        React.render(<AdminQuestions.EditChallenge challenge={this.props.challenge} categories={this.props.categories} updateCallback={this.props.updateCallback} />, document.getElementById("editChallenge"));
    }
});

AdminQuestions.EditChallenge = React.createClass({
    render: function () {
        return (
            <form onSubmit={this.save}>
                <ComboBox ref="challengeCategory" label="Category:" value={this.state.category} options={this.categories()} />
                <label htmlFor="challengeQuestion">Question:</label>
                <div className="input-control text" style={{marginLeft: 5}}>
                    <input id="challengeQuestion" type="text" value={this.state.question} placeholder="Enter question here..." onChange={this.handleQuestionChange} />
                </div>
                <br />
                <button className="image-button success" onClick={this.addAnswer} type="button">Add answer<span className="icon mif-plus bg-darkGreen"></span></button>
                <div>
                    {this.state.answers.map(function(item, index){
                        return (
                            <AdminQuestions.EditAnswer answer={item} key={item.Id} onRemove={this.removeAnswer} />
                        );
                    }, this)}
                </div>
                <LoadingButton text="Save" ref="saveButton" />
                {this.props.challenge.Id ? <LoadingButton text="Delete" ref="deleteButton" type="button" onClick={this.handleDelete} style={{marginLeft: 5}} /> : null}
                <button className="button primary" type="button" onClick={this.close} style={{marginLeft: 5}}>Close</button>
            </form>
        );
    },
    getInitialState: function () {
        // create a copy of the challenge property so we aren't modifying the parent
        var challenge = JSON.parse(JSON.stringify(this.props.challenge));
        var answerId = 0;
        challenge.Answers.map(function (item, index) {
            item.Id = answerId;
            answerId++;
        });
        // determine category name
        var category = this.props.categories.filter(function(value) {
                return value.Id === challenge.ChallengeCategoryId;
            })
            .map(function (item, index) {
                return item.Name;
            })[0];

        return {category: category, question: challenge ? challenge.Question : "", answers: challenge ? challenge.Answers : [], nextAnswerId: answerId};
    },
    categories: function () {
        return this.props.categories.map(function(item, index){return item.Name;});
    },
    save: function (event) {
        event.preventDefault();
        this.refs.saveButton.showLoading();

        if (this.props.challenge.Id) {
            $.ajax({
                url: "/Challenges/" + this.props.challenge.Id,
                method: "PUT",
                data:
                {
                    Category: this.refs.challengeCategory.value(),
                    Question: this.state.question,
                    Answers: JSON.stringify(this.state.answers),
                },
                success: function () {
                    this.refs.saveButton.hideLoading();
                    this.props.updateCallback();
                }.bind(this)
            })
            .fail(function (data) {
                this.refs.saveButton.hideLoading();
            }.bind(this));
        }
        else {
            $.post("/Challenges",
                {
                    Category: this.refs.challengeCategory.value(),
                    Question: this.state.question,
                    Answers: JSON.stringify(this.state.answers),
                },
                function (data) {
                    this.refs.saveButton.hideLoading();
                    this.props.updateCallback();
                }.bind(this)
            )
            .fail(function (data) {
                this.refs.saveButton.hideLoading();
            }.bind(this));
        }
    },
    handleDelete: function () {
        var dialog = React.render(<DeleteDialog action={this.deleteChallenge} />, document.getElementById("deleteDialog"));
        dialog.open();
    },
    deleteChallenge: function () {
        React.unmountComponentAtNode(document.getElementById("deleteDialog"));
        this.refs.deleteButton.showLoading();
        $.ajax({
            url: "/Challenges/" + this.props.challenge.Id,
            method: "DELETE",
            success: function () {
                this.refs.deleteButton.hideLoading();
                React.unmountComponentAtNode(document.getElementById("editChallenge"));
                this.props.updateCallback();
            }.bind(this)
        })
        .fail(function (data) {
            alert("failed to delete");
            this.refs.deleteButton.hideLoading();
        });
    },
    close: function() {
        React.unmountComponentAtNode(document.getElementById("editChallenge"));
    },
    addAnswer: function () {
        var answer = {Id: this.state.nextAnswerId, Answer: "", Correct: false};
        this.state.answers.push(answer);
        this.setState({answers: this.state.answers, nextAnswerId: this.state.nextAnswerId + 1});
    },
    removeAnswer: function (answerId) {
        var index = this.state.answers.map(function (item, index) {
                return item.Id;
            })
            .indexOf(answerId);
        this.state.answers.splice(index, 1);
        this.setState({answers: this.state.answers});
    },
    handleNameChange: function (event) {
        this.setState({name: event.target.value});
    },
    handleQuestionChange: function (event) {
        this.setState({question: event.target.value});
    }
});

AdminQuestions.EditAnswer = React.createClass({
    render: function () {
        return (
            <div>
                <button className="button small mini-button danger rounded" type="button" onClick={this.onRemove}>
                    <span className="mif-minus"></span>
                </button>
                <div className="input-control text" style={{marginLeft: 5}}>
                    <input type="text" value={this.state.answer.Answer} placeholder="Enter answer here..." onChange={this.handleAnswerInput} />
                </div>
                <label className="input-control checkbox" style={{marginLeft: 5}}>
                    <input type="checkbox" checked={this.state.answer.Correct} onChange={this.handleCorrectInput} />
                    <span className="check"></span>
                    <span className="caption">Correct</span>
                </label>
            </div>
        );
    },
    getInitialState: function () {
        return {answer: this.props.answer};
    },
    onRemove: function () {
        this.props.onRemove(this.props.id);
    },
    handleAnswerInput: function (event) {
        this.state.answer.Answer = event.target.value;
        this.setState({answer: this.state.answer});
    },
    handleCorrectInput: function (event) {
        this.state.answer.Correct = !this.state.answer.Correct;
        this.setState({answer: this.state.answer});
    }
});

AdminQuestions.EditCategory = React.createClass({
    render: function () {
        return (
            <form onSubmit={this.save}>
                <label htmlFor="categoryName">Name:</label>
                <div className="input-control text" style={{marginLeft: 5}}>
                    <input id="categoryName" type="text" value={this.state.name} placeholder="Enter category name here..." onChange={this.handleNameChange} />
                </div>
                <br />
                <LoadingButton text="Save" ref="saveButton" />
                {this.props.category.Id ? <LoadingButton text="Delete" ref="deleteButton" type="button" onClick={this.handleDelete} style={{marginLeft: 5}} /> : null}
                <button className="button primary" type="button" onClick={this.close} style={{marginLeft: 5}}>Close</button>
            </form>
        );
    },
    getInitialState: function () {
        return {name: this.props.category.Name};
    },
    close: function() {
        React.unmountComponentAtNode(document.getElementById("editChallenge"));
    },
    handleNameChange: function (event) {
        this.setState({name: event.target.value});
    },
    save: function (event) {
        event.preventDefault();
        this.refs.saveButton.showLoading();

        if (this.props.category.Id) {
            $.ajax({
                url: "/ChallengeCategories/" + this.props.category.Id,
                method: "PUT",
                data:
                {
                    Name: this.state.name,
                },
                success: function () {
                    this.refs.saveButton.hideLoading();
                    this.props.updateCallback();
                }.bind(this)
            });
        }
        else {
            $.post("/ChallengeCategories",
                {
                    Name: this.state.name,
                },
                function (data) {
                    this.refs.saveButton.hideLoading();
                    this.props.updateCallback();
                }.bind(this)
            );
        }
    },
    handleDelete: function () {
        var dialogMessage = "Any questions in this category will become uncategorized. Are you sure you want to delete?";
        var dialog = React.render(<DeleteDialog action={function () {this.deleteCategory()}.bind(this)} text={dialogMessage} />, document.getElementById("deleteDialog"));
        dialog.open();
    },
    deleteCategory: function () {
        React.unmountComponentAtNode(document.getElementById("deleteDialog"));
        $.ajax({
            url: "/ChallengeCategories/" + this.props.category.Id,
            method: "DELETE",
            success: function () {
                this.close();
                this.props.updateCallback();
            }.bind(this)
        })
        .fail(function (data) {
            alert("failed to delete");
        });
    },
});