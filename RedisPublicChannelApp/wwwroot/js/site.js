"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

connection.start().then(function () {
    console.log("Connected");

}).catch(function (err) {
    return console.error(err.toString());
})


connection.on("newmessage", async function (obj) {
    let myobj = {
        channell: obj.channel,
        message: obj.message,
        time: obj.time
    }
    console.log("newmessage");
    console.log(myobj)
    let cname = document.getElementById("channelname").innerHTML;
    console.log(cname)
    if (myobj.channell === cname) {
        let content = `
                    <div class="message">
                    <h5>${myobj.message}</h5>
                    <h6>${myobj.time}</h6>
                    </div>
                    `
        $('#messages').append(content);
    }
})


connection.on("newchannel", function () {
    getChannels();
})

let inp = document.getElementById("nameinput");
let messageinp = document.getElementById("messageinput");
let form = document.getElementById("channelform");
let messageform = document.getElementById("messageform");
let channels = document.getElementById("channels");



function AddChannelClickEvent() {

    let channel = document.querySelectorAll(".channel");
    for (var i = 0; i < channel.length; i++) {
        let element = channel[i];
        element.addEventListener('click', function () {
            let channelName = element.innerHTML;
            document.getElementById("channelname").innerHTML = channelName;
            messageform.style.display = "block";

            Subscribe();
            GetMessages(channelName);

        });
    }

}

AddChannelClickEvent();

form.onsubmit = (event) => {

    event.preventDefault();
    if (inp.value === "") return;
    $.ajax({
        url: `Home/MakeChannel?channelName=${inp.value}`,
        method: 'GET',
        success: function () {
            form.reset();
            getChannels();
        },
        error: function (xhr, status, error) {
            console.error(status, error);
        }
    });
}

function GetMessages(channelname) {
    $.ajax({
        url: `Home/GetChannelMessages?channelName=${channelname}`,
        success: function (data) {
            let content = "";
            if (data) {
                console.log(data)
                for (var i = 0; i < data.length; i++) {
                    content += `
                    <div class="message">
                    <h5>${data[i].message}</h5>
                    <h6>${data[i].timeStamp}</h6>
                    </div>
                    `
                }
                $('#messages').html(content);
            }
            else {
                console.log("Empty messages")
                $('#messages').html("<p>Messages Empty</p>");

            }
        },
        error: function (err) {
            console.log(err);
        }
    })
}


function Subscribe() {
    let cname = document.getElementById("channelname").innerHTML;
    $.ajax({
        url: `Home/Subscribe?channelName=${cname}`,
        success: function (data) {
            console.log("subscribed");
        },
        error: function (err) {
            console.log(err);
        }
    })
}


function getChannels() {
    $.ajax({
        url: `Home/GetChannels`,
        success: function (data) {
            let content = "";
            if (data.length > 0) {
                for (var i = 0; i < data.length; i++) {
                    content += `
                            <h4 class="channel">${data[i]}</h4>
                            `
                }
                channels.innerHTML = content;
            }
            AddChannelClickEvent();

        },
        error: function (err) {
            console.log(err);
        }
    })
}


messageform.onsubmit = (event) => {
    let channelName = document.getElementById("channelname").innerHTML;
    event.preventDefault();
    if (messageinp.value === "") return;
    $.ajax({
        url: `Home/AddMessage`,
        method: 'GET',
        data: {
            channelName: channelName,
            message: messageinp.value
        },
        success: function () {
            console.log("ok");
            messageform.reset();
        },
        error: function (xhr, status, error) {
            console.error(status, error);
        }
    });
}