import { Component, input } from '@angular/core';
import { StockMovementDto } from '../../../../data/dto/stock-movement.dto';
import { DatePipe, NgClass } from '@angular/common';

@Component({
  selector: 'app-movement-history',
  standalone: true,
  imports: [DatePipe, NgClass],
  templateUrl: './movement-history.component.html',
  styleUrl: './movement-history.component.scss'
})
export class MovementHistoryComponent {
  history = input.required<StockMovementDto[]>();
}