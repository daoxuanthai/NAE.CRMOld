using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using tcs.bo;
using tcs.lib;

namespace tcs.dao
{
    public class CommentSql : IDbConnect<CommentBo>
    {
        private static CommentSql _instance;

        public static CommentSql Instance => _instance ?? (_instance = new CommentSql(new SqlConnection(ConfigMgr.ConnectionString)));
        public CommentSql(SqlConnection connection) : base(connection) { }

        public override List<CommentBo> Select(IQuery query)
        {
            throw new NotImplementedException();
        }

        public override int Create(CommentBo obj)
        {
            var id = 0;
            var cmd = Connection.CreateCommand() as SqlCommand;

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "Comment_Insert";
            cmd.Parameters.AddWithValue("CompanyId", obj.CompanyId);
            cmd.Parameters.AddWithValue("CustomerId", obj.CustomerId);
            cmd.Parameters.AddWithValue("ObjectId", obj.ObjectId);
            cmd.Parameters.AddWithValue("ObjectType", obj.ObjectType);
            cmd.Parameters.AddWithValue("Title", obj.Title);
            cmd.Parameters.AddWithValue("Message", obj.Message);
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

        public override CommentBo Read(int id)
        {
            throw new NotImplementedException();
        }

        public override bool Delete(string ids, int companyId, int userId, string userName)
        {
            int num = 0;
            var cmd = Connection.CreateCommand() as SqlCommand;

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "Comment_Delete";
            cmd.Parameters.AddWithValue("Id", ids);

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

        public override bool Update(CommentBo obj)
        {
            throw new NotImplementedException();
        }

        public List<CommentBo> GetComment(int customerId)
        {
            var ret = new List<CommentBo>();
            var cmd = Connection.CreateCommand() as SqlCommand;

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "Comment_Search";
            cmd.Parameters.Add(new SqlParameter("CustomerId", customerId));

            try
            {
                Connection.Open();

                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var tmp = new CommentBo
                    {
                        Id = SqlHelper.Read<int>(reader, "Id"),
                        CustomerId = SqlHelper.Read<int>(reader, "CustomerId"),
                        ObjectId = SqlHelper.Read<int>(reader, "ObjectId"),
                        ObjectType = SqlHelper.Read<int>(reader, "ObjectType"),
                        Title = SqlHelper.Read<string>(reader, "Title"),
                        Message = SqlHelper.Read<string>(reader, "Message"),
                        CreateUserId = SqlHelper.Read<int>(reader, "CreateUserId"),
                        CreateUserName = SqlHelper.Read<string>(reader, "CreateUserName"),
                        CreateDate = SqlHelper.Read<DateTime>(reader, "CreateDate")
                    };

                    ret.Add(tmp);
                }

                reader.Close();
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

    }
}

