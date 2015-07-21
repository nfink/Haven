﻿/** @jsx React.DOM */
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
                    <button className="image-button primary" onClick={this.addQuestion} style={{marginLeft: 10}}>Add new question<span className="icon mif-plus bg-darkCobalt"></span></button>
                    <div className="grid padding10 bg-white">
                        <div className="row cells2">
                            <div className="cell">
                                <div className="listview-outlook" data-role="listview">
                                    {this.state.categories.map(function(item, index){
                                        return (
                                            <div className="list-group" key={item.Id}>
                                                <span className="list-group-toggle">{item.Name}</span>
                                                <div className="list-group-content">
			                                        {this.questionsInCategory(item.Id, this.state.challenges)}
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
    questionsInCategory (categoryId, challenges) {
        return challenges.filter(function (value) {
            return value.ChallengeCategoryId === categoryId;
        })
        .map(function (item, index) {
            return (
                <AdminQuestions.QuestionListItem challenge={item} categories={this.state.categories} updateCallback={this.componentDidMount.bind(this)} key={item.Id} />
            );
        }, this);
    },
    addCategory: function () {
    
    },
    addQuestion: function () {
        React.unmountComponentAtNode(document.getElementById("editChallenge"));
        var challenge = { Id: 0, Name: "", ChallengeCategoryId: 0, Question: "", Answers: []}
        React.render(<AdminQuestions.EditChallenge challenge={challenge} categories={this.state.categories} updateCallback={this.componentDidMount.bind(this)} />, document.getElementById("editChallenge"));
    }
});


AdminQuestions.QuestionListItem = React.createClass({
    render: function () {
        return (
            <div className="list" onClick={this.select}>
                <div className="list-content">
                    <span className="list-title">{this.props.challenge.Name}</span>
                    <span className="list-subtitle">{this.props.challenge.Question}</span>
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
                <label htmlFor="challengeName">Name:</label>
                <div className="input-control text" style={{marginLeft: 5}}>
                    <input id="challengeName" type="text" value={this.state.name} placeholder="Enter name here..." onChange={this.handleNameChange} />
                </div>
                <br />
                <label htmlFor="challengeCategory">Category:</label>
                <div className="input-control select" style={{marginLeft: 5}}>
                    <select id="challengeCategory" value={this.state.category} onChange={this.handleCategoryChange}>
                        <option value="0" key="0">None</option>
                        {this.props.categories.map(function(item, index){
                            return (
                                <option value={item.Id} key={item.Id}>{item.Name}</option>
                            );
                        }, this)}
                    </select>
                </div>
                <br />
                <label htmlFor="challengeQuestion">Question:</label>
                <div className="input-control text" style={{marginLeft: 5}}>
                    <input id="challengeQuestion" type="text" value={this.state.question} placeholder="Enter question here..." onChange={this.handleQuestionChange} />
                </div>
                <br />
                <button className="image-button success" onClick={this.addAnswer} type="button">Add answer<span className="icon mif-plus bg-darkGreen"></span></button>
                <div>
                    {this.state.answers.map(function(item, index){
                        return (
                            <AdminQuestions.EditAnswer answer={item.Answer} correct={item.Correct} key={item.Id} id={item.Id} onRemove={this.removeAnswer} />
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
        return {name: challenge ? challenge.Name : "", category: challenge ? challenge.ChallengeCategoryId : 0, question: challenge ? challenge.Question : "", answers: challenge ? challenge.Answers : [], nextAnswerId: answerId};
    },
    save: function (event) {
        event.preventDefault();
        this.refs.saveButton.showLoading();
        // ajax stuff
        // saveButton.hideLoading();
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
    handleCategoryChange: function (event) {
        this.setState({category: event.target.value});
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
                    <input type="text" value={this.state.answer} placeholder="Enter answer here..." onChange={this.handleAnswerInput} />
                </div>
                <label className="input-control checkbox" style={{marginLeft: 5}}>
                    <input type="checkbox" checked={this.state.correct} onChange={this.handleCorrectInput} />
                    <span className="check"></span>
                    <span className="caption">Correct</span>
                </label>
            </div>
        );
    },
    getInitialState: function () {
        return {answer: this.props.answer ? this.props.answer : "", correct: this.props.correct ? this.props.correct : false };
    },
    onRemove: function () {
        this.props.onRemove(this.props.id);
    },
    handleAnswerInput: function (event) {
        this.setState({answer: event.target.value});
    },
    handleCorrectInput: function (event) {
        this.setState({correct: !this.state.correct});
    }
});