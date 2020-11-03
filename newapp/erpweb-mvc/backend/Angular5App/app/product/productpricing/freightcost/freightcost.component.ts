import { Component, OnInit } from '@angular/core';
import { ProductpricingService } from '../../services/productpricing.service';
import { Router } from '@angular/router';
import { ContainerType, Location, FreighCost, CommonService } from '../../../common';
import { Market } from '../../domainclasses';
import { FreightCostRow, FreightCostMarket, FreightCostContainer } from './model';

@Component({
  selector: 'app-freightcost',
  templateUrl: './freightcost.component.html',
  styleUrls: ['./freightcost.component.css']
})
export class FreightcostComponent implements OnInit {

  constructor(private productPricingService: ProductpricingService,
    private router: Router,
    private commonService: CommonService) { }


    errorMessage = '';
    containers: ContainerType[] = [];
    locations: Location[] = [];
    markets: Market[] = [];
    costs: FreighCost[] = [];
    rows: FreightCostRow[] = [];

  ngOnInit() {
    this.productPricingService.getFreightCostsModel().subscribe(data => {
      this.containers = data.containerTypes;
      this.locations = data.locations;
      this.markets = data.markets;
      this.costs = data.costs;
      this.buildDataStructure();
    },
      err => this.errorMessage = this.commonService.getError(err));

  }

  back() {
    this.router.navigate(['productpricing']);
  }

  buildDataStructure() {
    this.rows = [];
    this.locations.forEach( l => {
      const row = new FreightCostRow();
      row.id = l.id;
      row.name = l.name;
      this.markets.forEach( m => {
        const market = new FreightCostMarket();
        market.id = m.id;
        market.name = m.name;
        this.containers.forEach(c => {
          const cont = new FreightCostContainer();
          cont.container_type_id = c.container_type_id;
          let fc = this.costs.find(co => co.container_id === c.container_type_id && co.market_id === m.id && co.location_id === l.id);
          if (fc == null) {
            fc = new FreighCost();
            fc.container_id = c.container_type_id;
            fc.location_id = l.id;
            fc.market_id = m.id;
            fc.value = null;
          }
          cont.Cost = fc;
          market.Containers.push(cont);
        });
        row.Markets.push(market);
      });
      this.rows.push(row);
    });
  }

  update() {

    // Flatten structure (like selectmany)
    const costs = this.rows.map(r => r.Markets).reduce((m1, m2) => m1.concat(m2)).map(m => m.Containers)
      .reduce((c1, c2) => c1.concat(c2)).map(c => c.Cost).filter(c => c.value != null && c.value > 0);

    this.productPricingService.updateFreightCosts(costs).subscribe(updated => {
      this.costs = updated;
      this.buildDataStructure();
      this.errorMessage = null;
    },
     err => this.errorMessage = this.commonService.getError(err));
  }

}


