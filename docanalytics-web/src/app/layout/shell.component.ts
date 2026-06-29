import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-shell',
  standalone: true,
  imports: [RouterOutlet],
  template: `
    <header style="padding:8px;border-bottom:1px solid #ccc"><strong>DocAnalytics</strong></header>
    <main style="padding:16px"><router-outlet /></main>
  `,
})
export class ShellComponent { }
