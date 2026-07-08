import { TestBed } from '@angular/core/testing';
import { Router, UrlTree, ActivatedRouteSnapshot } from '@angular/router';
import { siteAccessGuard } from './site-access.guard';
import { AuthService } from '../services/auth.service';

describe('siteAccessGuard', () => {
  function setup(granted: boolean) {
    TestBed.configureTestingModule({
      providers: [
        {
          provide: AuthService, useValue: {
            ensureSession: vi.fn().mockResolvedValue(true),
            hasSiteAccess: (_id: string) => granted,
            sites: () => [{ site_id: 's9', site_name: 'Other' }],
          }
        },
        {
          provide: Router, useValue: {
            parseUrl: (u: string) => ({ url: u } as unknown as UrlTree),
            createUrlTree: (c: any[]) => ({ url: c.join('/') } as unknown as UrlTree),
            navigate: vi.fn(),
          }
        },
      ],
    });
  }
  const route = { paramMap: { get: () => 's1' } } as unknown as ActivatedRouteSnapshot;
  const run = () => TestBed.runInInjectionContext(() => siteAccessGuard(route, {} as any));

  it('allows access to a granted site', async () => {
    setup(true);
    expect(await run()).toBe(true);
  });

  it('blocks access to a non-granted site', async () => {
    setup(false);
    expect(await run()).not.toBe(true);
  });
});
