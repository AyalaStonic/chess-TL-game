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

Example in Postman:
URL: http://localhost:5000/api/chess/games
Method: POST
Headers: Content-Type: application/json
Body:
json

{
  "name": "New Game",
  "status": "In Progress",
  "moves": []
}

URL: http://localhost:5000/api/chess/games/{gameId}/moves
Same but
"e2 to e4"
