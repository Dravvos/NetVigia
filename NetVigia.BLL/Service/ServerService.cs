using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using NetVigia.BLL.Repository.Interfaces;
using NetVigia.BLL.Service.Interfaces;
using NetVigia.DTO;

namespace NetVigia.BLL.Service
{
    public class ServerService : IServerService
    {

        private readonly IServerRepository _repository;
        private readonly ITabelaGeralItemRepository _tabelaGeralItemRepository;

        public ServerService(IServerRepository repository, ITabelaGeralItemRepository tabelaGeralItemRepository)
        {
            _repository = repository;
            _tabelaGeralItemRepository = tabelaGeralItemRepository;
        }

        private async Task ValidateDTO(ServerDTO dto)
        {
            if (dto.CheckIntervalSeconds < 0)
                throw new ArgumentOutOfRangeException("O intervalo de checagem tem que ser maior que zero segundos");
            
            if (dto.IdTGMonitoringType == Guid.Empty)
                throw new ArgumentException("Selecione o tipo de monitoramento");

            if (string.IsNullOrEmpty(dto.URL))
                throw new ArgumentException("A URL não pode ser vazia");

            if (string.IsNullOrEmpty(dto.Name))
                throw new ArgumentException("O nome do servidor não pode ser vazio");

            if (dto.TimeoutInSeconds < 1)
                throw new ArgumentOutOfRangeException("O tempo limite de resposta deve ser maior que zero segundos");

            var tgi = await _tabelaGeralItemRepository.GetByIdAsync(dto.IdTGMonitoringType);
            if (tgi.Sigla == "HTTP" && dto.URL.ToLower().Contains("http") == false)
                throw new ArgumentException("A URL deve começar com http:// ou https:// para o tipo de monitoramento HTTP");

            bool isIP = Regex.Match(dto.URL, "^(25[0-5]|2[0-4]\\d|1\\d\\d|[1-9]?\\d)(\\.(25[0-5]|2[0-4]\\d|1\\d\\d|[1-9]?\\d)){3}$\r\n").Success;
            if (tgi.Sigla == "ICMP" && isIP == false)
                throw new ArgumentException("O tipo de monitoramento 'Ping' requer um endereço IP válido");

            if (tgi.Sigla == "PORT" && isIP == false)
                throw new ArgumentException("O tipo de monitoramento 'Porta' requer um endereço IP válido");

            if (tgi.Sigla == "PORT" && dto.Port <= 0)
                throw new ArgumentOutOfRangeException("O tipo de monitoramento 'Porta' requer uma porta maior que zero");

            if (tgi.Sigla == "HTTP" && dto.IdTGHTTPMethod == Guid.Empty)
                throw new ArgumentException("Selecione o método HTTP para o tipo de monitoramento HTTP");
        }
        public async Task AddAsync(ServerDTO dto)
        {
            await ValidateDTO(dto);
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
            await ValidateDTO(dto);
            dto.DataAlteracao = DateTime.UtcNow;

            await _repository.UpdateAsync(dto);
        }

        public async Task<ServerDTO> GetById(Guid id)
        {
            return await _repository.GetByIdAsync(id);
        }
    }
}
