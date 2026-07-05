import { Injectable } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class CurrentUserService {
  // MVP placeholder until authentication provides the logged-in user.
  readonly userId = 1;
}
