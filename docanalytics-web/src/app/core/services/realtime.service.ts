import { Injectable, effect, inject, signal } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { environment } from '../../../environments/environment';
import { SiteContextService } from './site-context.service';

const TOKEN_KEY = 'da_token';   // must match AuthService / interceptor

// Matches the backend FileStateChangedNotification (snake_case on the wire).
export interface FileStateChanged {
  file_id: string;
  file_name: string;
  old_state?: string | null;
  new_state: string;
  step: string;
  at: string;
}

@Injectable({ providedIn: 'root' })
export class RealtimeService {
  private readonly site = inject(SiteContextService);
  private conn?: signalR.HubConnection;
  private joinedSite: string | null = null;

  readonly connected = signal(false);
  readonly lastEvent = signal<FileStateChanged | null>(null);

  constructor() {
    // (re)join the correct site group whenever the selected site changes
    effect(() => {
      const siteId = this.site.selectedSiteId();
      if (siteId) void this.ensureJoined(siteId);
    });
  }

  async start(): Promise<void> {
    if (this.conn) return;

    this.conn = new signalR.HubConnectionBuilder()
      .withUrl(environment.hubUrl, {
        accessTokenFactory: () => localStorage.getItem(TOKEN_KEY) ?? '',
      })
      .withAutomaticReconnect()
      .build();

    // server pushes "FileStateChanged" → store the latest event
    this.conn.on('FileStateChanged', (payload: FileStateChanged) => {
      this.lastEvent.set(payload);
    });

    this.conn.onreconnected(() => { this.connected.set(true); void this.rejoin(); });
    this.conn.onclose(() => this.connected.set(false));

    try {
      await this.conn.start();
      this.connected.set(true);
      await this.rejoin();
    } catch {
      this.connected.set(false);   // dashboard's polling fallback still runs
    }
  }

  private async ensureJoined(siteId: string): Promise<void> {
    if (!this.conn || this.conn.state !== signalR.HubConnectionState.Connected) {
      this.joinedSite = siteId;
      await this.start();
      return;
    }
    if (this.joinedSite && this.joinedSite !== siteId) {
      try { await this.conn.invoke('LeaveSite', this.joinedSite); } catch { /* ignore */ }
    }
    this.joinedSite = siteId;
    try { await this.conn.invoke('JoinSite', siteId); } catch { /* ignore */ }
  }

  private async rejoin(): Promise<void> {
    if (this.conn && this.joinedSite &&
      this.conn.state === signalR.HubConnectionState.Connected) {
      try { await this.conn.invoke('JoinSite', this.joinedSite); } catch { /* ignore */ }
    }
  }

  async stop(): Promise<void> {
    await this.conn?.stop();
    this.conn = undefined;
    this.connected.set(false);
    this.joinedSite = null;
  }
}
