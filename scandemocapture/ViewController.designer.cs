// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace scandemocapture
{
    [Register ("ViewController")]
    partial class ViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel battLevelLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel bdaddrLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel powerStatusLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel scannedDataLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel scannerLabel { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (battLevelLabel != null) {
                battLevelLabel.Dispose ();
                battLevelLabel = null;
            }

            if (bdaddrLabel != null) {
                bdaddrLabel.Dispose ();
                bdaddrLabel = null;
            }

            if (powerStatusLabel != null) {
                powerStatusLabel.Dispose ();
                powerStatusLabel = null;
            }

            if (scannedDataLabel != null) {
                scannedDataLabel.Dispose ();
                scannedDataLabel = null;
            }

            if (scannerLabel != null) {
                scannerLabel.Dispose ();
                scannerLabel = null;
            }
        }
    }
}