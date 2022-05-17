using System;
using InterSystems.Data.IRISClient;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Text;

namespace ADO
{
    class MainJob
    {
        private String connstr = null;
        private int columncount = 50;
        private int sleeptime = 10;

        private void AccessIRIS(double[] data,int seq)
        {

            String tablename = "";
            long reccnt;
            DateTime t;
            int totalSeconds;

            IRISConnection IRISConnect = new IRISConnection();
            IRISConnect.ConnectionString = connstr;
            IRISConnect.Open();

            var sqlInsert = new StringBuilder();
            tablename = "TestTable"; sqlInsert.AppendLine($"INSERT INTO {tablename} VALUES (?,?,?");
            for (int cnt = 2; cnt <= columncount; cnt++) sqlInsert.Append(",?");
            sqlInsert.AppendLine(")");

            var sqlInsert2 = new StringBuilder();
            tablename = "TestTable2"; sqlInsert2.AppendLine($"INSERT INTO {tablename} VALUES (?");
            for (int cnt = 2; cnt <= columncount; cnt++) sqlInsert2.Append(",?");
            sqlInsert2.AppendLine(")");

            var sqlInsert3 = new StringBuilder();
            tablename = "TestTable3"; sqlInsert3.AppendLine($"INSERT INTO {tablename} VALUES (?");
            for (int cnt = 2; cnt <= columncount; cnt++) sqlInsert3.Append(",?");
            sqlInsert3.AppendLine(")");

            String queryString = "SELECT count(*) FROM TestTable where sec>=? and sec<?";

            IRISCommand cmdInsert = new IRISCommand(sqlInsert.ToString(), IRISConnect);
            cmdInsert.Prepare();
            IRISCommand cmdInsert2 = new IRISCommand(sqlInsert2.ToString(), IRISConnect);
            cmdInsert2.Prepare();
            IRISCommand cmdInsert3 = new IRISCommand(sqlInsert3.ToString(), IRISConnect);
            cmdInsert3.Prepare();
            IRISCommand cmdQuery = new IRISCommand(queryString, IRISConnect);
            cmdQuery.Prepare();

            var sw = new Stopwatch();
            double ms;
            String timestampstring;

            sw.Reset();
            sw.Start();

            t = DateTime.Now;
            timestampstring = String.Format("{0:HH:mm:ss.fff}", t);
            totalSeconds = (int)(t.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            cmdInsert.Parameters.Clear();
            cmdInsert.Parameters.Add("@t", System.Data.SqlDbType.Int).Value = t;
            cmdInsert.Parameters.Add("@sec", System.Data.SqlDbType.Int).Value = totalSeconds;
            cmdInsert.Parameters.Add("@p1", System.Data.SqlDbType.Int).Value = seq;
            for (int cnt = 2; cnt <= columncount; cnt++) { cmdInsert.Parameters.Add($"@p{cnt}", System.Data.SqlDbType.Float).Value = seq * 0.1; }
            cmdInsert.ExecuteNonQuery();

            // Should check SQLCODE or status here....

            sw.Stop();
            ms = 1000.0 * sw.ElapsedTicks / Stopwatch.Frequency;
            Console.WriteLine(timestampstring + " " + sw.ElapsedTicks + " " + Stopwatch.Frequency + " " + ms);
            data[seq] = ms;

            cmdInsert2.Parameters.Clear();
            cmdInsert2.Parameters.Add("@p1", System.Data.SqlDbType.Int).Value = seq;
            for (int cnt = 2; cnt <= columncount; cnt++) { cmdInsert2.Parameters.Add($"@p{cnt}", System.Data.SqlDbType.Float).Value = seq * 0.1; }
            cmdInsert2.ExecuteNonQuery();


            //ExecuteReader() is used for SELECT
            cmdQuery.Parameters.Clear();
            cmdQuery.Parameters.Add("@p1", System.Data.SqlDbType.Int).Value = totalSeconds - 1; 
            cmdQuery.Parameters.Add("@p2", System.Data.SqlDbType.Int).Value = totalSeconds ;
            IRISDataReader Reader = cmdQuery.ExecuteReader();

            while (Reader.Read())
            {
                reccnt = Reader.GetInt64(0);
            }

            Reader.Close();

            cmdInsert3.Parameters.Clear();
            cmdInsert3.Parameters.Add("@p1", System.Data.SqlDbType.Int).Value = seq;
            for (int cnt = 2; cnt <= columncount; cnt++) { cmdInsert3.Parameters.Add($"@p{cnt}", System.Data.SqlDbType.Float).Value = seq * 0.1; }
            cmdInsert3.ExecuteNonQuery();

            cmdInsert.Dispose();
            cmdInsert2.Dispose();
            cmdInsert3.Dispose();
            cmdQuery.Dispose();

            IRISConnect.Close();
            IRISConnect.Dispose();

        }

        public void ExecSync(double[] data,int seq)
        {
            Task t=Task.Run(() => AccessIRIS(data,seq));
            t.Wait();
        }
        public void Exec(double[] data, int seq)
        {
            Task.Run(() => AccessIRIS(data, seq));
        }

        public MainJob(String connstr,int sleeptime)
        {
            this.connstr = connstr;
            this.sleeptime = sleeptime;
        }
    }
}
