using System.Globalization;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using NetVigia.BLL.Repository.Interfaces;
using NetVigia.Data;
using NetVigia.Data.Models;
using NetVigia.DTO;

namespace NetVigia.BLL.Repository
{
    public class MaintenanceRepository : IMaintenanceRepository
    {
        private readonly UptimeContext con;

        public MaintenanceRepository(UptimeContext con)
        {
            this.con = con;
        }

        public async Task CreateMaintenanceAsync(MaintenanceDTO dto)
        {
            var model = Map<MaintenanceModel>.Convert(dto);
            model.Servers.Clear();
            model.StartDate = DateTime.SpecifyKind(model.StartDate, DateTimeKind.Utc);
            model.EndDate = DateTime.SpecifyKind(model.EndDate, DateTimeKind.Utc);
            await con.Maintenances.AddAsync(model);
            await con.SaveChangesAsync();
            foreach (var server in dto.Servers)
            {
                var msModel = new MaintenanceServerModel
                {
                    DataInclusao = DateTime.UtcNow,
                    UsuarioInclusao = model.UsuarioInclusao,
                    Id = Guid.NewGuid(),
                    MaintenanceId = model.Id,
                    ServerId = server.Id.GetValueOrDefault()
                };
                await con.MaintenanceServers.AddAsync(msModel);
            }
            await con.SaveChangesAsync();
        }

        public Task DeleteMaintenanceAsync(Guid id)
        {
            var maintenance = con.Maintenances.FirstOrDefault(x => x.Id == id);
            con.MaintenanceServers.RemoveRange(con.MaintenanceServers.Where(x => x.MaintenanceId == id));
            con.Maintenances.Remove(maintenance);
            return con.SaveChangesAsync();
        }

        public async Task<IEnumerable<MaintenanceDTO>> GetActiveMaintenanceWindowsAsync()
        {
            var now = DateTime.UtcNow;
            var model = await con.Maintenances.Where(x => now >= x.StartDate && now <= x.EndDate).ToListAsync();

            foreach (var item in model)
            {
                item.Servers = await con.MaintenanceServers
                    .Where(x => x.MaintenanceId == item.Id)
                    .Select(x => x.Server)
                    .ToListAsync();
            }

            return Map<List<MaintenanceDTO>>.Convert(model);
        }

        public async Task<List<MaintenanceDTO>> GetAllMaintenanceAsync(Guid userId)
        {
            var model = await con.Maintenances.Where(x => x.UserId == userId).Include(x => x.Servers).ToListAsync();


            foreach (var item in model)
            {
                var listAux = await con.MaintenanceServers.Where(x => x.MaintenanceId == item.Id).ToListAsync();
                var serversIds = listAux.Select(x => x.ServerId).Distinct().ToList();
                item.Servers = await con.Servers.Where(x => serversIds.Contains(x.Id)).Include(x => x.MonitoringType).Include(x => x.HTTPMethod).ToListAsync();
            }

            var dto = Map<List<MaintenanceDTO>>.Convert(model);

            return dto;
        }

        public async Task<TimeSpan> GetAverageMaintenanceDuration(DateTime startDate, DateTime endDate, List<Guid> serverIds)
        {
            var model = await con.Maintenances.Where(x => x.StartDate >= startDate && x.EndDate <= endDate).ToListAsync();

            foreach (var item in model)
            {
                var listAux = await con.MaintenanceServers.Where(x => x.MaintenanceId == item.Id).ToListAsync();
                var serversIds = listAux.Select(x => x.ServerId).Distinct().ToList();
                item.Servers = await con.Servers.Where(x => serversIds.Contains(x.Id)).Include(x => x.MonitoringType).Include(x => x.HTTPMethod).ToListAsync();
            }

            if (serverIds != null && serverIds.Count > 0)
            {
                model = model.Where(x => x.Servers.Any(s => serverIds.Contains(s.Id))).ToList();
            }

            var times = new List<TimeSpan>();

            foreach (var item in model)
            {
                var duration = item.EndDate - item.StartDate;
                if (duration.TotalSeconds > 0)
                {
                    times.Add(duration);
                }
            }

            if (times.Count > 0)
            {
                var averageTicks = times.Sum(x => x.Ticks) / times.Count;
                return new TimeSpan(averageTicks);
            }

            return new TimeSpan();
        }

        public async Task<List<MaintenanceDTO>> GetByDateAsync(List<Guid> serverIds, DateTime startDate, DateTime endDate)
        {
            var model = await con.Maintenances.Where(x => x.StartDate >= startDate && x.EndDate <= endDate).ToListAsync();

            foreach (var item in model)
            {
                var listAux = await con.MaintenanceServers.Where(x => x.MaintenanceId == item.Id).ToListAsync();
                var serversIds = listAux.Select(x => x.ServerId).Distinct().ToList();
                item.Servers = await con.Servers.Where(x => serversIds.Contains(x.Id)).Include(x => x.MonitoringType).Include(x => x.HTTPMethod).ToListAsync();
            }

            if (serverIds != null && serverIds.Count > 0)
            {
                model = model.Where(x => x.Servers.Any(s => serverIds.Contains(s.Id))).ToList();
            }

            var dto = Map<List<MaintenanceDTO>>.Convert(model);
            return dto;
        }

        public async Task<Dictionary<string, int>> GetGroupedByDateAsync(Guid groupingType, List<Guid> serverIds, DateTime startDate, DateTime endDate)
        {
            startDate = DateTime.SpecifyKind(startDate, DateTimeKind.Utc);
            endDate = DateTime.SpecifyKind(endDate, DateTimeKind.Utc);
            var model = await con.Maintenances.Where(x => x.StartDate >= startDate && x.EndDate <= endDate).ToListAsync();

            foreach (var item in model)
            {
                var listAux = await con.MaintenanceServers.Where(x => x.MaintenanceId == item.Id).ToListAsync();
                var serversIds = listAux.Select(x => x.ServerId).Distinct().ToList();
                item.Servers = await con.Servers.Where(x => serversIds.Contains(x.Id)).Include(x => x.MonitoringType).Include(x => x.HTTPMethod).ToListAsync();
            }

            if (serverIds != null && serverIds.Count > 0)
            {
                model = model.Where(x => x.Servers.Any(s => serverIds.Contains(s.Id))).ToList();
            }

            var tgi = await con.TabelaGeralItem.FirstOrDefaultAsync(x => x.Id == groupingType);
            var groups = new Dictionary<string, int>();
            switch (tgi.Sigla)
            {
                case "DIA":
                    groups = model.GroupBy(x => x.StartDate.Date).Select(x => new KeyValuePair<string, int>
                    (x.Key.ToShortDateString(), x.Count())).ToDictionary(x => x.Key, x => x.Value);
                    break;
                case "MES":
                    groups = model.GroupBy(x => new DateTime(x.StartDate.Year, x.StartDate.Month, 1))
                        .Select(x => new KeyValuePair<string, int>
                        (x.Key.ToString("MMM yyyy"), x.Count())).ToDictionary(x => x.Key, x => x.Value);
                    break;
                case "ANO":
                    groups = model.GroupBy(x => x.StartDate.Year)
                        .Select(x => new KeyValuePair<string, int>
                        (x.Key.ToString(), x.Count())).ToDictionary(x => x.Key, x => x.Value);
                    break;
                case "SEM":
                    groups = model.GroupBy(x =>
                    {
                        var date = x.StartDate;
                        DateTime weekStart = date.StartOfWeek();
                        var culture = CultureInfo.CurrentCulture;
                        var firstDayOfWeek = culture.DateTimeFormat.FirstDayOfWeek;
                        int diff = (7 + (date.DayOfWeek - firstDayOfWeek)) % 7;

                        bool startsInDifferentMonth = weekStart.Month != date.Month;
                        bool startsInDifferentYear = weekStart.Year != date.Year;

                        if (startsInDifferentMonth || startsInDifferentYear)
                        {
                            if (date.Day < 7)
                                diff = date.Day - 1; 

                            return date.AddDays(-1 * diff).Date.ToString("MMM yyyy");
                        }
                        else
                            return date.AddDays(-1 * diff).Date.ToString("MMM yyyy");
                    })
                        .Select(x => new KeyValuePair<string, int>
                        (x.Key, x.Count())).ToDictionary(x => x.Key, x => x.Value);

                    break;
                case "HORA":
                    groups = model.GroupBy(x => new DateTime(x.StartDate.Year, x.StartDate.Month, x.StartDate.Day, x.StartDate.Hour, 0, 0)).Select(x => new KeyValuePair<string, int>
                    (x.Key.ToLocalTime().ToString("dd-MM-yyyy HH:mm:ss"), x.Count())).ToDictionary(x => x.Key, x => x.Value);
                    break;
                default:
                    break;
            }




            return groups;
        }

        public async Task<MaintenanceDTO> GetMaintenanceByIdAsync(Guid id)
        {
            var model = await con.Maintenances.Include(x => x.Servers).FirstOrDefaultAsync(x => x.Id == id);
            return Map<MaintenanceDTO>.Convert(model);
        }

        public async Task<TimeSpan> GetTotalMaintenanceDuration(DateTime startDate, DateTime endDate, List<Guid> serverIds)
        {
            var model = await con.Maintenances.Where(x => x.StartDate >= startDate && x.EndDate <= endDate).ToListAsync();

            foreach (var item in model)
            {
                var listAux = await con.MaintenanceServers.Where(x => x.MaintenanceId == item.Id).ToListAsync();
                var serversIds = listAux.Select(x => x.ServerId).Distinct().ToList();
                item.Servers = await con.Servers.Where(x => serversIds.Contains(x.Id)).Include(x => x.MonitoringType).Include(x => x.HTTPMethod).ToListAsync();
            }

            if (serverIds != null && serverIds.Count > 0)
            {
                model = model.Where(x => x.Servers.Any(s => serverIds.Contains(s.Id))).ToList();
            }

            var times = new List<TimeSpan>();

            foreach (var item in model)
            {
                var duration = item.EndDate - item.StartDate;
                if (duration.TotalSeconds > 0)
                {
                    times.Add(duration);
                }
            }
            return new TimeSpan(times.Sum(x => x.Ticks));
        }

        public async Task<IEnumerable<MaintenanceDTO>> GetUpcomingMaintenanceWindowsAsync()
        {
            var model = await con.Maintenances.Where(x => x.StartDate > DateTime.UtcNow).ToListAsync();
            return Map<List<MaintenanceDTO>>.Convert(model);
        }

        public async Task UpdateMaintenanceAsync(MaintenanceDTO dto)
        {
            var model = await con.Maintenances.FirstAsync(x => x.Id == dto.Id);

            model.StartDate = DateTime.SpecifyKind(dto.StartDate, DateTimeKind.Utc);
            model.EndDate = DateTime.SpecifyKind(dto.EndDate, DateTimeKind.Utc);
            model.Title = dto.Title;
            model.DataAlteracao = dto.DataAlteracao;
            model.UsuarioAlteracao = dto.UsuarioAlteracao;


            var servers = await con.MaintenanceServers
                .Where(x => x.MaintenanceId == dto.Id).ToListAsync();

            var serversToRemove = servers.Where(x => !dto.Servers.Any(s => s.Id == x.ServerId)).ToList();

            foreach (var server in serversToRemove)
                con.MaintenanceServers.Remove(server);

            var serversToAdd = dto.Servers.Where(s => !servers.Any(x => x.ServerId == s.Id)).ToList();
            foreach (var server in serversToAdd)
            {
                await con.MaintenanceServers.AddAsync(new MaintenanceServerModel
                {
                    DataInclusao = model.DataInclusao,
                    UsuarioInclusao = model.UsuarioInclusao,
                    Id = Guid.NewGuid(),
                    MaintenanceId = model.Id,
                    ServerId = server.Id.GetValueOrDefault()
                });
            }


            await con.SaveChangesAsync();
        }
    }
    public static class DateTimeExtensions
    {
        public static DateTime StartOfWeek(this DateTime dt, DayOfWeek startOfWeek = DayOfWeek.Sunday)
        {
            int diff = (7 + (dt.DayOfWeek - startOfWeek)) % 7;
            return dt.AddDays(-1 * diff).Date;
        }
    }
}
