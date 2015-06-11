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
        private const string COrder = "Order of Visit";

        /// <summary>
        /// exports the scenario matrix of the use case graph to the specfified file (.xlsx) 
        /// </summary>
        /// <param name="useCaseGraph">use case graph whose scenario matrix should be exported</param>
        /// <param name="file">file info of the file to save the scenario matrix to</param>
        public static void ExportScenarioMatrix(UseCaseGraph useCaseGraph, FileInfo file)
        {
            //check if given file is corrupted
            ValidateFile(file);

            SpreadsheetDocument document = SpreadsheetDocument.Create(file.FullName, SpreadsheetDocumentType.Workbook);
            ScenarioMatrixExporter.CreateScenarioMatrix(useCaseGraph, document, 1);
            document.Close();
        }

        private static INode FindStartNode(IGraph graph)
        {
            return
                graph.Nodes.FirstOrDefault(
                    node =>
                        node.GetAttributeByName(NodeAttributes.NodeType.AttributeName())
                            .Value.Equals(UseCaseGraph.NodeTypeAttribute.StartNode));
        }

        /// <summary>
        /// Exports the scenario matrix of all use case graphs to the specfified file (.xlsx)
        /// Each Use Case is a new excel sheet 
        /// </summary>
        /// <param name="scenarios"></param>
        /// <param name="file">file info of the file to save the scenario matrix to</param>
        public static void ExportScenarioMatrix(IEnumerable<IGraph> scenarios, FileInfo file)
        {
            

            //check if given file is corrupted
            //ValidateFile(file);
            //
            //using (SpreadsheetDocument document = SpreadsheetDocument.Open(file.FullName, true))
            //{
            //    for (UInt32Value i = 0; i < useCaseGraphs.Length; i++)
            //    {
            //        var useCaseGraph = useCaseGraphs[i];
            //        ScenarioMatrixExporter.CreateScenarioMatrix(useCaseGraph, document, i);
            //    }
            //}
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
                throw new InvalidOperationException("Wrong file type, please specify a *" + ExcelExtension + ".");
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
            //Sheet sheet = new Sheet()
            //{
            //    Id = spreadsheetDoc.WorkbookPart.GetIdOfPart(worksheetPart),
            //    SheetId = index,
            //    Name = ucName
            //};
            //
            //sheets.Append(sheet);

            // TODO: Excel
            Worksheet ws = worksheetPart.Worksheet;
            WriteScenarios(useCaseGraph.Scenarios, ws);

            sheets.Append(ws);

            workbookPart.Workbook.Save();
        }

        private static void WriteScenarios(IEnumerable<IGraph> scenarios, Worksheet ws)
        {
            for(int scenario = 0; scenario < scenarios.Count(); scenario++)
            {
                var order = (string)scenarios.ElementAt(scenario).GetAttributeByName(COrder).Value;
                var knotenNamen = order.Split(new[] { ' ', '\n', '\r', '\t' }, StringSplitOptions.RemoveEmptyEntries);

                if (!knotenNamen.Any(knoten => knoten.Length > 1)) continue; // das scenario durchläuft keine abzweigungen

                int abzweigungenGefunden = 0;
                for (int i = 1; i < knotenNamen.Length; i++)
                {
                    if (knotenNamen[i].Length > knotenNamen[i - 1].Length)
                    {
                        abzweigungenGefunden++;

                        string adresse = ((char)('C' + scenario)).ToString() + (scenario + 1).ToString();
                        var cell = InsertCellInWorksheet(ws, adresse);

                        cell.CellValue = new CellValue(knotenNamen[i]);


                    }
                }
            }
        }

        private static Cell InsertCellInWorksheet(Worksheet ws, string addressName)
        {
            SheetData sheetData = ws.GetFirstChild<SheetData>();
            Cell cell = null;

            UInt32 rowNumber = GetRowIndex(addressName);
            Row row = GetRow(sheetData, rowNumber);

            // If the cell you need already exists, return it.
            // If there is not a cell with the specified column name, insert one.  
            Cell refCell = row.Elements<Cell>().
                Where(c => c.CellReference.Value == addressName).FirstOrDefault();
            if (refCell != null)
            {
                cell = refCell;
            }
            else
            {
                cell = CreateCell(row, addressName);
            }
            return cell;
        } // Add a cell with the specified address to a row.

        private static Cell CreateCell(Row row, String address)
        {
            Cell cellResult;
            Cell refCell = null;

            // Cells must be in sequential order according to CellReference. 
            // Determine where to insert the new cell.
            foreach (Cell cell in row.Elements<Cell>())
            {
                if (string.Compare(cell.CellReference.Value, address, true) > 0)
                {
                    refCell = cell;
                    break;
                }
            }

            cellResult = new Cell();
            cellResult.CellReference = address;

            row.InsertBefore(cellResult, refCell);
            return cellResult;
        }

        // Return the row at the specified rowIndex located within
        // the sheet data passed in via wsData. If the row does not
        // exist, create it.
        private static Row GetRow(SheetData wsData, UInt32 rowIndex)
        {
            var row = wsData.Elements<Row>().
            Where(r => r.RowIndex.Value == rowIndex).FirstOrDefault();
            if (row == null)
            {
                row = new Row();
                row.RowIndex = rowIndex;
                wsData.Append(row);
            }
            return row;
        }

        // Given an Excel address such as E5 or AB128, GetRowIndex
        // parses the address and returns the row index.
        private static UInt32 GetRowIndex(string address)
        {
            string rowPart;
            UInt32 l;
            UInt32 result = 0;

            for (int i = 0; i < address.Length; i++)
            {
                if (UInt32.TryParse(address.Substring(i, 1), out l))
                {
                    rowPart = address.Substring(i, address.Length - i);
                    if (UInt32.TryParse(rowPart, out l))
                    {
                        result = l;
                        break;
                    }
                }
            }
            return result;
        }
    }
}