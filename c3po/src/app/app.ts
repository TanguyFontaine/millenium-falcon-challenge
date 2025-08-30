import { Component, signal } from '@angular/core';
import { EmpireDataUploadButton } from "./EmpireDataUploadButton/EmpireDataUploadButton";
import { FalconConfigUpload } from "./FalconConfigUpload/FalconConfigUpload";

@Component({
  selector: 'app-root',
  imports: [EmpireDataUploadButton, FalconConfigUpload],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  protected readonly title = signal('Millennium Falcon Path Computer');
}
