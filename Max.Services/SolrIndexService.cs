using Max.Core;
using Max.Core.Models;
using Max.Services.Interfaces;
using Serilog;
using SolrNet;
using SolrNet.Commands.Parameters;
using SolrNet.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Max.Services
{
    public class SolrIndexService<T, TSolrOperations> : ISolrIndexService<T>
	 where TSolrOperations : ISolrOperations<T>
	{
		private readonly TSolrOperations _solr;
		static readonly ILogger _logger = Serilog.Log.ForContext<SolrIndexService<T, TSolrOperations>>();
		public SolrIndexService(ISolrOperations<T> solr)
		{
			_solr = (TSolrOperations)solr;
		}
		public bool AddUpdate(T document)
		{
			try
			{
				// If the id already exists, the record is updated, otherwise added                         
				_solr.Add(document);
				_solr.Commit();
				return true;
			}
			catch (SolrNetException ex)
			{
				_logger.Error($"SolrIndexService Error:{ex.Message} {ex.StackTrace}");
				throw ex;
			}
		}

		public string ExtractDocumentText(string fileName)
		{
			using (var file = File.OpenRead(fileName))
			{
				try
				{
					var response = _solr.Extract(new ExtractParameters(file, fileName)
					{
						ExtractOnly = true,
						ExtractFormat = ExtractFormat.Text,
					});

					return response.Content;
				}
				catch (SolrNetException ex)
				{
					_logger.Error($"SolrIndexService Error:{ex.Message} {ex.StackTrace}");
					throw ex;
				}
			}
		}

		public bool Delete(T document)
		{
			try
			{
				//Can also delete by id                
				_solr.Delete(document);
				_solr.Commit();
				return true;
			}
			catch (SolrNetException ex)
			{
				_logger.Error($"SolrIndexService Error:{ex.Message} {ex.StackTrace}");
				return false;
			}
		}

		public IEnumerable<SolrDocumentModel> GetDocumentsByText(string text, string filter, string tenantId)
		{
			List<SolrDocumentModel> documents = new List<SolrDocumentModel>();

			var tokens = text.Trim().Split(" ");
			var searchQuery = string.Empty;
			var filterQuery = $"tenantId:{tenantId}";

			if(tokens.Length > 1)
            {
				searchQuery = text.AddDoubleQuotes()+"~5";
			}
			else
            {
				if(filter == "File Name")
                {
					searchQuery = $"{text}~";
				}
				else
                {
					searchQuery = $"{text}";
				}
			}
			try
			{
				if(filter=="File Name")
                {

					AbstractSolrQuery solrQuery =  new SolrQueryByField("fileName", searchQuery) { Quoted = false }; 
					var results = _solr.Query(solrQuery, new QueryOptions
					{
						Highlight = new HighlightingParameters
						{
							Fields = new[] { "fileName" },
							Fragsize = 150,
							BeforeTerm = "<b>",
							AfterTerm = "</b>",
							Snippets = 2
						},
						Fields = { "*", "score" },
						ExtraParams = new Dictionary<string, string> {
							{"fq", filterQuery}
						},
						Rows =100
					});

					var searchResults = (IEnumerable<SolrDocumentModel>)results;
				
					foreach (SolrDocumentModel doc in searchResults)
					{
						var document = new SolrDocumentModel();
						//extract Doc ID from solr.Id
						var id = document.Id;
						var idTokens = id.Split("-");
						var documentId = idTokens.Last();
						document.Id = documentId;
						document.Score = doc.Score;
						document.HighlightText = doc.FileName;
						documents.Add(document);
					}
				}
				else
				{
					AbstractSolrQuery solrQuery = new SolrQueryByField("text", searchQuery) { Quoted = false };
					var results = _solr.Query(solrQuery, new QueryOptions
					{
						Highlight = new HighlightingParameters
						{
							Fields = new[] { "text"},
							Fragsize = 150,
							BeforeTerm = "<b>",
							AfterTerm = "</b>",
							Snippets = 2,
							MaxAnalyzedChars = -1,
						},
						ExtraParams = new Dictionary<string, string> {

										{"defType", "edismax"},
										{"bq", "createdDate:[NOW-3YEAR/DAY TO NOW/DAY+1DAY]"},
										{"fq", filterQuery}
						},
						Fields = { "id", "score", "tenantId" },
						Rows=100
					});
					var totalCount = results.NumFound;

					var searchResults = (IEnumerable<SolrDocumentModel>)results;
			
					
					foreach (SolrDocumentModel doc in searchResults)
					{
						var document = new SolrDocumentModel();
						//extract Doc ID from solr.Id
						var id = doc.Id;
						var idTokens = id.Split("-");
						var documentId = idTokens.Last();
						document.Id = documentId;
						document.Score = doc.Score;
						foreach (var h in results.Highlights[doc.Id])
						{
							var highlightedText = string.Join(", ", h.Value.ToArray());
							var cleanString = string.Join(" ", Regex.Split(highlightedText, @"(?:\r\n|\n|\r)"));
							document.HighlightText = Regex.Replace(cleanString, @"[^0-9a-zA-Z<>/:\-, ]+", "");
							if (document.HighlightText.Length > 500)
                            {
                                document.HighlightText = document.HighlightText.Substring(0, 500);
                            }
                        }
                        documents.Add(document);
					}
				}
			}
			catch (SolrNetException ex)
			{
				//Log exception
				_logger.Error($"GetSearchCountByText Error:{ex.Message} {ex.StackTrace}");
				throw new Exception(ex.Message);
			}
			return documents;
		}
		public long GetSearchCountByText(string text, string filter, string tenantId)
        {
			long resultsCount = 0;
			var tokens = text.Trim().Split(" ");
			var searchQuery = string.Empty;
			var filterQuery = $"tenantId:{tenantId}";
			if (tokens.Length > 1)
			{
				searchQuery = text.AddDoubleQuotes() + "~5";
			}
			else
			{
				if (filter == "File Name")
				{
					searchQuery = $"{text}~";
				}
				else
				{
					searchQuery = $"{text}";
				}
			}
			try
			{
				if (filter == "File Name")
				{

					AbstractSolrQuery solrQuery = new SolrQueryByField("fileName", searchQuery) { Quoted = false };
					var results = _solr.Query(solrQuery, new QueryOptions
					{
						Highlight = new HighlightingParameters
						{
							Fields = new[] { "fileName" },
							Fragsize = 150,
							BeforeTerm = "<b>",
							AfterTerm = "</b>",
							Snippets = 2
						},
						Fields = { "*", "score" },
						ExtraParams = new Dictionary<string, string> {
							{"fq", filterQuery}
						},
						Rows = 0
					});
					resultsCount = results.NumFound;
				}
				else
				{

					AbstractSolrQuery solrQuery = new SolrQueryByField("text", searchQuery) { Quoted = false };
					var results = _solr.Query(solrQuery, new QueryOptions
					{
						Highlight = new HighlightingParameters
						{
							Fields = new[] { "text" },
							Fragsize = 150,
							BeforeTerm = "<b>",
							AfterTerm = "</b>",
							Snippets = 2,
							MaxAnalyzedChars = -1,
						},
						ExtraParams = new Dictionary<string, string> {
							{"defType", "edismax"},
							{"bq", "createdDate:[NOW-3YEAR/DAY TO NOW/DAY+1DAY]"},
							{"fq", filterQuery}
						},
						Fields = { "*", "score" },
						Rows = 0
					});
					resultsCount = results.NumFound;
				}
			}
			catch (SolrNetException ex)
			{
				//Log exception
				_logger.Error($"GetSearchCountByText Error:{ex.Message} {ex.StackTrace}");
				throw new Exception(ex.Message);
			}
			return resultsCount;
		}
		public IEnumerable<SolrDocumentModel> ExportDocuments(int startPage)
        {
			List<SolrDocumentModel> documents = new List<SolrDocumentModel>();
			try
			{
				AbstractSolrQuery solrQuery = new SolrQuery("*:*");
				var results = _solr.Query(solrQuery, new QueryOptions
				{
					Fields = { "*" },
					//Rows=100,
					Start= startPage
				});
				var searchResults = (IEnumerable<SolrDocumentModel>)results;

				return searchResults;
			}
			catch (SolrNetException ex)
			{
				//Log exception
				_logger.Error($"ExportDocuments Error:{ex.Message} {ex.StackTrace}");
				throw new Exception(ex.Message);
			}
		}
	}
}
