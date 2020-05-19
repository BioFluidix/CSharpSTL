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

grammar STL;

solid : ID_SOLID FILE_NAME?
	list=facetList
ID_ENDSOLID FILE_NAME?

EOF;

facetList : (facets+=facet)+ ;

facet : ID_FACET
	n=normal
	loop=outer_loop
ID_ENDFACET ;

normal : ID_NORMAL x=FLOAT y=FLOAT z=FLOAT ;

vertex : ID_VERTEX x=FLOAT y=FLOAT z=FLOAT ;

outer_loop : ID_OUTER ID_LOOP
	v0=vertex
	v1=vertex
	v2=vertex
ID_ENDLOOP ;


// --- LEXER GRAMMAR ---
ID_SOLID: 'solid';
ID_FACET: 'facet';
ID_NORMAL: 'normal';
ID_OUTER: 'outer';
ID_LOOP: 'loop';
ID_ENDLOOP: 'endloop';
ID_ENDFACET: 'endfacet';
ID_VERTEX: 'vertex';
ID_ENDSOLID: 'endsolid';


FLOAT : SIGN? DIGITS (DOT [0-9] +)? EXP? [fd]? ;

fragment DOT: '.';

fragment SIGN : '-'|'+' ;
fragment DIGITS : '0' | [1-9] [0-9]* ;
fragment EXP : [Ee] [+\-]? DIGITS ;

FILE_NAME: [a-zA-Z_][a-zA-Z0-9._]*;

// skip whitespace (newline, tab and space)
WS :   [ \r\t\n]+ -> skip;


