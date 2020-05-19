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

using Antlr4.Runtime.Misc;
using CSharpSTL.Parser;
using System.Collections.Generic;
using System.Linq;

namespace CSharpSTL.Grammar
{
    public class STLVisitor : STLBaseVisitor<object>
    {
        public STLSolid Solid { get; set; }


        public override object VisitSolid([NotNull] STLParser.SolidContext context)
        {
            Solid.Facets = VisitFacetList(context.list) as List<STLFacet>;
            return base.VisitSolid(context);
        }

        public override object VisitFacetList([NotNull] STLParser.FacetListContext context)
            => context._facets?.Select(facet => VisitFacet(facet) as STLFacet).ToList();

        public override object VisitFacet([NotNull] STLParser.FacetContext context)
            => new STLFacet
                {
                    Normal = VisitNormal(context.n) as STLNormal,
                    OuterLoop = VisitOuter_loop(context.loop) as STLOuterLoop
                };

        public override object VisitNormal([NotNull] STLParser.NormalContext context)
            => new STLNormal
            {
                X = float.Parse(context.x.Text),
                Y = float.Parse(context.y.Text),
                Z = float.Parse(context.z.Text)
            };

        public override object VisitVertex([NotNull] STLParser.VertexContext context)
            => new STLVertex
            {
                X = float.Parse(context.x.Text),
                Y = float.Parse(context.y.Text),
                Z = float.Parse(context.z.Text)
            };

        public override object VisitOuter_loop([NotNull] STLParser.Outer_loopContext context)
            => new STLOuterLoop
            {
                V0 = VisitVertex(context.v0) as STLVertex,
                V1 = VisitVertex(context.v1) as STLVertex,
                V2 = VisitVertex(context.v2) as STLVertex,
            };

    }
}
