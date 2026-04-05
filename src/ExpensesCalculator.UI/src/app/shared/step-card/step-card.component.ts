import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TranslateModule } from '@ngx-translate/core';

@Component({
  selector: 'app-step-card',
  standalone: true,
  templateUrl: './step-card.component.html',
  styleUrls: ['./step-card.component.css'],
  imports: [CommonModule, TranslateModule]
})
export class StepCardComponent {
  @Input() imageSrc!: string;
  @Input() translationKey!: string;
}
