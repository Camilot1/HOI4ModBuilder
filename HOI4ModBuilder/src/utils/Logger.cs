
using HOI4ModBuilder.src.forms;
using HOI4ModBuilder.src.managers;
using HOI4ModBuilder.src.Pdoxcl2Sharp;
using HOI4ModBuilder.src.utils.exceptions;
using Pdoxcl2Sharp;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HOI4ModBuilder.src.utils
{
    class Logger
    {
        public static readonly string logDirPath = FileManager.AssembleFolderPath(new string[] { "logs" });
        public static readonly string logFilePath = logDirPath + "latest.log";
        public static readonly string version = "Alpha v0.2.9.3a";
        public static readonly int versionId = 16;

        private static readonly object _warningsSync = new object();
        private static readonly object _errorsSync = new object();
        private static readonly object _exceptionsSync = new object();
        private static readonly object _additionalExceptionsSync = new object();
        private static readonly object _textBoxFormsSync = new object();
        private static readonly object _flushingTaskSync = new object();

        private static readonly List<string> _warnings = new List<string>();
        private static readonly List<string> _errors = new List<string>();
        private static readonly List<string> _exceptions = new List<string>();
        private static readonly List<string> _additionalExceptions = new List<string>();

        private static readonly List<TextBoxMessageForm> _textBoxMessageForms = new List<TextBoxMessageForm>();

        private static readonly ConcurrentQueue<string> flushQueue = new ConcurrentQueue<string>();
        private static Task flushingTask = null;

        public static void Init()
        {
            try
            {
                if (!Directory.Exists(logDirPath))
                    Directory.CreateDirectory(logDirPath);

                File.Delete(logFilePath);
                RunFlushingTaskIfNeeded();
                Log($"Program version: {version}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to remove latest.log file: {ex}");
            }
        }

        private static void RunFlushingTaskIfNeeded()
        {
            lock (_flushingTaskSync)
            {
                if (flushingTask != null)
                    return;

                flushingTask = Task.Run(() =>
                {
                    try
                    {
                        while (true)
                            Cycle();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }
                });
            }

            void Cycle()
            {
                var flushLines = new List<string>(64);

                while (flushQueue.TryDequeue(out var result))
                    flushLines.Add(result);

                if (flushLines.Count > 0)
                {
                    Console.WriteLine($"{DateTime.Now}: Flushing {flushLines.Count} lines to {logFilePath}");
                    File.AppendAllLines(logFilePath, flushLines);
                    flushLines.Clear();
                }
                Thread.Sleep(1000);
            }
        }

        public static void Log(string message)
        {
            string logEntry = $"[{DateTime.Now}]: {message}";
            try
            {
                flushQueue.Enqueue(logEntry);
                //Console.WriteLine(message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to write message {message} to log file: {ex}");
            }
        }

        public static void LogSingleErrorMessage(EnumLocKey enumLocKey)
            => LogSingleErrorMessage(GuiLocManager.GetLoc(enumLocKey));

        public static void LogSingleErrorMessage(EnumLocKey enumLocKey, Dictionary<string, string> replaceValues)
            => LogSingleErrorMessage(GuiLocManager.GetLoc(enumLocKey, replaceValues));

        public static void LogSingleErrorMessage(string message)
        {
            ShowMessageOnUiThread(
                message,
                GuiLocManager.GetLoc(EnumLocKey.ERROR_HAS_OCCURED),
                MessageBoxIcon.Error
            );
            Log(message);
        }

        public static void LogSingleInfoMessage(EnumLocKey enumLocKey)
            => LogSingleInfoMessage(GuiLocManager.GetLoc(enumLocKey));

        public static void LogSingleInfoMessage(EnumLocKey enumLocKey, Dictionary<string, string> replaceValues)
            => LogSingleInfoMessage(GuiLocManager.GetLoc(enumLocKey, replaceValues));

        public static void LogSingleInfoMessage(string message)
        {
            ShowMessageOnUiThread(
                message,
                GuiLocManager.GetLoc(EnumLocKey.INFORMATION_MESSAGE_TITLE),
                MessageBoxIcon.Information
            );
            Log(message);
        }

        public static void ShowMessageOnUiThread(string message, string caption, MessageBoxIcon icon)
            => MessageBoxUtils.Show(message, caption, MessageBoxButtons.OK, icon);

        public static void CheckLayeredValueOverrideAndSet<T>(LinkedLayer prevLayer, string parameterName, ref T oldValue, T newValue)
        {
            if (oldValue != null)
                LogLayeredWarning(
                    prevLayer, parameterName, EnumLocKey.LAYERED_LEVELS_PARAMETER_VALUE_OVERRIDDEN,
                    new Dictionary<string, string>
                    {
                        { "{oldParameterValue}", oldValue?.ToString() },
                        { "{newParameterValue}", newValue?.ToString() }
                    }
                );

            oldValue = newValue;
        }

        public static void CheckValueOverrideAndSet<T>(LinkedLayer prevLayer, string parameterName, ref T? oldValue, T newValue) where T : struct
        {
            if (oldValue != null)
                LogLayeredWarning(new LinkedLayer(prevLayer, parameterName), EnumLocKey.LAYERED_LEVELS_BLOCK_VALUE_OVERRIDDEN);

            oldValue = newValue;
        }

        public static void CheckValueOverrideAndSet<T>(LinkedLayer prevLayer, ref T? oldValue, T newValue) where T : struct
        {
            if (oldValue != null)
                LogLayeredWarning(prevLayer, EnumLocKey.LAYERED_LEVELS_BLOCK_VALUE_OVERRIDDEN);

            oldValue = newValue;
        }

        public static void CheckValueOverrideAndSet<T>(LinkedLayer prevLayer, string parameterName, ref T oldValue, T newValue) where T : class
        {
            if (oldValue != null)
                LogLayeredWarning(new LinkedLayer(prevLayer, parameterName), EnumLocKey.LAYERED_LEVELS_BLOCK_VALUE_OVERRIDDEN);

            oldValue = newValue;
        }

        public static void CheckValueOverrideAndSet<T>(LinkedLayer prevLayer, ref T oldValue, T newValue) where T : class
        {
            if (oldValue != null)
                LogLayeredWarning(prevLayer, EnumLocKey.LAYERED_LEVELS_BLOCK_VALUE_OVERRIDDEN);

            oldValue = newValue;
        }

        public static void ParseLayeredValueAndCheckOverride<T>(LinkedLayer prevLayer, string newLayerName, ref T oldValue, ParadoxParser parser, T newParseObject) where T : class, IParadoxObject
        {
            var newLayer = new LinkedLayer(prevLayer, newLayerName);

            T parsedValue = null;
            WrapTokenCallbackExceptions(newLayerName, () => parsedValue = parser.AdvancedParse(newLayer, newParseObject, out bool _));

            if (oldValue != null)
                LogLayeredWarning(newLayer, EnumLocKey.LAYERED_LEVELS_BLOCK_VALUE_OVERRIDDEN);

            oldValue = parsedValue;
        }

        public static void ParseLayeredValue<T>(LinkedLayer newLayer, ref T value, ParadoxParser parser, T newParseObject) where T : class, IParadoxObject
        {
            T parsedValue = null;
            WrapTokenCallbackExceptions(newLayer.Name, () => parsedValue = parser.AdvancedParse(newLayer, newParseObject, out bool _));
            value = parsedValue;
        }

        public static void ParseLayeredValue<T>(LinkedLayer prevLayer, string newLayerName, ref T value, ParadoxParser parser, T newParseObject) where T : class, IParadoxObject
        {
            var newLayer = new LinkedLayer(prevLayer, newLayerName);
            ParseLayeredValue(newLayer, ref value, parser, newParseObject);
        }

        public static void ParseLayeredValue<T>(LinkedLayer prevLayer, string newLayerName, ParadoxParser parser, T newParseObject) where T : class, IParadoxObject
        {
            var newLayer = new LinkedLayer(prevLayer, newLayerName);
            WrapTokenCallbackExceptions(newLayerName, () => parser.AdvancedParse(newLayer, newParseObject, out bool _));
        }

        public static void ParseLayeredValue<T>(LinkedLayer prevLayer, string newLayerName, T value, ParadoxParser parser) where T : class, IParadoxObject
        {
            var newLayer = new LinkedLayer(prevLayer, newLayerName);
            ParseLayeredValue(newLayer, value, parser);
        }
        public static void ParseLayeredValue<T>(LinkedLayer newLayer, T value, ParadoxParser parser) where T : class, IParadoxObject
        {
            WrapTokenCallbackExceptions(newLayer.Name, () => value = parser.AdvancedParse(newLayer, value, out bool _));
        }

        public static void ParseLayeredListedValue<T>(LinkedLayer prevLayer, string newLayerName, ref List<T> list, ParadoxParser parser, T newParseObject) where T : class, IParadoxObject
        {
            if (list == null) list = new List<T>();

            var newLayer = new LinkedLayer(prevLayer, newLayerName + $" [№{list.Count + 1}]");
            T parsedValue = null;
            WrapTokenCallbackExceptions(newLayerName, () => parsedValue = parser.AdvancedParse(newLayer, newParseObject, out bool _));
            list.Add(parsedValue);
        }

        public static void LogWarning(EnumLocKey enumLocKey, Dictionary<string, string> replaceValues)
        {
            AddWarningMessage(GuiLocManager.GetLoc(enumLocKey, replaceValues));
            Log($"WARNING: {enumLocKey}, Values: {Utils.DictionaryToString(replaceValues)}");
        }

        public static void LogWarning(string message)
        {
            AddWarningMessage(message);
            Log($"WARNING: {message}");
        }

        public static void LogError(EnumLocKey enumLocKey, Dictionary<string, string> replaceValues)
        {
            AddErrorMessage(GuiLocManager.GetLoc(enumLocKey, replaceValues));
            Log($"ERROR: {enumLocKey}, Values: {Utils.DictionaryToString(replaceValues)}");
        }

        public static void LogError(EnumLocKey enumLocKey, Dictionary<string, string> replaceValues, string additionalText)
        {
            AddErrorMessage($"{GuiLocManager.GetLoc(enumLocKey, replaceValues)} {additionalText}");
            Log($"ERROR: {enumLocKey}, Values: {Utils.DictionaryToString(replaceValues)}");
        }
        public static void LogWarning(EnumLocKey enumLocKey, Dictionary<string, string> replaceValues, string additionalText)
        {
            AddWarningMessage($"{GuiLocManager.GetLoc(enumLocKey, replaceValues)} {additionalText}");
            Log($"WARNING: {enumLocKey}, Values: {Utils.DictionaryToString(replaceValues)}");
        }

        public static void LogExceptionAsError(EnumLocKey enumLocKey, Dictionary<string, string> replaceValues, Exception ex)
        {
            LogError(enumLocKey, replaceValues, BuildExceptionMessage(ex));
            Log($"EXCEPTION AS ERROR: {enumLocKey}, Values: {Utils.DictionaryToString(replaceValues)}, Exception: {ex}\n");
        }

        public static void LogExceptionAsWarning(EnumLocKey enumLocKey, Dictionary<string, string> replaceValues, Exception ex)
        {
            LogWarning(enumLocKey, replaceValues, BuildExceptionMessage(ex));
            Log($"EXCEPTION AS WARNING: {enumLocKey}, Values: {Utils.DictionaryToString(replaceValues)}, Exception: {ex}\n");
        }

        public static void LogException(Exception ex)
        {
            string message = BuildExceptionMessage(ex);

            if (MainForm.IsLoadingSavingOrUpdating())
                AddExceptionMessage(message);
            else
                LogSingleErrorMessage(ex.ToString());
            Log($"EXCEPTION: {ex}\n");
        }

        public static void LogException(string message, Exception ex)
        {
            string exMessage = message + BuildExceptionMessage(ex);

            if (MainForm.IsLoadingSavingOrUpdating())
                AddExceptionMessage(exMessage);
            else
                LogSingleErrorMessage(ex.ToString());
            Log($"EXCEPTION: {ex}\n");
        }

        public static void LogAdditionalException(Exception ex)
        {
            string message = BuildExceptionMessage(ex);

            AddAdditionalExceptionMessage(message);
            Log($"ADDITIONAL EXCEPTION: {ex}\n");
        }

        public static void LogException(EnumLocKey enumLocKey, Dictionary<string, string> replaceValues, Exception ex)
        {
            AddExceptionMessage(GuiLocManager.GetLoc(enumLocKey, replaceValues));
            Log($"EXCEPTION: {enumLocKey}, Values: {Utils.DictionaryToString(replaceValues)}, Exception: {ex}\n");
        }

        private static string AssembleLayeredPrefix(EnumLocKey enumLocKey, LinkedLayer currentLayer, out Dictionary<string, string> replaceValues)
        {
            string filePath = null;
            string blockLayeredPath = null;

            currentLayer?.AssembleLayeredPath(ref filePath, ref blockLayeredPath);

            replaceValues = new Dictionary<string, string> {
                { "{filePath}", filePath },
                { "{blockLayeredPath}", blockLayeredPath }
            };

            return GuiLocManager.GetLoc(enumLocKey, replaceValues);
        }

        public static void LogLayeredWarning(LinkedLayer currentLayer, EnumLocKey enumLocKey)
            => LogLayeredWarning(currentLayer, enumLocKey, null);
        public static void LogLayeredWarning(LinkedLayer prevLayer, string currentLayer, EnumLocKey enumLocKey)
            => LogLayeredWarning(new LinkedLayer(prevLayer, currentLayer), enumLocKey, null);

        public static void LogLayeredWarning(LinkedLayer prevLayer, string currentLayer, EnumLocKey enumLocKey, Dictionary<string, string> replaceValues)
            => LogLayeredWarning(new LinkedLayer(prevLayer, currentLayer), enumLocKey, replaceValues);

        public static void LogLayeredWarning(LinkedLayer currentLayer, EnumLocKey enumLocKey, Dictionary<string, string> replaceValues)
        {
            string prefix = AssembleLayeredPrefix(EnumLocKey.WARNING_LAYERED_PREFIX, currentLayer, out Dictionary<string, string> prefixReplaceValues);
            string message = GuiLocManager.GetLoc(enumLocKey, replaceValues);
            AddWarningMessage(message + '\n' + prefix);
            Log($"WARNING: {enumLocKey}, Values: {Utils.DictionaryToString(replaceValues)}; Prefix values: {Utils.DictionaryToString(prefixReplaceValues)}");
        }
        public static void LogLayeredError(LinkedLayer currentLayer, EnumLocKey enumLocKey)
            => LogLayeredError(currentLayer, enumLocKey, null);
        public static void LogLayeredError(LinkedLayer prevLayer, string currentLayer, EnumLocKey enumLocKey)
            => LogLayeredError(new LinkedLayer(prevLayer, currentLayer), enumLocKey, null);

        public static void LogLayeredError(LinkedLayer prevLayer, string currentLayer, EnumLocKey enumLocKey, Dictionary<string, string> replaceValues)
            => LogLayeredError(new LinkedLayer(prevLayer, currentLayer), enumLocKey, replaceValues);

        public static void LogLayeredError(LinkedLayer currentLayer, EnumLocKey enumLocKey, Dictionary<string, string> replaceValues)
        {
            string prefix = AssembleLayeredPrefix(EnumLocKey.WARNING_LAYERED_PREFIX, currentLayer, out Dictionary<string, string> prefixReplaceValues);
            string message = GuiLocManager.GetLoc(enumLocKey, replaceValues);
            AddErrorMessage(message + '\n' + prefix);
            Log($"ERROR: {enumLocKey}, Values: {Utils.DictionaryToString(replaceValues)}; Prefix values: {Utils.DictionaryToString(prefixReplaceValues)}");
        }

        public static int WarningsCount
        {
            get
            {
                lock (_warningsSync)
                    return _warnings.Count;
            }
        }

        public static int ErrorsCount
        {
            get
            {
                lock (_errorsSync)
                    return _errors.Count;
            }
        }

        public static int ExceptionsCount
        {
            get
            {
                lock (_exceptionsSync)
                    return _exceptions.Count;
            }
        }

        public static void ClearAllWarningsErrorsAndExceptions()
        {
            lock (_warningsSync)
                _warnings.Clear();

            lock (_errorsSync)
                _errors.Clear();

            lock (_exceptionsSync)
                _exceptions.Clear();

            lock (_additionalExceptionsSync)
                _additionalExceptions.Clear();
        }

        public static void DisplayWarnings()
        {
            var warningsSnapshot = TakeSnapshotAndClear(_warningsSync, _warnings);
            if (warningsSnapshot.Count == 0)
                return;

            string title = GuiLocManager.GetLoc(EnumLocKey.FOUND_WARNINGS_FORM_TITLE);
            string mainText = GuiLocManager.GetLoc(
                EnumLocKey.FOUND_WARNINGS_COUNT,
                new Dictionary<string, string> {
                    { "{count}", "" + warningsSnapshot.Count}
                }
            );
            string richText = string.Join("\n\n", warningsSnapshot);
            CreateTextBoxMessageForm(title, mainText, richText);
        }

        public static void DisplayErrors()
        {
            var errorsSnapshot = TakeSnapshotAndClear(_errorsSync, _errors);
            if (errorsSnapshot.Count == 0)
                return;

            string title = GuiLocManager.GetLoc(EnumLocKey.FOUND_ERRORS_FORM_TITLE);
            string mainText = GuiLocManager.GetLoc(
                EnumLocKey.FOUND_ERRORS_COUNT,
                new Dictionary<string, string> {
                    { "{count}", "" + errorsSnapshot.Count}
                }
            );
            string richText = string.Join("\n\n", errorsSnapshot);
            CreateTextBoxMessageForm(title, mainText, richText);
        }

        public static void DisplayExceptions()
        {
            var exceptionsSnapshot = TakeSnapshotAndClear(_exceptionsSync, _exceptions);
            if (exceptionsSnapshot.Count == 0)
                return;

            string title = GuiLocManager.GetLoc(EnumLocKey.FOUND_EXCEPTIONS_FORM_TITLE);
            string mainText = GuiLocManager.GetLoc(
                    EnumLocKey.FOUND_EXCEPTIONS_COUNT,
                    new Dictionary<string, string> {
                        { "{count}", $"{exceptionsSnapshot.Count}" },
                        { "{logFilepath}", $"{logFilePath}" }
                    }
                );
            string richText = string.Join("\n\n", exceptionsSnapshot);
            CreateTextBoxMessageForm(title, mainText, richText);
        }

        public static void DisplayAdditionalExceptions()
        {
            var additionalExceptionsSnapshot = TakeSnapshotAndClear(_additionalExceptionsSync, _additionalExceptions);
            if (additionalExceptionsSnapshot.Count == 0)
                return;

            string title = GuiLocManager.GetLoc(EnumLocKey.FOUND_ADDITIONAL_EXCEPTIONS_FORM_TITLE);
            string mainText = GuiLocManager.GetLoc(
                    EnumLocKey.FOUND_ADDITIONAL_EXCEPTIONS_COUNT,
                    new Dictionary<string, string> {
                        { "{exceptionsCount}", $"{additionalExceptionsSnapshot.Count}" },
                        { "{logFilepath}", $"{logFilePath}" }
                    }
                );
            string richText = string.Join("\n\n", additionalExceptionsSnapshot);
            CreateTextBoxMessageForm(title, mainText, richText);
        }

        private static void CreateTextBoxMessageForm(string title, string mainText, string richText)
        {
            TextBoxMessageForm.CreateTasked(title, mainText, richText, true, form =>
            {
                lock (_textBoxFormsSync)
                    _textBoxMessageForms.Add(form);
            });
        }

        public static void CloseAllTextBoxMessageForms()
        {
            List<TextBoxMessageForm> formsSnapshot;
            lock (_textBoxFormsSync)
            {
                formsSnapshot = new List<TextBoxMessageForm>(_textBoxMessageForms);
                _textBoxMessageForms.Clear();
            }

            foreach (var form in formsSnapshot)
                TryOrLog(() =>
                {
                    if (!form.IsClosed)
                        form.InvokeAction(() => form.Close());
                });
        }

        public static void LogTime(string title, Action action)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            action();
            stopwatch.Stop();
            Log($"{title}: {stopwatch.ElapsedMilliseconds} ms");
        }

        public static void TryOrLog(Action action)
        {
            try
            {
                action();
            }
            catch (CancelActionException _)
            { }
            catch (Exception ex)
            {
                LogException(ex);
            }
        }

        public static void TryOrCatch(Action tryAction, Action<Exception> catchAction)
        {
            try
            {
                tryAction();
            }
            catch (CancelActionException _)
            { }
            catch (Exception ex)
            {
                catchAction(ex);
            }
        }

        public static void TryOrLog(Action action, Action onFinal)
        {
            try
            {
                action();
            }
            catch (CancelActionException _)
            { }
            catch (Exception ex)
            {
                LogException(ex);
            }
            finally
            {
                TryOrLog(onFinal);
            }
        }

        public static void WrapTokenCallbackExceptions(string layerName, Action action)
        {
            try
            {
                action();
            }
            catch (CancelActionException _)
            { }
            catch (Exception ex)
            {
                throw new Exception(layerName + " => ", ex);
            }
        }

        public static void WrapException<T>(string layerName, T ex) where T : Exception
            => throw new Exception(layerName + " => ", ex);

        public static void MeasureElapsedMS(string prefix, Action action)
        {
            var stopwatch = Stopwatch.StartNew();
            action.Invoke();
            stopwatch.Stop();
            Logger.Log(prefix + stopwatch.ElapsedMilliseconds + " ms");
        }

        private static string BuildExceptionMessage(Exception ex)
        {
            if (ex == null)
                return string.Empty;

            var message = ex.Message ?? string.Empty;
            var tempEx = ex.InnerException;

            while (tempEx != null)
            {
                if (!string.IsNullOrWhiteSpace(tempEx.Message))
                    message += " " + tempEx.Message;

                tempEx = tempEx.InnerException;
            }

            return message.Trim();
        }

        private static void AddWarningMessage(string message)
        {
            lock (_warningsSync)
                _warnings.Add(message);
        }

        private static void AddErrorMessage(string message)
        {
            lock (_errorsSync)
                _errors.Add(message);
        }

        private static void AddExceptionMessage(string message)
        {
            lock (_exceptionsSync)
                _exceptions.Add(message);
        }

        private static void AddAdditionalExceptionMessage(string message)
        {
            lock (_additionalExceptionsSync)
                _additionalExceptions.Add(message);
        }

        private static List<string> TakeSnapshotAndClear(object syncRoot, List<string> source)
        {
            lock (syncRoot)
            {
                if (source.Count == 0)
                    return new List<string>(0);

                var snapshot = new List<string>(source);
                source.Clear();
                return snapshot;
            }
        }
    }

    public class LinkedLayer
    {
        public LinkedLayer Prev { get; private set; }
        public string Name { get; private set; }
        public bool IsFilePath { get; private set; }

        public LinkedLayer(LinkedLayer prev, string name)
        {
            Prev = prev;
            Name = name;
        }

        public LinkedLayer(LinkedLayer prev, string name, bool isFilePath)
        {
            Prev = prev;
            Name = name;
            IsFilePath = isFilePath;
        }

        public void AssembleLayeredPath(ref string filePath, ref string layeredPath)
        {
            if (Prev != null)
            {
                Prev.AssembleLayeredPath(ref layeredPath, ref filePath);
                if (layeredPath == null || layeredPath.Length == 0) layeredPath = Name;
                else layeredPath += " => " + Name;
            }
            else if (IsFilePath) filePath = Name;
        }
    }
}
