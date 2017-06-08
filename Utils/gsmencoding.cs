using System;
using System.Collections.Generic;

namespace Bergfall.Oculos.Utils
{
    /*
 * Copyright (c) 2010 Mediaburst Ltd <hello@mediaburst.co.uk>
 *
 * Permission to use, copy, modify, and/or distribute this software for any
 * purpose with or without fee is hereby granted, provided that the above
 * copyright notice and this permission notice appear in all copies.
 *
 * THE SOFTWARE IS PROVIDED "AS IS" AND THE AUTHOR DISCLAIMS ALL WARRANTIES
 * WITH REGARD TO THIS SOFTWARE INCLUDING ALL IMPLIED WARRANTIES OF
 * MERCHANTABILITY AND FITNESS. IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR
 * ANY SPECIAL, DIRECT, INDIRECT, OR CONSEQUENTIAL DAMAGES OR ANY DAMAGES
 * WHATSOEVER RESULTING FROM LOSS OF USE, DATA OR PROFITS, WHETHER IN AN
 * ACTION OF CONTRACT, NEGLIGENCE OR OTHER TORTIOUS ACTION, ARISING OUT OF
 * OR IN CONNECTION WITH THE USE OR PERFORMANCE OF THIS SOFTWARE.
 */

    public class GSMEncoding : System.Text.Encoding
    {
        public SortedDictionary<char, byte[]> CharToByte
        {
            get; private set;
        }

        public SortedDictionary<uint, char> ByteToChar
        {
            get; private set;
        }

        public GSMEncoding() => PopulateDictionaries();

        public override int GetByteCount(char[] chars, int index, int count)
        {
            int byteCount = 0;

            if(chars == null)
            {
                throw new ArgumentNullException("chars");
            }
            if(index < 0 || index > chars.Length)
            {
                throw new ArgumentOutOfRangeException("index");
            }
            if(count < 0 || count > (chars.Length - index))
            {
                throw new ArgumentOutOfRangeException("count");
            }

            for(int i = index; i < count; i++)
            {
                if(CharToByte.ContainsKey(chars[i]))
                {
                    byteCount += CharToByte[chars[i]].Length;
                }
            }

            return byteCount;
        }

        public override byte[] GetBytes(string s)
        {
            byte[] bytes = new byte[this.GetByteCount(s.ToCharArray(), 0, s.Length)];
            this.GetBytes(s, 0, s.Length, bytes, 0);
            return bytes;
        }

        public override int GetBytes(char[] chars, int charIndex, int charCount, byte[] bytes, int byteIndex)
        {
            int byteCount = 0;

            // Validate the parameters.
            if(chars == null)
            {
                throw new ArgumentNullException("chars");
            }
            if(bytes == null)
            {
                throw new ArgumentNullException("bytes");
            }
            if(charIndex < 0 || charIndex > chars.Length)
            {
                throw new ArgumentOutOfRangeException("charIndex");
            }
            if(charCount < 0 || charCount > (chars.Length - charIndex))
            {
                throw new ArgumentOutOfRangeException("charCount");
            }
            if(byteIndex < 0 || byteIndex > bytes.Length)
            {
                throw new ArgumentOutOfRangeException("byteIndex");
            }
            if(byteIndex + GetByteCount(chars, charIndex, charCount) > bytes.Length)
            {
                throw new ArgumentException("bytes array too small", "bytes");
            }
            for(int i = charIndex; i < charIndex + charCount; i++)
            {
                byte[] charByte;
                if(CharToByte.TryGetValue(chars[i], out charByte))
                {
                    charByte.CopyTo(bytes, byteIndex + byteCount);
                    byteCount += charByte.Length;
                }
            }
            return byteCount;
        }

        public override int GetCharCount(byte[] bytes, int index, int count)
        {
            int charCount = 0;

            if(bytes == null)
            {
                throw new ArgumentNullException("bytes");
            }
            if(index < 0 || index > bytes.Length)
            {
                throw new ArgumentOutOfRangeException("index");
            }
            if(count < 0 || count > (bytes.Length - index))
            {
                throw new ArgumentOutOfRangeException("count");
            }

            int i = index;
            while(i < index + count)
            {
                if(bytes[i] <= 0x7F)
                {
                    if(bytes[i] == 0x1B)
                    {
                        i++;
                        if(i < bytes.Length && bytes[i] <= 0x7F)
                        {
                            charCount++; // GSM Spec says replace 1B 1B with space
                        }
                    }
                    else
                    {
                        charCount++;
                    }
                }
                i++;
            }

            return charCount;
        }

        /// <summary>
        /// Get number of characters a byte[] encoded in GSM-7
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="byteIndex"></param>
        /// <param name="byteCount"></param>
        /// <param name="chars"></param>
        /// <param name="charIndex"></param>
        /// <returns></returns>
        public override int GetChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex)
        {
            int charCount = 0;

            // Validate the parameters.
            if(bytes == null)
            {
                throw new ArgumentNullException("bytes");
            }
            if(chars == null)
            {
                throw new ArgumentNullException("chars");
            }
            if(byteIndex < 0 || byteIndex > bytes.Length)
            {
                throw new ArgumentOutOfRangeException("byteIndex");
            }
            if(byteCount < 0 || byteCount > (bytes.Length - byteIndex))
            {
                throw new ArgumentOutOfRangeException("byteCount");
            }
            if(charIndex < 0 || charIndex > chars.Length)
            {
                throw new ArgumentOutOfRangeException("charIndex");
            }
            if(charIndex + GetCharCount(bytes, byteIndex, byteCount) > chars.Length)
            {
                throw new ArgumentException("chars array too small", "chars");
            }

            int i = byteIndex;
            while(i < byteIndex + byteCount)
            {
                if(bytes[i] <= 0x7F)
                {
                    // If byte is 0x1B, The Basic Character Set Extension is used
                    if(bytes[i] == 0x1B)
                    {
                        i++;
                        if(i < bytes.Length && bytes[i] <= 0x7F)
                        {
                            char nextChar;
                            uint extendedChar = (0x1B * 255) + (uint)bytes[i];
                            if(ByteToChar.TryGetValue(extendedChar, out nextChar))
                            {
                                chars[charCount] = nextChar;
                                charCount++;
                            }
                            // GSM Spec says to try for normal character if escaped one doesn't exist
                            else if(ByteToChar.TryGetValue((uint)bytes[i], out nextChar))
                            {
                                chars[charCount] = nextChar;
                                charCount++;
                            }
                        }
                    }
                    else
                    {
                        chars[charCount] = ByteToChar[(uint)bytes[i]];
                        charCount++;
                    }
                }
                i++;
            }

            return charCount;
        }

        public override int GetMaxByteCount(int charCount)
        {
            if(charCount < 0)
            {
                throw new ArgumentOutOfRangeException("charCount");
            }
            else
            {
                return charCount * 2;
            }
        }

        public override int GetMaxCharCount(int byteCount)
        {
            if(byteCount > (160 * 7))
            {
                return 153;
            }
            else
            {
                return 160;
            }
        }

        private void PopulateDictionaries()
        {
            // Unicode char to GSM bytes
            CharToByte = new SortedDictionary<char, byte[]>();
            // GSM bytes to Unicode char
            ByteToChar = new SortedDictionary<uint, char>();

            CharToByte.Add('\u0040', new byte[] { 0x00 });
            CharToByte.Add('\u00A3', new byte[] { 0x01 });
            CharToByte.Add('\u0024', new byte[] { 0x02 });
            CharToByte.Add('\u00A5', new byte[] { 0x03 });
            CharToByte.Add('\u00E8', new byte[] { 0x04 });
            CharToByte.Add('\u00E9', new byte[] { 0x05 });
            CharToByte.Add('\u00F9', new byte[] { 0x06 });
            CharToByte.Add('\u00EC', new byte[] { 0x07 });
            CharToByte.Add('\u00F2', new byte[] { 0x08 });
            CharToByte.Add('\u00C7', new byte[] { 0x09 });
            CharToByte.Add('\u000A', new byte[] { 0x0A });
            CharToByte.Add('\u00D8', new byte[] { 0x0B });
            CharToByte.Add('\u00F8', new byte[] { 0x0C });
            CharToByte.Add('\u000D', new byte[] { 0x0D });
            CharToByte.Add('\u00C5', new byte[] { 0x0E });
            CharToByte.Add('\u00E5', new byte[] { 0x0F });
            CharToByte.Add('\u0394', new byte[] { 0x10 });
            CharToByte.Add('\u005F', new byte[] { 0x11 });
            CharToByte.Add('\u03A6', new byte[] { 0x12 });
            CharToByte.Add('\u0393', new byte[] { 0x13 });
            CharToByte.Add('\u039B', new byte[] { 0x14 });
            CharToByte.Add('\u03A9', new byte[] { 0x15 });
            CharToByte.Add('\u03A0', new byte[] { 0x16 });
            CharToByte.Add('\u03A8', new byte[] { 0x17 });
            CharToByte.Add('\u03A3', new byte[] { 0x18 });
            CharToByte.Add('\u0398', new byte[] { 0x19 });
            CharToByte.Add('\u039E', new byte[] { 0x1A });
            //_charToByte.Add('\u001B', new byte[] { 0x1B }); // Should we convert Unicode escape to GSM?
            CharToByte.Add('\u00C6', new byte[] { 0x1C });
            CharToByte.Add('\u00E6', new byte[] { 0x1D });
            CharToByte.Add('\u00DF', new byte[] { 0x1E });
            CharToByte.Add('\u00C9', new byte[] { 0x1F });
            CharToByte.Add('\u0020', new byte[] { 0x20 });
            CharToByte.Add('\u0021', new byte[] { 0x21 });
            CharToByte.Add('\u0022', new byte[] { 0x22 });
            CharToByte.Add('\u0023', new byte[] { 0x23 });
            CharToByte.Add('\u00A4', new byte[] { 0x24 });
            CharToByte.Add('\u0025', new byte[] { 0x25 });
            CharToByte.Add('\u0026', new byte[] { 0x26 });
            CharToByte.Add('\u0027', new byte[] { 0x27 });
            CharToByte.Add('\u0028', new byte[] { 0x28 });
            CharToByte.Add('\u0029', new byte[] { 0x29 });
            CharToByte.Add('\u002A', new byte[] { 0x2A });
            CharToByte.Add('\u002B', new byte[] { 0x2B });
            CharToByte.Add('\u002C', new byte[] { 0x2C });
            CharToByte.Add('\u002D', new byte[] { 0x2D });
            CharToByte.Add('\u002E', new byte[] { 0x2E });
            CharToByte.Add('\u002F', new byte[] { 0x2F });
            CharToByte.Add('\u0030', new byte[] { 0x30 });
            CharToByte.Add('\u0031', new byte[] { 0x31 });
            CharToByte.Add('\u0032', new byte[] { 0x32 });
            CharToByte.Add('\u0033', new byte[] { 0x33 });
            CharToByte.Add('\u0034', new byte[] { 0x34 });
            CharToByte.Add('\u0035', new byte[] { 0x35 });
            CharToByte.Add('\u0036', new byte[] { 0x36 });
            CharToByte.Add('\u0037', new byte[] { 0x37 });
            CharToByte.Add('\u0038', new byte[] { 0x38 });
            CharToByte.Add('\u0039', new byte[] { 0x39 });
            CharToByte.Add('\u003A', new byte[] { 0x3A });
            CharToByte.Add('\u003B', new byte[] { 0x3B });
            CharToByte.Add('\u003C', new byte[] { 0x3C });
            CharToByte.Add('\u003D', new byte[] { 0x3D });
            CharToByte.Add('\u003E', new byte[] { 0x3E });
            CharToByte.Add('\u003F', new byte[] { 0x3F });
            CharToByte.Add('\u00A1', new byte[] { 0x40 });
            CharToByte.Add('\u0041', new byte[] { 0x41 });
            CharToByte.Add('\u0042', new byte[] { 0x42 });
            CharToByte.Add('\u0043', new byte[] { 0x43 });
            CharToByte.Add('\u0044', new byte[] { 0x44 });
            CharToByte.Add('\u0045', new byte[] { 0x45 });
            CharToByte.Add('\u0046', new byte[] { 0x46 });
            CharToByte.Add('\u0047', new byte[] { 0x47 });
            CharToByte.Add('\u0048', new byte[] { 0x48 });
            CharToByte.Add('\u0049', new byte[] { 0x49 });
            CharToByte.Add('\u004A', new byte[] { 0x4A });
            CharToByte.Add('\u004B', new byte[] { 0x4B });
            CharToByte.Add('\u004C', new byte[] { 0x4C });
            CharToByte.Add('\u004D', new byte[] { 0x4D });
            CharToByte.Add('\u004E', new byte[] { 0x4E });
            CharToByte.Add('\u004F', new byte[] { 0x4F });
            CharToByte.Add('\u0050', new byte[] { 0x50 });
            CharToByte.Add('\u0051', new byte[] { 0x51 });
            CharToByte.Add('\u0052', new byte[] { 0x52 });
            CharToByte.Add('\u0053', new byte[] { 0x53 });
            CharToByte.Add('\u0054', new byte[] { 0x54 });
            CharToByte.Add('\u0055', new byte[] { 0x55 });
            CharToByte.Add('\u0056', new byte[] { 0x56 });
            CharToByte.Add('\u0057', new byte[] { 0x57 });
            CharToByte.Add('\u0058', new byte[] { 0x58 });
            CharToByte.Add('\u0059', new byte[] { 0x59 });
            CharToByte.Add('\u005A', new byte[] { 0x5A });
            CharToByte.Add('\u00C4', new byte[] { 0x5B });
            CharToByte.Add('\u00D6', new byte[] { 0x5C });
            CharToByte.Add('\u00D1', new byte[] { 0x5D });
            CharToByte.Add('\u00DC', new byte[] { 0x5E });
            CharToByte.Add('\u00A7', new byte[] { 0x5F });
            CharToByte.Add('\u00BF', new byte[] { 0x60 });
            CharToByte.Add('\u0061', new byte[] { 0x61 });
            CharToByte.Add('\u0062', new byte[] { 0x62 });
            CharToByte.Add('\u0063', new byte[] { 0x63 });
            CharToByte.Add('\u0064', new byte[] { 0x64 });
            CharToByte.Add('\u0065', new byte[] { 0x65 });
            CharToByte.Add('\u0066', new byte[] { 0x66 });
            CharToByte.Add('\u0067', new byte[] { 0x67 });
            CharToByte.Add('\u0068', new byte[] { 0x68 });
            CharToByte.Add('\u0069', new byte[] { 0x69 });
            CharToByte.Add('\u006A', new byte[] { 0x6A });
            CharToByte.Add('\u006B', new byte[] { 0x6B });
            CharToByte.Add('\u006C', new byte[] { 0x6C });
            CharToByte.Add('\u006D', new byte[] { 0x6D });
            CharToByte.Add('\u006E', new byte[] { 0x6E });
            CharToByte.Add('\u006F', new byte[] { 0x6F });
            CharToByte.Add('\u0070', new byte[] { 0x70 });
            CharToByte.Add('\u0071', new byte[] { 0x71 });
            CharToByte.Add('\u0072', new byte[] { 0x72 });
            CharToByte.Add('\u0073', new byte[] { 0x73 });
            CharToByte.Add('\u0074', new byte[] { 0x74 });
            CharToByte.Add('\u0075', new byte[] { 0x75 });
            CharToByte.Add('\u0076', new byte[] { 0x76 });
            CharToByte.Add('\u0077', new byte[] { 0x77 });
            CharToByte.Add('\u0078', new byte[] { 0x78 });
            CharToByte.Add('\u0079', new byte[] { 0x79 });
            CharToByte.Add('\u007A', new byte[] { 0x7A });
            CharToByte.Add('\u00E4', new byte[] { 0x7B });
            CharToByte.Add('\u00F6', new byte[] { 0x7C });
            CharToByte.Add('\u00F1', new byte[] { 0x7D });
            CharToByte.Add('\u00FC', new byte[] { 0x7E });
            CharToByte.Add('\u00E0', new byte[] { 0x7F });
            // Extended GSM
            CharToByte.Add('\u20AC', new byte[] { 0x1B, 0x65 });
            CharToByte.Add('\u000C', new byte[] { 0x1B, 0x0A });
            CharToByte.Add('\u005B', new byte[] { 0x1B, 0x3C });
            CharToByte.Add('\u005C', new byte[] { 0x1B, 0x2F });
            CharToByte.Add('\u005D', new byte[] { 0x1B, 0x3E });
            CharToByte.Add('\u005E', new byte[] { 0x1B, 0x14 });
            CharToByte.Add('\u007B', new byte[] { 0x1B, 0x28 });
            CharToByte.Add('\u007C', new byte[] { 0x1B, 0x40 });
            CharToByte.Add('\u007D', new byte[] { 0x1B, 0x29 });
            CharToByte.Add('\u007E', new byte[] { 0x1B, 0x3D });

            foreach(KeyValuePair<char, byte[]> charToByte in CharToByte)
            {
                uint charByteVal = 0;
                if(charToByte.Value.Length == 1)
                    charByteVal = (uint)charToByte.Value[0];
                else if(charToByte.Value.Length == 2)
                    charByteVal = ((uint)charToByte.Value[0] * 255) + (uint)charToByte.Value[1];
                ByteToChar.Add(charByteVal, charToByte.Key);
            }
            //_byteToChar.Add(0x1B1B, '\u0020'); // GSM char set says to map 1B1B to a space
        }
    }
}