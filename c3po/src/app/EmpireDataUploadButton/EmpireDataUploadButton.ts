import { Component, ViewChild, ElementRef, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { FalconConfigService } from '../FalconConfigUpload/FalconConfig.service';

@Component({
  selector: 'app-empire-data-upload-button',
  imports: [],
  templateUrl: './EmpireDataUploadButton.html',
  styleUrl: './EmpireDataUploadButton.css'
})
export class EmpireDataUploadButton {

  @ViewChild('empireDataInput') empireDataInput!: ElementRef<HTMLInputElement>;
  
  private http = inject(HttpClient);
  private falconConfigService = inject(FalconConfigService);
  private apiUrl = 'http://localhost:5000/api/pathfinder';

  empireDataJson: string = '';

  onEmpireDataUploadClick() { 
    this.empireDataInput.nativeElement.click(); 
  }

  onEmpireDataFileSelected(event: Event) 
  {
    const input = event.target as HTMLInputElement;
    const file = input.files?.[0];

    if (!file) return;

    if (!file.name.toLowerCase().endsWith('.json'))
    {
      alert('Please select a JSON file for millennium falcon configuration.');
      // Clear the input so we can select the same file again
      input.value = '';
      return;
    }

    this.readFile(file, (content) => {
      this.empireDataJson = content;
      this.checkAndCalculate();
      
      // Clear the input so we can select the same file again
      input.value = '';
    });
  }

  private readFile(file: File, callback: (content: string) => void)
  {
    const reader = new FileReader();

    reader.onload = (e) => {
      const content = e.target?.result as string;
      callback(content);
    };

    reader.onerror = () => {
      alert('Error reading the selected file.');
    };

    reader.readAsText(file);
  }

  private checkAndCalculate()
  {
    if (this.empireDataJson)
    {
      this.calculatePath();
    }
  }

  private calculatePath()
  {
    const falconConfig = this.falconConfigService.getFalconConfig();
    
    const request = {
      EmpireDataJson: this.empireDataJson,
      FalconDataJson: falconConfig ? JSON.stringify(falconConfig) : undefined
    };

    this.http.post(`${this.apiUrl}/compute`, request).subscribe({
      next: (response: any) => {
        console.log('Path result:', response);

        const message = `Path computation completed!

Success probability: ${response.successProbability}%
Days required: ${response.numberOfDays}

Configuration:
- From: ${response.configuration.departure}
- To: ${response.configuration.arrival}
- Autonomy: ${response.configuration.autonomy} days
- Countdown: ${response.configuration.countdown} days`;
        
        alert(message);
      },
      error: (error) => {
        console.error('Path calculation error:', error);
        let errorMessage = 'Error calculating path.';
        if (error.error?.message) {
          errorMessage += `\n\nDetails: ${error.error.message}`;
        }
        alert(errorMessage);
      }
    });
  }
}
