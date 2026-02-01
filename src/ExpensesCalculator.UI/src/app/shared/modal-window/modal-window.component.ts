import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-modal-window',
  imports: [CommonModule],
  templateUrl: './modal-window.component.html',
  styleUrl: './modal-window.component.css'
})
export class ModalWindowComponent {
  @Input() modalId = 'myModal';
  @Input() currentModalContent: 'add' | 'edit' | 'delete' | 'share' = 'add';
  @Input() modalTitle: string = '';
  @Input() modalSize: 'sm' | 'md' | 'lg' | 'xl' = 'md';
}
