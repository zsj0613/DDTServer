/*
* Licensed to the Apache Software Foundation (ASF) under one
* or more contributor license agreements.  See the NOTICE file
* distributed with this work for additional information
* regarding copyright ownership.  The ASF licenses this file
* to you under the Apache License, Version 2.0 (the
* "License"); you may not use this file except in compliance
* with the License.  You may obtain a copy of the License at
*
*   http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing,
* software distributed under the License is distributed on an
* "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
* KIND, either express or implied.  See the License for the
* specific language governing permissions and limitations
* under the License.    
*/

namespace NVelocity.Runtime.Directive
{
    using Context;
    using Parser;
    using Parser.Node;

    /// <summary>  Macro implements the macro definition directive of VTL.
    /// 
    /// example :
    /// 
    /// #macro( isnull $i )
    /// #if( $i )
    /// $i
    /// #end
    /// #end
    /// 
    /// This object is used at parse time to mainly process and register the
    /// macro.  It is used inline in the parser when processing a directive.
    /// 
    /// </summary>
    /// <author>  <a href="mailto:geirm@optonline.net">Geir Magnusson Jr.</a>
    /// </author>
    /// <author>  <a href="hps@intermeta.de">Henning P. Schmiedehausen</a>
    /// </author>
    /// <version>  $Id: Macro.java 685685 2008-08-13 21:43:27Z nbubna $
    /// </version>
    public class Macro : Directive
    {
        /// <summary> Return type of this directive.</summary>
        /// <returns> The type of this directive.
        /// </returns>
        override public int Type
        {
            get
            {
                return NVelocity.Runtime.Directive.DirectiveType.BLOCK;
            }

        }
        private static bool debugMode = false;

        /// <summary> Return name of this directive.</summary>
        /// <returns> The name of this directive.
        /// </returns>
        public override string Name
        {
            get
            {
                return "macro";
            }
        }

        /// <summary>   render() doesn't do anything in the final output rendering.
        /// There is no output from a #macro() directive.
        /// </summary>
        /// <param name="context">
        /// </param>
        /// <param name="writer">
        /// </param>
        /// <param name="node">
        /// </param>
        /// <returns> True if the directive rendered successfully.
        /// </returns>
        /// <throws>  IOException </throws>
        public override bool Render(IInternalContextAdapter context, System.IO.TextWriter writer, INode node)
        {
            /*
            *  do nothing : We never render.  The VelocimacroProxy object does that
            */

            return true;
        }

        /// <seealso cref="org.apache.velocity.runtime.directive.Directive.Init(org.apache.velocity.runtime.RuntimeServices, org.apache.velocity.context.InternalContextAdapter, NVelocity.Runtime.Paser.Node.Node)">
        /// </seealso>
        public override void Init(IRuntimeServices rs, IInternalContextAdapter context, INode node)
        {
            base.Init(rs, context, node);

            /*
            * again, don't do squat.  We want the AST of the macro
            * block to hang off of this but we don't want to
            * Init it... it's useless...
            */
        }

        /// <summary>  Used by Parser.java to process VMs during the parsing process.
        /// 
        /// This method does not render the macro to the output stream,
        /// but rather <i>processes the macro body</i> into the internal
        /// representation used by {#link
        /// org.apache.velocity.runtime.directive.VelocimacroProxy}
        /// objects, and if not currently used, adds it to the macro
        /// Factory.
        /// </summary>
        /// <param name="rs">
        /// </param>
        /// <param name="t">
        /// </param>
        /// <param name="node">
        /// </param>
        /// <param name="sourceTemplate">
        /// </param>
        /// <throws>  IOException </throws>
        /// <throws>  ParseException </throws>
        public static void ProcessAndRegister(IRuntimeServices rs, Token t, INode node, string sourceTemplate)
        {
            /*
            *  There must be at least one arg to  #macro,
            *  the name of the VM.  Note that 0 following
            *  args is ok for naming blocks of HTML
            */

            int numArgs = node.GetNumChildren();

            /*
            *  this number is the # of args + 1.  The + 1
            *  is for the block tree
            */

            if (numArgs < 2)
            {

                /*
                *  Error - they didn't name the macro or
                *  define a block
                */

                rs.Log.Error("#macro error : Velocimacro must have name as 1st " + "argument to #macro(). #args = " + numArgs);

                throw new MacroParseException("First argument to #macro() must be " + " macro name.", sourceTemplate, t);
            }

            /*
            *  lets make sure that the first arg is an ASTWord
            */

            int firstType = node.GetChild(0).Type;

            if (firstType != NVelocity.Runtime.Parser.ParserTreeConstants.JJTWORD)
            {
                throw new MacroParseException("First argument to #macro() must be a" + " token without surrounding \' or \", which specifies" + " the macro name.  Currently it is a " + NVelocity.Runtime.Parser.ParserTreeConstants.jjtNodeName[firstType], sourceTemplate, t);
            }

            // Get the arguments to the use of the VM - element 0 contains the macro name
            string[] argArray = GetArgArray(node, rs);

            /* 
            * we already have the macro parsed as AST so there is no point to
            * transform it into a String again
            */
            rs.AddVelocimacro(argArray[0], node.GetChild(numArgs - 1), argArray, sourceTemplate);

            /*
            * Even if the Add attempt failed, we don't Log anything here.
            * Logging must be done at VelocimacroFactory or VelocimacroManager because
            * those classes know the real reason.
            */
        }


        /// <summary> Creates an array containing the literal text from the macro
        /// arguement(s) (including the macro's name as the first arg).
        /// 
        /// </summary>
        /// <param name="node">The parse node from which to grok the argument
        /// list.  It's expected to include the block node tree (for the
        /// macro body).
        /// </param>
        /// <param name="rsvc">For debugging purposes only.
        /// </param>
        /// <returns> array of arguments
        /// </returns>
        private static string[] GetArgArray(INode node, IRuntimeServices rsvc)
        {
            /*
            * Get the number of arguments for the macro, excluding the
            * last child node which is the block tree containing the
            * macro body.
            */
            int numArgs = node.GetNumChildren();
            numArgs--; // avoid the block tree...

            string[] argArray = new string[numArgs];

            int i = 0;

            /*
            *  eat the args
            */

            while (i < numArgs)
            {
                argArray[i] = node.GetChild(i).FirstToken.Image;

                /*
                *  trim off the leading $ for the args after the macro name.
                *  saves everyone else from having to do it
                */

                if (i > 0)
                {
                    if (argArray[i].StartsWith("$"))
                    {
                        argArray[i] = argArray[i].Substring(1, (argArray[i].Length) - (1));
                    }
                }

                i++;
            }

            if (debugMode)
            {
                System.Text.StringBuilder msg = new System.Text.StringBuilder("Macro.getArgArray() : nbrArgs=");
                msg.Append(numArgs).Append(" : ");
                MacroToString(msg, argArray);
                rsvc.Log.Debug(msg);
            }

            return argArray;
        }

        /// <summary> For debugging purposes.  Formats the arguments from
        /// <code>argArray</code> and appends them to <code>buf</code>.
        /// 
        /// </summary>
        /// <param name="buf">A StringBuffer. If null, a new StringBuffer is allocated.
        /// </param>
        /// <param name="argArray">The Macro arguments to format
        /// 
        /// </param>
        /// <returns> A StringBuffer containing the formatted arguments. If a StringBuffer
        /// has passed in as buf, this method returns it.
        /// </returns>
        /// <since> 1.5
        /// </since>
        public static System.Text.StringBuilder MacroToString(System.Text.StringBuilder buf, string[] argArray)
        {
            System.Text.StringBuilder ret = (buf == null) ? new System.Text.StringBuilder() : buf;

            ret.Append('#').Append(argArray[0]).Append("( ");
            for (int i = 1; i < argArray.Length; i++)
            {
                ret.Append(' ').Append(argArray[i]);
            }
            ret.Append(" )");
            return ret;
        }
    }
}