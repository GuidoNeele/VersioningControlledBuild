/*
 * Filename:    InvalidFileFilterCharacters.cs
 * Product:     Versioning Controlled Build
 * Solution:    BuildAutoIncrement
 * Project:     Shared
 * Description: Provides characters not allowed for file filter. Class is
 *              currently not in use.
 * Copyright:   Julijan Šribar, 2004-2007
 */
using System;
using System.Collections;
using System.IO;
using System.Text;

namespace BuildAutoIncrement {
	/// <summary>
	/// Summary description for InvalidFileFilterCharacters.
	/// </summary>
	public class InvalidFileFilterCharacters {
        /*
        public InvalidFileFilterCharacters() {
            ArrayList invalidCharacters = new ArrayList();
            invalidCharacters.Add(Path.DirectorySeparatorChar);
            invalidCharacters.Add(Path.AltDirectorySeparatorChar);
            invalidCharacters.Add(Path.VolumeSeparatorChar);
#if VS7
            foreach (char ch in Path.InvalidPathChars) {
#elif VS8
            foreach (char ch in Path.GetInvalidPathChars()) {
#endif
                invalidCharacters.Add(ch);
            }
            InvalidCharacters = (char[])invalidCharacters.ToArray(typeof(char));
        }

        public override string ToString() {
            StringBuilder result = new StringBuilder(InvalidCharacters.Length * 2);
            foreach (char ch in InvalidCharacters) {
                result.Append(ch);
                result.Append(' ');
            }
            return result.ToString();
        }

        public readonly char[] InvalidCharacters;
        */
    }
}