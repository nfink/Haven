function OpenDialog(dialog) {
    // metro ui does something silly when creating so we have to wait for the dialog to be ready before opening
    if ($(React.findDOMNode(dialog)).data('dialog') == undefined) {
        window.setTimeout(function () {OpenDialog(dialog)}, 10);
    }
    else {
        $(React.findDOMNode(dialog)).data('dialog').open();
    }
}

function CloseDialog(dialog) {
    $(React.findDOMNode(dialog)).data('dialog').close();
}