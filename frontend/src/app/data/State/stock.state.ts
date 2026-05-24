import { StockMovementDto } from "../dto/stock-movement.dto";

export interface StockState {
  history: StockMovementDto[];
  loading: boolean;
  error: string | null;
  success: boolean;
}