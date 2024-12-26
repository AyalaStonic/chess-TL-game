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
    this.boardState = this.initializeBoardState();  // Ensure board state is initialized
    this.loadGames();  // Load games if necessary
  }

  initializeBoardState(): string[][] {
    // Define the default FEN
    const defaultFEN = 'rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1';

    // Convert the default FEN to a board state
    return this.convertFENToBoard(defaultFEN);
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
          if (game && game.fen) {
            // Update the board with the current FEN string
            this.boardState = this.convertFENToBoard(game.fen);
          } else {
            // Fallback if the FEN string is missing
            console.error('FEN string missing in game data');
            this.boardState = this.initializeBoardState();  // Fallback to default state
          }
        },
        (error: HttpErrorResponse) => {
          console.error('Error loading game state:', error);
        }
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
    if (!fen || typeof fen !== 'string') {
      console.error('Invalid FEN string:', fen);
      return this.initializeBoardState();  // Fallback to default board state
    }
  
    const rows = fen.split(' ')[0].split('/');
    return rows.map((row) => {
      const boardRow: string[] = [];
      for (const char of row) {
        if (isNaN(Number(char))) {
          // Add the piece if it's a letter (e.g., 'r', 'K', 'p')
          boardRow.push(char);
        } else {
          // If the character is a number, it represents empty squares
          boardRow.push(...Array(Number(char)).fill(''));
        }
      }
      return boardRow;
    });
  }
  

  // Handles a click on a square on the chessboard
  onSquareClick(rowIndex: number, colIndex: number) {
    const square = this.getSquareFromIndices(rowIndex, colIndex);  // Get square notation (e.g., "a1")

    if (this.selectedSquare) {
      // If a square is already selected, attempt to make a move
      this.makeMove(this.selectedSquare, square);
      this.selectedSquare = null; // Deselect after move
    } else {
      // Otherwise, select the square
      this.selectedSquare = square;
    }
  }

  // Converts row and column indices to chess notation (e.g., row 0, col 0 => "a8")
  getSquareFromIndices(rowIndex: number, colIndex: number): string {
    // Convert column index to letter (0 => 'a', 1 => 'b', etc.)
    return `${String.fromCharCode(97 + colIndex)}${8 - rowIndex}`;
  }

  makeMove(from: string, to: string) {
    if (this.gameId === null) return;
  
    // Ensure the piece exists on the source square before making the move
    const piece = this.getPieceFromSquare(from);
    if (!piece) {
      this.invalidMoveMessage = 'Source piece does not exist.';
      console.error('Error: Source piece does not exist.');
      return;
    }
  
    // Proceed with the move, send to the backend for validation
    this.chessService.makeMove(this.gameId, from, to).subscribe(
      (response) => {
        // Update the board state after a valid move
        this.invalidMoveMessage = null;
        if (response && response.fen) {
          // Update the board with the new FEN string returned by the backend
          this.boardState = this.convertFENToBoard(response.fen);
        } else {
          console.error('Invalid response from backend. FEN missing.');
          this.invalidMoveMessage = 'Error updating game state.';
        }
      },
      (error) => {
        this.invalidMoveMessage = 'Invalid move!';
        console.error('Error making move:', error);
      }
    );
  }
  
  

  getPieceFromSquare(square: string): string | null {
    const colIndex = square.charCodeAt(0) - 97; // Convert column (e.g., 'a') to index
    const rowIndex = 8 - parseInt(square[1], 10); // Convert row (e.g., '1') to index
    return this.boardState[rowIndex][colIndex];
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

    // Check if the piece exists in the map, and return the correct image path or an empty string
    return piece && pieceMap[piece] ? `/chess-pieces/${pieceMap[piece]}.png` : '';
  }

  logout() {
    this.currentUser = null;
    this.games = [];
    this.currentGame = null;
    this.gameId = null;
  }
}
