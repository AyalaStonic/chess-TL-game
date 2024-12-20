import { bootstrapApplication } from '@angular/platform-browser';
import { provideHttpClient } from '@angular/common/http'; // Use the default HttpClient configuration
import { AppComponent } from './app/app.component';

bootstrapApplication(AppComponent, {
  providers: [
    provideHttpClient() // Default HttpClient without fetch
  ]
}).catch(err => console.error('Error bootstrapping the application:', err));
