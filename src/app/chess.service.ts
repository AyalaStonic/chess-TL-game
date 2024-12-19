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

  // Move a piece on the board (this will be the most important part)
  movePiece(from: string, to: string): Observable<any> {
    return this.http.post(`${this.apiUrl}/move`, { from, to });
  }

}
