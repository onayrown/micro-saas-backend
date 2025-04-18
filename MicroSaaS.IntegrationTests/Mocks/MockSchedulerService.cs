using MicroSaaS.Application.Interfaces.Services;
using MicroSaaS.Domain.Entities;
using MicroSaaS.Application.DTOs;
using MicroSaaS.Shared.Enums;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MicroSaaS.Shared.DTOs;
using MicroSaaS.Shared.Results;
using Moq;

namespace MicroSaaS.IntegrationTests.Mocks
{
    public class MockSchedulerService : ISchedulerService
    {
        private readonly List<ScheduledPostDto> _scheduledPosts = new();
        private readonly List<ContentPost> _domainScheduledPosts = new();
        private static readonly Guid FIXED_CREATOR_ID = Guid.Parse("11111111-1111-1111-1111-111111111111");

        public MockSchedulerService()
        {
            InitializeMockData();
        }

        private void InitializeMockData()
        {
            _scheduledPosts.Clear();
            _scheduledPosts.Add(new ScheduledPostDto
            {
                Id = Guid.NewGuid(),
                CreatorId = FIXED_CREATOR_ID,
                Title = "Mock Post 1",
                Content = "Content for mock post 1",
                ScheduledFor = DateTime.UtcNow.AddDays(1),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Platform = SocialMediaPlatform.Twitter,
                Status = PostStatus.Scheduled,
                MediaUrls = new List<string> { "http://example.com/image1.jpg" },
                Tags = new List<string> { "mock", "test" }
            });
            _scheduledPosts.Add(new ScheduledPostDto
            {
                Id = Guid.NewGuid(),
                CreatorId = FIXED_CREATOR_ID,
                Title = "Mock Post 2",
                Content = "Content for mock post 2",
                ScheduledFor = DateTime.UtcNow.AddDays(2),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Platform = SocialMediaPlatform.Facebook,
                Status = PostStatus.Scheduled,
                MediaUrls = new List<string>(),
                Tags = new List<string> { "facebook", "mock" }
            });
            _domainScheduledPosts.Clear();
            _domainScheduledPosts.Add(new ContentPost
            {
                Id = Guid.NewGuid(),
                CreatorId = FIXED_CREATOR_ID,
                Title = "Legacy Mock Post",
                Status = PostStatus.Scheduled,
                ScheduledTime = DateTime.UtcNow.AddDays(3)
            });
        }

        public Task<Result<ScheduledPostDto>> CreateScheduledPostAsync(CreateScheduledPostDto request)
        {
            var newPostDto = new ScheduledPostDto
            {
                Id = Guid.NewGuid(),
                CreatorId = request.CreatorId,
                Title = request.Title,
                Content = request.Content,
                ScheduledFor = request.ScheduledFor,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Platform = request.Platform,
                Status = PostStatus.Scheduled,
                MediaUrls = request.MediaUrls ?? new List<string>(),
                Tags = request.Tags ?? new List<string>()
            };
            _scheduledPosts.Add(newPostDto);
            return Task.FromResult(Result<ScheduledPostDto>.Ok(newPostDto));
        }

        public Task<Result> DeleteScheduledPostAsync(Guid id)
        {
            var postToRemove = _scheduledPosts.FirstOrDefault(p => p.Id == id);
            if (postToRemove == null)
            {
                return Task.FromResult(Result.Fail("Scheduled post not found."));
            }
            _scheduledPosts.Remove(postToRemove);
            return Task.FromResult(Result.Ok());
        }

        public Task<Result<IEnumerable<ScheduledPostDto>>> GetAllScheduledPostsAsync(Guid creatorId)
        {
            var posts = _scheduledPosts.Where(p => p.CreatorId == creatorId).ToList();
            return Task.FromResult(Result<IEnumerable<ScheduledPostDto>>.Ok(posts));
        }

        public Task<Result<ScheduledPostDto>> GetScheduledPostByIdAsync(Guid id)
        {
            var post = _scheduledPosts.FirstOrDefault(p => p.Id == id);
            if (post == null)
            {
                return Task.FromResult(Result<ScheduledPostDto>.Fail("Scheduled post not found."));
            }
            return Task.FromResult(Result<ScheduledPostDto>.Ok(post));
        }

        public Task<Result<ScheduledPostDto>> UpdateScheduledPostAsync(UpdateScheduledPostDto request)
        {
            var postToUpdate = _scheduledPosts.FirstOrDefault(p => p.Id == request.Id);
            if (postToUpdate == null)
            {
                return Task.FromResult(Result<ScheduledPostDto>.Fail("Scheduled post not found."));
            }

            postToUpdate.Title = request.Title ?? postToUpdate.Title;
            postToUpdate.Content = request.Content ?? postToUpdate.Content;
            postToUpdate.ScheduledFor = request.ScheduledFor;
            postToUpdate.UpdatedAt = DateTime.UtcNow;
            postToUpdate.MediaUrls = request.MediaUrls ?? postToUpdate.MediaUrls;
            postToUpdate.Tags = request.Tags ?? postToUpdate.Tags;

            return Task.FromResult(Result<ScheduledPostDto>.Ok(postToUpdate));
        }

        public Task<ContentPost> SchedulePostAsync(ContentPost post)
        {
            Console.WriteLine($"MockSchedulerService: SchedulePostAsync(ContentPost: {post.Id}) called (Legacy).");
            post.Id = post.Id == Guid.Empty ? Guid.NewGuid() : post.Id;
            post.Status = PostStatus.Scheduled;
            _domainScheduledPosts.Add(post);
            return Task.FromResult(post);
        }

        public Task CancelScheduledPostAsync(Guid postId)
        {
            Console.WriteLine($"MockSchedulerService: CancelScheduledPostAsync(PostId: {postId}) called.");
            var post = _scheduledPosts.FirstOrDefault(p => p.Id == postId);
            if (post != null && post.Status == PostStatus.Scheduled)
            {
                _scheduledPosts.Remove(post);
                Console.WriteLine($"MockSchedulerService: Removed ScheduledPostDto {postId}.");
            }
            else
            {
                var domainPost = _domainScheduledPosts.FirstOrDefault(dp => dp.Id == postId);
                if (domainPost != null && domainPost.Status == PostStatus.Scheduled)
                {
                    _domainScheduledPosts.Remove(domainPost);
                    Console.WriteLine($"MockSchedulerService: Removed ContentPost {postId} (Legacy).");
                }
                else { Console.WriteLine($"MockSchedulerService: Post {postId} not found or not cancellable."); }
            }
            return Task.CompletedTask;
        }

        public Task<IEnumerable<ContentPost>> GetScheduledPostsInRangeAsync(DateTime startDate, DateTime endDate)
        {
            Console.WriteLine($"MockSchedulerService: GetScheduledPostsInRangeAsync called (Legacy).");
            var nonNullScheduled = _domainScheduledPosts.Where(c => c.ScheduledTime != null).ToList();
            
            var posts = nonNullScheduled.Where(c => 
                c.ScheduledTime.Value >= startDate &&
                c.ScheduledTime.Value < endDate &&
                c.Status == PostStatus.Scheduled
            ).ToList();
            return Task.FromResult<IEnumerable<ContentPost>>(posts);
        }

        public Task SendUpcomingPostNotificationsAsync(int hoursAhead = 1)
        {
            Console.WriteLine($"MockSchedulerService: SendUpcomingPostNotificationsAsync called (hoursAhead={hoursAhead}).");
            var notificationTime = DateTime.UtcNow.AddHours(hoursAhead);
            var upcomingPostsCount = _scheduledPosts.Count(p => p.Status == PostStatus.Scheduled && p.ScheduledFor <= notificationTime && p.ScheduledFor > DateTime.UtcNow);
            Console.WriteLine($"Mock SendUpcomingPostNotificationsAsync: Found {upcomingPostsCount} posts upcoming.");
            return Task.CompletedTask;
        }

        public Task<ScheduledPostDto> SchedulePostAsync(CreateScheduledPostDto request)
        {
            Console.WriteLine($"MockSchedulerService: SchedulePostAsync(CreateScheduledPostDto) called (Interface demands). Delegating to CreateScheduledPostAsync.");
            var result = CreateScheduledPostAsync(request).Result;
            if (result.Success && result.Data != null)
            {
                return Task.FromResult(result.Data);
            }
            return Task.FromResult(new ScheduledPostDto { Id = Guid.Empty, Title = "Failed Mock Schedule" });
        }

        public Task<ScheduledPostDto?> GetScheduledPostAsync(Guid id)
        {
            Console.WriteLine($"MockSchedulerService: GetScheduledPostAsync(Id: {id}) called.");
            var result = GetScheduledPostByIdAsync(id).Result;
            return Task.FromResult(result.Success ? result.Data : null);
        }

        public Task<ScheduledPostDto?> UpdateScheduledPostAsync(Guid id, UpdateScheduledPostDto request)
        {
            Console.WriteLine($"MockSchedulerService: UpdateScheduledPostAsync(Id: {id}, UpdateScheduledPostDto) called (Interface demands).");
            request.Id = id;

            var result = UpdateScheduledPostAsync(request).Result;
            return Task.FromResult(result.Success ? result.Data : null);
        }

        public Task<List<ScheduledPostDto>> GetScheduledPostsInRangeDtoAsync(DateTime startDate, DateTime endDate)
        {
            Console.WriteLine($"MockSchedulerService: GetScheduledPostsInRangeDtoAsync called.");
            var posts = _scheduledPosts.Where(p => p.ScheduledFor >= startDate && p.ScheduledFor <= endDate);
            return Task.FromResult(posts.ToList());
        }

        public Task StartAsync()
        {
            Console.WriteLine("MockSchedulerService: StartAsync() called.");
            return Task.CompletedTask;
        }

        public Task StopAsync()
        {
            Console.WriteLine("MockSchedulerService: StopAsync() called.");
            return Task.CompletedTask;
        }

        public Task ProcessScheduledPostsAsync()
        {
            Console.WriteLine("MockSchedulerService: ProcessScheduledPostsAsync() called - delegating to SimulateProcessingScheduledPosts.");
            SimulateProcessingScheduledPosts();
            return Task.CompletedTask;
        }

        public void SimulateProcessingScheduledPosts()
        {
            Console.WriteLine("MockSchedulerService: Simulating processing scheduled posts...");
            var now = DateTime.UtcNow;
            var postsToProcess = _scheduledPosts
                .Where(p => p.Status == PostStatus.Scheduled && p.ScheduledFor <= now)
                .ToList();

            foreach (var post in postsToProcess)
            {
                Console.WriteLine($"MockSchedulerService: Simulating processing for Post ID: {post.Id}");
                post.Status = PostStatus.Published;
                post.UpdatedAt = DateTime.UtcNow;
                Console.WriteLine($"MockSchedulerService: Post {post.Id} marked as Published.");
            }
            Console.WriteLine($"MockSchedulerService: Simulated processing for {postsToProcess.Count} posts.");
        }

        public void SetPostStatus(Guid postId, PostStatus status)
        {
            var post = _scheduledPosts.FirstOrDefault(p => p.Id == postId);
            if (post != null)
            {
                post.Status = status;
                post.UpdatedAt = DateTime.UtcNow;
                Console.WriteLine($"MockSchedulerService: Manually set status of Post {postId} to {status}");
            }
            else
            {
                Console.WriteLine($"MockSchedulerService: Could not find Post {postId} to set status.");
            }
        }
    }
} 