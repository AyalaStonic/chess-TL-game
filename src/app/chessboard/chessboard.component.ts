import { Component, OnInit } from '@angular/core';
import { ChessService } from '../services/chess.service';
import { HttpClient } from '@angular/common/http';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-chessboard',
  standalone: true,
  templateUrl: './chessboard.component.html',
  styleUrls: ['./chessboard.component.css'],
  providers: [ChessService, HttpClient], // Provide HttpClient without the `withFetch`
  imports: [CommonModule],
})
export class ChessboardComponent implements OnInit {
  boardState: string[][] = [];
  selectedSquare: string | null = null;
  invalidMoveMessage: string | null = null;
  currentGame: any;
  games: any[] = [];

  constructor(private chessService: ChessService, private http: HttpClient) {}

  ngOnInit() {
    // Initialize the chessboard with the starting pieces
    this.boardState = [
      ['R', 'N', 'B', 'Q', 'K', 'B', 'N', 'R'],  // Black pieces
      ['P', 'P', 'P', 'P', 'P', 'P', 'P', 'P'],  // Black pawns
      ['', '', '', '', '', '', '', ''],         // Empty row
      ['', '', '', '', '', '', '', ''],         // Empty row
      ['', '', '', '', '', '', '', ''],         // Empty row
      ['', '', '', '', '', '', '', ''],         // Empty row
      ['p', 'p', 'p', 'p', 'p', 'p', 'p', 'p'],  // White pawns
      ['r', 'n', 'b', 'q', 'k', 'b', 'n', 'r']        // White pieces
    ];
  
    // Fetch existing games (if needed)
    this.chessService.getAllGames().subscribe((games: any) => {
      this.games = games;
    });
  }

  // Get the class for the square (light or dark)
  getSquareClass(rowIndex: number, colIndex: number): string {
    return (rowIndex + colIndex) % 2 === 0 ? 'light' : 'dark';
  }

  // Convert row and column indices to chess notation (e.g., "A1", "H8")
  getSquareFromIndices(rowIndex: number, colIndex: number): string {
    return `${String.fromCharCode(65 + colIndex)}${8 - rowIndex}`;
  }

  // Handle square click event to select or move pieces
  onSquareClick(rowIndex: number, colIndex: number) {
    const square = this.getSquareFromIndices(rowIndex, colIndex);
  
    if (this.selectedSquare) {
      // Make the move when the second square is clicked
      this.makeMove(this.selectedSquare, square);
      this.selectedSquare = null; // Deselect after the move
    } else {
      // Select the square when clicked
      this.selectedSquare = square;
    }
  }

  // Make a move by sending the move request to the backend
  makeMove(from: string, to: string) {
    // Call backend service to move the piece
    this.chessService.movePiece(from, to).subscribe(
      (response: any) => {
        this.selectedSquare = null;
        this.invalidMoveMessage = null;

        // Update the game state after the move
        this.chessService.updateGame(this.currentGame).subscribe();
      },
      (error) => {
        this.invalidMoveMessage = 'Invalid move!';
      }
    );
  }

  // Start a new game
  startNewGame() {
    this.chessService.startNewGame().subscribe((newGame: any) => {
      this.boardState = []; // Reset the board state
      this.selectedSquare = null;
      this.invalidMoveMessage = null;
      this.currentGame = newGame; // Set the current game to the newly started game
    });
  }

  // Reset the game state
  resetGame() {
    this.chessService.resetGame().subscribe(
      (response) => {
        this.boardState = [];
        this.selectedSquare = null;
        this.invalidMoveMessage = null;
        this.currentGame = response; // Update current game with reset state
      },
      (error) => {
        console.error('Error resetting game:', error);
        this.invalidMoveMessage = 'Failed to reset the game!';
      }
    );
  }

  // Save the current game state
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

  // Load all previously saved games
  loadGames() {
    this.chessService.getAllGames().subscribe((games: any) => {
      this.games = games;
    });
  }
}
