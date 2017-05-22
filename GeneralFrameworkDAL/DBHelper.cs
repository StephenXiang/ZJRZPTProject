using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;

namespace GeneralFrameworkDAL
{
    public class DBHelper
    {
        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        public static string ConStr = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;


        /// <summary>
        /// 获取数据，返回DataReader
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public static SqlDataReader GetDataReader(string sql, params SqlParameter[] param)
        {
            SqlConnection conn = new SqlConnection(ConStr);
            SqlCommand cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddRange(param);
            conn.Open();
            return cmd.ExecuteReader(CommandBehavior.CloseConnection);
        }

        /// <summary>
        /// 执行单行SQL，返回DataTable
        /// </summary>
        /// <param name="safeSql"></param>
        /// <returns></returns>
        public static DataTable GetDataSet(string safeSql)
        {
            using (SqlConnection Connection = new SqlConnection(ConStr))
            {
                DataSet ds = new DataSet();
                SqlCommand cmd = new SqlCommand(safeSql, Connection);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                Connection.Open();
                da.Fill(ds);
                Connection.Close();
                return ds.Tables[0];
            }
        }


        /// <summary>
        /// 执行单行SQL，返回DataSet
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static DataSet GetDataSet2(string sql)
        {
            DataSet ds = new DataSet();
            SqlConnection conn = new SqlConnection(ConStr);
            conn.Open();
            SqlDataAdapter sda = new SqlDataAdapter(sql, conn);
            DataSet ds1 = new DataSet();
            sda.Fill(ds, "tb_FirstStage");
            conn.Close();
            return ds;
        }


        /// <summary>
        /// 获取数据，返回受影响行数
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public static int GetNonQuery(string sql, params SqlParameter[] param)
        {
            using (SqlConnection conn = new SqlConnection(ConStr))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddRange(param);
                    conn.Open();
                    return cmd.ExecuteNonQuery();
                }
                catch (Exception)
                {
                    conn.Close();
                    //conn.Dispose();
                    return 0;
                }

            }
        }


        /// <summary>
        /// 获取数据，返回首行首列
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public static object GetScalar(string sql, params SqlParameter[] param)
        {
            using (SqlConnection conn = new SqlConnection(ConStr))
            {
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddRange(param);
                conn.Open();
                return cmd.ExecuteScalar();
            }
        }


        /// <summary>
        /// 执行存储过程，返回Object
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="outparam"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public static Object ExecuteProc(string sql, string outparam, params SqlParameter[] param)
        {
            using (SqlConnection conn = new SqlConnection(ConStr))
            {
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddRange(param);
                cmd.ExecuteNonQuery();
                return cmd.Parameters[outparam].Value;
            }
        }

        /// <summary>
        /// 执行带参存储过程返回数据库影响行数
        /// </summary>
        /// <param name="procName">存储过程名字</param>
        /// <param name="param">参数列表</param>
        /// <returns>成功:>0/失败:0/异常:-1</returns>
        public int ExecuteProc(string procName, SqlParameter[] param)
        {
            using (SqlConnection conn = new SqlConnection(ConStr))
            {
                try
                {
                    conn.Open();
                    SqlCommand cmd = conn.CreateCommand();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = procName;
                    cmd.Parameters.AddRange(param);
                    return cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    return -1;
                    //throw;
                }
            }
        }


        /// <summary>
        /// 执行单行sql 返回首行首列
        /// </summary>
        /// <param name="str">sql语句 </param>
        /// <returns>返回object</returns>
        public static object GetScalar(string str)
        {
            using (SqlConnection conn = new SqlConnection(ConStr))
            {
                try
                {
                    conn.Open();
                    SqlCommand cmd = conn.CreateCommand();
                    cmd.CommandText = str;
                    return cmd.ExecuteScalar();
                }
                catch
                {
                    return -1;
                    //throw;
                }
            }
        }

        /// <summary>
        /// 执行存储过程返回datatable
        /// </summary>
        /// <param name="procName">存储过程名字</param>
        /// <param name="param">参数列表</param>
        /// <returns>返回DataTable</returns>
        public static DataTable GetDataTable(string procName, SqlParameter[] param)
        {
            using (SqlConnection conn = new SqlConnection(ConStr))
            {
                try
                {
                    conn.Open();
                    SqlCommand cmd = conn.CreateCommand();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = procName;
                    cmd.Parameters.AddRange(param);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataSet ds = new DataSet();
                    da.Fill(ds);
                    return ds.Tables[0];
                }
                catch
                {
                    return null;
                    //throw;
                }
            }
        }


        /// <summary>
        /// 执行单行sql 返回首行首列
        /// </summary>
        /// <param name="str">sql语句 </param>
        /// <returns>返回int</returns>
        public static int GetSingle(string str)
        {
            using (SqlConnection conn = new SqlConnection(ConStr))
            {
                try
                {
                    conn.Open();
                    SqlCommand cmd = conn.CreateCommand();
                    cmd.CommandText = str;
                    return cmd.ExecuteNonQuery();
                }
                catch
                {
                    return -1;
                    //throw;
                }
            }
        }

    }
}
