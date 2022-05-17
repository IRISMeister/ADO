using System;
using InterSystems.Data.IRISClient;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Text;
using System.Threading;

namespace ADO
{
    class MainJob2
    {
        private String connstr = null;
        private int columncount = 50;
        private int sleeptime = 100;
        private int loopcnt = 5000;


        private void AccessIRIS(double[] data,int seq)
        {

            String tablename = "";
            DateTime t;
            int totalSeconds;


            IRISConnection IRISConnect;
            IRISCommand cmdInsert;


            var sqlInsert = new StringBuilder();
            tablename = "TestTable"; sqlInsert.AppendLine($"INSERT INTO {tablename} VALUES (?,?,?");
            for (int cnt = 2; cnt <= columncount; cnt++) sqlInsert.Append(",?");
            sqlInsert.AppendLine(")");



            var sw = new Stopwatch();
            double ms;
            String timestampstring;

            for (int i = 0; i < loopcnt; i++)
            {
                IRISConnect = new IRISConnection();
                IRISConnect.ConnectionString = connstr;
                IRISConnect.Open();

                sw.Reset();
                sw.Start();
                cmdInsert = new IRISCommand(sqlInsert.ToString(), IRISConnect);
                cmdInsert.Prepare();

                t = DateTime.Now;
                timestampstring = String.Format("{0:HH:mm:ss.fff}", t);
                totalSeconds = (int)(t.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
                cmdInsert.Parameters.Clear();
                cmdInsert.Parameters.Add("@t", System.Data.SqlDbType.Int).Value = t;
                cmdInsert.Parameters.Add("@sec", System.Data.SqlDbType.Int).Value = totalSeconds;
                cmdInsert.Parameters.Add("@p1", System.Data.SqlDbType.Int).Value = seq;
                for (int cnt = 2; cnt <= columncount; cnt++) { cmdInsert.Parameters.Add($"@p{cnt}", System.Data.SqlDbType.Float).Value = seq * 0.1; }
                cmdInsert.ExecuteNonQuery();

                cmdInsert.Dispose();
                // Should check SQLCODE or status here....
                IRISConnect.Close();
                IRISConnect.Dispose();

                sw.Stop();
                ms = 1000.0 * sw.ElapsedTicks / Stopwatch.Frequency;
                Console.WriteLine(timestampstring + " " + sw.ElapsedTicks + " " + Stopwatch.Frequency + " " + ms);
                data[i] = ms;

                Thread.Sleep(sleeptime);

            }

        }

        public void ExecSync(double[] data,int seq)
        {
            Task t=Task.Run(() => AccessIRIS(data,seq));
            t.Wait();
        }

        public MainJob2(String connstr,int sleeptime, int loopcnt)
        {
            this.connstr = connstr;
            this.sleeptime = sleeptime;
            this.loopcnt = loopcnt;
        }
    }
}
