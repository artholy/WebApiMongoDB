using Microsoft.Extensions.Options;
using MongoDB.Driver;
using NotebookAppApi.Model;
using MongoDB.Driver.GridFS;

namespace NotebookAppApi.Data
{
    public class NoteContext
    {
        private readonly IMongoDatabase _database = null;
        private readonly GridFSBucket _bucket = null;

        public NoteContext(IOptions<Settings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            if (client != null)
            {
                _database = client.GetDatabase(settings.Value.Database);

                var gridFSBucketOptions = new GridFSBucketOptions()
                {
                    BucketName = "images",
                    ChunkSizeBytes = 1048576, // 1МБ
                };

                _bucket = new GridFSBucket(_database, gridFSBucketOptions);

            }
        }

        public IMongoCollection<Note> Notes
        {
            get
            {
                return _database.GetCollection<Note>("Note");
            }
        }

        public GridFSBucket Bucket
        {
            get
            {
                return _bucket;
            }
        }

    }
}
