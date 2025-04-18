using Microsoft.AspNetCore.Mvc;
using MicroSaaS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Extensions.Logging;
using MicroSaaS.Shared.Enums;
using MicroSaaS.Application.DTOs;
using MicroSaaS.Application.DTOs.Checklist;
using MicroSaaS.Shared.DTOs;

namespace MicroSaaS.IntegrationTests.Utils
{
    [ApiController]
    [Route("api/ContentChecklist")]
    public class TestContentChecklistController : ControllerBase
    {
        private readonly ILogger<TestContentChecklistController> _logger;
        private static readonly List<ContentChecklist> _checklists = new List<ContentChecklist>();
        private static readonly object _lock = new object();

        // IDs fixos para testes convertidos para Guid
        private static readonly Guid CREATOR_ID_1 = Guid.Parse("11111111-1111-1111-1111-111111111111");
        private static readonly Guid CREATOR_ID_2 = Guid.Parse("22222222-2222-2222-2222-222222222222");
        private static readonly Guid CHECKLIST_ID_1 = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
        private static readonly Guid CHECKLIST_ID_2 = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");

        public TestContentChecklistController(ILogger<TestContentChecklistController> logger)
        {
            _logger = logger;

            // Inicializar dados de exemplo se a lista estiver vazia
            lock (_lock)
            {
                if (!_checklists.Any())
                {
                    InitializeTestData();
                }
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ContentChecklistDto>>> GetAllChecklists([FromQuery] Guid? creatorId = null)
        {
            _logger.LogInformation("TestContentChecklistController.GetAllChecklists: Buscando checklists para criador {CreatorId}", creatorId);

            var checklists = _checklists;

            if (creatorId.HasValue && creatorId.Value != Guid.Empty)
            {
                checklists = checklists.Where(c => c.CreatorId == creatorId.Value).ToList();
            }

            return Ok(checklists.Select(c => MapToDto(c)));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ContentChecklistDto>> GetById(Guid id)
        {
            _logger.LogInformation("TestContentChecklistController.GetById: Buscando checklist com id {Id}", id);

            var checklist = _checklists.FirstOrDefault(c => c.Id == id);

            if (checklist == null)
            {
                return NotFound();
            }

            return Ok(MapToDto(checklist));
        }

        [HttpGet("creator/{creatorId}")]
        public async Task<ActionResult<List<ContentChecklistDto>>> GetByCreatorId(Guid creatorId)
        {
            _logger.LogInformation("TestContentChecklistController.GetByCreatorId: Buscando checklists para criador {CreatorId}", creatorId);

            var checklists = _checklists.Where(c => c.CreatorId == creatorId).ToList();

            return Ok(checklists.Select(c => MapToDto(c)).ToList());
        }

        [HttpPost]
        public async Task<ActionResult<ContentChecklistDto>> Create([FromBody] CreateChecklistRequestDto request)
        {
            _logger.LogInformation("TestContentChecklistController.Create: Criando checklist para criador {CreatorId}", request.CreatorId);

            var checklist = new ContentChecklist
            {
                Id = Guid.NewGuid(),
                CreatorId = request.CreatorId,
                Title = request.Title,
                Description = request.Description ?? "Descrição padrão",
                Status = ChecklistStatus.NotStarted,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Items = new List<ChecklistItem>()
            };

            lock (_lock)
            {
                _checklists.Add(checklist);
            }

            return CreatedAtAction(nameof(GetById), new { id = checklist.Id }, MapToDto(checklist));
        }

        [HttpPost("{checklistId}/items")]
        public async Task<ActionResult<ContentChecklistDto>> AddChecklistItem(Guid checklistId, [FromBody] AddChecklistItemRequestDto request)
        {
            _logger.LogInformation("TestContentChecklistController.AddChecklistItem: Adicionando item ao checklist {ChecklistId}", checklistId);

            var checklist = _checklists.FirstOrDefault(c => c.Id == checklistId);

            if (checklist == null)
            {
                return NotFound();
            }

            var item = new ChecklistItem
            {
                Id = Guid.NewGuid(),
                Title = $"Item {checklist.Items.Count + 1}",
                Description = request.Description,
                IsRequired = request.IsRequired,
                Priority = TaskPriority.Medium,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            checklist.Items.Add(item);
            checklist.UpdatedAt = DateTime.UtcNow;

            return Ok(MapToDto(checklist));
        }

        [HttpPut("{checklistId}/items/{itemId}")]
        public async Task<IActionResult> UpdateItem(Guid checklistId, Guid itemId, [FromBody] UpdateItemRequestDto request)
        {
            _logger.LogInformation("TestContentChecklistController.UpdateItem: Atualizando item {ItemId} do checklist {ChecklistId}", itemId, checklistId);

            var checklist = _checklists.FirstOrDefault(c => c.Id == checklistId);

            if (checklist == null)
            {
                return NotFound();
            }

            var item = checklist.Items.FirstOrDefault(i => i.Id == itemId);

            if (item == null)
            {
                return NotFound();
            }

            item.IsCompleted = request.IsCompleted;
            if (item.IsCompleted)
            {
                item.CompletedAt = DateTime.UtcNow;
            }
            else
            {
                item.CompletedAt = null;
            }

            item.UpdatedAt = DateTime.UtcNow;
            checklist.UpdatedAt = DateTime.UtcNow;

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteChecklist(Guid id)
        {
            _logger.LogInformation("TestContentChecklistController.DeleteChecklist: Excluindo checklist com id {Id}", id);

            var checklist = _checklists.FirstOrDefault(c => c.Id == id);

            if (checklist == null)
            {
                return NotFound();
            }

            lock (_lock)
            {
                _checklists.Remove(checklist);
            }

            return NoContent();
        }

        private void InitializeTestData()
        {
            // Criar alguns checklists de exemplo
            var checklist1 = new ContentChecklist
            {
                Id = CHECKLIST_ID_1,
                CreatorId = CREATOR_ID_1,
                Title = "Checklist de Teste 1",
                Description = "Este é um checklist de teste para integração",
                Status = ChecklistStatus.InProgress,
                Items = new List<ChecklistItem>
                {
                    new ChecklistItem
                    {
                        Id = Guid.NewGuid(),
                        Title = "Item 1",
                        Description = "Descrição do item 1",
                        Priority = TaskPriority.High,
                        IsCompleted = false,
                        IsRequired = true,
                        CreatedAt = DateTime.UtcNow.AddDays(-10),
                        UpdatedAt = DateTime.UtcNow.AddDays(-10)
                    },
                    new ChecklistItem
                    {
                        Id = Guid.NewGuid(),
                        Title = "Item 2",
                        Description = "Descrição do item 2",
                        Priority = TaskPriority.Medium,
                        IsCompleted = true,
                        IsRequired = false,
                        CreatedAt = DateTime.UtcNow.AddDays(-9),
                        UpdatedAt = DateTime.UtcNow.AddDays(-8)
                    }
                },
                CreatedAt = DateTime.UtcNow.AddDays(-10),
                UpdatedAt = DateTime.UtcNow.AddDays(-5)
            };

            var checklist2 = new ContentChecklist
            {
                Id = CHECKLIST_ID_2,
                CreatorId = CREATOR_ID_2,
                Title = "Checklist de Teste 2",
                Description = "Este é outro checklist de teste para integração",
                Status = ChecklistStatus.NotStarted,
                Items = new List<ChecklistItem>
                {
                    new ChecklistItem
                    {
                        Id = Guid.NewGuid(),
                        Title = "Item 1 do Checklist 2",
                        Description = "Descrição do item 1 do checklist 2",
                        Priority = TaskPriority.Low,
                        IsCompleted = false,
                        IsRequired = false,
                        CreatedAt = DateTime.UtcNow.AddDays(-5),
                        UpdatedAt = DateTime.UtcNow.AddDays(-5)
                    }
                },
                CreatedAt = DateTime.UtcNow.AddDays(-5),
                UpdatedAt = DateTime.UtcNow.AddDays(-5)
            };

            _checklists.Add(checklist1);
            _checklists.Add(checklist2);
        }

        private ContentChecklistDto MapToDto(ContentChecklist checklist)
        {
            return new ContentChecklistDto
            {
                Id = checklist.Id,
                CreatorId = checklist.CreatorId,
                Title = checklist.Title,
                Description = checklist.Description,
                Status = checklist.Status.ToString(),
                Items = checklist.Items.Select(i => new ChecklistItemDto
                {
                    Id = i.Id,
                    Title = i.Title,
                    Description = i.Description,
                    IsCompleted = i.IsCompleted,
                    Priority = i.Priority.ToString(),
                    DueDate = i.DueDate,
                    CreatedAt = i.CreatedAt,
                    UpdatedAt = i.UpdatedAt
                }).ToList(),
                CreatedAt = checklist.CreatedAt,
                UpdatedAt = checklist.UpdatedAt
            };
        }
    }
}