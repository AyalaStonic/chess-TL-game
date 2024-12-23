import { Injectable } from '@angular/core'; 
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ChessService {

  private apiUrl = 'http://localhost:5000/api/chess';  // Your backend API URL

  constructor(private http: HttpClient) { }

  // Get all games from the server
  getAllGames(userId: number): Observable<any> {
    return this.http.get(`${this.apiUrl}/games/${userId}`);  // Fetch games by userId
  }

  createUser(user: { username: string }): Observable<any> {
    return this.http.post(`${this.apiUrl}/user`, user);
  }
  
  // Get game by ID
  getGameById(gameId: number): Observable<any> {
    return this.http.get(`${this.apiUrl}/games/${gameId}`);  // Fetch game details by ID
  }

  getGamesByUserId(userId: number): Observable<any[]> {
    return this.http.get<any[]>(`http://localhost:5000/api/chess/user/${userId}`);
  }

  // Create a new game
  createGame(game: any): Observable<any> {
    return this.http.post(`${this.apiUrl}/create`, game);
  }

  // Start a new game (reset the board to the initial state)
  startNewGame(userId: number): Observable<any> {
    return this.http.post(`${this.apiUrl}/start/${userId}`, {});  // Start a new game for a specific user
  }

  // Reset the current game to its initial state
  resetGame(gameId: number): Observable<any> {
    return this.http.post(`${this.apiUrl}/reset/${gameId}`, {});
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
  movePiece(gameId: number, move: { from: string, to: string }): Observable<any> {
    const body = {
      gameId: gameId,  // Sending gameId as part of the body
      move: move       // Move should be sent as an object with 'from' and 'to' properties
    };

    const headers = new HttpHeaders({
      'Content-Type': 'application/json',  // Ensure the request is sent as JSON
    });

    return this.http.post(`${this.apiUrl}/move`, body, { headers });  // Sending the body with move data
  }

  // Get all moves for a specific game
  getMoves(gameId: number): Observable<any> {
    return this.http.get(`${this.apiUrl}/moves/${gameId}`);
  }

  // Mark a game as completed (when the game ends)
  completeGame(gameId: number): Observable<any> {
    return this.http.post(`${this.apiUrl}/complete/${gameId}`, {});
  }
}
