using System;
using System.Configuration;
using System.Linq;
using Elasticsearch.Net;
using Nest;

namespace tcs.adapter.Elastics
{
    public class ElasticIndexer
    {
        public static object LockQueue = new Object();

        public ElasticIndexer() : this("elastic_connection_string")
        {

        }

        public ElasticIndexer(string connectionStringKey)
        {
            var connectionString = ConfigurationManager.AppSettings[connectionStringKey];
            if (string.IsNullOrEmpty(connectionString))
                throw new Exception("Không tìm thấy " + connectionStringKey + " trong appsettings.config");

            string[] els = connectionString.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            Uri[] lstSlaveNode = els.Where(e => !string.IsNullOrEmpty(e)).Select(e => new Uri(e)).ToArray();
            if (!lstSlaveNode.Any())
                throw new Exception("Connection string " + connectionStringKey + " sai cú pháp");

            var connectionPool = new SniffingConnectionPool(lstSlaveNode);
            var config = new ConnectionSettings(connectionPool);
#if DEBUG
            config.EnableDebugMode();
            config.DisableDirectStreaming();
#endif
            _elasticClient = new ElasticClient(config);
        }

        private readonly ElasticClient _elasticClient;
        public ElasticClient IndexClient
        {
            get
            {
                lock (LockQueue)
                {
                    return _elasticClient;
                }
            }
        }

        public bool Delete<T>(string index, string type, params string[] idList) where T : class
        {
            lock (LockQueue)
            {
                if (idList == null || idList.Length == 0)
                    return false;

                var request =
                    new DeleteByQueryRequest<T>(index, type) { Query = Query<T>.Terms(t => t.Field("_id").Terms(idList)) };

                var response = _elasticClient.DeleteByQuery<T>(d => request);
                if (response != null)
                {
#if DEBUG
                    if (response.ServerError != null)
                        throw new Exception("ElasticIndexer::Delete::" + response.ServerError.Error);
#endif

                    return response.IsValid;
                }
                return false;
            }
        }
        
        public T Get<T>(string index, string type, string id) where T : class
        {
            lock (LockQueue)
            {
                var request = new GetRequest<T>(index, type, id);
                var response = _elasticClient.Get<T>(request);
                if (response != null)
                {
#if DEBUG
                    if (response.ServerError != null)
                        throw new Exception("ElasticIndexer::Get::" + response.ServerError.Error);
#endif

                    if (response.IsValid)
                        return response.Source;
                }

                return null;
            }
        }

        public bool Index<T>(string index, string type, T data, string id) where T : class
        {
            lock (LockQueue)
            {
                IIndexRequest<T> req = new IndexRequest<T>(data, index, type, id);
                IIndexResponse response = _elasticClient.Index(req);
                if (response != null)
                {
#if DEBUG
                    if (response.ServerError != null)
                        throw new Exception("ElasticIndexer::Index::" + response.ServerError.Error);
#endif

                    return response.IsValid;
                }
                return false;
            }
        }

        public bool IndexMany<T>(string index, string type, T[] data) where T : class
        {
            lock (LockQueue)
            {
                IBulkResponse response = _elasticClient.IndexMany(data, index, type);
                if (response != null)
                {
#if DEBUG
                    if (response.ServerError != null)
                        throw new Exception("ElasticIndexer::IndexMany::" + response.ServerError.Error);
#endif

                    return response.IsValid;
                }
                return false;
            }
        }

        public bool Update<T>(string index, string type, string id, object data) where T : class
        {
            var response = _elasticClient.UpdateAsync<T, object>(DocumentPath<T>.Id(id), u => u
                .Index(index)
                .Type(type)
                .Doc(data)
                .DocAsUpsert());
            return true;
        }

        #region Singleton

        static ElasticIndexer _instance;
        public static ElasticIndexer Current => _instance ?? (_instance = new ElasticIndexer());

        #endregion

    }
}