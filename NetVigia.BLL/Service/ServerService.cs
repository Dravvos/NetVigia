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
    public class ServerService : IServerService
    {

        private readonly IServerRepository _repository;

        public ServerService(IServerRepository repository)
        {
            _repository = repository;
        }

        private void ValidateDTO(ServerDTO dto)
        {
            if (dto.CheckIntervalSeconds < 0)
            {
                throw new ArgumentOutOfRangeException("O intervalo de checagem tem que ser maior que zero segundos");
            }
            if(dto.IdTGMonitoringType == Guid.Empty)
                throw new ArgumentException("Selecione o tipo de monitoramento");

            if(string.IsNullOrEmpty(dto.URL))
                throw new ArgumentException("A URL não pode ser vazia");

            if (string.IsNullOrEmpty(dto.Name))
                throw new ArgumentException("O nome do servidor não pode ser vazio");

            if(dto.TimeoutInSeconds < 1)
                throw new ArgumentOutOfRangeException("O tempo limite de resposta deve ser maior que zero segundos");

        }
        public async Task AddAsync(ServerDTO dto)
        {
            ValidateDTO(dto);
            dto.Id = Guid.NewGuid();
            dto.DataInclusao = DateTime.UtcNow;

            await _repository.AddAsync(dto);
        }

        public async Task DeleteAsync(Guid id)
        {
            var model = await _repository.GetByIdAsync(id);
            if (model == null)
                throw new KeyNotFoundException();

            await _repository.DeleteAsync(id);
        }

        public async Task<List<ServerDTO>> GetAllAsync(Guid userId)
        {
            return await _repository.GetAllAsync(userId);
        }

        public async Task<List<ServerDTO>> GetByNomeAsync(Guid userId, string nome)
        {
            return await _repository.GetByNomeAsync(userId, nome);
        }

        public async Task UpdateAsync(ServerDTO dto)
        {
            ValidateDTO(dto);
            dto.DataAlteracao = DateTime.UtcNow;

            await _repository.UpdateAsync(dto);
        }
    }
}
