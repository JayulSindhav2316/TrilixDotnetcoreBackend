using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Max.Data.Repositories;
using Max.Data.Interfaces;
using Max.Data.DataModel;
using Max.Services.Interfaces;
using Max.Core.Models;
using Max.Core;
using Max.Data;
using Max.Services;
using System.Linq;
using AutoMapper;
using Serilog;

namespace Max.Services
{
    public class ClientLogService: IClientLogService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        static readonly ILogger _logger = Serilog.Log.ForContext<ClientLogService>();
        public ClientLogService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this._unitOfWork = unitOfWork;
            this._mapper = mapper;
           
        }

        public async Task<Clientlog> CreateLog(ClientLogModel model)
        {
            Clientlog clientLog = new Clientlog();

            //Map Model Data
            clientLog = _mapper.Map<Clientlog>(model);
            clientLog.TimeStamp = DateTime.Now;
            try
            {
                await _unitOfWork.ClientLogs.AddAsync(clientLog);
                await _unitOfWork.CommitAsync();
            }
            catch (Exception ex)
            {
                _logger.Error($"Failed to create Log entry:{ex.Message}");
            }

            return clientLog;
        }

        public async Task<List<ClientLogModel>> GetCleintLogs()
        {
            var clientLogs = await _unitOfWork.ClientLogs.GetAllLientLogsAsync();
            return _mapper.Map<List<ClientLogModel>>(clientLogs);
        }
    }
}
