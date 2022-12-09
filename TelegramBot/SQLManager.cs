using System.Configuration;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;

namespace TelegramBot
{
#pragma warning disable 4014 // Allow for bot.SendChatAction to not be awaited
    // ReSharper disable FunctionNeverReturns
#pragma warning disable RECS0022 // A catch clause that catches System.Exception and has an empty body
    // ReSharper disable CatchAllClause
    internal static class SQLManager
    {
        internal async static Task<bool> IsUserInDB(long id)
        {
            using (SqlConnection sql = new SqlConnection(ConfigurationManager.AppSettings["SQLKey"]))
            {
                sql.Open();
                SqlCommand command = new SqlCommand($"select * from [dbo].[Users] where id='{id}'", sql);
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    return reader.Read();
                }
            }
        }
        internal async static Task<bool> IsCourseInDB(string name, string link)
        {
            using (SqlConnection sql = new SqlConnection(ConfigurationManager.AppSettings["SQLKey"]))
            {
                sql.Open();
                SqlCommand command = new SqlCommand($"select * from [dbo].[Courses] where Name='{name}' AND Link='{link}'", sql);
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    return reader.Read();
                }
            }
        }
        internal async static void AddNewUser(long id)
        {
            using (SqlConnection sql = new SqlConnection(ConfigurationManager.AppSettings["SQLKey"]))
            {
                sql.Open();
                SqlCommand command = new SqlCommand($"insert into [dbo].[Users] values ('{id}', ' ')", sql);
                await command.ExecuteNonQueryAsync();
            }
        }
        internal async static void UpdateTag(long id, string newTag)
        {
            using (SqlConnection sql = new SqlConnection(ConfigurationManager.AppSettings["SQLKey"]))
            {
                sql.Open();
                SqlCommand command = new SqlCommand($"update [dbo].[Users] set Tags='{newTag}' where Id='{id}'", sql);
                await command.ExecuteNonQueryAsync();
            }
        }
        internal async static void CLearTagList(long id)
        {
            using (SqlConnection sql = new SqlConnection(ConfigurationManager.AppSettings["SQLKey"]))
            {
                sql.Open();
                SqlCommand command = new SqlCommand($"update [dbo].[Users] set Tags='NULL' where Id='{id}'", sql);
                await command.ExecuteNonQueryAsync();
            }
        }
        internal async static Task<string> GetTagById(long id)
        {
            using (SqlConnection sql = new SqlConnection(ConfigurationManager.AppSettings["SQLKey"]))
            {
                sql.Open();
                SqlCommand command = new SqlCommand($"select * from [dbo].[Users] where id='{id}'", sql);
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    reader.Read();
                    return reader["Tags"].ToString();
                }
            }
        }
        internal static void AddCourses(List<CourseInfo> courses)
        {
            using (SqlConnection sql = new SqlConnection(ConfigurationManager.AppSettings["SQLKey"]))
            {
                sql.Open();
                foreach (var item in courses)
                {

                    SqlCommand command = new SqlCommand($"insert into [dbo].[Courses] values (N'{item.Link.ToLower()}', N'{item.Name.ToLower().Replace("\'", "")}', N'{item.Discr.Replace("\'", "")}')", sql);
                    command.ExecuteNonQuery();
                }
            }
        }
        internal async static void ClearAllWritesInTable(string tableName)
        {
            using (SqlConnection sql = new SqlConnection(ConfigurationManager.AppSettings["SQLKey"]))
            {
                sql.Open();
                SqlCommand command = new SqlCommand($"delete from {tableName}", sql);
                await command.ExecuteNonQueryAsync();
            }
        }
        internal async static Task<List<CourseInfo>> FindAllCoursesByTag(string tag)
        {
            List<CourseInfo> courses = new List<CourseInfo>();
            using (SqlConnection sql = new SqlConnection(ConfigurationManager.AppSettings["SQLKey"]))
            {
                sql.Open();
                SqlCommand command = new SqlCommand($"select * from [dbo].[Courses] where Name like '%{tag}%'", sql);
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (reader.ReadAsync().Result)
                    {
                        courses.Add(new CourseInfo($"{reader["Name"]}", $"{reader["Info"]}", $"{reader["Link"]}"));
                    }
                }
            }
            return courses;
        }
        internal async static Task<List<botUser>> GetAllUsers()
        {
            List<botUser> users = new List<botUser>();
            using (SqlConnection sql = new SqlConnection(ConfigurationManager.AppSettings["SQLKey"]))
            {
                sql.Open();
                SqlCommand command = new SqlCommand($"select * from [dbo].[Users]", sql);
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (reader.ReadAsync().Result)
                    {
                        users.Add(new botUser(Convert.ToInt64(reader["Id"]), $"{reader["Info"]}"));
                    }
                }
            }
            return users;
        }
    }
}