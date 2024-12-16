import { Component, OnInit } from '@angular/core';
import { Chess } from 'chess.js';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-chessboard',
  templateUrl: './chessboard.component.html',
  styleUrls: ['./chessboard.component.css'],
  standalone: true,
  imports: [CommonModule]
})
export class ChessboardComponent implements OnInit {
  board: any;
  game: Chess;
  boardState: any[]; 
  selectedSquare: string | null = null;  // Track selected square
  invalidMoveMessage: string | null = null;  // Store invalid move message

  constructor() {
    this.game = new Chess();
    this.boardState = [];
  }

  ngOnInit(): void {
    this.updateBoard(); // Ensure the board is updated when the component loads
  }

  // Update the board state from the game object
  updateBoard() {
    const board = this.game.board();
    this.boardState = board.map((row: any) =>
      row.map((piece: any) => (piece ? piece.type : null))  // Only show piece type (e.g., p, r, n, etc.)
    );
  }

  // Handle making a move
  makeMove(from: string, to: string) {
    const move = this.game.move({ from, to });

    // Handle invalid moves
    if (move === null) {
      this.invalidMoveMessage = 'Invalid move! Try again.';
      console.log('Invalid move');
      return;
    }

    this.invalidMoveMessage = null; // Clear invalid move message
    this.updateBoard(); // Update the board after a valid move
    console.log(this.game.fen());
  }

  // Reset the game
  resetGame() {
    this.game.reset();          // Reset the chess game state
    this.updateBoard();         // Update the board after reset
    this.selectedSquare = null; // Clear the selected square
    this.invalidMoveMessage = null;  // Clear any invalid move message
  }

  // Get the square class for alternating colors
  getSquareClass(rowIndex: number, colIndex: number): string {
    const isDarkSquare = (rowIndex + colIndex) % 2 === 1;
    return isDarkSquare ? 'dark' : 'light';  // Return different class based on square color
  }

  // Handle square click logic
  onSquareClick(rowIndex: number, colIndex: number) {
    const square: string = this.getSquareFromIndices(rowIndex, colIndex);

    // If the player clicks on the same square again (i.e., deselect the piece)
    if (this.selectedSquare === square) {
      this.selectedSquare = null;  // Deselect the square
      this.invalidMoveMessage = null;  // Clear any previous invalid move message
      return;
    }

    // If a square is already selected, try to make a move
    if (this.selectedSquare) {
      const move = this.game.move({
        from: this.selectedSquare,
        to: square,
      });

      // Check if the move is invalid
      if (move === null) {
        this.invalidMoveMessage = 'Invalid move! Try again.';
        console.log('Invalid move from', this.selectedSquare, 'to', square);
        
        // Do not update the board or clear the selected square on invalid move
        return;
      }

      // If move is valid, update the board
      this.invalidMoveMessage = null; // Clear invalid move message
      this.updateBoard(); // Update the board after the valid move
      this.selectedSquare = null; // Deselect the square after making the move
    } else {
      // If there's no selected square, select the current square
      const piece = this.game.get(square as any);

      if (piece) {
        this.selectedSquare = square; // Select the square if it contains a piece
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
