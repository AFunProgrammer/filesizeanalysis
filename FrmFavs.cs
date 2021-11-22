using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace favs
{
    public partial class FrmFavs : Form
    {
        private FileSystemWatcher _fswWindows;
        private static object _oMsgsLock = new object();
        private static object _oLstCtlLock = new object();
        static Queue<String> _qConsoleMessages = new Queue<string>();
        List<String> lstExceptionPath = new List<string>();
        Mutex _mLockMessages = new Mutex(false, "messages");
        System.Threading.Timer _ReadMessagesTimer = null;

        SortedDictionary<String, long> _dPathAndSize = new SortedDictionary<string, long>();

        const UInt32 INVALID_FILE_SIZE = 0xFFFFFFFF;

        [DllImport("kernel32.dll",SetLastError=true)]
        static extern uint GetCompressedFileSizeW(
           [In, MarshalAs(UnmanagedType.LPWStr)] string lpFileName,
           [Out, MarshalAs(UnmanagedType.U4)] out uint lpFileSizeHigh);

        public static long GetFileSizeOnDisk(string filepath)
        {
            FileInfo file = new FileInfo(filepath);
            uint hisize;
            uint losize = GetCompressedFileSizeW(filepath, out hisize);

            //invalid file size - I don't understand why a file size would have an issue
            // but probably should investigate further to understand the issue.
            if (INVALID_FILE_SIZE == losize)
            {
                int uiLastError = Marshal.GetLastWin32Error();
                int uiLastHResult = Marshal.GetHRForLastWin32Error();

                System.Diagnostics.Debug.WriteLine("INVALID_FILE_SIZE: HRESULT: {0:X2} Size: {1} File: {2}", uiLastHResult, file.Length, file.FullName);
                return file.Length; // return the length in bytes without compression to appx nearest value;
            }

            long size = ((long)hisize << 32) | losize;

            return size;
        }

        private void GetFilesFromPath(string Path)
        {
            DirectoryInfo Dir = new DirectoryInfo(Path);
            FileInfo[] Files;
            (string Name, long Size) listViewFields;

            lvFiles.Items.Clear();

            if (null == Dir || "" == Path.Trim())
                return;


            Files = Dir.GetFiles();
            string strFileSize = "";
            foreach (FileInfo File in Files)
            {
                float fFileSize = ((float)GetFileSizeOnDisk(File.FullName) / (1024 * 1024));

                if (fFileSize < 1.0f)
                    strFileSize = String.Format("0 MB", fFileSize * 1024);
                else
                    strFileSize = String.Format("{0,8:#,###,###.##} MB", fFileSize);
                listViewFields = new(File.Name, File.Length);

                ListViewItem lvItem = new ListViewItem(new String[] { File.Name, strFileSize });
                lvItem.Tag = listViewFields;

                lvFiles.Items.Add(lvItem);

                //_dFileNameAndSize.Add(File.Name, File.Length);
            }
        }

        private long GetFileSizeFromDir(DirectoryInfo Dir)
        {
            long lSizeOnDisk = 0;
            FileInfo[] Files;
            DirectoryInfo[] SubDirs;
            List<DirectoryInfo> Directories = new List<DirectoryInfo>();
            
            try
            {
                SubDirs = Dir.GetDirectories();
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(Dir.FullName + " " + e.HResult.ToString());
                lstExceptionPath.Add(Dir.FullName);
                return 0;
            }
            //System.Diagnostics.Debug.WriteLine(Dir.FullName);

            //AddRange seems to only create references but doesn't actually copy
            // which means the underlying collection still remains ...
            Directories.AddRange(SubDirs);

            if (0 == Directories.Count)
            {
                Files = Dir.GetFiles();
                foreach (FileInfo File in Files)
                    lSizeOnDisk += GetFileSizeOnDisk(File.FullName); //File.Length;

                _dPathAndSize.Add(Dir.FullName, lSizeOnDisk);
                return lSizeOnDisk;
            }

            while (Directories.Count > 0)
            {
                if (Directories[0].LinkTarget == null)
                    lSizeOnDisk += GetFileSizeFromDir(Directories[0]);
                Directories.RemoveAt(0);
            }

            Files = Dir.GetFiles();
            foreach (FileInfo File in Files)
                lSizeOnDisk += File.Length;

            _dPathAndSize.Add(Dir.FullName, lSizeOnDisk);

            return lSizeOnDisk;
        }

        private void GetSubDirectories(DirectoryInfo Dir, TreeNode ParentNode, int MaxDepth, int Depth )
        {
            TreeNode Node;
            DirectoryInfo[] SubDirs;
            List<DirectoryInfo> Directories = new List<DirectoryInfo>();

            if (Depth >= MaxDepth )
            {
                return;
            }

            try
            {
                SubDirs = Dir.GetDirectories();
            }
            catch(Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Path: {0} Exception: {1}", Dir.FullName, e.Message);
                return;
            }
            //System.Diagnostics.Debug.WriteLine(Dir.FullName);

            //AddRange seems to only create references but doesn't actually copy
            // which means the underlying collection still remains ...
            Directories.AddRange(SubDirs);

            string strNodeText = Dir.Name;
            float fSizeOnDisk = (float)_dPathAndSize[Dir.FullName] / (1024 * 1024);

            if (fSizeOnDisk < 1.0f)
                strNodeText += "  0 MB";
            else
                strNodeText += String.Format("  {0,8:#,###,###.##} MB", fSizeOnDisk);

            if (null == ParentNode)
                Node = trvFileSystem.Nodes.Add(strNodeText);
            else
            {
                ValueTuple<string, float> vtTag = (ValueTuple<string,float>)ParentNode.Tag;
                if (vtTag.Item1 == Dir.FullName)
                    Node = ParentNode;
                else
                    Node = ParentNode.Nodes.Add(strNodeText);
            }

            //Add the top level directory
            Node.Tag = (Dir.FullName, fSizeOnDisk);

            if (0 == Directories.Count)
            {
                return;
            }

            while ( Directories.Count > 0 )
            {
                if ( Directories[0].LinkTarget == null )
                    GetSubDirectories(Directories[0], Node, MaxDepth, (Depth+1));
                Directories.RemoveAt(0);
            }
        }


        private void PopulateFileSystemView()
        {
            System.IO.DriveInfo[] allDrives = System.IO.DriveInfo.GetDrives();

            foreach (DriveInfo drive in allDrives)
            {
                GetFileSizeFromDir(new DirectoryInfo(drive.Name));
                GetSubDirectories(new DirectoryInfo(drive.Name), null, 3, 0);
            }
        }

        static int count = 0;

        private void ReadFromMessages(object sender)
        {
            Interlocked.Increment(ref count);

            //throw new Exception("InMyCodeException - Are You Actually Calling In My Code");
            List<String> lstMessages = new List<string>();

            lock (_oMsgsLock)
            {
                for( int iMsg = 0; iMsg < _qConsoleMessages.Count; iMsg++ )
                {
                    lstMessages.Add(_qConsoleMessages.Dequeue());
                }
            }

            lstConsole.Invoke(new Action(() =>
               {
                   bool bAtLastItem = false;

                   if (null == lstMessages || 0 == lstMessages.Count)
                       return;

                   if (lstConsole.Items.Count - 1 == lstConsole.SelectedIndex)
                       bAtLastItem = true;

                   foreach (string msg in lstMessages)
                   {
                       lstConsole.Items.Add(msg);
                   }

                   if (true == bAtLastItem)
                       lstConsole.SelectedIndex = lstConsole.Items.Count - 1;
               }));

            //System.Diagnostics.Debug.WriteLine("Read From Messages: {0}", count);
        }

        private void WriteToMessages(string Out)
        {
            lock (_oMsgsLock)
            {
                _qConsoleMessages.Enqueue(Out);
            }
        }

        private void FileSystemWatcher_OnEvent(object sender, FileSystemEventArgs e)
        {
            String strEventInformation = "";

            strEventInformation = e.ChangeType.ToString() + " - " + e.FullPath;
            WriteToMessages(strEventInformation);
        }

        private void FileSystemWatcher_OnError(object sender, ErrorEventArgs e)
        {
            string strErrorInformation = "";
            strErrorInformation = "Error: " + e.ToString();
            WriteToMessages(strErrorInformation);
        }

        private void SetupFileSystemWatcher(string Path)
        {
            _fswWindows = new FileSystemWatcher(Path);

            _fswWindows.NotifyFilter = NotifyFilters.DirectoryName | NotifyFilters.LastWrite | NotifyFilters.Security;
            _fswWindows.Created += FileSystemWatcher_OnEvent;
            _fswWindows.Changed += FileSystemWatcher_OnEvent;
            _fswWindows.Deleted += FileSystemWatcher_OnEvent;
            _fswWindows.Renamed += FileSystemWatcher_OnEvent;
            _fswWindows.Error += FileSystemWatcher_OnError;

            _fswWindows.Filter = "*.*";
            _fswWindows.IncludeSubdirectories = true;
            _fswWindows.EnableRaisingEvents = true;
        }

        public FrmFavs()
        {
            InitializeComponent();
            lstConsole.Items.Add(" ");
            lstConsole.SelectedIndex = 0;
            SetupFileSystemWatcher(@"C:\");
        }

        private void FrmFavs_Load(object sender, EventArgs e)
        {
            _ReadMessagesTimer = new System.Threading.Timer(ReadFromMessages);
            _ReadMessagesTimer.Change(500, 500);
           // PopulateFileSystemView();
        }

        private void trvFileSystem_AfterExpand(object sender, TreeViewEventArgs e)
        {
            for ( int i = 0; i < e.Node.Nodes.Count; i++ )
            {
                if (0 == e.Node.Nodes[i].Nodes.Count)
                {
                    String strPath = ((ValueTuple<string, float>)e.Node.Nodes[i].Tag).Item1;

                    DirectoryInfo Dir = new DirectoryInfo(strPath);
                    GetSubDirectories(Dir, e.Node.Nodes[i], 2, 0);
                }
            }
        }

        private void FrmFavs_FormClosing(object sender, FormClosingEventArgs e)
        {
            lstExceptionPath.Sort();
            foreach(string path in lstExceptionPath)
            {
                System.Diagnostics.Debug.WriteLine(path);
            }
        }

        private void FrmFavs_Shown(object sender, EventArgs e)
        {
            PopulateFileSystemView();

            System.IO.DriveInfo[] allDrives = System.IO.DriveInfo.GetDrives();
            
            //foreach (DriveInfo drive in allDrives )
            //{
            //    var TotalFreeSpace = drive.TotalFreeSpace;
            //    var AvailableFreeSpace = drive.AvailableFreeSpace;
            //
            //    float fUsedSpace = (float)(drive.TotalSize - drive.AvailableFreeSpace) / (1024 * 1024);
            //    String strDriveInfo = String.Format("{0} - Used Space {1:###,###.##} MB", drive.Name, fUsedSpace);
            //    cboDriveList.Items.Add(strDriveInfo);
            //}
        }

        private void trvFileSystem_AfterSelect(object sender, TreeViewEventArgs e)
        {
            var tagData = e.Node.Tag;

            GetFilesFromPath(((ValueTuple<string,float>)tagData).Item1);
        }

        private int lvFilesSortColumn = 0;
        private SortOrder lvSortOrder = SortOrder.None;
        private void lvFiles_ColumnClick(object sender, ColumnClickEventArgs e)
        {

            if ( lvFilesSortColumn != e.Column )
            {
                lvFilesSortColumn = e.Column;
                lvSortOrder = SortOrder.Ascending;
            }
            else
            {
                switch(lvSortOrder)
                {
                    case SortOrder.Ascending:
                        lvSortOrder = SortOrder.Descending;
                        break;
                    case SortOrder.Descending:
                    default:
                        lvSortOrder = SortOrder.Ascending;
                        break;
                }
            }

            lvFiles.Sorting = lvSortOrder;
            lvFiles.ListViewItemSorter = new ListViewItemComparer(e.Column);
            lvFiles.Sort();
        }

        class ListViewItemComparer : System.Collections.IComparer
        {
            private int _SortByColumn = 0;
            bool _SortLong = false;
            public ListViewItemComparer() 
            {
                _SortByColumn = 0;
                _SortLong = false;
            }

            public ListViewItemComparer(int Column)
            {
                _SortByColumn = Column;
                if ( 1 == _SortByColumn)
                {
                    _SortLong = true;
                }
            }

            public int Compare(object A, object B)
            {
                ValueTuple<string,long> vObjectA, vObjectB;
                ListView lvContaining = null;
                int iSortOrder = 1;
                //null check 'Check to see if necessary first I suppose
                if ( null == A || null == B )
                {
                    throw new Exception("Object is null Exception");
                }

                try
                {
                    vObjectA = (ValueTuple<string, long>)((ListViewItem)A).Tag;
                    vObjectB = (ValueTuple<string, long>)((ListViewItem)B).Tag;
                }
                catch
                {
                    //throw new Exception("Unable to cast ValueTuple<string,long> in ListViewItemComparer:Compare");
                    System.Diagnostics.Debug.WriteLine("ListViewItemComparer:Compare - Unable to cast ValueTuple<string, long>");
                    return 0;
                }

                lvContaining = ((ListViewItem)A).ListView;
                iSortOrder = (SortOrder.Ascending == lvContaining.Sorting ? 1 : -1);

                if ( _SortLong )
                {
                    if (vObjectA.Item2 == vObjectB.Item2)
                        return 0;

                    if (vObjectA.Item2 > vObjectB.Item2)
                        return (iSortOrder * 1);
                    else
                        return (iSortOrder * -1);
                }
                else
                {
                    return (iSortOrder * string.Compare(vObjectA.Item1, vObjectB.Item1, true));
                }

                return 0;
            }
        }
    }
}