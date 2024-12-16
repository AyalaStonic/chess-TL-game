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
    if (move === null) {
      console.log('Invalid move');
      return;
    }
    this.updateBoard();
    console.log(this.game.fen());
  }

  // Reset the game, including clearing the selected piece
  resetGame() {
    this.game.reset();  // Reset the game state
    this.selectedSquare = null;  // Clear the selected square
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

      if (move === null) {
        console.log('Invalid move');
      } else {
        this.updateBoard(); // Refresh the board after a successful move
      }

      this.selectedSquare = null; // Deselect after making the move
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
