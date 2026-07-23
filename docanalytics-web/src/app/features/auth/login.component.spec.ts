import { TestBed } from '@angular/core/testing';
import { ActivatedRoute, Router } from '@angular/router';
import { of, throwError } from 'rxjs';
import { LoginComponent } from './login.component';
import { AuthService } from '../../core/services/auth.service';

describe('LoginComponent', () => {
  const navSpy = vi.fn();
  let loginSpy: ReturnType<typeof vi.fn>;

  function setup() {
    loginSpy = vi.fn();
    TestBed.configureTestingModule({
      imports: [LoginComponent],
      providers: [
        {
          provide: AuthService,
          useValue: {
            login: loginSpy,
            ensureSession: vi.fn().mockResolvedValue(false),
            sites: () => [{ site_id: 's1', site_name: 'Plant One' }],
            currentUser: () => ({ id: '1', email: 'a@org.com', role: 'Viewer' }),
            logout: vi.fn(),
          },
        },
        { provide: Router, useValue: { navigate: navSpy, navigateByUrl: navSpy } },
        // RouterLink (added for the "Forgot password?" link) needs ActivatedRoute.
        { provide: ActivatedRoute, useValue: {} },
      ],
    });
    return TestBed.createComponent(LoginComponent).componentInstance;
  }

  beforeEach(() => navSpy.mockClear());

  it('form is invalid when empty', () => {
    const comp = setup();
    expect(comp.form.valid).toBe(false);
  });

  it('valid form + successful login navigates away', () => {
    const comp = setup();
    loginSpy.mockReturnValue(of({ data: { token: 't', user: {}, sites: [] }, error: null }));
    comp.form.setValue({ email: 'a@org.com', password: 'pw' });
    comp.submit();
    expect(loginSpy).toHaveBeenCalledWith('a@org.com', 'pw');
    expect(navSpy).toHaveBeenCalled();
  });

  it('shows an error message on bad credentials (401)', () => {
    const comp = setup();
    loginSpy.mockReturnValue(throwError(() => ({ status: 401 })));
    comp.form.setValue({ email: 'a@org.com', password: 'bad' });
    comp.submit();
    expect(comp.errorMessage()).toBeTruthy();
    expect(navSpy).not.toHaveBeenCalled();
  });
});
