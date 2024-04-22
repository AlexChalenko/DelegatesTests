namespace DelegatesTraverser
{
    public class DirectoryTraverser
    {
        public event EventHandler<FileArgs> FileFound;

        public IList<FileInfo> Files { get; private set; }
        protected virtual void OnFileFound(FileArgs e)
        {
            EventHandler<FileArgs> handler = FileFound;
            if (handler != null) handler(this, e);
        }

        public DirectoryTraverser()
        {
            Files = new List<FileInfo>();
        }
        public bool CancelRequested { get; private set; }

        public void RequestCancel()
        {
            CancelRequested = true;
        }

        public void Traverse(string path)
        {
            if (string.IsNullOrWhiteSpace(path) || !Directory.Exists(path))
                throw new ArgumentException("Invalid path provided.");

            CancelRequested = false; // Reset cancel flag at the start of each traverse.

            DirectoryInfo dirInfo = new DirectoryInfo(path);
            FileInfo[] files = dirInfo.GetFiles();

            foreach (FileInfo file in files)
            {
                OnFileFound(new FileArgs(file.Name));
                Files.Add(file);
                if (CancelRequested)
                    break;
            }

            // Recursively traverse subdirectories if not cancelled.
            if (!CancelRequested)
            {
                DirectoryInfo[] subDirs = dirInfo.GetDirectories();
                foreach (DirectoryInfo dir in subDirs)
                {
                    Traverse(dir.FullName);
                    if (CancelRequested)
                        break;
                }
            }
        }
    }


}
