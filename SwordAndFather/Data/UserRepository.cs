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
