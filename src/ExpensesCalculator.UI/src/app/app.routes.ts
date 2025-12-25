import { Routes } from '@angular/router';
import { AuthGuard } from './services/auth.service';
import { HomeComponent } from './home/home/home.component';
import { LoginComponent } from './auth/login/login.component';
import { RegisterComponent } from './auth/register/register.component';
import { DayExpensesListComponent } from './dayExpenses/day-expenses-list/day-expenses-list.component';

export const routes: Routes = [
    {
        path: '',
        component: HomeComponent
    },
    {
        path: 'login',
        component: LoginComponent
    },
    {
        path: 'register',
        component: RegisterComponent
    },
    {
        path: 'day-expenses',
        canActivate: [AuthGuard],
        component: DayExpensesListComponent
    }
];