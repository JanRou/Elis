import { Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { AppComponent } from './app.component';
import { SettingsComponent } from './settings/settings.component';
import { DashboardComponent } from './views/dashboard/dashboard.component';

export const routes: Routes = [
    { path: '',   redirectTo: '/dashboard', pathMatch: 'full' },    
    { path: 'dashboard', component: DashboardComponent },
    { path: 'homes', component: HomeComponent },
    { path: 'settings', component: SettingsComponent },
    //{ path: '**', component: PageNotFoundComponent },  // Wildcard route for a 404 page
];
