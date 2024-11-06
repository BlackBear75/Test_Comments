using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Test_Comments.Entities.RecordGroup;
using Test_Comments.Entities.RecordGroup.Repository;
using Test_Comments.Models.RecordModels;

namespace Test_Comments.Services
{
    public interface IRecordService
    {
        Task<Response> AddRecordAsync(Record request);
        Task<List<Record>> GetAllRecordsAsync();
        Task<List<Record>> GetRecordsAsync(int skip, int take);
        Task<int> GetTotalRecordsCountAsync();
        Task<Response> AddCommentAsync(Guid recordId, string? comment,string userName); 
    }

    public class RecordService : IRecordService
    {
        private readonly IRecordRepository<Record> _recordRepository;

        public RecordService(IRecordRepository<Record> recordRepository)
        {
            _recordRepository = recordRepository;
        }

        public async Task<Response> AddRecordAsync(Record request)
        {
            try
            {
                await _recordRepository.InsertOneAsync(request); 

                return new Response
                {
                    Success = true,
                    Message = "Запис успішно додано"
                };
            }
            catch (Exception ex)
            {
                return new Response
                {
                    Success = false,
                    Message = $"Помилка: {ex.Message}"
                };
            }
        }
        

        public async Task<List<Record>> GetAllRecordsAsync()
        {
            var result = await _recordRepository.GetAllAsync(); 
            return result.ToList();
        }

        public async Task<List<Record>> GetRecordsAsync(int skip, int take)
        {
            var result = await _recordRepository.GetWithSkipAsync(skip-1, take); 
            return result.ToList();
        }

        public async Task<int> GetTotalRecordsCountAsync()
        {
            return await _recordRepository.CountAsync(record => true); 
        }

        public async Task<Response> AddCommentAsync(Guid recordId, string? commentText, string userName)
        {
            try
            {
                var record = await _recordRepository.FindByIdAsync(recordId);
                if (record == null)
                {
                    return new Response
                    {
                        Success = false,
                        Message = "Запис або коментар не знайдено"
                    };
                }

                var newComment = new Record
                {
                    UserName = userName,
                    Text = commentText,
                    Comments = new List<Record>()
                };

                record.Comments.Add(newComment);
        
                await _recordRepository.UpdateOneAsync(record);

                return new Response
                {
                    Success = true,
                    Message = "Коментар успішно додано"
                };
            }
            catch (Exception ex)
            {
                return new Response
                {
                    Success = false,
                    Message = $"Помилка: {ex.Message}"
                };
            }
        }


    }
    

    public class Response
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }
}
