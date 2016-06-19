using NVelocity.Context;
using System;
using System.IO;

namespace NVelocity.Runtime.Parser.Node
{
	public interface INode
	{
		Token FirstToken
		{
			get;
		}

		Token LastToken
		{
			get;
		}

		int Type
		{
			get;
		}

		int Info
		{
			get;
			set;
		}

		int Line
		{
			get;
		}

		int Column
		{
			get;
		}

		INode Parent
		{
			get;
			set;
		}

		int ChildrenCount
		{
			get;
		}

		string Literal
		{
			get;
		}

		bool IsInvalid
		{
			get;
			set;
		}

		void Open();

		void Close();

		void AddChild(INode n, int i);

		INode GetChild(int i);

		object Accept(IParserVisitor visitor, object data);

		object ChildrenAccept(IParserVisitor visitor, object data);

		object Init(IInternalContextAdapter context, object data);

		bool Evaluate(IInternalContextAdapter context);

		object Value(IInternalContextAdapter context);

		bool Render(IInternalContextAdapter context, TextWriter writer);

		object Execute(object o, IInternalContextAdapter context);
	}
}
