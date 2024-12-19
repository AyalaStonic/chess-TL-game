import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class ChessService {
  private apiUrl = 'http://localhost:5000/api/chess'; // Replace with your actual API URL

  constructor(private http: HttpClient) {}

  // Get all games
  getAllGames(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/games`);
  }

  // Update an existing game
  updateGame(game: any): Observable<any> {
    return this.http.put<any>(`${this.apiUrl}/games/${game.id}`, game);
  }

  // Move a piece
  movePiece(from: string, to: string): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/move`, { from, to });
  }

  // Reset the game
  resetGame(): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/reset`, {}); // Calls the backend to reset the game
  }

  // Save a game
  saveGame(game: any): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/games`, game); // Save game state to backend
  }

  // Start a new game
  startNewGame(): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/start`, {}); // Calls backend to start a new game
  }
}
