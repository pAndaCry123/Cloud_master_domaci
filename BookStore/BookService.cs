using Contracts;
using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Data.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore
{
    public class BookService : IBookStore
    {
        IReliableStateManager StateManager { get; set; }
        string book { get; set; }
        int count { get; set; }
        public BookService(IReliableStateManager StateManager)
        {
            this.StateManager = StateManager;
        }

  

  
        public async Task<Book> list_books()
        {
            try
            {
                var books_dict = await this.StateManager.GetOrAddAsync<IReliableDictionary<string, Book>>("books");

                using (var tx = this.StateManager.CreateTransaction())
                {
                    var book = await books_dict.TryGetValueAsync(tx, "1");
          

                    await tx.CommitAsync();
                    return book.Value;
                }
            }
            catch (Exception e) {
                return null;
            }
        }

        public async Task<bool> prepare()
        {

            this.book = "1";
            this.count = 1;
            Book book = await list_books();

            if (book != null && book.Count>this.count)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public Task Commit()
        {
            throw new NotImplementedException();
        }

        public Task Rollback()
        {
            throw new NotImplementedException();
        }
    }
}
