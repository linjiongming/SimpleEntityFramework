using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace SimpleEntityFramework.Infrastracture
{
    public class Clipboard
    {
        [DllImport("User32")]
        public static extern bool OpenClipboard(IntPtr hWndNewOwner);

        [DllImport("User32")]
        public static extern bool CloseClipboard();

        [DllImport("User32")]
        public static extern bool EmptyClipboard();

        [DllImport("User32")]
        public static extern bool IsClipboardFormatAvailable(int format);

        [DllImport("User32")]
        public static extern IntPtr GetClipboardData(int uFormat);

        [DllImport("User32", CharSet = CharSet.Unicode)]
        public static extern IntPtr SetClipboardData(int uFormat, IntPtr hMem);

        /// <summary>
        /// 向剪贴板中添加文本
        /// </summary>
        /// <param name="text">文本</param>
        public static void SetText(string text)
        {
            if (!OpenClipboard(IntPtr.Zero))
            {
                SetText(text);
                return;
            }
            EmptyClipboard();
            try
            {
                SetClipboardData(13, Marshal.StringToHGlobalUni(text));
            }
            catch { throw; }
            finally
            {
                CloseClipboard();
            }
        }
    }
}