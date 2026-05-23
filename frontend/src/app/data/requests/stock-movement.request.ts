export interface StockMovementRequest {
  productId: string;
  quantityChange: number;
  reason: string;
  version: string;
}