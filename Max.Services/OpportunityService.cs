using AutoMapper;
using Max.Core;
using Max.Core.Models;
using Max.Data.DataModel;
using Max.Data.Interfaces;
using Max.Services.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Max.Services
{
    public class OpportunityService : IOpportunityService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<OpportunityService> _logger;

        public OpportunityService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<OpportunityService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<OpportunityPipelineModel>> GetAllOpportunityPipelines()
        {
            var opportunityPipelineList = new List<OpportunityPipelineModel>();
            var opportunityPipelines = await _unitOfWork.OpportunityPipelines.GetAllPipelinesAsync();
            foreach (var pipeline in opportunityPipelines)
            {
                var pipelineModel = new OpportunityPipelineModel();

                pipelineModel.PipelineId = pipeline.PipelineId;
                pipelineModel.Status = pipeline.Status;
                pipelineModel.Name = pipeline.Name;

                pipelineModel.PipelineStages = pipeline.Pipelinestages.Count > 0 ?
                    _mapper.Map<List<PipelineStageModel>>(pipeline.Pipelinestages
                    .OrderBy(x => x.StageIndex).ToList()) : null;
                pipelineModel.PipelineProducts = pipeline.Pipelineproducts.Count > 0 ?
                    _mapper.Map<List<PipelineProductModel>>(pipeline.Pipelineproducts
                    .OrderBy(x => x.ProductIndex).ToList()) :
                    null;
                opportunityPipelineList.Add(pipelineModel);
            }

            return opportunityPipelineList;

        }

        public async Task<IEnumerable<OpportunityPipelineModel>> GetAllActiveOpportunityPipelines()
        {
            var opportunities = await _unitOfWork.OpportunityPipelines.GetAllActivePipelinesAsync();

            return _mapper.Map<List<OpportunityPipelineModel>>(opportunities);
        }

        public async Task<OpportunityPipelineModel> GetOpportunityPipelineById(int id)
        {
            var opportunityPipeline = await _unitOfWork.OpportunityPipelines.GetPiplelineByIdAsync(id);

            var pipelineModel = new OpportunityPipelineModel();

            pipelineModel.PipelineId = opportunityPipeline.PipelineId;
            pipelineModel.Status = opportunityPipeline.Status;
            pipelineModel.Name = opportunityPipeline.Name;

            pipelineModel.PipelineStages = opportunityPipeline.Pipelinestages.Count > 0 ?
                _mapper.Map<List<PipelineStageModel>>(opportunityPipeline.Pipelinestages
                .OrderBy(x => x.StageIndex).ToList()) :
                new List<PipelineStageModel>();
            pipelineModel.PipelineProducts = opportunityPipeline.Pipelineproducts.Count > 0 ?
                _mapper.Map<List<PipelineProductModel>>(opportunityPipeline.Pipelineproducts
                .OrderBy(x => x.ProductIndex).ToList()) :
                new List<PipelineProductModel>();
            return pipelineModel;
        }


        public async Task<OpportunityPipelineModel> CreateOpportunityPipeline(OpportunityPipelineModel model)
        {

            var isValidName = await ValidPipeline(model);
            if (isValidName)
            {
                var opportunityPipeline = new Opportunitypipeline();

                opportunityPipeline.Name = model.Name;
                opportunityPipeline.Status = model.Status;

                //Check if it has stages

                if (model.PipelineStages.Count > 0)
                {
                    var index = 1;
                    foreach (var stage in model.PipelineStages)
                    {
                        var pipelineStage = new Pipelinestage();

                        pipelineStage.StageName = stage.StageName;
                        pipelineStage.Probability = stage.Probability;
                        pipelineStage.Status = (int)Status.Active;
                        pipelineStage.StageIndex = index;

                        opportunityPipeline.Pipelinestages.Add(pipelineStage);
                        index++;
                    }
                }

                //Check if it has products

                if (model.PipelineProducts.Count > 0)
                {
                    var index = 1;
                    foreach (var prouduct in model.PipelineProducts)
                    {
                        var pipelineProduct = new Pipelineproduct();

                        pipelineProduct.ProductName = prouduct.ProductName;
                        pipelineProduct.Status = (int)Status.Active;
                        pipelineProduct.ProductIndex = index;

                        opportunityPipeline.Pipelineproducts.Add(pipelineProduct);
                        index++;

                    }
                }

                try
                {
                    await _unitOfWork.OpportunityPipelines.AddAsync(opportunityPipeline);
                    await _unitOfWork.CommitAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError("An error occurred {Error} {StackTrace} {InnerException} {Source}",
                             ex.Message, ex.StackTrace, ex.InnerException, ex.Source);
                }
                return _mapper.Map<OpportunityPipelineModel>(opportunityPipeline);
            }
            return new OpportunityPipelineModel();
        }

        public async Task<bool> UpdateOpportunityPipeline(OpportunityPipelineModel model)
        {
            if (model == null)
            {
                return false;
            }

            //Check if pipeline exists

            var opporunityPipeline = await _unitOfWork.OpportunityPipelines.GetPiplelineByIdAsync(model.PipelineId);

            if (opporunityPipeline == null)
            {
                return false;
            }

            //Validate that pipeline name is still uniqueue
            var isValidName = await ValidPipeline(model);

            if (isValidName)
            {
                opporunityPipeline.Name = model.Name;
                opporunityPipeline.Status = model.Status;

                //Check if any stage has been removed/Updated
                var stages = opporunityPipeline.Pipelinestages.ToList();

                foreach (var item in stages)
                {
                    if (model.PipelineStages.Any(x => x.StageId == item.StageId))
                    {
                        var stage = model.PipelineStages.Where(x => x.StageId == item.StageId).FirstOrDefault();
                        item.StageName = stage.StageName;
                        item.StageIndex = stage.StageIndex;
                        item.Probability = stage.Probability;
                        item.Status = stage.Status;
                        try
                        {
                            _unitOfWork.PipelineStages.Update(item);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError($"Update Pipeline: failed with error {ex.Message} {ex.StackTrace} {ex.InnerException.Message}");
                            throw new Exception($"Failed to update Pipeline Stage");
                        }

                        continue;

                    }

                    _unitOfWork.PipelineStages.Remove(item);
                    opporunityPipeline.Pipelinestages.Remove(item);
                }
                //Add  Stages
                //Newly added stage shall go to the end
                var newStageIndex = model.PipelineStages.Select(x => x.StageIndex).Max();
                foreach (var item in model.PipelineStages.Where(x => x.StageId == 0).ToList())
                {
                    Pipelinestage stage = new Pipelinestage();
                    stage.StageName = item.StageName;
                    stage.StageIndex = ++newStageIndex;
                    stage.Probability = item.Probability;
                    stage.Status = (int)Status.Active;
                    try
                    {
                        opporunityPipeline.Pipelinestages.Add(stage);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"Update Pipeline: failed with error {ex.Message} {ex.StackTrace} {ex.InnerException.Message}");
                        throw new Exception($"Failed to update Pipeline stage");
                    }
                }

                //Check if any product has been removed/Updated
                var products = opporunityPipeline.Pipelineproducts.ToList();

                foreach (var item in products)
                {
                    if (model.PipelineProducts.Any(x => x.ProductId == item.ProductId))
                    {
                        var product = model.PipelineProducts.Where(x => x.ProductId == item.ProductId).FirstOrDefault();
                        item.ProductName = product.ProductName;
                        item.ProductIndex = product.ProductIndex;
                        item.Status = product.Status;
                        try
                        {
                            _unitOfWork.PipelineProducts.Update(item);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError($"Update Pipeline: failed with error {ex.Message} {ex.StackTrace} {ex.InnerException.Message}");
                            throw new Exception($"Failed to update Pipeline Product");
                        }

                        continue;
                    }

                    _unitOfWork.PipelineProducts.Remove(item);
                    opporunityPipeline.Pipelineproducts.Remove(item);
                }
                //Add  Products
                //Newly added product shall go to the end
                var newProductIndex = model.PipelineProducts.Select(x => x.ProductIndex).Max();
                foreach (var item in model.PipelineProducts.Where(x => x.ProductId == 0).ToList())
                {
                    Pipelineproduct product = new Pipelineproduct();
                    product.ProductName = item.ProductName;
                    product.ProductIndex = ++newProductIndex;
                    product.Status = (int)Status.Active;
                    try
                    {
                        opporunityPipeline.Pipelineproducts.Add(product);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"Update Pipeline: failed with error {ex.Message} {ex.StackTrace} {ex.InnerException.Message}");
                        throw new Exception($"Failed to update Pipeline stage");
                    }
                }
                try
                {
                    _unitOfWork.OpportunityPipelines.Update(opporunityPipeline);
                    await _unitOfWork.CommitAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError("An error occurred {Error} {StackTrace} {InnerException} {Source}",
                             ex.Message, ex.StackTrace, ex.InnerException, ex.Source);
                }
                return true;
            }
            return false;
        }



        public async Task<bool> DeleteOpportunityPipeline(int id)
        {
            var opporunityPipeline = await _unitOfWork.OpportunityPipelines.GetPiplelineByIdAsync(id);
            if (opporunityPipeline != null)
            {
                //check if there are no related opportunities that are linked.
                var opportunities = await _unitOfWork.Opportunities.GetOpportunitiesByPipelineIdAsync(id);
                if (opportunities == null || !opportunities.Any())
                {
                    try
                    {
                        _unitOfWork.OpportunityPipelines.Remove(opporunityPipeline);
                        await _unitOfWork.CommitAsync();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError("An error occurred {Error} {StackTrace} {InnerException} {Source}",
                                 ex.Message, ex.StackTrace, ex.InnerException, ex.Source);
                    }
                }
                else
                {
                    return false;
                }

            }
            throw new InvalidOperationException($"Opportunity Pipeline: {id} not found.");
        }

        public async Task<OpportunityPipelineModel> CloneOpportunityPipeline(OpportunityPipelineModel pipelineModel)
        {

            //Check if pipeline exists

            var opporunityPipeline = await _unitOfWork.OpportunityPipelines.GetPiplelineByIdAsync(pipelineModel.PipelineId);

            if (opporunityPipeline == null)
            {
                return new OpportunityPipelineModel();
            }
            //Create clone pipeline
            var pipeline = await CreateOpportunityPipeline(pipelineModel);

            return pipeline;
        }


        public Task<bool> UpdatePipelineStage(PipelineStageModel model)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeletePipelineStage(int id)
        {
            throw new NotImplementedException();
        }

        public Task<PipelineProductModel> CreatePipelineProduct(PipelineProductModel model)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdatePipelineProduct(PipelineProductModel model)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeletePipelineProduct(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<OpportunityModel> CreateOpportunity(OpportunityModel model)
        {

            var opportunity = new Opportunity();

            opportunity.PipelineId = model.PipelineId;
            opportunity.CompanyId = model.CompanyId;
            opportunity.ProductId = model.ProductId;
            opportunity.CreatedDate = DateTime.Now;
            opportunity.EstimatedCloseDate = model.EstimatedCloseDate;
            opportunity.Potential = model.Potential;
            opportunity.StaffUserId = model.StaffUserId;
            opportunity.AccountContactId = model.AccountContactId;
            opportunity.StageId = model.StageId;
            opportunity.Probability = model.Probability;
            opportunity.Notes = model.Notes;
            try
            {
                await _unitOfWork.Opportunities.AddAsync(opportunity);
                await _unitOfWork.CommitAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred {Error} {StackTrace} {InnerException} {Source}",
                            ex.Message, ex.StackTrace, ex.InnerException, ex.Source);
            }
            return _mapper.Map<OpportunityModel>(opportunity);
        }

        public async Task<bool> UpdateOpportunity(OpportunityModel model)
        {
            var opportunity = await _unitOfWork.Opportunities.GetByIdAsync(model.OpportunityId);

            if (opportunity != null)
            {
                opportunity.PipelineId = model.PipelineId;
                opportunity.ProductId = model.ProductId;
                opportunity.EstimatedCloseDate = model.EstimatedCloseDate;
                opportunity.Potential = model.Potential;
                opportunity.StaffUserId = model.StaffUserId;
                opportunity.AccountContactId = model.AccountContactId;
                opportunity.StageId = model.StageId;
                opportunity.Probability = model.Probability;
                opportunity.Notes = model.Notes;

                try
                {
                    _unitOfWork.Opportunities.Update(opportunity);
                    await _unitOfWork.CommitAsync();
                    return true;
                }
                catch (Exception ex)
                {
                    _logger.LogError("An error occurred {Error} {StackTrace} {InnerException} {Source}",
                                ex.Message, ex.StackTrace, ex.InnerException, ex.Source);
                }
            }
            return false;
        }
        public async Task<IEnumerable<OpportunityStageGroupModel>> MoveOpportunityToAnotherStage(OpportunityModel model)
        {
            //Validate opportunity
            var opportunity = await _unitOfWork.Opportunities.GetOpportunityByIdAsync(model.OpportunityId);

            if (opportunity != null)
            {
                var stage = await _unitOfWork.PipelineStages.GetPiplelineStageByIdAsync(model.StageId);

                if (stage != null)
                {
                    opportunity.StageId = stage.StageId;
                    opportunity.Probability = stage.Probability;

                    try
                    {
                        _unitOfWork.Opportunities.Update(opportunity);
                        await _unitOfWork.CommitAsync();
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError("An error occurred {Error} {StackTrace} {InnerException} {Source}",
                                    ex.Message, ex.StackTrace, ex.InnerException, ex.Source);
                    }


                }
            }
            //Return updated List
            return await GetAllOpportunitesByPipelineIdGroupedByStages(model.PipelineId);
        }
        public async Task<IEnumerable<OpportunityModel>> GetAllOpportunites()
        {

            var opportunities = await _unitOfWork.Opportunities.GetAllOpportunitiesAsync();

            return _mapper.Map<List<OpportunityModel>>(opportunities);
        }

        public async Task<IEnumerable<OpportunityModel>> GetAllOpportunitesByAccountContactId(int id)
        {

            var opportunities = await _unitOfWork.Opportunities.GetOpportunitiesByAccountContactAsync(id);

            return _mapper.Map<List<OpportunityModel>>(opportunities);
        }

        public async Task<IEnumerable<OpportunityModel>> GetAllOpportunitesByPipelineId(int id)
        {

            var opportunities = await _unitOfWork.Opportunities.GetOpportunitiesByPipelineIdAsync(id);

            var opportunityList = new List<OpportunityModel>();

            foreach (var opportunity in opportunities)
            {
                var item = new OpportunityModel();

                item.OpportunityId = opportunity.OpportunityId;
                item.PipelineId = opportunity.PipelineId ?? 0;
                item.CompanyId = opportunity.CompanyId ?? 0;
                item.StageId = opportunity.StageId ?? 0;
                item.ProductId = opportunity.ProductId ?? 0;
                item.Potential = opportunity?.Potential ?? 0;
                item.EstimatedCloseDate = opportunity?.EstimatedCloseDate ?? Constants.MySQL_MinDate;
                item.AccountContactId = opportunity?.AccountContactId ?? 0;
                item.ContactName = opportunity.AccountContact?.Name ?? null;
                item.PipelineName = opportunity.Pipeline.Name;
                item.StageName = opportunity.Pipeline.Pipelinestages.Where(x => x.StageId == opportunity.StageId).Select(x => x.StageName).FirstOrDefault();
                item.CompanyName = opportunity.Company.CompanyName;
                item.OwnerName = $"{opportunity.StaffUser.FirstName} {opportunity.StaffUser.LastName}";
                item.ProductName = opportunity.Pipeline.Pipelineproducts.Where(x => x.ProductId == opportunity.ProductId).Select(x => x.ProductName).FirstOrDefault();
                item.Probability = opportunity.Probability ?? 0;
                item.Notes = $"{opportunity.Notes}";
                opportunityList.Add(item);
            }
            return opportunityList;
        }

        public async Task<IEnumerable<OpportunityModel>> GetAllOpportunitesByCompanyId(int id)
        {

            var opportunities = await _unitOfWork.Opportunities.GetOpportunitiesByCompanyIdAsync(id);

            var opportunityList = new List<OpportunityModel>();

            foreach (var opportunity in opportunities)
            {
                var item = new OpportunityModel();

                item.OpportunityId = opportunity.OpportunityId;
                item.PipelineId = opportunity.PipelineId ?? 0;
                item.CompanyId = opportunity.CompanyId ?? 0;
                item.StageId = opportunity.StageId ?? 0;
                item.ProductId = opportunity.ProductId ?? 0;
                item.Potential = opportunity?.Potential ?? 0;
                item.EstimatedCloseDate = opportunity?.EstimatedCloseDate ?? Constants.MySQL_MinDate;
                item.AccountContactId = opportunity?.AccountContactId ?? 0;
                item.ContactName = opportunity.AccountContact?.Name ?? null;
                item.PipelineName = opportunity.Pipeline.Name;
                item.StageName = opportunity.Pipeline.Pipelinestages.Where(x => x.StageId == opportunity.StageId).Select(x => x.StageName).FirstOrDefault();
                item.CompanyName = opportunity.Company.CompanyName;
                item.OwnerName = $"{opportunity.StaffUser.FirstName} {opportunity.StaffUser.LastName}";
                item.ProductName = opportunity.Pipeline.Pipelineproducts.Where(x => x.ProductId == opportunity.ProductId).Select(x => x.ProductName).FirstOrDefault();
                item.Probability = opportunity.Probability ?? 0;
                item.Notes = $"{opportunity.Notes}";
                opportunityList.Add(item);
            }
            return opportunityList;
        }

        public async Task<IEnumerable<OpportunityStageGroupModel>> GetOpportunitiesBySearchText(int pipelineId, string searchText)
        {

            var opportunities = await _unitOfWork.Opportunities.GetOpportunitiesBySearchTextAsync(pipelineId, searchText);

            var currentPipeline = await _unitOfWork.OpportunityPipelines.GetPiplelineByIdAsync(pipelineId);

            var opportunityStageGroups = new List<OpportunityStageGroupModel>();

            var currentOpportunities = opportunities.Where(x => x.PipelineId == pipelineId).OrderByDescending(x => x.OpportunityId).ToList();
            foreach (var stage in currentPipeline.Pipelinestages.OrderBy(x => x.StageIndex))
            {
                var stageModel = new OpportunityStageGroupModel();
                stageModel.stageId = stage.StageId;
                stageModel.stageName = stage.StageName;
                var stageOopportunities = currentOpportunities.Where(x => x.StageId == stage.StageId).ToList();
                foreach (var stageOpportunity in stageOopportunities)
                {
                    //Map opportunity
                    var item = new OpportunityModel();

                    item.OpportunityId = stageOpportunity.OpportunityId;
                    item.PipelineId = stageOpportunity.PipelineId ?? 0;
                    item.CompanyId = stageOpportunity.CompanyId ?? 0;
                    item.StageId = stageOpportunity.StageId ?? 0;
                    item.ProductId = stageOpportunity.ProductId ?? 0;
                    item.Potential = stageOpportunity?.Potential ?? 0;
                    item.EstimatedCloseDate = stageOpportunity?.EstimatedCloseDate ?? Constants.MySQL_MinDate;
                    item.AccountContactId = stageOpportunity?.AccountContactId ?? 0;
                    item.ContactName = stageOpportunity.AccountContact?.Name ?? null;
                    item.PipelineName = stageOpportunity.Pipeline.Name;
                    item.StageName = stageOpportunity.Pipeline.Pipelinestages.Where(x => x.StageId == stageOpportunity.StageId).Select(x => x.StageName).FirstOrDefault();
                    item.CompanyName = stageOpportunity.Company.CompanyName;
                    item.OwnerName = $"{stageOpportunity.StaffUser.FirstName} {stageOpportunity.StaffUser.LastName}";
                    item.ProductName = stageOpportunity.Pipeline.Pipelineproducts.Where(x => x.ProductId == stageOpportunity.ProductId).Select(x => x.ProductName).FirstOrDefault();
                    item.Probability = stageOpportunity.Probability ?? 0;
                    item.Notes = stageOpportunity.Notes;
                    item.Company = _mapper.Map<CompanyModel>(stageOpportunity.Company);
                    stageModel.Opportunities.Add(item);
                }
                opportunityStageGroups.Add(stageModel);
            }

            return opportunityStageGroups;
        }

        public async Task<IEnumerable<OpportunityStageGroupModel>> GetAllOpportunitesByPipelineIdGroupedByStages(int id)
        {

            var opportunities = await _unitOfWork.Opportunities.GetOpportunitiesByPipelineIdAsync(id);
            var currentPipeline = await _unitOfWork.OpportunityPipelines.GetPiplelineByIdAsync(id);

            var opportunityStageGroups = new List<OpportunityStageGroupModel>();

            var currentOpportunities = opportunities.Where(x => x.PipelineId == id).OrderByDescending(x => x.OpportunityId).ToList();
            foreach (var stage in currentPipeline.Pipelinestages.OrderBy(x => x.StageIndex))
            {
                var stageModel = new OpportunityStageGroupModel();
                stageModel.stageId = stage.StageId;
                stageModel.stageName = stage.StageName;
                var stageOopportunities = currentOpportunities.Where(x => x.StageId == stage.StageId).ToList();
                foreach (var stageOpportunity in stageOopportunities)
                {
                    //Map opportunity
                    var item = new OpportunityModel();

                    item.OpportunityId = stageOpportunity.OpportunityId;
                    item.PipelineId = stageOpportunity.PipelineId ?? 0;
                    item.CompanyId = stageOpportunity.CompanyId ?? 0;
                    item.StageId = stageOpportunity.StageId ?? 0;
                    item.ProductId = stageOpportunity.ProductId ?? 0;
                    item.Potential = stageOpportunity?.Potential ?? 0;
                    item.EstimatedCloseDate = stageOpportunity?.EstimatedCloseDate ?? Constants.MySQL_MinDate;
                    item.AccountContactId = stageOpportunity?.AccountContactId ?? 0;
                    item.ContactName = stageOpportunity.AccountContact?.Name ?? null;
                    item.PipelineName = stageOpportunity.Pipeline.Name;
                    item.StageName = stageOpportunity.Pipeline.Pipelinestages.Where(x => x.StageId == stageOpportunity.StageId).Select(x => x.StageName).FirstOrDefault();
                    item.CompanyName = stageOpportunity.Company.CompanyName;
                    item.OwnerName = $"{stageOpportunity.StaffUser.FirstName} {stageOpportunity.StaffUser.LastName}";
                    item.ProductName = stageOpportunity.Pipeline.Pipelineproducts.Where(x => x.ProductId == stageOpportunity.ProductId).Select(x => x.ProductName).FirstOrDefault();
                    item.Probability = stageOpportunity.Probability ?? 0;
                    item.Notes = stageOpportunity.Notes;
                    item.Company = _mapper.Map<CompanyModel>(stageOpportunity.Company);
                    stageModel.Opportunities.Add(item);
                }
                opportunityStageGroups.Add(stageModel);
            }

            return opportunityStageGroups;
        }

        public async Task<OpportunityModel> GetOpportunityById(int id)
        {
            var opportunity = await _unitOfWork.Opportunities.GetOpportunityByIdAsync(id);

            return _mapper.Map<OpportunityModel>(opportunity);
        }

        public async Task<List<SelectListModel>> GetPipelineSelectList()
        {
            var pipelines = await _unitOfWork.OpportunityPipelines.GetAllPipelinesAsync();
            pipelines = pipelines.OrderByDescending(x => x.Status)
                          .ThenByDescending(x => x.PipelineId).ToList();

            List<SelectListModel> selectList = new List<SelectListModel>();
            foreach (var pipeline in pipelines)
            {
                SelectListModel selectListItem = new SelectListModel();
                selectListItem.code = pipeline.PipelineId.ToString();
                selectListItem.name = pipeline.Name;
                selectList.Add(selectListItem);
            }
            return selectList;
        }

        public async Task<List<SelectListModel>> GetStageSelectList(int pipelineId)
        {
            var pipeline = await _unitOfWork.OpportunityPipelines.GetPiplelineByIdAsync(pipelineId);

            List<SelectListModel> selectList = new List<SelectListModel>();
            foreach (var stage in pipeline.Pipelinestages.OrderBy(x => x.StageIndex))
            {
                SelectListModel selectListItem = new SelectListModel();
                selectListItem.code = stage.StageId.ToString();
                selectListItem.name = stage.StageName;
                selectList.Add(selectListItem);
            }
            return selectList;
        }

        public async Task<List<SelectListModel>> GetProductSelectList(int pipelineId)
        {
            var pipeline = await _unitOfWork.OpportunityPipelines.GetPiplelineByIdAsync(pipelineId);

            List<SelectListModel> selectList = new List<SelectListModel>();
            foreach (var product in pipeline.Pipelineproducts.OrderBy(x => x.ProductIndex))
            {
                SelectListModel selectListItem = new SelectListModel();
                selectListItem.code = product.ProductId.ToString();
                selectListItem.name = product.ProductName;
                selectList.Add(selectListItem);
            }
            return selectList;
        }

        public async Task<IEnumerable<OpportunityModel>> GetOpportunitesByStageId(int id)
        {
            var opportunities = await _unitOfWork.Opportunities.GetOpportunitiesByStageIdAsync(id);

            var opportunityList = _mapper.Map<List<OpportunityModel>>(opportunities);
            return opportunityList;
        }

        public async Task<IEnumerable<OpportunityModel>> GetOpportunitesByProductId(int id)
        {
            var opportunities = await _unitOfWork.Opportunities.GetOpportunitiesByProductIdAsync(id);

            var opportunityList = _mapper.Map<List<OpportunityModel>>(opportunities);
            return opportunityList;
        }

        public async Task<List<PipelineStageModel>> GetStagesByPipelineId(int pipelineId)
        {
            var pipeline = await _unitOfWork.PipelineStages.GetAllPipelineStagesByPipelineIdAsync(pipelineId);
            var model = _mapper.Map<List<PipelineStageModel>>(pipeline);
            return model;
        }


        private async Task<bool> ValidPipeline(OpportunityPipelineModel model)
        {

            //Validate  Name
            if (model.Name.IsNullOrEmpty())
            {
                throw new InvalidOperationException($"Pipline name can not be empty.");
            }

            //Check if Pipeline already exists

            var pipeline = await _unitOfWork.OpportunityPipelines.GetPipelineByNameAsync(model.Name);

            if (pipeline != null && pipeline.PipelineId != model.PipelineId)
            {
                throw new InvalidOperationException($"This name has already been taken. Please choose a different name.");
            }
            return true;
        }
    }
}