using System;
using System.Collections.Generic;
using System.Text;

namespace SWE1_SEB_Eory
{
    public class RequestContext
    {
        public string Method { get; set; }
        public string FirstLine { get; set; }
        public string endpoint { get; set; }
        public int contentlength { get; set; }
        public string content { get; set; }
        public bool body { get; set; } = false;
        public string AuthorizationToken { get; set; }

        private List<string> messagesList = new List<string>();

        public RequestContext(string inputMessage, List<string> list)
        {
            messagesList = list;
            var messageSplitLine = inputMessage.Split("\r\n");
            FirstLine = messageSplitLine[0];
            int rowCount = 2;
            GetAuthToken(messageSplitLine);
            int index = FindContentLength(messageSplitLine);

            content = messageSplitLine[index + rowCount];



            if (body == true)
            {
                while (content.Length != contentlength)
                {
                    rowCount++;
                    content += "\r\n";
                    content += messageSplitLine[index + rowCount];
                }
            }

            var messageSplit = FirstLine.Split(" ");
            Method = messageSplit[0];
            endpoint = messageSplit[1];
            var messagePathSplit = endpoint.Split("/");
        }

        private void GetAuthToken(string[] messageSplitLine)
        {
            string tmp = "";

            for (int i = 0; i < messageSplitLine.Length; i++)
            {
                tmp = messageSplitLine[i];
                var tmp2 = tmp.Split(":");
                string tmp3 = tmp2[0];
                if (tmp3 == "Authorization")
                {
                    AuthorizationToken = tmp2[1];
                    return;
                }
            }
            return;
        }

        private int FindContentLength(string[] messageSplitLine)
        {
            int index = 0;
            string tmp = "";

            for (int i = 0; i < messageSplitLine.Length; i++)
            {
                tmp = messageSplitLine[i];
                var tmp2 = tmp.Split(":");
                string tmp3 = tmp2[0];
                if (tmp3 == "Content-Length")
                {
                    var contentLengthString = tmp2[1];
                    contentlength = Convert.ToInt32(contentLengthString);
                    index = i;
                    body = true;
                    return i;
                }
                else
                {
                    body = false;
                }
            }
            return index;
        }


    }
}
