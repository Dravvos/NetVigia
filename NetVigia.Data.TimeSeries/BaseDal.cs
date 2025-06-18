using System.ComponentModel.DataAnnotations;
using Apache.IoTDB;
using Apache.IoTDB.DataStructure;
using Microsoft.AspNetCore.Http;
using NetVigia.DTO;

namespace NetVigia.Data.TimeSeries
{
    public class BaseDal
    {
        private readonly int _port;
        private readonly string _host;
        private readonly string _userService;
        private readonly string _passwordService;

        public readonly SessionPool sessionPool;

        public BaseDal()
        {
            var configs = Environment.GetEnvironmentVariable("IotDBConnection").Split(';');
            _host = configs[0].Split('=')[1];
            _port = int.Parse(configs[1].Split('=')[1]);
            _userService = configs[2].Split('=')[1];
            _passwordService = configs[3].Split('=')[1];

            sessionPool = new SessionPool(_host, _port, _userService, _passwordService, 1000000);
        }

    }
}
