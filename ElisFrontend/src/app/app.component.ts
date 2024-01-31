import { Component, ViewChild } from '@angular/core';
import { BreakpointObserver } from '@angular/cdk/layout';
import { CommonModule } from '@angular/common';
import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';

import { MatExpansionModule } from '@angular/material/expansion';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatSidenav } from '@angular/material/sidenav';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatListModule } from '@angular/material/list';

import { HomeComponent } from './home/home.component';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [ 
      HomeComponent
    , CommonModule
    , MatExpansionModule
    , MatButtonModule
    , MatIconModule
    , MatSidenavModule
    , MatToolbarModule
    , MatListModule
    , RouterOutlet
    , RouterLink
    , RouterLinkActive],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent {
  title = 'Elis Your Stock Analyzer';

  @ViewChild(MatSidenav)
  sidenav!: MatSidenav;
  isCollapsed = false;

  toggleMenu() {
    this.isCollapsed = !this.isCollapsed;
    //this.sidenav.open();
  }
}
