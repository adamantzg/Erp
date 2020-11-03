using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Razor;
using erp.Model;
using backend.Models;
using erp.Model.DAL;
using erp.DAL.EF;
using backend.Properties;

namespace backend.Controllers
{
    [AllowAnonymous]
    public class ContainerController : BaseController
    {
        //
        // GET: /Conteiner/

        UnitOfWork unitOfWork = new UnitOfWork();

        public ActionResult Index(Guid statsKey)
        {
            if(statsKey == new Guid(Settings.Default.StatsKey)) {
                var model = new ContainerModel
                {
                    PacksData = unitOfWork.MastProductRepository.Get().ToList(),
                    ContainersData = Container_typesDAL.GetAll(),
                    CalculationData = new List<ContainerCalculation>()

                };

                foreach (var c in model.PacksData) {
                    model.CalculationData.Add(new ContainerCalculation
                    {
                        Mast_id = c.mast_id,

                        Pack_carton = c.carton_length == 0 ? "p" : "c",
                        PackCartonHeight = c.carton_length == 0 ? c.pack_height : c.carton_height,
                        PackCartonLen = c.carton_length == 0 ? c.pack_length : c.carton_length,
                        PackCartonWidth = c.carton_length == 0 ? c.pack_width : c.carton_width,

                        PaletHeight = c.pallet_height,
                        PaletLen = c.pallet_length,
                        PaletWidth = c.pallet_width,

                        UnitsInCarton = c.units_per_carton,
                        UnitsPerPaletSingle = c.units_per_pallet_single,

                        Pallet40gp = GetPalets(c, 0, model.ContainersData),     // "40gp"
                        Pallet20gp = GetPalets(c, 1, model.ContainersData), //"20gp"
                        Pallet40hc = GetPalets(c, 2, model.ContainersData),

                        UnitIn40hc = GetUnitsInContainer(c, 2, model.ContainersData),
                        UnitsIn40gp = GetUnitsInContainer(c, 0, model.ContainersData),
                        UnitsIn20gp = GetUnitsInContainer(c, 1, model.ContainersData),

                        Pallets_per_20 = c.pallets_per_20,
                        Pallets_per_40 = c.pallets_per_40,
                        Units_per_20pallet = c.units_per_20pallet,
                        Units_per_20nopallet = c.units_per_20nopallet,
                        Units_per_40pallet_gp = c.units_per_40pallet_gp,
                        Units_per_40pallet_hc = c.units_per_40pallet_hc,
                        Untis_per_40nopallet_gp = c.units_per_40nopallet_gp,
                        Untis_per_40nopallet_hc = c.units_per_40nopallet_hc

                        //40hc


                    });
                }

                foreach (var item in model.CalculationData) {
                    var productToUpdate = model.PacksData.FirstOrDefault(p => p.mast_id == item.Mast_id);
                    if (productToUpdate != null)
                        Update(item, productToUpdate);
                }

                unitOfWork.Save();

                //GetFor40Gp(model.CalculationData);

                //Response.AddHeader("Content-Disposition", "attachment;filename=container.xls");
                //Response.ContentType = "application/vnd.ms-excel";
                return View("ContainerSimple", model);
            }
            ViewBag.message = "No key";
            return View("Message");
            
        }


        /// <summary>
        /// Uzmi proizvode, i za zadate dimenzije kontejnera odaberi način kako će se palete posložiti
        /// 
        /// </summary>
        /// <param name="c"></param>
        /// <param name="i"></param>
        /// <param name="containersData"></param>
        /// <param name="list"></param>
        private int GetPalets(Mast_products c, int i, List<Container_types> containersData)
        {
            
            int containerWidth =  0;
             int containerHeight=  0;
             int containerLenght = 0;

            var m = containersData;
           

            foreach(var a in m.Where(n=>n.container_type_id==i)){
                 containerWidth = (int)a.width;
                 containerHeight =(int)a.height;
                 containerLenght = (int)a.length;
            }
            
            /*
             * 1. mogu iči 2 po width
             * 2. mogu ići 2 po length
             * 3. može ići samo jedna
             * 
             * jkkj
             */
            //int numberOfPaletInContainer = 0;
            //int numberOfPaletInOneRow = 0;
            //int numberOfPaletInTwoRow =0;
            //var resault = new ContainerFill();

                                                   
        

            //int numberPaletsForLength = 0;
            //int numberPaletsForWidth = 0;
            //int numberPaletsForHeight = 0;

             int numbPalletOnWidth = c.pallet_width !=0 && c.pallet_width >10  ? containerWidth / (int)c.pallet_width:0;
             int numbPalletOnHeight = c.pallet_height !=0 && c.pallet_height >10 ? containerHeight / (int)c.pallet_height:0;
             int numbPalletOnLength = c.pallet_length !=0 && c.pallet_length >10 ? containerLenght / (int)c.pallet_length:0;
             var numbPalletInConteinerWidth = (int)numbPalletOnWidth * (int)numbPalletOnHeight * (int)numbPalletOnLength;

             numbPalletOnWidth = c.pallet_length != 0 && c.pallet_width >10 ? containerWidth / (int)c.pallet_length : 0;
             numbPalletOnHeight = c.pallet_height != 0 && c.pallet_height >10 ? containerHeight / (int)c.pallet_height : 0;
             numbPalletOnLength = c.pallet_width != 0 && c.pallet_width >10 ? containerLenght / (int)c.pallet_width : 0;
             var numbPalletInConteinerLength = (int)numbPalletOnWidth * (int)numbPalletOnHeight * (int)numbPalletOnLength;

             int[] numbers = { numbPalletInConteinerLength, numbPalletInConteinerWidth };
             int num = numbers.Max();
             return num; 
                 //* (int)c.units_per_pallet_single;
           
        }


        private int? GetUnitsInContainer(Mast_products c, int i, List<Container_types> containersData)
        {
            /*Dfiniram kontejner*/
            int containerWidth = 0;
            int containerHeight = 0;
            int containerLenght = 0;

            var m = containersData;
            //var m = new ContainerModel
            //{
            //    ContainersData = Container_typesDAL.GetAll()
            //};

            //foreach (var a in m.ContainersData.Where(n => n.container_type_id == i))
            foreach (var a in m.Where(n => n.container_type_id == i))
            {
                containerWidth = (int)a.width;
                containerHeight = (int)a.height;
                containerLenght = (int)a.length;
            }

            //int numberUnitsForLength = 0;
            //int numberUnitsForWidth = 0;
            //int numberUnisForHeight = 0;

            
            
             int? packCartonHeight = Convert.ToInt32( c.carton_length== 0  ?c.pack_height:c.carton_height);
             int? packCartonWidth = Convert.ToInt32(c.carton_length== 0 ? c.pack_width:c.carton_width);  
             int? packCartonLength = Convert.ToInt32(c.carton_length == 0 ?c.pack_length:c.carton_length); 
             /*po širini  slažem kutije
              * a*b*c
              */
             var numbPackOnWidth = packCartonWidth >1 ? containerWidth / packCartonWidth:0;
             var numbPackOnHeight = packCartonHeight >1 ? containerHeight / packCartonHeight:0;
             var numbPackOnLength = packCartonLength >1 ? containerLenght / packCartonLength:0;
             var numbPackInConteinerWidth = numbPackOnWidth * numbPackOnHeight * numbPackOnLength;
            
            /*po dužini slažem kutije*/
              numbPackOnWidth = packCartonLength >1 ? containerWidth / packCartonLength:0;
              numbPackOnHeight = packCartonHeight >1 ? containerHeight / packCartonHeight:0;
              numbPackOnLength = packCartonWidth >1 ? containerLenght / packCartonWidth:0;
            var numbPackInConteinerLength =numbPackOnWidth * numbPackOnHeight * numbPackOnLength;

            /* po visini */
             numbPackOnWidth = packCartonHeight >1 ? containerWidth / packCartonHeight:0;
             numbPackOnHeight = packCartonLength >1 ? containerHeight / packCartonLength:0;
             numbPackOnLength = packCartonWidth >1 ? containerLenght / packCartonWidth:0;
            var numbPackInConteinerHeight = numbPackOnWidth * numbPackOnHeight * numbPackOnLength;

            var numbers = new[] { numbPackInConteinerWidth, numbPackInConteinerLength, numbPackInConteinerHeight };
            var num = numbers.Max();
            return c.carton_length !=0 ? num * c.units_per_carton : num* c.packunits;
        }

        public void Update(ContainerCalculation item, Mast_products m) { 
            
            
            //var m = Mast_productsDAL.GetById( item.Mast_id);
            m.pallets_per_20 =item.Pallet20gp;
            m.units_per_20pallet = item.Pallet20gp * item.UnitsPerPaletSingle;
            m.units_per_20nopallet=item.UnitsIn20gp;

            m.pallets_per_40=item.Pallet40gp;
            m.units_per_40pallet_gp = item.UnitsPerPaletSingle * item.Pallet40gp;
            m.units_per_40nopallet_gp = item.UnitsIn40gp;

            m.units_per_40pallet_hc = item.UnitsPerPaletSingle * item.Pallet40hc;            
            m.units_per_40nopallet_hc=item.UnitIn40hc; 
            
                   
          
            //Mast_productsDAL.Update(m);
           
        }

    }

}
