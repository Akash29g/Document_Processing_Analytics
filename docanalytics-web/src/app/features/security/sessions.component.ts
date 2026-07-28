import { ChangeDetectionStrategy, Component, inject, OnInit } from '@angular/core';
import { DatePipe, Location } from '@angular/common';
import { RouterLink } from '@angular/router';
import { SessionsService } from './sessions.service';

@Component({
  selector: 'app-sessions',
  standalone: true,
  imports: [DatePipe, RouterLink],
  templateUrl: './sessions.component.html',
  styleUrl: './sessions.component.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class SessionsComponent implements OnInit {
  protected sessions = inject(SessionsService);

  private location = inject(Location);

  ngOnInit(): void {
    this.sessions.load();
  }

  protected revoke(id: string): void {
    if (confirm('Log out this device?')) this.sessions.revoke(id);
  }

  protected revokeOthers(): void {
    if (confirm('Log out all other devices? This device will stay signed in.')) {
      this.sessions.revokeAllOthers();
    }
  }

  goBack(): void {
    this.location.back();
  }
}
