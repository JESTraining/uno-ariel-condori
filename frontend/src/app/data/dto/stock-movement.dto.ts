export interface StockMovementDto {
  id: string;
  productId: string;
  quantityChange: number;
  previousStock: number;
  newStock: number;
  reason: string;
  createdAt: string;
  createdBy: string;
}