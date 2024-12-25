using SQLite;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Rise.Common.Extensions
{
    public static partial class SQLiteConnectionExtensions
    {
        public static int RemoveAll(this ISQLiteConnection connection, IEnumerable<object> items, bool runInTransaction = true)
        {
            int i = 0;

            if (runInTransaction)
            {
                connection.RunInTransaction(() =>
                {
                    foreach (var item in items)
                        i += connection.Delete(item);
                });
            } else
            {
                foreach (var item in items)
                    i += connection.Delete(item);
            }

            return i;
        }

        public static Task<int> RemoveAllAsync(this ISQLiteAsyncConnection connection, IEnumerable<object> items, bool runInTransaction = true)
            => connection.WriteAsync((connection1) => RemoveAll(connection1, items, runInTransaction));

        public static int InsertOrReplaceAll(this ISQLiteConnection connection, IEnumerable<object> items, bool runInTransaction = true)
        {
            int i = 0;

            if (runInTransaction)
            {
                connection.RunInTransaction(() =>
                {
                    foreach (var item in items)
                        i += connection.InsertOrReplace(item);
                });
            }
            else
            {
                foreach (var item in items)
                    i += connection.InsertOrReplace(item);
            }

            return i;
        }

        public static Task<int> InsertOrReplaceAllAsync(this ISQLiteAsyncConnection connection, IEnumerable<object> items, bool runInTransaction = true)
            => connection.WriteAsync((connection1) => InsertOrReplaceAll(connection1, items, runInTransaction));
    }

    public static partial class SQLiteConnectionExtensions
    {
        private static Task<T> WriteAsync<T>(this ISQLiteAsyncConnection connection, Func<SQLiteConnectionWithLock, T> write)
        {
            return Task.Factory.StartNew(delegate
            {
                SQLiteConnectionWithLock connection1 = connection.GetConnection();
                using (connection1.Lock())
                    return write(connection1);
            }, CancellationToken.None, TaskCreationOptions.DenyChildAttach, TaskScheduler.Default);
        }
    }
}
