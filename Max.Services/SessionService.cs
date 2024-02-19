using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using iTextSharp.text.pdf;
using Max.Core.Helpers;
using Max.Core.Models;
using Max.Data.DataModel;
using Max.Data.Interfaces;
using Max.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Max.Services
{
    public class SessionService : ISessionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<SessionService> _logger;
        private readonly ITenantService _tenantService;
        private readonly IQuestionLinkService _questionLinkService;
        private readonly ILinkEventFeeTypeService _linkEventFeeTypeService;
        private readonly IDocumentService _documentService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IHostEnvironment _appEnvironment;
        public SessionService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<SessionService> logger, ITenantService tenantService, IHostEnvironment appEnvironment,
            IQuestionLinkService questionLinkService, ILinkEventFeeTypeService linkEventFeeTypeService, IDocumentService documentService, IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _tenantService = tenantService;
            _questionLinkService = questionLinkService;
            _linkEventFeeTypeService = linkEventFeeTypeService;
            _documentService = documentService;
            _httpContextAccessor = httpContextAccessor;
            _appEnvironment = appEnvironment;
        }

        public async Task<SessionModel> CreateSession(SessionModel model)
        {
            SessionModel sessionModel = new SessionModel();
            bool isValid = await ValidateSessionDetails(model);

            if (isValid)
            {
                try
                {
                    Session session = new Session();
                    session.EventId = model.EventId;
                    session.Code = model.Code == string.Empty || model.Code == null ? await GenerateSessionCode() : model.Code;
                    session.Name = model.Name;
                    session.StartDatetime = model.StartDatetime;
                    session.EndDateTime = model.EndDateTime;
                    session.MaxCapacity = model.MaxCapacity;
                    session.GlAccountId = model.GlAccountId ?? 0;
                    session.Status = model.Status;
                    session.Location = model.Location;

                    await _unitOfWork.Sessions.AddAsync(session);
                    await _unitOfWork.CommitAsync();

                    if (model.SessionLeaders.Count() > 0)
                    {
                        foreach (var leader in model.SessionLeaders)
                        {
                            Sessionleaderlink sessionLeader = new Sessionleaderlink();
                            sessionLeader.SessionId = session.SessionId;
                            sessionLeader.EntityId = leader.EntityId;
                            await _unitOfWork.SessionLeaderLinks.AddAsync(sessionLeader);
                        }
                        await _unitOfWork.CommitAsync();
                    }

                    if (model.SessionQuestions.Count() > 0)
                    {
                        foreach (var question in model.SessionQuestions)
                        {
                            Questionlink questionlink = new Questionlink();
                            questionlink.SessionId = session.SessionId;
                            questionlink.QuestionBankId = question.QuestionBankId;
                            await _unitOfWork.QuestionLinks.AddAsync(questionlink);
                        }
                        await _unitOfWork.CommitAsync();
                    }

                    if (model.GroupPricing.Count() > 0)
                    {
                        foreach (var group in model.GroupPricing)
                        {
                            foreach (var pricing in group.GroupPriorityFeeSettings)
                            {
                                Sessionregistrationgrouppricing sessionRegistrationGroupPricing = new Sessionregistrationgrouppricing();
                                sessionRegistrationGroupPricing.SessionId = session.SessionId;
                                sessionRegistrationGroupPricing.RegistrationGroupId = pricing.RegistrationGroupId;
                                sessionRegistrationGroupPricing.RegistrationFeeTypeId = pricing.RegistrationFeeTypeId;
                                sessionRegistrationGroupPricing.Price = pricing.Price;

                                await _unitOfWork.SessionRegistrationGroupPricings.AddAsync(sessionRegistrationGroupPricing);
                            }
                        }
                        await _unitOfWork.CommitAsync();
                    }

                    if (model.EventId != null)
                    {
                        var eventDetails = await _unitOfWork.Events.GetByIdAsync(Convert.ToInt32(model.EventId));
                        if(eventDetails!=null)
                        {
                            eventDetails.Status = 1;
                            _unitOfWork.Events.Update(eventDetails);
                            await _unitOfWork.CommitAsync();
                        }
                    }

                    sessionModel = _mapper.Map<SessionModel>(session);
                }
                catch (Exception ex)
                {
                    _logger.LogError("An error occurred {Error} {StackTrace} {InnerException} {Source}",
                                   ex.Message, ex.StackTrace, ex.InnerException, ex.Source);
                }
            }

            return sessionModel;
        }

        public async Task<bool> UpdateSession(SessionModel model)
        {
            var response = false;
            bool isValid = await ValidateSessionDetails(model);

            if (isValid)
            {
                try
                {
                    Session session = await _unitOfWork.Sessions.GetSessionByIdAsync(model.SessionId);
                    session.Code = model.Code != session.Code ? model.Code : session.Code;
                    session.Code = session.Code == null ? await GenerateSessionCode() : model.Code;
                    session.Name = model.Name;
                    session.StartDatetime = model.StartDatetime;
                    session.EndDateTime = model.EndDateTime;
                    session.MaxCapacity = model.MaxCapacity;
                    session.GlAccountId = model.GlAccountId ?? 0;
                    session.Status = model.Status;
                    session.Location = model.Location;

                    _unitOfWork.Sessions.Update(session);

                    var sessionLeaderList = await _unitOfWork.SessionLeaderLinks.GetSessionLeadersBySessionIdAsync(model.SessionId);
                    if (sessionLeaderList.Count() > 0)
                    {
                        _unitOfWork.SessionLeaderLinks.RemoveRange(sessionLeaderList);
                        await _unitOfWork.CommitAsync();
                    }
                    if (model.SessionLeaders.Count() > 0)
                    {
                        foreach (var leader in model.SessionLeaders)
                        {
                            Sessionleaderlink sessionLeader = new Sessionleaderlink();
                            sessionLeader.SessionId = leader.SessionId;
                            sessionLeader.EntityId = leader.EntityId;
                            await _unitOfWork.SessionLeaderLinks.AddAsync(sessionLeader);
                        }
                        await _unitOfWork.CommitAsync();
                    }

                    var questionLinked = await _questionLinkService.LinkQuestionsBySessionId(model);

                    if (model.GroupPricing.Count() > 0)
                    {
                        var sessionGroupPricing = await _unitOfWork.SessionRegistrationGroupPricings.GetSessionPricingBySessionIdAsync(model.SessionId);
                        if (sessionGroupPricing.Count() > 0)
                        {
                            _unitOfWork.SessionRegistrationGroupPricings.RemoveRange(sessionGroupPricing);
                        }
                        foreach (var group in model.GroupPricing)
                        {
                            foreach (var pricing in group.GroupPriorityFeeSettings)
                            {
                                Sessionregistrationgrouppricing sessionRegistrationGroupPricing = new Sessionregistrationgrouppricing();
                                sessionRegistrationGroupPricing.SessionId = model.SessionId;
                                sessionRegistrationGroupPricing.RegistrationGroupId = pricing.RegistrationGroupId;
                                sessionRegistrationGroupPricing.RegistrationFeeTypeId = pricing.RegistrationFeeTypeId;
                                sessionRegistrationGroupPricing.Price = pricing.Price;

                                await _unitOfWork.SessionRegistrationGroupPricings.AddAsync(sessionRegistrationGroupPricing);
                            }

                        }

                    }

                    if (model.EventId != null)
                    {
                        var eventDetails = await _unitOfWork.Events.GetByIdAsync(Convert.ToInt32(model.EventId));
                        if (eventDetails != null)
                        {
                            eventDetails.Status = 1;
                            _unitOfWork.Events.Update(eventDetails);
                            await _unitOfWork.CommitAsync();
                        }
                    }

                    await _unitOfWork.CommitAsync();
                    response = true;
                }
                catch (Exception ex)
                {
                    _logger.LogError("An error occurred {Error} {StackTrace} {InnerException} {Source}",
                                   ex.Message, ex.StackTrace, ex.InnerException, ex.Source);
                }
            }
            return response;
        }

        public async Task<IEnumerable<SessionModel>> GetAllSessionsByEventId(int sessionId)
        {
            var sessionList = await _unitOfWork.Sessions.GetAllSessionsByEventIdAsync(sessionId);
            List<SessionModel> sessionModelList = _mapper.Map<List<SessionModel>>(sessionList);
            return sessionModelList;

        }

        public async Task<SessionModel> GetSessionById(int sessionId)
        {
            var tenantId = _httpContextAccessor.HttpContext.Request.Headers["X-Tenant-Id"].FirstOrDefault();
            string documentRoot = _appEnvironment.ContentRootPath;

            SessionModel sessionModel = new SessionModel();
            try
            {
                var session = await _unitOfWork.Sessions.GetSessionByIdAsync(sessionId);
                var eventDetails = await _unitOfWork.Events.GetEventByIdAsync(session.EventId ?? 0);
                EventModel eventModel = _mapper.Map<EventModel>(eventDetails);

                sessionModel = _mapper.Map<SessionModel>(session);
                sessionModel.Event = eventModel;

                var sessionQuestions = await _questionLinkService.GetQuestionBankBySessionId(sessionId);

                if (sessionQuestions.Count() > 0)
                {
                    sessionModel.SessionQuestions = sessionQuestions.ToList();
                }

                var sessionLeaders = await _unitOfWork.SessionLeaderLinks.GetSessionLeadersBySessionIdAsync(sessionId);

                foreach (var leader in sessionLeaders)
                {
                    SessionLeaderLinkModel sessionLeaderLinkModel = new SessionLeaderLinkModel();
                    sessionLeaderLinkModel.EntityId = leader.EntityId;
                    sessionLeaderLinkModel.SessionLeaderLinkId = leader.SessionLeaderLinkId;
                    sessionLeaderLinkModel.SessionId = leader.SessionId;
                    sessionLeaderLinkModel.EntityName = leader.Entity.Name;
                    var imageDocument = await _documentService.GetProfileImageById(documentRoot, tenantId, leader.EntityId ?? 0);
                    sessionLeaderLinkModel.base64ProfileImageData = Convert.ToBase64String(imageDocument.Document);
                    sessionLeaderLinkModel.imageArray = imageDocument.Document;
                    sessionModel.SessionLeaders.Add(sessionLeaderLinkModel);
                }


                var linkEventGroup = await _unitOfWork.LinkEventGroups.GetLinkEventGroupByEventIdAsync(session.EventId ?? 0);
                List<LinkEventGroupModel> linkEventGroupModelList = new List<LinkEventGroupModel>();


                var registrationFees = await _linkEventFeeTypeService.GetLinkedFeesByEventId(session.EventId ?? 0);

                foreach (var group in linkEventGroup)
                {
                    LinkEventGroupModel linkEventGroupModel = new LinkEventGroupModel();
                    linkEventGroupModel.LinkEventGroupId = group.LinkEventGroupId;
                    linkEventGroupModel.EventId = session.EventId ?? 0;
                    linkEventGroupModel.RegistrationGroupId = group.RegistrationGroupId;
                    linkEventGroupModel.EnableOnlineRegistration = group.EnableOnlineRegistration;
                    linkEventGroupModel.GroupName = group.RegistrationGroup.Name;

                    foreach (var fee in registrationFees)
                    {
                        LinkEventFeeTypeModel linkEventFeeTypeModel = new LinkEventFeeTypeModel();
                        linkEventFeeTypeModel.RegistrationFeeTypeId = fee.RegistrationFeeTypeId;
                        linkEventFeeTypeModel.RegistrationFeeTypeName = fee.RegistrationFeeTypeName;
                        sessionModel.LinkedFeeTypes.Add(linkEventFeeTypeModel);

                        var sessionPricing = await _unitOfWork.SessionRegistrationGroupPricings.GetPricingBySessionIdGroupIdFeeIdAsync(sessionId, group.RegistrationGroupId ?? 0, fee.RegistrationFeeTypeId);
                        SessionRegistrationGroupPricingModel sessionRegistrationGroupPricingModel = new SessionRegistrationGroupPricingModel();
                        sessionRegistrationGroupPricingModel.SessionId = sessionId;
                        sessionRegistrationGroupPricingModel.RegistrationGroupId = group.RegistrationGroupId;
                        sessionRegistrationGroupPricingModel.RegistrationFeeTypeId = fee.RegistrationFeeTypeId;
                        sessionRegistrationGroupPricingModel.Price = sessionPricing != null ? sessionPricing.Price : 0;

                        linkEventGroupModel.GroupPriorityFeeSettings.Add(sessionRegistrationGroupPricingModel);
                    }

                    sessionModel.GroupPricing.Add(linkEventGroupModel);
                }

                sessionModel.LinkedFeeTypes = registrationFees;
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred {Error} {StackTrace} {InnerException} {Source}",
                               ex.Message, ex.StackTrace, ex.InnerException, ex.Source);
            }
            return sessionModel;

        }
        public async Task<bool> DeleteSession(int sessionId)
        {
            var response = false;
            try
            {

                var sessionPricingList = await _unitOfWork.SessionRegistrationGroupPricings.GetSessionPricingBySessionIdAsync(sessionId);
                if (sessionPricingList.Count() > 0)
                {
                    _unitOfWork.SessionRegistrationGroupPricings.RemoveRange(sessionPricingList);
                }

                var sessionLeaders = await _unitOfWork.SessionLeaderLinks.GetSessionLeadersBySessionIdAsync(sessionId);
                if (sessionLeaders.Count() > 0)
                {
                    _unitOfWork.SessionLeaderLinks.RemoveRange(sessionLeaders);
                }

                var sessionQuestions = await _unitOfWork.QuestionLinks.GetQuestionsBySessionIdAsync(sessionId);
                if (sessionQuestions.Count() > 0)
                {
                    _unitOfWork.QuestionLinks.RemoveRange(sessionQuestions);
                }

                var session = await _unitOfWork.Sessions.GetByIdAsync(sessionId);
                if (session != null)
                {
                    _unitOfWork.Sessions.Remove(session);
                    await _unitOfWork.CommitAsync();
                    response = true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred {Error} {StackTrace} {InnerException} {Source}",
                               ex.Message, ex.StackTrace, ex.InnerException, ex.Source);
            }
            return response;
        }

        public async Task<SessionModel> GetNewSessionModel(int eventId)
        {
            SessionModel sessionModel = new SessionModel();
            try
            {
                var eventDetails = await _unitOfWork.Events.GetEventByIdAsync(eventId);
                EventModel eventModel = _mapper.Map<EventModel>(eventDetails);


                sessionModel.Event = eventModel;

                var linkEventGroup = await _unitOfWork.LinkEventGroups.GetLinkEventGroupByEventIdAsync(eventId);
                List<LinkEventGroupModel> linkEventGroupModelList = new List<LinkEventGroupModel>();


                var registrationFees = await _linkEventFeeTypeService.GetLinkedFeesByEventId(eventId);

                foreach (var group in linkEventGroup)
                {
                    LinkEventGroupModel linkEventGroupModel = new LinkEventGroupModel();
                    linkEventGroupModel.LinkEventGroupId = group.LinkEventGroupId;
                    linkEventGroupModel.EventId = eventId;
                    linkEventGroupModel.RegistrationGroupId = group.RegistrationGroupId;
                    linkEventGroupModel.EnableOnlineRegistration = group.EnableOnlineRegistration;
                    linkEventGroupModel.GroupName = group.RegistrationGroup.Name;

                    foreach (var fee in registrationFees)
                    {
                        LinkEventFeeTypeModel linkEventFeeTypeModel = new LinkEventFeeTypeModel();
                        linkEventFeeTypeModel.RegistrationFeeTypeId = fee.RegistrationFeeTypeId;
                        linkEventFeeTypeModel.RegistrationFeeTypeName = fee.RegistrationFeeTypeName;
                        sessionModel.LinkedFeeTypes.Add(linkEventFeeTypeModel);

                        SessionRegistrationGroupPricingModel sessionRegistrationGroupPricingModel = new SessionRegistrationGroupPricingModel();
                        sessionRegistrationGroupPricingModel.SessionId = 0;
                        sessionRegistrationGroupPricingModel.RegistrationGroupId = group.RegistrationGroupId;
                        sessionRegistrationGroupPricingModel.RegistrationFeeTypeId = fee.RegistrationFeeTypeId;
                        sessionRegistrationGroupPricingModel.Price = 0;

                        linkEventGroupModel.GroupPriorityFeeSettings.Add(sessionRegistrationGroupPricingModel);
                    }

                    sessionModel.GroupPricing.Add(linkEventGroupModel);
                }

                sessionModel.LinkedFeeTypes = registrationFees;

            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred {Error} {StackTrace} {InnerException} {Source}",
                               ex.Message, ex.StackTrace, ex.InnerException, ex.Source);
            }
            return sessionModel;

        }

        public async Task<bool> CloneSession(int sessionId, int eventId = 0)
        {
            var response = false;

            try
            {
                var masterSession = await _unitOfWork.Sessions.GetSessionByIdAsync(sessionId);
                int masterEventId = eventId > 0 ? eventId : masterSession.EventId ?? 0;
                Session cloneSession = new Session();
                cloneSession.Code = await GenerateSessionCode();
                cloneSession.Name = await GenerateCloneSessionName(masterSession.Name);
                cloneSession.EventId = masterEventId;
                cloneSession.StartDatetime = masterSession.StartDatetime;
                cloneSession.EndDateTime = masterSession.EndDateTime;
                cloneSession.MaxCapacity = eventId == 0 ? 0 : masterSession.MaxCapacity;
                cloneSession.GlAccountId = masterSession.GlAccountId;
                cloneSession.EnableCeu = masterSession.EnableCeu;
                cloneSession.EnableTax = masterSession.EnableTax;
                cloneSession.Status = masterSession.Status;
                cloneSession.Location = masterSession.Location;

                await _unitOfWork.Sessions.AddAsync(cloneSession);
                await _unitOfWork.CommitAsync();

                var sessionQuestions = await _questionLinkService.GetQuestionBankBySessionId(masterSession.SessionId);
                if (sessionQuestions.Count() > 0)
                {
                    foreach (var question in sessionQuestions)
                    {
                        Questionlink questionlink = new Questionlink();
                        questionlink.SessionId = cloneSession.SessionId;
                        questionlink.QuestionBankId = question.QuestionBankId;
                        await _unitOfWork.QuestionLinks.AddAsync(questionlink);
                    }
                }

                var sessionLeaders = await _unitOfWork.SessionLeaderLinks.GetSessionLeadersBySessionIdAsync(sessionId);
                if (sessionLeaders.Count() > 0)
                {
                    foreach (var leader in sessionLeaders)
                    {
                        Sessionleaderlink sessionLeader = new Sessionleaderlink();
                        sessionLeader.SessionId = cloneSession.SessionId;
                        sessionLeader.EntityId = leader.EntityId;
                        await _unitOfWork.SessionLeaderLinks.AddAsync(sessionLeader);
                    }
                }

                var linkEventGroup = await _unitOfWork.LinkEventGroups.GetLinkEventGroupByEventIdAsync(masterEventId);
                var registrationFees = await _linkEventFeeTypeService.GetLinkedFeesByEventId(masterEventId);
                List<LinkEventGroupModel> linkEventGroupModelList = new List<LinkEventGroupModel>();

                foreach (var group in linkEventGroup)
                {
                    LinkEventGroupModel linkEventGroupModel = new LinkEventGroupModel();
                    linkEventGroupModel.LinkEventGroupId = group.LinkEventGroupId;
                    linkEventGroupModel.EventId = masterEventId;
                    linkEventGroupModel.RegistrationGroupId = group.RegistrationGroupId;
                    linkEventGroupModel.EnableOnlineRegistration = group.EnableOnlineRegistration;
                    linkEventGroupModel.GroupName = group.RegistrationGroup.Name;

                    foreach (var fee in registrationFees)
                    {
                        var sessionPricing = await _unitOfWork.SessionRegistrationGroupPricings.GetPricingBySessionIdGroupIdFeeIdAsync(masterSession.SessionId, group.RegistrationGroupId ?? 0, fee.RegistrationFeeTypeId);
                        Sessionregistrationgrouppricing sessionRegistrationGroupPricing = new Sessionregistrationgrouppricing();
                        sessionRegistrationGroupPricing.SessionId = cloneSession.SessionId;
                        sessionRegistrationGroupPricing.RegistrationGroupId = group.RegistrationGroupId;
                        sessionRegistrationGroupPricing.RegistrationFeeTypeId = fee.RegistrationFeeTypeId;
                        sessionRegistrationGroupPricing.Price = sessionPricing != null ? sessionPricing.Price : 0;

                        await _unitOfWork.SessionRegistrationGroupPricings.AddAsync(sessionRegistrationGroupPricing);
                    }
                }
                await _unitOfWork.CommitAsync();
                response = true;

            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred {Error} {StackTrace} {InnerException} {Source}",
                               ex.Message, ex.StackTrace, ex.InnerException, ex.Source);
            }

            return response;
        }


        public async Task<string> GenerateSessionCode()
        {
            Utility utility = new Utility();
            string code = utility.GenerateRandomCode();

            //Check for duplicate code
            if (await _unitOfWork.Sessions.GetSessionBySessionCodeAsync(code) == null)
            {
                return code;
            }
            else
            {
                code = string.Empty;
            }

            return code;
        }

        public async Task<bool> ValidateSessionDetails(SessionModel sessionModel)
        {
            var sessionByName = await _unitOfWork.Sessions.GetSessionByNameAndEventIdAsync(sessionModel.Name, sessionModel.EventId ?? 0);

            if (sessionByName != null)
            {
                //check if code already exists
                if (sessionByName.SessionId != sessionModel.SessionId)
                {
                    if (sessionByName.Name == sessionModel.Name)
                    {
                        throw new InvalidOperationException($"Duplicate session name.");
                    }
                }
            }

            var sessionByCode = await _unitOfWork.Sessions.GetSessionBySessionCodeAsync(sessionModel.Code);

            if (sessionByCode != null)
            {
                //check if code already exists
                if (sessionByCode.SessionId != sessionModel.SessionId)
                {
                    if (sessionByCode.Code == sessionModel.Code)
                    {
                        throw new InvalidOperationException($"Duplicate session code.");
                    }
                }
            }

            //var sessionByDate = await _unitOfWork.Sessions.GetSessionByStartAndEndDateAsync(sessionModel.StartDatetime ?? DateTime.Now, sessionModel.EndDateTime ?? DateTime.Now, sessionModel.EventId ?? 0);

            //if (sessionByDate != null)
            //{
            //    //check if code already exists
            //    if (sessionByCode.SessionId != sessionModel.SessionId)
            //    {
            //        if (sessionByCode.Code == sessionModel.Code)
            //        {
            //            throw new InvalidOperationException($"Session with same start date & end date already exists.");
            //        }
            //    }
            //}


            return true;
        }

        public async Task<string> GenerateCloneSessionName(string name)
        {
            int i = 1;
            string cloneName = string.Empty;
            Session session = new Session();

            do
            {
                cloneName = name + " Copy-" + i.ToString();
                session = await _unitOfWork.Sessions.GetSessionByNameAsync(cloneName);
                i++;
            }
            while (session != null);
            return cloneName;
        }

        public async Task<IEnumerable<SessionModel>> GetSessionLeadersBySessionId(string documentRoot, string tenantId, int eventId)
        {
            List<SessionModel> sessionModelList = new List<SessionModel>();
            try
            {
                var eventDetails = await _unitOfWork.Events.GetEventByIdAsync(eventId);

                if (eventDetails.Sessions.Count() > 0)
                {
                    foreach (var session in eventDetails.Sessions)
                    {
                        SessionModel sessionModel = new SessionModel();
                        var sessionDetails = await _unitOfWork.Sessions.GetSessionByIdAsync(session.SessionId);
                        sessionModel = _mapper.Map<SessionModel>(sessionDetails);
                        sessionModel.SessionLeaders.Clear();

                        var sessionLeaders = await _unitOfWork.SessionLeaderLinks.GetSessionLeadersBySessionIdAsync(session.SessionId);

                        foreach (var leader in sessionLeaders)
                        {
                            SessionLeaderLinkModel sessionLeaderLinkModel = new SessionLeaderLinkModel();
                            sessionLeaderLinkModel.EntityId = leader.EntityId;
                            sessionLeaderLinkModel.SessionLeaderLinkId = leader.SessionLeaderLinkId;
                            sessionLeaderLinkModel.SessionId = leader.SessionId;
                            sessionLeaderLinkModel.EntityName = leader.Entity.Name;
                            var imageDocument = await _documentService.GetProfileImageById(documentRoot, tenantId, leader.EntityId ?? 0);
                            sessionLeaderLinkModel.base64ProfileImageData = Convert.ToBase64String(imageDocument.Document);
                            sessionLeaderLinkModel.imageArray = imageDocument.Document;
                            sessionModel.SessionLeaders.Add(sessionLeaderLinkModel);
                        }
                        sessionModelList.Add(sessionModel);
                    }
                }

            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred {Error} {StackTrace} {InnerException} {Source}",
                               ex.Message, ex.StackTrace, ex.InnerException, ex.Source);
            }
            return sessionModelList;

        }

        public async Task<IEnumerable<EventRegistrationSessionGroupAndPricingModel>> GetEventRegistrationSessionGroupAndPricing(int eventId, int entityId)
        {
            List<EventRegistrationSessionGroupAndPricingModel> eventRegistrationGroupAndPricingList = new List<EventRegistrationSessionGroupAndPricingModel>();
            var getMembersActiveMemberShips = await _unitOfWork.Memberships.GetActiveMembershipByEntityIdAsync(entityId);
            var sessions = await GetAllSessionsByEventId(eventId);
            foreach (var session in sessions)
            {
                var sessionRegistrationGroupPricing = await _unitOfWork.SessionRegistrationGroupPricings.GetSessionPricingBySessionIdAsync(session.SessionId);
                foreach (var item in sessionRegistrationGroupPricing)
                {
                    if (item.RegistrationGroup != null)
                    {
                        if (item.RegistrationGroup.Name.ToLower() == "member" || item.RegistrationGroup.Name.ToLower() == "non-member")
                        {
                            EventRegistrationSessionGroupAndPricingModel eventRegistrationSessionGroupAndPricing = new EventRegistrationSessionGroupAndPricingModel();
                            eventRegistrationSessionGroupAndPricing.SessionRegistrationGroupPricingId = item.SessionRegistrationGroupPricingId;
                            eventRegistrationSessionGroupAndPricing.SessionId = session.SessionId;
                            eventRegistrationSessionGroupAndPricing.GroupPricing = item.RegistrationGroup.Name + " - $" + item.Price;
                            eventRegistrationSessionGroupAndPricing.GroupName = item.RegistrationGroup.Name;
                            eventRegistrationSessionGroupAndPricing.Pricing = item.Price;
                            if((getMembersActiveMemberShips.Any() && item.RegistrationGroup.Name.ToLower() == "member") || (!getMembersActiveMemberShips.Any() && item.RegistrationGroup.Name.ToLower() == "non-member"))
                            {
                                eventRegistrationSessionGroupAndPricing.SelectedItem = eventRegistrationSessionGroupAndPricing.SessionRegistrationGroupPricingId;
                            }
                            eventRegistrationGroupAndPricingList.Add(eventRegistrationSessionGroupAndPricing);
                        }
                    }
                }
            }
            return eventRegistrationGroupAndPricingList;
        }

        public async Task<IEnumerable<string>> GetRegisteredSessionsByEntity(int eventId, int entityId, string sessionIds)
        {
            List<string> sessionNames = new List<string>();
            if (!string.IsNullOrEmpty(sessionIds))
            {
                foreach (var session in sessionIds.Split(",").ToList())
                {
                    var registeredSessions = await _unitOfWork.EventRegisterSession.GetRegisteredSessionsByEntity(eventId, entityId, Convert.ToInt32(session));
                    if (registeredSessions != null)
                    {
                        sessionNames.Add(registeredSessions.Session.Name);
                    }
                }
            }
            return sessionNames;
        }
        public async Task<IEnumerable<Eventregistersession>> GetRegisteredSessions(int sessionId)
        {
            var registerSessions = await _unitOfWork.EventRegisterSession.GetRegisteredSessionsBySessionIdAsync(sessionId);
            return registerSessions;
        }
    }
}
