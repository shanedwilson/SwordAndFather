﻿using Dapper;
using Microsoft.Extensions.Options;
using SwordAndFather.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace SwordAndFather.Data
{
    public class UserRepository
    {
        readonly TargetRepository _targetRepository;
        readonly string _connectionString;


        public UserRepository(TargetRepository targetRepository, IOptions<DbConfiguration> dbConfig)
        {
            _targetRepository = targetRepository;
            _connectionString = dbConfig.Value.ConnectionString;
        }

        public User AddUser(string username, string password)
        {
            using (var db = new SqlConnection(_connectionString))
            {
                var newUser = db.QueryFirstOrDefault<User>(@"
                    Insert into users (username,password)
                    Output inserted.*
                    Values(@username,@password)",
                    new { username, password });

                if (newUser != null)
                {
                    return newUser;
                }
            }

            throw new Exception("No user created");
        }

        public void DeleteUser(int userId)
        {
            using (var db = new SqlConnection(_connectionString))
            {
                var parameter = new { Id = userId };

                var deleteQuery = "Delete From Users where Id = @id";

                var rowsAffected = db.Execute(deleteQuery, parameter);

                if (rowsAffected != 1)
                {
                    throw new Exception("Didn't do right");
                }
            }
        }

        public User UpdateUser(User userToUpdate)
        {
            using (var db = new SqlConnection(_connectionString))
            {
                var sql = @"Update Users
                            Set username = @username,
                                password = @password
                            Where id = @id";

                var rowsAffected = db.Execute(sql, userToUpdate);

                if (rowsAffected == 1)
                    return userToUpdate;
            }

            throw new Exception("Could not update user");
        }

        public IEnumerable<User> GetAll()
        {
            using (var db = new SqlConnection(_connectionString))
            {
                var users = db.Query<User>("select username, password, id from users").ToList();

                var targets = _targetRepository.GetAll();

                foreach (var user in users)
                {
                    var matchingTargets = targets.Where(target => target.UserId == user.Id).ToList();
                    user.Targets = matchingTargets;
                }

                //var targets = new TargetRepository().GetAll().GroupBy(target => target.UserId);

                //foreach (var user in users)
                //{
                //    var matchingTargets = targets.FirstOrDefault(grouping => grouping.Key == user.Id);

                //    user.Targets = matchingTargets?.ToList();
                //}

                return users;
            }
        }
    }
}