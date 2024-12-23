﻿/*
 *  IconExtractor/IconUtil for .NET
 *  Copyright (C) 2014 Tsuda Kageyu. All rights reserved.
 *
 *  Redistribution and use in source and binary forms, with or without
 *  modification, are permitted provided that the following conditions
 *  are met:
 *
 *   1. Redistributions of source code must retain the above copyright
 *      notice, this list of conditions and the following disclaimer.
 *   2. Redistributions in binary form must reproduce the above copyright
 *      notice, this list of conditions and the following disclaimer in the
 *      documentation and/or other materials provided with the distribution.
 *
 *  THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
 *  "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED
 *  TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A
 *  PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER
 *  OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
 *  EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
 *  PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
 *  PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
 *  LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
 *  NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
 *  SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 */

using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Nostrum.WinAPI;

namespace PieLauncher.Core;

public class IconExtractor
{
    #region Constants

    // Flags for LoadLibraryEx().

    private const uint LOAD_LIBRARY_AS_DATAFILE = 0x00000002;
    private const int MaxPath = 256;

    // Resource types for EnumResourceNames().

    private const IntPtr RT_ICON = 3;
    private const IntPtr RT_GROUP_ICON = 14;

    #endregion

    #region Fields
    /// <summary>
    /// Binary data of each icon.
    /// </summary>
    private readonly byte[][] _iconData;

    #endregion

    #region Public properties

    /// <summary>
    /// Gets the count of the icons in the associated file.
    /// </summary>
    public int Count => _iconData.Length;

    #endregion

    /// <summary>
    /// Initializes a new instance of the IconExtractor class from the specified file name.
    /// </summary>
    /// <param name="fileName">The file to extract icons from.</param>
#pragma warning disable CS8618 // too lazy to fix this now
    public IconExtractor(string fileName)
#pragma warning restore CS8618 
    {
        if (fileName == null)
            throw new ArgumentNullException(nameof(fileName));

        var hModule = IntPtr.Zero;

        try
        {
            hModule = Kernel32.LoadLibraryEx(fileName, IntPtr.Zero, LOAD_LIBRARY_AS_DATAFILE);

            if (hModule == IntPtr.Zero)
                throw new Win32Exception();

            _ = GetFileName(hModule);

            // Enumerate the icon resource and build .ico files in memory.

            var tmpData = new List<byte[]>();

            bool callback(IntPtr h, IntPtr t, IntPtr name, IntPtr l)
            {
                // Refer the following URL for the data structures used here:
                // http://msdn.microsoft.com/en-us/library/ms997538.aspx

                // RT_GROUP_ICON resource consists of a GRPICONDIR and GRPICONDIRENTRY's.

                var dir = GetDataFromResource(hModule, RT_GROUP_ICON, name);

                // Calculate the size of an entire .icon file.

                int count = BitConverter.ToUInt16(dir, 4);  // GRPICONDIR.idCount
                var len = 6 + (16 * count);                   // sizeof(ICONDIR) + sizeof(ICONDIRENTRY) * count

                for (var i = 0; i < count; ++i)

                    len += BitConverter.ToInt32(dir, 6 + 14 * i + 8);   // GRPICONDIRENTRY.dwBytesInRes

                using var dst = new BinaryWriter(new MemoryStream(len));
                // Copy GRPICONDIR to ICONDIR.

                dst.Write(dir, 0, 6);

                var picOffset = 6 + (16 * count); // sizeof(ICONDIR) + sizeof(ICONDIRENTRY) * count

                for (var i = 0; i < count; ++i)
                {
                    // Load the picture.

                    var id = BitConverter.ToUInt16(dir, 6 + (14 * i) + 12);    // GRPICONDIRENTRY.nID
                    var pic = GetDataFromResource(hModule, RT_ICON, id);

                    // Copy GRPICONDIRENTRY to ICONDIRENTRY.

                    _ = dst.Seek(6 + (16 * i), SeekOrigin.Begin);

                    dst.Write(dir, 6 + (14 * i), 8);  // First 8bytes are identical.
                    dst.Write(pic.Length);          // ICONDIRENTRY.dwBytesInRes
                    dst.Write(picOffset);           // ICONDIRENTRY.dwImageOffset

                    // Copy a picture.

                    _ = dst.Seek(picOffset, SeekOrigin.Begin);
                    dst.Write(pic, 0, pic.Length);

                    picOffset += pic.Length;
                }

                tmpData.Add(((MemoryStream)dst.BaseStream).ToArray());

                return true;
            }

            Kernel32.EnumResourceNames(hModule, RT_GROUP_ICON, callback, IntPtr.Zero);

            _iconData = tmpData.ToArray();
        }
        finally
        {
            if (hModule != IntPtr.Zero)
                Kernel32.FreeLibrary(hModule);
        }
    }

    /// <summary>
    /// Extracts an icon from the file.
    /// </summary>
    /// <param name="index">Zero based index of the icon to be extracted.</param>
    /// <returns>A System.Drawing.Icon object.</returns>
    /// <remarks>Always returns new copy of the Icon. It should be disposed by the user.</remarks>
    public Icon GetIcon(int index)
    {
        if (index < 0 || Count <= index)
            throw new ArgumentOutOfRangeException(nameof(index));

        // Create an Icon based on a .ico file in memory.

        using var ms = new MemoryStream(_iconData[index]);
        return new Icon(ms, -1,-1);
    }

    /// <summary>
    /// Extracts all the icons from the file.
    /// </summary>
    /// <returns>An array of System.Drawing.Icon objects.</returns>
    /// <remarks>Always returns new copies of the Icons. They should be disposed by the user.</remarks>
    public Icon[] GetAllIcons()
    {
        var icons = new List<Icon>();

        for (var i = 0; i < Count; ++i)
            icons.Add(GetIcon(i));

        return icons.ToArray();
    }

    /// <summary>
    /// Save an icon to the specified output Stream.
    /// </summary>
    /// <param name="index">Zero based index of the icon to be saved.</param>
    /// <param name="outputStream">The Stream to save to.</param>
    public void Save(int index, Stream outputStream)
    {
        if (index < 0 || Count <= index)
            throw new ArgumentOutOfRangeException(nameof(index));

        if (outputStream == null)
            throw new ArgumentNullException(nameof(outputStream));

        var data = _iconData[index];
        outputStream.Write(data, 0, data.Length);
    }

    private byte[] GetDataFromResource(IntPtr hModule, IntPtr type, IntPtr name)
    {
        // Load the binary data from the specified resource.

        var hResInfo = Kernel32.FindResource(hModule, name, type);

        if (hResInfo == IntPtr.Zero)
            throw new Win32Exception();

        var hResData = Kernel32.LoadResource(hModule, hResInfo);

        if (hResData == IntPtr.Zero)
            throw new Win32Exception();

        var pResData = Kernel32.LockResource(hResData);

        if (pResData == IntPtr.Zero)
            throw new Win32Exception();

        var size = Kernel32.SizeofResource(hModule, hResInfo);

        if (size == 0)
            throw new Win32Exception();

        var buf = new byte[size];
        Marshal.Copy(pResData, buf, 0, buf.Length);

        return buf;
    }

    private static string GetFileName(IntPtr hModule)
    {
        // Alternative to GetModuleFileName() for the module loaded with
        // LOAD_LIBRARY_AS_DATAFILE option.

        // Get the file name in the format like:
        // "\\Device\\HarddiskVolume2\\Windows\\System32\\shell32.dll"

        string fileName;
        {
            var buf = new StringBuilder(MaxPath);
            var len = Psapi.GetMappedFileName(Kernel32.GetCurrentProcess(), hModule, buf, buf.Capacity);

            if (len == 0)

                throw new Win32Exception();

            fileName = buf.ToString();
        }

        // Convert the device name to drive name like:
        // "C:\\Windows\\System32\\shell32.dll"

        for (var c = 'A'; c <= 'Z'; ++c)
        {
            var drive = c + ":";
            var buf = new StringBuilder(MaxPath);
            var len = Kernel32.QueryDosDevice(drive, buf, buf.Capacity);

            if (len == 0)
                continue;

            var devPath = buf.ToString();

            if (fileName.StartsWith(devPath, StringComparison.OrdinalIgnoreCase))
                return (drive + fileName.Substring(devPath.Length));
        }

        return fileName;
    }
}