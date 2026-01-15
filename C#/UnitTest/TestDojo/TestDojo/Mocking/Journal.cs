namespace TestDojo.Mocking
{
    public class Journal
    {
        // example from https://learn.microsoft.com/en-us/dotnet/api/system.io.file.readalltext?view=net-7.0

        public void FileProcess(string filepath)
        {
            // Check if the input file exists
            string logMessage;
            if (!File.Exists(filepath))
            {
                logMessage = "File not found" + Environment.NewLine;
            }
            else
            {
                // Read the file content if it exists
                string readText = File.ReadAllText(filepath);
                logMessage = readText + Environment.NewLine;
            }

            // Write the message to log.txt
            File.WriteAllText("log.txt", logMessage);
        }
    }
}
