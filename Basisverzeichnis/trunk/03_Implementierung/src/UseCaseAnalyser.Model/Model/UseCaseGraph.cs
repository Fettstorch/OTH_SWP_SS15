#region Copyright information
// <summary>
// <copyright file="UseCaseGraph.cs">Copyright (c) 2015</copyright>
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
using System.Linq;
using GraphFramework;
using GraphFramework.Interfaces;
using LogManager;

namespace UseCaseAnalyser.Model.Model
{

    /// <summary>
    /// The access enum to the array UseCaseGraphAttributeNames
    /// </summary>
    public enum UseCaseAttributes
    {
        /// <summary>
        /// the name of the use case, e.g. "UseCase-Dokument Importieren"
        /// </summary>
        Name = 0,
        /// <summary>
        /// the id of the use case, e.g. "UC-1"
        /// </summary>
        Id,
        /// <summary>
        /// the priority of the use case, e.g. "hoch"
        /// </summary>
        Priority,
        /// <summary>
        /// the description of the use case, e.g. "Der Anwender möchte ein vorliegendes Word Dokument, welches UseCases beinhaltet in das Tool importieren."
        /// </summary>
        Description,
        /// <summary>
        /// the pre condition of the use case, e.g. "Das Dokument (.docx) hat das richtige Format und ist nicht beschaedigt."
        /// </summary>
        PreCondition,
        /// <summary>
        /// the post condition of the use case, e.g. "Die UseCases existieren als Datenstruktur und können weiterverarbeitet werden."
        /// </summary>
        PostCondition,
        /// <summary>
        /// the normal routine of the use case
        /// </summary>
        NormalRoutine,
        /// <summary>
        /// the sequence variation of the use case
        /// </summary>
        SequenceVariation,
        /// <summary>
        /// the special requirements of the use case, e.g. "keine"
        /// </summary>
        SpecialRequirements,
        /// <summary>
        /// the open points of the use case, e.g. "Soll der Anwender mehrere Dateien auswählen können, die eingelesen werden sollen?"
        /// </summary>
        OpenPoints,
        /// <summary>
        /// how many variants should be traversed in one scenario
        /// </summary>
        TraverseVariantCount,

        /// <summary>
        /// how often loops should be traversed in the scenarios
        /// </summary>
        TraverseLoopCount
    }
    
    /// <summary>
    /// This enum is used to access the attribute names of the string array NodeAttributeNames
    /// </summary>
    public enum NodeAttributes
    {
        /// <summary>
        /// the index of the normal routine, e.g. "2"
        /// </summary>
        NormalIndex,
        /// <summary>
        /// the variant identifier, e.g. "a"
        /// </summary>
        VariantIndex,
        /// <summary>
        /// the variant sequence step, e.g. "1."
        /// </summary>
        VarSeqStep,
        /// <summary>
        /// The description of the node
        /// </summary>
        Description,
        /// <summary>
        /// The type of the node which are start, end, jump nodes, etc.
        /// </summary>
        NodeType
    }

    /// <summary>
    /// class to represent a use case. 
    /// </summary>
    public class UseCaseGraph : Graph
    {
        /// <summary>
        /// The expressions in the use case table
        /// </summary>
        public static readonly string[] UseCaseGraphAttributeNames = 
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
            "Zu klärende Punkte:",
            "Varianten-Traversierungs-Anzahl",
            "Schleifen-Traversierungs-Anzahl"
        };

        /// <summary>
        /// The attribute names of the graph nodes. You can access this array with the enum NodeAttributes
        /// </summary>
        public static readonly string[] NodeAttributeNames = 
        {
            "Normal Index",
            "Variant Index",
            "Variant Sequence Step",
            "Description",
            "NodeType"
        };

        /// <summary>
        /// The nodes are sorted in their different node types
        /// </summary>
        public enum NodeTypeAttribute
        {
            /// <summary>
            /// The node with which the use case starts
            /// </summary>
            StartNode,
            /// <summary>
            /// A variant sequence node which is connected with a normal routine node
            /// </summary>
            JumpNode,
            /// <summary>
            /// A normal routine node
            /// </summary>
            NormalNode,
            /// <summary>
            /// a variant node
            /// </summary>
            VariantNode,
            /// <summary>
            /// a node which ends the use case
            /// </summary>
            EndNode
        }

        private IEnumerable<IGraph> mScenarios;

        /// <summary>
        /// creates a new use case graph with the given attributes
        /// </summary>
        /// <param name="attributes">attributes to add to the use case graph</param>
        public UseCaseGraph(params IAttribute[] attributes)
            : base(attributes) { }

        /// <summary>
        /// scenarios of the use case graph
        /// lazy initialized when getter is called
        /// </summary>
        public IEnumerable<IGraph> Scenarios
        {
            //  lazy initialization of the scenarios
            get
            {
                if (mScenarios == null)
                {
                    //  default traverse variant count: number of edges with description (variants) / 3 (but minimum 3)
                    int variantCount = Nodes.Count(e =>
                        e.Attribute(NodeAttributes.NodeType,false) != null                                                   //first check if node has node type attribut
                        && e.AttributeValue<NodeTypeAttribute>(NodeAttributes.NodeType).Equals(NodeTypeAttribute.JumpNode)   //afterwards check if node type is jump node
                    );
                    InitAttribute(UseCaseAttributes.TraverseVariantCount, (int) Math.Round(variantCount <= 2 ? 1.0 : variantCount / 2.0));
                    InitAttribute(UseCaseAttributes.TraverseLoopCount, 1);

                    try
                    {
                        //  scenario creator can use 'TraverseVariantCount' attribute to create scenarios
                        mScenarios = ScenarioMatrixCreator.CreateScenarios(this);
                    }
                    catch (OutOfMemoryException) 
                    {
                        GC.Collect(3, GCCollectionMode.Forced, true);
                        GetAttributeByName(UseCaseAttributes.TraverseLoopCount.AttributeName()).Value = 1;
                        LoggingFunctions.Error(string.Format("Scenarios of {0} could not be created: Too many Scenarios to create.", GetAttributeByName(UseCaseAttributes.Name.AttributeName())));
                        throw;
                    }
                    
                }

                return mScenarios;
            }
        }

        private void InitAttribute<T>(UseCaseAttributes attribute, T value)
        {
            if (this.Attribute(attribute, false) == null)
            {
                AddAttribute(attribute.CreateAttribute(value, true));
            }
        }

        /// <summary>
        /// returns the use case graph as a string by returning its name attribute
        /// </summary>
        /// <returns>the use case graph as string</returns>
        public override string ToString()
        {
            return (string)GetAttributeByName(UseCaseAttributes.Name.AttributeName()).Value;
        }

        /// <summary>
        /// sets the scenarios to null, so they will be initialized again when getting the property.
        /// </summary>
        public void RecalculateScenarios()
        {
            mScenarios = null;
        }
    }
}