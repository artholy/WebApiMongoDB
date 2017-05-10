using System.Collections.Generic;
using System.Threading.Tasks;
using NotebookAppApi.Model;
using MongoDB.Driver;
using Microsoft.AspNetCore.Http;
using MongoDB.Bson;
using MongoDB.Driver.GridFS;
using System;

namespace NotebookAppApi.Interfaces
{
    public interface INoteRepository
    {
        Task<IEnumerable<Note>> GetAllNotes();
        Task<Note> GetNote(string id);
        Task AddNote(Note item);
        Task<DeleteResult> RemoveNote(string id);

        Task<UpdateResult> UpdateNote(string id, string body);

        // demo interface - full document update
        Task<ReplaceOneResult> UpdateNoteDocument(string id, string body);

        // should be used with high cautious, only in relation with demo setup
        Task<DeleteResult> RemoveAllNotes();

        Task<ObjectId> UploadFile(IFormFile file);

        Task<String> GetFileInfo(string id);
    }
}
