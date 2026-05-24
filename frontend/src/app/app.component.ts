import { Component, signal } from '@angular/core';
import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, RouterLink, RouterLinkActive],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
})
export class AppComponent {
  title = 'Warehouse Management System';
  isMenuOpen = signal(false);

  toggleMenu(): void {
    this.isMenuOpen.update(state => !state);
  }

  closeMenu(): void {
    this.isMenuOpen.set(false);
  }
}
