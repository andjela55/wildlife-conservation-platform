import { Component } from '@angular/core';
import { UntypedFormBuilder, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { finalize } from 'rxjs';
import { AuthService } from '../core/services/auth.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent {
  isLoading = false;
  errorMessage = '';

  loginForm = this.fb.group({
    email: ['anika.admin@example.org', [Validators.required, Validators.email]],
    password: ['Admin123!', [Validators.required, Validators.minLength(6)]]
  });

  constructor(
    private readonly authService: AuthService,
    private readonly fb: UntypedFormBuilder,
    private readonly route: ActivatedRoute,
    private readonly router: Router
  ) {}

  login(): void {
    this.errorMessage = '';

    if (this.loginForm.invalid) {
      this.loginForm.markAllAsTouched();
      this.errorMessage = 'Please enter a valid email and password.';
      return;
    }

    this.isLoading = true;
    this.authService.login(this.loginForm.getRawValue())
      .pipe(
        finalize(() => (this.isLoading = false))
      )
      .subscribe({
        next: () => {
          const returnUrl = this.getSafeReturnUrl(this.route.snapshot.queryParamMap.get('returnUrl'));
          void this.router.navigateByUrl(returnUrl);
        },
        error: () => {
          this.errorMessage = 'Invalid email or password.';
        }
      });
  }

  private getSafeReturnUrl(returnUrl: string | null): string {
    return returnUrl?.startsWith('/') && !returnUrl.startsWith('//') ? returnUrl : '/';
  }
}
