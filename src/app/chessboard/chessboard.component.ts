import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';  // Import CommonModule
import { Chess } from 'chess.js';
import { ChessService } from '../chess.service'; // Import ChessService


@Component({
  selector: 'app-chessboard',
  standalone: true,  
  templateUrl: './chessboard.component.html',
  styleUrls: ['./chessboard.component.css'],
  imports: [CommonModule],  // Add CommonModule to imports
})
export class ChessboardComponent {
  boardState: any[] = [];
  game: Chess;
  selectedSquare: string | null = null;  // Allow null value
  invalidMoveMessage: string | null = null;

  constructor() {
    this.game = new Chess();
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
      this.invalidMoveMessage = 'Invalid move! Try again.';
      return;
    }
    this.invalidMoveMessage = null;
    this.updateBoard();
  }

  resetGame() {
    this.game.reset();
    this.updateBoard();
    this.selectedSquare = null;
    this.invalidMoveMessage = null;
  }

  getSquareClass(rowIndex: number, colIndex: number): string {
    return (rowIndex + colIndex) % 2 === 0 ? 'light' : 'dark';
  }

  onSquareClick(rowIndex: number, colIndex: number) {
    const square = this.getSquareFromIndices(rowIndex, colIndex);

    if (this.selectedSquare === square) {
      this.selectedSquare = null;
      this.invalidMoveMessage = null;
      return;
    }

    if (this.selectedSquare) {
      const move = this.game.move({
        from: this.selectedSquare,
        to: square,
      });

      if (move === null) {
        this.invalidMoveMessage = 'Invalid move! Try again.';
        return;
      }

      this.invalidMoveMessage = null;
      this.updateBoard();
      this.selectedSquare = null;
    } else {
      const piece = this.game.get(square as any);
      if (piece) {
        this.selectedSquare = square;
      }
    }
  }

  getSquareFromIndices(rowIndex: number, colIndex: number): string {
    const file = String.fromCharCode(97 + colIndex); // 'a' to 'h'
    const rank = 8 - rowIndex; // 8 to 1
    return file + rank;
  }
}
