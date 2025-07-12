using System.Linq;
using System.Threading.Tasks;
using Uniconta.API.System;
using Uniconta.Common;

namespace Zendo.UnicontaPowerToys.Extentions
{
    public static class UnicontaQueryAPIExtentions
    {
        /// <summary>
        /// This extension method allows you to retrieve a single entity by its RowId.
        /// 
        /// Fun fact: Uniconta dosnt have a Get call only query so we will need to use a "Query with PropValuePair" to retrieve a single entity.
        /// If a model dosnt have a RowId it will return all entities from uniconta.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="api"></param>
        /// <param name="rowId"></param>
        /// <returns></returns>
        public static async Task<T> GetByRowIdAsync<T>(this QueryAPI api, int rowId)
            where T : class, UnicontaBaseEntity, new()
        {
            var queryResult = await api.Query<T>(new []
            {
                PropValuePair.GenereteWhereElements("RowId", typeof(int), rowId.ToString())
            });

            return queryResult?.SingleOrDefault();
        }

        /// <summary>
        /// This extension method allows you to retrieve a single entity by its KeyStr.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="api"></param>
        /// <param name="keyStr"></param>
        /// <returns></returns>
        public static async Task<T> GetByKeyStrAsync<T>(this QueryAPI api, string keyStr)
            where T : class, UnicontaBaseEntity, IdKey, new()
        {
            var queryResult = await api.Query<T>(new []
            {
                PropValuePair.GenereteWhereElements("KeyStr", typeof(string), keyStr)
            });

            return queryResult?.SingleOrDefault();
        }

        /// <summary>
        /// This extension method allows you to get and refresh the cache.
        /// 
        /// Fun fact: There is currently an issue with Uniconta's caching mechanism. so it wont work as expected if the PC or Server's time is not in sync with the Uniconta Server.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="crudApi"></param>
        /// <returns></returns>
        public static async Task<SQLCache> GetAndRefreshCacheAsync<T>(this QueryAPI crudApi)
            where T : UnicontaBaseEntity, IdKey
        {
            var company = crudApi.CompanyEntity;
            if (company.HasCache(typeof(T)))
                await crudApi.UpdateCache([typeof(T)]);

            var cache = crudApi.GetCache(typeof(T));
            if (cache == null)
                cache = await crudApi.LoadCache(typeof(T));

            return cache;
        }

        /// <summary>
        /// This extension method allows you to refresh the cache for a specific entity type.
        /// This is useful when you want to use underlying properties in other entities.
        /// And an example would be if you get an DebtorOrderLineClient and want to beable to access it's InvItem property with out getting null.
        /// 
        /// Fun fact: There is currently an issue with Uniconta's caching mechanism. so it wont work as expected if the PC or Server's time is not in sync with the Uniconta Server.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="crudApi"></param>
        /// <returns></returns>
        public static async Task RefreshCacheAsync<T>(this QueryAPI crudApi)
            where T : UnicontaBaseEntity, IdKey
        {
            var company = crudApi.CompanyEntity;
            if (company.HasCache(typeof(T)))
                await crudApi.UpdateCache([typeof(T)]);

            if (crudApi.GetCache(typeof(T)) == null)
                await crudApi.LoadCache(typeof(T));
        }
    }
}
