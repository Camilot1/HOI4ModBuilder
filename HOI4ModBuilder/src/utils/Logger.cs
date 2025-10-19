
using HOI4ModBuilder.src.forms;
using HOI4ModBuilder.src.managers;
using HOI4ModBuilder.src.Pdoxcl2Sharp;
using Pdoxcl2Sharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HOI4ModBuilder.src.utils
{
    class Logger
    {
        public static readonly string logDirPath = FileManager.AssembleFolderPath(new string[] { "logs" });
        public static readonly string logFilePath = logDirPath + "latest.log";
        public static readonly string version = "Alpha 0.2.9 Pre-Release 3";
        public static readonly int versionId = 12;

        private static List<string> _warnings = new List<string>();
        private static List<string> _errors = new List<string>();
        private static List<string> _exceptions = new List<string>();
        private static List<string> _additionalExceptions = new List<string>();

        private static readonly List<TextBoxMessageForm> _textBoxMessageForms = new List<TextBoxMessageForm>();

        public static void Init()
        {
            try
            {
                if (!Directory.Exists(logDirPath))
                    Directory.CreateDirectory(logDirPath);

                File.Delete(logFilePath);
                Log($"Program version: {version}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to remove latels.log file: {ex}");
            }
        }

        public static void Log(string message)
        {
            string logEntry = $"[{DateTime.Now}]: {message}{Environment.NewLine}";
            try
            {
                File.AppendAllText(logFilePath, logEntry);
                Console.WriteLine(message);
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
            Task.Run(() => MessageBox.Show(
                message,
                GuiLocManager.GetLoc(EnumLocKey.ERROR_HAS_OCCURED),
                MessageBoxButtons.OK,
                MessageBoxIcon.Error
            ));
            Log(message);
        }

        public static void LogSingleInfoMessage(EnumLocKey enumLocKey)
            => LogSingleInfoMessage(GuiLocManager.GetLoc(enumLocKey));

        public static void LogSingleInfoMessage(EnumLocKey enumLocKey, Dictionary<string, string> replaceValues)
            => LogSingleInfoMessage(GuiLocManager.GetLoc(enumLocKey, replaceValues));

        public static void LogSingleInfoMessage(string message)
        {
            Task.Run(() => MessageBox.Show(
                message,
                GuiLocManager.GetLoc(EnumLocKey.INFORMATION_MESSAGE_TITLE),
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            ));
            Log(message);
        }

        internal static void ShowMessageOnUiThread(string message, string caption, MessageBoxIcon icon)
        {
            void ShowBox()
            {
                MessageBox.Show(message, caption, MessageBoxButtons.OK, icon);
            }

            var mainForm = MainForm.Instance;
            if (mainForm == null || mainForm.IsDisposed)
            {
                ShowBox();
                return;
            }

            try
            {
                if (mainForm.InvokeRequired)
                {
                    if (!mainForm.IsHandleCreated)
                    {
                        ShowBox();
                        return;
                    }

                    mainForm.BeginInvoke((MethodInvoker)(() => ShowBox()));
                }
                else
                {
                    ShowBox();
                }
            }
            catch (ObjectDisposedException)
            {
                ShowBox();
            }
            catch (InvalidOperationException)
            {
                ShowBox();
            }
        }

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
            _warnings.Add(GuiLocManager.GetLoc(enumLocKey, replaceValues));
            Log($"WARNING: {enumLocKey}, Values: {Utils.DictionaryToString(replaceValues)}");
        }

        public static void LogWarning(string message)
        {
            _warnings.Add(message);
            Log($"WARNING: {message}");
        }

        public static void LogError(EnumLocKey enumLocKey, Dictionary<string, string> replaceValues)
        {
            _errors.Add(GuiLocManager.GetLoc(enumLocKey, replaceValues));
            Log($"ERROR: {enumLocKey}, Values: {Utils.DictionaryToString(replaceValues)}");
        }

        public static void LogError(EnumLocKey enumLocKey, Dictionary<string, string> replaceValues, string additionalText)
        {
            _errors.Add($"{GuiLocManager.GetLoc(enumLocKey, replaceValues)} {additionalText}");
            Log($"ERROR: {enumLocKey}, Values: {Utils.DictionaryToString(replaceValues)}");
        }
        public static void LogWarning(EnumLocKey enumLocKey, Dictionary<string, string> replaceValues, string additionalText)
        {
            _warnings.Add($"{GuiLocManager.GetLoc(enumLocKey, replaceValues)} {additionalText}");
            Log($"WARNING: {enumLocKey}, Values: {Utils.DictionaryToString(replaceValues)}");
        }

        public static void LogExceptionAsError(EnumLocKey enumLocKey, Dictionary<string, string> replaceValues, Exception ex)
        {
            string message = ex.Message;
            var tempEx = ex.InnerException;

            while (tempEx != null)
            {
                message += " " + tempEx.Message;
                tempEx = tempEx.InnerException;
            }

            LogError(enumLocKey, replaceValues, message);
            Log($"EXCEPTION AS ERROR: {enumLocKey}, Values: {Utils.DictionaryToString(replaceValues)}, Exception: {ex}\n");
        }

        public static void LogExceptionAsWarning(EnumLocKey enumLocKey, Dictionary<string, string> replaceValues, Exception ex)
        {
            string message = ex.Message;
            var tempEx = ex.InnerException;

            while (tempEx != null)
            {
                message += " " + tempEx.Message;
                tempEx = tempEx.InnerException;
            }

            LogWarning(enumLocKey, replaceValues, message);
            Log($"EXCEPTION AS WARNING: {enumLocKey}, Values: {Utils.DictionaryToString(replaceValues)}, Exception: {ex}\n");
        }

        public static void LogException(Exception ex)
        {
            string message = ex.Message;
            var tempEx = ex.InnerException;

            while (tempEx != null)
            {
                message += " " + tempEx.Message;
                tempEx = tempEx.InnerException;
            }

            if (MainForm.IsLoadingOrSaving)
                _exceptions.Add(message);
            else
                LogSingleErrorMessage(ex.ToString());
            Log($"EXCEPTION: {ex}\n");
        }

        public static void LogException(string message, Exception ex)
        {
            string exMessage = message + ex.Message;
            var tempEx = ex.InnerException;

            while (tempEx != null)
            {
                exMessage += " " + tempEx.Message;
                tempEx = tempEx.InnerException;
            }

            if (MainForm.IsLoadingOrSaving)
                _exceptions.Add(message);
            else
                LogSingleErrorMessage(ex.ToString());
            Log($"EXCEPTION: {ex}\n");
        }

        public static void LogAdditionalException(Exception ex)
        {
            string message = ex.Message;
            var tempEx = ex.InnerException;

            while (tempEx != null)
            {
                message += " " + tempEx.Message;
                tempEx = tempEx.InnerException;
            }

            _additionalExceptions.Add(message);
            Log($"ADDITIONAL EXCEPTION: {ex}\n");
        }

        public static void LogException(EnumLocKey enumLocKey, Dictionary<string, string> replaceValues, Exception ex)
        {
            _exceptions.Add(GuiLocManager.GetLoc(enumLocKey, replaceValues));
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
            _warnings.Add(message + '\n' + prefix);
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
            _errors.Add(message + '\n' + prefix);
            Log($"WARNING: {enumLocKey}, Values: {Utils.DictionaryToString(replaceValues)}; Prefix values: {Utils.DictionaryToString(prefixReplaceValues)}");
        }

        public static int WarningsCount => _warnings.Count;
        public static int ErrorsCount => _errors.Count;
        public static int ExceptionsCount => _exceptions.Count;

        public static void DisplayWarnings()
        {
            if (_warnings.Count == 0)
                return;

            string title = GuiLocManager.GetLoc(EnumLocKey.FOUND_WARNINGS_FORM_TITLE);
            string mainText = GuiLocManager.GetLoc(
                EnumLocKey.FOUND_WARNINGS_COUNT,
                new Dictionary<string, string> {
                    { "{count}", "" + _warnings.Count}
                }
            );
            string richText = string.Join("\n\n", _warnings);
            Task.Run(() => TextBoxMessageForm.CreateTasked(title, mainText, richText, true, _textBoxMessageForms));
            _warnings = new List<string>();
        }

        public static void DisplayErrors()
        {
            if (_errors.Count == 0)
                return;

            string title = GuiLocManager.GetLoc(EnumLocKey.FOUND_ERRORS_FORM_TITLE);
            string mainText = GuiLocManager.GetLoc(
                EnumLocKey.FOUND_ERRORS_COUNT,
                new Dictionary<string, string> {
                    { "{count}", "" + _errors.Count}
                }
            );
            string richText = string.Join("\n\n", _errors);
            Task.Run(() => TextBoxMessageForm.CreateTasked(title, mainText, richText, true, _textBoxMessageForms));
            _errors = new List<string>();
        }

        public static void DisplayExceptions()
        {
            if (_exceptions.Count == 0)
                return;

            string title = GuiLocManager.GetLoc(EnumLocKey.FOUND_EXCEPTIONS_FORM_TITLE);
            string mainText = GuiLocManager.GetLoc(
                    EnumLocKey.FOUND_EXCEPTIONS_COUNT,
                    new Dictionary<string, string> {
                        { "{count}", $"{_exceptions.Count}" },
                        { "{logFilepath}", $"{logFilePath}" }
                    }
                );
            string richText = string.Join("\n\n", _exceptions);
            Task.Run(() => TextBoxMessageForm.CreateTasked(title, mainText, richText, true, _textBoxMessageForms));
            _exceptions = new List<string>();
        }

        public static void DisplayAdditionalExceptions()
        {
            if (_additionalExceptions.Count == 0) return;

            string title = GuiLocManager.GetLoc(EnumLocKey.FOUND_ADDITIONAL_EXCEPTIONS_FORM_TITLE);
            string mainText = GuiLocManager.GetLoc(
                    EnumLocKey.FOUND_ADDITIONAL_EXCEPTIONS_COUNT,
                    new Dictionary<string, string> {
                        { "{exceptionsCount}", $"{_additionalExceptions.Count}" },
                        { "{logFilepath}", $"{logFilePath}" }
                    }
                );
            string richText = string.Join("\n\n", _additionalExceptions);
            Task.Run(() => TextBoxMessageForm.CreateTasked(title, mainText, richText, true, _textBoxMessageForms));
            _additionalExceptions = new List<string>();
        }

        public static void CloseAllTextBoxMessageForms()
        {
            foreach (var form in _textBoxMessageForms)
                TryOrLog(() =>
                {
                    if (!form.IsClosed)
                        form.InvokeAction(() => form.Close());
                });
            _textBoxMessageForms.Clear();
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
