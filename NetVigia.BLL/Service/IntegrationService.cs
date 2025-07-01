using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using NetVigia.BLL.Repository.Interfaces;
using NetVigia.BLL.Service.Interfaces;
using NetVigia.DTO;

namespace NetVigia.BLL.Service
{
    public class IntegrationService : IIntegrationService
    {
        private readonly IIntegrationRepository _repository;
        private readonly ILogger<IntegrationService> _logger;

        public IntegrationService(IIntegrationRepository repository, ILogger<IntegrationService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        private void ValidarDTO(IntegrationDTO dto)
        {
            if (string.IsNullOrEmpty(dto.IntegrationName))
                throw new ArgumentNullException("Preencha o nome da integração");

            if (string.IsNullOrEmpty(dto.IntegrationEndpoint))
                throw new ArgumentNullException("Preencha o endpoint da integração");

            if (dto.IdTGIntegrationMethod == Guid.Empty)
                throw new ArgumentNullException("Selecione um método de integração (WEBHOOK, DISCORD, TELEGRAM, etc...)");

            if (dto.UserId == Guid.Empty)
                throw new ArgumentNullException("Id do usuário não pode ser vazio");

            if (dto.Servers == null || dto.Servers.Any() == false)
                throw new ArgumentNullException("Selecione quais servidores estão conectados a esta integração");
        }

        public async Task CreateAsync(IntegrationDTO dto)
        {
            ValidarDTO(dto);
            dto.Id = Guid.NewGuid();
            dto.DataInclusao = DateTime.UtcNow;

            await _repository.CreateAsync(dto);
        }

        public async Task DeleteAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentNullException("Id da integração não pode ser vazio");
            if ((await _repository.GetByIdAsync(id)) == null)
                throw new KeyNotFoundException();

            await _repository.DeleteAsync(id);
        }


        public async Task<List<IntegrationDTO>> GetByUserAsync(Guid userId)
        {
            return await _repository.GetByUserAsync(userId);
        }

        public async Task UpdateAsync(IntegrationDTO dto)
        {
            ValidarDTO(dto);
            dto.DataAlteracao = DateTime.UtcNow;

            await _repository.UpdateAsync(dto);
        }

        public async Task SendNotificationAsync(string endpoint, object data)
        {
            using var client = new HttpClient();
            var json = JsonSerializer.Serialize(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync(endpoint, content);
            response.EnsureSuccessStatusCode();    
            
        }
    }
}
