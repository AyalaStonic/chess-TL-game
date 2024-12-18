import { Component, OnInit } from '@angular/core';
import { ChessService } from '../services/chess.service';
import { HttpClient } from '@angular/common/http';
import { CommonModule } from '@angular/common';  // Import CommonModule

@Component({
  selector: 'app-chessboard',
  standalone: true,
  templateUrl: './chessboard.component.html',
  styleUrls: ['./chessboard.component.css'],
  providers: [ChessService, HttpClient],
  imports: [CommonModule]  // Add CommonModule here to resolve the *ngFor, *ngIf, ngClass, and uppercase pipe
})
export class ChessboardComponent {
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
    // Logic for making a move
    this.chessService.movePiece(from, to).subscribe(
      (response: any) => {
        this.selectedSquare = null;
        this.invalidMoveMessage = null;
        this.chessService.updateGame(this.currentGame).subscribe();
      },
      (error) => {
        this.invalidMoveMessage = "Invalid move!";
      }
    );
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

  // Define the loadGames method
  loadGames() {
    this.chessService.getAllGames().subscribe((games: any) => {
      this.games = games;
    });
  }
}