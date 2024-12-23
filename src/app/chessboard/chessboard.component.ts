import { Component, OnInit } from '@angular/core';
import { ChessService } from '../chess.service';
import { HttpClient } from '@angular/common/http';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';  // Import FormsModule for ngModel
import { HttpErrorResponse } from '@angular/common/http'; // Import for error handling

@Component({
  selector: 'app-chessboard',
  standalone: true,
  templateUrl: './chessboard.component.html',
  styleUrls: ['./chessboard.component.css'],
  providers: [ChessService, HttpClient],
  imports: [CommonModule, FormsModule],  // Add FormsModule here
})
export class ChessboardComponent implements OnInit {
  boardState: string[][] = [];
  selectedSquare: string | null = null;
  invalidMoveMessage: string | null = null;
  currentGame: any = null;
  games: any[] = [];
  gameId: number | null = null;
  username: string = '';
  currentUser: any = null;  // User object to manage current user

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

  createUser() {
    if (this.username) {
      const user = { username: this.username };

      this.chessService.createUser(user).subscribe(
        (user: any) => {
          this.currentUser = user;
          console.log('User created successfully:', user);
          this.loadGames();  // If needed to load games for the user
        },
        (error) => {
          console.error('Error creating user:', error);  // Log actual error
          alert('Failed to create user');
        }
      );
    } else {
      alert('Please enter a username');
    }
  }

  // Fetch all games from the backend for the current user
  loadGames() {
    if (this.currentUser) {
      this.chessService.getGamesByUserId(this.currentUser.id).subscribe(
        (games: any[] | null | undefined) => {
          // Ensure games is an array
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
          this.games = []; // Reset games in case of error
        }
      );
    } else {
      this.games = []; // Ensure games is reset when no user is logged in
      console.warn('No current user found. Cannot load games.');
    }
  }

  // Load the current game's state from the backend
  loadGameState() {
    if (this.gameId !== null) {
      this.chessService.getGameById(this.gameId).subscribe(
        (game: any) => {
          this.currentGame = game;
          this.boardState = this.convertFENToBoard(game.fen);
        },
        (error: HttpErrorResponse) => {
          console.error('Error loading game state:', error);
        }
      );
    }
  }

  // Convert FEN string to a 2D array representing the board state
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

  // Handle a click on a square
  onSquareClick(rowIndex: number, colIndex: number) {
    const square = this.getSquareFromIndices(rowIndex, colIndex);

    if (this.selectedSquare) {
      this.makeMove(this.selectedSquare, square);
      this.selectedSquare = null;
    } else {
      this.selectedSquare = square;
    }
  }

  // Convert row and column indices to chess notation (e.g., A1)
  getSquareFromIndices(rowIndex: number, colIndex: number): string {
    return `${String.fromCharCode(97 + colIndex)}${8 - rowIndex}`;
  }

  // Perform a move and update the game state
  makeMove(from: string, to: string) {
    if (this.gameId === null) {
      console.error('Game ID is null. Cannot make a move.');
      return;
    }
  
    // Call the chess service with the gameId, and the 'from' and 'to' values
    this.chessService.makeMove(this.gameId, from, to).subscribe(
      (response: any) => {
        this.invalidMoveMessage = null;  // Clear any previous invalid move messages
        this.loadGameState();  // Reload the game state after the move
      },
      (error: HttpErrorResponse) => {
        this.invalidMoveMessage = 'Invalid move!';  // Set an error message for invalid moves
        console.error('Error making move:', error);
      }
    );
  }
  
  
  


  // Start a new game
  startNewGame() {
    if (this.currentUser) {
      this.chessService.startNewGame(this.currentUser.id).subscribe(
        (newGame: any) => {
          this.initializeBoardState();
          this.selectedSquare = null;
          this.invalidMoveMessage = null;
          this.currentGame = newGame;
          this.gameId = newGame.id;
        },
        (error: HttpErrorResponse) => {
          console.error('Error starting new game:', error);
          alert('Failed to start a new game. Please try again.');
        }
      );
    } else {
      alert('Please log in to start a new game.');
    }
  }

  // Reset the current game
  resetGame() {
    if (this.gameId !== null) {
      this.chessService.resetGame(this.gameId).subscribe(
        () => {
          this.initializeBoardState();
          this.selectedSquare = null;
          this.invalidMoveMessage = null;
        },
        (error: HttpErrorResponse) => {
          console.error('Error resetting game:', error);
        }
      );
    } else {
      console.error('No game selected to reset!');
    }
  }

  // Save the current game
  saveGame() {
    if (this.currentGame) {
      this.chessService.saveGame(this.currentGame).subscribe(
        (response) => {
          console.log('Game saved successfully:', response);
        },
        (error: HttpErrorResponse) => {
          console.error('Error saving game:', error);
        }
      );
    } else {
      console.error('No current game to save!');
    }
  }

  // Replay the game from the beginning
  replayGame() {
    if (this.currentGame) {
      const moves = this.currentGame.moves;
      this.initializeBoardState();
      this.invalidMoveMessage = null;
      this.selectedSquare = null;

      moves.forEach((move: any, index: number) => {
        setTimeout(() => {
          this.makeMove(move.from, move.to);
        }, index * 1000);
      });
    } else {
      console.error('No game selected to replay!');
    }
  }

  // Map the board state to piece images for display
  getPieceImage(piece: string): string {
    if (!piece) return '';
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
    return `./chess-pieces/${pieceMap[piece]}.png`;
  }

  // Get the class for each square
  getSquareClass(rowIndex: number, colIndex: number): string {
    const isLightSquare = (rowIndex + colIndex) % 2 === 0;
    return isLightSquare ? 'square light' : 'square dark';
  }

  // Logout the current user
  logout() {
    this.currentUser = null;
    this.games = [];
    this.currentGame = null;
    this.gameId = null;
    console.log('User logged out');
  }
}
