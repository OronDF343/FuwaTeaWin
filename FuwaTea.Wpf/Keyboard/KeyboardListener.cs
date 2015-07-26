using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Input;
using System.Windows.Threading;

// ReSharper disable FieldCanBeMadeReadOnly.Local
namespace FuwaTea.Wpf.Keyboard
{
    // Credit: Ciantic https://gist.github.com/Ciantic/471698
    // resharped
    /// <summary>
    /// Listens keyboard globally.
    ///
    /// <remarks>Uses WH_KEYBOARD_LL.</remarks>
    /// </summary>
    public sealed class KeyboardListener : IDisposable
    {
        /// <summary>
        /// Creates global keyboard listener.
        /// </summary>
        public KeyboardListener()
        {
            // Dispatcher thread handling the KeyDown/KeyUp events.
            _dispatcher = Dispatcher.CurrentDispatcher;
            // We have to store the LowLevelKeyboardProc, so that it is not garbage collected runtime
            _hookedLowLevelKeyboardProc = LowLevelKeyboardProc;

            // Set the hook
            _hookId = InterceptKeys.SetHook(_hookedLowLevelKeyboardProc);
            _enabled = true;

            // Assign the asynchronous callback event
            _hookedKeyboardCallbackAsync = KeyboardListener_KeyboardCallbackAsync;
        }

        private Dispatcher _dispatcher;

        /// <summary>
        /// Destroys global keyboard listener.
        /// </summary>
        ~KeyboardListener()
        {
            Dispose();
        }

        /// <summary>
        /// Fired when any of the keys is pressed down.
        /// </summary>
        public event RawKeyEventHandler KeyDown;

        /// <summary>
        /// Fired when any of the keys is released.
        /// </summary>
        public event RawKeyEventHandler KeyUp;

        #region Inner workings

        /// <summary>
        /// Hook ID
        /// </summary>
        private IntPtr _hookId = IntPtr.Zero;

        /// <summary>
        /// Asynchronous callback hook.
        /// </summary>
        /// <param name="character">Character</param>
        /// <param name="keyEvent">Keyboard event</param>
        /// <param name="vkCode">VKCode</param>
        private delegate void KeyboardCallbackAsync(InterceptKeys.KeyEvent keyEvent, int vkCode, string character);

        /// <summary>
        /// Actual callback hook.
        ///
        /// <remarks>Calls asynchronously the asyncCallback.</remarks>
        /// </summary>
        /// <param name="nCode"></param>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.NoInlining)]
        private IntPtr LowLevelKeyboardProc(int nCode, UIntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)
                if (wParam.ToUInt32() == (int)InterceptKeys.KeyEvent.WM_KEYDOWN ||
                wParam.ToUInt32() == (int)InterceptKeys.KeyEvent.WM_KEYUP /*||
                wParam.ToUInt32() == (int)InterceptKeys.KeyEvent.WM_SYSKEYDOWN ||
                wParam.ToUInt32() == (int)InterceptKeys.KeyEvent.WM_SYSKEYUP*/)
                {
                    // Captures the character(s) pressed only on WM_KEYDOWN
                    var chars = InterceptKeys.VkCodeToString((uint)Marshal.ReadInt32(lParam),
                        (wParam.ToUInt32() == (int)InterceptKeys.KeyEvent.WM_KEYDOWN /*||
                         wParam.ToUInt32() == (int)InterceptKeys.KeyEvent.WM_SYSKEYDOWN*/));

                    _hookedKeyboardCallbackAsync.BeginInvoke((InterceptKeys.KeyEvent)wParam.ToUInt32(), Marshal.ReadInt32(lParam), chars, null, null);
                }
            return NativeMethods.CallNextHookEx(_hookId, nCode, wParam, lParam);
        }

        /// <summary>
        /// Event to be invoked asynchronously (BeginInvoke) each time key is pressed.
        /// </summary>
        private KeyboardCallbackAsync _hookedKeyboardCallbackAsync;

        /// <summary>
        /// Contains the hooked callback in runtime.
        /// </summary>
        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        private InterceptKeys.LowLevelKeyboardProc _hookedLowLevelKeyboardProc;

        /// <summary>
        /// HookCallbackAsync procedure that calls accordingly the KeyDown or KeyUp events.
        /// </summary>
        /// <param name="keyEvent">Keyboard event</param>
        /// <param name="vkCode">VKCode</param>
        /// <param name="character">Character as string.</param>
        void KeyboardListener_KeyboardCallbackAsync(InterceptKeys.KeyEvent keyEvent, int vkCode, string character)
        {
            switch (keyEvent)
            {
                // KeyDown events
                case InterceptKeys.KeyEvent.WM_KEYDOWN:
                    if (KeyDown != null)
                        _dispatcher.BeginInvoke(new RawKeyEventHandler(KeyDown), this, new RawKeyEventArgs(vkCode, false, character));
                    break;
                case InterceptKeys.KeyEvent.WM_SYSKEYDOWN:
                    if (KeyDown != null)
                        _dispatcher.BeginInvoke(new RawKeyEventHandler(KeyDown), this, new RawKeyEventArgs(vkCode, true, character));
                    break;

                // KeyUp events
                case InterceptKeys.KeyEvent.WM_KEYUP:
                    if (KeyUp != null)
                        _dispatcher.BeginInvoke(new RawKeyEventHandler(KeyUp), this, new RawKeyEventArgs(vkCode, false, character));
                    break;
                case InterceptKeys.KeyEvent.WM_SYSKEYUP:
                    if (KeyUp != null)
                        _dispatcher.BeginInvoke(new RawKeyEventHandler(KeyUp), this, new RawKeyEventArgs(vkCode, true, character));
                    break;
            }
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        /// Disposes the hook.
        /// <remarks>This call is required as it calls the UnhookWindowsHookEx.</remarks>
        /// </summary>
        public void Dispose()
        {
            NativeMethods.UnhookWindowsHookEx(_hookId);
        }

        #endregion

        private bool _enabled;
        public bool IsEnabled
        {
            get { return _enabled; }
            set
            {
                if (_enabled && !value)
                    NativeMethods.UnhookWindowsHookEx(_hookId);
                else if (!_enabled && value)
                    _hookId = InterceptKeys.SetHook(_hookedLowLevelKeyboardProc);
                _enabled = value;
            }
        }
    }

    /// <summary>
    /// Raw KeyEvent arguments.
    /// </summary>
    public class RawKeyEventArgs : EventArgs
    {
        /// <summary>
        /// VKCode of the key.
        /// </summary>
        public int VkCode;

        /// <summary>
        /// WPF Key of the key.
        /// </summary>
        public Key Key;

        /// <summary>
        /// Is the hitted key system key.
        /// </summary>
        public bool IsSysKey;

        /// <summary>
        /// Convert to string.
        /// </summary>
        /// <returns>Returns string representation of this key, if not possible empty string is returned.</returns>
        public override string ToString()
        {
            return Character;
        }

        /// <summary>
        /// Unicode character of key pressed.
        /// </summary>
        public string Character;

        /// <summary>
        /// Create raw keyevent arguments.
        /// </summary>
        /// <param name="vkCode"></param>
        /// <param name="isSysKey"></param>
        /// <param name="character">Character</param>
        public RawKeyEventArgs(int vkCode, bool isSysKey, string character)
        {
            VkCode = vkCode;
            IsSysKey = isSysKey;
            Character = character;
            Key = KeyInterop.KeyFromVirtualKey(vkCode);
        }

    }

    /// <summary>
    /// Raw keyevent handler.
    /// </summary>
    /// <param name="sender">sender</param>
    /// <param name="e">raw keyevent arguments</param>
    public delegate void RawKeyEventHandler(object sender, RawKeyEventArgs e);

    #region WINAPI Helper class
    /// <summary>
    /// Winapi Key interception helper class.
    /// </summary>
    internal static class InterceptKeys
    {
        public delegate IntPtr LowLevelKeyboardProc(int nCode, UIntPtr wParam, IntPtr lParam);

        public const int WH_KEYBOARD_LL = 13;

        /// <summary>
        /// Key event
        /// </summary>
        public enum KeyEvent
        {
            /// <summary>
            /// Key down
            /// </summary>
            WM_KEYDOWN = 256,

            /// <summary>
            /// Key up
            /// </summary>
            WM_KEYUP = 257,

            /// <summary>
            /// System key up
            /// </summary>
            WM_SYSKEYUP = 261,

            /// <summary>
            /// System key down
            /// </summary>
            WM_SYSKEYDOWN = 260
        }

        public static IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            using (var curProcess = Process.GetCurrentProcess())
            using (var curModule = curProcess.MainModule)
            {
                return NativeMethods.SetWindowsHookEx(WH_KEYBOARD_LL, proc, NativeMethods.GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        #region Convert VKCode to string
        // Note: Sometimes single VKCode represents multiple chars, thus string.
        // E.g. typing "^1" (notice that when pressing 1 the both characters appear,
        // because of this behavior, "^" is called dead key)

        private static uint _lastVkCode;
        private static uint _lastScanCode;
        private static byte[] _lastKeyState = new byte[255];
        private static bool _lastIsDead;

        /// <summary>
        /// Convert VKCode to Unicode.
        /// <remarks>isKeyDown is required for because of keyboard state inconsistencies!</remarks>
        /// </summary>
        /// <param name="vkCode">VKCode</param>
        /// <param name="isKeyDown">Is the key down event?</param>
        /// <returns>String representing single unicode character.</returns>
        public static string VkCodeToString(uint vkCode, bool isKeyDown)
        {
            // ToUnicodeEx needs StringBuilder, it populates that during execution.
            var sbString = new StringBuilder(5);

            var bKeyState = new byte[255];
            bool bKeyStateStatus;
            var isDead = false;

            // Gets the current windows window handle, threadID, processID
            var currentHWnd = NativeMethods.GetForegroundWindow();
            uint currentProcessId;
            var currentWindowThreadId = NativeMethods.GetWindowThreadProcessId(currentHWnd, out currentProcessId);

            // This programs Thread ID
            var thisProgramThreadId = NativeMethods.GetCurrentThreadId();

            // Attach to active thread so we can get that keyboard state
            if (NativeMethods.AttachThreadInput(thisProgramThreadId, currentWindowThreadId, true))
            {
                // Current state of the modifiers in keyboard
                bKeyStateStatus = NativeMethods.GetKeyboardState(bKeyState);

                // Detach
                NativeMethods.AttachThreadInput(thisProgramThreadId, currentWindowThreadId, false);
            }
            else
            {
                // Could not attach, perhaps it is this process?
                bKeyStateStatus = NativeMethods.GetKeyboardState(bKeyState);
            }

            // On failure we return empty string.
            if (!bKeyStateStatus)
                return "";

            // Gets the layout of keyboard
            var hkl = NativeMethods.GetKeyboardLayout(currentWindowThreadId);

            // Maps the virtual keycode
            var lScanCode = NativeMethods.MapVirtualKeyEx(vkCode, 0, hkl);

            // Keyboard state goes inconsistent if this is not in place. In other words, we need to call above commands in UP events also.
            if (!isKeyDown)
                return "";

            // Converts the VKCode to unicode
            var relevantKeyCountInBuffer = NativeMethods.ToUnicodeEx(vkCode, lScanCode, bKeyState, sbString, sbString.Capacity, 0, hkl);

            var ret = "";

            switch (relevantKeyCountInBuffer)
            {
                // Dead keys (^,`...)
                case -1:
                    isDead = true;

                    // We must clear the buffer because ToUnicodeEx messed it up, see below.
                    ClearKeyboardBuffer(vkCode, lScanCode, hkl);
                    break;

                case 0:
                    break;

                // Single character in buffer
                case 1:
                    ret = sbString[0].ToString(CultureInfo.InvariantCulture);
                    break;

                // Two or more (only two of them is relevant)
                default:
                    ret = sbString.ToString().Substring(0, 2);
                    break;
            }

            // We inject the last dead key back, since ToUnicodeEx removed it.
            // More about this peculiar behavior see e.g:
            // http://www.experts-exchange.com/Programming/System/Windows__Programming/Q_23453780.html
            // http://blogs.msdn.com/michkap/archive/2005/01/19/355870.aspx
            // http://blogs.msdn.com/michkap/archive/2007/10/27/5717859.aspx
            if (_lastVkCode != 0 && _lastIsDead)
            {
                var sbTemp = new StringBuilder(5);
                NativeMethods.ToUnicodeEx(_lastVkCode, _lastScanCode, _lastKeyState, sbTemp, sbTemp.Capacity, 0, hkl);
                _lastVkCode = 0;

                return ret;
            }
            // Save these
            _lastScanCode = lScanCode;
            _lastVkCode = vkCode;
            _lastIsDead = isDead;
            _lastKeyState = (byte[])bKeyState.Clone();

            return ret;
        }

        private static void ClearKeyboardBuffer(uint vk, uint sc, IntPtr hkl)
        {
            var sb = new StringBuilder(10);
            int rc;
            do
            {
                var lpKeyStateNull = new Byte[255];
                rc = NativeMethods.ToUnicodeEx(vk, sc, lpKeyStateNull, sb, sb.Capacity, 0, hkl);
            } while (rc < 0);
        }
        #endregion
    }
    #endregion
}
