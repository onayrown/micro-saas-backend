using MicroSaaS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroSaaS.Domain.Interfaces;

public interface IContentPerformanceRepository
{
    Task<ContentPerformance> GetByIdAsync(Guid id);
    Task<List<ContentPerformance>> GetByCreatorIdAsync(Guid creatorId);
    Task<List<ContentPerformance>> GetByPostIdAsync(Guid postId);
    Task<ContentPerformance> AddAsync(ContentPerformance performance);
    Task<ContentPerformance> UpdateAsync(ContentPerformance performance);
    Task DeleteAsync(Guid id);
} 