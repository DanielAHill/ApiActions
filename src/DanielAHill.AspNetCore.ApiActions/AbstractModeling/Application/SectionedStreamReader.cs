#region Copyright
// Copyright (c) 2016 Daniel A Hill. All rights reserved.
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DanielAHill.AspNetCore.ApiActions.AbstractModeling.Application
{
    internal class SectionedStreamReader
    {
        private readonly StreamReader _reader;
        private readonly CancellationToken _cancellationToken;
        private readonly StringBuilder _builder;
        private readonly char[] _readBuffer;

        private const char Delimiter = '\\';
        private const char UnicodeHexPrefix = 'u';
        private static readonly IReadOnlyDictionary<char, char> DelimitedValueMapping = new Dictionary<char, char>()
            {
                { 'r', '\r' },
                { 'R', '\r' },
                { 'n', '\n'},
                { 'N', '\n'},
                { 't', '\t'},
                { 'T', '\t'},
                { 'f', '\f'},
                { 'F', '\f'},
                { 'b', '\b'},
                { 'B', '\b'}
            };

        internal bool EndOfStream { get { return _reader.EndOfStream; } }
        internal char LastDelimiter { get { return _readBuffer[0]; } }

        internal SectionedStreamReader(Stream stream, CancellationToken cancellationToken)
        {
            _reader = new StreamReader(stream);
            _cancellationToken = cancellationToken;
            _builder = new StringBuilder();
            _readBuffer = new char[1];
        }

        internal async Task<string> ReadSection(char[] terminators, bool ignoreSurroundingWhitespace = true)
        {
            _cancellationToken.ThrowIfCancellationRequested();

            var writeWhitespace = !ignoreSurroundingWhitespace;
            var lastNonWhiteSpace = 0;

            while (await _reader.ReadAsync(_readBuffer, 0, 1) == 1 && !terminators.Contains(_readBuffer[0]))
            {
                var c = _readBuffer[0];

                var isCharNonWhiteSpace = !char.IsWhiteSpace(c);

                if (c == Delimiter)
                {
                    if (await _reader.ReadAsync(_readBuffer, 0, 1) != 1)
                    {
                        throw new InvalidDataException("Expected value following delimeter: " + Delimiter);
                    }
                    c = _readBuffer[0];

                    char replacementChar;
                    if (DelimitedValueMapping.TryGetValue(c, out replacementChar))
                    {
                        c = replacementChar;
                    }
                    else if (c == UnicodeHexPrefix)
                    {
                        // Read up to 4 digits of hex
                        var numericBuffer = new char[4];
                        if (4 != await _reader.ReadAsync(numericBuffer, 0, 4))
                        {
                            throw new InvalidDataException(@"Expected 4 hex digits following \u");
                        }

                        try
                        {
                            c = (char)int.Parse(new string(numericBuffer), System.Globalization.NumberStyles.HexNumber);
                        }
                        catch (Exception ex)
                        {
                            throw new InvalidDataException(@"Expected 4 valid hex digits following \u", ex);
                        }
                    }
                }

                if (writeWhitespace || isCharNonWhiteSpace)
                {
                    writeWhitespace = true;
                    _builder.Append(c);

                    if (isCharNonWhiteSpace)
                    {
                        lastNonWhiteSpace = _builder.Length;
                    }
                }

                if (_builder.Length % 1024 == 0)
                {
                    _cancellationToken.ThrowIfCancellationRequested();
                }
            }

            if (ignoreSurroundingWhitespace)
            {
                _builder.Length = lastNonWhiteSpace;
            }

            var result = _builder.ToString();
            _builder.Clear();

            return result;
        }
    }
}