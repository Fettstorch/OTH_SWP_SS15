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
using System.IO;
using System.Linq;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using GraphFramework.Interfaces;

namespace UseCaseAnalyser.Model.Model
{
    /// <summary>
    /// class to export the scenario matrix to a file
    /// </summary>
    public static class ScenarioMatrixExporter
    {

        private const string ExcelExtension = ".xlsx";

        /// <summary>
        /// exports the scenario matrix of the use case graph to the specfified file (.xlsx) 
        /// </summary>
        /// <param name="useCaseGraph">use case graph whose scenario matrix should be exported</param>
        /// <param name="file">file info of the file to save the scenario matrix to</param>
        public static void ExportScenarioMatrix(UseCaseGraph useCaseGraph, FileInfo file)
        {
            if (!string.Equals(file.Extension, ExcelExtension)) throw new InvalidOperationException("wrong file type!");
            SpreadsheetDocument document = SpreadsheetDocument.Create(file.FullName, SpreadsheetDocumentType.Workbook);
                ScenarioMatrixExporter.CreateScenarioMatrix(useCaseGraph, document, 1);
            document.Close();
        }

        /// <summary>
        /// Exports the scenario matrix of all use case graphs to the specfified file (.xlsx)
        /// Each Use Case is a new excel sheet 
        /// </summary>
        /// <param name="useCaseGraphs"></param>
        /// <param name="file">file info of the file to save the scenario matrix to</param>
        public static void ExportScenarioMatrix(UseCaseGraph[] useCaseGraphs, FileInfo file)
        {
            using (SpreadsheetDocument document = SpreadsheetDocument.Open(file.FullName, true))
            {
                for (UInt32Value i = 0; i < useCaseGraphs.Length; i++)
                {
                    var useCaseGraph = useCaseGraphs[i];
                    ScenarioMatrixExporter.CreateScenarioMatrix(useCaseGraph, document, i);
                }
            }
        }

        private static void CreateScenarioMatrix(UseCaseGraph useCaseGraph, SpreadsheetDocument spreadsheetDoc, UInt32Value index)
        {
            string ucName = useCaseGraph.GetAttributeByName("Name").Value as string;
            if (string.IsNullOrEmpty(ucName)) throw new InvalidOperationException("Use case is corrupt!");

            // Add a WorkbookPart to the document.
            WorkbookPart workbookPart = spreadsheetDoc.AddWorkbookPart();
            workbookPart.Workbook = new Workbook();

            // Add a WorksheetPart to the WorkbookPart.
            WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
            worksheetPart.Worksheet = new Worksheet(new SheetData());

            // Add Sheets to the Workbook.
            Sheets sheets = spreadsheetDoc.WorkbookPart.Workbook.AppendChild(new Sheets());

            // Append a new worksheet and associate it with the workbook.
            Sheet sheet = new Sheet()
            {
                Id = spreadsheetDoc.WorkbookPart.
                    GetIdOfPart(worksheetPart),
                SheetId = index,
                Name = ucName
            };

            sheets.Append(sheet);

            workbookPart.Workbook.Save();
        }

        private static int GetNumberOfVariants(UseCaseGraph useCaseGraph)
        {
            int number = 0;
            int lastNumber = 0;
            IGraph scenario;
            IGraph[] scenarios = useCaseGraph.Scenarios.ToArray();                       
            for (int i = 0; i < scenarios.Length; i++)
            {
                INode[] nodes = scenarios[i].Nodes.ToArray();
                IAttribute lastNodeType = nodes[0].GetAttributeByName(UseCaseGraph.AttributeNames[(int)UseCaseGraph.NodeAttributes.NodeType]);
                if (lastNodeType == null) throw new NotImplementedException();

                foreach (var node in scenarios[i].Nodes)
                {
                    IAttribute attr = node.GetAttributeByName(UseCaseGraph.AttributeNames[(int) UseCaseGraph.NodeAttributes.NodeType]);
                    if(attr == null) throw new NotImplementedException();
                    if ((UseCaseGraph.NodeTypeAttribute) attr.Value == UseCaseGraph.NodeTypeAttribute.VariantNode
                        && lastNodeType.Value != attr.Value)
                    {
                        number++;
                    }
                }

                lastNumber = (number > lastNumber) ? number : lastNumber;
            }

            return lastNumber;
        }
    }
}