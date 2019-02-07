using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace FYPBallotingService.DataLogic.DataAccess.Common
{
    class CommonDataLogic
    {

        public DataTable ReturnDataTable(string strquery, string strTableName = null)
        {
            DataTable dtQueryResult = new DataTable();
            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["fypEntities"].ConnectionString);
            try
            {
                SqlCommand cmm = new SqlCommand(strquery, conn);
                conn.Open();
                SqlDataAdapter da = new SqlDataAdapter(cmm);
                da.Fill(dtQueryResult);
                if (strTableName != null)
                {
                    dtQueryResult.TableName = strTableName;
                }
                return dtQueryResult;
            }
            catch (Exception ex)
            {
                dtQueryResult.Dispose();
                throw ex;
            }
            finally
            {
                conn.Close();
            }
        }

        public DataTable ReturnDataTableInExistingConnection(ref SqlCommand command, string strquery, string strTableName = null)
        {
            DataTable dtQueryResult = new DataTable();
            try
            {
                command.CommandText = strquery;
                SqlDataAdapter da = new SqlDataAdapter(command);
                da.Fill(dtQueryResult);
                if (strTableName != null)
                {
                    dtQueryResult.TableName = strTableName;
                }
                return dtQueryResult;
            }
            catch (Exception ex)
            {
                dtQueryResult.Dispose();
                throw ex;
            }
        }

        public string getValueByQuery(string strQuery)
        {
            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["fypEntities"].ConnectionString);
            try
            {
                SqlCommand cmm = new SqlCommand(strQuery, conn);
                conn.Open();
                object objValue = cmm.ExecuteScalar();
                string strValue = "";
                if (objValue != null)
                {
                    strValue = objValue.ToString();
                }
                return strValue;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                conn.Close();
            }
        }

        public string getValueByQueryInExistingConnection(ref SqlCommand command, string strquery, string strTableName = null)
        {
            try
            {
                command.CommandType = CommandType.Text;
                command.CommandText = strquery;
                object objValue = command.ExecuteScalar();
                string strValue = "";
                if (objValue != null)
                {
                    strValue = objValue.ToString();
                }
                return strValue;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void ExcequteScalar(ref SqlCommand command, string strQuery)
        {
            try
            {
                command.CommandType = CommandType.Text;
                command.CommandText = strQuery;
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int getSequenceValue(string strSequenceName)
        {
            try
            {
                SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["fypEntities"].ConnectionString);
                SqlCommand cmm = new SqlCommand("SELECT (NEXT VALUE FOR " + strSequenceName + ")", conn);
                conn.Open();
                int intNextValue = (int)cmm.ExecuteScalar();
                conn.Close();
                return intNextValue;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
