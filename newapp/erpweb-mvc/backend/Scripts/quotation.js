function Round(value, numOfDecimalPlaces) {
    return Math.round(value * Math.pow(10, numOfDecimalPlaces)) / Math.pow(10, numOfDecimalPlaces);
}

function UpdateTableRecord(product, container_price, margin, exchange_rate, commission_percent, tableId, currencies,tariffs,vat)
{
    var retailPrice = product.cprod_retail * (1+vat );
    var qtyperCont = product.MastProduct.units_per_40nopallet_gp;
    var priceUSD = product.MastProduct.price_dollar;
    var priceGBP = product.MastProduct.price_pound;
    var gbpusdRatio = GetCurrencyGBPRatio(currencies, "USD");
    if(priceGBP == 0)
    {
        priceGBP = priceUSD / gbpusdRatio;
    }
    var costPerUnit = 0;
    if(qtyperCont > 0)
        costPerUnit = container_price/gbpusdRatio/qtyperCont;
    var eu_tax = $.grep(tariffs, function (e) {return e.tariff_id == product.MastProduct.tariff_code;});
    var tariff_rate = 0;
    var tariff_code = '';
    if(eu_tax.length > 0)
    {
        tariff_rate = eu_tax[0].tariff_rate;
        tariff_code = eu_tax[0].tariff_code;
    }
    var costNoCommision = priceGBP*(1+tariff_rate) + costPerUnit;
            
    var cifgbp = retailPrice * margin / 100;
            
    var ciflocalCurr = cifgbp * exchange_rate;
    var commission = cifgbp * commission_percent/100;
    var grossmargin = (cifgbp - costNoCommision - commission) / cifgbp;

    //<td>Code</td>
    //<td>Image</td>
    //<td>Description</td>
    //<td>Catalogue price</td>
    //<td>Factory price</td>
    //<td>Qty per container</td>
    //<td>Cost of freight per piece</td>
    //<td>EU duty code</td>
    //<td>EU duty %</td>
    //<td>True landed cost ex. comm.</td>
    //<td>Commission</td>
    //<td>Gross profit margin</td>
    //<td>CIF &pound;</td>
    //<td>CIF <span id="spnCurrency"></span></td>

    var tds = [];
    tds.push($.validator.format('<td><a href="javascript:void(0)" onclick="DeleteLine({0})"><img src="/images/menus/delete.gif" ></a><input type="hidden" id="line_{0}" name="line_{0}" value="{0}"/> </td>',product.cprod_id));
    tds.push($.validator.format("<td>{0}</td>", product.cprod_code1));
    tds.push($.validator.format('<td><img src="{0}" width="50" alt="image"></td>', product.MastProduct.prod_image1));
    tds.push($.validator.format("<td>{0}</td>", product.cprod_name));
    tds.push($.validator.format('<td class="number">{0}</td>', Round(retailPrice,2)));
    tds.push($.validator.format('<td class="number">{0}</td>', Round(priceGBP,2)));
    tds.push($.validator.format('<td class="number">{0}</td>', qtyperCont));
    tds.push($.validator.format('<td class="number">{0}</td>', Round(costPerUnit,2)));
    tds.push($.validator.format("<td>{0}</td>", tariff_code));
    tds.push($.validator.format('<td class="number">{0}</td>', Round(tariff_rate*100,2)));
    tds.push($.validator.format('<td class="number">{0}</td>', Round(costNoCommision,2)));
    tds.push($.validator.format('<td class="number">{0}</td>', Round(commission,2)));
    tds.push($.validator.format('<td class="number">{0}</td>', Round(grossmargin,2)));
    tds.push($.validator.format('<td class="number">{0}</td>', Round(cifgbp,2)));
    tds.push($.validator.format('<td class="number">{0}</td>', Round(ciflocalCurr,2)));
    tds.push($.validator.format("<td>{0}</td>", product.cprod_stock_code));
    tds.push($.validator.format("<td>{0}</td>", product.moq));
    tds.push($.validator.format("<td>{0}</td>", product.MastProduct.Factory.consolidated_port));

            
    if($('#' + tableId + ' > tbody').children('[id="' + product.cprod_id + '"]').length == 0)
        //Add if it doesn't exist
        $(('#' + tableId + ' > tbody')).append($.validator.format('<tr class="tablerow" id="{1}">{0}</tr>', tds.join(''), product.cprod_id));
    else
    {
        //Update existing
        var tr = $(('#' + tableId + ' > tbody')).children('[id="' + product.cprod_id + '"]');
        //cost per unit
        tr.children(':eq(7)').html(tds[7]);
        //cost no commission
        tr.children(':eq(10)').html(tds[10]);
        // commission
        tr.children(':eq(11)').html(tds[11]);
        // gross margin
        tr.children(':eq(12)').html(tds[12]);
        //cif
        tr.children(':eq(13)').html(tds[13]);
        // cif gbp
        tr.children(':eq(14)').html(tds[14]);

    }
            
}

function GetCurrencyGBPRatio(currencies, curr_symbol)
{
    var curr = $.grep(currencies, function (e) {return e.curr_symbol == curr_symbol;});
    if(curr.length > 0)
        priceGBP = curr[0].curr_exch2;
    else
        priceGBP = 1.6;
    return priceGBP;
}



function GetFloatValue(svalue)
{
    var retval = parseFloat(svalue);
    if (isNaN(retval))
        retval = 0;
    return retval;
}