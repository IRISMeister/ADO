using System;
using InterSystems.Data.IRISClient;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Text;
using System.Threading;

namespace ADO
{
    class CreateManyTables
    {
        private String connstr = null;
        private int columncount = 10;
        private int sleeptime = 100;
        private int loopcnt = 5000;


        private void AccessIRIS(double[] data,int seq)
        {

            String tablename = "";
            String errstr = "";

            IRISCommand cmd1=null;
            IRISConnection IRISConnect = new IRISConnection();
            IRISConnect.ConnectionString = connstr;
            IRISConnect.Open();

            String sqlStatement0 = null;
            var sqlInsert = new StringBuilder();
            var sqlStatement1 = new StringBuilder();
            for (seq = 0; seq < 1000; seq++)
            {
                tablename = "DummyTestTable" + seq;
                sqlStatement0 = $"DROP TABLE {tablename}";
                sqlStatement1.Clear();
                sqlStatement1.AppendLine($"CREATE TABLE {tablename} (t varchar(50), sec int, p1 int ");
                for (int cnt = 2; cnt <= columncount; cnt++) sqlStatement1.Append($",p{cnt} numeric(10,2)");
                sqlStatement1.AppendLine(")");

                cmd1 = new IRISCommand(sqlStatement0, IRISConnect);
                try
                {
                    cmd1.ExecuteNonQuery();
                }
                catch (Exception e) { errstr = e.ToString(); }



                cmd1 = new IRISCommand(sqlStatement1.ToString(), IRISConnect);
                cmd1.ExecuteNonQuery();

                sqlInsert.Clear();
                sqlInsert.AppendLine("SELECT t , sec , p1 ");
                for (int cnt = 2; cnt <= columncount; cnt++) sqlInsert.Append($",p{cnt}");
                sqlInsert.AppendLine($"  FROM {tablename}");
                cmd1 = new IRISCommand(sqlInsert.ToString(), IRISConnect);
                cmd1.ExecuteNonQuery();

            }



            IRISConnect.Close();
            IRISConnect.Dispose();

        }

        public void ExecSync(double[] data,int seq)
        {
            Task t=Task.Run(() => AccessIRIS(data,seq));
            t.Wait();
        }

        public CreateManyTables(String connstr, int sleeptime, int loopcnt)
        {
            this.connstr = connstr;
            this.sleeptime = sleeptime;
            this.loopcnt = loopcnt;
        }
    }
}
