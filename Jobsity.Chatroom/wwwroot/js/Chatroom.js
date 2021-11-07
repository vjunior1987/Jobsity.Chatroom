// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
$(document).ready(function () {
    retrieveMsg();
});

function retrieveMsg() {
    $.ajax({
        url: "/Home/RetrieveMessage",
        type: "GET",
        success: function (result) {
            $("#divChatbox").html(result);
            $("#divChatbox").scrollTop($("#divChatbox")[0].scrollHeight);
            setTimeout(retrieveMsg, 5000);
        },
        error: function (error) {
            alert(error);
        }
    })
}

$("#send").on('click', function () {
    $.ajax({
        url: "/Home/SendMessage",
        type: "POST",
        data: { message: $("#inpstrTextbox").val() },
        error: function (error) {
            alert(error.message);
        },
        complete: function() {
            $("#inpstrTextbox").val('');
        }
    });
});
