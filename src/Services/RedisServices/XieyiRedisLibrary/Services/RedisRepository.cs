using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using StackExchange.Redis;
using XieyiRedisLibrary.Config;
using XieyiRedisLibrary.Interfaces;

namespace XieyiRedisLibrary.Services
{
    public class RedisRepository : IRedisRepository
    {
        private readonly int _dbNum;
        private readonly ConnectionMultiplexer _redisClint;
        private string _customKey;

        #region 构造函数

        public RedisRepository(IRedisProvider redisProvider, IOptions<RedisConfig> redisOptions)
        {
            _redisClint = redisProvider.RedisMultiplexer ?? throw new ArgumentNullException(nameof(redisProvider));
            _customKey = redisOptions.Value.KeyPrefix;
            _dbNum = redisOptions.Value.DbNum;
        }

        #endregion 构造函数

        #region String

        #region 同步方法

        /// <summary>
        ///     保存单个key value
        /// </summary>
        /// <param name="key">Redis Key</param>
        /// <param name="value">保存的值</param>
        /// <param name="expiry">过期时间</param>
        /// <returns></returns>
        public bool StringSet(string key, string value, TimeSpan? expiry = default)
        {
            key = _AddSysCustomKey(key);
            return _Do(db => db.StringSet(key, value, expiry));
        }

        /// <summary>
        ///     保存多个key value
        /// </summary>
        /// <param name="keyValues">键值对</param>
        /// <returns></returns>
        public bool StringSet(List<KeyValuePair<RedisKey, RedisValue>> keyValues)
        {
            var newKeyValues = keyValues
                .Select(p => new KeyValuePair<RedisKey, RedisValue>(_AddSysCustomKey(p.Key), p.Value)).ToList();
            return _Do(db => db.StringSet(newKeyValues.ToArray()));
        }

        /// <summary>
        ///     保存一个对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="obj"></param>
        /// <param name="expiry"></param>
        /// <returns></returns>
        public bool StringSet<T>(string key, T obj, TimeSpan? expiry = default)
        {
            key = _AddSysCustomKey(key);
            var json = _ConvertJson(obj);
            return _Do(db => db.StringSet(key, json, expiry));
        }

        /// <summary>
        ///     获取单个key的值
        /// </summary>
        /// <param name="key">Redis Key</param>
        /// <returns></returns>
        public string StringGet(string key)
        {
            key = _AddSysCustomKey(key);
            return _Do(db => db.StringGet(key));
        }

        /// <summary>
        ///     获取多个Key
        /// </summary>
        /// <param name="listKey">Redis Key集合</param>
        /// <returns></returns>
        public RedisValue[] StringGet(List<string> listKey)
        {
            var newKeys = listKey.Select(_AddSysCustomKey).ToList();
            return _Do(db => db.StringGet(_ConvertRedisKeys(newKeys)));
        }

        /// <summary>
        ///     获取一个key的对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public T StringGet<T>(string key)
        {
            key = _AddSysCustomKey(key);
            return _Do(db => _ConvertObj<T>(db.StringGet(key)));
        }

        /// <summary>
        ///     为数字增长val
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val">可以为负</param>
        /// <returns>增长后的值</returns>
        public double StringIncrement(string key, double val = 1)
        {
            key = _AddSysCustomKey(key);
            return _Do(db => db.StringIncrement(key, val));
        }

        /// <summary>
        ///     为数字减少val
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val">可以为负</param>
        /// <returns>减少后的值</returns>
        public double StringDecrement(string key, double val = 1)
        {
            key = _AddSysCustomKey(key);
            return _Do(db => db.StringDecrement(key, val));
        }

        #endregion 同步方法

        #region 异步方法

        /// <summary>
        ///     保存单个key value
        /// </summary>
        /// <param name="key">Redis Key</param>
        /// <param name="value">保存的值</param>
        /// <param name="expiry">过期时间</param>
        /// <returns></returns>
        public async Task<bool> StringSetAsync(string key, string value, TimeSpan? expiry = default)
        {
            key = _AddSysCustomKey(key);
            return await _Do(db => db.StringSetAsync(key, value, expiry));
        }

        /// <summary>
        ///     保存多个key value
        /// </summary>
        /// <param name="keyValues">键值对</param>
        /// <returns></returns>
        public async Task<bool> StringSetAsync(List<KeyValuePair<RedisKey, RedisValue>> keyValues)
        {
            var newKeyValues =
                keyValues.Select(p => new KeyValuePair<RedisKey, RedisValue>(_AddSysCustomKey(p.Key), p.Value)).ToList();
            return await _Do(db => db.StringSetAsync(newKeyValues.ToArray()));
        }

        /// <summary>
        ///     保存一个对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="obj"></param>
        /// <param name="expiry"></param>
        /// <returns></returns>
        public async Task<bool> StringSetAsync<T>(string key, T obj, TimeSpan? expiry = default)
        {
            key = _AddSysCustomKey(key);
            var json = _ConvertJson(obj);
            return await _Do(db => db.StringSetAsync(key, json, expiry));
        }

        /// <summary>
        ///     获取单个key的值
        /// </summary>
        /// <param name="key">Redis Key</param>
        /// <returns></returns>
        public async Task<string> StringGetAsync(string key)
        {
            key = _AddSysCustomKey(key);
            return await _Do(db => db.StringGetAsync(key));
        }

        /// <summary>
        ///     获取多个Key
        /// </summary>
        /// <param name="listKey">Redis Key集合</param>
        /// <returns></returns>
        public async Task<RedisValue[]> StringGetAsync(List<string> listKey)
        {
            var newKeys = listKey.Select(_AddSysCustomKey).ToList();
            return await _Do(db => db.StringGetAsync(_ConvertRedisKeys(newKeys)));
        }

        /// <summary>
        ///     获取一个key的对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<T> StringGetAsync<T>(string key)
        {
            key = _AddSysCustomKey(key);
            string result = await _Do(db => db.StringGetAsync(key));
            return _ConvertObj<T>(result);
        }

        /// <summary>
        ///     为数字增长val
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val">可以为负</param>
        /// <returns>增长后的值</returns>
        public async Task<double> StringIncrementAsync(string key, double val = 1)
        {
            key = _AddSysCustomKey(key);
            return await _Do(db => db.StringIncrementAsync(key, val));
        }

        /// <summary>
        ///     为数字减少val
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val">可以为负</param>
        /// <returns>减少后的值</returns>
        public async Task<double> StringDecrementAsync(string key, double val = 1)
        {
            key = _AddSysCustomKey(key);
            return await _Do(db => db.StringDecrementAsync(key, val));
        }

        #endregion 异步方法

        #endregion String

        #region Hash

        #region 同步方法

        /// <summary>
        ///     判断某个数据是否已经被缓存
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <returns></returns>
        public bool HashExists(string key, string dataKey)
        {
            key = _AddSysCustomKey(key);
            return _Do(db => db.HashExists(key, dataKey));
        }

        /// <summary>
        ///     存储数据到hash表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public bool HashSet<T>(string key, string dataKey, T t)
        {
            key = _AddSysCustomKey(key);
            return _Do(db =>
            {
                var json = _ConvertJson(t);
                return db.HashSet(key, dataKey, json);
            });
        }

        /// <summary>
        ///     移除hash中的某值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <returns></returns>
        public bool HashDelete(string key, string dataKey)
        {
            key = _AddSysCustomKey(key);
            return _Do(db => db.HashDelete(key, dataKey));
        }

        /// <summary>
        ///     移除hash中的多个值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dataKeys"></param>
        /// <returns></returns>
        public long HashDelete(string key, List<RedisValue> dataKeys)
        {
            key = _AddSysCustomKey(key);
            //List<RedisValue> dataKeys1 = new List<RedisValue>() {"1","2"};
            return _Do(db => db.HashDelete(key, dataKeys.ToArray()));
        }

        /// <summary>
        ///     从hash表获取数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <returns></returns>
        public T HashGet<T>(string key, string dataKey)
        {
            key = _AddSysCustomKey(key);
            return _Do(db =>
            {
                string value = db.HashGet(key, dataKey);
                return _ConvertObj<T>(value);
            });
        }

        /// <summary>
        ///     为数字增长val
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <param name="val">可以为负</param>
        /// <returns>增长后的值</returns>
        public double HashIncrement(string key, string dataKey, double val = 1)
        {
            key = _AddSysCustomKey(key);
            return _Do(db => db.HashIncrement(key, dataKey, val));
        }

        /// <summary>
        ///     为数字减少val
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <param name="val">可以为负</param>
        /// <returns>减少后的值</returns>
        public double HashDecrement(string key, string dataKey, double val = 1)
        {
            key = _AddSysCustomKey(key);
            return _Do(db => db.HashDecrement(key, dataKey, val));
        }

        /// <summary>
        ///     获取hashKey所有Redis key
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public List<T> HashKeys<T>(string key)
        {
            key = _AddSysCustomKey(key);
            return _Do(db =>
            {
                var values = db.HashKeys(key);
                return _ConvertList<T>(values);
            });
        }

        #endregion 同步方法

        #region 异步方法

        /// <summary>
        ///     判断某个数据是否已经被缓存
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <returns></returns>
        public async Task<bool> HashExistsAsync(string key, string dataKey)
        {
            key = _AddSysCustomKey(key);
            return await _Do(db => db.HashExistsAsync(key, dataKey));
        }

        /// <summary>
        ///     存储数据到hash表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public async Task<bool> HashSetAsync<T>(string key, string dataKey, T t)
        {
            key = _AddSysCustomKey(key);
            return await _Do(db =>
            {
                var json = _ConvertJson(t);
                return db.HashSetAsync(key, dataKey, json);
            });
        }

        /// <summary>
        ///     移除hash中的某值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <returns></returns>
        public async Task<bool> HashDeleteAsync(string key, string dataKey)
        {
            key = _AddSysCustomKey(key);
            return await _Do(db => db.HashDeleteAsync(key, dataKey));
        }

        /// <summary>
        ///     移除hash中的多个值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dataKeys"></param>
        /// <returns></returns>
        public async Task<long> HashDeleteAsync(string key, List<RedisValue> dataKeys)
        {
            key = _AddSysCustomKey(key);
            //List<RedisValue> dataKeys1 = new List<RedisValue>() {"1","2"};
            return await _Do(db => db.HashDeleteAsync(key, dataKeys.ToArray()));
        }

        /// <summary>
        ///     从hash表获取数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <returns></returns>
        public async Task<T> HashGeAsync<T>(string key, string dataKey)
        {
            key = _AddSysCustomKey(key);
            string value = await _Do(db => db.HashGetAsync(key, dataKey));
            return _ConvertObj<T>(value);
        }

        /// <summary>
        ///     为数字增长val
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <param name="val">可以为负</param>
        /// <returns>增长后的值</returns>
        public async Task<double> HashIncrementAsync(string key, string dataKey, double val = 1)
        {
            key = _AddSysCustomKey(key);
            return await _Do(db => db.HashIncrementAsync(key, dataKey, val));
        }

        /// <summary>
        ///     为数字减少val
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <param name="val">可以为负</param>
        /// <returns>减少后的值</returns>
        public async Task<double> HashDecrementAsync(string key, string dataKey, double val = 1)
        {
            key = _AddSysCustomKey(key);
            return await _Do(db => db.HashDecrementAsync(key, dataKey, val));
        }

        /// <summary>
        ///     获取hashKey所有Redis key
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<List<T>> HashKeysAsync<T>(string key)
        {
            key = _AddSysCustomKey(key);
            var values = await _Do(db => db.HashKeysAsync(key));
            return _ConvertList<T>(values);
        }

        #endregion 异步方法

        #endregion Hash

        #region List

        #region 同步方法

        /// <summary>
        ///     移除指定ListId的内部List的值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void ListRemove<T>(string key, T value)
        {
            key = _AddSysCustomKey(key);
            _Do(db => db.ListRemove(key, _ConvertJson(value)));
        }

        /// <summary>
        ///     获取指定key的List
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public List<T> ListRange<T>(string key)
        {
            key = _AddSysCustomKey(key);
            return _Do(redis =>
            {
                var values = redis.ListRange(key);
                return _ConvertList<T>(values);
            });
        }

        /// <summary>
        ///     入队
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void ListRightPush<T>(string key, T value)
        {
            key = _AddSysCustomKey(key);
            _Do(db => db.ListRightPush(key, _ConvertJson(value)));
        }

        /// <summary>
        ///     出队
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public T ListRightPop<T>(string key)
        {
            key = _AddSysCustomKey(key);
            return _Do(db =>
            {
                var value = db.ListRightPop(key);
                return _ConvertObj<T>(value);
            });
        }

        /// <summary>
        ///     入栈
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void ListLeftPush<T>(string key, T value)
        {
            key = _AddSysCustomKey(key);
            _Do(db => db.ListLeftPush(key, _ConvertJson(value)));
        }

        /// <summary>
        ///     出栈
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public T ListLeftPop<T>(string key)
        {
            key = _AddSysCustomKey(key);
            return _Do(db =>
            {
                var value = db.ListLeftPop(key);
                return _ConvertObj<T>(value);
            });
        }

        /// <summary>
        ///     获取集合中的数量
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public long ListLength(string key)
        {
            key = _AddSysCustomKey(key);
            return _Do(redis => redis.ListLength(key));
        }

        #endregion 同步方法

        #region 异步方法

        /// <summary>
        ///     移除指定ListId的内部List的值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public async Task<long> ListRemoveAsync<T>(string key, T value)
        {
            key = _AddSysCustomKey(key);
            return await _Do(db => db.ListRemoveAsync(key, _ConvertJson(value)));
        }

        /// <summary>
        ///     获取指定key的List
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<List<T>> ListRangeAsync<T>(string key)
        {
            key = _AddSysCustomKey(key);
            var values = await _Do(redis => redis.ListRangeAsync(key));
            return _ConvertList<T>(values);
        }

        /// <summary>
        ///     入队
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public async Task<long> ListRightPushAsync<T>(string key, T value)
        {
            key = _AddSysCustomKey(key);
            return await _Do(db => db.ListRightPushAsync(key, _ConvertJson(value)));
        }

        /// <summary>
        ///     出队
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<T> ListRightPopAsync<T>(string key)
        {
            key = _AddSysCustomKey(key);
            var value = await _Do(db => db.ListRightPopAsync(key));
            return _ConvertObj<T>(value);
        }

        /// <summary>
        ///     入栈
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public async Task<long> ListLeftPushAsync<T>(string key, T value)
        {
            key = _AddSysCustomKey(key);
            return await _Do(db => db.ListLeftPushAsync(key, _ConvertJson(value)));
        }

        /// <summary>
        ///     出栈
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<T> ListLeftPopAsync<T>(string key)
        {
            key = _AddSysCustomKey(key);
            var value = await _Do(db => db.ListLeftPopAsync(key));
            return _ConvertObj<T>(value);
        }

        /// <summary>
        ///     获取集合中的数量
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<long> ListLengthAsync(string key)
        {
            key = _AddSysCustomKey(key);
            return await _Do(redis => redis.ListLengthAsync(key));
        }

        #endregion 异步方法

        #endregion List

        #region SortedSet 有序集合

        #region 同步方法

        /// <summary>
        ///     添加
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="score"></param>
        public bool SortedSetAdd<T>(string key, T value, double score)
        {
            key = _AddSysCustomKey(key);
            return _Do(redis => redis.SortedSetAdd(key, _ConvertJson(value), score));
        }

        /// <summary>
        ///     删除
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public bool SortedSetRemove<T>(string key, T value)
        {
            key = _AddSysCustomKey(key);
            return _Do(redis => redis.SortedSetRemove(key, _ConvertJson(value)));
        }

        /// <summary>
        ///     获取全部
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public List<T> SortedSetRangeByRank<T>(string key)
        {
            key = _AddSysCustomKey(key);
            return _Do(redis =>
            {
                var values = redis.SortedSetRangeByRank(key);
                return _ConvertList<T>(values);
            });
        }

        /// <summary>
        ///     获取集合中的数量
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public long SortedSetLength(string key)
        {
            key = _AddSysCustomKey(key);
            return _Do(redis => redis.SortedSetLength(key));
        }

        #endregion 同步方法

        #region 异步方法

        /// <summary>
        ///     添加
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="score"></param>
        public async Task<bool> SortedSetAddAsync<T>(string key, T value, double score)
        {
            key = _AddSysCustomKey(key);
            return await _Do(redis => redis.SortedSetAddAsync(key, _ConvertJson(value), score));
        }

        /// <summary>
        ///     删除
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public async Task<bool> SortedSetRemoveAsync<T>(string key, T value)
        {
            key = _AddSysCustomKey(key);
            return await _Do(redis => redis.SortedSetRemoveAsync(key, _ConvertJson(value)));
        }

        /// <summary>
        ///     获取全部
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<List<T>> SortedSetRangeByRankAsync<T>(string key)
        {
            key = _AddSysCustomKey(key);
            var values = await _Do(redis => redis.SortedSetRangeByRankAsync(key));
            return _ConvertList<T>(values);
        }

        /// <summary>
        ///     获取集合中的数量
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<long> SortedSetLengthAsync(string key)
        {
            key = _AddSysCustomKey(key);
            return await _Do(redis => redis.SortedSetLengthAsync(key));
        }

        #endregion 异步方法

        #endregion SortedSet 有序集合

        #region key

        /// <summary>
        ///     删除单个key
        /// </summary>
        /// <param name="key">redis key</param>
        /// <returns>是否删除成功</returns>
        public bool KeyDelete(string key)
        {
            key = _AddSysCustomKey(key);
            return _Do(db => db.KeyDelete(key));
        }

        /// <summary>
        ///     删除多个key
        /// </summary>
        /// <param name="keys">redis key</param>
        /// <returns>成功删除的个数</returns>
        public long KeyDelete(List<string> keys)
        {
            var newKeys = keys.Select(_AddSysCustomKey).ToList();
            return _Do(db => db.KeyDelete(_ConvertRedisKeys(newKeys)));
        }

        /// <summary>
        ///     判断key是否存储
        /// </summary>
        /// <param name="key">redis key</param>
        /// <returns></returns>
        public bool KeyExists(string key)
        {
            key = _AddSysCustomKey(key);
            return _Do(db => db.KeyExists(key));
        }

        /// <summary>
        ///     重新命名key
        /// </summary>
        /// <param name="key">就的redis key</param>
        /// <param name="newKey">新的redis key</param>
        /// <returns></returns>
        public bool KeyRename(string key, string newKey)
        {
            key = _AddSysCustomKey(key);
            return _Do(db => db.KeyRename(key, newKey));
        }

        /// <summary>
        ///     设置Key的时间
        /// </summary>
        /// <param name="key">redis key</param>
        /// <param name="expiry"></param>
        /// <returns></returns>
        public bool KeyExpire(string key, TimeSpan? expiry = default)
        {
            key = _AddSysCustomKey(key);
            return _Do(db => db.KeyExpire(key, expiry));
        }

        #endregion key

        #region 发布订阅

        /// <summary>
        ///     Redis发布订阅  订阅
        /// </summary>
        /// <param name="subChannel"></param>
        /// <param name="handler"></param>
        public void Subscribe(string subChannel, Action<RedisChannel, RedisValue> handler = null)
        {
            var sub = _redisClint.GetSubscriber();
            sub.Subscribe(subChannel, (channel, message) =>
            {
                if (handler == null)
                    Console.WriteLine(subChannel + " 订阅收到消息：" + message);
                else
                    handler(channel, message);
            });
        }

        /// <summary>
        ///     Redis发布订阅  发布
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="channel"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public long Publish<T>(string channel, T msg)
        {
            var sub = _redisClint.GetSubscriber();
            return sub.Publish(channel, _ConvertJson(msg));
        }

        /// <summary>
        ///     Redis发布订阅  取消订阅
        /// </summary>
        /// <param name="channel"></param>
        public void Unsubscribe(string channel)
        {
            var sub = _redisClint.GetSubscriber();
            sub.Unsubscribe(channel);
        }

        /// <summary>
        ///     Redis发布订阅  取消全部订阅
        /// </summary>
        public void UnsubscribeAll()
        {
            var sub = _redisClint.GetSubscriber();
            sub.UnsubscribeAll();
        }

        #endregion 发布订阅

        #region 其他

        public IDatabase GetDatabase()
        {
            return _redisClint.GetDatabase(_dbNum);
        }

        public IServer GetServer(string hostAndPort)
        {
            return _redisClint.GetServer(hostAndPort);
        }

        public ITransaction CreateTransaction()
        {
            return GetDatabase().CreateTransaction();
        }

        public void SetSysCustomKey(string customKey)
        {
            _customKey = customKey;
        }

        #endregion 其他

        #region 辅助方法

        private string _AddSysCustomKey(string oldKey)
        {
            var prefixKey = _customKey;
            return prefixKey + oldKey;
        }

        private T _Do<T>(Func<IDatabase, T> func)
        {
            var database = _redisClint.GetDatabase(_dbNum);
            return func(database);
        }

        private string _ConvertJson<T>(T value)
        {
            var result = value is string ? value.ToString() : JsonConvert.SerializeObject(value);
            return result;
        }

        private T _ConvertObj<T>(RedisValue value)
        {
            if (typeof(T).Name.Equals(nameof(String))) return JsonConvert.DeserializeObject<T>($"'{value}'");
            return JsonConvert.DeserializeObject<T>(value);
        }

        private List<T> _ConvertList<T>(IEnumerable<RedisValue> values)
        {
            var result = new List<T>();
            foreach (var item in values)
            {
                var model = _ConvertObj<T>(item);
                result.Add(model);
            }

            return result;
        }

        private RedisKey[] _ConvertRedisKeys(List<string> redisKeys)
        {
            return redisKeys.Select(redisKey => (RedisKey)redisKey).ToArray();
        }

        #endregion 辅助方法
    }

}