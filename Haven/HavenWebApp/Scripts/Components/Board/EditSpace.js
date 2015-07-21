/** @jsx React.DOM */
/// <reference path="https://cdnjs.cloudflare.com/ajax/libs/react/0.13.3/react.js" />
/// <reference path="https://cdnjs.cloudflare.com/ajax/libs/react/0.13.3/JSXTransformer.js" />

var EditSpace = React.createClass({
    render: function () {
        return (
            <form onsubmit={this.editSpace}>
                <div className="header">Edit Space</div>
                <div style={{display: "flex"}}>
                    <div class="row cells2">
                        <div class="cell">
                            <label for="spaceType">Type:</label>
                            <br />
                            <div class="input-control select">
                                <select id="spaceType" name="Type" onchange="TypeSelectionChange(this);">
                                    @foreach (SpaceType type in Enum.GetValues(typeof(SpaceType)))
                                    {
                                        string selected = "";
                                        if (@Model.Type == type)
                                        {
                                            selected = "selected";
                                        }
                                        <option value="@Convert.ChangeType(type, type.GetTypeCode())" @selected>@type.ToString()</option>
                                    }
                                </select>
                            </div>
                            <br />
                            <label for="spaceOrder">Order:</label>
                            <br />
                            <div class="input-control text">
                                <input id="spaceOrder" name="Order" type="text" value="@Model.Order" />
                            </div>
                            <div class="additionalEdit @nameCardEditType" style="@nameCardSelected">
                                <label for="nameCardIcon">Icon:<span id="nameCardIconDisplay" style="float: right;width: 70px; height: 70px; background-image: url('@nameCardImage'); background-size: cover; background-repeat: no-repeat; border-radius: 0px;" /></label>
                                <br />
                                <div id="nameCardIconFileInput" style="width: 350px;">
                                    <input id="nameCardIcon" name="Icon" value="@nameCardImageName" tabindex="-1" style="z-index: 0; display: none;" type="file" onchange="$('#nameCardIconSelectOverlay').val($(this).val()); PreviewImage(this, $('#nameCardIconDisplay'));">
                                    <div class="input-control text" data-role="input">
                                        <input id="nameCardIconSelectOverlay" style="padding-right: 62px;" type="text" readonly="" value="@nameCardImageName" onclick="$('#nameCardIcon').trigger('click');">
                                    </div>
                                </div>
                            </div>
                            <div class="additionalEdit @safeHavenCardEditType" style="@safeHavenCardSelected">
                                <label for="safeHavenCardIcon">Icon:<span id="safeHavenCardIconDisplay" style="float: right;width: 70px; height: 70px; background-image: url('@safeHavenCardImage'); background-size: cover; background-repeat: no-repeat; border-radius: 0px;" /></label>
                                <br />
                                <div id="safeHavenCardIconFileInput" style="width: 350px;">
                                    <input id="safeHavenCardIcon" name="Icon" value="@safeHavenCardImageName" tabindex="-1" style="z-index: 0; display: none;" type="file" onchange="$('#safeHavenCardIconSelectOverlay').val($(this).val()); PreviewImage(this, $('#safeHavenCardIconDisplay'));">
                                    <div class="input-control text" data-role="input">
                                        <input id="safeHavenCardIconSelectOverlay" style="padding-right: 62px;" type="text" readonly="" value="@safeHavenCardImageName" onclick="$('#safeHavenCardIcon').trigger('click');">
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="cell">
                            <div class="additionalEdit @recallEditType" style="@recallSelected">
                                <label for="spaceRecallText">Text:</label>
                                <br />
                                <div class="input-control text">
                                    <input id="spaceRecallText" name="RecallText" type="text" value="@recallText" />
                                </div>
                            </div>
                            <div class="additionalEdit @nameCardEditType" style="@nameCardSelected">
                                <label for="spaceNameCardName">Name:</label>
                                <br />
                                <div class="input-control text">
                                    <input id="spaceNameCardName" name="NameCardName" type="text" value="@nameCardName" />
                                </div>
                                <br />
                                <label for="spaceNameCardDetails">Details:</label>
                                <br />
                                <div class="input-control textarea">
                                    <textarea id="spaceNameCardDetails" name="NameCardDetails">@nameCardDetails</textarea>
                                </div>
                            </div>
                            <div class="additionalEdit @safeHavenCardEditType" style="@safeHavenCardSelected">
                                <label for="spaceSafeHavenCardName">Name:</label>
                                <br />
                                <div class="input-control text">
                                    <input id="spaceSafeHavenCardName" name="SafeHavenCardName" type="text" value="@safeHavenCardName" />
                                </div>
                                <br />
                                <label for="spaceSafeHavenCardDetails">Details:</label>
                                <br />
                                <div class="input-control textarea">
                                    <textarea id="spaceSafeHavenCardDetails" name="SafeHavenCardDetails">@safeHavenCardDetails</textarea>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

                <button class="button primary" type="submit">Save</button>
                @if (@Model.Id != 0)
                            {
                    <button class="button primary" type="button" onclick="ConfirmDelete(function () { DeleteSpace(@Model.Id); });">Delete</button>
                                }
                <button class="button primary" type="button" onclick="$('#editPane').empty();">Close</button>
            </form>
        );
    },
    editSpace: function () {
    
    }
});