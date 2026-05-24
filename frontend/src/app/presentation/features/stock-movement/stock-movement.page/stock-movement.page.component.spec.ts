import { ComponentFixture, TestBed } from '@angular/core/testing';

import { StockMovementPageComponent } from './stock-movement.page.component';

describe('StockMovementPageComponent', () => {
  let component: StockMovementPageComponent;
  let fixture: ComponentFixture<StockMovementPageComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [StockMovementPageComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(StockMovementPageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
