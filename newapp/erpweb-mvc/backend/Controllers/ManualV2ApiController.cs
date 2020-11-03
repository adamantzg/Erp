using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using company.Common;
using erp.DAL.EF.New;
using erp.Model;
using System.IO;
using System.Net.Http.Headers;
using System.Web;
using Utilities = company.Common.Utilities;
using erp.Model.Dal.New;
using backend.Models;
using System.Text;
using backend.Properties;
using backend.ApiServices;

namespace backend.Controllers
{
    [Authorize]
    public class ManualV2ApiController : ApiController
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IAccountService accountService;

        public ManualV2ApiController(IUnitOfWork unitOfWork, IAccountService accountService)
        {
            this.unitOfWork = unitOfWork;
            this.accountService = accountService;
        }


        [Route("api/manualv2/GetManualV2Model")]
        [HttpGet]
        public object GetManualV2Model(string state, int? id = null)
        {
            var model = new ManualV2Model
            {
                ImageRootFolder = string.Empty,
                Manual = state == "create" ? new manual {AdministrationGroups = new List<manual_administration>(),Nodes = new List<manual_node>(), date_created = DateTime.Today} 
                                                : unitOfWork.ManualV2Repository.Get(m => m.manual_id == id, includeProperties: "Nodes.EditHistoryRecords").FirstOrDefault()
            };

            return model;
        }
        
        [Route("api/manualv2/GetManuals")]
        [HttpGet]
        public object GetManuals()
        {
            var manuals = unitOfWork.ManualV2Repository.Get( includeProperties:"Nodes.EditHistoryRecords").OrderByDescending(m => m.date_created).ToList();

            var creators_ids = manuals.Select(m => m.creator_id).ToList();

            //creators
            var creators = unitOfWork.UserRepository.Get(u => creators_ids.Contains(u.userid)).ToList();

            return manuals.Select(m => new {
                m.manual_id,
                m.title,
                m.short_description,
                m.version,
                m.creator_id,
                m.date_created,
                Creator = creators.Where(c => c.userid == m.creator_id).FirstOrDefault(),
                LastEdit = m.Nodes.SelectMany(n => n.EditHistoryRecords).Where(n => n.EditUser != null).OrderByDescending(n => n.edit_timestamp)
                                .Select(n => new { n.EditUser, n.edit_timestamp }).FirstOrDefault()
               }
            );
        }
        
        [Route("api/manualv2/GetManual")]
        [HttpGet]
        public object GetManual(int? manual_id)
        {
            var manual = unitOfWork.ManualV2Repository.Get(m => m.manual_id == manual_id,includeProperties: "Nodes.EditHistoryRecords").FirstOrDefault();
            return manual;
        }

        [Route("api/manualv2/CreateManual")]
        [HttpPost]
        public object CreateManual(manual manual)
        {
            manual.creator_id = accountService.GetCurrentUser().userid;
            manual.date_created = DateTime.Now;

            unitOfWork.ManualV2Repository.Insert(manual);
            unitOfWork.Save();

            //reminder
            //WebUtilities.ClearTempFiles("temp_");

            return manual;

        }

        [Route("api/manualv2/UpdateManual")]
        [HttpPut]
        public object UpdateManual(manual manual)
        {
            unitOfWork.ManualV2Repository.Update(manual);
            unitOfWork.Save();

            return manual;
        }

        [Route("api/manualv2/DeleteManual")]
        [HttpPost]
        public object DeleteManual(manual manual)
        {
            if(manual != null)
            {
                unitOfWork.ManualV2Repository.Delete(manual);
                unitOfWork.Save();

                return Json("OK");
            }

            return Json("NOTOK");
        }
            
        [Route("api/manualv2/CopyManual")]
        [HttpPost]
        public object CopyManual(manual manual)
        {
            manual manual_copy = new manual();

            if (manual != null)
            {
                var man = unitOfWork.ManualV2Repository.Get(m => m.manual_id == manual.manual_id, includeProperties:"Nodes").FirstOrDefault();

                if (man != null)
                {
                    unitOfWork.ManualV2Repository.Insert(manual_copy);
                    unitOfWork.ManualV2Repository.Copy(man, manual_copy);

                    manual_copy.title = "copy of " + manual_copy.title;
                    unitOfWork.Save();

                }
            }

            return manual_copy;
        }

        [Route("api/manualv2/CreateManualNode")]
        [HttpPost]
        public object CreateManualNode(manual manual, manual_node node)
        {
            return node;
        }

        [Route("api/manualv2/UpdateManualNode")]
        [HttpPut]
        public object UpdateManualNode(manual manual, manual_node node)
        {
            return node;
        }
        
        [Route("api/manualv2/DeleteManualNode")]
        [HttpPost]
        public object DeleteManualNode(manual manual, manual_node node)
        {

            return Json("OK");
        }

    }
}
