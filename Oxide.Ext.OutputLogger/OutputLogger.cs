using System;
using System.IO;
using UnityEngine;

namespace Oxide.Ext.OutputLogger
{
    public class OutputLogger
    {
        private string filePath;

        public OutputLogger()
        {
            string directory = Directory.GetParent(Application.dataPath).FullName;

            string oldFilePath = Path.Combine(directory, "output_old.txt");

            filePath = Path.Combine(directory, "output.txt");

            if (File.Exists(oldFilePath))
                File.Delete(oldFilePath);

            if (File.Exists(filePath))            
                File.Move(filePath, oldFilePath);            

            Facepunch.Output.OnMessage += HandleOutputMessage;
        }  

        private void HandleOutputMessage(string message, string stackTrace, UnityEngine.LogType logType)
        {
            if (string.IsNullOrEmpty(message))
                return;
           
            this.Write(logType, string.Concat(message, Environment.NewLine, stackTrace));
        }
       
        public void Write(LogType type, string format, params object[] args)
        {
            this.Write(this.CreateLog(type, format, args));
        }


        protected Log CreateLog(LogType type, string format, object[] args)
        {
            Log logMessage = new Log();

            DateTime now = DateTime.Now;

            logMessage.Message = string.Format("{0} [{1}] {2}", now.ToShortTimeString(), type, format);

            return logMessage;
        }

        protected void Write(Log message)
        {
            using (StreamWriter streamWriter = new StreamWriter(filePath, true))
            {
                streamWriter.WriteLine(message.Message);
            }
        }

        protected struct Log
        {
            public string Message;
        }
    }
}
