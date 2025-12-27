
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using PEMS.Web.DAL.Interfaces;
using PEMS.Web.Models;

namespace PEMS.Web.DAL.Sql
{
    public class SqlCategoryRepository : ICategoryRepository
    {
        private readonly string _conn = ConfigurationManager.ConnectionStrings["PEMSDb"].ConnectionString;

        // Show user-owned + global (UserId IS NULL)
        public IEnumerable<Category> GetAll(string userId)
        {
            var list = new List<Category>();
            using (var cn = new SqlConnection(_conn))
            using (var cmd = new SqlCommand(
                @"SELECT CategoryId, Name, CategoryType, IsActive, UserId
                  FROM dbo.Categories
                  WHERE UserId = @UserId OR UserId IS NULL
                  ORDER BY Name;", cn))
            {
                cmd.Parameters.AddWithValue("@UserId", (object)userId ?? DBNull.Value);
                cn.Open();
                using (var r = cmd.ExecuteReader())
                {
                    while (r.Read())
                    {
                        list.Add(new Category
                        {
                            CategoryId = r.GetInt32(0),
                            Name = r.GetString(1),
                            CategoryType = r.GetString(2),
                            IsActive = r.GetBoolean(3),
                            UserId = r.IsDBNull(4) ? null : r.GetString(4)
                        });
                    }
                }
            }
            return list;
        }

        // Read one: owned or global
        public Category GetById(string userId, int id)
        {
            using (var cn = new SqlConnection(_conn))
            using (var cmd = new SqlCommand(
                @"SELECT CategoryId, Name, CategoryType, IsActive, UserId
                  FROM dbo.Categories
                  WHERE CategoryId = @Id
                    AND (UserId = @UserId OR UserId IS NULL);", cn))
            {
                cmd.Parameters.AddWithValue("@Id", id);
                cmd.Parameters.AddWithValue("@UserId", (object)userId ?? DBNull.Value);
                cn.Open();
                using (var r = cmd.ExecuteReader())
                {
                    if (!r.Read()) return null;
                    return new Category
                    {
                        CategoryId = r.GetInt32(0),
                        Name = r.GetString(1),
                        CategoryType = r.GetString(2),
                        IsActive = r.GetBoolean(3),
                        UserId = r.IsDBNull(4) ? null : r.GetString(4)
                    };
                }
            }
        }

        // Create user-owned categories (UserId required)
        public int Create(Category c)
        {
            using (var cn = new SqlConnection(_conn))
            using (var cmd = new SqlCommand(
                @"INSERT INTO dbo.Categories (Name, CategoryType, IsActive, UserId)
                  OUTPUT INSERTED.CategoryId
                  VALUES (@Name, @Type, @IsActive, @UserId);", cn))
            {
                cmd.Parameters.AddWithValue("@Name", c.Name);
                cmd.Parameters.AddWithValue("@Type", c.CategoryType);
                cmd.Parameters.AddWithValue("@IsActive", c.IsActive);
                cmd.Parameters.AddWithValue("@UserId", (object)c.UserId ?? DBNull.Value);
                cn.Open();
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        // Update only if owned by user (global categories are read-only)
        public bool Update(Category c)
        {
            using (var cn = new SqlConnection(_conn))
            using (var cmd = new SqlCommand(
                @"UPDATE dbo.Categories
                      SET Name = @Name,
                          CategoryType = @Type,
                          IsActive = @IsActive
                  WHERE CategoryId = @Id
                    AND UserId = @UserId;", cn))
            {
                cmd.Parameters.AddWithValue("@Id", c.CategoryId);
                cmd.Parameters.AddWithValue("@Name", c.Name);
                cmd.Parameters.AddWithValue("@Type", c.CategoryType);
                cmd.Parameters.AddWithValue("@IsActive", c.IsActive);
                cmd.Parameters.AddWithValue("@UserId", (object)c.UserId ?? DBNull.Value);
                cn.Open();
                return cmd.ExecuteNonQuery() == 1;
            }
        }

        // Delete only if owned by user (global cannot be deleted)
        public bool Delete(string userId, int id)
        {
            using (var cn = new SqlConnection(_conn))
            using (var cmd = new SqlCommand(
                @"DELETE FROM dbo.Categories
                  WHERE CategoryId = @Id
                    AND UserId = @UserId;", cn))
            {
                cmd.Parameters.AddWithValue("@Id", id);
                cmd.Parameters.AddWithValue("@UserId", (object)userId ?? DBNull.Value);
                cn.Open();
                return cmd.ExecuteNonQuery() == 1;
            }
        }

        // Duplicate check within user's categories
        public bool ExistsByName(string userId, string name, int? excludeId = null)
        {
            using (var cn = new SqlConnection(_conn))
            using (var cmd = new SqlCommand(
                @"SELECT COUNT(1)
                  FROM dbo.Categories
                  WHERE UserId = @UserId AND Name = @Name
                    AND (@ExcludeId IS NULL OR CategoryId <> @ExcludeId);", cn))
            {
                cmd.Parameters.AddWithValue("@UserId", (object)userId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Name", name);
                cmd.Parameters.AddWithValue("@ExcludeId", (object)excludeId ?? DBNull.Value);
                cn.Open();
                return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
            }
        }
    }
}
