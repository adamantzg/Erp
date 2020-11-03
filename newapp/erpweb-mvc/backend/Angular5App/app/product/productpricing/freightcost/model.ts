import { Market  } from '../../domainclasses';
import { Location, FreighCost} from '../../../common';

export class FreightCostRow extends Location {
    Markets: FreightCostMarket[] = [];
}

export class FreightCostMarket extends Market {
    Containers: FreightCostContainer[] = [];
}

export class FreightCostContainer {
    container_type_id: number;
    Cost: FreighCost;
}
