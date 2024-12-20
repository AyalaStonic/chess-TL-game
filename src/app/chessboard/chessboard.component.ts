import { Component, OnInit } from '@angular/core';
import { ChessService } from '../chess.service';
import { HttpClient } from '@angular/common/http';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-chessboard',
  standalone: true,
  templateUrl: './chessboard.component.html',
  styleUrls: ['./chessboard.component.css'],
  providers: [ChessService, HttpClient],
  imports: [CommonModule],
})
export class ChessboardComponent implements OnInit {
  boardState: string[][] = [];
  selectedSquare: string | null = null;
  invalidMoveMessage: string | null = null;
  currentGame: any;
  games: any[] = [];
  gameId: number | null = null;

  constructor(private chessService: ChessService, private http: HttpClient) {}

  ngOnInit() {
    this.initializeBoardState();

    // Fetch existing games (if needed)
    this.chessService.getAllGames().subscribe((games: any) => {
      this.games = games;
    });

    // Get the first game (for example purposes, adjust as necessary)
    this.currentGame = this.games.length > 0 ? this.games[0] : null;
    if (this.currentGame) {
      this.gameId = this.currentGame.id;
    }
  }

  // Initialize the chessboard to its default state
  initializeBoardState() {
    this.boardState = [
      ['R', 'N', 'B', 'Q', 'K', 'B', 'N', 'R'],
      ['P', 'P', 'P', 'P', 'P', 'P', 'P', 'P'],
      ['', '', '', '', '', '', '', ''],
      ['', '', '', '', '', '', '', ''],
      ['', '', '', '', '', '', '', ''],
      ['', '', '', '', '', '', '', ''],
      ['p', 'p', 'p', 'p', 'p', 'p', 'p', 'p'],
      ['r', 'n', 'b', 'q', 'k', 'b', 'n', 'r']
    ];
  }

  // Determine the CSS class for a square based on its row and column index
  getSquareClass(rowIndex: number, colIndex: number): string {
    return (rowIndex + colIndex) % 2 === 0 ? 'light' : 'dark';
  }

  // Convert row and column indices to a chess square notation (e.g., A1, H8)
  getSquareFromIndices(rowIndex: number, colIndex: number): string {
    return `${String.fromCharCode(65 + colIndex)}${8 - rowIndex}`;
  }

  // Handle a click on a square
  onSquareClick(rowIndex: number, colIndex: number) {
    const square = this.getSquareFromIndices(rowIndex, colIndex);

    if (this.selectedSquare) {
      // Make the move if a square is selected
      this.makeMove(this.selectedSquare, square);
      this.selectedSquare = null;
    } else {
      // Select a square to start the move
      this.selectedSquare = square;
    }
  }

  // Perform the move (send it to the backend)
  makeMove(from: string, to: string) {
    const move = `${from}-${to}`;

    this.chessService.movePiece(this.currentGame.id, move).subscribe(
      (response: any) => {
        this.selectedSquare = null;
        this.invalidMoveMessage = null;
        // Update the game state after the move
        this.chessService.updateGame(this.currentGame).subscribe();
      },
      (error) => {
        this.invalidMoveMessage = 'Invalid move!';
        console.error('Error making move:', error);
      }
    );
  }

  // Start a new game
  startNewGame() {
    this.chessService.startNewGame().subscribe((newGame: any) => {
      this.initializeBoardState();
      this.selectedSquare = null;
      this.invalidMoveMessage = null;
      this.currentGame = newGame;
      this.gameId = newGame.id;
    });
  }

  // Reset the current game
  resetGame(): void {
    if (this.gameId !== null) {
      this.chessService.resetGame(this.gameId).subscribe(
        (response) => {
          this.initializeBoardState();
          this.selectedSquare = null;
          this.invalidMoveMessage = null;
        },
        (error) => {
          console.error('Error resetting game:', error);
        }
      );
    } else {
      console.error('No game selected to reset!');
    }
  }

  // Save the current game
  saveGame() {
    this.chessService.saveGame(this.currentGame).subscribe(
      (response) => {
        console.log('Game saved successfully:', response);
      },
      (error) => {
        console.error('Error saving game:', error);
      }
    );
  }

  // Load all games
  loadGames() {
    this.chessService.getAllGames().subscribe((games: any) => {
      this.games = games;
    });
  }

  // Map the board state to piece images for display
  getPieceImage(piece: string): string {
    if (!piece) return ''; // Return empty string if no piece
    const pieceMap: { [key: string]: string } = {
      'K': 'white_king',
      'Q': 'white_queen',
      'R': 'white_rook',
      'B': 'white_bishop',
      'N': 'white_knight',
      'P': 'white_pawn',
      'k': 'black_king',
      'q': 'black_queen',
      'r': 'black_rook',
      'b': 'black_bishop',
      'n': 'black_knight',
      'p': 'black_pawn',
    };
    return `./chess-pieces/${pieceMap[piece]}.png`;
  }

  // Replay the game (fetch all moves and play them)
  replayGame() {
    if (this.gameId !== null) {
      this.chessService.getMoves(this.gameId).subscribe((moves: any) => {
        moves.forEach((move: string, index: number) => {
          setTimeout(() => {
            // Simulate the move on the board
            const from = move.split('-')[0];
            const to = move.split('-')[1];
            this.makeMove(from, to);
          }, index * 1000);  // Adjust timing between moves if necessary
        });
      });
    } else {
      console.error('No game selected for replay!');
    }
  }
}
