function ObjectToLookup(arrayOfObjects, valueField, idField,transformFunc) {
    var result = [];
    if (typeof(transformFunc) != "undefined") {
        return transformFunc(arrayOfObjects);
    }
    
    for (var i = 0; i < arrayOfObjects.length; i++) {
        result.push(new LookupItem(arrayOfObjects[i][idField], arrayOfObjects[i][valueField]));
    }
    return result;
}

function LookupItem(id, value) {
    this.id = id;
    this.value = value;
    this.label = value;
}

function GetFileNameWithoutExtension(filename) {
    var point = filename.lastIndexOf(".");
    if (point >= 0)
        return filename.substr(0, point);
    else {
        return filename;
    }
}

function GetFileName(path)
{
    var slash = path.lastIndexOf('/');
    if(slash >= 0)
        return path.substr(slash + 1, path.length);
    return path;
}

function GetExtension(filename) {
    var point = filename.lastIndexOf(".");
    if (point >= 0)
        return filename.substr(point,filename.length);
    else {
        return '';
    }
}

function isPicture(filename)
{
    if (filename == null)
        return false;
    var ext = GetExtension(filename).toLowerCase();
    return ext == "jpg" || ext == "jpeg" || ext == "gif" || ext == "png" || ext == "bmp";
}

function BindAutoComplete(jqObject, url, valueField, idField, hiddenId,dataVarName, transformFunction) {
    jqObject.autocomplete({
        source: function (request, response) {

            var id = this.element[0].id;

            $.ajax({
                type: "POST",
                contentType: "application/json; charset=utf-8",
                url: url,
                dataType: "json",
                data: "{" +
                    "'prefixText': '" + request.term + "'," +
                    "'country_id': ''}",
                success: function (data) {
                    if(dataVarName.length>0)
                        eval(dataVarName + "=data;");
                    if (typeof transformFunction == "undefined") {
                        response(ObjectToLookup(data, valueField, idField));
                    } else {
                        response(transformFunction(data));
                    }
                },
                error: function (data) {
                }
            });
        },
        minLength: 0,
        select: function (event, ui) {
            if (ui.item) {
                $(hiddenId).val(ui.item.id);
            }
        },
        change: function (event, ui) {
            var id = '';
            if (ui.item != null)
                id = ui.item.id;
            $(hiddenId).val(id);
            return false;
        }
    });
}

function PrepareDate(dmyDate) {
    //TODO: Logic for other cultures
    var arr = dmyDate.split('/');
    return arr[2] + '/' + arr[1] + '/' + arr[0];
}

function FormatDate(date) {
    return date.getDate() + "/" + (date.getMonth() + 1) + "/" + date.getFullYear();
}

//returns formatted string
function fromJSONDateFormatted(jsonDate) {
    if (jsonDate != null) {
        var value = new Date(parseInt(jsonDate.substr(6)));
        return moment(value).format('DD/MM/YYYY');
    } else {
        return '';
    }

}

function fromDateFormatted(date) {
    if (date.substring(0, 5) === '/Date')
        return fromJSONDateFormatted(date);
    else {
        //var value = new Date(date);
        //return $.global.format(value, 'd');
        return moment(date).format('DD/MM/YYYY');
    }
}

function fromJSONDateTime(jsonDate) {
    if (jsonDate != null) {
        var value = new Date(parseInt(jsonDate.substr(6)));
        return value;// $.global.format(value, 'dd/MM/yyyy hh:mm');
    } else {
        return null;
    }

}

function fromJSONDate(jsonDate) {
    if (jsonDate != null) {
        var value = new Date(parseInt(jsonDate.substr(6)));
        return value; //$.global.format(value, 'd');
    } else {
        return null;
    }

}


function UpdateCalendar(id) {
    var date = $('#' + id + '_Calendar').val();
    if (date.length > 0) {
        date = new Date(PrepareDate(date));

        $('#' + id +'_Day').val(date.getDate());
        $('#' + id +'_Month').val(date.getMonth() + 1);
        $('#' + id + '_Year').val(date.getFullYear());
        DateTimeInput_UpdateCalendar(id);
        //$('#' + id + '___Hidden').data('_Curr',date);
    }
}

function BindMonthYear(name) {
    $('[name="' + name + '__Month"],[name="' + name + '__Year"]').change(function () {
        var month = $('[name="' + name + '__Month"]:visible').val();
        var year = $('[name="' + name + '__Year"]:visible').val();
        var hidden = $('[name="' + name + '"]:last');
        if (month.length > 0 && year.length > 0) {
            hidden.val(year + '-' + month + '-01');
        } else {
            hidden.val('');
        }
    });
}

function numberWithCommas(x) {
    var parts = x.toString().split(".");
    parts[0] = parts[0].replace(/\B(?=(\d{3})+(?!\d))/g, ",");
    return parts.join(".");
}

function number_format(number, decimals, dec_point, thousands_sep) {
    // http://kevin.vanzonneveld.net
    // +   original by: Jonas Raoni Soares Silva (http://www.jsfromhell.com)
    // +   improved by: Kevin van Zonneveld (http://kevin.vanzonneveld.net)
    // +     bugfix by: Michael White (http://getsprink.com)
    // +     bugfix by: Benjamin Lupton
    // +     bugfix by: Allan Jensen (http://www.winternet.no)
    // +    revised by: Jonas Raoni Soares Silva (http://www.jsfromhell.com)
    // +     bugfix by: Howard Yeend
    // +    revised by: Luke Smith (http://lucassmith.name)
    // +     bugfix by: Diogo Resende
    // +     bugfix by: Rival
    // +      input by: Kheang Hok Chin (http://www.distantia.ca/)
    // +   improved by: davook
    // +   improved by: Brett Zamir (http://brett-zamir.me)
    // +      input by: Jay Klehr
    // +   improved by: Brett Zamir (http://brett-zamir.me)
    // +      input by: Amir Habibi (http://www.residence-mixte.com/)
    // +     bugfix by: Brett Zamir (http://brett-zamir.me)
    // +   improved by: Theriault
    // +      input by: Amirouche
    // +   improved by: Kevin van Zonneveld (http://kevin.vanzonneveld.net)
    // *     example 1: number_format(1234.56);
    // *     returns 1: '1,235'
    // *     example 2: number_format(1234.56, 2, ',', ' ');
    // *     returns 2: '1 234,56'
    // *     example 3: number_format(1234.5678, 2, '.', '');
    // *     returns 3: '1234.57'
    // *     example 4: number_format(67, 2, ',', '.');
    // *     returns 4: '67,00'
    // *     example 5: number_format(1000);
    // *     returns 5: '1,000'
    // *     example 6: number_format(67.311, 2);
    // *     returns 6: '67.31'
    // *     example 7: number_format(1000.55, 1);
    // *     returns 7: '1,000.6'
    // *     example 8: number_format(67000, 5, ',', '.');
    // *     returns 8: '67.000,00000'
    // *     example 9: number_format(0.9, 0);
    // *     returns 9: '1'
    // *    example 10: number_format('1.20', 2);
    // *    returns 10: '1.20'
    // *    example 11: number_format('1.20', 4);
    // *    returns 11: '1.2000'
    // *    example 12: number_format('1.2000', 3);
    // *    returns 12: '1.200'
    // *    example 13: number_format('1 000,50', 2, '.', ' ');
    // *    returns 13: '100 050.00'
    // Strip all characters but numerical ones.
    number = (number + '').replace(/[^0-9+\-Ee.]/g, '');
    var n = !isFinite(+number) ? 0 : +number,
      prec = !isFinite(+decimals) ? 0 : Math.abs(decimals),
      sep = (typeof thousands_sep === 'undefined') ? ',' : thousands_sep,
      dec = (typeof dec_point === 'undefined') ? '.' : dec_point,
      s = '',
      toFixedFix = function (n, prec) {
          var k = Math.pow(10, prec);
          return '' + Math.round(n * k) / k;
      };
    // Fix for IE parseFloat(0.55).toFixed(0) = 0;
    s = (prec ? toFixedFix(n, prec) : '' + Math.round(n)).split('.');
    if (s[0].length > 3) {
        s[0] = s[0].replace(/\B(?=(?:\d{3})+(?!\d))/g, sep);
    }
    if ((s[1] || '').length < prec) {
        s[1] = s[1] || '';
        s[1] += new Array(prec - s[1].length + 1).join('0');
    }
    return s.join(dec);
}


function GetExtension(filename) {
    var parts = filename.split('.');
    if (parts.length > 1) {
        return parts[parts.length - 1];

    }
    return '';
}

function ToStringSafe(value) {
    if (value != null)
        return value.toString();
    return value;
}

function CombineUrls(url1, url2)
{
    var uri1 = _.trimEnd(url1,'/');
    var uri2 = _.trimStart(url2,'/');
    return uri1 + "/" + uri2;
}

function toMonth21(year, month)
{
    return (year - 2000).toString() + ('0' + month.toString()).slice(-2);
}

function loadQueryString() {
    var parameters = {};
    var searchString = location.search.substr(1);
    var pairs = searchString.split("&");
    var parts;
    for (i = 0; i < pairs.length; i++) {
        parts = pairs[i].split("=");
        var name = parts[0];
        var data = decodeURI(parts[1]);
        parameters[name] = data;
    }
    return parameters;
}

function clone(obj)
{
    return JSON.parse(JSON.stringify(obj));
}

function getErrorFromResponse(response) {
    if (response.data.ExceptionMessage)
        return response.data.ExceptionMessage;
    return response.data;
}