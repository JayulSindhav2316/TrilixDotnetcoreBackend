using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Max.Core;
using Max.Core.Models;
using Max.Data.DataModel;
using Max.Data.Interfaces;
using Max.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Max.Core.Helpers;
using static Max.Core.Constants;

namespace Max.Services
{
    public class EventService : IEventService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<EventService> _logger;
        private readonly ISessionService _sessionService;
        private readonly ITenantService _tenantService;
        private readonly IQuestionLinkService _questionLinkService;
        private readonly ILinkEventFeeTypeService _linkEventFeeTypeService;
        private readonly IDocumentService _documentService;
        public EventService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<EventService> logger, ITenantService tenantService,
            IQuestionLinkService questionLinkService, ILinkEventFeeTypeService linkEventFeeTypeService,
            ISessionService sessionService, IDocumentService documentService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _tenantService = tenantService;
            _sessionService = sessionService;
            _questionLinkService = questionLinkService;
            _linkEventFeeTypeService = linkEventFeeTypeService;
            _documentService = documentService;
        }

        public async Task<EventModel> CreateEvent(EventModel model)
        {
            EventModel eventModel = new EventModel();

            bool isValid = await ValidateEventDetails(model);
            if (isValid)
            {
                try
                {
                    Event eventData = new Event();
                    eventData.Name = model.Name;
                    while (string.IsNullOrEmpty(eventData.Code))
                    {
                        eventData.Code = model.Code != string.Empty ? model.Code : await GenerateEventCode();
                    }
                    eventData.EventTypeId = model.EventTypeId;
                    eventData.FromDate = model.FromDate;
                    eventData.ToDate = model.ToDate;
                    eventData.RegStartDate = model.RegStartDate;
                    eventData.RegEndDate = model.RegEndDate;
                    eventData.TimeZoneId = model.TimeZoneId;
                    eventData.Location = model.EventTypeId == 3 ? null : model.Location;
                    eventData.Area = model.EventTypeId == 2 || model.EventTypeId == 3 ? null : model.Area;
                    eventData.City = model.EventTypeId == 2 || model.EventTypeId == 3 ? null : model.City;
                    eventData.State = model.EventTypeId == 2 || model.EventTypeId == 3 ? null : model.State;
                    eventData.Country = model.EventTypeId == 2 || model.EventTypeId == 3 ? null : model.Country;
                    eventData.Zip = model.EventTypeId == 2 || model.EventTypeId == 3 ? null : model.Zip;
                    eventData.WebinarLiveLink = model.EventTypeId == 1 || model.EventTypeId == 3 ? null : model.WebinarLiveLink;
                    eventData.WebinarRecordedLink = model.EventTypeId == 1 || model.EventTypeId == 2 ? null : model.WebinarRecordedLink;
                    eventData.Summary = model.Summary;
                    eventData.Description = model.Description;
                    eventData.Status = model.Status;
                    eventData.MaxCapacity = model.EventTypeId == 3 ? null : model.MaxCapacity;
                    eventData.OrganizationId = model.OrganizationId;
                    eventData.DueDate = model.DueDate;

                    await _unitOfWork.Events.AddAsync(eventData);
                    await _unitOfWork.CommitAsync();

                    if (model.EventContacts.Count() > 0)
                    {
                        foreach (var contact in model.EventContacts)
                        {
                            Eventcontact eventcontact = new Eventcontact();
                            eventcontact.EventId = eventData.EventId;
                            eventcontact.StaffId = contact.StaffId;
                            eventcontact.Name = contact.Name;
                            eventcontact.Email = contact.Email;
                            eventcontact.PhoneNumber = contact.PhoneNumber;
                            await _unitOfWork.EventContacts.AddAsync(eventcontact);
                        }
                        await _unitOfWork.CommitAsync();
                    }

                    eventModel = _mapper.Map<EventModel>(eventData);
                }
                catch (Exception ex)
                {
                    _logger.LogError("An error occurred {Error} {StackTrace} {InnerException} {Source}",
                                   ex.Message, ex.StackTrace, ex.InnerException, ex.Source);
                }

            }
            return eventModel;
        }

        public async Task<bool> UpdateEvent(EventModel model)
        {
            var response = false;
            bool isValid = await ValidateEventDetails(model);
            if (isValid)
            {
                try
                {
                    Event eventData = await _unitOfWork.Events.GetByIdAsync(model.EventId);
                    eventData.Name = model.Name;
                    eventData.Code = model.Code != eventData.Code ? model.Code : eventData.Code;
                    eventData.EventTypeId = model.EventTypeId;
                    eventData.FromDate = model.FromDate;
                    eventData.ToDate = model.ToDate;
                    eventData.RegStartDate = model.RegStartDate;
                    eventData.RegEndDate = model.RegEndDate;
                    eventData.TimeZoneId = model.TimeZoneId;
                    eventData.Location = model.EventTypeId == 3 ? null : model.Location;
                    eventData.Area = model.EventTypeId == 2 || model.EventTypeId == 3 ? null : model.Area;
                    eventData.City = model.EventTypeId == 2 || model.EventTypeId == 3 ? null : model.City;
                    eventData.State = model.EventTypeId == 2 || model.EventTypeId == 3 ? null : model.State;
                    eventData.Country = model.EventTypeId == 2 || model.EventTypeId == 3 ? null : model.Country;
                    eventData.Zip = model.EventTypeId == 2 || model.EventTypeId == 3 ? null : model.Zip;
                    eventData.WebinarLiveLink = model.EventTypeId == 1 || model.EventTypeId == 3 ? null : model.WebinarLiveLink;
                    eventData.WebinarRecordedLink = model.EventTypeId == 1 || model.EventTypeId == 2 ? null : model.WebinarRecordedLink;
                    eventData.Summary = model.Summary;
                    eventData.Description = model.Description;
                    eventData.Status = model.Status;
                    eventData.MaxCapacity = model.EventTypeId == 3 ? null : model.MaxCapacity;
                    eventData.DueDate = model.DueDate;

                    var eventContactsList = await _unitOfWork.EventContacts.GetEventContactsByEventIdAsync(model.EventId);

                    if (eventContactsList.Count() > 0)
                    {
                        _unitOfWork.EventContacts.RemoveRange(eventContactsList);
                        await _unitOfWork.CommitAsync();
                    }

                    if (model.EventContacts.Count() > 0)
                    {
                        foreach (var contact in model.EventContacts)
                        {
                            Eventcontact eventcontact = new Eventcontact();
                            eventcontact.EventId = model.EventId;
                            eventcontact.StaffId = contact.StaffId;
                            eventcontact.Name = contact.Name;
                            eventcontact.Email = contact.Email;
                            eventcontact.PhoneNumber = contact.PhoneNumber;
                            await _unitOfWork.EventContacts.AddAsync(eventcontact);
                        }
                        await _unitOfWork.CommitAsync();
                    }

                    _unitOfWork.Events.Update(eventData);
                    await _unitOfWork.CommitAsync();

                    // Update invoice due date if event due date is changed
                    if (model.IsDueDateDialogSaveForAll)
                    {
                        var eventInvoices = await _unitOfWork.Invoices.GetAllInvoicesByEventIdAsync(model.EventId);
                        if (eventInvoices.Any())
                        {
                            foreach (var invoice in eventInvoices)
                            {
                                if (model.DueDate != null)
                                {
                                    var invoiceDetails = await _unitOfWork.Invoices.GetByIdAsync(invoice.InvoiceId);
                                    if (invoiceDetails != null)
                                    {
                                        invoiceDetails.DueDate = Convert.ToDateTime(model.DueDate);
                                        _unitOfWork.Invoices.Update(invoiceDetails);
                                        await _unitOfWork.CommitAsync();
                                    }
                                }
                            }
                        }
                    }

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

        public async Task<bool> UpdateEventSettings(EventModel model)
        {
            var response = false;
            try
            {
                Event eventData = await _unitOfWork.Events.GetByIdAsync(model.EventId);

                eventData.AllowMultipleRegistration = model.AllowMultipleRegistration;
                eventData.AllowWaitlist = model.AllowWaitlist;
                eventData.AllowNonMember = model.LinkedGroups.Any(x => x.GroupName == "Non-Member") ? 1 : 0;

                _unitOfWork.Events.Update(eventData);
                await _unitOfWork.CommitAsync();


                var linkfeetypes = await _unitOfWork.LinkEventFeeTypes.GetFeeTypesByEventIdAsync(model.EventId);
                if (linkfeetypes.Count() > 0)
                {
                    _unitOfWork.LinkEventFeeTypes.RemoveRange(linkfeetypes);
                    await _unitOfWork.CommitAsync();
                }

                if (model.LinkedFeeTypes.Count() > 0)
                {
                    foreach (var fee in model.LinkedFeeTypes)
                    {
                        Linkeventfeetype feeType = new Linkeventfeetype();
                        feeType.EventId = model.EventId;
                        feeType.RegistrationFeeTypeId = fee.RegistrationFeeTypeId;
                        await _unitOfWork.LinkEventFeeTypes.AddAsync(feeType);
                    }
                    await _unitOfWork.CommitAsync();
                }

                if (model.LinkedGroups.Count() > 0)
                {
                    var linkedGroups = await _unitOfWork.LinkEventGroups.GetLinkEventGroupByEventIdAsync(model.EventId);
                    if (linkedGroups.Count() > 0)
                    {
                        foreach (var group in linkedGroups)
                        {
                            var linkedFees = await _unitOfWork.LinkRegistrationGroupFees.GetLinkRegistrationGroupFeesByLinkEventGroupIdAsync(group.LinkEventGroupId);
                            if (linkedFees.Count() > 0)
                            {
                                _unitOfWork.LinkRegistrationGroupFees.RemoveRange(linkedFees);
                            }
                        }
                        _unitOfWork.LinkEventGroups.RemoveRange(linkedGroups);
                        await _unitOfWork.CommitAsync();
                    }

                    foreach (var group in model.LinkedGroups)
                    {
                        Linkeventgroup linkeventgroup = new Linkeventgroup();
                        linkeventgroup.RegistrationGroupId = group.RegistrationGroupId;
                        linkeventgroup.EventId = model.EventId;
                        linkeventgroup.EnableOnlineRegistration = group.EnableOnlineRegistration;

                        //if (group.EnableOnlineRegistration == 1)
                        //{
                        foreach (var feeType in group.GroupPriorityDateSettings)
                        {
                            Linkregistrationgroupfee linkregistrationgroupfee = new Linkregistrationgroupfee();
                            linkregistrationgroupfee.RegistrationFeeTypeId = feeType.RegistrationFeeTypeId;
                            linkregistrationgroupfee.RegistrationGroupId = feeType.RegistrationGroupId;
                            linkregistrationgroupfee.RegistrationGroupDateTime = feeType.RegistrationGroupDateTime;
                            linkregistrationgroupfee.RegistrationGroupEndDateTime = feeType.RegistrationGroupEndDateTime;
                            linkeventgroup.Linkregistrationgroupfees.Add(linkregistrationgroupfee);
                        }
                        //}

                        await _unitOfWork.LinkEventGroups.AddAsync(linkeventgroup);
                    }
                    await _unitOfWork.CommitAsync();
                }

                response = true;
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred {Error} {StackTrace} {InnerException} {Source}",
                               ex.Message, ex.StackTrace, ex.InnerException, ex.Source);
            }


            return response;
        }

        public async Task<IEnumerable<EventModel>> GetAllEvents()
        {
            var eventsList = await _unitOfWork.Events.GetAllEventsAsync();
            List<EventModel> eventModelList = _mapper.Map<List<EventModel>>(eventsList);
            return eventModelList;

        }

        public async Task<IEnumerable<EventListModel>> GetEventsByFilter(DateTime date, int filter = 1)
        {
            List<EventListModel> eventModelList = new List<EventListModel>();
            if (filter == 1)
            {
                var eventsList = await _unitOfWork.Events.GetUpcomingEventsAsync(date);
                eventModelList = _mapper.Map<List<EventListModel>>(eventsList);
            }
            if (filter == 2)
            {
                var eventsList = await _unitOfWork.Events.GetPastEventsAsync(date);
                eventModelList = _mapper.Map<List<EventListModel>>(eventsList);
            }
            if (filter == 3)
            {
                var eventsList = await _unitOfWork.Events.GetEventsByStatusAsync(0);
                eventModelList = _mapper.Map<List<EventListModel>>(eventsList);
            }
            return eventModelList;

        }

        public async Task<EventModel> GetEventDetailsById(int eventId)
        {
            var eventDetails = await _unitOfWork.Events.GetEventByIdAsync(eventId);
            EventModel eventDetailsModel = _mapper.Map<EventModel>(eventDetails);
            eventDetailsModel.EventType = eventDetails.EventType.EventType1;

            if (eventDetails.TimeZone != null)
            {
                var offSet = eventDetails.TimeZone.TimeZoneOffset > 0 ? "+" + eventDetails.TimeZone.TimeZoneOffset.ToString() : eventDetails.TimeZone.TimeZoneOffset.ToString();
                eventDetailsModel.TimeZone = eventDetails.TimeZone.TimeZoneAbbreviation + " " + offSet;
            }

            var eventQuestions = await _questionLinkService.GetQuestionBankByEventId(eventId);
            List<QuestionBankModel> questionBankModelList = new List<QuestionBankModel>();
            foreach (var question in eventQuestions)
            {
                QuestionBankModel questionBankModel = _mapper.Map<QuestionBankModel>(question);
                questionBankModelList.Add(questionBankModel);
            }
            eventDetailsModel.EventQuestions = questionBankModelList;

            var sessionList = await _unitOfWork.Sessions.GetAllSessionsByEventIdAsync(eventId);
            List<SessionModel> sessionModelList = new List<SessionModel>();
            foreach (var session in sessionList)
            {
                SessionModel sessionModel = await _sessionService.GetSessionById(session.SessionId);
                var registeredSessions = await _unitOfWork.EventRegisterSession.GetRegisteredSessionsBySessionIdAsync(session.SessionId);
                if (registeredSessions.Any())
                {
                    sessionModel.RegisteredSessions = registeredSessions.Count();
                }
                sessionModelList.Add(sessionModel);
            }
            eventDetailsModel.Sessions = sessionModelList.OrderBy(x => x.StartDatetime).ToList(); ;

            return eventDetailsModel;

        }

        public async Task<EventModel> GetEventBasicDetailsById(int eventId)
        {
            var eventDetails = await _unitOfWork.Events.GetEventByIdAsync(eventId);
            EventModel eventDetailsModel = _mapper.Map<EventModel>(eventDetails);
            eventDetailsModel.EventType = eventDetails.EventType.EventType1;

            if (eventDetails.TimeZone != null)
            {
                var offSet = eventDetails.TimeZone.TimeZoneOffset > 0 ? "+" + eventDetails.TimeZone.TimeZoneOffset.ToString() : eventDetails.TimeZone.TimeZoneOffset.ToString();
                eventDetailsModel.TimeZone = eventDetails.TimeZone.TimeZoneAbbreviation + " " + offSet;
            }
            return eventDetailsModel;

        }

        public async Task<string> GenerateEventCode()
        {
            Utility utility = new Utility();
            string code = utility.GenerateRandomCode();

            //Check for duplicate code
            if (await _unitOfWork.Events.GetEventByEventCodeAsync(code) == null)
            {
                return code;
            }
            else
            {
                code = await GenerateEventCode();
            }

            return code;
        }

        public async Task<bool> DeleteEvent(int eventId)
        {
            var response = false;
            try
            {

                var sessionList = await _unitOfWork.Sessions.GetAllSessionsByEventIdAsync(eventId);
                if (sessionList.Count() > 0)
                {
                    foreach (var currentSession in sessionList)
                    {
                        await _sessionService.DeleteSession(currentSession.SessionId);
                    }
                }

                var eventQuestions = await _unitOfWork.QuestionLinks.GetQuestionsByEventIdAsync(eventId);
                if (eventQuestions.Count() > 0)
                {
                    _unitOfWork.QuestionLinks.RemoveRange(eventQuestions);
                }

                var linkedFeeTypes = await _unitOfWork.LinkEventFeeTypes.GetFeeTypesByEventIdAsync(eventId);
                if (linkedFeeTypes.Count() > 0)
                {
                    _unitOfWork.LinkEventFeeTypes.RemoveRange(linkedFeeTypes);
                }

                var linkedGroups = await _unitOfWork.LinkEventGroups.GetLinkEventGroupByEventIdAsync(eventId);
                if (linkedGroups.Count() > 0)
                {
                    foreach (var group in linkedGroups)
                    {
                        var linkRegistrationGroupFee = await _unitOfWork.LinkRegistrationGroupFees.GetLinkRegistrationGroupFeesByLinkEventGroupIdAsync(group.LinkEventGroupId);
                        if (linkRegistrationGroupFee.Count() > 0)
                        {
                            _unitOfWork.LinkRegistrationGroupFees.RemoveRange(linkRegistrationGroupFee);
                        }
                    }
                    _unitOfWork.LinkEventGroups.RemoveRange(linkedGroups);
                }

                var eventContactsList = await _unitOfWork.EventContacts.GetEventContactsByEventIdAsync(eventId);
                if (eventContactsList.Count() > 0)
                {
                    _unitOfWork.EventContacts.RemoveRange(eventContactsList);
                }

                var currentEvent = await _unitOfWork.Events.GetByIdAsync(eventId);
                if (currentEvent != null)
                {
                    _unitOfWork.Events.Remove(currentEvent);
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

        public async Task<bool> CloneEvent(int eventId)
        {
            var response = false;

            try
            {
                var masterEvent = await _unitOfWork.Events.GetByIdAsync(eventId);
                Event cloneEvent = new Event();
                cloneEvent.Name = await GenerateCloneEventName(masterEvent.Name);
                while (string.IsNullOrEmpty(cloneEvent.Code))
                {
                    cloneEvent.Code = await GenerateEventCode();
                }
                cloneEvent.EventTypeId = masterEvent.EventTypeId;
                cloneEvent.FromDate = masterEvent.FromDate;
                cloneEvent.RegStartDate = masterEvent.RegStartDate;
                cloneEvent.RegEndDate = masterEvent.RegEndDate;
                cloneEvent.ToDate = masterEvent.ToDate;
                cloneEvent.TimeZoneId = masterEvent.TimeZoneId;
                cloneEvent.Location = masterEvent.Location;
                cloneEvent.Area = masterEvent.Area;
                cloneEvent.City = masterEvent.City;
                cloneEvent.State = masterEvent.State;
                cloneEvent.Country = masterEvent.Country;
                cloneEvent.Zip = masterEvent.Zip;
                cloneEvent.WebinarLiveLink = masterEvent.WebinarLiveLink;
                cloneEvent.WebinarRecordedLink = masterEvent.WebinarRecordedLink;
                cloneEvent.Summary = masterEvent.Summary;
                cloneEvent.Description = masterEvent.Description;
                cloneEvent.Status = masterEvent.Status;
                cloneEvent.MaxCapacity = masterEvent.MaxCapacity;
                cloneEvent.AllowMultipleRegistration = masterEvent.AllowMultipleRegistration;
                cloneEvent.AllowWaitlist = masterEvent.AllowWaitlist;
                cloneEvent.AllowNonMember = masterEvent.AllowNonMember;

                await _unitOfWork.Events.AddAsync(cloneEvent);
                await _unitOfWork.CommitAsync();

                var linkedFees = await _linkEventFeeTypeService.GetLinkedFeesByEventId(masterEvent.EventId);
                if (linkedFees.Count() > 0)
                {
                    foreach (var fee in linkedFees)
                    {
                        Linkeventfeetype feeType = new Linkeventfeetype();
                        feeType.EventId = cloneEvent.EventId;
                        feeType.RegistrationFeeTypeId = fee.RegistrationFeeTypeId;
                        await _unitOfWork.LinkEventFeeTypes.AddAsync(feeType);
                    }
                }

                var linkEventGroup = await _unitOfWork.LinkEventGroups.GetLinkEventGroupByEventIdAsync(masterEvent.EventId);
                foreach (var group in linkEventGroup)
                {
                    Linkeventgroup linkeventgroup = new Linkeventgroup();
                    linkeventgroup.RegistrationGroupId = group.RegistrationGroupId;
                    linkeventgroup.EventId = cloneEvent.EventId;
                    linkeventgroup.EnableOnlineRegistration = group.EnableOnlineRegistration;

                    var linkedGroupRegistrationFees = await _unitOfWork.LinkRegistrationGroupFees.GetLinkRegistrationGroupFeesByLinkEventGroupIdAsync(group.LinkEventGroupId);

                    foreach (var feeType in linkedGroupRegistrationFees)
                    {
                        Linkregistrationgroupfee linkregistrationgroupfee = new Linkregistrationgroupfee();
                        linkregistrationgroupfee.RegistrationFeeTypeId = feeType.RegistrationFeeTypeId;
                        linkregistrationgroupfee.RegistrationGroupId = feeType.RegistrationGroupId;
                        linkregistrationgroupfee.RegistrationGroupDateTime = feeType.RegistrationGroupDateTime;
                        linkregistrationgroupfee.RegistrationGroupEndDateTime = feeType.RegistrationGroupEndDateTime;
                        linkeventgroup.Linkregistrationgroupfees.Add(linkregistrationgroupfee);
                    }
                    await _unitOfWork.LinkEventGroups.AddAsync(linkeventgroup);
                }

                var sessionList = await _unitOfWork.Sessions.GetAllSessionsByEventIdAsync(masterEvent.EventId);
                if (sessionList.Count() > 0)
                {
                    foreach (var currentSession in sessionList)
                    {
                        await _sessionService.CloneSession(currentSession.SessionId, cloneEvent.EventId);
                    }
                }

                var eventQuestions = await _questionLinkService.GetQuestionBankByEventId(masterEvent.EventId);
                if (eventQuestions.Count() > 0)
                {
                    foreach (var question in eventQuestions)
                    {
                        Questionlink questionlink = new Questionlink();
                        questionlink.EventId = cloneEvent.EventId;
                        questionlink.QuestionBankId = question.QuestionBankId;
                        await _unitOfWork.QuestionLinks.AddAsync(questionlink);
                    }
                }

                var eventContactsList = await _unitOfWork.EventContacts.GetEventContactsByEventIdAsync(masterEvent.EventId);

                if (eventContactsList.Count() > 0)
                {
                    foreach (var contact in eventContactsList)
                    {
                        Eventcontact eventcontact = new Eventcontact();
                        eventcontact.EventId = cloneEvent.EventId;
                        eventcontact.StaffId = contact.StaffId;
                        eventcontact.Name = contact.Name;
                        eventcontact.Email = contact.Email;
                        eventcontact.PhoneNumber = contact.PhoneNumber;
                        await _unitOfWork.EventContacts.AddAsync(eventcontact);
                    }
                }

                await _documentService.CloneEventImageById(cloneEvent.EventId, masterEvent.EventId);

                await _documentService.CloneEventCoverImageById(cloneEvent.EventId, masterEvent.EventId);

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

        public async Task<string> GenerateCloneEventName(string name)
        {
            int i = 1;
            string cloneName = string.Empty;
            Event eventInstance = new Event();

            do
            {
                cloneName = name + " Copy-" + i.ToString();
                eventInstance = await _unitOfWork.Events.GetEventByEventNameAsync(cloneName);
                i++;
            }
            while (eventInstance != null);
            return cloneName;
        }

        public async Task<bool> ValidateEventDetails(EventModel eventModel)
        {
            var currentEventByName = await _unitOfWork.Events.GetEventByEventNameAsync(eventModel.Name);

            if (currentEventByName != null)
            {
                //check if code already exists
                if (currentEventByName.EventId != eventModel.EventId)
                {
                    if (currentEventByName.Name == eventModel.Name)
                    {
                        throw new InvalidOperationException($"Duplicate event name.");
                    }
                }
            }

            if (!string.IsNullOrEmpty(eventModel.Code))
            {
                var currentEventByCode = await _unitOfWork.Events.GetEventByEventCodeAsync(eventModel.Code);

                if (currentEventByCode != null)
                {
                    //check if code already exists
                    if (currentEventByCode.EventId != eventModel.EventId)
                    {
                        if (currentEventByCode.Code == eventModel.Code)
                        {
                            throw new InvalidOperationException($"Duplicate event code.");
                        }
                    }
                }
            }
            return true;
        }

        public async Task<IEnumerable<EventModel>> GetAllActiveEvents(bool includePastEvents)
        {
            List<EventModel> eventModelList = new List<EventModel>();
            IEnumerable<Event> eventsList = new List<Event>();
            if (includePastEvents)
            {
                eventsList = await _unitOfWork.Events.GetEventsByStatusAsync(1);
            }
            else
            {
                eventsList = await _unitOfWork.Events.GetUpcomingEventsAsync(DateTime.Now);
            }
            if (eventsList.Any())
            {
                eventModelList = _mapper.Map<List<EventModel>>(eventsList);
                var inPersonEvents = eventModelList.Where(s => s.EventTypeId == 3).ToList();
                if (inPersonEvents.Any())
                {
                    eventModelList = eventModelList.Except(inPersonEvents).ToList();
                }
                eventModelList = eventModelList.OrderBy(e => e.FromDate).ToList();
                eventModelList.AddRange(inPersonEvents);
            }
            return eventModelList;
        }

        public async Task<int> CreateEventRegister(EventRegisterModel eventRegisterModel)
        {
            try
            {
                int invoiceId = 0;
                if (eventRegisterModel != null)
                {
                    Eventregister eventRegister = new Eventregister();
                    eventRegister.EventId = eventRegisterModel.EventId;
                    eventRegister.EntityId = eventRegisterModel.EntityId;
                    await _unitOfWork.EventRegister.AddAsync(eventRegister);
                    await _unitOfWork.CommitAsync();



                    foreach (var sesstion in eventRegisterModel.SelectedSession)
                    {
                        Eventregistersession eventRegisterSession = new Eventregistersession();
                        eventRegisterSession.EventRegisterId = eventRegister.EventRegisterId;
                        eventRegisterSession.SessionId = sesstion.SessionId;
                        eventRegisterSession.Price = sesstion.Price;
                        await _unitOfWork.EventRegisterSession.AddAsync(eventRegisterSession);
                        await _unitOfWork.CommitAsync();
                    }

                    foreach (var question in eventRegisterModel.SelectedQuestion)
                    {
                        if (question.SelectedAnwserOptions != null && question.SelectedAnwserOptions.Count() > 0)
                        {
                            foreach (var selectedAnswerOption in question.SelectedAnwserOptions)
                            {
                                Eventregisterquestion eventRegisterQuestion = new Eventregisterquestion();
                                eventRegisterQuestion.EventRegisterId = eventRegister.EventRegisterId;
                                if (question.SessionId != 0)
                                {
                                    eventRegisterQuestion.SessionId = question.SessionId;
                                }
                                else
                                {
                                    eventRegisterQuestion.SessionId = null;
                                }
                                if(question.EventId != 0)
                                {
                                    eventRegisterQuestion.EventId = question.EventId;
                                }
                                else
                                {
                                    eventRegisterQuestion.EventId = null;
                                }
                                eventRegisterQuestion.QuestionId = question.QuestionId;
                                eventRegisterQuestion.AnswerOptionId = selectedAnswerOption;
                                if (question.AnswerOption != null)
                                {
                                    eventRegisterQuestion.Answer = question.AnswerOption.FirstOrDefault(s => s.AnswerOptionId == selectedAnswerOption).Option;
                                }
                                await _unitOfWork.EventRegisterQuestion.AddAsync(eventRegisterQuestion);
                            }
                        }
                        else
                        {
                            Eventregisterquestion eventRegisterQuestion = new Eventregisterquestion();
                            eventRegisterQuestion.EventRegisterId = eventRegister.EventRegisterId;
                            if (question.SessionId != 0)
                            {
                                eventRegisterQuestion.SessionId = question.SessionId;
                            }
                            else
                            {
                                eventRegisterQuestion.SessionId = null;
                            }
                            if (question.EventId != 0)
                            {
                                eventRegisterQuestion.EventId = question.EventId;
                            }
                            else
                            {
                                eventRegisterQuestion.EventId = null;
                            }
                            eventRegisterQuestion.QuestionId = question.QuestionId;
                            if (question.AnswerOption != null)
                            {
                                if (question.AnswerValue != null)
                                {
                                    eventRegisterQuestion.AnswerOptionId = Convert.ToInt32(question.AnswerValue);
                                    eventRegisterQuestion.Answer = question.AnswerOption.FirstOrDefault(s => s.AnswerOptionId == Convert.ToInt32(question.AnswerValue)).Option;
                                }
                            }
                            else
                            {
                                eventRegisterQuestion.Answer = question.AnswerValue;
                                eventRegisterQuestion.AnswerOptionId = null;
                            }
                            await _unitOfWork.EventRegisterQuestion.AddAsync(eventRegisterQuestion);
                        }
                        await _unitOfWork.CommitAsync();
                    }

                    //Create invoice
                    var eventDetails = await _unitOfWork.Events.GetByIdAsync(eventRegisterModel.EventId);
                    if (eventDetails != null)
                    {
                        var invoice = new Invoice();
                        invoice.BillableEntityId = eventRegisterModel.EntityId;
                        invoice.BillingType = BillingTypes.GENERAL;
                        invoice.Date = DateTime.Now;
                        invoice.InvoiceType = InvoiceType.INDIVIDUAL;
                        invoice.EntityId = eventRegisterModel.EntityId;
                        invoice.UserId = eventRegisterModel.CurrentUserId;
                        invoice.Status = (int)InvoiceStatus.Finalized;
                        invoice.DueDate = Convert.ToDateTime(eventDetails.DueDate);
                        invoice.EventId = eventRegisterModel.EventId;

                        foreach (var item in eventRegisterModel.SelectedSession)
                        {
                            var invoiceDetail = new Invoicedetail();
                            var session = await _unitOfWork.Sessions.GetSessionByIdAsync(item.SessionId);
                            if (session != null)
                            {
                                invoiceDetail.Description = $"{eventDetails.Name}";
                                invoiceDetail.Description += $" : {session.Name}";
                                if (session.StartDatetime!=null && session.EndDateTime!=null)
                                {
                                    invoiceDetail.Description += $" : ({session.StartDatetime.Value.ToString("MM/dd/yyyy hh:mm tt")} - {session.EndDateTime.Value.ToString("MM/dd/yyyy hh:mm tt")})";
                                }
                                if(session.GlAccountId!=null)
                                {
                                    var glAccount = await _unitOfWork.GlAccounts.GetByIdAsync(Convert.ToInt32(session.GlAccountId));
                                    invoiceDetail.GlAccount = glAccount.Code;
                                }
                                invoiceDetail.ItemType = (int)InvoiceItemType.Event;
                                invoiceDetail.Price = item.Price;
                                invoiceDetail.Amount = item.Price;
                                invoiceDetail.Status = (int)InvoiceStatus.Finalized;
                                invoiceDetail.Quantity = 1;
                                invoice.Invoicedetails.Add(invoiceDetail);
                            }
                        }

                        await _unitOfWork.Invoices.AddAsync(invoice);
                        await _unitOfWork.CommitAsync();

                        invoiceId = invoice.InvoiceId;
                    }
                }
                return invoiceId;
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred {Error} {StackTrace} {InnerException} {Source}",
                                  ex.Message, ex.StackTrace, ex.InnerException, ex.Source);
                return 0;
            }
        }

        public async Task<bool> CheckEventRegistrationByEventId(int eventId)
        {
            var events = await _unitOfWork.EventRegister.GetAllEventRegistrationsByEventIdAsync(eventId);
            if (events.Any())
            {
                return true;
            }
            return false;
        }
    }
}
