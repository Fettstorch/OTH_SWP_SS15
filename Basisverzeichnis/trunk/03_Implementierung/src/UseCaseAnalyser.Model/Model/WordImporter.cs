#region Copyright information
// <summary>
// <copyright file="WordImporter.cs">Copyright (c) 2015</copyright>
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
using System.IO.Compression;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
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

        #region EXPRESSIONS
        /// <summary>
        /// The expression which initiates a jump to another use case
        /// ToDo: Use this expression for use case jumps
        /// </summary>
        private const string UseCaseJump = "Weiter mit:";

        /// <summary>
        /// The expression which initiates a jump from the sequence variant to the normal routine
        /// </summary>
        private const string SequenceJump = "Rückkehr nach:";

        /// <summary>
        /// The expression which defines the end of the use case
        /// </summary>
        private const string UseCaseEnd = "Ende.";

        /// <summary>
        /// The expression which is used as the description for the last node, the end node
        /// </summary>
        private const string EndDescription = "Der Use Case endet.";

        #endregion

        /// <summary>
        /// The report of the actual import process
        /// </summary>
        private static Report wordImporterReport;

        /// <summary>
        /// The use case ID of the actual use case
        /// </summary>
        private static string actUseCaseId;

        /// <summary>
        /// List of all use case graphs in the actual import process
        /// </summary>
        private static List<UseCaseGraph> actUseCases;

        /// <summary>
        /// Imports all use cases that can be found in the file. 
        /// </summary>
        /// <param name="file">the word document (.docx)</param>
        /// <returns>list of the use case graphs generated from the file</returns>
        public static List<UseCaseGraph> ImportUseCases(FileInfo file)
        {
            Report temp;
            return ImportUseCases(file, out temp);
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
            actUseCases = new List<UseCaseGraph>();
            wordImporterReport = report = new Report();

            // Check if the file exists
            if (!file.Exists)
            {
                throw new Exception("File not found!");
            }

            // Try to open the document
            WordprocessingDocument doc;
            try
            {
                doc = FixedOpen(file.FullName, false);
            }
            catch (Exception)
            {
                throw new Exception("File could not be open. Either the file is corrupt, or opened in another process!");
            }
            // Fetch all tables in the document
            IEnumerable<Table> tables = doc.MainDocumentPart.Document.Descendants<Table>();

            doc.Close();
            doc.Dispose();

            int numberOfUseCases = 0;
            foreach (Table table in tables)
            {
                // every possible use case table
                IEnumerable<TableRow> rows = table.Descendants<TableRow>();
                UseCaseGraph useCaseGraph;
                // Try interpret table 
                if (TryReadInUseCase(rows, out useCaseGraph))
                {
                    // Valid Use Case found:
                    actUseCases.Add(useCaseGraph);
                    wordImporterReport.AddReportEntry(new Report.ReportEntry("Success!",
                        "Use Case " + actUseCaseId + " successfully imported!", Report.Entrytype.LOG, actUseCaseId));
                    numberOfUseCases++;
                }
            }

            if (numberOfUseCases == 0)
            {
                // No Use Cases were imported
                wordImporterReport.AddReportEntry(new Report.ReportEntry("Warning!",
                    "No use cases found", Report.Entrytype.WARNING));
            }

            return actUseCases;
        }

        /// <summary>
        /// Tries to get the name of the use case which is alway in the first cell in the first row in the use case table
        /// </summary>
        /// <param name="row">the first row of the table</param>
        /// <param name="useCaseGraph">the actual use case graph</param>
        /// <returns>true if success, false if not</returns>
        private static bool TryGetUseCaseName(TableRow row, UseCaseGraph useCaseGraph)
        {
            // Get name field of the use case:
            List<TableCell> cellsOfFirstRow = row.Descendants<TableCell>().ToList();
            if (cellsOfFirstRow.Count == 1)
            {
                string useCaseName = cellsOfFirstRow[0].InnerText;
                if (string.IsNullOrEmpty(useCaseName))
                {
                    wordImporterReport.AddReportEntry(new Report.ReportEntry(actUseCaseId, "Use Case doesn't have a name",
                        Report.Entrytype.ERROR));
                    return false;
                }

                IAttribute nameAttribute = new Attribute(UseCaseAttributes.Name.AttributeName(), useCaseName);
                useCaseGraph.AddAttribute((nameAttribute));
                if (actUseCases.Exists(x => string.Equals(x.GetAttributeByName(
                    UseCaseAttributes.Name.AttributeName()).Value, useCaseName)))
                {
                    wordImporterReport.AddReportEntry(new Report.ReportEntry(actUseCaseId, string.Format("Use Case name \"{0}\" occured more than once!", useCaseName),
                        Report.Entrytype.ERROR));
                    return false;
                }

                return true;
            }

            wordImporterReport.AddReportEntry(new Report.ReportEntry(actUseCaseId, "Use Case doesn't have a name",
                Report.Entrytype.ERROR));
            return false;
        }

        /// <summary>
        /// Tries to interpret the given table as a use case
        /// </summary>
        /// <param name="tableRows"></param>
        /// <param name="useCaseGraph"></param>
        /// <returns>False when a error occurs</returns>
        private static bool TryReadInUseCase(IEnumerable<TableRow> tableRows, out UseCaseGraph useCaseGraph)
        {
            // Initialize
            useCaseGraph = new UseCaseGraph();
            if (tableRows == null) return false;
            List<TableRow> rows = tableRows.ToList(); // Table is empty
            if (rows.Count == 0 || rows[0] == null) return false;
            if (rows.Count < 16) return false; // Not a use case but a table -> stop importing, but no error

            if (!TryGetUseCaseId(rows[1], out actUseCaseId))
            {
                // Table is not a use case table -> stop importing, but no error                
                return false;
            }

            string prio, desc, precon, postcon, spec, open;

            if (TryGetHorizontalContent(rows[2], out prio, UseCaseAttributes.Priority.AttributeName()) &&
                TryGetVerticalContent(rows, 3, out desc, UseCaseAttributes.Description.AttributeName()) &&
                TryGetVerticalContent(rows, 5, out precon, UseCaseAttributes.PreCondition.AttributeName()) &&
                TryGetVerticalContent(rows, 7, out postcon, UseCaseAttributes.PostCondition.AttributeName()) &&
                TryGetVerticalContent(rows, rows.Count - 4, out spec, UseCaseAttributes.SpecialRequirements.AttributeName()) &&
                TryGetVerticalContent(rows, rows.Count - 2, out open, UseCaseAttributes.OpenPoints.AttributeName()) &&
                TryGetUseCaseName(rows[0], useCaseGraph))
            {
                IAttribute idAttribute = new Attribute(UseCaseAttributes.Id.AttributeName(), actUseCaseId);
                IAttribute prioAttribute = new Attribute(UseCaseAttributes.Priority.AttributeName(), prio);
                IAttribute descAttribute = new Attribute(UseCaseAttributes.Description.AttributeName(), desc);
                IAttribute preconAttribute = new Attribute(UseCaseAttributes.PreCondition.AttributeName(), precon);
                IAttribute posconAttribute = new Attribute(UseCaseAttributes.PostCondition.AttributeName(), postcon);
                IAttribute specAttribute = new Attribute(UseCaseAttributes.SpecialRequirements.AttributeName(), spec);
                IAttribute openAttribute = new Attribute(UseCaseAttributes.OpenPoints.AttributeName(), open);
                useCaseGraph.AddAttribute(idAttribute);
                useCaseGraph.AddAttribute(prioAttribute);
                useCaseGraph.AddAttribute(descAttribute);
                useCaseGraph.AddAttribute(preconAttribute);
                useCaseGraph.AddAttribute(posconAttribute);
                useCaseGraph.AddAttribute(specAttribute);
                useCaseGraph.AddAttribute(openAttribute);

                // Get normal routine and sequence variants. Inline because of performance reasons
                if (TryGetNormalRoutineAndSeqVars(useCaseGraph, rows, 9)) return true;
            }

            return false;
        }

        /// <summary>
        /// Tries to get the use case id from the given row and checks if the id doesn't occurs twice
        /// </summary>
        /// <param name="row">the row with the id cell</param>
        /// <param name="id">the use case id</param>
        /// <returns>true if success</returns>
        private static bool TryGetUseCaseId(TableRow row, out string id)
        {
            id = null;
            List<TableCell> cells = row.Descendants<TableCell>().ToList();
            if (cells.Count == 2)
            {
                if (string.Equals(cells[0].InnerText, UseCaseAttributes.Id.AttributeName()))
                {                    
                    if (actUseCases.Exists(x => string.Equals(x.GetAttributeByName(
                        UseCaseAttributes.Id.AttributeName()).Value, cells[1].InnerText)))
                    {
                        wordImporterReport.AddReportEntry(new Report.ReportEntry(actUseCaseId, string.Format("Use Case id \"{0}\" occured more than once!", cells[1].InnerText),
                            Report.Entrytype.ERROR));
                        return false;
                    }

                    id = cells[1].InnerText;
                    return true;
                }
            }

            string cellContent = cells.Count > 0 ? cells[0].InnerText : "(no content!)";

            wordImporterReport.AddReportEntry(new Report.ReportEntry("unknown id", "Table was not detected as a use case! Content of cell is: "
                    + cellContent, Report.Entrytype.WARNING));
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
            if (cells.Count == 2)
            {
                if (string.Equals(cells[0].InnerText, heading))
                {
                    result = cells[1].InnerText;
                    return true;
                }
            }

            wordImporterReport.AddReportEntry(new Report.ReportEntry(actUseCaseId, "Could not read '" + heading + "'!",
                Report.Entrytype.WARNING));
            return false;
        }

        /// <summary>
        /// This function tries to fetch the content from a use case table, that is declared underneeth each other,
        /// e.g. "Kurzbeschreibung", "Vorbedingung", etc.
        /// </summary>
        /// <param name="rows">List of the table rows of this use case</param>
        /// <param name="actRowIndex">index of the row the heading is located</param>
        /// <param name="result">the cell content</param>
        /// <param name="heading">the heading</param>
        /// <returns></returns>
        private static bool TryGetVerticalContent(IReadOnlyList<TableRow> rows, int actRowIndex, out string result, string heading)
        {
            result = null;
            if (rows == null) throw new NullReferenceException("rows");
            if (actRowIndex < 0) throw new InvalidOperationException("actRowIndex < 0!!");
            // Get content in the first row
            List<TableCell> cells = rows[actRowIndex].Descendants<TableCell>().ToList();
            if (cells.Count == 1 && ++actRowIndex < rows.Count)
            {
                if (string.Equals(cells[0].InnerText, heading))
                {
                    // Get content in the second row
                    cells = rows[actRowIndex].Descendants<TableCell>().ToList();
                    if (cells.Count == 1)
                    {
                        result = cells[0].InnerText;
                        return true;
                    }
                }
            }

            wordImporterReport.AddReportEntry(new Report.ReportEntry(actUseCaseId,
                "the content of '" + heading + "' could not be interpreted", Report.Entrytype.ERROR));
            return false;
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
            if (rows == null) throw new NullReferenceException("rows");
            if (rowIndex < 0) throw new InvalidOperationException("rowIndex < 0 !!");
            if (useCaseGraph == null) throw new NullReferenceException("useCaseGraph");

            // Initializing
            Dictionary<string, INode> nodes = new Dictionary<string, INode>();
            List<TableCell> cells = rows[rowIndex].Descendants<TableCell>().ToList();
            string variantIndex = " ", previousVariantIndex = "";
            Regex rgx = new Regex("[^0-9]");

            // Check the heading ("Normaler Ablauf")
            if (cells.Count != 1 || rowIndex + 1 >= rows.Count) return false;
            if (!string.Equals(cells[0].InnerText, UseCaseAttributes.NormalRoutine.AttributeName()))
                return false;

            // Try to get the normal use case routine
            cells = rows[rowIndex + 1].Descendants<TableCell>().ToList();
            if (cells.Count != 2) return false;
            List<Paragraph> paragraphList = cells[1].Descendants<Paragraph>().ToList();
            INode oldNode = null;
            if (paragraphList.Count < 1) return false; // no normal routine found
            IAttribute nodeType, normalIndex, descAttribute;
            for (int i = 0; i < paragraphList.Count; i++)
            {                
                if (i == 0)
                {
                    // Type = Start Node
                    nodeType = new HiddenAttribute(NodeAttributes.NodeType.AttributeName(),
                        UseCaseGraph.NodeTypeAttribute.StartNode);
                }
                else
                {
                    // Type = Normal Node
                    nodeType = new HiddenAttribute(NodeAttributes.NodeType.AttributeName(),
                        UseCaseGraph.NodeTypeAttribute.NormalNode);
                }

                normalIndex = new Attribute(NodeAttributes.NormalIndex.AttributeName(),
                    (i + 1).ToString());
                descAttribute = new Attribute(NodeAttributes.Description.AttributeName(),
                    paragraphList[i].InnerText);
                // Create the node:
                INode node = new Node(normalIndex, descAttribute, nodeType);
                useCaseGraph.AddNode(node);
                nodes.Add(normalIndex.Value.ToString(), node);
                // Create edge
                if (oldNode != null) useCaseGraph.AddEdge(oldNode, node);
                oldNode = node;
            }

            // Add Endnode:
            INode endNode = new Node(new HiddenAttribute(NodeAttributes.NormalIndex.AttributeName(),
                    (paragraphList.Count+1).ToString()), new Attribute(NodeAttributes.Description.AttributeName(),
                EndDescription), new HiddenAttribute(NodeAttributes.NodeType.AttributeName(),
                UseCaseGraph.NodeTypeAttribute.EndNode));
            useCaseGraph.AddNode(endNode);
            useCaseGraph.AddEdge(oldNode, endNode);

            // Try to get the sequence variants
            rowIndex += 2;
            cells = rows[rowIndex].Descendants<TableCell>().ToList();

            if (cells.Count != 1 || rowIndex + 1 >= rows.Count) return false;
            if (!string.Equals(cells[0].InnerText, UseCaseAttributes.SequenceVariation.AttributeName()))
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
                        descAttribute = new Attribute(NodeAttributes.Description.AttributeName(),
                            paragraphList[j].InnerText);
                        IAttribute normIndexAttribute = new Attribute(NodeAttributes.NormalIndex.AttributeName(),
                            rgx.Replace(variantIndex, ""));
                        IAttribute varIndexAttribute = new Attribute(NodeAttributes.VariantIndex.AttributeName(),
                            variantIndex[variantIndex.Length - 1]);
                        IAttribute varStepAttribute = new Attribute(NodeAttributes.VarSeqStep.AttributeName(),
                                    (j + 1).ToString());

                        INode node = new Node(descAttribute, normIndexAttribute, varIndexAttribute, varStepAttribute);
                        if (j < paragraphList.Count - 2)
                        {
                            node.AddAttribute(new HiddenAttribute(NodeAttributes.NodeType.AttributeName(),
                                UseCaseGraph.NodeTypeAttribute.VariantNode));
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
                                    new Attribute(NodeAttributes.Description.AttributeName(), varDesc));
                            }
                            else
                                useCaseGraph.AddEdge(searchedNode, node);
                        }
                        if (sequenceVarNodes.TryGetValue(previousVariantIndex, out searchedNode))
                            useCaseGraph.AddEdge(searchedNode, node);
                        previousVariantIndex = variantIndex + (j + 1);
                        sequenceVarNodes.Add(previousVariantIndex, node);
                    }

                    // Sequence Jump Node
                    if (lastCondition.StartsWith(SequenceJump))
                    {
                        string lastConditionId = lastCondition.Replace(SequenceJump, "").Replace(" ", "");
                        INode indexNode;
                        if (!nodes.TryGetValue(lastConditionId, out indexNode))
                        {
                            // Node refers to a node not included in the graph
                            wordImporterReport.AddReportEntry(new Report.ReportEntry(actUseCaseId, "Wrong reference! '" + lastCondition 
                                + "' leads to a node which is not in the graph! " + "If you wanted the node to refer to an use case, write '"
                                + UseCaseJump + "' instead!", Report.Entrytype.WARNING));
                            INode node;
                            if (!sequenceVarNodes.TryGetValue(previousVariantIndex, out node)) continue;
                            node.AddAttribute(new HiddenAttribute(NodeAttributes.NodeType.AttributeName(),
                                UseCaseGraph.NodeTypeAttribute.JumpNode));
                            continue;
                        }

                        INode lastNode;
                        if (!sequenceVarNodes.TryGetValue(previousVariantIndex, out lastNode))
                        {
                            // Should never happen!
                            continue;
                        }
                        // make a edge between the normal routine node and the first node of the sequence variant
                        useCaseGraph.AddEdge(lastNode, indexNode);
                        lastNode.AddAttribute(new HiddenAttribute(NodeAttributes.NodeType.AttributeName(),
                            UseCaseGraph.NodeTypeAttribute.JumpNode));
                    }
                    else if (lastCondition.StartsWith(UseCaseJump))
                    {
                        INode lastNode;
                        if (!sequenceVarNodes.TryGetValue(previousVariantIndex, out lastNode)) continue;
                        lastNode.AddAttribute(new HiddenAttribute(NodeAttributes.NodeType.AttributeName(),
                            UseCaseGraph.NodeTypeAttribute.JumpNode));
                        lastNode.GetAttributeByName(NodeAttributes.Description.AttributeName()).Value = 
                            string.Concat((lastNode.GetAttributeByName(NodeAttributes.Description.AttributeName()).Value as string),
                            "\n" + lastCondition);
                        //lastNode.AddAttribute(new Attribute(NodeAttributes.Description.AttributeName(),
                        //  lastCondition));
                    }
                    // End Node
                    else if (lastCondition.StartsWith(UseCaseEnd))
                    {
                        INode lastNode;
                        if (!sequenceVarNodes.TryGetValue(previousVariantIndex, out lastNode)) continue;
                        lastNode.AddAttribute(new HiddenAttribute(NodeAttributes.NodeType.AttributeName(),
                            UseCaseGraph.NodeTypeAttribute.EndNode));
                    }

                }
            }

            return true;
        }

        /// <summary>
        /// Workaround for OpenXML bug regarding Invalid hyperlinks exception (thrown by System.IO.Packaging)
        /// source: http://openxmldeveloper.org/blog/b/openxmldeveloper/archive/2014/08/19/handling-invalid-hyperlinks-openxmlpackageexception-in-the-open-xml-sdk.aspx
        /// Check after first try of opening whether Invalid hyperlink exception was thrown. If it was thrown, create a copy of the document and remove invalid hyperlinks.
        /// Afterwards try to read content of fixed document. If there are still opening errors these exceptions will be forwarded otherwise a WordprocessingDocument reference
        /// will be returned.
        /// </summary>
        /// <param name="docPath">Path of docuemnt which should be opened.</param>
        /// <param name="isEditable">Flag for enable editing while open the file.</param>
        /// <returns>Reference of opened WordprocessingDocument.</returns>
        private static WordprocessingDocument FixedOpen(string docPath, bool isEditable)
        {
            WordprocessingDocument wDoc;
            try
            {
                wDoc = WordprocessingDocument.Open(docPath, isEditable);
            }
            catch (Exception e)
            {
                //try to fix
                if (!e.ToString().Contains("Invalid Hyperlink"))
                {
                    throw;
                }

                //create copy of corrupted doc file in temp directory
                //ToDo decide where to create copy
                String newDocPath = Path.GetTempPath() + Path.GetFileNameWithoutExtension(docPath) + ".docx";
                FileInfo newDocFileInfo = new FileInfo(newDocPath);
                if (newDocFileInfo.Exists)
                    newDocFileInfo.Delete();
                File.Copy(docPath, newDocPath);

                using (FileStream fs = new FileStream(newDocPath, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                {
                    FixInvalidUri(fs, FixUri);
                }

                //try it again
                wDoc = WordprocessingDocument.Open(newDocPath, isEditable);
            }

            return wDoc;
        }

        /// <summary>
        /// Handler method for dealing with broken URIs.
        /// </summary>
        /// <returns>Returns a valid (dummy) URI.</returns>
        private static Uri FixUri()
        {
            return new Uri("http://broken-link/");
        }

        /// <summary>
        /// Replaces all invalid URIs from a stream by using invalidUriHandler and writes it back into the stream.
        /// </summary>
        /// <param name="fs">Stream that should be checked and fixed regarding invalid URIs.</param>
        /// <param name="invalidUriHandler">Function that should be called if URI is invalid.</param>
        private static void FixInvalidUri(Stream fs, Func<Uri> invalidUriHandler)
        {
            XNamespace relNs = "http://schemas.openxmlformats.org/package/2006/relationships";
            using (ZipArchive za = new ZipArchive(fs, ZipArchiveMode.Update))
            {
                foreach (ZipArchiveEntry entry in za.Entries.ToList())
                {
                    if (!entry.Name.EndsWith(".rels"))
                        continue;
                    bool replaceEntry = false;
                    XDocument entryXDoc;
                    using (Stream entryStream = entry.Open())
                    {
                        try
                        {
                            entryXDoc = XDocument.Load(entryStream);
                            if (entryXDoc.Root != null && entryXDoc.Root.Name.Namespace == relNs)
                            {
                                IEnumerable<XElement> urisToCheck = entryXDoc
                                    .Descendants(relNs + "Relationship")
                                    .Where(r => r.Attribute("TargetMode") != null && (string)r.Attribute("TargetMode") == "External");
                                foreach (XElement rel in urisToCheck)
                                {
                                    string target = (string)rel.Attribute("Target");
                                    if (target == null) continue;
                                    //check if URI is invalid
                                    try
                                    {
                                        // ReSharper disable once UnusedVariable
                                        //[Mathias Schneider] necessary to check if exception will be thrown
                                        Uri uri = new Uri(target);
                                    }
                                    catch (UriFormatException)
                                    {
                                        Uri newUri = invalidUriHandler();
                                        rel.Attribute("Target").Value = newUri.ToString();
                                        replaceEntry = true;
                                    }
                                }
                            }
                        }
                        catch (XmlException)
                        {
                            continue;
                        }
                    }

                    if (!replaceEntry) continue;

                    string fullName = entry.FullName;
                    entry.Delete();
                    ZipArchiveEntry newEntry = za.CreateEntry(fullName);
                    //write back to entryXDoc
                    using (StreamWriter writer = new StreamWriter(newEntry.Open()))
                    using (XmlWriter xmlWriter = XmlWriter.Create(writer))
                    {
                        entryXDoc.WriteTo(xmlWriter);
                    }
                }
            }
        }
    }
}
