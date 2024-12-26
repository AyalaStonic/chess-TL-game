import { Component, OnInit } from '@angular/core';
import { ChessService } from '../chess.service';
import { HttpClient } from '@angular/common/http';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpErrorResponse } from '@angular/common/http';

@Component({
  selector: 'app-chessboard',
  standalone: true,
  templateUrl: './chessboard.component.html',
  styleUrls: ['./chessboard.component.css'],
  providers: [ChessService, HttpClient],
  imports: [CommonModule, FormsModule],
})
export class ChessboardComponent implements OnInit {
  boardState: string[][] = [];
  selectedSquare: string | null = null;
  invalidMoveMessage: string | null = null;
  currentGame: any = null;
  games: any[] = [];
  gameId: number | null = null;
  username: string = '';
  currentUser: any = null;

  constructor(private chessService: ChessService, private http: HttpClient) {}

  ngOnInit() {
    this.initializeBoardState();
    this.loadGames();
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
      ['r', 'n', 'b', 'q', 'k', 'b', 'n', 'r'],
    ];
  }

  createUser(): void {
    if (!this.username.trim()) {
      alert('Please enter a valid username.');
      return;
    }

    const user = { username: this.username.trim() };
    this.chessService.createUser(user).subscribe(
      (createdUser) => {
        this.currentUser = createdUser;
        alert(`User "${createdUser.username}" created successfully!`);
        this.loadGames();
      },
      (error: HttpErrorResponse) => {
        if (error.status === 409) {
          alert('Username already exists. Please try another.');
        } else {
          alert('Failed to create user. Please try again.');
        }
      }
    );
  }

  loginUser(): void {
    if (!this.username || this.username.trim().length === 0) {
      alert('Please enter a valid username.');
      return;
    }

    const user = { username: this.username.trim() };

    this.chessService.loginUser(user).subscribe(
      (response: any) => {
        if (response.user && response.user.id) {
          this.currentUser = response.user;
          alert(`Welcome back, ${this.currentUser.username}!`);
          this.loadGames();
        } else {
          alert('Login failed. Invalid user data received.');
        }
      },
      (error: HttpErrorResponse) => {
        if (error.status === 404) {
          alert('User not found. Please check the username or create a new account.');
        } else {
          alert('Failed to log in. Please try again.');
        }
      }
    );
  }

  loadGames(): void {
    if (this.currentUser && this.currentUser.id) {
      this.chessService.getGamesByUserId(this.currentUser.id).subscribe(
        (games: any[] | null | undefined) => {
          this.games = Array.isArray(games) ? games : [];
          if (this.games.length > 0) {
            this.currentGame = this.games[0];
            this.gameId = this.currentGame.id;
            this.loadGameState();
          } else {
            this.currentGame = null;
            this.gameId = null;
          }
        },
        (error: HttpErrorResponse) => {
          this.games = [];
        }
      );
    } else {
      this.games = [];
    }
  }

  loadGameState() {
    if (this.gameId !== null) {
      this.chessService.getGameById(this.gameId).subscribe(
        (game) => {
          this.currentGame = game;
          this.boardState = this.convertFENToBoard(game.fen);
        },
        (error: HttpErrorResponse) => {}
      );
    }
  }

  replayGame() {
    if (this.currentGame && this.currentGame.moves && Array.isArray(this.currentGame.moves)) {
      this.initializeBoardState();
      this.invalidMoveMessage = null;
      this.selectedSquare = null;

      this.currentGame.moves.forEach((move: any, index: number) => {
        setTimeout(() => {
          this.makeMove(move.from, move.to);
        }, index * 1000);
      });
    } else {
      alert('No valid game moves to replay!');
    }
  }

  getSquareClass(rowIndex: number, colIndex: number): string {
    const isLightSquare = (rowIndex + colIndex) % 2 === 0;
    return isLightSquare ? 'light' : 'dark';
  }

  convertFENToBoard(fen: string): string[][] {
    const rows = fen.split(' ')[0].split('/');
    return rows.map((row) => {
      const boardRow: string[] = [];
      for (const char of row) {
        if (isNaN(Number(char))) {
          boardRow.push(char);
        } else {
          for (let i = 0; i < Number(char); i++) {
            boardRow.push('');
          }
        }
      }
      return boardRow;
    });
  }

  onSquareClick(rowIndex: number, colIndex: number) {
    const square = this.getSquareFromIndices(rowIndex, colIndex);

    if (this.selectedSquare) {
      this.makeMove(this.selectedSquare, square);
      this.selectedSquare = null;
    } else {
      this.selectedSquare = square;
    }
  }

  getSquareFromIndices(rowIndex: number, colIndex: number): string {
    return `${String.fromCharCode(97 + colIndex)}${8 - rowIndex}`;
  }

  makeMove(from: string, to: string) {
    if (this.gameId === null) return;
  
    this.chessService.makeMove(this.gameId, from, to).subscribe(
      (response) => {
        this.invalidMoveMessage = null;
        this.loadGameState();
      },
      () => {
        this.invalidMoveMessage = 'Invalid move!';
      }
    );
  }
  

  startNewGame() {
    if (this.currentUser) {
      this.chessService.startNewGame(this.currentUser.id).subscribe(
        (newGame) => {
          this.initializeBoardState();
          this.currentGame = newGame;
          this.gameId = newGame.id;
        },
        (error) => console.error('Error starting a new game:', error)
      );
    }
  }

  resetGame() {
    if (this.gameId !== null) {
      this.chessService.resetGame(this.gameId).subscribe(
        () => this.initializeBoardState(),
        (error) => console.error('Error resetting game:', error)
      );
    }
  }

  saveGame() {
    if (this.currentGame) {
      this.chessService.saveGame(this.currentGame).subscribe(
        () => alert('Game saved successfully.'),
        (error) => console.error('Error saving game:', error)
      );
    }
  }

  getPieceImage(piece: string): string {
    const pieceMap: { [key: string]: string } = {
      K: 'white_king',
      Q: 'white_queen',
      R: 'white_rook',
      B: 'white_bishop',
      N: 'white_knight',
      P: 'white_pawn',
      k: 'black_king',
      q: 'black_queen',
      r: 'black_rook',
      b: 'black_bishop',
      n: 'black_knight',
      p: 'black_pawn',
    };
    return piece ? `./chess-pieces/${pieceMap[piece]}.png` : '';
  }

  logout() {
    this.currentUser = null;
    this.games = [];
    this.currentGame = null;
    this.gameId = null;
  }
}
