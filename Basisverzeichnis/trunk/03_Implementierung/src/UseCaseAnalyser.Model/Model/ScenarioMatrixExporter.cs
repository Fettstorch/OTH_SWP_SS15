#region Copyright information
// <summary>
// <copyright file="ScenarioMatrixExporter.cs">Copyright (c) 2015</copyright>
// 
// <creationDate>22/04/2015</creationDate>
// 
// <professor>Prof. Dr. Kurt Hoffmann</professor>
// <studyCourse>Angewandte Informatik</studyCourse>
// <branchOfStudy>Industrieinformatik</branchOfStudy>
// <subject>Software Projekt</subject>
// </summary>
#endregion

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using GraphFramework.Interfaces;
using LogManager;

// ReSharper disable PossiblyMistakenUseOfParamsMethod
// ReSharper disable PossibleMultipleEnumeration
// ReSharper disable InconsistentNaming

namespace UseCaseAnalyser.Model.Model
{
    /// <summary>
    /// class to export the scenario matrix to a file
    /// </summary>
    public static class ScenarioMatrixExporter
    {

        private const string ExcelExtension = ".xlsx";
        private const string COrder = "Order of Visit";

        /// <summary>
        /// exports the scenario matrix of one ore more use case graphs to the specfified file (.xlsx) 
        /// </summary>
        /// <param name="useCaseGraphs">use case graph(s) whose scenario matrix should be exported</param>
        /// <param name="file">file info of the file to save the scenario matrix to</param>
        public static void ExportScenarioMatrix(IEnumerable<UseCaseGraph> useCaseGraphs, FileInfo file)
        {
            ValidateFile(file);
            LoggingFunctions.Debug("Creating Excel file");
            using (SpreadsheetDocument document = SpreadsheetDocument.Create(file.FullName, SpreadsheetDocumentType.Workbook))
            {
                CreateAndFillExcelPages(useCaseGraphs, document);
            }
            LoggingFunctions.Debug("Done.");
        }

        // ReSharper disable once UnusedParameter.Local
        // [Mathias Schneider] needed for checking for exceptions
        /// <summary>
        /// Helper method to check if file complies with preconditions.
        /// </summary>
        /// <param name="file">FileInfo that should be checked.</param>
        private static void ValidateFile(FileInfo file)
        {
            if (file == null || file.FullName.Equals(ExcelExtension))
                throw new InvalidOperationException("Please specify an output file name.");
            if (!string.Equals(file.Extension, ExcelExtension))
                throw new InvalidOperationException("Wrong file type, please specify a *." + ExcelExtension);
        }

        /// <summary>
        /// Creates the excel pages.
        /// </summary>
        /// <param name="useCaseGraphs">The use case graphs.</param>
        /// <param name="document">The excel document.</param>
        /// <exception cref="System.InvalidOperationException">Use case is corrupt!</exception>
        private static void CreateAndFillExcelPages(IEnumerable<UseCaseGraph> useCaseGraphs, SpreadsheetDocument document)
        {
            // setting up document
            WorkbookPart workbookPart1 = document.AddWorkbookPart();
            Workbook workbook1 = new Workbook();
            workbookPart1.Workbook = workbook1;
            Sheets sheets1 = new Sheets();
            workbook1.Append(sheets1);
            workbook1.AddNamespaceDeclaration("r", "http://schemas.openxmlformats.org/officeDocument/2006/relationships");

            // creating pages
            for (uint i = 0; i < useCaseGraphs.Count(); i++)
            {
                UseCaseGraph useCaseGraph = useCaseGraphs.ElementAt((int)i);
                string ucName = useCaseGraph.GetAttributeByName("Name").Value as string;
                if (string.IsNullOrEmpty(ucName))
                {
                    LoggingFunctions.Error("Use case name is not valid.");
                    throw new InvalidOperationException("Use case is corrupt!");
                }

                LoggingFunctions.Debug("Creating Excel sheet for usecase " + ucName);

                string id = "rId" + (i + 1);
                Sheet sheet1 = new Sheet { Name = ucName, SheetId = i + 1, Id = id };
                sheets1.Append(sheet1);
                WorksheetPart worksheetPart1 = workbookPart1.AddNewPart<WorksheetPart>(id);
                Worksheet worksheet1 = new Worksheet();
                SheetData sheetData1 = new SheetData();

                // writing scenarios into page
                WriteScenarios(useCaseGraph.Scenarios, sheetData1);

                worksheet1.Append(sheetData1);
                worksheetPart1.Worksheet = worksheet1;
            }
        }

        /// <summary>
        /// Fills the excel page.
        /// </summary>
        /// <param name="scenarios">The scenarios.</param>
        /// <param name="sd">The sd.</param>
        private static void WriteScenarios(IEnumerable<IGraph> scenarios, SheetData sd)
        {
            Row header = new Row();
            sd.Append(header);

            int maxAbzweigungenGefunden = 0;
            int rowcount = 1;

            for (int scenario = 0; scenario < scenarios.Count(); scenario++)
            {
                string order = (string)scenarios.ElementAt(scenario).GetAttributeByName(COrder).Value;
                string[] knotenNamen = order.Split(new[] { ' ', '\n', '\r', '\t' }, StringSplitOptions.RemoveEmptyEntries);

                // auskommentiert da auch der normale ablauf in der excel tabelle sein soll
                //if (!knotenNamen.Any(KnotenIstAblaufVariante)) continue; // das scenario durchläuft keine abzweigungen

                // erste und zweite zelle jeder zeile
                Row row = new Row();
                string adresse = GetExcelAdressFromXY(1, rowcount + 1);
                Cell cell = new Cell { CellReference = adresse, DataType = CellValues.InlineString };
                InlineString inlineString1 = new InlineString();
                Text text1 = new Text { Text = (scenario + 1).ToString() };
                inlineString1.Append(text1);
                cell.Append(inlineString1);
                row.Append(cell);
                adresse = GetExcelAdressFromXY(2, rowcount + 1);
                cell = new Cell { CellReference = adresse, DataType = CellValues.InlineString };
                row.Append(cell);

                int abzweigungenGefunden = 0;
                for (int i = 1; i < knotenNamen.Length; i++)
                {
                    if (KnotenIstAblaufVariante(knotenNamen[i]) && !KnotenIstAblaufVariante(knotenNamen[i - 1]))
                    {
                        abzweigungenGefunden++;

                        adresse = GetExcelAdressFromXY(abzweigungenGefunden + 2, rowcount + 1);
                        Cell cell1 = new Cell { CellReference = adresse, DataType = CellValues.InlineString };
                        inlineString1 = new InlineString();
                        text1 = new Text { Text = knotenNamen[i] };
                        inlineString1.Append(text1);
                        cell1.Append(inlineString1);
                        row.Append(cell1);
                    }
                }
                //if (abzweigungenGefunden > 0) // auskommentiert da auch der normale ablauf in der excel tabelle sein soll
                {
                    rowcount++;
                    maxAbzweigungenGefunden = Math.Max(abzweigungenGefunden, maxAbzweigungenGefunden);
                    sd.Append(row);
                }
            }

            LoggingFunctions.Debug(String.Format("Wrote {0} scenarios in excel file", scenarios.Count()));

            //header befüllen
            for (int x = 0; x < maxAbzweigungenGefunden + 2; x++)
            {
                string text;
                if (x == 0) { text = "ID"; }
                else if (x == 1) { text = "Beschreibung"; }
                else { text = "V" + (x - 1); }

                string headerAdress = GetExcelAdressFromXY(x + 1, 1);
                Cell headerCell = new Cell { CellReference = headerAdress, DataType = CellValues.InlineString };
                InlineString headerInlineString = new InlineString();
                Text headerText = new Text { Text = text };
                headerInlineString.Append(headerText);
                headerCell.Append(headerInlineString);
                header.Append(headerCell);
            }
        }

        private static string GetExcelAdressFromXY(int x, int y)
        {
            int dividend = x;
            string columnName = String.Empty;

            while (dividend > 0)
            {
                int modulo = (dividend - 1) % 26;
                columnName = Convert.ToChar(65 + modulo) + columnName;
                dividend = ((dividend - modulo) / 26);
            }

            return columnName + y;
        }

        private static bool KnotenIstAblaufVariante(string name)
        {
            return Regex.Match(name, @"[a-zA-Z]").Success;

        }
    }
}