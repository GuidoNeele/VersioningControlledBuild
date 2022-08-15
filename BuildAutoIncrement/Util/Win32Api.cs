/*
 * Filename:    Win32Api.cs
 * Product:     Versioning Controlled Build
 * Solution:    BuildAutoIncrement
 * Project:     Shared
 * Description: Windows API methods, constants and structures.
 * Copyright:   Julijan Šribar, 2004-2013
 * 
 * This software is provided 'as-is', without any express or implied
 * warranty.  In no event will the author(s) be held liable for any damages
 * arising from the use of this software.
 *
 * Permission is granted to anyone to use this software for any purpose,
 * including commercial applications, and to alter it and redistribute it
 * freely, subject to the following restrictions:
 *
 * 1. The origin of this software must not be misrepresented; you must not
 *    claim that you wrote the original software. If you use this software
 *    in a product, an acknowledgment in the product documentation would be
 *    appreciated but is not required.
 * 2. Altered source versions must be plainly marked as such, and must not be
 *    misrepresented as being the original software.
 * 3. This notice may not be removed or altered from any source distribution.
 */
using System;
using System.Runtime.InteropServices;
using System.Text;

namespace BuildAutoIncrement {
	/// <summary>
	///   Windows API methods, constants and structures.
	/// </summary>
    public struct Win32Api
    {

        #region shell32.dll

        [DllImport("shell32.dll")]
        public static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, ref SHFILEINFO psfi, uint cbSizeFileInfo, uint uFlags);

        #endregion // shell32.dll

        #region user32.dll

        [DllImport("user32.dll")]
        public static extern int AppendMenu(int hMenu, UInt32 uFlags, UInt32 uIDNewItem, string lpNewItem);

        [DllImport("user32.dll")]
        public static extern IntPtr CreateIconFromResourceEx(byte[] pbIconBits, uint cbIconBits, bool fIcon, uint dwVersion, int cxDesired, int cyDesired, uint uFlags);

        [DllImport("user32.dll")] 
        public static extern int EnableWindow(int hWnd, bool bEnable);
        
        [DllImport("user32.dll")]
        public static extern IntPtr GetDC(IntPtr HWnd);

        [DllImport("user32.dll")]
        public static extern bool GetScrollInfo(IntPtr hwnd, int fnBar, ref SCROLLINFO lpsi);

        [DllImport("user32.dll")]
        public static extern int GetSystemMenu(IntPtr hWnd, bool bRevert);

        [DllImport("user32.dll")] 
        public static extern int GetWindowRect(IntPtr hwnd, ref RECT lpRect);

        [DllImport("user32.dll")]
        public static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

        [DllImport("user32.dll")]
        public static extern int ScrollWindowEx(IntPtr hWnd, int dx, int dy, IntPtr prcScroll, IntPtr prcClip, IntPtr hrgnUpdate, IntPtr prcUpdate, uint flags);

        [DllImport("user32.dll")]
        public static extern IntPtr SendMessage(IntPtr hWnd, int msg, int wParam, int lParam);

        [DllImport("user32.dll")]
        public static extern void SendMessage(IntPtr hWnd, int msg, int wParam, ref RECT lParam);

        [DllImport("user32.dll")]
        public static extern void SendMessage(IntPtr hWnd, HDM msg, int wParam, ref HD_HITTESTINFO hti);

        [DllImport("user32.dll")]
        public static extern int SetScrollInfo(IntPtr hwnd, int fnBar, [In] ref SCROLLINFO lpsi, bool fRedraw);

        [DllImport("user32.dll")] 
        public static extern int UpdateWindow(IntPtr HWnd);

        [DllImport("user32.dll")] 
        public static extern int ValidateRect(IntPtr hwnd, ref RECT lpRect);

        #endregion // user32.dll

        #region gdi32.dll

        [DllImport("gdi32.dll")]
        public static extern int GetDeviceCaps(IntPtr hDC, int index);

        #endregion // gdi32.dll

        #region comctl32.dll

        [DllImport("comctl32.dll")]
        public static extern int DllGetVersion(ref DLLVersionInfo version);

        #endregion // comctl32.dll

        #region uxtheme.dll

        [DllImport("uxtheme.dll", ExactSpelling=true, CharSet=CharSet.Unicode)]
        public static extern IntPtr OpenThemeData(IntPtr hWnd, string classList);

        [DllImport("UxTheme.dll", ExactSpelling=true)]
        public extern static Int32 CloseThemeData(IntPtr hTheme);

        [DllImport("UxTheme.dll", ExactSpelling=true)]
        public extern static Int32 DrawThemeBackground(IntPtr hTheme, IntPtr hdc, int iPartId, int iStateId, ref RECT pRect, ref RECT pClipRect);

        [DllImport("UxTheme.dll")]
        public static extern bool IsAppThemed();

        [DllImport("UxTheme.dll")]
        public static extern bool IsThemeActive();

        #endregion // uxtheme.dll

        #region Structures

        [StructLayout(LayoutKind.Sequential)]
        public struct SHFILEINFO {
            public IntPtr hIcon;
            public IntPtr iIcon;
            public uint dwAttributes;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szDisplayName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            public string szTypeName;
        };

        [StructLayout(LayoutKind.Sequential)]
        public struct DLLVersionInfo {
            public int cbSize;
            public int dwMajorVersion;
            public int dwMinorVersion;
            public int dwBuildNumber;
            public int dwPlatformID;
        }

        #endregion // Structures

        #region Windows Messages
        public enum WM {
            NULL                   = 0x0000,
            CREATE                 = 0x0001,
            DESTROY                = 0x0002,
            MOVE                   = 0x0003,
            SIZE                   = 0x0005,
            ACTIVATE               = 0x0006,
            SETFOCUS               = 0x0007,
            KILLFOCUS              = 0x0008,
            ENABLE                 = 0x000A,
            SETREDRAW              = 0x000B,
            SETTEXT                = 0x000C,
            GETTEXT                = 0x000D,
            GETTEXTLENGTH          = 0x000E,
            PAINT                  = 0x000F,
            CLOSE                  = 0x0010,
            QUERYENDSESSION        = 0x0011,
            QUIT                   = 0x0012,
            QUERYOPEN              = 0x0013,
            ERASEBKGND             = 0x0014,
            SYSCOLORCHANGE         = 0x0015,
            ENDSESSION             = 0x0016,
            SHOWWINDOW             = 0x0018,
            CTLCOLOR               = 0x0019,
            WININICHANGE           = 0x001A,
            SETTINGCHANGE          = 0x001A,
            DEVMODECHANGE          = 0x001B,
            ACTIVATEAPP            = 0x001C,
            FONTCHANGE             = 0x001D,
            TIMECHANGE             = 0x001E,
            CANCELMODE             = 0x001F,
            SETCURSOR              = 0x0020,
            MOUSEACTIVATE          = 0x0021,
            CHILDACTIVATE          = 0x0022,
            QUEUESYNC              = 0x0023,
            GETMINMAXINFO          = 0x0024,
            PAINTICON              = 0x0026,
            ICONERASEBKGND         = 0x0027,
            NEXTDLGCTL             = 0x0028,
            SPOOLERSTATUS          = 0x002A,
            DRAWITEM               = 0x002B,
            MEASUREITEM            = 0x002C,
            DELETEITEM             = 0x002D,
            VKEYTOITEM             = 0x002E,
            CHARTOITEM             = 0x002F,
            SETFONT                = 0x0030,
            GETFONT                = 0x0031,
            SETHOTKEY              = 0x0032,
            GETHOTKEY              = 0x0033,
            QUERYDRAGICON          = 0x0037,
            COMPAREITEM            = 0x0039,
            GETOBJECT              = 0x003D,
            COMPACTING             = 0x0041,
            COMMNOTIFY             = 0x0044,
            WINDOWPOSCHANGING      = 0x0046,
            WINDOWPOSCHANGED       = 0x0047,
            POWER                  = 0x0048,
            COPYDATA               = 0x004A,
            CANCELJOURNAL          = 0x004B,
            NOTIFY                 = 0x004E,
            INPUTLANGCHANGEREQUEST = 0x0050,
            INPUTLANGCHANGE        = 0x0051,
            TCARD                  = 0x0052,
            HELP                   = 0x0053,
            USERCHANGED            = 0x0054,
            NOTIFYFORMAT           = 0x0055,
            CONTEXTMENU            = 0x007B,
            STYLECHANGING          = 0x007C,
            STYLECHANGED           = 0x007D,
            DISPLAYCHANGE          = 0x007E,
            GETICON                = 0x007F,
            SETICON                = 0x0080,
            NCCREATE               = 0x0081,
            NCDESTROY              = 0x0082,
            NCCALCSIZE             = 0x0083,
            NCHITTEST              = 0x0084,
            NCPAINT                = 0x0085,
            NCACTIVATE             = 0x0086,
            GETDLGCODE             = 0x0087,
            SYNCPAINT              = 0x0088,
            NCMOUSEMOVE            = 0x00A0,
            NCLBUTTONDOWN          = 0x00A1,
            NCLBUTTONUP            = 0x00A2,
            NCLBUTTONDBLCLK        = 0x00A3,
            NCRBUTTONDOWN          = 0x00A4,
            NCRBUTTONUP            = 0x00A5,
            NCRBUTTONDBLCLK        = 0x00A6,
            NCMBUTTONDOWN          = 0x00A7,
            NCMBUTTONUP            = 0x00A8,
            NCMBUTTONDBLCLK        = 0x00A9,
            KEYDOWN                = 0x0100,
            KEYUP                  = 0x0101,
            CHAR                   = 0x0102,
            DEADCHAR               = 0x0103,
            SYSKEYDOWN             = 0x0104,
            SYSKEYUP               = 0x0105,
            SYSCHAR                = 0x0106,
            SYSDEADCHAR            = 0x0107,
            KEYLAST                = 0x0108,
            IME_STARTCOMPOSITION   = 0x010D,
            IME_ENDCOMPOSITION     = 0x010E,
            IME_COMPOSITION        = 0x010F,
            IME_KEYLAST            = 0x010F,
            INITDIALOG             = 0x0110,
            COMMAND                = 0x0111,
            SYSCOMMAND             = 0x0112,
            TIMER                  = 0x0113,
            HSCROLL                = 0x0114,
            VSCROLL                = 0x0115,
            INITMENU               = 0x0116,
            INITMENUPOPUP          = 0x0117,
            MENUSELECT             = 0x011F,
            MENUCHAR               = 0x0120,
            ENTERIDLE              = 0x0121,
            MENURBUTTONUP          = 0x0122,
            MENUDRAG               = 0x0123,
            MENUGETOBJECT          = 0x0124,
            UNINITMENUPOPUP        = 0x0125,
            MENUCOMMAND            = 0x0126,
            CTLCOLORMSGBOX         = 0x0132,
            CTLCOLOREDIT           = 0x0133,
            CTLCOLORLISTBOX        = 0x0134,
            CTLCOLORBTN            = 0x0135,
            CTLCOLORDLG            = 0x0136,
            CTLCOLORSCROLLBAR      = 0x0137,
            CTLCOLORSTATIC         = 0x0138,
            MOUSEMOVE              = 0x0200,
            LBUTTONDOWN            = 0x0201,
            LBUTTONUP              = 0x0202,
            LBUTTONDBLCLK          = 0x0203,
            RBUTTONDOWN            = 0x0204,
            RBUTTONUP              = 0x0205,
            RBUTTONDBLCLK          = 0x0206,
            MBUTTONDOWN            = 0x0207,
            MBUTTONUP              = 0x0208,
            MBUTTONDBLCLK          = 0x0209,
            MOUSEWHEEL             = 0x020A,
            PARENTNOTIFY           = 0x0210,
            ENTERMENULOOP          = 0x0211,
            EXITMENULOOP           = 0x0212,
            NEXTMENU               = 0x0213,
            SIZING                 = 0x0214,
            CAPTURECHANGED         = 0x0215,
            MOVING                 = 0x0216,
            DEVICECHANGE           = 0x0219,
            MDICREATE              = 0x0220,
            MDIDESTROY             = 0x0221,
            MDIACTIVATE            = 0x0222,
            MDIRESTORE             = 0x0223,
            MDINEXT                = 0x0224,
            MDIMAXIMIZE            = 0x0225,
            MDITILE                = 0x0226,
            MDICASCADE             = 0x0227,
            MDIICONARRANGE         = 0x0228,
            MDIGETACTIVE           = 0x0229,
            MDISETMENU             = 0x0230,
            ENTERSIZEMOVE          = 0x0231,
            EXITSIZEMOVE           = 0x0232,
            DROPFILES              = 0x0233,
            MDIREFRESHMENU         = 0x0234,
            IME_SETCONTEXT         = 0x0281,
            IME_NOTIFY             = 0x0282,
            IME_CONTROL            = 0x0283,
            IME_COMPOSITIONFULL    = 0x0284,
            IME_SELECT             = 0x0285,
            IME_CHAR               = 0x0286,
            IME_REQUEST            = 0x0288,
            IME_KEYDOWN            = 0x0290,
            IME_KEYUP              = 0x0291,
            MOUSEHOVER             = 0x02A1,
            MOUSELEAVE             = 0x02A3,
            CUT                    = 0x0300,
            COPY                   = 0x0301,
            PASTE                  = 0x0302,
            CLEAR                  = 0x0303,
            UNDO                   = 0x0304,
            RENDERFORMAT           = 0x0305,
            RENDERALLFORMATS       = 0x0306,
            DESTROYCLIPBOARD       = 0x0307,
            DRAWCLIPBOARD          = 0x0308,
            PAINTCLIPBOARD         = 0x0309,
            VSCROLLCLIPBOARD       = 0x030A,
            SIZECLIPBOARD          = 0x030B,
            ASKCBFORMATNAME        = 0x030C,
            CHANGECBCHAIN          = 0x030D,
            HSCROLLCLIPBOARD       = 0x030E,
            QUERYNEWPALETTE        = 0x030F,
            PALETTEISCHANGING      = 0x0310,
            PALETTECHANGED         = 0x0311,
            HOTKEY                 = 0x0312,
            PRINT                  = 0x0317,
            PRINTCLIENT            = 0x0318,
            HANDHELDFIRST          = 0x0358,
            HANDHELDLAST           = 0x035F,
            AFXFIRST               = 0x0360,
            AFXLAST                = 0x037F,
            PENWINFIRST            = 0x0380,
            PENWINLAST             = 0x038F,
            APP                    = 0x8000,
            USER                   = 0x0400,
            REFLECT                = USER + 0x1c00,
        }
        #endregion  // Windows Messages

        #region Menu Flags
        public enum MF : int {
            INSERT          = 0x00000000,
            CHANGE          = 0x00000080,
            APPEND          = 0x00000100,
            DELETE          = 0x00000200,
            REMOVE          = 0x00001000,
            BYCOMMAND       = 0x00000000,
            BYPOSITION      = 0x00000400,
            SEPARATOR       = 0x00000800,
            ENABLED         = 0x00000000,
            GRAYED          = 0x00000001,
            DISABLED        = 0x00000002,
            UNCHECKED       = 0x00000000,
            CHECKED         = 0x00000008,
            USECHECKBITMAPS = 0x00000200,
            STRING          = 0x00000000,
            BITMAP          = 0x00000004,
            OWNERDRAW       = 0x00000100,
            POPUP           = 0x00000010,
            MENUBARBREAK    = 0x00000020,
            MENUBREAK       = 0x00000040,
            UNHILITE        = 0x00000000,
            HILITE          = 0x00000080,
            DEFAULT         = 0x00001000,
            SYSMENU         = 0x00002000,
            HELP            = 0x00004000,
            RIGHTJUSTIFY    = 0x00004000,
            MOUSESELECT     = 0x00008000,
            END             = 0x00000080  /* Obsolete -- only used by old RES files */
        }
        #endregion // Menu Flags

        public const int IDM_CUSTOM  = 1010;

        #region Edit Control Messages 
        public enum EM {
            GETSEL               = 0x00B0,
            SETSEL               = 0x00B1,
            GETRECT              = 0x00B2,
            SETRECT              = 0x00B3,
            SETRECTNP            = 0x00B4,
            SCROLL               = 0x00B5,
            LINESCROLL           = 0x00B6,
            SCROLLCARET          = 0x00B7,
            GETMODIFY            = 0x00B8,
            SETMODIFY            = 0x00B9,
            GETLINECOUNT         = 0x00BA,
            LINEINDEX            = 0x00BB,
            SETHANDLE            = 0x00BC,
            GETHANDLE            = 0x00BD,
            GETTHUMB             = 0x00BE,
            LINELENGTH           = 0x00C1,
            REPLACESEL           = 0x00C2,
            GETLINE              = 0x00C4,
            LIMITTEXT            = 0x00C5,
            CANUNDO              = 0x00C6,
            UNDO                 = 0x00C7,
            FMTLINES             = 0x00C8,
            LINEFROMCHAR         = 0x00C9,
            SETTABSTOPS          = 0x00CB,
            SETPASSWORDCHAR      = 0x00CC,
            EMPTYUNDOBUFFER      = 0x00CD,
            GETFIRSTVISIBLELINE  = 0x00CE,
            SETREADONLY          = 0x00CF,
            SETWORDBREAKPROC     = 0x00D0,
            GETWORDBREAKPROC     = 0x00D1,
            GETPASSWORDCHAR      = 0x00D2,
            SETMARGINS           = 0x00D3,
            GETMARGINS           = 0x00D4,
            SETLIMITTEXT         = LIMITTEXT,
            GETLIMITTEXT         = 0x00D5,
            POSFROMCHAR          = 0x00D6,
            CHARFROMPOS          = 0x00D7,
            SETIMESTATUS         = 0x00D8,
            GETIMESTATUS         = 0x00D9
        }

        #endregion // Edit Control Messages 

        #region ListView Messages 
        public enum LVM {
            FIRST        = 0x1000,
            SCROLL       = FIRST + 20,
            GETHEADER    = FIRST + 31,
            SETITEM	     = FIRST + 76,
            GETCOLUMN    = FIRST + 95,
            SETCOLUMN    = FIRST + 96,
            REDRAWITEMS  = FIRST + 21
        }
        #endregion // ListView Messages 

        #region Header Notification
        public enum HDN : int {
            FIRST              = (0-300),
            BEGINDRAG          = (FIRST-10),
            ITEMCHANGING       = (FIRST-20),
            ITEMCHANGED        = (FIRST-21),
            BEGINTRACK         = (FIRST-26),
            ENDTRACK           = (FIRST-27),
            ITEMCLICK          = (FIRST-22),
            DIVIDERDBLCLICK    = (FIRST-25)
        }
        #endregion // Header Notification

        #region Notification Messages
        public enum NM {
            FIRST            = (0-0),
            OUTOFMEMORY      = (FIRST-1),
            CLICK            = (FIRST-2),
            DBLCLK           = (FIRST-3),
            RETURN           = (FIRST-4),
            RCLICK           = (FIRST-5),
            RDBLCLK          = (FIRST-6),
            SETFOCUS         = (FIRST-7),
            KILLFOCUS        = (FIRST-8),
            CUSTOMDRAW       = (FIRST-12),
            HOVER            = (FIRST-13),
            NCHITTEST        = (FIRST-14),
            KEYDOWN          = (FIRST-15),
            RELEASEDCAPTURE  = (FIRST-16),
            SETCURSOR        = (FIRST-17),
            CHAR             = (FIRST-18),
            TOOLTIPSCREATED  = (FIRST-19),
            LDOWN            = (FIRST-20),
            RDOWN            = (FIRST-21),
            THEMECHANGED     = (FIRST-22)
        }
		#endregion

        #region HeaderControl Messages
        public enum HDM {
            FIRST        =  0x1200,
            GETITEMRECT  = (FIRST + 7),
            HITTEST      = (FIRST + 6),
            SETIMAGELIST = (FIRST + 8),
            GETITEMW     = (FIRST + 11),
            ORDERTOINDEX = (FIRST + 15)
        }
		#endregion

        #region Header Control HitTest Flags
        public enum HHT {
            NOWHERE             = 0x0001,
            ONHEADER            = 0x0002,
            ONDIVIDER           = 0x0004,
            ONDIVOPEN           = 0x0008,
            ABOVE               = 0x0100,
            BELOW               = 0x0200,
            TORIGHT             = 0x0400,
            TOLEFT              = 0x0800
        }
		#endregion // Header Control HitTest Flags

        #region Scrollbar related
        [StructLayout(LayoutKind.Sequential)]
        public struct SCROLLINFO {
            public int cbSize;
            public int fMask;
            public int nMin;
            public int nMax;
            public int nPage;
            public int nPos;
            public int nTrackPos;
        }

        public enum ScrollBarDirection {
            SB_HORZ = 0,
            SB_VERT = 1
        }

        public enum ScrollInfoMask {
            SIF_RANGE = 0x1,
            SIF_PAGE = 0x2,
            SIF_POS = 0x4,
            SIF_DISABLENOSCROLL = 0x8,
            SIF_TRACKPOS = 0x10,
            SIF_ALL = SIF_RANGE + SIF_PAGE + SIF_POS + SIF_TRACKPOS
        }
        #endregion // Scrollbar related

        public enum SW : uint {
            SCROLLCHILDREN = 0x0001,
            INVALIDATE     = 0x0002,
            ERASE          = 0x0004,
            SMOOTHSCROLL   = 0x0010
        }

        #region File attributes
        [Flags]
        public enum FILE_ATTRIBUTE : uint {
            READONLY             = 0x00000001,
            HIDDEN               = 0x00000002, 
            SYSTEM               = 0x00000004, 
            DIRECTORY            = 0x00000010,
            ARCHIVE              = 0x00000020,
            DEVICE               = 0x00000040,
            NORMAL               = 0x00000080,
            TEMPORARY            = 0x00000100,
            SPARSE_FILE          = 0x00000200,
            REPARSE_POINT        = 0x00000400,
            COMPRESSED           = 0x00000800,
            OFFLINE              = 0x00001000,
            NOT_CONTENT_INDEXED  = 0x00002000,
            ENCRYPTED            = 0x00004000
        }
        #endregion // File attributes

        #region ShellGetFileInfo flags
        
        [Flags]
        public enum SHGFI : uint {
            ICON              = 0x000000100,    // get icon
            DISPLAYNAME       = 0x000000200,    // get display name
            TYPENAME          = 0x000000400,    // get type name
            ATTRIBUTES        = 0x000000800,    // get attributes
            ICONLOCATION      = 0x000001000,    // get icon location
            EXETYPE           = 0x000002000,    // return exe type
            SYSICONINDEX      = 0x000004000,    // get system icon index
            LINKOVERLAY       = 0x000008000,    // put a link overlay on icon
            SELECTED          = 0x000010000,    // show icon in selected state
            ATTR_SPECIFIED    = 0x000020000,    // get only specified attributes
            LARGEICON         = 0x000000000,    // get large icon
            SMALLICON         = 0x000000001,    // get small icon
            OPENICON          = 0x000000002,    // get open icon
            SHELLICONSIZE     = 0x000000004,    // get shell size icon
            PIDL              = 0x000000008,    // pszPath is a pidl
            USEFILEATTRIBUTES = 0x000000010,    // use passed dwFileAttribute

            ADDOVERLAYS       = 0x000000020,    // apply the appropriate overlays
            OVERLAYINDEX      = 0x000000040     // Get the index of the overlay
        }
        
        #endregion // ShellGetFileInfo flags

        #region CustomDrawReturnFlags
        public enum CDRF {
            DODEFAULT          = 0x00000000,
            NEWFONT            = 0x00000002,
            SKIPDEFAULT        = 0x00000004,
            NOTIFYPOSTPAINT    = 0x00000010,
            NOTIFYITEMDRAW     = 0x00000020,
            NOTIFYSUBITEMDRAW  = 0x00000020, 
            NOTIFYPOSTERASE    = 0x00000040
        }
		#endregion

        #region Custom Draw State Flags
        public enum CDDS {
            PREPAINT           = 0x00000001,
            POSTPAINT          = 0x00000002,
            PREERASE           = 0x00000003,
            POSTERASE          = 0x00000004,
            ITEM               = 0x00010000,
            ITEMPREPAINT       = (ITEM | PREPAINT),
            ITEMPOSTPAINT      = (ITEM | POSTPAINT),
            ITEMPREERASE       = (ITEM | PREERASE),
            ITEMPOSTERASE      = (ITEM | POSTERASE),
            SUBITEM            = 0x00020000
        }
		#endregion

        #region Notification message structure
        [StructLayout(LayoutKind.Sequential)]
        public struct NMHDR { 
            public IntPtr hwndFrom; 
            public int  idFrom; 
            public int  code; 
        }
        #endregion // Notification message structure

        #region Tab control notification messages
        public enum TCN {
            FIRST       = (0 - 550),
            SELCHANGE   = (FIRST - 1),
            SELCHANGING = (FIRST - 2)
        }
        #endregion // Tab control notification messages

        #region HD_HITTESTINFO
        [StructLayout(LayoutKind.Sequential)]
        public struct HD_HITTESTINFO {  
            public Win32Api.POINT pt;  
            public HHT flags; 
            public int iItem; 
        }
		#endregion

        #region NMCUSTOMDRAW
        [StructLayout(LayoutKind.Sequential)]
        public struct NMCUSTOMDRAW {
            public NMHDR hdr;
            public int dwDrawStage;
            public IntPtr hdc;
            public RECT rc;
            public uint dwItemSpec;
            public uint uItemState;
            public IntPtr lItemlParam;
        }
		#endregion

        #region POINT
        [StructLayout(LayoutKind.Sequential)]
        public struct POINT {
            public int x;
            public int y;
        }
		#endregion

        #region RECT
        [StructLayout(LayoutKind.Sequential)]
        public struct RECT {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }
		#endregion

        public const int BITSPIXEL = 12;
    }
}