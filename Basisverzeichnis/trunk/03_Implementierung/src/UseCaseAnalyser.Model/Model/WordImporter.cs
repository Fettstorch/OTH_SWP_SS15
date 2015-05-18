using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using GraphFramework;
using GraphFramework.Interfaces;
using Attribute = GraphFramework.Attribute;
using Table = DocumentFormat.OpenXml.Wordprocessing.Table;

namespace UseCaseAnalyser.Model.Model
{
    /// <summary>
    /// Imports use case graphs from a word document
    /// </summary>
    public static class WordImporter
    {

        private const string SequenceJump = "Rückkehr nach:";

        private static readonly List<string> ImportStepNames = new List<string>()
        {
            "Name",
            "Kennung",
            "Priorität",
            "Kurzbeschreibung:",
            "Vorbedingung(en):",
            "Nachbedingung(en):",
            "Normaler Ablauf:",
            "Ablauf-Varianten:",
            "Spezielle Anforderungen:",
            "Zu klärende Punkte:"
        };

        private enum ImportStep
        {
            Name = 0, 
            Id,  
            Priority, 
            Description, 
            PreCondition, 
            PostCondition, 
            NormalRoutine,
            SequenceVariation, 
            SpecialRequirements, 
            OpenPoints
        }

        public static readonly List<string> UseCaseNodeAttributeNames = new List<string>()
        {
            "Index",
            "Normal Index",
            "Variant Index",
            "Variant Sequnce Step",
            "Description",
            "NodeType"
        };

        public enum UseCaseNodeAttributes
        {
            Index = 0,
            NormalIndex,
            VariantIndex,
            VarSeqStep,
            Description,
            NodeType
        }

        public enum NodeTypeAttribute
        {
            StartNode, 
            JumpNode,
            NormalNode,
            VariantNode,
            EndNode
        }

        private static Report wordImporteReport;

        /// <summary>
        /// Imports all use cases that can be found in the file. 
        /// </summary>
        /// <param name="file">the word document (.docx)</param>
        /// <returns>list of the use case graphs generated from the file</returns>
        public static List<UseCaseGraph> ImportUseCases(FileInfo file)
        {
            Report temp;
            return WordImporter.ImportUseCases(file, out temp);
        }
        
        /// <summary>
        /// Imports all use cases that can be found in the file. It also generates a report für errors, warnings and log entries
        /// </summary>
        /// <param name="file">the word document (.docx)</param>
        /// <param name="report">the report</param>
        /// <returns>list of all use case graphs</returns>
        public static List<UseCaseGraph> ImportUseCases(FileInfo file, out Report report)
        {
            // Initialize
            List<UseCaseGraph> useCaseList = new List<UseCaseGraph>();
            WordImporter.wordImporteReport = report = new Report();

            // Check if the file exists
            if (!file.Exists)
            {
                WordImporter.wordImporteReport.AddReportEntry(new Report.ReportEntry("ERROR", "File not found!", Report.Entrytype.ERROR));
                return useCaseList;
            }
            
            // Try to open the document
            WordprocessingDocument doc;
            try
            {
                doc = WordprocessingDocument.Open(file.FullName, false);
            }
            catch (Exception ex)
            {
                WordImporter.wordImporteReport.AddReportEntry(new Report.ReportEntry("ERROR", ex.Message, Report.Entrytype.ERROR));
                return useCaseList;
            }

            // Fetch all tables in the document
            IEnumerable<Table> tables = doc.MainDocumentPart.Document.Descendants<Table>();
            int numberOfUseCases = 0;
            foreach (Table table in tables)
            {
                // every possible use case table
                IEnumerable<TableRow> rows = table.Descendants<TableRow>();
                UseCaseGraph useCaseGraph;
                if (WordImporter.TryReadInUseCase(rows, out useCaseGraph))
                {
                    useCaseList.Add(useCaseGraph);
                    numberOfUseCases++;
                }
            }

            if (numberOfUseCases == 0)
            {
                WordImporter.wordImporteReport.AddReportEntry(new Report.ReportEntry("WARNING", "No use cases found", Report.Entrytype.WARNING));
            }

            WordImporter.wordImporteReport.AddReportEntry(new Report.ReportEntry("LOG", numberOfUseCases + " Use Cases successfully imported!", Report.Entrytype.LOG));

            return useCaseList; 
        }

        /// <summary>
        /// Tries to read in a use case table
        /// </summary>
        /// <param name="tableRows"></param>
        /// <param name="useCaseGraph"></param>
        /// <returns>False when a error occurs</returns>
        private static bool TryReadInUseCase(IEnumerable<TableRow> tableRows, out UseCaseGraph useCaseGraph)
        {
            useCaseGraph = new UseCaseGraph();
            if (tableRows == null) return false;
            List<TableRow> rows = tableRows.ToList();
            if (rows.Count == 0 || rows[0] == null) return false;
            List<TableCell> cellsOfFirstRow = rows[0].Descendants<TableCell>().ToList();

            string useCaseName;
            if (cellsOfFirstRow.Count == 1)
            {
                // Get name field of the use case
                useCaseName = cellsOfFirstRow[0].InnerText;
                IAttribute nameAttribute = new Attribute(ImportStepNames[0], useCaseName);
                useCaseGraph.AddAttribute((nameAttribute));
            }
            else
            {
                // Use Case doesn't have a name
                return false;
            }

            string id, prio, desc, precon, postcon, spec, open;

            if (TryGetHorizontalContent(rows[1], out id, ImportStepNames[(int)ImportStep.Id]) &&
                TryGetHorizontalContent(rows[2], out prio, ImportStepNames[(int)ImportStep.Priority]) &&
                TryGetVerticalContent(rows, 3, out desc, ImportStepNames[(int)ImportStep.Description]) &&
                TryGetVerticalContent(rows, 5, out precon, ImportStepNames[(int)ImportStep.PreCondition]) &&
                TryGetVerticalContent(rows, 7, out postcon, ImportStepNames[(int)ImportStep.PostCondition]) &&
                TryGetVerticalContent(rows, rows.Count-4, out spec, ImportStepNames[(int)ImportStep.SpecialRequirements]) &&
                TryGetVerticalContent(rows, rows.Count-2, out open, ImportStepNames[(int)ImportStep.OpenPoints]))
            {
                IAttribute idAttribute = new Attribute(ImportStepNames[(int)ImportStep.Id], id);
                IAttribute prioAttribute = new Attribute(ImportStepNames[(int)ImportStep.Priority], prio);
                IAttribute descAttribute = new Attribute(ImportStepNames[(int)ImportStep.Description], desc);
                IAttribute preconAttribute = new Attribute(ImportStepNames[(int)ImportStep.PreCondition], precon);
                IAttribute posconAttribute = new Attribute(ImportStepNames[(int)ImportStep.PostCondition], postcon);
                IAttribute specAttribute = new Attribute(ImportStepNames[(int)ImportStep.SpecialRequirements], spec);
                IAttribute openAttribute = new Attribute(ImportStepNames[(int)ImportStep.OpenPoints], open);
                useCaseGraph.AddAttribute(idAttribute);
                useCaseGraph.AddAttribute(prioAttribute);
                useCaseGraph.AddAttribute(descAttribute);
                useCaseGraph.AddAttribute(preconAttribute);
                useCaseGraph.AddAttribute(posconAttribute);
                useCaseGraph.AddAttribute(specAttribute);
                useCaseGraph.AddAttribute(openAttribute);

                // Get normal routine and sequence variants
                if (TryGetNormalRoutineAndSeqVars(useCaseGraph, rows, 9)) return true;
                WordImporter.wordImporteReport.AddReportEntry(new Report.ReportEntry("ERROR", "Use Case '" + useCaseName + "': format is invalid", Report.Entrytype.ERROR, useCaseName));
                return false;
            }            

            return false;
        }

        /// <summary>
        /// Checks if the heading is correct, if the format is correct and returns the content string and true if successful or false if not
        /// </summary>
        /// <param name="row">row where the content is expected</param>
        /// <param name="result">the cell content</param>
        /// <param name="heading">the heading</param>
        /// <returns></returns>
        private static bool TryGetHorizontalContent(TableRow row, out string result, string heading)
        {
            result = null;
            List<TableCell> cells = row.Descendants<TableCell>().ToList();
            if (cells.Count != 2) return false;
            if (!string.Equals(cells[0].InnerText, heading)) return false;
            result = cells[1].InnerText;
            return true;
        }

        /// <summary>
        /// Returns true or false if success
        /// </summary>
        /// <param name="rows">List of the table rows of this use case</param>
        /// <param name="actRowIndex">index of the row the heading is located</param>
        /// <param name="result">the cell content</param>
        /// <param name="heading">the heading</param>
        /// <returns></returns>
        private static bool TryGetVerticalContent(IReadOnlyList<TableRow> rows, int actRowIndex, out string result, string heading)
        {
            result = null;
            if (rows == null) return false;
            if (actRowIndex < 0) return false;
            List<TableCell> cells = rows[actRowIndex].Descendants<TableCell>().ToList();
            if (cells.Count != 1 || actRowIndex + 1 >= rows.Count) return false;
            if (!string.Equals(cells[0].InnerText, heading)) return false;
            cells = rows[actRowIndex+1].Descendants<TableCell>().ToList();
            if (cells.Count != 1) return false;
            result = cells[0].InnerText;
            return true;
        }

        /// <summary>
        /// Tries to get the normal routine (Normaler Ablauf) of the use case
        /// </summary>
        /// <param name="useCaseGraph"></param>
        /// <param name="rows"></param>
        /// <param name="rowIndex"></param>
        /// <returns>true when the use case could be successfully parsed</returns>
        private static bool TryGetNormalRoutineAndSeqVars(UseCaseGraph useCaseGraph, IReadOnlyList<TableRow> rows, int rowIndex)
        {
            // Defensive programming
            if (rows == null) return false;
            if (rowIndex < 0) return false;
            if (useCaseGraph == null) return false;

            // Initializing
            Dictionary<string, INode> nodes = new Dictionary<string, INode>();            
            List<TableCell> cells = rows[rowIndex].Descendants<TableCell>().ToList();
            string variantIndex = " ", previousVariantIndex = "";
            Regex rgx = new Regex("[^0-9]");
            
            // Check the heading ("Normaler Ablauf")
            if (cells.Count != 1|| rowIndex + 1 >= rows.Count) return false;
            if (!string.Equals(cells[0].InnerText, ImportStepNames[(int)ImportStep.NormalRoutine])) 
                return false;

            // Try to get the normal use case routine
            cells = rows[rowIndex + 1].Descendants<TableCell>().ToList();
            if (cells.Count != 2) return false;
            List<Paragraph> paragraphList = cells[1].Descendants<Paragraph>().ToList();
            INode oldNode = null;
            if (paragraphList.Count < 1) return false; // no normal routine found
            for (int i = 0; i < paragraphList.Count - 1; i++) // last paragraph is the end tag: "Ende"
            {
                IAttribute nodeType;
                if (i == 0)
                {
                    nodeType = new Attribute(UseCaseNodeAttributeNames[(int)UseCaseNodeAttributes.NodeType], NodeTypeAttribute.StartNode);
                }
                else if (i == paragraphList.Count - 2)
                {
                    nodeType = new Attribute(UseCaseNodeAttributeNames[(int)UseCaseNodeAttributes.NodeType], NodeTypeAttribute.EndNode);
                }
                else
                {
                    nodeType = new Attribute(UseCaseNodeAttributeNames[(int)UseCaseNodeAttributes.NodeType], NodeTypeAttribute.NormalNode);
                }

                IAttribute indexAttribute = new Attribute(UseCaseNodeAttributeNames[(int)UseCaseNodeAttributes.Index], (i + 1).ToString());
                IAttribute descAttribute = new Attribute(UseCaseNodeAttributeNames[(int)UseCaseNodeAttributes.Description], paragraphList[i].InnerText);                
                INode node = new Node(indexAttribute, descAttribute, nodeType);
                useCaseGraph.AddNode(node);
                nodes.Add(indexAttribute.Value.ToString(), node);
                if (oldNode != null)
                    useCaseGraph.AddEdge(oldNode, node);
                oldNode = node;
            }

            // Try to get the sequence variants
            rowIndex += 2;
            cells = rows[rowIndex].Descendants<TableCell>().ToList();

            if (cells.Count != 1 || rowIndex + 1 >= rows.Count) return false;
            if (!string.Equals(cells[0].InnerText, WordImporter.ImportStepNames[(int)ImportStep.SequenceVariation]))
                return false; // invalid format

            string varDesc = "";
            for (int i = rowIndex + 1; i < rows.Count - 4; i++)
            {
                cells = rows[i].Descendants<TableCell>().ToList();
                if (cells.Count != 2) return false; // invalid format
                if (!cells[0].InnerText.Equals(""))
                {
                    variantIndex = cells[0].InnerText; 
                    previousVariantIndex = "";
                    varDesc = cells[1].InnerText;
                }
                else
                {
                    paragraphList = cells[1].Descendants<Paragraph>().ToList();
                    if (paragraphList.Count == 0) continue; // Sequence variant is invalid, because it has no nodes
                    
                    Dictionary<string, INode> sequenceVarNodes = new Dictionary<string, INode>();
                    string lastCondition = paragraphList[paragraphList.Count - 1].InnerText;
                    // Get all sequence nodes:
                    for (int j = 0; j < paragraphList.Count - 1; j++)
                    {
                        IAttribute indexAttribute = new Attribute(UseCaseNodeAttributeNames[(int)UseCaseNodeAttributes.Index], 
                            variantIndex + (j + 1)); // obsulete
                        IAttribute descAttribute = new Attribute(UseCaseNodeAttributeNames[(int)UseCaseNodeAttributes.Description], 
                            paragraphList[j].InnerText);
                        IAttribute normIndexAttribute = new Attribute(UseCaseNodeAttributeNames[(int) UseCaseNodeAttributes.NormalIndex], 
                            rgx.Replace(variantIndex, ""));
                        IAttribute varIndexAttribute = new Attribute(UseCaseNodeAttributeNames[(int) UseCaseNodeAttributes.VariantIndex], 
                            variantIndex[variantIndex.Length-1]);
                        IAttribute varStepAttribute = new Attribute(UseCaseNodeAttributeNames[(int)UseCaseNodeAttributes.VarSeqStep],
                                    (j+1).ToString());

                        INode node = new Node(indexAttribute, descAttribute, normIndexAttribute, varIndexAttribute, varStepAttribute);
                        if (j < paragraphList.Count - 2)
                        {
                            node.AddAttribute(new Attribute(UseCaseNodeAttributeNames[(int)UseCaseNodeAttributes.NodeType], 
                                NodeTypeAttribute.VariantNode));
                        }
                        useCaseGraph.AddNode(node);

                        string indexNumber = rgx.Replace(variantIndex, "");

                        if (previousVariantIndex.Equals(""))
                            previousVariantIndex = indexNumber;

                        INode searchedNode;
                        if (nodes.TryGetValue(previousVariantIndex, out searchedNode))
                        {                            
                            if (j == 0)
                            {
                                useCaseGraph.AddEdge(searchedNode, node, 
                                    new Attribute(UseCaseNodeAttributeNames[(int)UseCaseNodeAttributes.Description], varDesc));    
                            }
                            else
                                useCaseGraph.AddEdge(searchedNode, node);
                        }
                        if (sequenceVarNodes.TryGetValue(previousVariantIndex, out searchedNode))
                            useCaseGraph.AddEdge(searchedNode, node);
                        previousVariantIndex = variantIndex + (j + 1);
                        sequenceVarNodes.Add(previousVariantIndex, node);
                    }

                    if (lastCondition.StartsWith(WordImporter.SequenceJump))
                    {
                        string lastConditionId = lastCondition.Replace(WordImporter.SequenceJump, "").Replace(" ", "");
                        INode indexNode;
                        if (!nodes.TryGetValue(lastConditionId, out indexNode)) continue;
                        INode lastNode;
                        if (sequenceVarNodes.TryGetValue(previousVariantIndex, out lastNode))
                        {
                            // make a edge between the normal routine node and the first node of the sequence variant
                            useCaseGraph.AddEdge(lastNode, indexNode);
                            lastNode.AddAttribute(new Attribute(UseCaseNodeAttributeNames[(int)UseCaseNodeAttributes.NodeType], NodeTypeAttribute.JumpNode));
                        }
                    }
                    else if (lastCondition.StartsWith("Ende."))
                    {
                        INode lastNode;
                        if (sequenceVarNodes.TryGetValue(previousVariantIndex, out lastNode))
                        {
                            lastNode.AddAttribute(new Attribute(UseCaseNodeAttributeNames[(int)UseCaseNodeAttributes.NodeType], NodeTypeAttribute.EndNode));
                        }
                    }
                                       
                }
            }

            return true;
        }       
    }
}
