import { TestBed } from '@angular/core/testing';
import { Router, UrlTree } from '@angular/router';
import { authGuard } from './auth.guard';
import { AuthService } from '../services/auth.service';

describe('authGuard', () => {
  const run = () => TestBed.runInInjectionContext(() => authGuard({} as any, {} as any));

  function setup(auth: Partial<AuthService>) {
    TestBed.configureTestingModule({
      providers: [
        { provide: AuthService, useValue: auth },
        {
          provide: Router,
          useValue: {
            parseUrl: (u: string) => ({ url: u }) as unknown as UrlTree,
            createUrlTree: (c: any[]) => ({ url: c.join('/') }) as unknown as UrlTree,
            navigate: vi.fn(),
          },
        },
      ],
    });
  }

  it('redirects to /login when there is no token', async () => {
    setup({ token: () => null, ensureSession: vi.fn().mockResolvedValue(false) } as any);
    expect(await run()).not.toBe(true); // UrlTree → redirect
  });

  it('allows navigation when a session exists', async () => {
    setup({
      token: () => 'jwt',
      currentUser: () => ({ id: 'u1', email: 'a@org.com', role: 'Viewer' }),
      ensureSession: vi.fn().mockResolvedValue(true),
    } as any);
    expect(await run()).toBe(true);
  });
});
