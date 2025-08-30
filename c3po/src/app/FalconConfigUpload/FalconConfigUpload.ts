import { Component, ViewChild, ElementRef, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { JsonPipe, CommonModule } from '@angular/common';
import { FalconConfigService } from './FalconConfig.service';

@Component({
  selector: 'app-falcon-config-upload',
  imports: [JsonPipe, CommonModule],
  templateUrl: './FalconConfigUpload.html',
  styleUrl: './FalconConfigUpload.css'
})
export class FalconConfigUpload {

  @ViewChild('falconConfigInput') falconConfigInput!: ElementRef<HTMLInputElement>;
  
  private http = inject(HttpClient);
  private falconConfigService = inject(FalconConfigService);
  private onboardApiUrl = 'http://localhost:5001/api/OnboardComputer';

  configData: any = null;
  errorMessage: string = '';

  onFalconConfigUploadClick() { 
    this.falconConfigInput.nativeElement.click(); 
  }

  onFalconConfigFileSelected(event: Event) {
    const input = event.target as HTMLInputElement;
    const file = input.files?.[0];

    if (!file) return;

    if (!file.name.toLowerCase().endsWith('.json')) {
      this.errorMessage = 'Please select a JSON file for millennium falcon configuration.';
      input.value = '';
      return;
    }

    this.errorMessage = '';
    this.readFileAndSendToBackend(file);
    
    // Clear the input so we can select the same file again
    input.value = '';
  }

  private readFileAndSendToBackend(file: File) {
    this.configData = null;

    const reader = new FileReader();
    reader.onload = (e) => {
      try {
        const fileContent = e.target?.result as string;
        const jsonData = JSON.parse(fileContent);
        
        this.sendJsonToBackend(jsonData);

      } catch (error) {
        this.errorMessage = 'Invalid JSON file format.';
        console.error('JSON parse error:', error);
      }
    };

    reader.onerror = () => {
      this.errorMessage = 'Error reading file.';
    };

    reader.readAsText(file);
  }

  private sendJsonToBackend(jsonData: any) {
    const url = `${this.onboardApiUrl}/read`;
    
    this.http.post(url, jsonData).subscribe({
      next: (response) => {
        this.configData = response;
        
        // Store the original JSON config in the service for use by other components
        this.falconConfigService.setFalconConfig(jsonData);
      },
      error: (error) => {
        this.errorMessage = `Failed to process configuration: ${error.error?.message || error.message}`;
        console.error('Error processing falcon config:', error);
      }
    });
  }

  useDefaultConfig() {
    this.errorMessage = '';
    this.configData = null;

    // Clear the uploaded config from the service
    this.falconConfigService.clearFalconConfig();

    // Call the GET endpoint to use default file
    const url = `${this.onboardApiUrl}/read`;
    
    this.http.get(url).subscribe({
      next: (response) => {
        this.configData = response;
      },
      error: (error) => {
        this.errorMessage = `Failed to load default configuration: ${error.error?.message || error.message}`;
        console.error('Error loading default falcon config:', error);
      }
    });
  }
}
