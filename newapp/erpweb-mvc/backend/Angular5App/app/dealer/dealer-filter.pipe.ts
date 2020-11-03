import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'dealerFilterPipe'
})
export class DealerFilterPipe implements PipeTransform {

  transform(value: any, args?: any): any {
    
    if(!args)
    {
      return value;
      // const m=value;
      // // m.forEach(element => {
      // //   element.user_name = element.user_name.replace(/<b>/g,'').replace('</b>','');
      // // });
      // return m;
    }
    const res= value.filter(function (el) {
      
       return (el.user_name.toLowerCase().indexOf(args.toLowerCase())>-1 
         || el.postcode.toLowerCase().indexOf(args.toLowerCase())>-1
         || el.user_address1.toLowerCase().indexOf(args.toLowerCase())>-1
         || el.user_address2.toLowerCase().indexOf(args.toLowerCase())>-1
         || el.user_address3.toLowerCase().indexOf(args.toLowerCase())>-1
         || el.user_address4.toLowerCase().indexOf(args.toLowerCase())>-1
         || el.user_address5.toLowerCase().indexOf(args.toLowerCase())>-1
       )
     })
     // const res= value.filter((el)=>{
     //   el.user_name.toLowerCase().indexOf(args.toLowerCase()>-1)
     // });
    
       // res.forEach(element => {
       //   var re = new RegExp('(<([^>]+)>)','ig');
       //   element.user_name=element.user_name.replace(re,"");
         
       // });
       // res.forEach(element => {
       //   var substr= args;
       //   console.log("REPLACE: " + args);
       //   element.user_name = element.user_name.replace(new RegExp(substr,'ig'),`<b>${args}</b>`);
       // });
     
     return res;

  }

}
