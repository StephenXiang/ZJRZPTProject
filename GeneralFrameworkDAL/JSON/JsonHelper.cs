using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.IO;
using System.Data;

namespace GeneralFrameworkDAL.JSON
{
    public class JsonHelper
    {
        /// <summary>
        /// 将对象序列化为JSON格式
        /// </summary>
        /// <param name="o">对象</param>
        /// <returns>json字符串</returns>
        public static string SerializeObject(object o)
        {
            string json = JsonConvert.SerializeObject(o);
            return json;
        }

        /// <summary>
        /// 解析JSON字符串生成对象实体
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="json">json字符串(eg.{"ID":"112","Name":"石子儿"})</param>
        /// <returns>对象实体</returns>
        public static T DeserializeJsonToObject<T>(string json) where T : class
        {
            JsonSerializer serializer = new JsonSerializer();
            StringReader sr = new StringReader(json);
            object o = serializer.Deserialize(new JsonTextReader(sr), typeof(T));
            T t = o as T;
            return t;
        }

        /// <summary>
        /// 解析JSON数组生成对象实体集合
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="json">json数组字符串(eg.[{"ID":"112","Name":"石子儿"}])</param>
        /// <returns>对象实体集合</returns>
        public static List<T> DeserializeJsonToList<T>(string json) where T : class
        {
            JsonSerializer serializer = new JsonSerializer();
            StringReader sr = new StringReader(json);
            object o = serializer.Deserialize(new JsonTextReader(sr), typeof(List<T>));
            List<T> list = o as List<T>;
            return list;
        }

        /// <summary>
        /// 反序列化JSON到给定的匿名对象.
        /// </summary>
        /// <typeparam name="T">匿名对象类型</typeparam>
        /// <param name="json">json字符串</param>
        /// <param name="anonymousTypeObject">匿名对象</param>
        /// <returns>匿名对象</returns>
        public static T DeserializeAnonymousType<T>(string json, T anonymousTypeObject)
        {
            T t = JsonConvert.DeserializeAnonymousType(json, anonymousTypeObject);
            return t;
        }

        /// <summary>
        /// 将DATATABLE转换成树形菜单
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string GetMenuJson(DataTable dt)
        {
            string json = "[{";
            foreach (DataRow dr in dt.Rows)
            {
                if (int.Parse(dr[3].ToString()) == 0)
                {
                    json += "\"text\":\"" + dr[1].ToString() + "\",\"children\":[ ";
                }
                else if (int.Parse(dr[3].ToString()) > 0)
                {
                    json += "{\"text\":\"" + dr[1].ToString() + "\",\"attributes\": { \"url\": \"" + dr[2].ToString() + "\"} },";
                }
            }
            json = json.Remove(json.ToString().LastIndexOf(','), 1);
            json += "]}]";
            return json;
        }

        public static string GetSysDepartmentJson(DataTable Roledt, DataTable Departmentdt)
        {
            string json = "[";
            foreach (DataRow Roledr in Roledt.Rows)
            {
                DataRow[] DmArrdr = Departmentdt.Select("RoleID='" + Roledr[0].ToString() + "'");
                if (DmArrdr.Length > 0)
                {
                    json += "{\"text\":\"" + Roledr[1].ToString() + "\",\"children\":[ ";
                    foreach (DataRow Dmdr in DmArrdr)
                    {
                        json += "{\"text\":\"" + Dmdr[1].ToString() + "\",\"attributes\": { \"ID\": \"" + Dmdr[0].ToString() + "\"} },";
                    }
                    json = json.Remove(json.ToString().LastIndexOf(','), 1);
                    json += "]},";
                }
            }
            json = json.Remove(json.ToString().LastIndexOf(','), 1);
            json += "]";
            return json;
        }

        public static string ConvertJosnData(DataTable p_dt)
        {
            string str = "[";
            for (int i = 0; i < p_dt.Rows.Count; i++)
            {
                str = str + "{";
                for (int j = 0; j < p_dt.Columns.Count; j++)
                {
                    str = str + "\"" + p_dt.Columns[j].ToString() + "\":\"" + p_dt.Rows[i][p_dt.Columns[j].ToString()].ToString().Trim() + "\"" + ",";
                }
                if (str.Length > 0)
                {
                    str = str.Substring(0, str.Length - 1);
                }
                str = str + "},";
            }
            if (p_dt.Rows.Count > 0)
            {
                str = str.Substring(0, str.Length - 1);
            }
            str = str + "]";
            return str;
        }
        /// <summary>
        /// DataTable转JSON
        /// </summary>
        /// <param name="count">记录数</param>
        /// <param name="table">table</param>
        /// <returns></returns>
        public static string TableToJson(int count, DataTable table)
        {
            if (table.Rows.Count > 0)
            {
                string strJson = "";
                strJson = "{\"total\":" + count + ",\"rows\":[";
                try
                {
                    foreach (DataRow dr in table.Rows)
                    {
                        strJson += "{";
                        for (int c = 0; c < table.Columns.Count; c++)
                        {
                            strJson += "\"" + table.Columns[c].ColumnName + "\":\"" + dr[c].ToString().Trim().Replace("\n", "") + "\",";
                        }
                        strJson = strJson.Substring(0, strJson.Length - 1);
                        strJson += "},";
                    }

                }
                catch
                {
                    strJson = strJson.Substring(0, strJson.Length - 1);
                }
                strJson = strJson.Substring(0, strJson.Length - 1);
                strJson += "]}";
                return strJson;
            }
            else
            {
                return "{\"total\":0,\"rows\":[]}";
            }
        }

        /// <summary>
        /// DATATABLE分页
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="PageIndex">PageIndex表示第几页</param>
        /// <param name="PageSize">PageSize表示每页的记录数</param>
        /// <returns></returns>
        public static DataTable GetPagedTable(DataTable dt, int PageIndex, int PageSize)
        {
            if (PageIndex == 0)
                return dt;//0页代表每页数据，直接返回

            DataTable newdt = dt.Copy();
            newdt.Clear();//copy dt的框架

            int rowbegin = (PageIndex - 1) * PageSize;
            int rowend = PageIndex * PageSize;

            if (rowbegin >= dt.Rows.Count)
                return newdt;//源数据记录数小于等于要显示的记录，直接返回dt

            if (rowend > dt.Rows.Count)
                rowend = dt.Rows.Count;
            for (int i = rowbegin; i <= rowend - 1; i++)
            {
                DataRow newdr = newdt.NewRow();
                DataRow dr = dt.Rows[i];
                foreach (DataColumn column in dt.Columns)
                {
                    newdr[column.ColumnName] = dr[column.ColumnName];
                }
                newdt.Rows.Add(newdr);
            }
            return newdt;
        }
    }
}
