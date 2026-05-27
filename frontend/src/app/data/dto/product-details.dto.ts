export interface ProductDetailsDto {
  id: string;
  sku: string;
  barcode: string | null;
  name: string;
  category: string;
  location: string;
  price: number;
  currentStock: number;
  reorderThreshold: number;
  isActive: boolean;
  version: string;
}
