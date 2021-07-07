using BFNet_2021_StorageMVC.Models.Entities;
using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BFNet_2021_StorageMVC.Repositories
{
    public interface UserRepository
    {
        User FindOneById(long id);
        User Create(User user);
        Task<User> CreateAsync(User user);
    }
    public class UserRepositoryImpl : UserRepository
    {
        private string _AccessKey;
        public UserRepositoryImpl(string accessKey)
        {
            _AccessKey = accessKey;
        }

        private CloudTable Table { 
            get {
                CloudStorageAccount account = CloudStorageAccount.Parse(_AccessKey);
                CloudTableClient client = account.CreateCloudTableClient(new TableClientConfiguration());

                CloudTable user = client.GetTableReference("user");
                var task = Task.Run(() => user.CreateIfNotExistsAsync());
                task.Wait();
                return user;
            } 
        }

        public User FindOneById(long id)
        {
            TableOperation operation = TableOperation.Retrieve<User>("user", $"{id}");

            return Table.Execute(operation).Result as User;
        }
        public User Create(User user)
        {
            var task = Task.Run(() => this.CreateAsync(user));
            task.Wait();
            return task.Result;
        }

        public async Task<User> CreateAsync(User user)
        {
            TableOperation insertOperation = TableOperation.InsertOrMerge(user);
            TableResult result = await Table.ExecuteAsync(insertOperation);
            return result.Result as User;
        }
    }
}
