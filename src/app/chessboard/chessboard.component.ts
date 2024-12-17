import { Component, OnInit } from '@angular/core';
import { Chess } from 'chess.js';
import { ChessService } from '../chess.service'; // Import ChessService

@Component({
  selector: 'app-chessboard',
  standalone: true,  // Mark this component as standalone
  templateUrl: './chessboard.component.html',
  styleUrls: ['./chessboard.component.css'],
})
export class ChessboardComponent implements OnInit {
  boardState: any[] = [];
  game: Chess;
  selectedSquare: string | null = null;
  invalidMoveMessage: string | null = null;
  gameId: number = 1; // Placeholder game ID, can be set dynamically
  moves: any[] = [];  // Array to hold game moves

  constructor(private chessService: ChessService) {
    this.game = new Chess();
  }

  ngOnInit(): void {
    this.updateBoard();
    this.loadGameMoves();
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
    this.saveMoveToDatabase(from, to, move.piece);
  }

  saveMoveToDatabase(from: string, to: string, piece: string) {
    const move = {
      gameId: this.gameId,
      fromSquare: from,
      toSquare: to,
      pieceType: piece,
    };

    this.chessService.saveMove(move).subscribe({
      next: (response) => {
        console.log('Move saved:', response);
      },
      error: (err) => {
        console.error('Error saving move:', err);
      },
    });
  }

  loadGameMoves() {
    this.chessService.getGameMoves(this.gameId).subscribe({
      next: (moves) => {
        this.moves = moves;
        this.replayGame();
      },
      error: (err) => {
        console.error('Error loading moves:', err);
      },
    });
  }

  replayGame() {
    this.moves.forEach((move, index) => {
      setTimeout(() => {
        this.game.move({
          from: move.fromSquare,
          to: move.toSquare,
        });
        this.updateBoard();
      }, 1000 * index); // Delay between moves
    });
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
      this.saveMoveToDatabase(this.selectedSquare, square, move.piece);
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
