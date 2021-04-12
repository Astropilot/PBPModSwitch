using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Threading;
using Memory;
using PBPModSwitch.Core;
using PBPModSwitch.Utils;

namespace PBPModSwitch.ViewModels
{
    class MainWindowViewModel : ViewModel
    {
        private WaitingStatus _waitingStatus;
        public WaitingStatus waitingStatus
        {
            get => _waitingStatus;
            set => Set(ref _waitingStatus, value);
        }

        private List<MapInfoViewModel> _mapFiles;
        public List<MapInfoViewModel> mapFiles
        {
            get => _mapFiles;
            set => Set(ref _mapFiles, value);
        }

        private bool _isModdedEnable;
        public bool isModdedEnable
        {
            get => _isModdedEnable;
            set => Set(ref _isModdedEnable, value);
        }

        public RelayCommand ToggleModsCommand { get; }
        public RelayCommand OnWindowClosingCommand { get; }

        private readonly Mem _memLib;
        private readonly DispatcherTimer _timer;
        private bool _isInitialized;

        public static readonly string s_mapsInAoB = "0x6D 0x61 0x70 0x73";
        public static readonly string s_modsInAoB = "0x6D 0x6F 0x64 0x73";
        private static readonly string[] s_fileWhitelist = { "dae", "hps", "nodes" };
        private static string s_modCachePath;

        public MainWindowViewModel()
        {
            ToggleModsCommand = new RelayCommand(ExecuteToggleMods);
            OnWindowClosingCommand = new RelayCommand(ExecuteOnWindowClosingCommand);
            waitingStatus = new WaitingStatus(true, "Waiting for Penumbra...");
            mapFiles = new List<MapInfoViewModel>();
            _memLib = new Mem();
            _isInitialized = false;
            isModdedEnable = false;

            s_modCachePath = Path.Combine("core", "_mods");

            if (!File.Exists("Penumbra.exe"))
            {
                waitingStatus.waitingMessage = "Error: It seems that the program is not placed in the game folder!";
                return;
            }

            try
            {
                if (Directory.Exists(s_modCachePath))
                {
                    Directory.Delete(s_modCachePath, true);
                }
            }
            catch (Exception exception)
            {
                Logger.Log("Failed to deleting _mods contents.");
                Logger.Log(exception.ToString());
            }

            Directory.CreateDirectory(s_modCachePath);

            _timer = new DispatcherTimer(DispatcherPriority.Normal)
            {
                Interval = TimeSpan.FromSeconds(2)
            };
            _timer.Tick += OnTimerTick;
            _timer.Start();
        }

        public void ExecuteOnWindowClosingCommand(object parameter)
        {
            if (_isInitialized)
            {
                foreach (var mapFile in mapFiles)
                {
                    mapFile.IsChecked = false;
                }
            }

            try
            {
                if (Directory.Exists(s_modCachePath))
                {
                    Directory.Delete(s_modCachePath, true);
                }
            }
            catch (Exception exception)
            {
                Logger.Log("Failed to deleting _mods contents.");
                Logger.Log(exception.ToString());
            }
        }

        private async void OnTimerTick(object sender, EventArgs args)
        {
            bool isProcessAlive = _memLib.OpenProcess("penumbra");

            if (!isProcessAlive && _isInitialized)
            {
                waitingStatus.isWaiting = true;
                waitingStatus.waitingMessage = "Waiting for Penumbra...";
                waitingStatus.showProgress = false;
                mapFiles = new List<MapInfoViewModel>();
                _isInitialized = false;
                isModdedEnable = false;
            }
            else if (isProcessAlive && !_isInitialized)
            {
                _isInitialized = true;

                Thread.Sleep(2500);

                var gameLogPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Penumbra", "Black Plague", "hpl.log");

                if (File.Exists(gameLogPath))
                {
                    using (FileStream stream = File.Open(gameLogPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        using (StreamReader reader = new StreamReader(stream))
                        {
                            if (reader.ReadToEnd().Contains("Game Running"))
                            {
                                waitingStatus.isWaiting = true;
                                waitingStatus.waitingMessage = "Loading maps...";
                                waitingStatus.showProgress = true;

                                Dictionary<string, MapFileInformation> mapsInfo = new Dictionary<string, MapFileInformation>();
                                var mapFiles = CommonUtils.FilterFiles(@"maps\", s_fileWhitelist).Select(f => Path.GetFileName(f));
                                var moddedFiles = CommonUtils.FilterFiles(@"mods\", s_fileWhitelist).Select(f => Path.GetFileName(f));
                                List<MapInfoViewModel> treeviewFiles = new List<MapInfoViewModel>();

                                waitingStatus.progressValue = 0;
                                waitingStatus.progressMax = mapFiles.Count();

                                int progress = 1;
                                foreach (var mapFile in mapFiles)
                                {
                                    waitingStatus.progressValue = progress;
                                    progress += 1;

                                    // Is there is no modded version available (same name) we skip it
                                    if (!moddedFiles.Contains(mapFile))
                                        continue;
                                    // Same if the two files are exactly the same (no modification)
                                    if (new FileInfo(Path.Combine("maps", mapFile)).ComputeHash() == new FileInfo(Path.Combine("mods", mapFile)).ComputeHash())
                                        continue;

                                    string filePrefix = Path.GetFileNameWithoutExtension(mapFile);
                                    string fileExt = Path.GetExtension(mapFile);

                                    if (fileExt.Equals(".nodes")) // Matching nodes files that have a suffix (nodes files)
                                        filePrefix = filePrefix.Substring(0, filePrefix.LastIndexOf("_"));

                                    if (!mapsInfo.ContainsKey(filePrefix)) // New prefix entry
                                    {
                                        var scanInfo = new Dictionary<string, long>();
                                        var scanMaps = await _memLib.AoBScan(0x00400000, 0x0FFFFFFF, CommonUtils.StringToAoB("maps/" + filePrefix), true, false);

                                        // We scan every string containing the prefix (.dae, .hps and .nodes files in memory)

                                        foreach (var scanMap in scanMaps)
                                        {
                                            string str = _memLib.ReadString(Convert.ToString(scanMap, 16), "", 100);

                                            str = str.Substring(5); // Removing 'maps/' part

                                            scanInfo.Add(str, scanMap);
                                        }

                                        mapsInfo.Add(filePrefix, new MapFileInformation(filePrefix, scanInfo));
                                    }

                                    // If scan failed we skip this file
                                    if (!mapsInfo[filePrefix].ScanResults.ContainsKey(mapFile))
                                        continue;

                                    switch (fileExt)
                                    {
                                        case ".dae": // Dealing with Map File
                                            mapsInfo[filePrefix].MapFile.FullName = mapFile;
                                            mapsInfo[filePrefix].MapFile.MemoryPtr = mapsInfo[filePrefix].ScanResults[mapFile];
                                            break;
                                        case ".hps": // Dealing with Program File
                                            mapsInfo[filePrefix].ProgramFile.FullName = mapFile;
                                            mapsInfo[filePrefix].ProgramFile.MemoryPtr = mapsInfo[filePrefix].ScanResults[mapFile];
                                            break;
                                        case ".nodes": // Dealing with Nodes File (can have multiples files for a prefix)
                                            MemoryFileInformation memoryFileInformation = new MemoryFileInformation
                                            {
                                                FullName = mapFile,
                                                MemoryPtr = mapsInfo[filePrefix].ScanResults[mapFile]
                                            };
                                            mapsInfo[filePrefix].NodesFiles.Add(mapFile, memoryFileInformation);
                                            break;
                                    }
                                }

                                foreach (KeyValuePair<string, MapFileInformation> prefixMap in mapsInfo)
                                {
                                    // In the case of a prefix with no match in memory, we skip it
                                    if (!prefixMap.Value.MapFile.isValidPtr() && !prefixMap.Value.ProgramFile.isValidPtr() && prefixMap.Value.NodesFiles.Count == 0)
                                        continue;

                                    MapFileInformation mapFile = prefixMap.Value;
                                    MapInfoViewModel rootMap = new MapInfoViewModel(prefixMap.Key);

                                    rootMap.CheckedStateChanged += OnMapCheckedStateChanged;

                                    if (mapFile.MapFile.isValidPtr())
                                    {
                                        MapFileViewModel mapFileViewModel = new MapFileViewModel("Map file (dae)", mapFile.MapFile.MemoryPtr, _memLib);

                                        mapFileViewModel.CheckedStateChanged += OnMapCheckedStateChanged;
                                        rootMap.Children.Add(mapFileViewModel);
                                    }
                                    if (mapFile.ProgramFile.isValidPtr())
                                    {
                                        MapFileViewModel mapFileViewModel = new MapFileViewModel("Map program (hps)", mapFile.ProgramFile.MemoryPtr, _memLib);

                                        mapFileViewModel.CheckedStateChanged += OnMapCheckedStateChanged;
                                        rootMap.Children.Add(mapFileViewModel);
                                    }
                                    if (mapFile.NodesFiles.Count > 0)
                                    {
                                        MapInfoViewModel nodesChild = new MapInfoViewModel("Map nodes (.nodes)");

                                        foreach (KeyValuePair<string, MemoryFileInformation> nodeFile in mapFile.NodesFiles)
                                        {
                                            MapFileViewModel mapFileViewModel = new MapFileViewModel(nodeFile.Key, nodeFile.Value.MemoryPtr, _memLib);

                                            mapFileViewModel.CheckedStateChanged += OnMapCheckedStateChanged;
                                            nodesChild.Children.Add(mapFileViewModel);
                                        }

                                        nodesChild.CheckedStateChanged += OnMapCheckedStateChanged;
                                        rootMap.Children.Add(nodesChild);
                                    }

                                    rootMap.Initialize();

                                    treeviewFiles.Add(rootMap);
                                }

                                this.mapFiles = treeviewFiles;

                                waitingStatus.isWaiting = false;
                            }
                        }
                    }
                }
            }
        }

        private void OnMapCheckedStateChanged(object sender, EventArgs eventArgs)
        {
            foreach (var mapFile in mapFiles)
            {
                if (!mapFile.IsChecked.HasValue || mapFile.IsChecked.Value == true)
                {
                    isModdedEnable = true;
                    ChangeGameCacheState(true);
                    return;
                }
            }
            isModdedEnable = false;
            ChangeGameCacheState(false);
        }

        private void ChangeGameCacheState(bool disableCache)
        {
            _memLib.WriteMemory("base+2EB9D0", "string", disableCache ? "core/_mods/" : "core/cache/");
        }

        private void ExecuteToggleMods(object parameter)
        {
            var mIsModdedEnable = isModdedEnable;
            foreach (var mapFile in mapFiles)
            {
                mapFile.IsChecked = !mIsModdedEnable;
            }
        }
    }
}
