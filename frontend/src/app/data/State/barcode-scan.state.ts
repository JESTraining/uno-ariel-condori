import { ProductDetailsDto } from "../dto/product-details.dto";

export interface BarcodeScanState {
  scannedProduct: ProductDetailsDto | null;
  loading: boolean;
  error: string | null;
  successMessage: string | null;
}