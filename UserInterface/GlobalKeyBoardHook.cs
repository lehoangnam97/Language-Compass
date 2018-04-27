using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Input;


namespace GlobalKeyBoardHook
{
    public class LowLevelKeyboardListener
    {

        #region declare system parameter 
        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        private const int WM_SYSKEYDOWN = 0x0104;
        #endregion

        #region window api setup function for hook procedure, all these are private and will use by another method
        //Setup hook procedure to hook chain
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);


        //Remove hook procedure which setup by setWindowsHookEx from hook chain
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);


        //Call the next hook procedure in hook chain 
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);


        //Get the name of your window application , the name = hInstance will be gotten from string module name
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);
        #endregion

        //create the delegate type LowlevelKeyboardProc which will use for this library to call back the event as an event
        public delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);
        //declare a delegate type LowlevelKeyBoardProc: _proc
        private LowLevelKeyboardProc _proc;


        //create event OnKeypressed
        public event EventHandler<KeyPressedArgs> OnKeyPressed;


        //hook ID in hookchain, if hook ID=zero / or null integer pointer, the hook is 
        private IntPtr _hookID = IntPtr.Zero;

        //Constructer of this class lowlevelkeyboardlistener, Constructer will assign hook call back method
        public LowLevelKeyboardListener()
        {
            _proc = HookCallback;
        }


        //Setup the hook (this will use at formMain)
        public void HookKeyboard()
        {
            _hookID = SetHook(_proc);
        }


        //public method use to unhook ( we will use it when window is closing)
        public void UnHookKeyboard()
        {
            UnhookWindowsHookEx(_hookID);
        }

        //private method called by HookKeyboard to assign the return value to hookID value
        private IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            //get the current using process
            using (Process curProcess = Process.GetCurrentProcess())
            {
                //get the main module of urrent using process
                using (ProcessModule curModule = curProcess.MainModule)
                {
                    //set hook to the current process main module
                    return SetWindowsHookEx(WH_KEYBOARD_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
                }
            }
        }


        private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            //nCode decide this procedure will be handled or not
            //When nCode smaller than 0, HookCallBack will call nexthook, else HookCallBack will conduct our hook

            //wParam: The identifier of the keyboard message. This parameter can be one of the following messages: WM_KEYDOWN, WM_KEYUP, WM_SYSKEYDOWN, or WM_SYSKEYUP.
            //lParam: a pointer to keyboard hook structure, aka KeyboardHookProc
            if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN || wParam == (IntPtr)WM_SYSKEYDOWN)
            {
                //Conver lParam(type pointer) to ineger to get virtual key code
                int vkCode = Marshal.ReadInt32(lParam);


                if (OnKeyPressed != null)
                {
                    //KeyInterop: the class from system Window input, provide method to convert win32 virtualKey to WPF Key enumeration

                    //Because OnKeyPressed event inherit from EventAgrs, it will have the same delegate structure (object sender, EventAgrument e)

                    OnKeyPressed(this, new KeyPressedArgs(KeyInterop.KeyFromVirtualKey(vkCode)));
                }
            }
            //
            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }
    }

    #region Create KeyPressEvent
    //Create Keypress Event , inherit from event args which can catch keypress method for WPF
    //Because WPF does not have keypress method, it just have keydown
    public class KeyPressedArgs : EventArgs
    {
        //Key from system.Windows.Input key
        public Key KeyPressed { get; private set; }

        public KeyPressedArgs(Key key)
        {
            KeyPressed = key;
        }
    }
    #endregion
}
