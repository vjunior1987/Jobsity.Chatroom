// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
var chatId = 1;

"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/chatroomHub").build();

//Disable send button until connection is established
document.getElementById("send").disabled = true;

connection.on("ReceiveMessage", function () {
    retrieveMsg();
});

connection.on("RequestBotMessage", function (chatid) {
    if (chatId) {
        connection.invoke("ConsumerMessageBus").catch(function (err) {
            return console.error(err.toString());
        });
    }
});

connection.on("PublishBotMessage", function (message) {
    $.ajax({
        url: "/Home/PublishBotMessage",
        type: "POST",
        data: message,
        error: function (err) {
            alert(err.message);
        },
        complete: function () {
            $("#inpstrTextbox").val('');
        }
    });
});

connection.start().then(function () {
    document.getElementById("send").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});


$(document).ready(function () {
    retrieveMsg();
});

function retrieveMsg() {
    $.ajax({
        url: `/Home/RetrieveMessage/`,
        type: "GET",
        data: { chatId },
        success: function (result) {
            $("#divChatbox").html(result);
            $("#divChatbox").scrollTop($("#divChatbox")[0].scrollHeight);
        },
        error: function (error) {
        }
    })
}

$("#send").on('click', function () {
    var message = $("#inpstrTextbox").val();
    if (message && message !== "") {
        if (message.indexOf('/', 0) !== -1) {
            connection.invoke("SendBotMessage", message, chatId).catch(function (err) {
                return console.error(err.toString());
            });
            $("#inpstrTextbox").val('');
        } else {
            $.ajax({
                url: "/Home/SendMessage",
                type: "POST",
                data: { message: $("#inpstrTextbox").val(), chatId },
                error: function (err) {
                    alert(err.message);
                },
                complete: function () {
                    $("#inpstrTextbox").val('');
                }
            });
        }
    }
});

function changeChatroom(value) {
    switch (value) {
        case 1:
            $(".chat1").attr("disabled", true);
            $(".chat2").removeAttr("disabled");
            $("h3").html("Chatroom 1");
            break;
        case 2:
            $(".chat2").attr("disabled", true);
            $(".chat1").removeAttr("disabled");
            $("h3").html("Chatroom 2");
            break;
    }
    chatId = value;
    $.ajax({
        url: `/Home/RetrieveMessage/`,
        type: "GET",
        data: { chatId },
        success: function (result) {
            $("#divChatbox").html(result);
            $("#divChatbox").scrollTop($("#divChatbox")[0].scrollHeight);
        },
        error: function (error) {
        }
    })
}
