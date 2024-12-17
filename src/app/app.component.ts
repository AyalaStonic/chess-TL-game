import { Component } from '@angular/core';
import { ChessboardComponent } from './chessboard/chessboard.component';  // Correct the import path here

@Component({
  selector: 'app-root',
  standalone: true,  // Ensure this is set to true if it's a standalone component
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css'],
  imports: [ChessboardComponent],  // Add the ChessboardComponent here
})
export class AppComponent {
  title = 'chess-game';
}
