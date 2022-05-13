﻿using System;
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
        DateTime t;


        private void HeavyMethod(double[] data,int seq)
        {

            String tablename = "";
            long reccnt;


            IRISConnection IRISConnect = new IRISConnection();
            IRISConnect.ConnectionString = connstr;
            IRISConnect.Open();


            var sqlInsert = new StringBuilder();
            tablename = "People"; sqlInsert.AppendLine($"INSERT INTO {tablename} VALUES (?,?,?");
            for (int cnt = 2; cnt <= columncount; cnt++) sqlInsert.Append(",?");
            sqlInsert.AppendLine(")");

            var sqlInsert2 = new StringBuilder();
            tablename = "People2"; sqlInsert2.AppendLine($"INSERT INTO {tablename} VALUES (?");
            for (int cnt = 2; cnt <= columncount; cnt++) sqlInsert2.Append(",?");
            sqlInsert2.AppendLine(")");

            var sqlInsert3 = new StringBuilder();
            tablename = "People3"; sqlInsert3.AppendLine($"INSERT INTO {tablename} VALUES (?");
            for (int cnt = 2; cnt <= columncount; cnt++) sqlInsert3.Append(",?");
            sqlInsert3.AppendLine(")");

            String queryString = "SELECT count(*) FROM People";
            //String queryString = "SELECT count(*) FROM People where ID=1";



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
            cmdInsert.Parameters.Clear();
            cmdInsert.Parameters.Add("@t", System.Data.SqlDbType.Int).Value = t;
            cmdInsert.Parameters.Add("@p0", System.Data.SqlDbType.Int).Value = (int)(t.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            cmdInsert.Parameters.Add("@p1", System.Data.SqlDbType.Int).Value = seq;
            for (int cnt = 2; cnt <= columncount; cnt++) { cmdInsert.Parameters.Add($"@p{cnt}", System.Data.SqlDbType.Float).Value = seq * 0.1; }
            cmdInsert.ExecuteNonQuery();

            sw.Stop();
            ms = 1000.0 * sw.ElapsedTicks / Stopwatch.Frequency;
            Console.WriteLine(timestampstring + " " + sw.ElapsedTicks + " " + Stopwatch.Frequency + " " + ms);
            data[seq] = ms;

            cmdInsert2.Parameters.Clear();
            cmdInsert2.Parameters.Add("@p1", System.Data.SqlDbType.Int).Value = seq;
            for (int cnt = 2; cnt <= columncount; cnt++) { cmdInsert2.Parameters.Add($"@p{cnt}", System.Data.SqlDbType.Float).Value = seq * 0.1; }
            cmdInsert2.ExecuteNonQuery();


            //ExecuteReader() is used for SELECT
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


            //GC.Collect();

            cmdInsert.Dispose();
            cmdInsert2.Dispose();
            cmdInsert3.Dispose();
            cmdQuery.Dispose();

            IRISConnect.Close();
            IRISConnect.Dispose();


        }

        public void Exec(double[] data,int seq)
        {
            Task.Run(() => HeavyMethod(data,seq));
        }

       public MainJob(String connstr,int sleeptime)
        {
            this.connstr = connstr;
            this.sleeptime = sleeptime;
        }


    }
}
