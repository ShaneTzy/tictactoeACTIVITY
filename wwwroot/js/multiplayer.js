$(document).ready(function () {
    // Initialize SignalR
    const connection = new signalR.HubConnectionBuilder()
        .withUrl("/gameHub")
        .withAutomaticReconnect()
        .build();

    let playerSymbol = '';
    let currentGameId = '';
    let isMyTurn = false;

    // Start the connection
    connection.start().catch(err => console.error(err));

    // Connection established handler
    connection.onreconnected(() => {
        if (currentGameId) {
            joinGame(currentGameId);
        }
    });

    // Create a new game
    $('#create-game-btn').click(function () {
        // Generate a random game ID
        currentGameId = Math.random().toString(36).substring(2, 8);
        $('#current-game-id').text(currentGameId);
        joinGame(currentGameId);
    });

    // Join an existing game
    $('#join-game-btn').click(function () {
        const gameId = $('#game-id-input').val().trim();
        if (gameId) {
            currentGameId = gameId;
            $('#current-game-id').text(currentGameId);
            joinGame(currentGameId);
        }
    });

    // Join a game
    function joinGame(gameId) {
        connection.invoke("JoinGame", gameId)
            .catch(err => console.error(err));

        // Show game board and hide lobby
        $('#lobby').addClass('d-none');
        $('#game-container').removeClass('d-none');
    }

    // Player assigned handler
    connection.on("PlayerAssigned", function (symbol) {
        playerSymbol = symbol;
        $('#player-symbol').text(symbol);

        if (symbol === 'X') {
            isMyTurn = true;
            $('#game-status').text("Your turn");
        } else {
            isMyTurn = false;
            $('#game-status').text("Opponent's turn");
        }
    });

    // Update board handler
    connection.on("UpdateBoard", function (board) {
        // Update the UI based on the board state
        for (let i = 0; i < 9; i++) {
            $(`.mp-game-cell[data-position="${i}"] span`).text(board.board[i]);
        }

        // Update turn information
        isMyTurn = board.currentPlayer === playerSymbol;

        if (!board.isGameOver) {
            if (isMyTurn) {
                $('#game-status').text("Your turn");
            } else {
                $('#game-status').text("Opponent's turn");
            }
        }
    });

    // Game over handler
    connection.on("GameOver", function (message) {
        $('#game-status').text(message);
    });

    // Player disconnected handler
    connection.on("PlayerDisconnected", function (message) {
        $('#game-status').text(message);
        isMyTurn = false;
    });

    // Make a move
    $('.mp-game-cell').click(function () {
        if (isMyTurn) {
            const position = $(this).data('position');
            connection.invoke("MakeMove", currentGameId, position, playerSymbol)
                .catch(err => console.error(err));
        }
    });

    // Reset game
    $('#reset-game').click(function () {
        connection.invoke("ResetGame", currentGameId)
            .catch(err => console.error(err));
    });

    // Leave game
    $('#leave-game').click(function () {
        location.reload();
    });
});
