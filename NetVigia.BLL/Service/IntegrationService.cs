using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetVigia.BLL.Repository.Interfaces;
using NetVigia.BLL.Service.Interfaces;
using NetVigia.DTO;

namespace NetVigia.BLL.Service
{
    public class IntegrationService : IIntegrationService
    {
        private readonly IIntegrationRepository _repository;
        public IntegrationService(IIntegrationRepository repository)
        {
            _repository = repository;
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
    }
}
