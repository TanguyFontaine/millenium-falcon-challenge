import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class FalconConfigService {
  private falconConfig: any = null;

  setFalconConfig(config: any) {
    this.falconConfig = config;
  }

  getFalconConfig() {
    return this.falconConfig;
  }

  hasFalconConfig(): boolean {
    return this.falconConfig !== null;
  }

  clearFalconConfig() {
    this.falconConfig = null;
  }
}
