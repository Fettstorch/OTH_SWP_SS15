using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using LogManager;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using GraphFramework;
using GraphFramework.Interfaces;
using Table = DocumentFormat.OpenXml.Wordprocessing.Table;

namespace UseCaseAnalyser.Model.Model
{
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
        };

        /// <summary>
        /// imports the specified file (.docx) to a use case graph
        /// </summary>
        /// <param name="file">file to import</param>
        /// <returns>list of the use case graphs generated from the file</returns>
        public static List<UseCaseGraph> ImportUseCases(FileInfo file)
        {
            List<UseCaseGraph> useCaseList = new List<UseCaseGraph>();

            if (!File.Exists((file.Name)))
            {
                // To-Do: Loggers
                // LoggingFunctions.Debug("File does not exist!"); geht net... weil internal ach ka.. 
                return useCaseList;
            }
            
            WordprocessingDocument doc;
            try
            {
                doc = WordprocessingDocument.Open(file.Name, false);
            }
            catch (Exception ex)
            {
                // To-Do: Logger
                MessageBox.Show(ex.Message);
                return useCaseList;
            }

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
                // Es konnten keine Use Cases gefunden werden. To-Do: Warnung, Logger, etc!
            }

            return useCaseList; 
        }

        private static bool TryReadInUseCase(IEnumerable<TableRow> tableRows, out UseCaseGraph useCaseGraph)
        {
            useCaseGraph = new UseCaseGraph();
            if (tableRows == null) return false;
            List<TableRow> rows = tableRows.ToList();
            if (rows.Count == 0 || rows[0] == null) return false;
            List<TableCell> cellsOfFirstRow = rows[0].Descendants<TableCell>().ToList();

            if (cellsOfFirstRow.Count == 1)
            {
                // Get name field of the use case
                IAttribute nameAttribute = new GraphFramework.Attribute(ImportStepNames[0], cellsOfFirstRow[0].InnerText);
                useCaseGraph.AddAttribute((nameAttribute));
            }
            else
            {
                // Use Case doesn't have a name
                return false;
            }

            string id, prio = null, desc = null, precon = null, postcon = null, spec = null, open = null;

            if (TryGetHorizontalContent(rows[1], out id, ImportStepNames[(int)ImportStep.Id]) &&
                TryGetHorizontalContent(rows[2], out prio, ImportStepNames[(int)ImportStep.Priority]) &&
                TryGetVerticalContent(rows, 3, out desc, ImportStepNames[(int)ImportStep.Description]) &&
                TryGetVerticalContent(rows, 5, out precon, ImportStepNames[(int)ImportStep.PreCondition]) &&
                TryGetVerticalContent(rows, 7, out postcon, ImportStepNames[(int)ImportStep.PostCondition]) &&
                TryGetVerticalContent(rows, rows.Count-4, out spec, ImportStepNames[(int)ImportStep.SpecialRequirements]) &&
                TryGetVerticalContent(rows, rows.Count-2, out open, ImportStepNames[(int)ImportStep.OpenPoints]))
            {
                IAttribute idAttribute = new GraphFramework.Attribute(ImportStepNames[(int)ImportStep.Id], id);
                IAttribute prioAttribute = new GraphFramework.Attribute(ImportStepNames[(int)ImportStep.Priority], prio);
                IAttribute descAttribute = new GraphFramework.Attribute(ImportStepNames[(int)ImportStep.Description], desc);
                IAttribute preconAttribute = new GraphFramework.Attribute(ImportStepNames[(int)ImportStep.PreCondition], precon);
                IAttribute posconAttribute = new GraphFramework.Attribute(ImportStepNames[(int)ImportStep.PostCondition], postcon);
                IAttribute specAttribute = new GraphFramework.Attribute(ImportStepNames[(int)ImportStep.SpecialRequirements], spec);
                IAttribute openAttribute = new GraphFramework.Attribute(ImportStepNames[(int)ImportStep.OpenPoints], open);
                useCaseGraph.AddAttribute(idAttribute);
                useCaseGraph.AddAttribute(prioAttribute);
                useCaseGraph.AddAttribute(descAttribute);
                useCaseGraph.AddAttribute(preconAttribute);
                useCaseGraph.AddAttribute(posconAttribute);
                useCaseGraph.AddAttribute(specAttribute);
                useCaseGraph.AddAttribute(openAttribute);

                // Get normal routine and sequence variants
                Dictionary<string, INode> nodes;
                if (TryGetNormalRoutine(useCaseGraph, rows, 9, ImportStepNames[(int) ImportStep.NormalRoutine], out nodes) &&
                    TryGetSequenceVariants(useCaseGraph, rows, 11, WordImporter.ImportStepNames[(int)WordImporter.ImportStep.SequenceVariation], ref nodes))
                    return true;

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
            result = cells[0].InnerText;
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
        private static bool TryGetVerticalContent(List<TableRow> rows, int actRowIndex, out string result, string heading)
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

        private static bool TryGetNormalRoutine(UseCaseGraph useCaseGraph, List<TableRow> rows, int actRowIndex,
            string heading)
        {
            Dictionary<string, INode> temp;
            return WordImporter.TryGetNormalRoutine(useCaseGraph, rows, actRowIndex, heading, out temp);
        }

        private static bool TryGetNormalRoutine(UseCaseGraph useCaseGraph, List<TableRow> rows, int actRowIndex,
            string heading, out Dictionary<string, INode> nodes)
        {
            nodes = new Dictionary<string, INode>();
            if (rows == null) return false;
            if (actRowIndex < 0) return false;
            List<TableCell> cells = rows[actRowIndex].Descendants<TableCell>().ToList();
            if (useCaseGraph == null) return false;
            if (cells.Count != 1|| actRowIndex + 1 >= rows.Count) return false;
            if (!string.Equals(cells[0].InnerText, heading)) return false;
            cells = rows[actRowIndex + 1].Descendants<TableCell>().ToList();
            if (cells.Count != 2) return false;
            List<Paragraph> paragraphList = cells[1].Descendants<Paragraph>().ToList();
            INode oldNode = null;
            for (int i = 0; i < paragraphList.Count - 1; i++)
            {
                IAttribute indexAttribute = new GraphFramework.Attribute("index", (i + 1).ToString());
                IAttribute descAttribute = new GraphFramework.Attribute("description", paragraphList[i].InnerText);
                INode node = new Node(indexAttribute, descAttribute);
                useCaseGraph.AddNode(node);
                nodes.Add(indexAttribute.Value.ToString(), node);
                if (oldNode != null)
                    useCaseGraph.AddEdge(oldNode, node);
                oldNode = node;
            }

            return true;
        }

        private static bool TryGetSequenceVariants(UseCaseGraph useCaseGraph, List<TableRow> rows, int actRowIndex,
            string heading, ref Dictionary<string, INode> nodes)
        {
            if (rows == null) return false;
            if (actRowIndex < 0) return false;
            List<TableCell> cells = rows[actRowIndex].Descendants<TableCell>().ToList();
            if (useCaseGraph == null) return false;
            if (cells.Count != 1 || actRowIndex + 1 >= rows.Count) return false;
            if (!string.Equals(cells[0].InnerText, heading)) return false;

            string variantIndex = "", previousVariantIndex = "", variantName="";
            Regex rgx = new Regex("[^0-9]");

            for (int i = actRowIndex+1; i < rows.Count - 4; i++)
            {
                cells = rows[i].Descendants<TableCell>().ToList();
                if (cells.Count != 2) return false;
                if (!cells[0].InnerText.Equals(""))
                {
                    variantIndex = cells[0].InnerText;
                    previousVariantIndex = "";
                    if (cells.Count > 1) variantName = cells[1].InnerText;
                }
                else
                {
                    List<Paragraph> paragraphList = cells[1].Descendants<Paragraph>().ToList();
                    if(paragraphList.Count == 0) continue;

                    string lastCondition = paragraphList[paragraphList.Count - 1].InnerText;
                    for (int j = 0; j < paragraphList.Count - 1; j++)
                    {
                        IAttribute indexAttribute = new GraphFramework.Attribute("index", variantIndex + j + 1);
                        IAttribute descAttribute = new GraphFramework.Attribute("description", paragraphList[j].InnerText);
                        INode node = new Node(indexAttribute, descAttribute);
                        useCaseGraph.AddNode(node);
                        
                        string indexNumber = rgx.Replace(variantIndex, "");

                        if (previousVariantIndex.Equals(""))
                            previousVariantIndex = indexNumber;
                        /*
                        INode searchedNode = useCaseGraph.Nodes.ToList().Find(
                            x => string.Equals(x.Attributes.ToList().Find(
                                y => string.Equals(y.Name, "index")).Value, previousVariantIndex));
                        // lol? .. :D
                        */
                        INode searchedNode;

                        if (nodes.TryGetValue(previousVariantIndex, out searchedNode))
                            useCaseGraph.AddEdge(searchedNode, node);                            
                        previousVariantIndex = variantIndex + (j + 1);
                    }

                    if (lastCondition.StartsWith(WordImporter.SequenceJump))
                    {
                        string lastConditionId = lastCondition.Replace(WordImporter.SequenceJump, "").Replace(" ", "");
                        /*if (useCaseGraph.Nodes.ToList().Exists(x => string.Equals(x.Attributes.ToList().Find(
                                y => string.Equals(y.Name, "index")).Value, lastConditionId))) */
                        INode searchedNode;
                        if(nodes.TryGetValue(lastConditionId, out searchedNode))
                        {
                            /*
                            Knoten searchKnoten = ergGraph.KnotenList.Find(knoten => knoten.Index == lastConditionId2);
                            Knoten lastKnoten = ergGraph.KnotenList.Find(knoten => knoten.Index == previousVariantIndex);
                            if (searchKnoten != null && lastKnoten != null)
                                ergGraph.KantenList.Add(new Kanten(lastKnoten, searchKnoten));*/
                            INode lastNode;
                            if (nodes.TryGetValue(previousVariantIndex, out lastNode))
                            {
                                useCaseGraph.AddEdge(lastNode, searchedNode);
                            }
                        }
                    }

                }
            }

            return true;
        }
    }
}
