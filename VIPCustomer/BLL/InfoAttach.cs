using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class InfoAttach
    {
        string uploadPath, uploadTempPath;
        public InfoAttach()
        {
            uploadPath = ConfigurationManager.AppSettings["uploadPath"];
            uploadTempPath = ConfigurationManager.AppSettings["uploadTempPath"];
        }

        #region 删除附件
        /// <summary>
        /// 删除附件
        /// </summary>
        /// <param name="deleteAttachmentList">包含附件ID的附加列表</param>
        /// <returns></returns>
        public int DeleteByList(ArrayList deleteAttachmentList)
        {
            int deleteAttachmentFlag = 0;
            for (int i = 0; i < deleteAttachmentList.Count; i++)
            {
                String attachmenID = deleteAttachmentList[i].ToString();
                Common.Entity.InfoAttach infoAttach = new DAL.InfoAttach().GetModel(attachmenID);
                String infoAttachmentFullPath = uploadPath + infoAttach.AttachPath;
                if (infoAttach.AttachPath != null && infoAttach.AttachPath.Length > 0)
                {
                    deleteAttachmentFlag += Delete(attachmenID, infoAttachmentFullPath);
                }
            }
            return deleteAttachmentFlag;
        }
        #endregion

        public int DeleteByID(String id)
        {
            return new DAL.InfoAttach().DeleteByID(id);
        }
        /// <summary>
        /// 删除附件文件及目录并从数据库中删除附件
        /// </summary>
        /// <param name="infoAttachmentId"></param>
        /// <param name="infoAttachmentFullPath"></param>
        /// <returns></returns>
        public int Delete(string infoAttachmentId, string infoAttachmentFullPath)
        {
            int deleteAttachmentFlag = 0;
            DeleteFile(infoAttachmentFullPath);
            //如果信息ID附件文件夹为空，删除
            String infoDirectory = Directory.GetParent(System.IO.Path.GetDirectoryName(infoAttachmentFullPath)).ToString();
            if (Directory.Exists(infoDirectory))
            {
                //删除文件夹及其内容
                //Directory.Delete(infoDirectory, true);
                //如果文件夹为空，删除文件夹
                if (Directory.GetFileSystemEntries(infoDirectory).Length == 0)
                {
                    Directory.Delete(infoDirectory);
                }
            }
            deleteAttachmentFlag = DeleteByID("'" + infoAttachmentId + "'");

            return deleteAttachmentFlag;
        }

        /// <summary>
        /// 删除附件文件及文件夹
        /// </summary>
        /// <param name="infoAttachmentFullPath"></param>
        public void DeleteFile(string infoAttachmentFullPath)
        {
            //删除附件文件
            if (File.Exists(infoAttachmentFullPath))
            {
                File.Delete(infoAttachmentFullPath);
            }
            //删除在线预览文件
            if (infoAttachmentFullPath.EndsWith(".pdf"))
            {
                if (File.Exists(infoAttachmentFullPath + ".swf"))
                {
                    File.Delete(infoAttachmentFullPath + ".swf");
                }
            }
            else
            {
                if (File.Exists(infoAttachmentFullPath + ".html"))
                {
                    File.Delete(infoAttachmentFullPath + ".html");
                }
                if (Directory.Exists(infoAttachmentFullPath + ".files"))
                {
                    Directory.Delete(infoAttachmentFullPath + ".files", true);
                }
            }
            //删除附件文件的文件夹
            String attachmentDirectory = System.IO.Path.GetDirectoryName(infoAttachmentFullPath);
            if (Directory.Exists(attachmentDirectory))
            {
                if (Directory.GetFileSystemEntries(attachmentDirectory).Length == 0)
                {
                    Directory.Delete(attachmentDirectory);
                }
            }
        }

        public int Add(Common.Entity.InfoAttach model)
        {
            return new DAL.InfoAttach().Add(model);
        }

        /// <summary>
        /// 添加附件
        /// </summary>
        /// <param name="infoID"></param>
        /// <param name="addAttachmentList"></param>
        /// <returns></returns>
        public int AddAttachment(int infoID, ArrayList addAttachmentList)
        {
            Common.Entity.InfoAttach infoAttach;
            int addAttachment = 0;
            Hashtable htAttachment;
            String infoAttachmentTempFullPath, infoAttachmentFullPath;
            String infoAttachmentTempPath, infoAttachmentPath;
            for (int i = 0; i < addAttachmentList.Count; i++)
            {
                htAttachment = addAttachmentList[i] as Hashtable;
                infoAttachmentTempFullPath = htAttachment["infoAttachmentFullPath"].ToString();
                infoAttachmentTempPath = System.IO.Path.GetDirectoryName(infoAttachmentTempFullPath);
                infoAttachmentFullPath = infoAttachmentTempFullPath.Replace("UploadTemp", "Upload");
                infoAttachmentPath = infoAttachmentTempPath.Replace("UploadTemp", "Upload");

                //创建正式文件夹，保存附件
                if (!Directory.Exists(infoAttachmentPath))
                {
                    Directory.CreateDirectory(infoAttachmentPath);
                }

                //从临时目录转移附件到正式目录
                if (System.IO.File.Exists(infoAttachmentTempFullPath))
                {
                    //把附件复制（覆盖）到正式目录
                    File.Copy(infoAttachmentTempFullPath, infoAttachmentFullPath, true);
                    //File.Delete(infoAttachmentTempFullPath); //不删除临时文件，节约时间
                }

                //新建附件对象，插入到数据库
                infoAttach = new Common.Entity.InfoAttach();
                //infoAttachment.FN_ID_InfoAttachment = new Guid(this.cboAttachment.Items[i].Value);
                infoAttach.PKID =Convert.ToInt32(htAttachment["infoAttachmentID"]);
                infoAttach.InfoID = infoID;
                infoAttach.AttachName = System.IO.Path.GetFileName(infoAttachmentFullPath);
                infoAttach.AttachSize = int.Parse(htAttachment["infoAttachmentSize"].ToString());
                infoAttach.AttachType = htAttachment["infoAttachmentType"].ToString();
                infoAttach.AttachPath = infoAttachmentFullPath.Replace(uploadPath, "");
                infoAttach.CreateTime = DateTime.Now;
                addAttachment += Add(infoAttach);
            }
            return addAttachment;
        }


    }
}
