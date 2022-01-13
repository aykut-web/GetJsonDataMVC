using GetJsonDataMVC.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Xml;

namespace GetJsonDataMVC.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            IndexViewModel ivm = new IndexViewModel();

            try
            {
                GetQSFilterParams(ivm);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://intestapi.radore.com/api/account/get-all-accounts");

                request.Method = "GET";
                request.ProtocolVersion = HttpVersion.Version11;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;
                ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                request.UserAgent = "aykuterdogan";
                var httpResponse = (HttpWebResponse)request.GetResponse();
                string jsonReturn = "";
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    jsonReturn = streamReader.ReadToEnd();
                }

                ivm.Json = jsonReturn.Replace("[", "").Replace("]", "").Replace('"', ' ');
                string[] userList = ivm.Json.Split(',');

                ivm.UserList = new List<IndexRowResponse>();
                for (int i = 0; i < userList.Length; i++)
                {
                    IndexRowResponse irr = new IndexRowResponse();
                    irr.HostingDomainName = userList[i].Trim();

                    if (!String.IsNullOrEmpty(ivm.Arama))
                    {
                        if (irr.HostingDomainName.Contains(ivm.Arama))
                        {
                            ivm.UserList.Add(irr);
                        }
                    }
                    else
                    {
                        ivm.UserList.Add(irr);
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["AlertMessage"] = "Error";
            }


            return View(ivm);
        }

        [HttpPost]
        public ActionResult Index(IndexViewModel ivm)
        {
            try
            {
                string kullaniciAdiQS = ivm.Arama;

                string url = Url.Action("Index", "Home") + "?KullaniciAdi=" + Url.Encode(kullaniciAdiQS);

                return Redirect(url);
            }
            catch (Exception ex)
            {

            }

            return View(ivm);
        }


        [Route("kullanici-sil/{UserName}")]
        public ActionResult Delete(string UserName)
        {
            try
            {
                if (!String.IsNullOrEmpty(UserName))
                {
                    string newUserName = UserName.Replace("|", ".");
                    var request = (HttpWebRequest)WebRequest.Create("https://intestapi.radore.com/api/account/delete-account");

                    var postData = "HostingDomainName=" + Uri.EscapeDataString(newUserName);
                    var data = Encoding.ASCII.GetBytes(postData);

                    request.Method = "POST";
                    request.ContentType = "application/x-www-form-urlencoded";
                    request.ContentLength = data.Length;
                    request.ProtocolVersion = HttpVersion.Version11;
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;
                    ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                    request.UserAgent = "aykuterdogan";

                    using (var stream = request.GetRequestStream())
                    {
                        stream.Write(data, 0, data.Length);
                    }
                    var response = (HttpWebResponse)request.GetResponse();

                    //Şu Anki Durumda Mesaja Göre İşlem Yapılabilir
                    var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
                    TempData["AlertMessage"] = "Kayıt Başarıyla Silindi!";
                }
            }
            catch (Exception ex)
            {
                TempData["AlertMessage"] = "Error";
            }
            return RedirectToAction("Index", "Home");
        }

        [Route("kullanici-ekle-guncelle/{UserName?}")]
        public ActionResult AddEdit(string UserName)
        {
            AddEditViewModel aevm = new AddEditViewModel();

            try
            {
                if (!String.IsNullOrEmpty(UserName))
                {
                    //Edit
                    aevm.Adding = false;
                    string newUserName = UserName.Replace("|", ".");
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://intestapi.radore.com/api/account/get-account" + "?hostingDomainName=" + newUserName);

                    request.Method = "GET";
                    request.ProtocolVersion = HttpVersion.Version11;
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;
                    ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                    request.UserAgent = "aykuterdogan";
                    var httpResponse = (HttpWebResponse)request.GetResponse();
                    string jsonReturn = "";
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        jsonReturn = streamReader.ReadToEnd();
                    }

                    aevm = JsonConvert.DeserializeObject<AddEditViewModel>(jsonReturn);

                    if (aevm.Message.Contains("Success"))
                    {
                        return View(aevm);
                    }
                    else
                    {
                        //error
                    }
                }
                else
                {
                    //Add
                    aevm.Adding = true;
                }
            }
            catch (Exception ex)
            {
                TempData["AlertMessage"] = "Error";
            }

            return View(aevm);
        }

        [HttpPost]
        [Route("kullanici-ekle-guncelle/{UserName?}")]
        public ActionResult AddEdit(AddEditViewModel aevm)
        {
            try
            {
                var request = (HttpWebRequest)WebRequest.Create("https://intestapi.radore.com/api/account/create-account");

                var postData = "HostingDomainName=" + Uri.EscapeDataString(aevm.HostingDomainName);
                postData += "&HostingPackage=" + Uri.EscapeDataString(aevm.HostingPackage);
                var data = Encoding.ASCII.GetBytes(postData);

                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = data.Length;
                request.ProtocolVersion = HttpVersion.Version11;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;
                ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                request.UserAgent = "aykuterdogan";

                using (var stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }
                var response = (HttpWebResponse)request.GetResponse();

                //var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();

                if (!aevm.Adding)
                {
                    //Edit
                    TempData["AlertMessage"] = "Kayıt Başarıyla Değiştirildi!";
                    return View(aevm);
                }
                else
                {
                    //Add
                    TempData["AlertMessage"] = "Kayıt Başarıyla Eklendi!";
                    return RedirectToAction("Index", "Home");
                }
            }
            catch (Exception ex)
            {
                TempData["AlertMessage"] = "Error";
            }

            return View(aevm);
        }

        private void GetQSFilterParams(IndexViewModel ivm)
        {
            try
            {
                string kullaniciAdi = String.IsNullOrEmpty(Request.QueryString["KullaniciAdi"]) ? "" : Request.QueryString["KullaniciAdi"];

                ivm.Arama = kullaniciAdi;

            }
            catch (Exception)
            {

            }
        }
    }
}