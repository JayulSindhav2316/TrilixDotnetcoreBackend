using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System;
using System.IO;
using Telerik.WebReportDesigner.Services;

namespace Max.Reporting.Infrastructure.MySqlStorage
{
    internal static class ValidationExtensions
    {
        internal static readonly char[] invalidFileNameChars = Path.GetInvalidFileNameChars().Union(new char[1] { '+' }).ToArray();

        internal static readonly char[] invalidFolderNameChars = Path.GetInvalidPathChars().ToArray();

        internal static string[] ValidReportDefinitionIdFileExtensions { get; } = new string[3] { ".trdx", ".trdp", ".trbp" };


        internal static string[] ValidSdsDefinitionIdFileExtensions { get; } = new string[2] { ".sdsx", ".sdsp" };


       
        private static void ValidateDefinition(string[] definitionPathParts, string definitionId)
        {
            definitionId.ValidateReportDefinitionId();
            Array.ForEach(definitionPathParts, delegate (string p)
            {
                p.ValidateDefinitionPath();
            });
        }

        internal static void ValidateDefinitionPath(this string definitionUri)
        {
            ValidateDefinition(definitionUri, invalidFolderNameChars);
        }

        internal static void ValidateReportDefinitionId(this string definitionId)
        {
            ValidateDefinitionIdCore(definitionId, ValidReportDefinitionIdFileExtensions);
        }

        internal static void ValidateSharedDataSourceDefinitionId(this string sdsId)
        {
            ValidateDefinitionIdCore(sdsId, ValidSdsDefinitionIdFileExtensions);
        }

        private static void ValidateDefinitionIdCore(string definitionId, string[] validFileExtensions)
        {
            ValidateDefinition(definitionId, invalidFileNameChars);
            string extension = Path.GetExtension(definitionId);
            if (string.IsNullOrEmpty(extension))
            {
                throw new ValidationException("No file extension provided.");
            }

            if (definitionId == extension)
            {
                throw new ValidationException("No file name provided.");
            }

            if (!validFileExtensions.Contains<string>(extension, StringComparer.OrdinalIgnoreCase))
            {
                DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(57, 2);
                defaultInterpolatedStringHandler.AppendLiteral("Unsupported file extension ");
                defaultInterpolatedStringHandler.AppendFormatted(extension);
                defaultInterpolatedStringHandler.AppendLiteral(". Supported file extensions: ");
                defaultInterpolatedStringHandler.AppendFormatted(string.Join(", ", ValidReportDefinitionIdFileExtensions));
                defaultInterpolatedStringHandler.AppendLiteral(".");
                throw new ValidationException(defaultInterpolatedStringHandler.ToStringAndClear());
            }
        }

        private static void ValidateDefinition(string definitionId, char[] invalidCharactersSet)
        {
            if (string.IsNullOrEmpty(definitionId))
            {
                throw new ValidationException("Definition id is null or empty.");
            }

            IEnumerable<char> allInvalidCharacters = GetAllInvalidCharacters(definitionId, invalidCharactersSet);
            if (allInvalidCharacters.Any())
            {
                IEnumerable<string> values = allInvalidCharacters.Select(delegate (char c)
                {
                    DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(2, 1);
                    defaultInterpolatedStringHandler.AppendLiteral("'");
                    defaultInterpolatedStringHandler.AppendFormatted(c);
                    defaultInterpolatedStringHandler.AppendLiteral("'");
                    return defaultInterpolatedStringHandler.ToStringAndClear();
                });
                throw new ValidationException("Definition id contains characters that are not valid file name characters - " + string.Join(", ", values) + ".");
            }
        }

        internal static IEnumerable<char> GetAllInvalidCharacters(string definitionId, IEnumerable<char> invalidChars)
        {
            foreach (char invalidChar in invalidChars)
            {
                if (definitionId.IndexOf(invalidChar) > -1)
                {
                    yield return invalidChar;
                }
            }
        }
    }

}
