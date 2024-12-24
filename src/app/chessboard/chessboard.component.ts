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
    if (!this.username.trim()) {
      alert('Please enter a valid username.');
      return;
    }

    const user = { username: this.username.trim() };
    this.chessService.loginUser(user).subscribe(
      (existingUser) => {
        this.currentUser = existingUser;
        alert(`Welcome back, ${existingUser.username}!`);
        this.loadGames();
      },
      (error: HttpErrorResponse) => {
        if (error.status === 404) {
          alert('User not found. Please create a new account.');
        } else {
          alert('Failed to log in. Please try again.');
        }
      }
    );
  }

  loadGames() {
    if (this.currentUser) {
      this.chessService.getGamesByUserId(this.currentUser.id).subscribe(
        (games) => {
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
          console.error('Error fetching games:', error);
          this.games = [];
        }
      );
    }
  }

  loadGameState() {
    if (this.gameId !== null) {
      this.chessService.getGameById(this.gameId).subscribe(
        (game) => {
          this.currentGame = game;
          this.boardState = this.convertFENToBoard(game.fen);
        },
        (error: HttpErrorResponse) => {
          console.error('Error loading game state:', error);
        }
      );
    }
  }

  // Replay the game from the beginning
replayGame() {
  if (this.currentGame && this.currentGame.moves && Array.isArray(this.currentGame.moves)) {
    // Reset the board to the initial state
    this.initializeBoardState();
    this.invalidMoveMessage = null;
    this.selectedSquare = null;

    // Replay each move sequentially with a delay
    this.currentGame.moves.forEach((move: any, index: number) => {
      setTimeout(() => {
        this.makeMove(move.from, move.to);
      }, index * 1000); // Adjust delay as needed (e.g., 1 second per move)
    });
  } else {
    console.error('No valid game moves to replay!');
  }
}

// Get the CSS class for each square based on its position
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
      () => {
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
        () => console.log('Game saved successfully.'),
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
