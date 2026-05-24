import { ComponentFixture, TestBed } from '@angular/core/testing';

import { LowStockDashboardPageComponent } from './low-stock-dashboard.page.component';

describe('LowStockDashboardPageComponent', () => {
  let component: LowStockDashboardPageComponent;
  let fixture: ComponentFixture<LowStockDashboardPageComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [LowStockDashboardPageComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(LowStockDashboardPageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
