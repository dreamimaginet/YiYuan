using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace YiYuan.Web.Admin.Controllers
{
    /// <summary>
    /// 上传控制器
    /// </summary>
    public class UploaderController : Controller
    {

        /// <summary>
        /// 图片上传
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ImageUpload()
        {
            // 需要返回给前台的结果

            //List<UploadFileResult> results = new List<UploadFileResult>();

            //遍历从前台传递而来的文件
            foreach (string file in Request.Files)
            {
                //把每一个上传文件封装成HttpPostedFileBase
                HttpPostedFileBase hpf = Request.Files[file];
                //如果前台传来的文件集合中有null，继续遍历其他文件
                if (hpf.ContentLength == 0 || hpf == null)
                {
                    continue;
                }

                //save path 
                //string newFilePath = @"D:/";
                string newFilePath = Server.MapPath("~/Images/Goods/");

                //save file 
                hpf.SaveAs(newFilePath + Path.GetFileName(hpf.FileName));
                
                ////给上传文件改名
                //string date = DateTime.Now.ToString("yyyyMMddhhmmss");
                ////目标文件夹的相对路径 ImageSize需要的格式
                //string pathForSaving = Server.MapPath("~/Images/Goods/");
            }

            return Json("");
            //return View();
        }


        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="largename"></param>
        /// <param name="mediumname"></param>
        /// <param name="smallname"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DeleteFileByNames(string fileName)
        {
            string pathForSaving = Server.MapPath("~/AjaxUpload");
            System.IO.File.Delete(Path.Combine(pathForSaving, fileName));
            return Json("");
        }

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="largename"></param>
        /// <param name="mediumname"></param>
        /// <param name="smallname"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DeleteFileByNames(string largename, string mediumname, string smallname)
        {
            string pathForSaving = Server.MapPath("~/AjaxUpload");
            System.IO.File.Delete(Path.Combine(pathForSaving, largename));
            System.IO.File.Delete(Path.Combine(pathForSaving, mediumname));
            System.IO.File.Delete(Path.Combine(pathForSaving, smallname));
            return Json("");
        }
    }

}