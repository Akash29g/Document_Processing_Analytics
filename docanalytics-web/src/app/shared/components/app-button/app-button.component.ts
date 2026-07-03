import { Component, input, output } from '@angular/core';

@Component({
  selector: 'app-button',
  standalone: true,
  templateUrl: './app-button.component.html',
  styleUrl: './app-button.component.css',
})
export class AppButtonComponent {
  loading = input(false);
  disabled = input(false);
  clicked = output<void>();
}
