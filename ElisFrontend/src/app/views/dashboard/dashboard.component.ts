import { Component, OnInit } from '@angular/core';
import { MatExpansionModule } from '@angular/material/expansion';
import {Observable} from 'rxjs'
import { Apollo, gql } from 'apollo-angular';
import { CommonModule } from '@angular/common';


@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [
      MatExpansionModule
    , CommonModule
    , 
  ],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.css'
})
export class DashboardComponent implements OnInit {
  stocks: any[] = [];
  loading: boolean = false;
  errors: any;

  constructor(private apollo: Apollo) { }

  ngOnInit(): void {
    this.apollo
      .watchQuery( {
        query: gql`
          {
            stock {
              name,
              isin,
              exchangeUrl
            }
          }
        `
      }).valueChanges.subscribe( (result: any) => {
        this.stocks = result.data?.stock;
        this.loading = result.loading;
        this.errors = result.errors;
      });
  }

}
