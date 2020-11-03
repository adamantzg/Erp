using erp.DAL.EF.New;
using erp.Model;
using erp.Model.Dal.New;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using backend.Properties;
using System.IO;
using backend.ApiServices;

namespace backend.Controllers
{
    [Authorize]
    public class ClaimsUSApiController : ApiController
    {
        public const int BBUS_ClientId = 1105;
        public const int AmmaraFactoryId = 406;
        private readonly IUnitOfWork unitOfWork;
        private readonly IBrandsDAL brandsDAL;
        private readonly IAccountService accountService;

        public ClaimsUSApiController(IUnitOfWork unitOfWork, IBrandsDAL brandsDAL, IAccountService accountService)
        {
            this.unitOfWork = unitOfWork;
            this.brandsDAL = brandsDAL;
            this.accountService = accountService;
        }
        
        [Route("api/claimsUS/test")]
        [HttpGet]
        public string GetTest()
        {
            return "TEST API CLAIMS";
        }

        [Route("api/claimsUS/getProducts")]
        [HttpGet]
        public object GetProducts(string word, bool option)
        {
            /*option - Ammara == true, Crosswater == false*/
            var m = unitOfWork.CustProductDetailsRepository.Get(c=>c.cprod_user == BBUS_ClientId && (option ? c.factory_id  == AmmaraFactoryId : c.factory_id != AmmaraFactoryId)).ToList();
            return m;
        }

        [Route("api/claimsUS/get")]
        [HttpGet]
        public object Get()
        {
            return unitOfWork.UsDealerRepository.Get().Select(
                s=>new {
                    s.customer,s.alpha,s.name, s.address1, s.town_city,s.state_region, Expname= s.name +", "+s.town_city
                }).ToList();

        }

        [Route("api/claimsUS/createReturn")]
        [HttpPost]
        public object CreateReturn(Returns model)
        {
            model.Images=GetFiles(model.Images);

            WebUtilities.ClearTempFiles();
            var claimExist = unitOfWork.ReturnRepository.Get(c => c.return_no == model.return_no).ToList().Count>0;
            var tempFiles = WebUtilities.GetTempFiles();
            var currentUser= accountService.GetCurrentUser();
            if (claimExist )
            {
                model.request_user = currentUser.userwelcome;
                unitOfWork.ReturnRepository.Update(model);
                unitOfWork.Save();
                return "OK";
            }

            if (model.client_id == null)
                model.client_id=BBUS_ClientId;
            model.request_date =  DateTime.Now;
            model.request_user =currentUser.userwelcome; 
            model.request_userid = currentUser.userid;

            //return uof.UsDealersRepository.Get().ToList();
            unitOfWork.ReturnRepository.Insert(model);
            unitOfWork.Save();

            return "OK";
        }
        [Route("api/claimsUS/delete")]
        [HttpPost]
        public object DeleteClaim(Returns ret)
        {
            unitOfWork.ReturnRepository.Delete(ret.returnsid);
            unitOfWork.Save();
            return "OK";
        }
        //[Route("api/claimsUS/files")]
        //[HttpPost]
        //public object Files(string name)
        //{
        //    string fileName = WebUtilities.GetFileName(name, Request);
        //    return Json(WebUtilities.SaveTempFile(fileName, Request, Settings.Default.Enquiries_MaxFileSize), "text/html");

        //}

        [Route("api/claimsUS/getReference")]
        [HttpGet]
        public string GetReference(int rId)
        {
            var ClaimType = new Dictionary<int, string>
            {
                { 1, "R" }, /* Returned */
                { 5, "T" } /* Technical enquiry*/
            };
            var returnid = "";
            var retNo = $"BBUS-{DateTime.Now.ToString("ddMMyy")}-{ClaimType[rId]}";
            var returns = unitOfWork.ReturnRepository
                .Get(c => (c.claim_type == rId ) && c.return_no.Contains(retNo))
                .Select(s => new { s.returnsid, s.return_no, s.request_user, s.return_qty, s.claim_type })
            .ToList();
            if (returns.Count > 0)
            {
                /*
                    (1)st - BBUS
                    (2)nd - 100217 - ddMMyy
                    (3)rd - R or T
                    (4)th - +1
                 */
                var r = returns.Select(s => new
                {
                    returnsid = s.returnsid,
                    return_no = s.return_no,
                    claim_type = s.claim_type,
                    returnSplited = s.return_no.Split('-').ToList(),
                    st = s.return_no.Split('-')[0],
                    nd = int.Parse(s.return_no.Split('-')[1]),
                    rd = s.return_no.Split('-')[2],
                    th = int.Parse(s.return_no.Split('-')[3])
                }).OrderBy(c => c.nd).ThenBy(c => c.th).Last();
                if (r.return_no.Contains(DateTime.Now.ToString("ddMMyy")))
                {
                    returnid = $"{r.st}-{DateTime.Now.ToString("ddMMyy")}-{r.rd}-{r.th + 1}";
                }else
                {
                    returnid = $"{r.st}-{DateTime.Now.ToString("ddMMyy")}-{rId}-{0}";
                }
            }
            else
            {
                var date = DateTime.Now.ToString("ddMMyy");
                returnid= $"BBUS-{date}-{ClaimType[rId]}-0";   
                
            }


            return returnid;
        }
        [Route("api/claimsUS/getUser")]
        [HttpGet]
        public object GetCurrentUser()
        {
            var user = accountService.GetCurrentUser();

            return new
            {
                userid = user.userid,
                username = user.userwelcome
            };
        }


        [Route("api/claimsUS/getFeedbacks")]
        [HttpGet]
        public object GetFeedbacks(DateTime date)
        {
            var brands = brandsDAL.GetAll();
            var claims = unitOfWork.ReturnRepository.Get(c => c.client_id == BBUS_ClientId && c.request_date.Value.Month == date.Month && c.request_date.Value.Year == date.Year, 
                includeProperties: "Product.MastProduct,Images");

            //var b = brands.First().bra
            foreach (var item in claims)
            {
                if(item.Product != null)
                    item.Product.Brand = brands.FirstOrDefault(b=>b.user_id==item.Product.brand_userid);
            }
            return claims.Select(
                s => new {
                    s.returnsid,
                    s.return_no,
                    s.client_id,
                    s.Product?.Brand?.brandname,
                    s.request_date,
                    s.request_userid,
                    s.client_comments,
                    s.claim_type,
                    s.reason,
                    s.dealer_id,
                    s.contact_name,
                    s.contact_email,
                    s.contact_tel,
                    s.Product?.cprod_name,
                    s.Product?.cprod_code1,
                    s.Product?.cprod_id,
                    //s.Product.Client.factory_code,
                    s.Product?.MastProduct?.factory_id,
                    s.Images
                }).ToList();

        }

        [Route("api/claimsUS/getDealerFeedbacks")]
        [HttpGet]
        public object GetDealerFeedbacks(string customer)
        {
            var brands = brandsDAL.GetAll();
            var claims = unitOfWork.ReturnRepository.Get(c => c.dealer_id==customer && c.client_id == BBUS_ClientId,
                includeProperties: "Product.MastProduct,Images").ToList();

            //var b = brands.First().bra
            /*foreach (var item in claims)
            {
                item.Product.Brand = brands.FirstOrDefault(b => b.user_id == item.Product.brand_userid);
            }*/
            return claims.Select(
                s => new {
                    s.returnsid,
                    s.return_no,
                    s.client_id,
                    brands?.FirstOrDefault(b=>b.user_id == s.Product?.brand_userid)?.brandname,
                    s.request_date,
                    s.request_userid,
                    s.client_comments,
                    s.claim_type,
                    s.reason,
                    s.dealer_id,
                    s.contact_name,
                    s.contact_email,
                    s.contact_tel,
                    s.Product?.cprod_name,
                    s.Product?.cprod_code1,
                    s.Product?.cprod_id,
                    //s.Product.Client.factory_code,
                    s.Product?.MastProduct?.factory_id,
                    s.Images
                }).ToList();

        }

        public object DeleteTempImage()
        {
            var sessionFiles = WebUtilities.GetTempFiles();
            return "OK";
        }
        [Route("api/claimsUS/deleteImage")]
        [HttpPost]
        public object DeleteImage(Returns_images image)
        {
            unitOfWork.ReturnRepository.Delete(image.image_unique);
            unitOfWork.Save();
            return "OK";
        }

        public List<Returns_images> GetFiles( List<Returns_images> images )
        {
            List<string> written_files = new List<string>();
            if (images == null)
                images = new List<Returns_images>();

            try
            {
                var sessionFiles = WebUtilities.GetTempFiles();
                if (sessionFiles != null)
                {
                    Debug.WriteLine("SESSION FILES");
                    foreach (KeyValuePair<string, byte[]> kv in sessionFiles)
                    {
                        /*Write file*/
                        string filePath = company.Common.Utilities.GetFilePath(kv.Key, System.Web.Hosting.HostingEnvironment.MapPath(Settings.Default.returns_USfileroot));
                        Debug.WriteLine(filePath);
                        
                        var sw = new StreamWriter(filePath);
                        var ms = new MemoryStream(kv.Value);

                        ms.WriteTo(sw.BaseStream);
                        sw.Close();
                        if (images.Count()>0)
                        {
                          
                            foreach (var img in images)
                            {
                                if (img.return_image == kv.Key)
                                {
                                    img.return_image = $"{Settings.Default.returns_USfileroot}/{Path.GetFileName(filePath)}";
                                    img.added_date= DateTime.Now;
                            }
                            }
                        }
                        else
                        {
                            images.Add(new Returns_images { return_image = $"{Settings.Default.returns_USfileroot}/{Path.GetFileName(filePath)}", added_date = DateTime.Now });

                        }
                        written_files.Add(filePath);
                    }
                }
                else
                {

                }
            }
            catch (Exception)
            {
                //if some files writtten and error occurred, delete them
                foreach (var item in written_files)
                {
                    System.IO.File.Delete(item);
                }
                throw;
            }
            return images;
        }
    }
}
