using SwordAndFather.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Dapper;

namespace SwordAndFather.Data
{
    public class UserRepository
    {
        const string ConnectionString = "Server = localhost; Database = SwordAndFather; Trusted_Connection = True;";

        public User AddUser(string username, string password)
        {
            using (var db = new SqlConnection(ConnectionString))
            {
                var newUser = db.QueryFirstOrDefault<User>(@"
                    Insert into users (username, password)
                    Output inserted.*
                    Values(@username, @password)", 
                    new {username, password });

                if (newUser != null)
                {
                    return newUser;
                }
            }
            throw new Exception("No user created");
        }

        public void DeleteUser(int id)
        {
            using (var db = new SqlConnection(ConnectionString))
            {
               var rowsAffected =  db.Execute("Delete From Users Where Id = @id", new {id});

                if (rowsAffected != 1)
                {
                    throw new Exception("That didn't work out.");
                }
            }
        }

        public User UpdateUser(User userToUpdate)
        {
            using (var db = new SqlConnection(ConnectionString))
            {
                var rowsAffected = db.Execute(@"Update Users
                             Set username = @username,
                                 password = @password
                             Where Id = @id", userToUpdate);

                if (rowsAffected == 1)
                    return userToUpdate;
            }
            throw new Exception("Could not update user");
        }

        public IEnumerable<User> GetAll()
        {
            using (var db = new SqlConnection(ConnectionString))
            {
                db.Open();

                return db.Query<User>("select username,password,id from users");
            }
        }
    }
}
