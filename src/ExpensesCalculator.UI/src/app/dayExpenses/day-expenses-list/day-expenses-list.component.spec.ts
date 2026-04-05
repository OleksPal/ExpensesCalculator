import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DayExpensesListComponent } from './day-expenses-list.component';

describe('DayExpensesListComponent', () => {
  let component: DayExpensesListComponent;
  let fixture: ComponentFixture<DayExpensesListComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DayExpensesListComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(DayExpensesListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
