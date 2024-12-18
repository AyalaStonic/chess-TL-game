import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class ChessService {

  private apiUrl = 'http://localhost:5000/api/chess/games';  // Your backend API URL

  constructor(private http: HttpClient) { }

  // Example method to get all games
  getAllGames() {
    return this.http.get(this.apiUrl + '/GetAllGames');
  }

  // Example method to create a new game
  createGame(game: any) {
    return this.http.post(this.apiUrl + '/CreateGame', game);
  }
}
