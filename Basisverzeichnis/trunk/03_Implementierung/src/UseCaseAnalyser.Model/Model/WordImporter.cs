using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using LogManager;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using GraphFramework;
using Table = DocumentFormat.OpenXml.Wordprocessing.Table;

namespace UseCaseAnalyser.Model.Model
{
    public static class WordImporter
    {
        private static string[] importStepNames =
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
            int tableNumber = 0;
            foreach (Table table in tables)
            {
                // every possible use case table
                IEnumerable<TableRow> rows = table.Descendants<TableRow>();
                UseCaseGraph useCaseGraph;
                if (WordImporter.TryReadInUseCase(rows, out useCaseGraph))
                {
                    useCaseList.Add(useCaseGraph);
                    tableNumber++;
                }
            }

            if (tableNumber == 0)
            {
                // Es konnten keine Use Cases gefunden werden. To-Do: Warnung, Logger, etc!
            }

            return useCaseList; 
        }

        private static bool TryReadInUseCase(IEnumerable<TableRow> tableRows, out UseCaseGraph useCaseGraph)
        {
            useCaseGraph = new UseCaseGraph();

            // To-Do: Auslesen, Validieren, etc. ...to be continued
            foreach (TableRow row in tableRows)
            {
                IEnumerable<Cell> cells = row.Descendants<Cell>();
                foreach (Cell cell in cells)
                {
                    
                }
            }

            return false;
        }
    }
}
