using System;

namespace NVelocity.Runtime.Parser.Node
{
	public interface IParserVisitor
	{
		object Visit(SimpleNode node, object data);

		object Visit(ASTprocess node, object data);

		object Visit(ASTComment node, object data);

		object Visit(ASTNumberLiteral node, object data);

		object Visit(ASTStringLiteral node, object data);

		object Visit(ASTIdentifier node, object data);

		object Visit(ASTWord node, object data);

		object Visit(ASTDirective node, object data);

		object Visit(ASTBlock node, object data);

		object Visit(ASTObjectArray node, object data);

		object Visit(ASTMethod node, object data);

		object Visit(ASTReference node, object data);

		object Visit(ASTTrue node, object data);

		object Visit(ASTFalse node, object data);

		object Visit(ASTText node, object data);

		object Visit(ASTIfStatement node, object data);

		object Visit(ASTElseStatement node, object data);

		object Visit(ASTElseIfStatement node, object data);

		object Visit(ASTSetDirective node, object data);

		object Visit(ASTExpression node, object data);

		object Visit(ASTAssignment node, object data);

		object Visit(ASTOrNode node, object data);

		object Visit(ASTAndNode node, object data);

		object Visit(ASTEQNode node, object data);

		object Visit(ASTNENode node, object data);

		object Visit(ASTLTNode node, object data);

		object Visit(ASTGTNode node, object data);

		object Visit(ASTLENode node, object data);

		object Visit(ASTGENode node, object data);

		object Visit(ASTAddNode node, object data);

		object Visit(ASTSubtractNode node, object data);

		object Visit(ASTMulNode node, object data);

		object Visit(ASTDivNode node, object data);

		object Visit(ASTModNode node, object data);

		object Visit(ASTNotNode node, object data);
	}
}
