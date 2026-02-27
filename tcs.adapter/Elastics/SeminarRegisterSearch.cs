
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using tcs.adapter.Sql;
using tcs.bo;
using tcs.lib;

namespace tcs.adapter.Elastics
{
    public class SeminarRegisterSearch
    {
        private string _currentIndexDB = "seminar_register_nae";
        private string _currentTypeDB = "register";
        private readonly ElasticIndexer _indexer;

        public SeminarRegisterSearch()
        {
            _indexer = new ElasticIndexer();
        }
        private static SeminarRegisterSearch _instance;
        public static SeminarRegisterSearch Instance => _instance ?? (_instance = new SeminarRegisterSearch());

        public bool Index(SeminarRegisterSE model)
        {
            return _indexer.Index(_currentIndexDB, _currentTypeDB, model, model.Id.ToString());
        }

        public bool Update(string id, object data)
        {
            return _indexer.Update<SeminarRegisterSE>(_currentIndexDB, _currentTypeDB, id, data);
        }

        public bool Delete(int id)
        {
            return _indexer.Delete<SeminarRegisterSE>(_currentIndexDB, _currentTypeDB, id.ToString());
        }

        public bool DeleteMany(string ids)
        {
            return _indexer.Delete<SeminarRegisterSE>(_currentIndexDB, _currentTypeDB, ids.SplitTotArray());
        }

        public SeminarRegisterSE Get(int id)
        {
            return _indexer.Get<SeminarRegisterSE>(_currentIndexDB, _currentTypeDB, id.ToString());
        }

        public List<SeminarRegisterSE> Search(int seminarId = -1, int placeId = -1, int titleId = -1, int page = 0, int pageSize = 1000)
        {
            QueryContainer filter = null;

            if (seminarId > 0)
                filter &= Query<SeminarRegisterSE>.Terms(t => t.Field("seminarId").Terms(seminarId));

            if (placeId > 0)
                filter &= Query<SeminarRegisterSE>.Terms(t => t.Field("seminarPlaceId").Terms(placeId));

            if (titleId > 0)
                filter &= Query<SeminarRegisterSE>.Terms(t => t.Field("titleId").Terms(titleId));

            var from = page * pageSize;
            var response = _indexer.IndexClient.Search<SeminarRegisterSE>(s => s
                    .Index(_currentIndexDB)
                    .Type(_currentTypeDB)
                    .Query(q => filter)
                    .Sort(so =>
                    {
                        return so.Descending(i => i.UpdateDate);
                    })
                    .From(from)
                    .Size(pageSize));

            return response.Documents != null && response.Documents.Any() ? response.Documents.ToList() : null;
        }
    }
}
