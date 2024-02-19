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
namespace Max.Services
{
    public class CommunicationService : ICommunicationService
    {

        private readonly IUnitOfWork _unitOfWork;
        public CommunicationService(IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Communication>> GetAllCommunications()
        {
            return await _unitOfWork.Communications
                .GetAllCommunicationsAsync();
        }

        public async Task<Communication> GetCommunicationById(int id)
        {
            return await _unitOfWork.Communications
                .GetCommunicationByIdAsync(id);
        }

        public async Task<IEnumerable<Communication>> GetAllCommunicationsByEntityIdId(int entityId)
        {
            return await _unitOfWork.Communications
                .GetAllCommunicationsByEntityIdAsync(entityId);
        }
        public async Task<Communication> CreateCommunication(CommunicationModel model)
        {
            Communication communication = new Communication();
            var isValid = ValidCommunication(model);
            if (isValid)
            {
                //Map Model Data
                communication.EntityId = model.EntityId;
                communication.Date = DateTime.Now;
                communication.From = model.From;
                communication.Subject = model.Subject;
                communication.StaffUserId = model.StaffUserId;
                communication.Start = model.Start;
                communication.End = model.End;
                communication.Notes = model.Notes;
                communication.Scheduled = model.Scheduled;
                communication.Type = model.Type;

                await _unitOfWork.Communications.AddAsync(communication);
                await _unitOfWork.CommitAsync();
            }
            return communication;
        }

        public async Task<Communication> UpdateCommunication(CommunicationModel model)
        {
            Communication communication = await _unitOfWork.Communications.GetCommunicationByIdAsync(model.CommunicationId);

            var isValid =  ValidCommunication(model);
            if (isValid)
            {
                //Map Model Data
                communication.EntityId = model.EntityId;
                communication.From = model.From;
                communication.StaffUserId = model.StaffUserId;
                communication.Start = model.Start;
                communication.End = model.End;
                communication.Notes = model.Notes;
                communication.Scheduled = model.Scheduled;
                communication.Type = model.Type;

                _unitOfWork.Communications.Update(communication);
                await _unitOfWork.CommitAsync();
            }
            return communication;
        }

        public async Task<bool> DeleteCommunication(int communicationId)
        {
            Communication Communication = await _unitOfWork.Communications.GetCommunicationByIdAsync(communicationId);

            if (Communication != null)
            {
                _unitOfWork.Communications.Remove(Communication);
                await _unitOfWork.CommitAsync();
                return true;

            }
            throw new InvalidOperationException($"Communication: {communicationId} not found.");

        }
        private bool ValidCommunication(CommunicationModel model)
        {
            //Validate  Name
            if (model.Subject.IsNullOrEmpty())
            {
                throw new InvalidOperationException($"Subject can not be empty.");
            }

            if (model.From.IsNullOrEmpty())
            {
                throw new NullReferenceException($"Communication From Name can not be NULL or Empty.");
            }

            return true;
        }
    }
}
