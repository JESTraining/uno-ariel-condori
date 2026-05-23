import { ComponentFixture, TestBed } from '@angular/core/testing';

import { BarcodeScanPageComponent } from './barcode-scan.page.component';

describe('BarcodeScanPageComponent', () => {
  let component: BarcodeScanPageComponent;
  let fixture: ComponentFixture<BarcodeScanPageComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [BarcodeScanPageComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(BarcodeScanPageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
