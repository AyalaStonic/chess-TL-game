import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ChessService {
  private apiUrl = 'http://localhost:5000/api/gamemoves'; // Your API URL for moves
  private gameApiUrl = 'http://localhost:5000/api/games'; // Your API URL for games

  constructor(private http: HttpClient) {}

  // Save a move to the database
  saveMove(move: any): Observable<any> {
    return this.http.post(this.apiUrl, move);
  }

  // Get all moves for a specific game
  getGameMoves(gameId: number): Observable<any[]> {
    return this.http.get<any[]>(`${this.gameApiUrl}/${gameId}/moves`);
  }

  // Get all saved games (if you want to allow loading games)
  getAllGames(): Observable<any[]> {
    return this.http.get<any[]>(this.gameApiUrl);
  }

  // Save a game (if you're linking to users)
  saveGame(game: any): Observable<any> {
    return this.http.post(this.gameApiUrl, game);
  }
}
