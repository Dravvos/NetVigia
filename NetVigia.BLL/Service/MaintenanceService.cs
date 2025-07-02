using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MongoDB.Driver.Core.Servers;
using NetVigia.BLL.Repository.Interfaces;
using NetVigia.BLL.Service.Interfaces;
using NetVigia.DTO;

namespace NetVigia.BLL.Service
{
    public class MaintenanceService : IMaintenanceService
    {
        private readonly IMaintenanceRepository _repository;
        private readonly ILogger<MaintenanceService> _logger;

        public MaintenanceService( IMaintenanceRepository repository, ILogger<MaintenanceService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        private void ValidarDTO(MaintenanceDTO dto)
        {
            if (string.IsNullOrEmpty(dto.Title))
                throw new ArgumentNullException("O título da manutenção não pode ser vazio.");

            if (dto.StartDate == default)
                throw new ArgumentNullException("A data de início da manutenção não pode ser vazia.");

            if (dto.EndDate == default)
                throw new ArgumentNullException("A data de término da manutenção não pode ser vazia.");

            if (dto.UserId == Guid.Empty)
                throw new ArgumentNullException("O ID do usuário não pode ser vazio.");

            if (dto.Servers == null || !dto.Servers.Any())
                throw new ArgumentNullException("A lista de servidores não pode ser vazia.");
        }

        public async Task CreateMaintenanceAsync(MaintenanceDTO maintenance)
        {
            ValidarDTO(maintenance);
            maintenance.Id = Guid.NewGuid();
            maintenance.DataInclusao = DateTime.UtcNow;

            await _repository.CreateMaintenanceAsync(maintenance);
        }

        public async Task DeleteMaintenanceAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentNullException("O ID da manutenção não pode ser vazio.");
            if (_repository.GetMaintenanceByIdAsync(id) == null)
                throw new KeyNotFoundException("Manutenção não encontrada.");

            await _repository.DeleteMaintenanceAsync(id);
        }

        public async Task<List<MaintenanceDTO>> GetAllMaintenanceAsync(Guid userId)
        {
            if (userId == Guid.Empty)
                throw new ArgumentNullException("O ID do usuário não pode ser vazio.");
            return await _repository.GetAllMaintenanceAsync(userId);
        }

        public async Task UpdateMaintenanceAsync(MaintenanceDTO maintenance)
        {
            ValidarDTO(maintenance);
            if (maintenance.Id == Guid.Empty)
                throw new ArgumentNullException("O ID da manutenção não pode ser vazio.");
            if (_repository.GetMaintenanceByIdAsync(maintenance.Id.GetValueOrDefault()) == null)
                throw new KeyNotFoundException("Manutenção não encontrada.");

            maintenance.DataAlteracao = DateTime.UtcNow;

            await _repository.UpdateMaintenanceAsync(maintenance);
        }

        public async Task<bool> IsUnderMaintenanceAsync(Guid serverId)
        {
            try
            {
                var activeMaintenances = await _repository.GetActiveMaintenanceWindowsAsync();
                var now = DateTime.UtcNow;

                return activeMaintenances.Any(x =>
                    x.Servers.Any(wm => wm.Id == serverId) &&
                    x.StartDate <= now &&
                    x.EndDate >= now);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking maintenance status for server {ServerId}", serverId);
                return false;
            }
        }

        public async Task<bool> IsUnderMaintenanceAsync(ServerDTO server)
        {
            try
            {
                var activeMaintenances = await _repository.GetActiveMaintenanceWindowsAsync();
                var now = DateTime.UtcNow;

                return activeMaintenances.Any(x =>
                    x.Servers.Any(wm => wm.Id == server.Id) &&
                    x.StartDate <= now &&
                    x.EndDate >= now);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking maintenance status for server {ServerId}", server.Id);
                return false;
            }
        }

        public async Task<List<MaintenanceDTO>> GetByDateAsync(Guid? serverId, Guid userId, DateTime startDate, DateTime endDate)
        {
            return await _repository.GetByDateAsync(serverId, userId, startDate, endDate);
        }
    }
}
