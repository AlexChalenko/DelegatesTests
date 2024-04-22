namespace DelegatesTraverser
{
    public class FileArgs : EventArgs
    {
        public string FileName { get; private set; }

        public FileArgs(string fileName)
        {
            FileName = fileName;
        }
    }
}
