﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Test_Comments.Entities.RecordGroup.Repository;
using Test_Comments.Models.RecordModels;
using Test_Comments.Services;

public interface IRecordService
{
    Task<Response> AddRecordAsync(RecordRequest request, IFormFile file, Guid userId);
    Task<Response> AddCommentAsync(Guid parentRecordId, RecordController.CommentRequest request, IFormFile? file, Guid userId);
    Task<List<Record>> GetPagedRootRecordsWithCommentsAsync(int page, int pageSize, string sortField, string sortDirection);
    Task<int> GetTotalRootRecordsCountAsync();
}

public class RecordService : IRecordService
{
    private readonly IRecordRepository<Record> _recordRepository;
    private readonly IUserService _userService;

    public RecordService(IRecordRepository<Record> recordRepository, IUserService userService)
    {
        _recordRepository = recordRepository;
        _userService = userService;
    }

    public async Task<Response> AddRecordAsync(RecordRequest request, IFormFile file, Guid userId)
    {
        try
        {
            var user = await _userService.GetUserAsync(userId);
            if (user == null)
            {
                return new Response { Success = false, Message = "Користувача не знайдено" };
            }

            var record = new Record
            {
                Id = Guid.NewGuid(),
                UserName = user.Name,
                Email = user.Email,
                Text = request.Text,
                CreationDate = DateTime.UtcNow
            };

            if (file != null)
            {
                if (file.Length > 100 * 1024)
                {
                    return new Response { Success = false, Message = "Файл перевищує максимальний розмір 100 КБ" };
                }

                record.FileName = file.FileName;
                record.FileType = file.ContentType;

                using (var ms = new MemoryStream())
                {
                    await file.CopyToAsync(ms);
                    record.FileData = ms.ToArray();
                }
            }

            await _recordRepository.InsertOneAsync(record);
            return new Response { Success = true, Message = "Запис успішно додано" };
        }
        catch (Exception ex)
        {
            return new Response { Success = false, Message = $"Помилка: {ex.Message}" };
        }
    }

    public async Task<Response> AddCommentAsync(Guid parentRecordId, RecordController.CommentRequest request, IFormFile? file, Guid userId)
    {
        try
        {
            var parentRecord = await _recordRepository.FindByIdAsync(parentRecordId);
            if (parentRecord == null)
            {
                return new Response { Success = false, Message = "Батьківський запис не знайдено" };
            }

            var user = await _userService.GetUserAsync(userId);
            if (user == null)
            {
                return new Response { Success = false, Message = "Користувача не знайдено" };
            }

            var newComment = new Record
            {
                Id = Guid.NewGuid(),
                UserName = user.Name,
                Email = user.Email,
                Text = request.Text,
                ParentRecordId = parentRecordId,
                CreationDate = DateTime.UtcNow
            };

            if (file != null)
            {
                if (file.Length > 100 * 1024) // Максимальний розмір файлу 100 КБ
                {
                    return new Response { Success = false, Message = "Файл перевищує максимальний розмір 100 КБ" };
                }

                newComment.FileName = file.FileName;
                newComment.FileType = file.ContentType;

                using (var ms = new MemoryStream())
                {
                    await file.CopyToAsync(ms);
                    newComment.FileData = ms.ToArray();
                }
            }

            await _recordRepository.InsertOneAsync(newComment);
            return new Response { Success = true, Message = "Коментар успішно додано" };
        }
        catch (Exception ex)
        {
            return new Response { Success = false, Message = $"Помилка: {ex.Message}" };
        }
    }

    public async Task<List<Record>> GetPagedRootRecordsWithCommentsAsync(int page, int pageSize, string sortField, string sortDirection)
    {
        int skip = (page - 1) * pageSize;

        Expression<Func<Record, object>> sortExpression = sortField.ToLower() switch
        {
            "username" => r => r.UserName,
            "email" => r => r.Email,
            "creationdate" => r => r.CreationDate,
            _ => r => r.CreationDate
        };

        bool ascending = sortDirection.ToLower() == "asc";

        var pagedRootRecords = await _recordRepository.SortFilterBySkipAsync(
            r => r.ParentRecordId == null,
            sortExpression,
            ascending,
            skip,
            pageSize
        );

        var rootRecordIds = pagedRootRecords.Select(r => r.Id).ToList();
        var allCommentsForRootRecords = await GetAllCommentsRecursively(rootRecordIds);
        var commentsDict = allCommentsForRootRecords
            .GroupBy(c => c.ParentRecordId)
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
