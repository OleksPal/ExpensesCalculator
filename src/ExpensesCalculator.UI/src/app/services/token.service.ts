import { Injectable, Inject, PLATFORM_ID } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';

export interface LoginRequest {
  email: string;
  password: string;
}

export interface AuthResponse {
  accessToken: string;
  refreshToken: string;
}

@Injectable({
  providedIn: 'root'
})
export class TokenService {
  private readonly ACCESS = 'access_token';
  private readonly REFRESH = 'refresh_token';
  private isBrowser: boolean;

  constructor(@Inject(PLATFORM_ID) platformId: object) {
    this.isBrowser = isPlatformBrowser(platformId);
  }

  getAccessToken(): string | null {
    if (!this.isBrowser) return null;
    return sessionStorage.getItem(this.ACCESS);
  }

  getRefreshToken(): string | null {
    if (!this.isBrowser) return null;
    return sessionStorage.getItem(this.REFRESH);
  }

  setTokens(access: string, refresh: string) {
    if (!this.isBrowser) return null;
    sessionStorage.setItem(this.ACCESS, access);
    sessionStorage.setItem(this.REFRESH, refresh);
    return;
  }

  clear() {
    if (!this.isBrowser) return null;
    sessionStorage.removeItem(this.ACCESS);
    sessionStorage.removeItem(this.REFRESH);
    
    return;
  }
}
