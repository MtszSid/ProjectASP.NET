$(function () {
    var mainHub = $.connection.mainHub;
    mainHub.client.displayMessage = function (name, message) {
        // Html encode display name and message. 
        var encodedName = $('<div />').text(name).html();
        var encodedMsg = $('<div />').text(message).html();
        // Add the message to the page. 
        $('#discussion').append('<li><strong>' + encodedName
            + '</strong>:&nbsp;&nbsp;' + encodedMsg + '</li>');
    };
    var groupId = null
    // Start the connection.
    $.connection.hub.start().done(function () {

        function getUpdatedList() {

            var groups = document.getElementById("Groups");

            if (groupId != null && groups.style.display == "none") {
                return;
            }

            mainHub.server.getGroups().done(function (result) {
                var list = document.createElement("ul");
                for (var i in result) {
                    var anchor = document.createElement("a");
                    anchor.onclick = (function (id) {
                        return function () {
                            var user = document.querySelector('#User');
                            mainHub.server.joinGroup(user.dataset.name, id).done(function (result) {
                                if (result == true) {
                                    groupId = id;
                                    var groups = document.getElementById("Groups");
                                    var board = document.getElementById("GameBoard");
                                    groups.style.display = "none";
                                    board.style.display = "block";
                                }
                            });
                        }
                    })(result[i].GroupId);

                    anchor.innerText = result[i].GroupId;

                    var elem = document.createElement("li");
                    elem.appendChild(anchor);
                    var text = document.createTextNode("    " + result[i].playerName)
                    elem.appendChild(text)
                    list.appendChild(elem);
                }

                var listOfGroups = document.getElementById("listOfGroups");
                while (listOfGroups.firstChild) {
                    listOfGroups.firstChild.remove();
                }
                listOfGroups.appendChild(list);
            });
        };

        function proposeMove(cell) {
            if (groupId != null) {
                mainHub.server.makeMove(groupId, cell);
            }
        }

        function getGameData() {

            var game = document.getElementById("GameBoard");
            if (groupId == null) {
                return;
            }
            if (groupId != null && game.style.display == "none") {
                game.style.display = "block"
            }

            mainHub.server.getGameState(groupId).done(function (result) {
                if (result == null) {
                    return
                }
                var info = document.getElementById("temp_info");
                var noughtsId = result.NoughtsClientId;
                var crossesId = result.CrossesClientId;
                var nougthsName = result.NoughtsName;
                var crossesName = result.CrossesName;
                var turn = result.Turn;
                var ended = result.Ended;
                var win = result.Winner;
                var data = result.Board;

                if (win == null && turn == noughtsId) {
                    document.getElementById("turnWinInfo").textContent = "Turn:";
                    document.getElementById("turnWinInfoUserName").textContent = "O";
                }
                if (win == null && turn == crossesId) {
                    document.getElementById("turnWinInfo").textContent = "Turn:";
                    document.getElementById("turnWinInfoUserName").textContent = "X";
                }
                if (win != null) {
                    document.getElementById("turnWinInfo").textContent = "Winner:";
                    document.getElementById("turnWinInfoUserName").textContent = win;
                }

                if (noughtsId != null) {
                    document.getElementById("noughtsUserName").textContent = nougthsName;
                }
                else {
                    document.getElementById("noughtsUserName").textContent = nougthsName + "(disc.)";
                }

                if (crossesId != null) {
                    document.getElementById("crossesUserName").textContent = crossesName;
                }
                else {
                    document.getElementById("crossesUserName").textContent = crossesName + "(disc.)";
                }

                info.textContent = result.GroupId;

                for (let i = 0; i < 3; i++) {
                    for (let j = 0; j < 3; j++) {
                        var cell = document.getElementById("c" + i + j);
                        cell.textContent = data[i][j];
                    }
                }
            });
        };

        var messageFrequency = 1,
            updateRate = 1000 / messageFrequency;
        setInterval(getUpdatedList, updateRate);
        setInterval(getGameData, updateRate);

        $('#newGroup').click(function () {
            var group_id = Math.random().toString(36).slice(2);
            var user = document.querySelector('#User');
            mainHub.server.joinGroup(user.dataset.name, group_id).done(function (result) {
                if (result == true) {
                    groupId = group_id;
                    var groups = document.getElementById("Groups");
                    var board = document.getElementById("GameBoard");
                    groups.style.display = "none";
                    board.style.display = "block";
                }
            });
        });

        $('#c00').click(function () {
            proposeMove("0,0");
        });
        $('#c01').click(function () {
            proposeMove("0,1");
        });
        $('#c02').click(function () {
            proposeMove("0,2");
        });
        $('#c10').click(function () {
            proposeMove("1,0");
        });
        $('#c11').click(function () {
            proposeMove("1,1");
        });
        $('#c12').click(function () {
            proposeMove("1,2");
        });
        $('#c20').click(function () {
            proposeMove("2,0");
        });
        $('#c21').click(function () {
            proposeMove("2,1");
        });
        $('#c22').click(function () {
            proposeMove("2,2");
        });
    });
});