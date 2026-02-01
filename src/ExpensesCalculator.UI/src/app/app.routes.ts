import { Routes } from '@angular/router';
import { AuthGuard } from './services/auth.service';
import { HomeComponent } from './home/home/home.component';
import { LoginComponent } from './auth/login/login.component';
import { RegisterComponent } from './auth/register/register.component';
import { DayExpensesListComponent } from './dayExpenses/day-expenses-list/day-expenses-list.component';
import { DayExpensesDetailsComponent } from './dayExpenses/day-expenses-details/day-expenses-details.component';
import { DayExpensesCalculationsComponent } from './dayExpenses/day-expenses-calculations/day-expenses-calculations.component';
import { RecommendationsComponent } from './recommendations/recommendations/recommendations.component';

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
    },
    {
        path: 'day-expenses-details/:id',
        canActivate: [AuthGuard],
        component: DayExpensesDetailsComponent
    },
    {
        path: 'day-expenses/:id/calculations',
        canActivate: [AuthGuard],
        component: DayExpensesCalculationsComponent
    },
    {
        path: 'recommendations',
        canActivate: [AuthGuard],
        component: RecommendationsComponent
    }
];