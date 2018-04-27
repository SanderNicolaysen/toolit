function ConfirmDialog (arg1, arg2, clickHandlerSet) {
    modal = $("#confirmDialog");
    modal.modal('show'); // Show dialog
    
    // One param => only callback - use default values
    if (arg2 === undefined) {
        callback = arg1;
        options = {}
    } else {
        options = arg1;
        callback = arg2;
    }

    // Standard values
    var labels = {
        title: "Bekreft",
        message: "Er du sikker på at du vil fortsette?",
        dismissBtn: "Nei",
        confirmBtn: "Ja"
    };
    
    // Parse options
    if (options.title != undefined) labels.title = options.title;
    if (options.message != undefined) labels.message = options.message;
    if (options.dismissBtn != undefined) labels.dismissBtn = options.dismissBtn;
    if (options.confirmBtn != undefined) labels.confirmBtn = options.confirmBtn;

    modal.find('.modal-title').text(labels.title);
    modal.find('.dialogMessage').text(labels.message);
    modal.find('.btn.btn-default').text(labels.dismissBtn);
    modal.find('.btn.btn-primary').text(labels.confirmBtn);

    // Make sure that only one click event handler is set
    if (!clickHandlerSet)
    {
        // Set up callback when the user clicks on the confirmation button
        modal.find('.btn.btn-primary').click(function(){
            modal.modal('hide'); // Hide dialog
            callback();
        });
    }
}