import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DayExpensesDetailsComponent } from './day-expenses-details.component';

describe('DayExpensesDetailsComponent', () => {
  let component: DayExpensesDetailsComponent;
  let fixture: ComponentFixture<DayExpensesDetailsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DayExpensesDetailsComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(DayExpensesDetailsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
