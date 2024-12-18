import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ChessService } from '../services/chess.service';  // Import ChessService
import { HttpClientModule } from '@angular/common/http';   // Import HttpClientModule
import { Chess } from 'chess.js'; // Import Chess.js for game logic

@Component({
  selector: 'app-chessboard',
  standalone: true,
  templateUrl: './chessboard.component.html',
  styleUrls: ['./chessboard.component.css'],
  imports: [CommonModule, HttpClientModule],  // Add HttpClientModule here
})
export class ChessboardComponent implements OnInit {
  boardState: any[] = [];
  game: Chess;
  selectedSquare: string | null = null;  // Allow null value
  invalidMoveMessage: string | null = null;

  constructor(private chessService: ChessService) {
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

  // Example method to call backend for creating a new game
  createGame() {
    const gameData = { moves: ['e2e4', 'e7e5'] };  // Sample moves
    this.chessService.createGame(gameData).subscribe(response => {
      console.log('Game created:', response);
    });
  }

  // Example method to fetch and replay moves for a specific game
  replayGame(gameId: number) {
    this.chessService.getGameMoves(gameId).subscribe(moves => {
      console.log('Replaying game moves:', moves);
      this.game.reset(); // Reset game state
      moves.forEach(move => {
        this.game.move(move); // Apply each move
      });
      this.updateBoard();
    });
  }
}
