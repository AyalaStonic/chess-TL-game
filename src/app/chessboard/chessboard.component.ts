import { Component, OnInit } from '@angular/core';
import { ChessService } from '../services/chess.service';
import { HttpClient } from '@angular/common/http';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-chessboard',
  standalone: true,
  templateUrl: './chessboard.component.html',
  styleUrls: ['./chessboard.component.css'],
  providers: [ChessService, HttpClient],
  imports: [CommonModule]
})
export class ChessboardComponent implements OnInit {
  boardState: string[][] = [];
  selectedSquare: string | null = null;
  invalidMoveMessage: string | null = null;
  currentGame: any;
  games: any[] = [];

  constructor(private chessService: ChessService, private http: HttpClient) {}

  ngOnInit() {
    this.chessService.getAllGames().subscribe((games: any) => {
      this.games = games;
    });
  }

  getSquareClass(rowIndex: number, colIndex: number): string {
    return (rowIndex + colIndex) % 2 === 0 ? 'white' : 'black';
  }

  getSquareFromIndices(rowIndex: number, colIndex: number): string {
    return `${String.fromCharCode(65 + colIndex)}${8 - rowIndex}`;
  }

  onSquareClick(rowIndex: number, colIndex: number) {
    const square = this.getSquareFromIndices(rowIndex, colIndex);
    if (this.selectedSquare) {
      this.makeMove(this.selectedSquare, square);
    } else {
      this.selectedSquare = square;
    }
  }

  makeMove(from: string, to: string) {
    this.chessService.movePiece(from, to).subscribe(
      (response: any) => {
        this.selectedSquare = null;
        this.invalidMoveMessage = null;
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

  resetGame() {
    this.chessService.resetGame().subscribe(() => {
      this.boardState = [];
      this.selectedSquare = null;
      this.invalidMoveMessage = null;
    });
  }

  saveGame() {
    this.chessService.saveGame(this.currentGame).subscribe();
  }

  loadGames() {
    this.chessService.getAllGames().subscribe((games: any) => {
      this.games = games;
    });
  }
}
