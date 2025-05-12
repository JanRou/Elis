import { Component, OnInit, OnDestroy } from '@angular/core';
import { MatExpansionModule } from '@angular/material/expansion';
import { Subscription, Observable, map} from 'rxjs'
import { Apollo, gql } from 'apollo-angular';
import { CommonModule } from '@angular/common';

const GET_STOCKS = gql` { 
  stocks { 
    stocks( take: 0, skip: 0 ) 
    { name, isin, exchange, currency, instrumentCode } 
  } 
}`;

const GET_TIMESERIES = gql`query GetStockTimeSerieFacts($isin: String!, $timeseriesname: String!, $from: String!, $to: String!) {
  timeseries {
    stockTimeSerieFacts(
      isin: $isin
      timeseriesname: $timeseriesname
      from: $from
      to: $to
    ) {
      isin
      name
      timeSeriesData {
        date
        price
        volume
      }
    }
  }
}`;

type stockType = {
  name: string;
  isin: string;
  currency: string;
  exchange: string;
  instrumentCode: string;
  stockTimeSerieFacts: any;
}

@Component({
    selector: 'app-dashboard',
    standalone: true,
    imports: [
        MatExpansionModule,
        CommonModule,
    ],
    templateUrl: './dashboard.component.html',
    styleUrl: './dashboard.component.css'
})
export class DashboardComponent implements OnInit, OnDestroy  {
  stocks: stockType[] = [];
  loading: boolean = false;
  errors: any;

  private stockQuerySubscription: Subscription | undefined;
  private timeSeriesSubscription: Subscription | undefined;

  constructor(private apollo: Apollo) { }

  ngOnInit(): void {
    this.stockQuerySubscription = this.apollo
      .watchQuery( {
        query: GET_STOCKS
      }).valueChanges.subscribe( (result: any) => {
        this.stocks = result.data?.stocks?.stocks.map(  
            (s: { name: any; isin: any; currency: any; exchange: any; instrumentCode: any; }) => ({
          name: s.name,
          isin: s.isin,
          currency: s.currency,
          exchange: s.exchange,
          instrumentCode: s.instrumentCode,
          stockTimeSerieFacts: {}
      }));
        this.loading = result.loading;
        this.errors = result.errors;
      });
  }

  ngOnDestroy() {
    if (this.stockQuerySubscription != undefined) {
      this.stockQuerySubscription.unsubscribe();
    }
    if (this.timeSeriesSubscription != undefined) {
      this.timeSeriesSubscription.unsubscribe();
    }
  }

  getTimeseries(isin: string) {
    this.timeSeriesSubscription = this.apollo
      .watchQuery( { 
        query: GET_TIMESERIES,
        variables: {
          isin: isin,
          timeseriesname: "PriceAndVolume",
          from: "2024-05-12 02:00:00+00",
          to: "2025-05-12 02:00:00+00",
        },
      }).valueChanges.subscribe( (result: any) => {
          let stockIx = this.stocks.findIndex( s => s.isin === isin);
          if (stockIx > -1 ) {
            this.stocks[stockIx].stockTimeSerieFacts= result.data?.timeseries?.stockTimeSerieFacts;
          }
          this.loading = result.loading;
          this.errors = result.errors;
      });
  }

}
