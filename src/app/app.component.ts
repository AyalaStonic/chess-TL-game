import { Component } from '@angular/core';
import { ChessboardComponent } from './chessboard/chessboard.component';  // Import ChessboardComponent
import { CommonModule } from '@angular/common';  // Import CommonModule
import { HttpClientModule } from '@angular/common/http';  // Import HttpClientModule if you are making HTTP requests

@Component({
  selector: 'app-root',
  standalone: true,  // This is for standalone components in Angular
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css'],
  imports: [
    CommonModule,  // Add CommonModule for the usage of Angular directives like *ngFor
    HttpClientModule,  // Import HttpClientModule for HTTP requests
    ChessboardComponent  // Add ChessboardComponent here
  ],
})
export class AppComponent {
  // Your component logic
}
