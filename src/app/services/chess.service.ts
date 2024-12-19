import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable()
export class ChessService {
  private apiUrl = 'http://localhost:5000/api/chess'; // Replace with your actual API URL

  constructor(private http: HttpClient) {}

  getAllGames(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/games`);
  }

  updateGame(game: any): Observable<any> {
    return this.http.put<any>(`${this.apiUrl}/games/${game.id}`, game);
  }

  movePiece(from: string, to: string): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/move`, { from, to });
  }

  resetGame(): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/reset`, {});
  }

  saveGame(game: any): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/games`, game);
  }

  // Add the startNewGame method to create a new game
  startNewGame(): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/games/start`, {}); // Sending an empty body to start a new game
  }
}
