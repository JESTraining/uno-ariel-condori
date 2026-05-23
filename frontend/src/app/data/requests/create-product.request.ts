export interface CreateProductRequest {
  sku: string;
  barcode: string | null;
  name: string;
  category: string;
  location: string;
  price: number;
  reorderThreshold: number;
  version: string;
}
