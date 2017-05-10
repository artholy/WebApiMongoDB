using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

using NotebookAppApi.Interfaces;
using NotebookAppApi.Model;
using MongoDB.Bson;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.Net.Http.Headers;
using MongoDB.Driver.GridFS;

namespace NotebookAppApi.Data
{
    public class NoteRepository : INoteRepository
    {
        private readonly NoteContext _context = null;

        public NoteRepository(IOptions<Settings> settings)
        {
            _context = new NoteContext(settings);
        }

        public async Task<IEnumerable<Note>> GetAllNotes()
        {
            try
            {
                return await _context.Notes.Find(_ => true).ToListAsync();
            }
            catch (Exception ex)
            {
                // log or manage the exception
                throw ex;
            }
        }

        public async Task<Note> GetNote(string id)
        {
            var filter = Builders<Note>.Filter.Eq("Id", id);

            try
            {
                return await _context.Notes
                                .Find(filter)
                                .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                // log or manage the exception
                throw ex;
            }
        }

        public async Task AddNote(Note item)
        {
            try
            {
                await _context.Notes.InsertOneAsync(item);
            }
            catch (Exception ex)
            {
                // log or manage the exception
                throw ex;
            }
        }

        public async Task<DeleteResult> RemoveNote(string id)
        {
            try
            {
                return await _context.Notes.DeleteOneAsync(
                     Builders<Note>.Filter.Eq("Id", id));
            }
            catch (Exception ex)
            {
                // log or manage the exception
                throw ex;
            }
        }

        public async Task<UpdateResult> UpdateNote(string id, string body)
        {
            var filter = Builders<Note>.Filter.Eq(s => s.Id, id);
            var update = Builders<Note>.Update
                            .Set(s => s.Body, body)
                            .CurrentDate(s => s.UpdatedOn);

            try
            {
                return await _context.Notes.UpdateOneAsync(filter, update);
            }
            catch (Exception ex)
            {
                // log or manage the exception
                throw ex;
            }
        }

        public async Task<ReplaceOneResult> UpdateNote(string id, Note item)
        {
            try
            {
                return await _context.Notes
                            .ReplaceOneAsync(n => n.Id.Equals(id)
                                            , item
                                            , new UpdateOptions { IsUpsert = true });
            }
            catch (Exception ex)
            {
                // log or manage the exception
                throw ex;
            }
        }

        // Demo function - full document update
        public async Task<ReplaceOneResult> UpdateNoteDocument(string id, string body)
        {
            var item = await GetNote(id) ?? new Note();
            item.Body = body;
            item.UpdatedOn = DateTime.Now;

            return await UpdateNote(id, item);
        }

        public async Task<DeleteResult> RemoveAllNotes()
        {
            try
            {
                return await _context.Notes.DeleteManyAsync(new BsonDocument());
            }
            catch (Exception ex)
            {
                // log or manage the exception
                throw ex;
            }
        }

        public async Task<ObjectId> UploadFile(IFormFile file)
        {
            try
            {
                var stream = file.OpenReadStream();
                var filename = file.FileName;
                return await _context.Bucket.UploadFromStreamAsync(filename, stream);
            }
            catch (Exception ex)
            {
                // log or manage the exception
                return new ObjectId(ex.ToString());
            } 
        }

        public async Task<String> GetFileInfo(string id)
        {
            GridFSFileInfo info = null;
            var objectId = new ObjectId(id);
            try
            {
                using (var stream = await _context.Bucket.OpenDownloadStreamAsync(objectId))
                {
                    info = stream.FileInfo;
                }
                return info.Filename;
            }
            catch (Exception)
            {
                return "Not Found";
            }
        }
    }
}
