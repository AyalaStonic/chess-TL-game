import { Component, OnInit } from '@angular/core';
import { Chess } from 'chess.js';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-chessboard',
  templateUrl: './chessboard.component.html',
  styleUrls: ['./chessboard.component.css'],
  standalone: true,
  imports: [CommonModule]  // Include CommonModule for Angular directives like *ngFor, *ngIf
})
export class ChessboardComponent implements OnInit {
  board: any;
  game: Chess;
  boardState: any[];
  selectedSquare: string | null = null; // Track selected square
  isMoveValid: boolean = true; // Track the validity of the move

  constructor() {
    this.game = new Chess();
    this.boardState = [];
  }

  ngOnInit(): void {
    this.updateBoard();
  }

  updateBoard() {
    const board = this.game.board();
    this.boardState = board.map((row: any) =>
      row.map((piece: any) => (piece ? piece.type : null))
    );
  }

  makeMove(from: string, to: string) {
    const move = this.game.move({ from, to });
    
    // Check if the move is valid
    if (move === null) {
      this.isMoveValid = false;  // Move is invalid
      return false;  // Exit the function early
    }

    this.isMoveValid = true;  // Reset move validity
    this.updateBoard();  // Refresh the board after a successful move
    console.log(this.game.fen()); // Log the current game state
    return true;
  }

  // Reset the game, including clearing the selected piece
  resetGame() {
    this.game.reset();  // Reset the game state
    this.selectedSquare = null;  // Clear the selected square
    this.isMoveValid = true;  // Reset move validity
    this.updateBoard();  // Refresh the board after reset
  }

  // Get the square class for alternating colors
  getSquareClass(rowIndex: number, colIndex: number): string {
    const isDarkSquare = (rowIndex + colIndex) % 2 === 1;
    return isDarkSquare ? 'dark' : 'light';  // Return different class based on square color
  }

  // Handle square click logic
  onSquareClick(rowIndex: number, colIndex: number) {
    const square: string = this.getSquareFromIndices(rowIndex, colIndex);

    if (this.selectedSquare) {
      // If there's already a selected square, attempt to make the move
      const move = this.game.move({
        from: this.selectedSquare,
        to: square,
      });

      // Handle invalid move
      if (move === null) {
        this.isMoveValid = false; // Move is invalid
        console.log("Invalid move!");
      } else {
        // If the move is successful, update the board
        this.updateBoard();
        this.isMoveValid = true; // Reset move validity
      }

      // Clear the selected square after a move (whether valid or invalid)
      this.selectedSquare = null;
    } else {
      // If there's no selected square, select the current square
      const piece = this.game.get(square as any);  // Cast 'square' to any type to avoid TS error

      if (piece) {
        // If there's a piece on the selected square, select it
        this.selectedSquare = square;
      }
    }
  }

  // Convert row/column indices to chess notation (e.g., 'a1', 'b3', etc.)
  getSquareFromIndices(rowIndex: number, colIndex: number): string {
    const file = String.fromCharCode(97 + colIndex); // 'a' to 'h'
    const rank = 8 - rowIndex; // 8 to 1
    return file + rank;
  }
}
