Config nocht nicht Implementiert, kommt wenn ich rausgefunden hab ich ich mit git richtig umgehe!


Hier ein Codebeispiel zur Nutzung:


        static void Main(string[] args)
        {
            Dictionary<int, string> dic = new Dictionary<int, string>();
            dic.Add(1, "super");
            dic.Add(100, "duper");

            List<string> list = new List<string>();

            list.Add("super liste");
            list.Add("zweites listenelement");

            string[] strarr = new[] { "asd", "asdfffff" };

            LoggingFunctions.Debug("test");
            LoggingFunctions.Exception(new Exception("help"));
            LoggingFunctions.Trace(dic);
            LoggingFunctions.Trace(list);
            LoggingFunctions.Trace(strarr);




            Console.ReadKey();
        }