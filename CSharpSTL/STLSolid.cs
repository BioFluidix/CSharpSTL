/*
 * Copyright(c) 2020, BioFluidix GmbH
 * All rights reserved.
 * 
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions are met:
 *     * Redistributions of source code must retain the above copyright
 *       notice, this list of conditions and the following disclaimer.
 *     * Redistributions in binary form must reproduce the above copyright
 *       notice, this list of conditions and the following disclaimer in the
 *       documentation and/or other materials provided with the distribution.
 *     * Neither the name of the <organization> nor the
 *       names of its contributors may be used to endorse or promote products
 *       derived from this software without specific prior written permission.
 * 
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
 * ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
 * WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
 * DISCLAIMED.IN NO EVENT SHALL <COPYRIGHT HOLDER> BE LIABLE FOR ANY
 * DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
 * (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
 * LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
 * ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
 * (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
 * SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 * 
 */

using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using CSharpSTL.Grammar;
using CSharpSTL.Parser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CSharpSTL
{



    /// <summary>
    /// A reader for the STL file format.
    /// </summary>
    public class STLSolid
    {
        /// <summary>
        /// The facets of the solid.
        /// </summary>
        public List<STLFacet> Facets { get; set; }

        /// <summary>
        /// Creates a STL solid from a file. 
        /// </summary>
        /// <param name="file">The source file.</param>
        /// <returns>A STL solid.</returns>

        public static STLSolid CreateFromFile(string file)
        {
            STLSolid result;
            using (FileStream fs = File.Open(file, FileMode.Open, FileAccess.Read))
            {
                result = CreateFromStream(fs);
            }
            return result;
        }

        /// <summary>
        /// Creates a STL solid from a stream. 
        /// </summary>
        /// <param name="input">The source stream.</param>
        /// <returns>A STL solid.</returns>
        public static STLSolid CreateFromStream(Stream input)
        {
            const string C_SOLID = "solid";
            var bstream = new BufferedStream(input);
            var magic = new byte[C_SOLID.Length];
            bstream.Read(magic, 0, magic.Length);
            bstream.Seek(0, SeekOrigin.Begin);
            return (Encoding.ASCII.GetString(magic) == C_SOLID) ?
                    CreateFromASCIIStream(bstream) :
                    CreateFromBinaryStream(bstream);
        }

        /// <summary>
        /// Creates a STL solid from a binary stream. 
        /// </summary>
        /// <param name="input">The source stream.</param>
        /// <returns>A STL solid.</returns>
        public static STLSolid CreateFromASCIIStream(Stream input)
        {
            AntlrInputStream inputStream = new AntlrInputStream(input);
            STLLexer lexer = new STLLexer(inputStream);
            CommonTokenStream tokens = new CommonTokenStream(lexer);
            STLParser parser = new STLParser(tokens);
            IParseTree tree = parser.solid();
            STLVisitor visitor = new STLVisitor() { Solid = new STLSolid() };
            visitor.Visit(tree);
            return visitor.Solid;
        }

        /// <summary>
        /// Creates a STL solid from an ASCII stream. 
        /// </summary>
        /// <param name="input">The source stream.</param>
        /// <returns>A STL solid.</returns>
        public static STLSolid CreateFromBinaryStream(Stream input)
        {
            var solid = new STLSolid();
            var buffer = new byte[1024];

            // Skip header
            Read(input, buffer, 80);

            // Read number of facets
            var facetCount = DecodeUInt32(input, buffer);

            for(int i = 0; i < facetCount; i++)
            {
                var facet = new STLFacet();
                facet.Normal = new STLNormal
                {
                    X = DecodeReal32(input, buffer),
                    Y = DecodeReal32(input, buffer),
                    Z = DecodeReal32(input, buffer)
                };

                var outerLoop = new STLOuterLoop();
                outerLoop.V0 = new STLVertex
                {
                    X = DecodeReal32(input, buffer),
                    Y = DecodeReal32(input, buffer),
                    Z = DecodeReal32(input, buffer)
                };

                outerLoop.V1 = new STLVertex
                {
                    X = DecodeReal32(input, buffer),
                    Y = DecodeReal32(input, buffer),
                    Z = DecodeReal32(input, buffer)
                };

                outerLoop.V2 = new STLVertex
                {
                    X = DecodeReal32(input, buffer),
                    Y = DecodeReal32(input, buffer),
                    Z = DecodeReal32(input, buffer)
                };

                facet.OuterLoop = outerLoop;

                var devNull = DecodeUInt16(input, buffer);

                solid.Facets.Add(facet);
            }

            return solid;
        }



        #region machine room

        private static readonly Func<Stream, byte[], uint> DecodeUInt16
            = (s, b) => BitConverter.ToUInt16(Read(s, b, 2, ByteTransferOperator(2)), 0);

        private static readonly Func<Stream, byte[], uint> DecodeUInt32
            = (s, b) => BitConverter.ToUInt32(Read(s, b, 4, ByteTransferOperator(4)), 0);

        private static readonly Func<Stream, byte[], float> DecodeReal32
            = (s, b) => BitConverter.ToSingle(Read(s, b, 4, ByteTransferOperator(4)), 0);


        private static byte[] Read(Stream input, byte[] buffer, int length, Action<byte[]> op = null)
        {
            int pointer = 0;
            int bytesToRead = length;
            while (bytesToRead > 0)
            {
                var bytesRead = input.Read(buffer, pointer, bytesToRead);
                bytesToRead -= bytesRead;
                pointer += bytesRead;
            }
            op?.Invoke(buffer);
            return buffer;
        }

        private static Action<byte[]> ByteTransferOperator(int length)
            => BitConverter.IsLittleEndian ? null : new Action<byte[]>(s => Array.Reverse(s, 0, length));

        #endregion

    }


}
