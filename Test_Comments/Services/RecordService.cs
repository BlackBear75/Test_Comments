﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Test_Comments.Entities.RecordGroup.Repository;

public interface IRecordService
{
    Task<Response> AddRecordAsync(Record record);
    Task<Response> AddCommentAsync(Guid parentRecordId, string text, string userName, string email,  Guid?  parentId);
    Task<List<Record>> GetPagedRootRecordsWithCommentsAsync(int skip, int take);
    Task<int> GetTotalRootRecordsCountAsync();
}

public class RecordService : IRecordService
{
    private readonly IRecordRepository<Record> _recordRepository;

    public RecordService(IRecordRepository<Record> recordRepository)
    {
        _recordRepository = recordRepository;
    }

    public async Task<Response> AddRecordAsync(Record record)
    {
        try
        {
            await _recordRepository.InsertOneAsync(record);
            return new Response { Success = true, Message = "Запис успішно додано" };
        }
        catch (Exception ex)
        {
            return new Response { Success = false, Message = $"Помилка: {ex.Message}" };
        }
    }

    public async Task<Response> AddCommentAsync(Guid parentRecordId, string text, string userName, string email, Guid?  parentId)
    {
        try
        {
            var parentRecord = await _recordRepository.FindByIdAsync(parentRecordId);
            if (parentRecord == null)
            {
                return new Response { Success = false, Message = "Батьківський запис не знайдено" };
            }

            var newComment = new Record
            {
                Id = Guid.NewGuid(),
                UserName = userName,
                Email = email,
                Text = text,
                ParentRecordId = parentRecordId,
            };

            await _recordRepository.InsertOneAsync(newComment);
            return new Response { Success = true, Message = "Коментар успішно додано" };
        }
        catch (Exception ex)
        {
            return new Response { Success = false, Message = $"Помилка: {ex.Message}" };
        }
    }

  public async Task<List<Record>> GetPagedRootRecordsWithCommentsAsync(int skip, int take)
{
    // Отримуємо кореневі записи
    var pagedRootRecords = await _recordRepository.FilterBySkipAsync(r => r.ParentRecordId == null, skip, take);

    var rootRecordIds = pagedRootRecords.Select(r => r.Id).ToList();

    var allCommentsForRootRecords = await GetAllCommentsRecursively(rootRecordIds);

    var commentsDict = allCommentsForRootRecords.GroupBy(c => c.ParentRecordId)
        .ToDictionary(g => g.Key, g => g.ToList());

    foreach (var record in pagedRootRecords)
    {
        record.Comments = BuildCommentsHierarchy(record.Id, commentsDict);
    }

    return pagedRootRecords.ToList();
}

private async Task<List<Record>> GetAllCommentsRecursively(List<Guid> parentIds)
{
    var allComments = new List<Record>();

    while (parentIds.Any())
    {
        var comments = await _recordRepository.FilterByAsync(c => parentIds.Contains(c.ParentRecordId ?? Guid.Empty));

        allComments.AddRange(comments);

        parentIds = comments.Select(c => c.Id).ToList();
    }

    return allComments;
}

private List<Record> BuildCommentsHierarchy(Guid recordId, Dictionary<Guid?, List<Record>> commentsDict)
{
    var comments = new List<Record>();

    if (commentsDict.TryGetValue(recordId, out var childComments))
    {
        foreach (var comment in childComments)
        {
            comment.Comments = BuildCommentsHierarchy(comment.Id, commentsDict);
            comments.Add(comment);
        }
    }

    return comments;
}

    public async Task<int> GetTotalRootRecordsCountAsync()
    {
        return await _recordRepository.CountAsync(r => r.ParentRecordId == null);
    }

   
}

public class Response
{
    public bool Success { get; set; }
    public string Message { get; set; }
}
