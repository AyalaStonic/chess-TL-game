// src/app/app.component.ts
import { Component } from '@angular/core';
import { ChessboardComponent } from './chessboard/chessboard.component';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css'],
  standalone: true,  // Make this component standalone
  imports: [ChessboardComponent]  // Import ChessboardComponent directly here
})
export class AppComponent {
  title = 'chess-game';
}