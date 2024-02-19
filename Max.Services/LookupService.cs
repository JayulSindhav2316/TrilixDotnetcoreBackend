
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
using Newtonsoft.Json;

namespace Max.Services
{
    public class LookupService : ILookupService
    {
        private readonly IUnitOfWork _unitOfWork;
        public LookupService(IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }

        public async Task<Lookup> CreateLookup(LookupModel LookupModel)
        {
            Lookup Lookup = new Lookup();
            var isValidLookup = ValidLookup(LookupModel);
            if (isValidLookup)
            {
                //Map Model Data
                Lookup.Group = LookupModel.Group;
                Lookup.Values = LookupModel.Values;
                Lookup.Status = LookupModel.Status;

                await _unitOfWork.Lookups.AddAsync(Lookup);
                await _unitOfWork.CommitAsync();
            }
            return Lookup;
        }

        public async Task<bool> DeleteLookup(int id)
        {
            var Lookup = await _unitOfWork.Lookups.GetByIdAsync(id);
            if (Lookup != null)
            {
                _unitOfWork.Lookups.Remove(Lookup);
                await _unitOfWork.CommitAsync();
                return true;
            }
            return false;

        }

        public async Task<IEnumerable<Lookup>> GetAllLookups()
        {
            return await _unitOfWork.Lookups
                .GetAllLookupsAsync();
        }

        public async Task<string> GetLookupValueByGroupName(string name)
        {
            return await _unitOfWork.Lookups
                .GetLookupValueByGroupNameAsync(name);
        }

        public async Task<Lookup> GetLookupById(int id)
        {
            return await _unitOfWork.Lookups
                .GetLookupByIdAsync(id);
        }

        public async Task<bool> UpdateLookup(LookupModel LookupModel)
        {
            var isValidLookup = ValidLookup(LookupModel);
            if (isValidLookup)
            {
                var Lookup = await _unitOfWork.Lookups.GetByIdAsync(LookupModel.LookupId);

                if (Lookup != null)
                {
                    Lookup.Group = LookupModel.Group;
                    Lookup.Values = LookupModel.Values;
                    Lookup.Status = LookupModel.Status;

                }
                _unitOfWork.Lookups.Update(Lookup);
                await _unitOfWork.CommitAsync();
                return true;
            }
            return false;
        }
        private bool ValidLookup(LookupModel LookupModel)
        {
            //Validate  Name
            if (LookupModel.Group == null)
            {
                throw new NullReferenceException($"Lookup Name can not be NULL.");
            }

            if (LookupModel.Group.Trim() == String.Empty)
            {
                throw new InvalidOperationException($"Lookup Name can not be empty.");
            }           

            return true;
        }

        public async Task<IEnumerable<Answertypelookup>> GetAnswerTypeLookup()
        {
            return await _unitOfWork.AnswerTypeLookUps.GetAnswertypelookup();
        }
        public async Task<IEnumerable<Eventtype>> GetEventTypeLookup()
        {
            return await _unitOfWork.EventTypes.GetAllAsync();
        }
        public async Task<IEnumerable<Timezone>> GetTimezoneLookup()
        {
            return await _unitOfWork.TimeZones.GetAllAsync();
        }
        public async Task<IEnumerable<Registrationfeetype>> GetRegistrationFeeTypes()
        {
            return await _unitOfWork.RegistrationFeeTypes.GetAllAsync();
        }
    }
}