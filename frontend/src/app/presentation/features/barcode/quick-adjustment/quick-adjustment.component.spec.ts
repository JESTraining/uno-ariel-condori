import { ComponentFixture, TestBed } from '@angular/core/testing';

import { QuickAdjustmentComponent } from './quick-adjustment.component';

describe('QuickAdjustmentComponent', () => {
  let component: QuickAdjustmentComponent;
  let fixture: ComponentFixture<QuickAdjustmentComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [QuickAdjustmentComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(QuickAdjustmentComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
