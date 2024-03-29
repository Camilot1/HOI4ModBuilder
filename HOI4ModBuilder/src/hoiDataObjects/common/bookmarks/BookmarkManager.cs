﻿using HOI4ModBuilder.src.hoiDataObjects.common.buildings;
using HOI4ModBuilder.src.managers;
using Pdoxcl2Sharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HOI4ModBuilder.src.hoiDataObjects.common.bookmarks
{
    class BookmarkManager : IParadoxRead
    {
        public static BookmarkManager Instance { get; private set; }
        private static FileInfo _currentFile = null;
        private static Dictionary<FileInfo, List<Bookmark>> _bookmarksByFilesMap = new Dictionary<FileInfo, List<Bookmark>>();
        private static Dictionary<string, Bookmark> _allBookmarks = new Dictionary<string, Bookmark>();
        public static Dictionary<string, Bookmark>.ValueCollection GetAllBookmarks => _allBookmarks.Values;
        private static Dictionary<DateTime, Bookmark> _allBookmarksByDateTimes = new Dictionary<DateTime, Bookmark>();

        private static Action _guiReinitAction = null;

        public static void Load(Settings settings)
        {
            Instance = new BookmarkManager();
            _bookmarksByFilesMap = new Dictionary<FileInfo, List<Bookmark>>();
            _allBookmarks = new Dictionary<string, Bookmark>();
            _allBookmarksByDateTimes = new Dictionary<DateTime, Bookmark>();

            var fileInfos = FileManager.ReadMultiTXTFileInfos(settings, @"common\bookmarks\");

            MainForm.Instance.InvokeAction(() => MainForm.Instance.ToolStripComboBox_Data_Bookmark.Items.Clear());

            foreach (var fileInfo in fileInfos.Values)
            {
                _currentFile = fileInfo;
                using (var fs = new System.IO.FileStream(fileInfo.filePath, System.IO.FileMode.Open))
                    ParadoxParser.Parse(fs, Instance);
            }

            if (_guiReinitAction == null)
            {
                _guiReinitAction = () =>
                {
                    foreach (var bookmark in GetAllBookmarks)
                        MainForm.Instance.ToolStripComboBox_Data_Bookmark.Items.Add($"[{Utils.DateTimeStampToString(bookmark.dateTimeStamp)}] {bookmark.name}");
                };
                MainForm.SubscribeGuiReinitAction(_guiReinitAction);
            }

            MainForm.Instance.InvokeAction(() => _guiReinitAction());
        }

        public void TokenCallback(ParadoxParser parser, string token)
        {
            if (token == "bookmarks")
            {
                var bookmarks = new List<Bookmark>();
                var list = new BookramkList(bookmarks);
                parser.Parse(list);
                _bookmarksByFilesMap[_currentFile] = bookmarks;
            }
        }

        class BookramkList : IParadoxRead
        {
            private List<Bookmark> _bookmarks;
            public BookramkList(List<Bookmark> bookmarks)
            {
                _bookmarks = bookmarks;
            }

            public void TokenCallback(ParadoxParser parser, string token)
            {
                Bookmark bookmark = new Bookmark();
                parser.Parse(bookmark);
                _bookmarks.Add(bookmark);
                _allBookmarks[bookmark.name] = bookmark;
                _allBookmarksByDateTimes[bookmark.dateTimeStamp] = bookmark;
            }
        }
    }
}
