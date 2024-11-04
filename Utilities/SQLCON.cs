using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;

namespace EMRMS.Utilities
{
    internal class SQLCON
    {
        #region SQL
        public static int ExecuteInsertPost(int UserID, string body, DateTime postTime, List<Users.MediaItem> files)
        {
            string insertPost = "INSERT INTO posts (UserID, postDate, BODY) VALUES (@UserID, @postDate, @postContent)";
            string insertMedia = "INSERT INTO files (PostID, fileType, fileUri) VALUES (@PostID, @FileType, @fileUri)";
            int postId;
            using (var connection = new MySqlConnection(App.sqlConn))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // Insertar el post y obtener su ID

                        using (var command = new MySqlCommand(insertPost, connection, transaction))
                        {
                            command.Parameters.AddWithValue("@UserID", UserID);
                            command.Parameters.AddWithValue("@postDate", postTime);
                            command.Parameters.AddWithValue("@postContent", body);
                            command.ExecuteNonQuery();

                            // Obtener el ID del último post insertado
                            postId = (int)command.LastInsertedId;
                        }

                        // Insertar cada archivo en FILES con el ID de post obtenido
                        foreach (var file in files)
                        {
                            using (var command = new MySqlCommand(insertMedia, connection, transaction))
                            {
                                command.Parameters.AddWithValue("@PostID", postId);
                                command.Parameters.AddWithValue("@FileType", file.FileType);
                                string[] s = { };
                                if (file.FileType == "video")
                                    s = file.VideoPath.Split(@"\");
                                else
                                    s = file.Path.Split(@"\");
                                command.Parameters.AddWithValue("@fileUri", s[s.Length - 1]);
                                command.ExecuteNonQuery();
                            }
                        }

                        transaction.Commit();
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        throw;
                    }
                    finally
                    {
                        connection.Close();

                    }
                }
            }
            return postId;
        }
        public static void ExecuteInsertUser(string nickname, string name, DateTime birthDate, string email, string password)
        {
            string insertUserQuery = "INSERT INTO users (nickname, name, bornDate, Privacity, location) " +
                                     "VALUES (@Nickname, @Name, @BornDate, @Privacity, @Location)";

            string insertCredentialQuery = "INSERT INTO credential (UserID, email, passwordField) " +
                                           "VALUES (LAST_INSERT_ID(), @Email, @PasswordField)";

            string hashedPassword = HashPassword(password);

            using (var connection = new MySqlConnection(App.sqlConn))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        using (var command = new MySqlCommand(insertUserQuery, connection, transaction))
                        {
                            command.Parameters.AddWithValue("@Nickname", nickname);
                            command.Parameters.AddWithValue("@Name", name);
                            command.Parameters.AddWithValue("@BornDate", birthDate);
                            command.Parameters.AddWithValue("@Privacity", "user");
                            command.Parameters.AddWithValue("@Location", "");

                            command.ExecuteNonQuery();
                        }

                        using (var command = new MySqlCommand(insertCredentialQuery, connection, transaction))
                        {
                            command.Parameters.AddWithValue("@Email", email);
                            command.Parameters.AddWithValue("@PasswordField", hashedPassword);

                            command.ExecuteNonQuery();
                        }

                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }
        public static string ExecuteSearchUserByCredentials(string email, string password)
        {
            string HashedPassword = HashPassword(password);
            string searchUserQuery = "SELECT * FROM credential WHERE email = @Email AND passwordField = @Password";
            string result = "";
            using (var connection = new MySqlConnection(App.sqlConn))
            {
                connection.Open();
                using (var command = new MySqlCommand(searchUserQuery, connection))
                {
                    command.Parameters.AddWithValue("@Email", email);
                    command.Parameters.AddWithValue("@Password", HashedPassword);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            result = reader["UserID"].ToString();
                        }
                    }
                }
            }
            return result;
        }
        private static string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2")); // Convertir a hexadecimal
                }
                return builder.ToString();
            }
        }
        public static List<Dictionary<string, object>> ExecuteQuery(string sqlQuery, Dictionary<string, object> parameters = null)
        {
            var results = new List<Dictionary<string, object>>();

            using (MySqlConnection connection = new MySqlConnection(App.sqlConn))
            {
                try
                {
                    connection.Open();
                    using (MySqlCommand command = new MySqlCommand(sqlQuery, connection))
                    {
                        // Agregar parámetros si existen
                        if (parameters != null)
                        {
                            foreach (var param in parameters)
                            {
                                command.Parameters.AddWithValue(param.Key, param.Value);
                            }
                        }

                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var row = new Dictionary<string, object>();
                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    row[reader.GetName(i)] = reader[i];
                                }
                                results.Add(row);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
                finally
                {
                    connection.Close();
                }
            }

            return results;
        }
        #endregion
    }
}
