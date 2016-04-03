/*
 * Filename:    MessageFilter.cs
 * Product:     Versioning Controlled Build
 * Solution:    BuildAutoIncrement
 * Project:     Shared
 * Description: OLE message filter used to avoid  'Application is Busy' and 
 *              'Call was Rejected By Callee' errors.
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

namespace BuildAutoIncrement {
    /// <summary>
    ///   Class implements an OLE message filter, appropriate for use with the 
    ///   VS automation clients. Used to avoid 'Application is Busy' and 
    ///   'Call was Rejected By Callee' errors:
    ///   http://msdn2.microsoft.com/en-us/library/ms228772(VS.80).aspx
    /// </summary>
    public class MessageFilter : IOleMessageFilter {

        public static void Register() {
            IOleMessageFilter newfilter = new MessageFilter(); 

            IOleMessageFilter oldfilter = null; 
            CoRegisterMessageFilter(newfilter, out oldfilter);
        }

        public static void Revoke() {
            IOleMessageFilter oldfilter = null; 
            CoRegisterMessageFilter(null, out oldfilter);
        }

        /// <summary>
        ///   IOleMessageFilter::HandleInComingCall implementation
        /// </summary>
        /// <param name="dwCallType"></param>
        /// <param name="hTaskCaller"></param>
        /// <param name="dwTickCount"></param>
        /// <param name="lpInterfaceInfo"></param>
        /// <returns></returns>
        int IOleMessageFilter.HandleInComingCall(int dwCallType, System.IntPtr hTaskCaller, int dwTickCount, System.IntPtr lpInterfaceInfo) {
            System.Diagnostics.Debug.WriteLine("IOleMessageFilter::HandleInComingCall");
            return 0; //SERVERCALL_ISHANDLED
        }

        int IOleMessageFilter.RetryRejectedCall(System.IntPtr hTaskCallee, int dwTickCount, int dwRejectType) {
            System.Diagnostics.Debug.WriteLine("IOleMessageFilter::RetryRejectedCall");
            if (dwRejectType == 2 ) { //SERVERCALL_RETRYLATER
                System.Diagnostics.Debug.WriteLine("Retry call later");
                return 99; //retry immediately if return >=0 & <100
            }
            return -1; //cancel call
        }

        int IOleMessageFilter.MessagePending(System.IntPtr hTaskCallee, int dwTickCount, int dwPendingType) {
            System.Diagnostics.Debug.WriteLine("IOleMessageFilter::MessagePending");
            return 2; //PENDINGMSG_WAITDEFPROCESS 
        }

        //
        // Implementation
        [DllImport("Ole32.dll")]
        private static extern int CoRegisterMessageFilter(IOleMessageFilter newfilter, out IOleMessageFilter oldfilter);
    }

    [ComImport(), Guid("00000016-0000-0000-C000-000000000046"),    
    InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)]
    interface IOleMessageFilter { // deliberately renamed to avoid confusion w/ System.Windows.Forms.IMessageFilter
        [PreserveSig]
        int HandleInComingCall( 
            int dwCallType, 
            IntPtr hTaskCaller, 
            int dwTickCount, 
            IntPtr lpInterfaceInfo);

        [PreserveSig]
        int RetryRejectedCall( 
            IntPtr hTaskCallee, 
            int dwTickCount,
            int dwRejectType);

        [PreserveSig]
        int MessagePending( 
            IntPtr hTaskCallee, 
            int dwTickCount,
            int dwPendingType);
    }
}