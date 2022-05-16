using System;
using InterSystems.Data.IRISClient;
using System.Threading;
using MathNet.Numerics.Statistics;
using System.Text;

namespace ADO
{
    class Program
    {
        static void Main(string[] args)
        {

            String host = "192.168.11.2";
            String port = "1972";
            String username = "_SYSTEM";
            String password = "SYS";
            String Namespace = "USER";

            String errstr;
            int loopcnt = 5000;
            int columncount = 50;
            int sleeptime = 10;
            double[] data = new double[loopcnt];

            if (args.Length >= 1) sleeptime = Int32.Parse(args[0]);
            if (args.Length >= 2) host = args[1];
            if (args.Length >= 3) port = args[2];


            String ConnectionString = "Server = " + host
                + "; Port = " + port + "; Namespace = " + Namespace
                + "; Password = " + password + "; User ID = " + username + "; SharedMemory=false";
            IRISConnection IRISConnect = new IRISConnection();
            IRISConnect.ConnectionString = ConnectionString;

            IRISConnect.Open();

            String tablename = "";

            String sqlStatement = "DROP TABLE TestTable";
            String sqlStatementa = "DROP TABLE TestTable2";
            String sqlStatementb = "DROP TABLE TestTable3";
            String sqlStatement2 = "CREATE INDEX idx1 ON TABLE TestTable (sec)";

            var sqlStatement1 = new StringBuilder();
            tablename = "TestTable"; sqlStatement1.AppendLine($"CREATE TABLE {tablename} (t varchar(50), sec int, p1 int ");
            for (int cnt=2; cnt<= columncount; cnt++) sqlStatement1.Append($",p{cnt} numeric(10,2)");
            sqlStatement1.AppendLine(")");

            var sqlStatement1a = new StringBuilder();
            tablename = "TestTable2"; sqlStatement1a.AppendLine($"CREATE TABLE {tablename} (p1 int ");
            for (int cnt = 2; cnt <= columncount; cnt++) sqlStatement1a.Append($",p{cnt} numeric(10,2)");
            sqlStatement1a.AppendLine(")");

            var sqlStatement1b = new StringBuilder();
            tablename = "TestTable3"; sqlStatement1b.AppendLine($"CREATE TABLE {tablename} (p1 int ");
            for (int cnt = 2; cnt <= columncount; cnt++) sqlStatement1b.Append($",p{cnt} numeric(10,2)");
            sqlStatement1b.AppendLine(")");


            IRISCommand cmd = new IRISCommand(sqlStatement, IRISConnect);
            IRISCommand cmd1 = new IRISCommand(sqlStatement1.ToString(), IRISConnect);
            IRISCommand cmd2 = new IRISCommand(sqlStatement2.ToString(), IRISConnect);
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception e) { errstr=e.ToString(); }
            cmd1.ExecuteNonQuery();
            cmd2.ExecuteNonQuery();


            cmd = new IRISCommand(sqlStatementa, IRISConnect);
            cmd1 = new IRISCommand(sqlStatement1a.ToString(), IRISConnect);
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception e) { errstr = e.ToString(); }
            cmd1.ExecuteNonQuery();

            cmd = new IRISCommand(sqlStatementb, IRISConnect);
            cmd1 = new IRISCommand(sqlStatement1b.ToString(), IRISConnect);
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception e) { errstr = e.ToString(); }
            cmd1.ExecuteNonQuery();



            cmd.Dispose();
            cmd1.Dispose();
            cmd2.Dispose();
            IRISConnect.Close();

            MainJob mainJob = new(ConnectionString, sleeptime);
            for (int r = 0; r < loopcnt; r++)
            {
                mainJob.Exec(data,r);
                if (sleeptime > 0) Thread.Sleep(sleeptime);
                GC.Collect();
            }

            //wait last job to finish
            if (sleeptime > 0) Thread.Sleep(sleeptime);

            Console.WriteLine("件数　　　：{0}", data.Length);
            Console.WriteLine("平均　　　：{0}", data.Mean());
            Console.WriteLine("中央値　　：{0}", data.Median());
            Console.WriteLine("分散　　　：{0}", data.PopulationVariance());
            Console.WriteLine("母分散　　：{0}", data.Variance());
            Console.WriteLine("標準偏差　：{0}", data.PopulationStandardDeviation());
            Console.WriteLine("母標準偏差：{0}", data.StandardDeviation());
            Console.WriteLine("最小　　　：{0}", data.Minimum());
            Console.WriteLine("最大　　　：{0}", data.Maximum());
            Console.WriteLine("Percentile(95%)：{0}", data.Percentile(95));

            Console.WriteLine("Sleep time in ms:" + sleeptime);
            Console.WriteLine(ConnectionString);

        }

    }
}
