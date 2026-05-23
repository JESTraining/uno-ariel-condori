import { Component, input, output } from '@angular/core';

@Component({
  selector: 'app-confirm-modal',
  standalone: true,
  imports: [],
  templateUrl: './confirm-modal.component.html',
  styleUrl: './confirm-modal.component.scss'
})

export class ConfirmModalComponent {
  isOpen = input.required<boolean>();
  title = input<string>('Confirm Action');
  message = input<string>('This action cannot be undone.');

  confirm = output<void>();
  cancel = output<void>();
}