import { bootstrapApplication } from '@angular/platform-browser';
import { AppComponent } from './app/app.component';  // Import AppComponent

bootstrapApplication(AppComponent)  // Bootstrap the root AppComponent
  .catch(err => console.error('Bootstrap error:', err));  // Error handling