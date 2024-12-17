import { Component } from '@angular/core';
import { ChessboardComponent } from './chessboard/chessboard.component'; // Import the chessboard component

@Component({
  selector: 'app-root',
  standalone: true,  // Mark this component as standalone
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css'],
  imports: [ChessboardComponent], // Import the ChessboardComponent here
})
export class AppComponent {
  title = 'chess-game';
}
