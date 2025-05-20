import { Component, OnInit, OnDestroy } from '@angular/core';
import { MatExpansionModule } from '@angular/material/expansion';
import { Subscription, Observable, map} from 'rxjs'
import { Apollo, gql } from 'apollo-angular';
import { CommonModule } from '@angular/common';
import { BaseChartDirective } from 'ng2-charts';
import { ChartConfiguration, ChartOptions, ChartType } from "chart.js";

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
        BaseChartDirective,
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
          stockTimeSerieFacts: null
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

  getFrom() : string {
    // TODO UTC
    let today = new Date();
    today.setFullYear(today.getFullYear()-1)
    return today.toISOString(); 
  }

  getTo() : string {
    // TODO UTC
    let today = new Date();
    return today.toISOString();     
  }

  getTimeseries(isin: string) {
    // TODO resolve cache error
    this.timeSeriesSubscription = this.apollo
      .watchQuery( { 
        query: GET_TIMESERIES,
        variables: {
          isin: isin,
          timeseriesname: "PriceAndVolume",
          from: this.getFrom(),
          to: this.getTo(),
        },
      }).valueChanges.subscribe( (result: any) => {
          // TODO replace linear search
          let stockIx = this.stocks.findIndex( s => s.isin === isin);
          if (stockIx > -1 ) {
            this.stocks[stockIx].stockTimeSerieFacts= result.data?.timeseries?.stockTimeSerieFacts;
          }
          this.loading = result.loading;
          this.errors = result.errors;
      });
  }

  // TODO Code a component for the stock chart, and resolve responsiveness
  getLineChartData(isin: string) : ChartConfiguration<'line'>['data'] {
    let stockIx = this.stocks.findIndex( s => s.isin === isin);
    let result : ChartConfiguration<'line'>['data'] = { 
      labels: [],
      datasets: [
      {
        data: [],
        label: '',
        fill: true,
        borderColor: 'black',
        backgroundColor: 'rgba(120, 196, 240, 0.3)'
      }
    ]};
    if ( (stockIx > -1) && (this.stocks[stockIx].stockTimeSerieFacts !== null ) ) {      
      let timeSeriesData = this.stocks[stockIx].stockTimeSerieFacts.timeSeriesData;
      result.datasets[0].label =  this.stocks[stockIx].stockTimeSerieFacts.name;
      var i: number;
      for (i=0; i<timeSeriesData.length; i++) {
        let date = new Date(timeSeriesData[i].date);
        result.labels?.push( date.toISOString().slice(0,10));
        result.datasets[0].data.push(parseFloat(timeSeriesData[i].price))
      };
    }
    return result;
  }

  public lineChartOptions: ChartOptions<'line'> = {
    responsive: true
  };

  public lineChartLegend = false;

}
