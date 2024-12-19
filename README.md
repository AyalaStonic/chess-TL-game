# ChessTlGame

Frontend Angular
```bash
ng serve
```
Backend with C#

```bash
dotnet build
dotnet run
```
1. GET Request (Get All Games)

URL: http://localhost:5000/api/chess/games
Method: GET

2. POST Request (Create a New Game)

URL: http://localhost:5000/api/chess/games
Method: POST

{
  "name": "New Game",
  "status": "In Progress",
  "moves": []
}

3. POST Request (Start a New Game)

URL: http://localhost:5000/api/chess/games/start
Method: POST

{}

4. POST Request (Move a Piece)

URL: http://localhost:5000/api/chess/games/{gameId}/moves
Method: POST

"e2 to e4"


start chrome --disable-web-security --user-data-dir="C:\chrome_dev"