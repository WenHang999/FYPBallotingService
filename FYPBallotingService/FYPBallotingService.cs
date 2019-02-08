using FYPBallotingService.Business;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace FYPBallotingService
{
    public partial class FYPBallotingService : ServiceBase
    {
        System.Timers.Timer timeDelay;

        public void OnServiceStart()
        {
            NotificationBusiness objNotificationBusiness = new NotificationBusiness();
            objNotificationBusiness.WriteErrLog("Service is Started");
            timeDelay.Enabled = true;
        }
        public FYPBallotingService()
        {
            InitializeComponent();
            timeDelay = new System.Timers.Timer();
            //timeDelay.Interval = 300000;//Every 5 min
            timeDelay.Interval = 5000;//Every 30s
            timeDelay.Elapsed += new System.Timers.ElapsedEventHandler(WorkProcess);
        }

        protected override void OnStart(string[] args)
        {
            NotificationBusiness objNotificationBusiness = new NotificationBusiness();
            objNotificationBusiness.WriteErrLog("Service is Started");
            timeDelay.Enabled = true;
        }

        protected override void OnStop()
        {
            NotificationBusiness objNotificationBusiness = new NotificationBusiness();
            objNotificationBusiness.WriteErrLog("Service is Stoped");
            timeDelay.Enabled = false;
        }

        public void WorkProcess(object sender, System.Timers.ElapsedEventArgs e)
        {
            NotificationBusiness objNotificationBusiness = new NotificationBusiness();
            try
            {
                timeDelay.Enabled = false;
                BallotingProcess();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                timeDelay.Enabled = true;
            }
        }
        private void BallotingProcess()
        {
            NotificationBusiness objNotificationBusiness = new NotificationBusiness();
            DataTable GetBookedTable = new DataTable();
            DataTable GetResultTalbe = new DataTable();
            try
            {

                GetBookedTable = objNotificationBusiness.GetBookedTable();
                GetBookedTable = CollectionExtensions.OrderRandomly(GetBookedTable.AsEnumerable()).CopyToDataTable();
                foreach (DataRow row in GetBookedTable.Rows)
                {
                    // start balloting 7 day in advance before the start date
                    DateTime startDate = DateTime.Parse(row["startDate"].ToString());
                    DateTime ballotDay = startDate.AddDays(-7);
                    // get record from record table with eventcode condition
                    GetResultTalbe = objNotificationBusiness.GetResultTable(row["eventCode"].ToString());
                    // count number of quantity and number of booked record
                    int numberOfRecords = GetResultTalbe.AsEnumerable().Where(x => x["eventCode"].ToString() == row["eventCode"].ToString()).ToList().Count;                    
                    int quantity = Int32.Parse(row["quantity"].ToString());
                    // count user uid to prevent duplication
                    int bookedRecord = GetResultTalbe.AsEnumerable().Where(x => x["uid"].ToString() == row["uid"].ToString()).ToList().Count;
                    // < 0 earlier      > 0 later 

                    if (DateTime.Compare(ballotDay, DateTime.Now) < 0 && numberOfRecords < quantity && bookedRecord == 0 )
                    {
                        var status = "approved";                     
                        try
                        {
                            objNotificationBusiness.UpdateBallotResult(row["eventCode"].ToString(), Int32.Parse(row["uid"].ToString()), DateTime.Parse(row["date"].ToString()) , status);
                            //send email upon successfully booked
                            var strSubject = "Success Booking On " + row["eventname"].ToString();
                            var strBody = "you have successfully book event "+ row["eventname"].ToString() +"on"+ row["bookedDate"].ToString();
                            var Email = row["email"].ToString();
                            SendEmail(strSubject, strBody, Email);
                            //send to admin
                            var strAdminSubject = "Booking On " + row["eventname"].ToString();
                            var strAdminBody = row["eventname"].ToString()+"have successfully book event " + row["eventname"].ToString() + "on" + row["bookedDate"].ToString();
                            var adminEmail = "panwh1024@gmail.com";
                            SendEmail(strAdminSubject, strAdminBody, adminEmail);
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                    }
                }


            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void SendEmail(string strSubject, string strBody, string Email)
        {
            try
            {
                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient("smtp.sendgrid.net");
                mail.From = new MailAddress("noreply@cnb.com");
                mail.IsBodyHtml = true;
                mail.To.Add(new MailAddress(Email));
                mail.Subject = strSubject;
                mail.Body = strBody;
                SmtpServer.Port = 587;
                SmtpServer.Credentials = new NetworkCredential("apikey", "your api key here");
                SmtpServer.EnableSsl = true;
                SmtpServer.Send(mail);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



    }


    public static class CollectionExtensions

    {

        private static Random random = new Random();

        public static IEnumerable<T> OrderRandomly<T>(this IEnumerable<T> collection)

        {

            // Order items randomly

            List<T> randomly = new List<T>(collection);

            while (randomly.Count > 0)

            {

                Int32 index = random.Next(randomly.Count);

                yield return randomly[index];

                randomly.RemoveAt(index);

            }

        } // OrderRandomly

    }



}

