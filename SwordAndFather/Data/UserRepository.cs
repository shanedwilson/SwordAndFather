using SwordAndFather.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Dapper;
using System.Linq;
using System.Threading.Tasks;

namespace SwordAndFather.Data
{
    public class UserRepository
    {
        const string ConnectionString = "Server = localhost; Database = SwordAndFather; Trusted_Connection = True;";

        public User AddUser(string username, string password)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                var insertUserCommand = connection.CreateCommand();
                insertUserCommand.CommandText = $@"Insert into users (username, password)
                                              Output inserted.*
                                              Values(@username, @password)";

                insertUserCommand.Parameters.AddWithValue("username", username);
                insertUserCommand.Parameters.AddWithValue("password", password);

                var reader = insertUserCommand.ExecuteReader();

                if (reader.Read())
                {
                    var insertedPassword = reader["password"].ToString();
                    var insertedUserName = reader["username"].ToString();
                    var insertedId = (int)reader["Id"];

                    var newUser = new User(insertedUserName, insertedPassword) { Id = insertedId };

                    return newUser;
                }
            }

            throw new Exception("No user found");
        }

        public IEnumerable<User> GetAll()
        {
            //var users = new List<User>();

            using (var db = new SqlConnection(ConnectionString))
            {
                db.Open();

                return db.Query<User>("select username,password,id from users");
            }
        }
    }
}
