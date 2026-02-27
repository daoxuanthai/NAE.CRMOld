
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using tcs.adapter.Sql;
using tcs.bo;
using tcs.lib;

namespace tcs.adapter.Elastics
{
    public class CustomerSearch
    {
        private string _currentIndexDB = "customer_nae";
        private string _currentTypeDB = "customer";
        private readonly ElasticIndexer _indexer;

        public CustomerSearch()
        {
            _indexer = new ElasticIndexer();
        }
        private static CustomerSearch _instance;
        public static CustomerSearch Instance => _instance ?? (_instance = new CustomerSearch());

        public bool Index(CustomerSE model)
        {
            return _indexer.Index(_currentIndexDB, _currentTypeDB, model, model.Id.ToString());
        }

        public bool Update(string id, object data)
        {
            return _indexer.Update<CustomerSE>(_currentIndexDB, _currentTypeDB, id, data);
        }

        public bool Delete(int id)
        {
            return _indexer.Delete<CustomerSE>(_currentIndexDB, _currentTypeDB, id.ToString());
        }

        public bool DeleteMany(string ids)
        {
            return _indexer.Delete<CustomerSE>(_currentIndexDB, _currentTypeDB, ids.SplitTotArray());
        }
    }
}
