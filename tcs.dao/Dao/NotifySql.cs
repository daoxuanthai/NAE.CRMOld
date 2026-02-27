using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using tcs.bo;
using tcs.lib;

namespace tcs.dao
{
    public class NotifySql : IDbConnect<NotifyBo>
    {
        private static NotifySql _instance;

        public static NotifySql Instance => _instance ?? (_instance = new NotifySql(new SqlConnection(ConfigMgr.ConnectionString)));
        public NotifySql(SqlConnection connection) : base(connection) { }

        public override List<NotifyBo> Select(IQuery query)
        {
            var qr = query as NotifyQuery;
            var ret = new List<NotifyBo>();
            var cmd = Connection.CreateCommand() as SqlCommand;

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "Notify_Search";
            cmd.Parameters.Add(new SqlParameter("UserId", qr.TitleId));
            cmd.Parameters.Add(new SqlParameter("ObjectType", qr.ObjectType));
            cmd.Parameters.Add(new SqlParameter("Type", qr.Type));
            cmd.Parameters.Add(new SqlParameter("IsRead", qr.IsRead ? 1 : -1));
            cmd.Parameters.Add(new SqlParameter("CurrentPage", qr.Page));
            cmd.Parameters.Add(new SqlParameter("RecordPerPage", qr.PageSize));

            SqlParameter outId = new SqlParameter
            {
                ParameterName = "NotRead",
                DbType = DbType.Int32,
                Direction = ParameterDirection.Output
            };
            cmd.Parameters.Add(outId);

            try
            {
                Connection.Open();

                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var tmp = new NotifyBo
                    {
                        Id = SqlHelper.Read<int>(reader, "Id"),
                        TitleId = SqlHelper.Read<int>(reader, "TitleId"),
                        ObjectId = SqlHelper.Read<int>(reader, "ObjectId"),
                        ObjectType = SqlHelper.Read<int>(reader, "ObjectType"),
                        ObjectTypeName = SqlHelper.Read<string>(reader, "ObjectTypeName"),
                        Title = SqlHelper.Read<string>(reader, "Title"),
                        Message = SqlHelper.Read<string>(reader, "Message"),
                        Type = SqlHelper.Read<int>(reader, "Type"),
                        Priority = SqlHelper.Read<int>(reader, "Priority"),
                        CreateUserId = SqlHelper.Read<int>(reader, "CreateUserId"),
                        CreateUserName = SqlHelper.Read<string>(reader, "CreateUserName"),
                        CreateDate = SqlHelper.Read<DateTime>(reader, "CreateDate")
                    };
                    if (qr.TotalRecord <= 0)
                        qr.TotalRecord = SqlHelper.Read<int>(reader, "TotalRecord");

                    ret.Add(tmp);
                }
                
                reader.Close();

                if (outId.Value != null)
                    qr.NotRead = (int)outId.Value;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                cmd.Dispose();
                Connection.Close();
            }

            return ret;
        }

        public override int Create(NotifyBo obj)
        {
            var id = 0;
            var cmd = Connection.CreateCommand() as SqlCommand;

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "Notify_Insert";
            cmd.Parameters.AddWithValue("TitleId", obj.TitleId);
            cmd.Parameters.AddWithValue("ObjectId", obj.ObjectId);
            cmd.Parameters.AddWithValue("ObjectType", obj.ObjectType);
            cmd.Parameters.AddWithValue("ObjectTypeName", obj.ObjectTypeName);
            cmd.Parameters.AddWithValue("Title", obj.Title);
            cmd.Parameters.AddWithValue("Message", obj.Message);
            cmd.Parameters.AddWithValue("IsRead", obj.IsRead);
            cmd.Parameters.AddWithValue("Type", obj.Type);
            cmd.Parameters.AddWithValue("Priority", obj.Priority);
            cmd.Parameters.AddWithValue("CreateUserId", obj.CreateUserId);
            cmd.Parameters.AddWithValue("CreateUserName", obj.CreateUserName);

            SqlParameter outId = new SqlParameter
            {
                ParameterName = "ID",
                DbType = DbType.Int32,
                Direction = ParameterDirection.Output
            };
            cmd.Parameters.Add(outId);

            try
            {
                Connection.Open();
                cmd.ExecuteNonQuery();
                id = (int)outId.Value;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                cmd.Dispose();
                Connection.Close();
            }

            return id;
        }

        public override NotifyBo Read(int id)
        {
            throw new NotImplementedException();
        }

        public override bool Delete(string ids, int companyId, int userId, string userName)
        {
            int num = 0;
            var cmd = Connection.CreateCommand() as SqlCommand;

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "Notify_Delete";
            cmd.Parameters.AddWithValue("Id", ids);
            cmd.Parameters.AddWithValue("CompanyId", companyId);

            try
            {
                Connection.Open();
                num = cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                cmd.Dispose();
                Connection.Close();
            }

            return num > 0;
        }

        public override bool Update(NotifyBo obj)
        {
            throw new NotImplementedException();
        }

        public bool UpdateRead(string ids)
        {
            int num = 0;
            var cmd = Connection.CreateCommand() as SqlCommand;

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "Notify_UpdateRead";
            cmd.Parameters.AddWithValue("Ids", ids);

            try
            {
                Connection.Open();
                num = cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                cmd.Dispose();
                Connection.Close();
            }

            return num > 0;
        }

    }
}

