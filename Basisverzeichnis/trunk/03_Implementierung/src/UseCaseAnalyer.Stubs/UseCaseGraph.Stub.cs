using System.Collections.Generic;
using System.IO;
using System.Linq;
using GraphFramework;
using GraphFramework.Interfaces;
using UseCaseAnalyser.Model.Model;

namespace UseCaseAnalyer.Stubs
{
    /// <summary>
    /// Stub for UseCaseGraph (singleton)
    /// </summary>
    public class UseCaseGraphStub
    {
        public List<UseCaseGraph> mDummy1;

        private static UseCaseGraphStub singleReference;

        public static UseCaseGraphStub GetInstance()
        {
            UseCaseGraphStub retUseCaseGraphStub;

            if (singleReference != null)
            {
                retUseCaseGraphStub=singleReference;
            }
            else
            {
                singleReference = new UseCaseGraphStub();
                retUseCaseGraphStub = singleReference;
            }

            return retUseCaseGraphStub;
        }


        private UseCaseGraphStub()
        {
            FileInfo dummyUseCaseFile = new FileInfo("UseCaseDummies.docx");
            mDummy1 = WordImporter.ImportUseCases(dummyUseCaseFile);
        }

    }
}
