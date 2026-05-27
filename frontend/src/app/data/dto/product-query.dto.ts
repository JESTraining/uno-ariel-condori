export interface ProductQuery {
  search?: string;
  category?: string;
  location?: string;
  page: number;
  pageSize: number;
}
