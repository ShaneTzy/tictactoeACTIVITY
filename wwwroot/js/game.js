$(document).ready(function () {
    // Add AI mode toggle functionality
    let aiModeEnabled = false;

    $('#ai-mode').change(function () {
        aiModeEnabled = $(this).is(':checked');
        resetGame();
    });

    // Handle cell click
    $('.game-cell').click(function () {
        const position = $(this).data('position');
        makeMove(position);
    });

    // Reset game
    $('#reset-game').click(function () {
        resetGame();
    });

    // Reset stats
    $('#reset-stats').click(function () {
        resetStats();
    });

    // Make a move
    function makeMove(position) {
        $.ajax({
            url: '/Game/MakeMove',
            type: 'POST',
            data: { position: position },
            success: function (response) {
                if (response.success) {
                    updateBoard(response.board);
                    $('#current-player').text(response.currentPlayer);

                    if (response.isGameOver) {
                        updateStats(response);
                        if (response.winner) {
                            $('#game-status').html(`Player <strong>${response.winner}</strong> wins!`);
                        } else {
                            $('#game-status').html('Game ended in a draw!');
                        }
                    } else {
                        $('#game-status').html(`Current Player: <strong>${response.currentPlayer}</strong>`);

                        // If AI mode is enabled and it's O's turn
                        if (aiModeEnabled && response.currentPlayer === 'O') {
                            setTimeout(function () {
                                makeAIMove();
                            }, 500);
                        }
                    }
                }
            }
        });
    }

    // Make AI move
    function makeAIMove() {
        $.ajax({
            url: '/Game/MakeAIMove',
            type: 'POST',
            success: function (response) {
                updateBoard(response.board);
                $('#current-player').text(response.currentPlayer);

                if (response.isGameOver) {
                    updateStats(response);
                    if (response.winner) {
                        $('#game-status').html(`Player <strong>${response.winner}</strong> wins!`);
                    } else {
                        $('#game-status').html('Game ended in a draw!');
                    }
                } else {
                    $('#game-status').html(`Current Player: <strong>${response.currentPlayer}</strong>`);
                }
            }
        });
    }

    // Reset the game
    function resetGame() {
        $.ajax({
            url: '/Game/Reset',
            type: 'POST',
            success: function (response) {
                updateBoard(response.board);
                $('#current-player').text(response.currentPlayer);
                $('#game-status').html(`Current Player: <strong>${response.currentPlayer}</strong>`);
            }
        });
    }

    // Reset stats
    function resetStats() {
        $.ajax({
            url: '/Game/ResetStats',
            type: 'POST',
            success: function (response) {
                $('#player-x-wins').text(response.playerXWins);
                $('#player-o-wins').text(response.playerOWins);
                $('#draws').text(response.draws);
                resetGame();
            }
        });
    }

    // Update the board display
    function updateBoard(board) {
        for (let i = 0; i < 9; i++) {
            const cell = $(`.game-cell[data-position="${i}"]`);
            cell.find('span').text(board[i]);

            if (board[i] === 'X') {
                cell.attr('data-player', 'X');
            } else if (board[i] === 'O') {
                cell.attr('data-player', 'O');
            } else {
                cell.removeAttr('data-player');
            }
        }
    }

    // Update stats display
    function updateStats(response) {
        $('#player-x-wins').text(response.playerXWins);
        $('#player-o-wins').text(response.playerOWins);
        $('#draws').text(response.draws);
    }
});