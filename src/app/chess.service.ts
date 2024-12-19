import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ChessService {

  private apiUrl = 'http://localhost:5000/api/chess';  // Your backend API URL

  constructor(private http: HttpClient) { }

  // Get all games from the server
  getAllGames(): Observable<any> {
    return this.http.get(`${this.apiUrl}/games`);
  }

  // Get game by ID
  getGameById(gameId: number): Observable<any> {
    return this.http.get(`${this.apiUrl}/games/${gameId}`);  // Fetch game details by ID
  }

  // Create a new game
  createGame(game: any): Observable<any> {
    return this.http.post(`${this.apiUrl}/create`, game);
  }

  // Start a new game (reset the board to the initial state)
  startNewGame(): Observable<any> {
    return this.http.post(`${this.apiUrl}/start`, {});
  }

  // Reset the current game to its initial state
  resetGame(): Observable<any> {
    return this.http.post(`${this.apiUrl}/reset`, {});
  }

  // Save the current game state to the backend
  saveGame(game: any): Observable<any> {
    return this.http.post(`${this.apiUrl}/save`, game);
  }

  // Update the game with the latest state
  updateGame(game: any): Observable<any> {
    return this.http.put(`${this.apiUrl}/update`, game);
  }

  // Make a move in the game
  movePiece(gameId: number, move: string) {
    return this.http.post(`http://localhost:5000/api/chess/move?gameId=${gameId}`, move);
  }

  // Alternative way to send a move (with gameId as query parameter)
  makeMove(gameId: number, move: string): Observable<any> {
    const url = `${this.apiUrl}/move?gameId=${gameId}`; // Include gameId as query parameter
    return this.http.post(url, move); // Send the move in the request body
  }
}
