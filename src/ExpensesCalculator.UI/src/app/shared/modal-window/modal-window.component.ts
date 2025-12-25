import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-modal-window',
  imports: [],
  templateUrl: './modal-window.component.html',
  styleUrl: './modal-window.component.css'
})
export class ModalWindowComponent {
  @Input() modalId = 'myModal';
  @Input() currentModalContent: 'add' | 'edit' | 'delete' | 'share' = 'add';
  @Input() modalTitle: string = '';
}
