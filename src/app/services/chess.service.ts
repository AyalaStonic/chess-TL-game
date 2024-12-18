// src/app/services/chess.service.ts
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { catchError } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class ChessService {
  private apiUrl = 'http://localhost:5000/api/games';

  constructor(private http: HttpClient) { }

  createGame(game: { moves: string[] }): Observable<any> {
    return this.http.post(`${this.apiUrl}/CreateGame`, game)
      .pipe(
        catchError(error => {
          console.error('Error creating game:', error);
          throw error;
        })
      );
  }

  getGameMoves(gameId: number): Observable<string[]> {
    return this.http.get<string[]>(`${this.apiUrl}/ReplayGame/${gameId}`)
      .pipe(
        catchError(error => {
          console.error('Error fetching game moves:', error);
          throw error;
        })
      );
  }
}
