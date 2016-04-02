using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using esscStandard;
using System.Data;
using System.Data.SqlClient;
using System.Security.Principal;

namespace ConsoleTest
{
    class Program
    {
        List<string> directReports = new List<string>();
        

        static void Main(string[] args)
        {
            //esUser me = esUser.Create(WindowsIdentity.GetCurrent());
            
 //           var _connection = new Connectors(new SqlConnection("Data Source=VNP8005;Initial Catalog=NW.CA.ESSC;Persist Security Info=True;Trusted_Connection=True"),"PRODUCTION");

 //           DBMSError error = DBMSError.Create();
 //           DBMSClass data = new DBMSClass(error, _connection);

 //           NPAWorksOrganizationChart orgChart = new NPAWorksOrganizationChart(data);

            // Lets try recursing through the org chart in memory....

 //           ListAllSubordinates(orgChart.ChiefClinicalOfficer);
            
 //           Console.ReadLine();
        }

        private static void ListAllSubordinates()
        {
 //           Console.WriteLine(String.Format("{0}: {1} has {2} direct reports", supervisor.JobCode, supervisor.FullName, supervisor.Subordinates.Count));

  //          foreach (NPAWorksEmployee elem in supervisor.Subordinates)
 //           {
 //               Console.WriteLine(String.Format("      {0}: {1}", elem.JobTitle, elem.FullName));
  //          }
 //           Console.WriteLine("<--------------------------------------------------------->");

 //           foreach (NPAWorksEmployee elem in supervisor.Subordinates)
 //               ListAllSubordinates(elem);
//
        }

        void oldmethods()
        {
            //            List<String> Interventionists;

            //            DBMSError error = new DBMSError();

            // Test Holiday Date Math
            //            NPAWorksHolidays holiday = new esscStandard.NPAWorksHolidays(_connection, error);

            //            DateTime FromDate = DateTime.Now;
            //            int AddDays = 40;



            //            Console.WriteLine(String.Format("{0:MM/dd/yyyy}", FromDate));

            //              DateTime plusfortydate = holiday.AsWorkingDaysFromDate(FromDate, AddDays);

            //            Console.WriteLine(String.Format("{0:MM/dd/yyyy}",plusfortydate));



            //            KaiserAdministrativeAreas KPAdmins = new KaiserAdministrativeAreas(error, _connection);


            //            esscStandard.BehavioralStaffList.Repository.LDAPSnapshot(_connection,error) ;
            //
            //            List<string> ClinicialSupervisors = new List<string>();

            //            esUser alsome = esUser.Create(WindowsIdentity.GetCurrent());
            //            esUser me = esUser.Create("Lourdes.Carranza");

            //            me.GetDirectReports(me.LoginName);

            //            Console.WriteLine(String.Format("My Manager is: {0}", me.Manager));
            //            foreach (esUser sub in me.Subordinates)
            //            {
            //               Console.WriteLine(String.Format("Direct Report: {0} {1}",sub.LoginName, sub.UserSID));
            //           }

            //           Console.WriteLine("");

            //            foreach (esUser sub in me.Subordinates)
            //            {
            //               esUser x = me.SubordinateBySID(sub.UserSID);

            //              Console.WriteLine(String.Format("Other way {0} {1}",x.UserSID, x.LoginName));
            //          }


            //            DBMSError error = new DBMSError();
            //            KaiserAdministrativeAreas KPAdmins = new KaiserAdministrativeAreas(error);

            //           ClinicialSupervisors = me.UsersByKPAreaTitle("Program Manager", KPAdmins);

            //           foreach (string e in ClinicialSupervisors)
            //           {
            //               Console.WriteLine(String.Format("{0}",e));
            //          }


        }

    }
}
