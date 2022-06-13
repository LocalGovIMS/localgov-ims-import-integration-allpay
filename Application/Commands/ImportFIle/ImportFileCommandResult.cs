using System.Collections.Generic;

namespace Application.Commands
{
    public class ImportFileCommandResult
    {
        public int FilesProcessed { get; set; }
        public List<string> ErrorString { get; set; } = new List<string>();
        public int ErrorsDetected { get; set; }
        public bool Success { get; set; }
    }
}
