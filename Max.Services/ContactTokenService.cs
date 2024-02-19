using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Max.Data.Interfaces;
using Max.Data.DataModel;
using Max.Services.Interfaces;
using Max.Core.Models;
using System.Linq;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Max.Core;

namespace Max.Services
{
    public class ContactTokenService : IContactTokenService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ContactTokenService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this._unitOfWork = unitOfWork;
            this._mapper = mapper;
        }
        public async Task<Contacttoken> CreateContactToken(string email, string ipAddress)
        {
            Contacttoken contactToken = new Contacttoken();

            int min = 100000;
            int max = 999999;
            Random _random = new Random();
            var token =  _random.Next(min, max);

            contactToken.Email = email;
            contactToken.IpAddress = ipAddress;
            contactToken.Token = token.ToString();
            contactToken.Status = (int)Status.Active;
            contactToken.Create = DateTime.Now;
            contactToken.Expire = DateTime.Now.AddMinutes(30);

            await _unitOfWork.ContactTokens.AddAsync(contactToken);
            await _unitOfWork.CommitAsync();
          
            return contactToken;
        }

        public async Task<Contacttoken> GetContactTokenByEmailId(string email)
        {
            var contactToken = await _unitOfWork.ContactTokens.GetTokenRequestByEmailAsync(email);

            return contactToken;
        }

        public async Task<bool> ValidateContactToken(string email,string token)
        {

            var contactToken = await _unitOfWork.ContactTokens.GetTokenRequestByTokenAsync(token);

            if(contactToken == null) return false;

            if (contactToken.Email != email) return false;

            if (contactToken.Status == (int)Status.InActive) return false;

            if (contactToken.Expire < DateTime.Now) return false;

            return true;
        }

        public async Task<bool> UpdateContactToken(string email,string token)
        {
            var contactToken = await _unitOfWork.ContactTokens.GetTokenRequestByTokenAsync(token);
            if (contactToken != null)
            {
                if(contactToken.Email != email) return false;
                try
                {
                    contactToken.Status = (int)Status.InActive;
                    contactToken.Expire = DateTime.Now;
                    _unitOfWork.ContactTokens.Update(contactToken);
                    await _unitOfWork.CommitAsync();
                    return true;
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"Failed to update contact token.");
                }
            }
            return false;
        }

    }
}
