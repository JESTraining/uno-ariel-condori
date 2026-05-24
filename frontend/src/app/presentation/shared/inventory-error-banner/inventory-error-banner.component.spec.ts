import { ComponentFixture, TestBed } from '@angular/core/testing';

import { InventoryErrorBannerComponent } from './inventory-error-banner.component';

describe('InventoryErrorBannerComponent', () => {
  let component: InventoryErrorBannerComponent;
  let fixture: ComponentFixture<InventoryErrorBannerComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [InventoryErrorBannerComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(InventoryErrorBannerComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
