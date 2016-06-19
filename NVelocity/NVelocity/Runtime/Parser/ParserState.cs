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

namespace NVelocity.Runtime.Parser
{
    using System.Collections.Generic;

    using Node;

    /// <summary>
    /// 
    /// </summary>
    public class ParserState
    {
        private Stack<INode> nodes;
        private Stack<int> marks;

        private int mk; // current mark
        private bool nodeCreated;

        public ParserState()
        {
            nodes = new Stack<INode>();
            marks = new Stack<int>();
        }

        /* Determines whether the current node was actually closed and
        pushed.  This should only be called in the final user action of a
        node scope.  */
        public virtual bool NodeCreated()
        {
            return nodeCreated;
        }

        /* Call this to reinitialize the node stack.  It is called
        automatically by the parser's ReInit() method. */
        public virtual void Reset()
        {
            nodes.Clear();
            marks.Clear();
            mk = 0;
        }

        /* Returns the root node of the AST.  It only makes sense to call
        this after a successful parse. */
        public INode RootNode()
        {
            return (nodes.ToArray())[nodes.Count - (0 + 1)];
        }

        /* Pushes a node on to the stack. */
        public void PushNode(INode n)
        {
            nodes.Push(n);
        }

        /* Returns the node on the top of the stack, and remove it from the
        stack.  */
        public INode PopNode()
        {
            if (nodes.Count < mk)
            {
                mk = marks.Pop();
            }

            return nodes.Pop();
        }

        /* Returns the node currently on the top of the stack. */
        public INode PeekNode()
        {
            return nodes.Peek();
        }

        /* Returns the number of children on the stack in the current node
        scope. */
        public int NodeArity()
        {
            return nodes.Count - mk;
        }


        public void ClearNodeScope(INode n)
        {
            while (nodes.Count > mk)
            {
                PopNode();
            }

            mk = marks.Pop();
        }


        public void OpenNodeScope(INode n)
        {
            marks.Push(mk);
            mk = nodes.Count;
            n.Open();
        }


        /* A definite node is constructed from a specified number of
        children.  That number of nodes are popped from the stack and
        made the children of the definite node.  Then the definite node
        is pushed on to the stack. */
        public void CloseNodeScope(INode n, int num)
        {
            mk = marks.Pop();
            while (num-- > 0)
            {
                INode node = PopNode();
                node.Parent = n;
                n.AddChild(node, num);
            }
            n.Close();
            PushNode(n);
            nodeCreated = true;
        }


        /* A conditional node is constructed if its condition is true.  All
        the nodes that have been pushed since the node was opened are
        made children of the conditional node, which is then pushed
        on to the stack.  If the condition is false the node is not
        constructed and they are left on the stack. */
        public void closeNodeScope(INode n, bool condition)
        {
            if (condition)
            {
                int a = NodeArity();

                mk = marks.Pop();
                while (a-- > 0)
                {
                    INode c = PopNode();
                    c.Parent = n;
                    n.AddChild(c, a);
                }
                n.Close();
                PushNode(n);
                nodeCreated = true;
            }
            else
            {

                mk = marks.Pop();
                nodeCreated = false;
            }
        }
    }
}
