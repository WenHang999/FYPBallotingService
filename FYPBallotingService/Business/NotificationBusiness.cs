using System;
using System.Text;
using FYPBallotingService.DataLogic.DataAccess.Common;
using System.IO;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;

namespace FYPBallotingService.Business
{
    class NotificationBusiness
    {
        public void WriteErrLog(string ErrorMessage)
        {
            string FilePath = ConfigurationManager.AppSettings["LogFilePath"] + @"\Notification\";
            string FileName = DateTime.Now.ToString("yyyyMMdd") + ".txt";
            FileStream fs = new FileStream(FilePath + FileName, FileMode.OpenOrCreate, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs);
            sw.BaseStream.Seek(0, SeekOrigin.End);
            sw.WriteLine(ErrorMessage.Trim());
            sw.Flush();
            sw.Close();
        }

        public DataTable GetBookedTable()
        {
            CommonDataLogic objCommonDataLogic = new CommonDataLogic();
            StringBuilder sbrConcat = new StringBuilder();
            try
            {
                sbrConcat.Append("select * from eventTicket a, booking b ,account c where c.uid = b.uid and a.eventCode = b.eventCode and ballotOption='yes'");
                return objCommonDataLogic.ReturnDataTable(sbrConcat.ToString(), "BallotingData");
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                sbrConcat.Remove(0, sbrConcat.Length);
            }
        }

        public DataTable GetResultTable(string ev)
        {
            CommonDataLogic objCommonDataLogic = new CommonDataLogic();
            StringBuilder sbrConcat = new StringBuilder();
            try
            {
                sbrConcat.Append("select * from ballotResult where eventCode = "+ev+"" );
                return objCommonDataLogic.ReturnDataTable(sbrConcat.ToString(), "BallotResult");
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                sbrConcat.Remove(0, sbrConcat.Length);
            }
        }


        public void UpdateBallotResult(string eventCode,int uid, DateTime date, string status)
        {
            CommonDataLogic objCommonDataLogic = new CommonDataLogic();
            SqlTransaction transaction = null;
            SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["fypEntities"].ConnectionString);
            connection.Open();
            SqlCommand command = connection.CreateCommand();
            transaction = connection.BeginTransaction("DbTransaction");
            command.Connection = connection;
            command.Transaction = transaction;
            StringBuilder sbrConcat = new StringBuilder();
            try
            {
                var date2 = date.ToString("dd/MM/yyyy");
                sbrConcat.Append("Insert Into ballotResult (eventCode,uid,date,status) Values (");
                sbrConcat.Append("'");
                sbrConcat.Append(eventCode.ToString());
                sbrConcat.Append("',");
                sbrConcat.Append(uid);
                sbrConcat.Append(",");
                sbrConcat.Append(date2);
                sbrConcat.Append(",'");
                sbrConcat.Append(status);
                sbrConcat.Append("')");
                objCommonDataLogic.ExcequteScalar(ref command, sbrConcat.ToString());
                transaction.Commit();               

            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw ex;
            }
            finally
            {
                connection.Close();
                sbrConcat.Remove(0, sbrConcat.Length);
            }
        }
    }

   
}
