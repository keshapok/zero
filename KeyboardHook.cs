using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

public static class KeyboardHook
{
    private static IntPtr _hookID = IntPtr.Zero;

    private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

    private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
    {
        if (nCode >= 0 && wParam == (IntPtr)0x0100)
        {
            int vkCode = Marshal.ReadInt32(lParam);
            if ((Keys)vkCode == Keys.F10)
            {
                MainForm mainForm = Application.OpenForms["MainForm"] as MainForm;
                if (mainForm != null)
                {
                    mainForm._botActive = !mainForm._botActive;
                    Console.WriteLine($"[F10] Бот: {(mainForm._botActive ? "Активен" : "Неактивен")}");
                }
            }
        }
        return CallNextHookEx(_hookID, 0, wParam, lParam);
    }

    public static void Start()
    {
        _hookID = SetHook(HookCallback);
    }

    private static IntPtr SetHook(LowLevelKeyboardProc proc)
    {
        IntPtr hInstance = GetModuleHandle(null);
        return SetWindowsHookEx(13, proc, hInstance, 0);
    }

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc callback, IntPtr hInstance, uint threadId);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr GetModuleHandle(string lpModuleName);
}
