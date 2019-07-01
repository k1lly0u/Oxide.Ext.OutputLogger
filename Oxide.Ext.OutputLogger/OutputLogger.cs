using System;
using System.IO;
using Oxide.Core;
using UnityEngine;

namespace Oxide.Ext.OutputLogger
{
    public class OutputLogger : Core.Logging.Logger
    {
        private string filePath;

        public OutputLogger() : base(true)
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
            Core.Logging.LogType coreType = Core.Logging.LogType.Info;

            switch (logType)
            {
                case LogType.Error:
                    coreType = Core.Logging.LogType.Error;
                    break;                
                case LogType.Warning:
                    coreType = Core.Logging.LogType.Warning;
                    break;
                case LogType.Exception:
                    coreType = Core.Logging.LogType.Error;
                    break;
                case LogType.Assert:
                case LogType.Log:
                default:
                    coreType = Core.Logging.LogType.Info;
                    break;
            }

            this.Write(coreType, string.Concat(message, Environment.NewLine, stackTrace));
        }

        public override void HandleMessage(string message, string stackTrace, Oxide.Core.Logging.LogType logType)
        {           
            this.Write(logType, string.Concat(message, Environment.NewLine, stackTrace));
        }
                
        public override void WriteException(string message, Exception ex)
        {
            string formatted = ExceptionHandler.FormatException(ex);
            if (formatted != null)
            {
                this.Write(Core.Logging.LogType.Error, $"{message}{Environment.NewLine}{formatted}");
                return;
            }

            Exception outerEx = ex;
            while (ex.InnerException != null)
            {
                ex = ex.InnerException;
            }

            if (outerEx.GetType() != ex.GetType())
            {
                this.Write(Core.Logging.LogType.Error, "ExType: {0}", outerEx.GetType().Name);
            }

            this.Write(Core.Logging.LogType.Error, $"{message} ({ex.GetType().Name}: {ex.Message})\n{ex.StackTrace}");
        }

        public override void Write(Core.Logging.LogType type, string format, params object[] args)
        {
            this.Write(this.CreateLog(type, format, args));
        }


        protected Log CreateLog(Core.Logging.LogType type, string format, object[] args)
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
