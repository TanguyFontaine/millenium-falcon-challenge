import { Component, signal } from '@angular/core';
import { EmpireDataUploadButton } from "./EmpireDataUploadButton/EmpireDataUploadButton";

@Component({
  selector: 'app-root',
  imports: [EmpireDataUploadButton],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  protected readonly title = signal('Millennium Falcon Mission Control');
}
