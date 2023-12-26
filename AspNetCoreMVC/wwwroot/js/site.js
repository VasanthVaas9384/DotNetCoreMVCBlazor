// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function confirmDelete(uniqueId, isDeleteClicked) {
    let deleteSpan = 'deleteSpan_' + uniqueId;
    let ConfirmDeleteSpan = 'confirmDeleteSpan_' + uniqueId;

    if (isDeleteClicked) {
        $('#' + deleteSpan).hide();
        $('#' + ConfirmDeleteSpan).show();
    }
    else {
        $('#' + deleteSpan).show();
        $('#' + ConfirmDeleteSpan).hide();
    }
}