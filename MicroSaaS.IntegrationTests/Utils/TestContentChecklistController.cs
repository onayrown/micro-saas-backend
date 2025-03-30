using Microsoft.AspNetCore.Mvc;
using MicroSaaS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace MicroSaaS.IntegrationTests.Utils
{
    [ApiController]
    [Route("api/ContentChecklist")]
    public class TestContentChecklistController : ControllerBase
    {
        private readonly ILogger<TestContentChecklistController> _logger;
        private static readonly List<ContentChecklist> _checklists = new List<ContentChecklist>();
        private static readonly List<ChecklistItem> _items = new List<ChecklistItem>();

        public TestContentChecklistController(ILogger<TestContentChecklistController> logger)
        {
            _logger = logger;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ContentChecklist>> GetById(Guid id)
        {
            _logger.LogInformation("TestContentChecklistController.GetById: Buscando checklist {Id}", id);
            
            var checklist = _checklists.FirstOrDefault(c => c.Id == id);
            if (checklist == null)
                return NotFound();

            return Ok(checklist);
        }

        [HttpGet("creator/{creatorId}")]
        public async Task<ActionResult<List<ContentChecklist>>> GetByCreatorId(Guid creatorId)
        {
            _logger.LogInformation("TestContentChecklistController.GetByCreatorId: Buscando checklists do criador {Id}", creatorId);
            
            var creatorChecklists = _checklists.Where(c => c.CreatorId == creatorId).ToList();
            return Ok(creatorChecklists);
        }

        [HttpPost]
        public async Task<ActionResult<ContentChecklist>> Create([FromBody] CreateChecklistRequest request)
        {
            _logger.LogInformation("TestContentChecklistController.Create: Criando checklist para criador {CreatorId}", request.CreatorId);
            
            var checklist = new ContentChecklist
            {
                Id = Guid.NewGuid(),
                CreatorId = request.CreatorId,
                Title = request.Title,
                Description = "Checklist gerado automaticamente para testes",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Items = new List<ChecklistItem>(),
                Creator = new ContentCreator
                {
                    Id = request.CreatorId,
                    Name = "Criador de Teste",
                    Email = "teste@email.com",
                    Username = "teste_username"
                }
            };

            _checklists.Add(checklist);
            
            return CreatedAtAction(nameof(GetById), new { id = checklist.Id }, checklist);
        }

        [HttpPost("{checklistId}/items")]
        public async Task<ActionResult<ContentChecklist>> AddItem(Guid checklistId, [FromBody] AddChecklistItemRequest request)
        {
            _logger.LogInformation("TestContentChecklistController.AddItem: Adicionando item ao checklist {ChecklistId}", checklistId);
            
            var checklist = _checklists.FirstOrDefault(c => c.Id == checklistId);
            if (checklist == null)
                return NotFound();

            var item = new ChecklistItem
            {
                Id = Guid.NewGuid(),
                Title = "Item de Teste",
                Description = request.Description,
                IsRequired = request.IsRequired,
                IsCompleted = false,
                CreatedAt = DateTime.UtcNow
            };

            if (checklist.Items == null)
                checklist.Items = new List<ChecklistItem>();
                
            checklist.Items.Add(item);
            _items.Add(item);
            
            return Ok(checklist);
        }

        [HttpPut("{checklistId}/items/{itemId}")]
        public async Task<IActionResult> UpdateItem(Guid checklistId, Guid itemId, [FromBody] UpdateItemRequest request)
        {
            _logger.LogInformation("TestContentChecklistController.UpdateItem: Atualizando item {ItemId} do checklist {ChecklistId}", itemId, checklistId);
            
            var checklist = _checklists.FirstOrDefault(c => c.Id == checklistId);
            if (checklist == null)
                return NotFound();

            var item = checklist.Items?.FirstOrDefault(i => i.Id == itemId);
            if (item == null)
                return NotFound();

            item.IsCompleted = request.IsCompleted;
            if (request.IsCompleted)
            {
                item.CompletedAt = DateTime.UtcNow;
            }
            
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            _logger.LogInformation("TestContentChecklistController.Delete: Excluindo checklist {Id}", id);
            
            var checklist = _checklists.FirstOrDefault(c => c.Id == id);
            if (checklist == null)
                return NotFound();

            _checklists.Remove(checklist);
            return NoContent();
        }
    }

    public class CreateChecklistRequest
    {
        public Guid CreatorId { get; set; }
        public string Title { get; set; }
    }

    public class AddChecklistItemRequest
    {
        public string Description { get; set; }
        public bool IsRequired { get; set; }
    }

    public class UpdateItemRequest
    {
        public bool IsCompleted { get; set; }
    }
} 