using System;
using System.Threading.Tasks;
using Test_Comments.Entities.RecordGroup;
using Test_Comments.Entities.RecordGroup.Repository;
using Test_Comments.Models.RecordModels;

namespace Test_Comments.Services
{
    public interface IRecordService
    {
        Task<Response> AddRecordAsync(RecordModel request);
    }

    public class RecordService : IRecordService
    {
        private readonly IRecordRepository<Record> _recordRepository;

        public RecordService(IRecordRepository<Record> recordRepository)
        {
            _recordRepository = recordRepository;
        }

        public async Task<Response> AddRecordAsync(RecordModel request)
        {
            try
            {
                var record = new Record
                {
                    UserName = request.UserName,
                    Email = request.Email,
                    Captcha = request.Captcha,
                    Text = request.Text
                };

                await _recordRepository.InsertOneAsync(record);

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
    }

    public class Response
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }
}