using PetaPoco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetaPoco
{
    public static class PetaPocoExtension
    {
        public static IEnumerable<TRet> Query<T1, T2, T3, T4, T5, TRet>(this Database db, Func<T1, T2, T3, T4, T5, TRet> cb, string sql, params object[] args) { return db.Query<TRet>(new Type[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5) }, cb, sql, args); }
        public static List<TRet> Fetch<T1, T2, T3, T4, T5, TRet>(this Database db, Func<T1, T2, T3, T4, T5, TRet> cb, string sql, params object[] args) { return db.Query<T1, T2, T3, T4, T5, TRet>(cb, sql, args).ToList(); }

        public static Page<TRet> Page<T1, T2, T3, TRet>(this Database db, Func<T1, T2, T3, TRet> cb, int page, long itemsPerPage, string sql, params object[] args)
        {
            string sqlCount, sqlPage;
            db.BuildPageQueries<TRet>((page - 1) * itemsPerPage, itemsPerPage, sql, ref args, out sqlCount, out sqlPage);
            return db.PageByCb<T1, T2, T3, TRet>(cb, page, itemsPerPage, sqlCount, args, sqlPage, args);
        }

        public static Page<TRet> Page<T1, T2, TRet>(this Database db, Func<T1, T2, TRet> cb, int page, long itemsPerPage, string sql, params object[] args)
        {
            string sqlCount, sqlPage;
            db.BuildPageQueries<TRet>((page - 1) * itemsPerPage, itemsPerPage, sql, ref args, out sqlCount, out sqlPage);
            return db.PageByCb<T1, T2, TRet>(cb, page, itemsPerPage, sqlCount, args, sqlPage, args);
        }

        public static Page<TRet> PageByCb<T1, T2, TRet>(this Database db, Func<T1, T2, TRet> cb, long page, long itemsPerPage, string sqlCount, object[] countArgs, string sqlPage, object[] pageArgs)
        {
            // Save the one-time command time out and use it for both queries
            var saveTimeout = db.OneTimeCommandTimeout;

            // Setup the paged result
            var result = new Page<TRet>
            {
                CurrentPage = page,
                ItemsPerPage = itemsPerPage,
                TotalItems = db.ExecuteScalar<long>(sqlCount, countArgs)
            };
            result.TotalPages = result.TotalItems / itemsPerPage;

            if ((result.TotalItems % itemsPerPage) != 0)
                result.TotalPages++;

            db.OneTimeCommandTimeout = saveTimeout;

            // Get the records
            result.Items = db.Fetch<T1, T2, TRet>(cb, sqlPage, pageArgs);

            // Done
            return result;
        }

        public static Page<TRet> PageByCb<T1, T2, T3, TRet>(this Database db, Func<T1, T2, T3, TRet> cb, long page, long itemsPerPage, string sqlCount, object[] countArgs, string sqlPage, object[] pageArgs)
        {
            // Save the one-time command time out and use it for both queries
            var saveTimeout = db.OneTimeCommandTimeout;

            // Setup the paged result
            var result = new Page<TRet>
            {
                CurrentPage = page,
                ItemsPerPage = itemsPerPage,
                TotalItems = db.ExecuteScalar<long>(sqlCount, countArgs)
            };
            result.TotalPages = result.TotalItems / itemsPerPage;

            if ((result.TotalItems % itemsPerPage) != 0)
                result.TotalPages++;

            db.OneTimeCommandTimeout = saveTimeout;

            // Get the records
            result.Items = db.Fetch<T1, T2, T3, TRet>(cb, sqlPage, pageArgs);

            // Done
            return result;
        }
    }
}
