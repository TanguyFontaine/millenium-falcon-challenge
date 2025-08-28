import { Component } from '@angular/core';

@Component({
  selector: 'app-empire-data-upload-button',
  imports: [],
  templateUrl: './EmpireDataUploadButton.html',
  styleUrl: './EmpireDataUploadButton.css'
})
export class EmpireDataUploadButton {

  onUploadClick() {
    console.log('Empire Data Upload button clicked!');
    // This button does nothing for now
  }
}
