export interface ProductListItemDto {
  id: string;
  sku: string;
  barcode: string | null;
  name: string;
  category: string;
  location: string;
  price: number;
  currentStock: number;
  reorderThreshold: number;
  isLowStock: boolean;
  version: string;
}
