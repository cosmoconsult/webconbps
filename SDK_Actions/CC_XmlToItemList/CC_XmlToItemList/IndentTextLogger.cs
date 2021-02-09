using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CC_XmlToItemList
{
    public class IndentTextLogger : IDisposable
    {
        private readonly StringWriter stringWriter = new StringWriter();
        private System.CodeDom.Compiler.IndentedTextWriter indentWriter;

        public IndentTextLogger(string indentationString = "\t")
        {

            indentWriter = new System.CodeDom.Compiler.IndentedTextWriter(stringWriter, indentationString);
        }

        public void Indent()
        {
            indentWriter.Indent++;
        }

        public void Outdent()
        {
            indentWriter.Indent--;
        }

        public void Log(string message)
        {
            indentWriter.WriteLine(message);
        }

        public void Log(string message, Exception ex)
        {
            indentWriter.Write(message);
            logException(ex);
        }
        private void logException(Exception ex)
        {
            indentWriter.Indent++;

            indentWriter.WriteLine($"Exception '{ex.GetType()}");
            indentWriter.WriteLine($"Message '{ex.Message}'");
            indentWriter.WriteLine($"Stack '{ex.StackTrace}'");
            if (ex.InnerException != null)
            {
                logException(ex.InnerException);
            }
            indentWriter.Indent--;
        }

        public override string ToString()
        {
            return stringWriter.ToString();
        }

        public void Dispose()
        {
            if (indentWriter != null)
            {
                indentWriter.Dispose();
            }
            if (stringWriter != null)
            {
                stringWriter.Dispose();
            }
        }
    }
}
