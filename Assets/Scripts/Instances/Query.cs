using System.Collections.Generic;

namespace Instances
{
    public class Query
    {
        public string Filename;
        public string Title;
        public List<string> Programs;
        public string Reply;

        public Query(string filename, string title, List<string> programs, string reply)
        {
            Filename = filename;
            Title = title;
            Programs = programs;
            Reply = reply;
        }
    }
}