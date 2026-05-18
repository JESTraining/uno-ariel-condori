## 📘 Intermediate Exercise: Inventory Management System with Barcode Scanning

### Business Context
Build a **warehouse inventory system** where staff can:
- Track products with quantities and locations
- Record stock movements (receive, ship, adjust)
- Scan barcodes for rapid product lookup
- Generate low-stock alerts

---

## 🎯 Core Requirements

### Backend (.NET 8+)
- [ ] REST API with these endpoints:
  - `GET /api/products` – list products (search by name, filter by category, pagination)
  - `GET /api/products/{id}` – get product details
  - `POST /api/products` – create product (name, SKU, category, price, reorderThreshold)
  - `PUT /api/products/{id}` – update product
  - `DELETE /api/products/{id}` – soft delete (set IsActive=false)
  - `GET /api/products/barcode/{barcode}` – lookup by barcode (quick scan)
  - `POST /api/stock/movements` – record stock change (+10 received, -3 shipped)
  - `GET /api/stock/alert/low` – list products below reorder threshold
  - `GET /api/stock/movements/{productId}` – movement history
- [ ] **PostgreSQL** with Entity Framework Core
- [ ] **Explicit transaction handling** – stock movements must be atomic (update product quantity + record movement in same transaction)
- [ ] **Concurrency handling** – use row version (`xmin` or `Timestamp`) to prevent lost updates
- [ ] **Business rules validation**:
  - Stock cannot go negative (reject ship/adjust if insufficient)
  - SKU must be unique
  - Barcode optional but unique if provided
- [ ] **Repository + Unit of Work pattern** (simplified but clear separation)
- [ ] **FluentValidation** for all request DTOs
- [ ] **Structured logging** with `ILogger` – log all stock changes with context (who, when, why)

### Frontend (Angular 17+)
- [ ] **Product Management**:
  - List view with search, category filter, pagination
  - Add/edit product form (reactive form with validation)
  - Delete with confirmation modal
- [ ] **Barcode Scan Interface**:
  - Input field that triggers lookup on Enter
  - **Mock barcode scanner simulation** (keyboard input with enter key)
  - Display product details instantly when scanned
  - Quick stock adjustment form (+/- quantity, reason text)
- [ ] **Stock Movement**:
  - Dedicated page for receiving/shipping stock
  - Real-time validation (show error if negative stock)
  - Movement history table for each product (date, quantity change, running balance, reason)
- [ ] **Low Stock Dashboard**:
  - Card/tile view of products needing reorder
  - Highlight in red if stock < 25% of threshold
  - "Request restock" button (just logs to console for now)
- [ ] **State management with Signals**:
  - `ProductStore` signal with products array, selected product, filter criteria
  - `StockStore` signal for current stock levels (refresh after movements)
- [ ] **Responsive layout** – works on tablet (warehouse floor use case)

### Docker
- [ ] Dockerfile for backend (multi-stage build)
- [ ] Dockerfile for frontend (nginx)
- [ ] `docker-compose.yml` with:
  - Backend (port 5000)
  - Frontend (port 8080)
  - PostgreSQL (port 5432, with volume)
  - pgAdmin (optional, for DB inspection)
- [ ] **Environment variables** for DB connection (separate for dev/prod)
- [ ] **Healthcheck** on backend (call `/health` endpoint)
- [ ] **Init script** to run migrations on startup

---

## 📅 1-Week Schedule

| Day | Focus | Specific Deliverables |
|-----|-------|----------------------|
| **Day 1** | Backend Core | API project, EF Core + PostgreSQL, Product CRUD, SKU unique constraint |
| **Day 2** | Backend Stock Logic | Stock movement endpoints, transaction handling, concurrency, low stock query |
| **Day 3** | Angular Setup | Project structure, routing, product list/add/edit, search/filter |
| **Day 4** | Angular Stock Features | Barcode lookup, stock movement UI, movement history, low stock dashboard |
| **Day 5** | Docker + Polish | Dockerfiles, compose, volume testing, form validation, error messages, README |

---

## 🎨 Stretch Goals (If done early)

- [ ] **Export to CSV** – products list, stock movements
- [ ] **Audit log** – who changed stock (add user column, even if hardcoded user "warehouse1")
- [ ] **Batch stock update** – upload CSV to adjust multiple products
- [ ] **Simple chart** – stock level history line chart (Chart.js)
- [ ] **Unit tests** – xUnit for stock movement business rules (negative stock prevention)
- [ ] **Barcode generation** – display barcode image for product (use any free library)

---

## 🛠️ Tech Stack

```
Backend:  .NET 8 Web API + EF Core + PostgreSQL
Frontend: Angular 18 + ReactiveForms + Signals + HttpClient
Database: PostgreSQL (running in Docker)
Container: Docker + Docker Compose
Testing:  Swagger/Postman + optional xUnit/Jasmine
```

---

## 📊 Sample Data Model

```sql
Products
- Id (Guid)
- SKU (string, unique)
- Barcode (string, nullable, unique)
- Name (string)
- Category (string)
- Price (decimal)
- CurrentStock (int)
- ReorderThreshold (int)
- IsActive (bool)
- RowVersion (timestamp)

StockMovements
- Id (Guid)
- ProductId (Guid, FK)
- QuantityChange (int, positive or negative)
- PreviousStock (int)
- NewStock (int)
- Reason (string, "received", "shipped", "adjustment")
- CreatedAt (timestamp)
- CreatedBy (string, "system" or hardcoded user)
```

---

## ✅ Success Criteria

Developer can demonstrate:

1. **Run** `docker-compose up` → app accessible at `http://localhost:8080`
2. **Create** a product (SKU: "PROD-001", Barcode: "1234567890")
3. **Receive** 100 units → stock shows 100
4. **Ship** 25 units → stock shows 75, history shows both movements
5. **Attempt** to ship 100 units → error "Insufficient stock"
6. **Scan** barcode "1234567890" (type into field + Enter) → product form pre-fills
7. **Filter** low stock (threshold 50) → shows products with stock < 50
8. **Restart** containers → data persists

---

## 🔧 Common Pitfalls (Watch for these in review)

| Pitfall | Why it's a problem |
|---------|---------------------|
| Stock quantity stored only in Products, recalculated from movements each time | Performance disaster with thousands of movements |
| No transaction on stock change | Product quantity could update without recording movement (data loss) |
| Concurrency ignored | Two users receiving stock simultaneously → lost updates |
| Barcode scan triggers full page reload | Terrible UX for warehouse scanning workflow |
| Hardcoded connection string in docker-compose | Security risk, environment coupling |
| No negative stock validation | Inventory goes negative (business rules broken) |
| Pagination missing | Product list unusable with 1000+ products |
| Frontend directly calls DELETE without confirmation | Accidental data loss |

---

## 📝 Deliverables Checklist

- [ ] GitHub repo with meaningful commit messages
- [ ] `README.md` containing:
  - Setup instructions (Docker + manual)
  - API examples (curl or Postman collection link)
  - Screenshots of product list, stock movement, low stock dashboard
  - Explanation of concurrency handling approach
- [ ] `docker-compose.yml` tested on clean Ubuntu/Mac/Windows
- [ ] Database migrations included (not just `EnsureCreated`)
- [ ] No compiler warnings or linting errors
- [ ] `.gitignore` excludes `.env`, `appsettings.Development.json`, `node_modules`, `bin/`, `obj/`

---

## 📊 Evaluation Rubric

| Criterion | Weight | Pass if... |
|-----------|--------|------------|
| Stock transaction integrity | 20% | Cannot create inconsistency, atomic updates work |
| Business rules enforcement | 15% | No negative stock, unique SKU/barcode |
| Angular functionality | 20% | All CRUD + barcode + stock movement UI works |
| Data persistence | 15% | Docker volumes survive restarts, migrations run |
| Code organization | 15% | Repository pattern clear, DTOs separate from entities |
| Error handling | 10% | User sees friendly messages for validation failures |
| Docker maturity | 5% | Healthchecks, environment variables, proper entrypoints |

---

## 🆚 How This Differs from Junior Exercise

| Aspect | Junior (Task Manager) | Intermediate (Inventory) |
|--------|----------------------|-------------------------|
| Database relationships | One table | Two related tables (FK constraint) |
| Transactions | None needed | Required for data integrity |
| Business logic | Simple CRUD | Complex rules (negative stock prevention) |
| Concurrency | Not applicable | Row version handling |
| Data validation | Basic required fields | Multiple business rules, cross-field validation |
| Frontend complexity | Simple list + form | Search, filters, pagination, signals, history view |
| Realistic workflow | Generic CRUD | Mimics real warehouse process |
| Docker complexity | Basic compose | Volume mounts, healthchecks, init migrations |

---

## 💡 Interview Questions to Ask During Review

1. "How would you add a 'reserved stock' feature (orders placed but not shipped)?"
2. "Show me how you'd prevent two warehouse workers from shipping the same item simultaneously"
3. "What happens if the database transaction succeeds but the API response fails? How would you handle that?"
4. "How would you generate a daily stock snapshot report without performance impact?"
5. "If barcode scanning needs to work offline, how would you redesign the frontend?"
