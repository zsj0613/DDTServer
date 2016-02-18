using NVelocity.Runtime.Directive;
using NVelocity.Runtime.Parser.Node;
using NVelocity.Util;
using System;
using System.Collections;
using System.IO;

namespace NVelocity.Runtime.Parser
{
	public class Parser
	{
		private sealed class Calls
		{
			internal int Gen;

			internal Token First;

			internal int Arg;

			internal Parser.Calls Next;
		}

		internal ParserState nodeTree;

		private IDirectiveManager directives;

		internal string currentTemplateName;

		internal VelocityCharStream velcharstream;

		private IRuntimeServices rsvc;

		protected Stack directiveStack;

		public ParserTokenManager token_source;

		public Token token;

		public Token jj_nt;

		private int jj_ntk_Renamed_Field;

		private Token jj_scanpos;

		private Token jj_lastpos;

		private int jj_la;

		public bool lookingAhead;

		private bool jj_semLA;

		private int jj_gen;

		private int[] jj_la1;

		private int[] jj_la1_0;

		private int[] jj_la1_1;

		private Parser.Calls[] jj_2_rtns;

		private bool jj_rescan;

		private int jj_gc;

		private ArrayList jj_expentries;

		private int[] jj_expentry;

		private int jj_kind;

		private int[] jj_lasttokens;

		private int jj_endpos;

		public IDirectiveManager Directives
		{
			set
			{
				this.directives = value;
			}
		}

		public Token NextToken
		{
			get
			{
				if (this.token.Next != null)
				{
					this.token = this.token.Next;
				}
				else
				{
					this.token = (this.token.Next = this.token_source.NextToken);
				}
				this.jj_ntk_Renamed_Field = -1;
				this.jj_gen++;
				return this.token;
			}
		}

		public Parser(IRuntimeServices rs) : this(new VelocityCharStream(new StringReader("\n"), 1, 1))
		{
			this.InitBlock();
			this.velcharstream = new VelocityCharStream(new StringReader("\n"), 1, 1);
			this.rsvc = rs;
		}

		private void InitBlock()
		{
			this.nodeTree = new ParserState();
			this.directives = null;
			this.jj_la1 = new int[53];
			this.jj_2_rtns = new Parser.Calls[12];
			this.jj_expentries = new ArrayList();
			this.jj_lasttokens = new int[100];
		}

		public SimpleNode Parse(TextReader reader, string templateName)
		{
			SimpleNode result = null;
			this.currentTemplateName = templateName;
			try
			{
				this.token_source.ClearStateVars();
				this.velcharstream.ReInit(reader, 1, 1);
				this.ReInit(this.velcharstream);
				result = this.Process();
			}
			catch (ParseException ex)
			{
				this.rsvc.Error("Parser Exception: " + templateName + " : " + StringUtils.StackTrace(ex));
				throw (ex.currentToken == null) ? ex : new ParseException(ex.currentToken, ex.expectedTokenSequences, ex.tokenImage);
			}
			catch (TokenMgrError tokenMgrError)
			{
				throw new ParseException("Lexical error: " + tokenMgrError.ToString());
			}
			catch (System.Exception e)
			{
				this.rsvc.Error("Parser Error: " + templateName + " : " + StringUtils.StackTrace(e));
			}
			this.currentTemplateName = "";
			return result;
		}

		public Directive.Directive GetDirective(string directive)
		{
			return this.directives.Create(directive, this.directiveStack);
		}

		public bool IsDirective(string directive)
		{
			return this.directives.Contains(directive);
		}

		private string EscapedDirective(string strImage)
		{
			int num = strImage.LastIndexOf("\\");
			string text = strImage.Substring(num + 1);
			bool flag = false;
			if (this.IsDirective(text.Substring(1)))
			{
				flag = true;
			}
			else if (this.rsvc.IsVelocimacro(text.Substring(1), this.currentTemplateName))
			{
				flag = true;
			}
			else if (text.Substring(1).Equals("if") || text.Substring(1).Equals("end") || text.Substring(1).Equals("set") || text.Substring(1).Equals("else") || text.Substring(1).Equals("elseif") || text.Substring(1).Equals("stop"))
			{
				flag = true;
			}
			string result;
			if (flag)
			{
				result = strImage.Substring(0, num / 2) + text;
			}
			else
			{
				result = strImage;
			}
			return result;
		}

		public SimpleNode Process()
		{
			ASTprocess aSTprocess = new ASTprocess(this, 0);
			bool flag = true;
			this.nodeTree.OpenNodeScope(aSTprocess);
			SimpleNode result;
			try
			{
				while (true)
				{
					int currentTokenKind = this.GetCurrentTokenKind();
					switch (currentTokenKind)
					{
					case 5:
					case 6:
					case 8:
					case 9:
					case 16:
					case 17:
					case 18:
					case 19:
					case 20:
					case 21:
					case 24:
						break;
					case 7:
					case 10:
					case 11:
					case 12:
					case 13:
					case 14:
					case 15:
					case 22:
					case 23:
						goto IL_C8;
					default:
						if (currentTokenKind != 44)
						{
							switch (currentTokenKind)
							{
							case 47:
							case 49:
							case 52:
							case 56:
							case 57:
							case 58:
							case 59:
								goto IL_C6;
							}
							goto Block_6;
						}
						break;
					}
					IL_C6:
					this.Statement();
				}
				Block_6:
				IL_C8:
				this.jj_la1[0] = this.jj_gen;
				this.ConsumeToken(0);
				this.nodeTree.CloseNodeScope(aSTprocess, true);
				flag = false;
				result = aSTprocess;
			}
			catch (System.Exception ex)
			{
				if (flag)
				{
					this.nodeTree.ClearNodeScope(aSTprocess);
					flag = false;
				}
				else
				{
					this.nodeTree.PopNode();
				}
				if (ex is SystemException)
				{
					throw (SystemException)ex;
				}
				if (ex is ParseException)
				{
					throw (ParseException)ex;
				}
				throw;
			}
			finally
			{
				if (flag)
				{
					this.nodeTree.CloseNodeScope(aSTprocess, true);
				}
			}
			return result;
		}

		public void Statement()
		{
			int currentTokenKind = this.GetCurrentTokenKind();
			int num = currentTokenKind;
			if (num != 44)
			{
				if (num != 47)
				{
					this.jj_la1[1] = this.jj_gen;
					if (this.jj_2_1(2))
					{
						this.Reference();
					}
					else
					{
						int currentTokenKind2 = this.GetCurrentTokenKind();
						num = currentTokenKind2;
						if (num <= 49)
						{
							switch (num)
							{
							case 5:
							case 6:
							case 17:
							case 18:
							case 24:
								break;
							case 7:
							case 10:
							case 11:
							case 12:
							case 13:
							case 14:
							case 15:
							case 22:
							case 23:
								goto IL_118;
							case 8:
								this.EscapedDirective();
								goto IL_134;
							case 9:
								this.SetDirective();
								goto IL_134;
							case 16:
								this.Escape();
								goto IL_134;
							case 19:
							case 20:
							case 21:
								this.Comment();
								goto IL_134;
							default:
								if (num != 49)
								{
									goto IL_118;
								}
								break;
							}
						}
						else
						{
							if (num == 52)
							{
								this.Directive();
								goto IL_134;
							}
							switch (num)
							{
							case 57:
							case 58:
							case 59:
								break;
							default:
								goto IL_118;
							}
						}
						this.Text();
						goto IL_134;
						IL_118:
						this.jj_la1[2] = this.jj_gen;
						this.ConsumeToken(-1);
						throw new ParseException();
						IL_134:;
					}
				}
				else
				{
					this.StopStatement();
				}
			}
			else
			{
				this.IfStatement();
			}
		}

		public void EscapedDirective()
		{
			ASTEscapedDirective n = new ASTEscapedDirective(this, 2);
			bool flag = true;
			this.nodeTree.OpenNodeScope(n);
			try
			{
				Token token = this.ConsumeToken(8);
				this.nodeTree.CloseNodeScope(n, true);
				flag = false;
				token.Image = this.EscapedDirective(token.Image);
			}
			finally
			{
				if (flag)
				{
					this.nodeTree.CloseNodeScope(n, true);
				}
			}
		}

		public void Escape()
		{
			ASTEscape n = new ASTEscape(this, 3);
			bool flag = true;
			this.nodeTree.OpenNodeScope(n);
			try
			{
				int num = 0;
				bool flag2 = false;
				Token token;
				do
				{
					token = this.ConsumeToken(16);
					num++;
				}
				while (this.jj_2_2(2));
				this.nodeTree.CloseNodeScope(n, true);
				flag = false;
				switch (token.Next.Kind)
				{
				case 43:
				case 44:
				case 45:
				case 46:
				case 47:
					flag2 = true;
					break;
				}
				if (this.IsDirective(token.Next.Image.Substring(1)))
				{
					flag2 = true;
				}
				else if (this.rsvc.IsVelocimacro(token.Next.Image.Substring(1), this.currentTemplateName))
				{
					flag2 = true;
				}
				token.Image = "";
				for (int i = 0; i < num; i++)
				{
					Token expr_F3 = token;
					expr_F3.Image += (flag2 ? "\\" : "\\\\");
				}
			}
			finally
			{
				if (flag)
				{
					this.nodeTree.CloseNodeScope(n, true);
				}
			}
		}

		public void Comment()
		{
			ASTComment n = new ASTComment(this, 4);
			bool flag = true;
			this.nodeTree.OpenNodeScope(n);
			try
			{
				switch (this.GetCurrentTokenKind())
				{
				case 19:
					this.ConsumeToken(19);
					break;
				case 20:
					this.ConsumeToken(20);
					break;
				case 21:
					this.ConsumeToken(21);
					break;
				default:
					this.jj_la1[3] = this.jj_gen;
					this.ConsumeToken(-1);
					throw new ParseException();
				}
			}
			finally
			{
				if (flag)
				{
					this.nodeTree.CloseNodeScope(n, true);
				}
			}
		}

		public void NumberLiteral()
		{
			ASTNumberLiteral n = new ASTNumberLiteral(this, 5);
			bool flag = true;
			this.nodeTree.OpenNodeScope(n);
			try
			{
				this.ConsumeToken(49);
			}
			finally
			{
				if (flag)
				{
					this.nodeTree.CloseNodeScope(n, true);
				}
			}
		}

		public void StringLiteral()
		{
			ASTStringLiteral n = new ASTStringLiteral(this, 6);
			bool flag = true;
			this.nodeTree.OpenNodeScope(n);
			try
			{
				this.ConsumeToken(24);
			}
			finally
			{
				if (flag)
				{
					this.nodeTree.CloseNodeScope(n, true);
				}
			}
		}

		public void Identifier()
		{
			ASTIdentifier n = new ASTIdentifier(this, 7);
			bool flag = true;
			this.nodeTree.OpenNodeScope(n);
			try
			{
				this.ConsumeToken(56);
			}
			finally
			{
				if (flag)
				{
					this.nodeTree.CloseNodeScope(n, true);
				}
			}
		}

		public void Word()
		{
			ASTWord n = new ASTWord(this, 8);
			bool flag = true;
			this.nodeTree.OpenNodeScope(n);
			try
			{
				this.ConsumeToken(52);
			}
			finally
			{
				if (flag)
				{
					this.nodeTree.CloseNodeScope(n, true);
				}
			}
		}

		public void DirectiveArg()
		{
			int currentTokenKind = this.GetCurrentTokenKind();
			if (currentTokenKind <= 49)
			{
				if (currentTokenKind == 24)
				{
					this.StringLiteral();
					return;
				}
				if (currentTokenKind == 49)
				{
					this.NumberLiteral();
					return;
				}
			}
			else
			{
				if (currentTokenKind == 52)
				{
					this.Word();
					return;
				}
				switch (currentTokenKind)
				{
				case 56:
				case 58:
					this.Reference();
					return;
				}
			}
			this.jj_la1[4] = this.jj_gen;
			if (this.jj_2_3(2147483647))
			{
				this.IntegerRange();
			}
			else
			{
				currentTokenKind = this.GetCurrentTokenKind();
				if (currentTokenKind != 1)
				{
					switch (currentTokenKind)
					{
					case 23:
						this.ConsumeToken(23);
						goto IL_F9;
					case 25:
						this.True();
						goto IL_F9;
					case 26:
						this.False();
						goto IL_F9;
					}
					this.jj_la1[5] = this.jj_gen;
					this.ConsumeToken(-1);
					throw new ParseException();
				}
				this.ObjectArray();
				IL_F9:;
			}
		}

		public SimpleNode Directive()
		{
			ASTDirective aSTDirective = new ASTDirective(this, 9);
			bool flag = true;
			this.nodeTree.OpenNodeScope(aSTDirective);
			bool flag2 = false;
			SimpleNode result;
			try
			{
				Token token = this.ConsumeToken(52);
				string text = token.Image.Substring(1);
                Directive.Directive directive = this.directives.Create(text, this.directiveStack);
				aSTDirective.Directive = directive;
				if (text.Equals("macro"))
				{
					flag2 = true;
				}
				aSTDirective.DirectiveName = text;
				DirectiveType directiveType;
				if (directive == null)
				{
					if (this.rsvc != null)
					{
						if (!this.rsvc.IsVelocimacro(text, this.currentTemplateName))
						{
							this.token_source.StateStackPop();
							this.token_source.inDirective = false;
							result = aSTDirective;
							return result;
						}
					}
					directiveType = DirectiveType.LINE;
				}
				else
				{
					directiveType = directive.Type;
				}
				this.token_source.SwitchTo(0);
				this.ConsumeWhiteSpaces();
				if (directive != null && !directive.AcceptParams)
				{
					int currentTokenKind = this.GetCurrentTokenKind();
					if (currentTokenKind != 27)
					{
						throw new ParseException("Foreach directives must be the only items on the line (comments or contents are not allowed)");
					}
					this.ConsumeToken(27);
				}
				if (directive == null || directive.AcceptParams)
				{
					this.ConsumeToken(5);
					while (true)
					{
						int num = this.GetCurrentTokenKind();
						if (num <= 26)
						{
							if (num != 1)
							{
								switch (num)
								{
								case 23:
								case 24:
								case 25:
								case 26:
									goto IL_1B0;
								}
								break;
							}
						}
						else if (num != 49 && num != 52)
						{
							switch (num)
							{
							case 56:
							case 58:
								goto IL_1B0;
							}
							break;
						}
						IL_1B0:
						this.DirectiveArg();
					}
					this.jj_la1[7] = this.jj_gen;
					this.ConsumeToken(6);
				}
				if (directiveType == DirectiveType.LINE)
				{
					result = aSTDirective;
				}
				else
				{
					this.directiveStack.Push(directive);
					ASTBlock n = new ASTBlock(this, 10);
					bool flag3 = true;
					this.nodeTree.OpenNodeScope(n);
					try
					{
						while (true)
						{
							this.Statement();
							int currentTokenKind2 = this.GetCurrentTokenKind();
							int num = currentTokenKind2;
							switch (num)
							{
							case 5:
							case 6:
							case 8:
							case 9:
							case 16:
							case 17:
							case 18:
							case 19:
							case 20:
							case 21:
							case 24:
								break;
							case 7:
							case 10:
							case 11:
							case 12:
							case 13:
							case 14:
							case 15:
							case 22:
							case 23:
								goto IL_2D2;
							default:
								if (num != 44)
								{
									switch (num)
									{
									case 47:
									case 49:
									case 52:
									case 56:
									case 57:
									case 58:
									case 59:
										continue;
									}
									goto Block_26;
								}
								break;
							}
						}
						Block_26:
						IL_2D2:
						this.jj_la1[8] = this.jj_gen;
					}
					catch (System.Exception ex)
					{
						if (flag3)
						{
							this.nodeTree.ClearNodeScope(n);
							flag3 = false;
						}
						else
						{
							this.nodeTree.PopNode();
						}
						if (ex is SystemException)
						{
							throw (SystemException)ex;
						}
						if (ex is ParseException)
						{
							throw (ParseException)ex;
						}
						throw (ApplicationException)ex;
					}
					finally
					{
						if (flag3)
						{
							this.nodeTree.CloseNodeScope(n, true);
						}
						this.directiveStack.Pop();
					}
					this.ConsumeToken(43);
					this.nodeTree.CloseNodeScope(aSTDirective, true);
					flag = false;
					if (flag2)
					{
						Macro.processAndRegister(this.rsvc, aSTDirective, this.currentTemplateName);
					}
					result = aSTDirective;
				}
			}
			catch (System.Exception ex2)
			{
				if (flag)
				{
					this.nodeTree.ClearNodeScope(aSTDirective);
					flag = false;
				}
				else
				{
					this.nodeTree.PopNode();
				}
				if (ex2 is SystemException)
				{
					throw (SystemException)ex2;
				}
				if (ex2 is ParseException)
				{
					throw (ParseException)ex2;
				}
				throw (ApplicationException)ex2;
			}
			finally
			{
				if (flag)
				{
					this.nodeTree.CloseNodeScope(aSTDirective, true);
				}
			}
			return result;
		}

		private int GetCurrentTokenKind()
		{
			return (this.jj_ntk_Renamed_Field == -1) ? this.jj_ntk() : this.jj_ntk_Renamed_Field;
		}

		private void ConsumeWhiteSpaces()
		{
			int currentTokenKind = this.GetCurrentTokenKind();
			if (currentTokenKind != 23)
			{
				this.jj_la1[6] = this.jj_gen;
			}
			else
			{
				this.ConsumeToken(23);
			}
		}

		public void ObjectArray()
		{
			ASTObjectArray n = new ASTObjectArray(this, 11);
			bool flag = true;
			this.nodeTree.OpenNodeScope(n);
			try
			{
				this.ConsumeToken(1);
				int currentTokenKind = this.GetCurrentTokenKind();
				if (currentTokenKind <= 26)
				{
					if (currentTokenKind != 1)
					{
						switch (currentTokenKind)
						{
						case 23:
						case 24:
						case 25:
						case 26:
							break;
						default:
							goto IL_AA;
						}
					}
				}
				else if (currentTokenKind != 49)
				{
					switch (currentTokenKind)
					{
					case 56:
					case 58:
						break;
					case 57:
						goto IL_AA;
					default:
						goto IL_AA;
					}
				}
				this.Parameter();
				while (true)
				{
					currentTokenKind = this.GetCurrentTokenKind();
					if (currentTokenKind != 3)
					{
						break;
					}
					this.ConsumeToken(3);
					this.Parameter();
				}
				this.jj_la1[9] = this.jj_gen;
				goto IL_BB;
				IL_AA:
				this.jj_la1[10] = this.jj_gen;
				IL_BB:
				this.ConsumeToken(2);
			}
			catch (System.Exception ex)
			{
				if (flag)
				{
					this.nodeTree.ClearNodeScope(n);
					flag = false;
				}
				else
				{
					this.nodeTree.PopNode();
				}
				if (ex is SystemException)
				{
					throw (SystemException)ex;
				}
				if (ex is ParseException)
				{
					throw (ParseException)ex;
				}
				throw (ApplicationException)ex;
			}
			finally
			{
				if (flag)
				{
					this.nodeTree.CloseNodeScope(n, true);
				}
			}
		}

		public void IntegerRange()
		{
			ASTIntegerRange n = new ASTIntegerRange(this, 12);
			bool flag = true;
			this.nodeTree.OpenNodeScope(n);
			try
			{
				this.ConsumeToken(1);
				int currentTokenKind = this.GetCurrentTokenKind();
				if (currentTokenKind != 23)
				{
					this.jj_la1[11] = this.jj_gen;
				}
				else
				{
					this.ConsumeToken(23);
				}
				currentTokenKind = this.GetCurrentTokenKind();
				if (currentTokenKind != 49)
				{
					switch (currentTokenKind)
					{
					case 56:
					case 58:
						this.Reference();
						goto IL_9E;
					}
					this.jj_la1[12] = this.jj_gen;
					this.ConsumeToken(-1);
					throw new ParseException();
				}
				this.NumberLiteral();
				IL_9E:
				currentTokenKind = this.GetCurrentTokenKind();
				if (currentTokenKind != 23)
				{
					this.jj_la1[13] = this.jj_gen;
				}
				else
				{
					this.ConsumeToken(23);
				}
				this.ConsumeToken(4);
				currentTokenKind = this.GetCurrentTokenKind();
				if (currentTokenKind != 23)
				{
					this.jj_la1[14] = this.jj_gen;
				}
				else
				{
					this.ConsumeToken(23);
				}
				currentTokenKind = this.GetCurrentTokenKind();
				if (currentTokenKind != 49)
				{
					switch (currentTokenKind)
					{
					case 56:
					case 58:
						this.Reference();
						goto IL_14C;
					}
					this.jj_la1[15] = this.jj_gen;
					this.ConsumeToken(-1);
					throw new ParseException();
				}
				this.NumberLiteral();
				IL_14C:
				currentTokenKind = this.GetCurrentTokenKind();
				if (currentTokenKind != 23)
				{
					this.jj_la1[16] = this.jj_gen;
				}
				else
				{
					this.ConsumeToken(23);
				}
				this.ConsumeToken(2);
			}
			catch (System.Exception ex)
			{
				if (flag)
				{
					this.nodeTree.ClearNodeScope(n);
					flag = false;
				}
				else
				{
					this.nodeTree.PopNode();
				}
				if (ex is SystemException)
				{
					throw (SystemException)ex;
				}
				if (ex is ParseException)
				{
					throw (ParseException)ex;
				}
				throw (ApplicationException)ex;
			}
			finally
			{
				if (flag)
				{
					this.nodeTree.CloseNodeScope(n, true);
				}
			}
		}

		public void Parameter()
		{
			int currentTokenKind = this.GetCurrentTokenKind();
			if (currentTokenKind != 23)
			{
				this.jj_la1[17] = this.jj_gen;
			}
			else
			{
				this.ConsumeToken(23);
			}
			currentTokenKind = this.GetCurrentTokenKind();
			if (currentTokenKind != 24)
			{
				this.jj_la1[18] = this.jj_gen;
				if (this.jj_2_4(2147483647))
				{
					this.IntegerRange();
				}
				else
				{
					currentTokenKind = this.GetCurrentTokenKind();
					if (currentTokenKind <= 26)
					{
						if (currentTokenKind == 1)
						{
							this.ObjectArray();
							goto IL_FE;
						}
						switch (currentTokenKind)
						{
						case 25:
							this.True();
							goto IL_FE;
						case 26:
							this.False();
							goto IL_FE;
						}
					}
					else
					{
						if (currentTokenKind == 49)
						{
							this.NumberLiteral();
							goto IL_FE;
						}
						switch (currentTokenKind)
						{
						case 56:
						case 58:
							this.Reference();
							goto IL_FE;
						}
					}
					this.jj_la1[19] = this.jj_gen;
					this.ConsumeToken(-1);
					throw new ParseException();
					IL_FE:;
				}
			}
			else
			{
				this.StringLiteral();
			}
			currentTokenKind = this.GetCurrentTokenKind();
			if (currentTokenKind != 23)
			{
				this.jj_la1[20] = this.jj_gen;
			}
			else
			{
				this.ConsumeToken(23);
			}
		}

		public void Method()
		{
			ASTMethod n = new ASTMethod(this, 13);
			bool flag = true;
			this.nodeTree.OpenNodeScope(n);
			try
			{
				this.Identifier();
				this.ConsumeToken(5);
				int currentTokenKind = this.GetCurrentTokenKind();
				if (currentTokenKind <= 26)
				{
					if (currentTokenKind != 1)
					{
						switch (currentTokenKind)
						{
						case 23:
						case 24:
						case 25:
						case 26:
							break;
						default:
							goto IL_B1;
						}
					}
				}
				else if (currentTokenKind != 49)
				{
					switch (currentTokenKind)
					{
					case 56:
					case 58:
						break;
					case 57:
						goto IL_B1;
					default:
						goto IL_B1;
					}
				}
				this.Parameter();
				while (true)
				{
					currentTokenKind = this.GetCurrentTokenKind();
					if (currentTokenKind != 3)
					{
						break;
					}
					this.ConsumeToken(3);
					this.Parameter();
				}
				this.jj_la1[21] = this.jj_gen;
				goto IL_C2;
				IL_B1:
				this.jj_la1[22] = this.jj_gen;
				IL_C2:
				this.ConsumeToken(7);
			}
			catch (System.Exception ex)
			{
				if (flag)
				{
					this.nodeTree.ClearNodeScope(n);
					flag = false;
				}
				else
				{
					this.nodeTree.PopNode();
				}
				if (ex is SystemException)
				{
					throw (SystemException)ex;
				}
				if (ex is ParseException)
				{
					throw (ParseException)ex;
				}
				throw (ApplicationException)ex;
			}
			finally
			{
				if (flag)
				{
					this.nodeTree.CloseNodeScope(n, true);
				}
			}
		}

		public void Reference()
		{
			ASTReference n = new ASTReference(this, 14);
			bool flag = true;
			this.nodeTree.OpenNodeScope(n);
			try
			{
				switch (this.GetCurrentTokenKind())
				{
				case 56:
					this.ConsumeToken(56);
					while (this.jj_2_5(2))
					{
						this.ConsumeToken(57);
						if (this.jj_2_6(3))
						{
							this.Method();
						}
						else
						{
							int currentTokenKind = this.GetCurrentTokenKind();
							if (currentTokenKind != 56)
							{
								this.jj_la1[23] = this.jj_gen;
								this.ConsumeToken(-1);
								throw new ParseException();
							}
							this.Identifier();
						}
					}
					goto IL_177;
				case 58:
					this.ConsumeToken(58);
					this.ConsumeToken(56);
					while (this.jj_2_7(2))
					{
						this.ConsumeToken(57);
						if (this.jj_2_8(3))
						{
							this.Method();
						}
						else
						{
							int currentTokenKind = this.GetCurrentTokenKind();
							if (currentTokenKind != 56)
							{
								this.jj_la1[24] = this.jj_gen;
								this.ConsumeToken(-1);
								throw new ParseException();
							}
							this.Identifier();
						}
					}
					this.ConsumeToken(59);
					goto IL_177;
				}
				this.jj_la1[25] = this.jj_gen;
				this.ConsumeToken(-1);
				throw new ParseException();
				IL_177:;
			}
			catch (System.Exception ex)
			{
				if (flag)
				{
					this.nodeTree.ClearNodeScope(n);
					flag = false;
				}
				else
				{
					this.nodeTree.PopNode();
				}
				if (ex is SystemException)
				{
					throw (SystemException)ex;
				}
				if (ex is ParseException)
				{
					throw (ParseException)ex;
				}
				throw (ApplicationException)ex;
			}
			finally
			{
				if (flag)
				{
					this.nodeTree.CloseNodeScope(n, true);
				}
			}
		}

		public void True()
		{
			ASTTrue n = new ASTTrue(this, 15);
			bool flag = true;
			this.nodeTree.OpenNodeScope(n);
			try
			{
				this.ConsumeToken(25);
			}
			finally
			{
				if (flag)
				{
					this.nodeTree.CloseNodeScope(n, true);
				}
			}
		}

		public void False()
		{
			ASTFalse n = new ASTFalse(this, 16);
			bool flag = true;
			this.nodeTree.OpenNodeScope(n);
			try
			{
				this.ConsumeToken(26);
			}
			finally
			{
				if (flag)
				{
					this.nodeTree.CloseNodeScope(n, true);
				}
			}
		}

		public void Text()
		{
			ASTText n = new ASTText(this, 17);
			bool flag = true;
			this.nodeTree.OpenNodeScope(n);
			try
			{
				int currentTokenKind = this.GetCurrentTokenKind();
				if (currentTokenKind <= 18)
				{
					switch (currentTokenKind)
					{
					case 5:
						this.ConsumeToken(5);
						goto IL_EB;
					case 6:
						this.ConsumeToken(6);
						goto IL_EB;
					default:
						switch (currentTokenKind)
						{
						case 17:
							this.ConsumeToken(17);
							goto IL_EB;
						case 18:
							this.ConsumeToken(18);
							goto IL_EB;
						}
						break;
					}
				}
				else
				{
					if (currentTokenKind == 24)
					{
						this.ConsumeToken(24);
						goto IL_EB;
					}
					if (currentTokenKind == 49)
					{
						this.ConsumeToken(49);
						goto IL_EB;
					}
					switch (currentTokenKind)
					{
					case 57:
						this.ConsumeToken(57);
						goto IL_EB;
					case 58:
						this.ConsumeToken(58);
						goto IL_EB;
					case 59:
						this.ConsumeToken(59);
						goto IL_EB;
					}
				}
				this.jj_la1[26] = this.jj_gen;
				this.ConsumeToken(-1);
				throw new ParseException();
				IL_EB:;
			}
			finally
			{
				if (flag)
				{
					this.nodeTree.CloseNodeScope(n, true);
				}
			}
		}

		public void IfStatement()
		{
			ASTIfStatement n = new ASTIfStatement(this, 18);
			bool flag = true;
			this.nodeTree.OpenNodeScope(n);
			try
			{
				this.ConsumeToken(44);
				int currentTokenKind = this.GetCurrentTokenKind();
				if (currentTokenKind != 23)
				{
					this.jj_la1[27] = this.jj_gen;
				}
				else
				{
					this.ConsumeToken(23);
				}
				this.ConsumeToken(5);
				this.Expression();
				this.ConsumeToken(6);
				ASTBlock n2 = new ASTBlock(this, 10);
				bool flag2 = true;
				this.nodeTree.OpenNodeScope(n2);
				try
				{
					while (true)
					{
						this.Statement();
						currentTokenKind = this.GetCurrentTokenKind();
						switch (currentTokenKind)
						{
						case 5:
						case 6:
						case 8:
						case 9:
						case 16:
						case 17:
						case 18:
						case 19:
						case 20:
						case 21:
						case 24:
							break;
						case 7:
						case 10:
						case 11:
						case 12:
						case 13:
						case 14:
						case 15:
						case 22:
						case 23:
							goto IL_135;
						default:
							if (currentTokenKind != 44)
							{
								switch (currentTokenKind)
								{
								case 47:
								case 49:
								case 52:
								case 56:
								case 57:
								case 58:
								case 59:
									continue;
								}
								goto Block_13;
							}
							break;
						}
					}
					Block_13:
					IL_135:
					this.jj_la1[28] = this.jj_gen;
				}
				catch (System.Exception ex)
				{
					if (flag2)
					{
						this.nodeTree.ClearNodeScope(n2);
						flag2 = false;
					}
					else
					{
						this.nodeTree.PopNode();
					}
					if (ex is SystemException)
					{
						throw (SystemException)ex;
					}
					if (ex is ParseException)
					{
						throw (ParseException)ex;
					}
					throw (ApplicationException)ex;
				}
				finally
				{
					if (flag2)
					{
						this.nodeTree.CloseNodeScope(n2, true);
					}
				}
				currentTokenKind = this.GetCurrentTokenKind();
				if (currentTokenKind != 45)
				{
					this.jj_la1[30] = this.jj_gen;
				}
				else
				{
					do
					{
						this.ElseIfStatement();
						currentTokenKind = this.GetCurrentTokenKind();
					}
					while (currentTokenKind == 45);
					this.jj_la1[29] = this.jj_gen;
				}
				currentTokenKind = this.GetCurrentTokenKind();
				if (currentTokenKind != 46)
				{
					this.jj_la1[31] = this.jj_gen;
				}
				else
				{
					this.ElseStatement();
				}
				this.ConsumeToken(43);
			}
			catch (System.Exception ex2)
			{
				if (flag)
				{
					this.nodeTree.ClearNodeScope(n);
					flag = false;
				}
				else
				{
					this.nodeTree.PopNode();
				}
				if (ex2 is SystemException)
				{
					throw (SystemException)ex2;
				}
				if (ex2 is ParseException)
				{
					throw (ParseException)ex2;
				}
				throw (ApplicationException)ex2;
			}
			finally
			{
				if (flag)
				{
					this.nodeTree.CloseNodeScope(n, true);
				}
			}
		}

		public void ElseStatement()
		{
			ASTElseStatement n = new ASTElseStatement(this, 19);
			bool flag = true;
			this.nodeTree.OpenNodeScope(n);
			try
			{
				this.ConsumeToken(46);
				ASTBlock n2 = new ASTBlock(this, 10);
				bool flag2 = true;
				this.nodeTree.OpenNodeScope(n2);
				try
				{
					while (true)
					{
						this.Statement();
						int currentTokenKind = this.GetCurrentTokenKind();
						switch (currentTokenKind)
						{
						case 5:
						case 6:
						case 8:
						case 9:
						case 16:
						case 17:
						case 18:
						case 19:
						case 20:
						case 21:
						case 24:
							break;
						case 7:
						case 10:
						case 11:
						case 12:
						case 13:
						case 14:
						case 15:
						case 22:
						case 23:
							goto IL_F2;
						default:
							if (currentTokenKind != 44)
							{
								switch (currentTokenKind)
								{
								case 47:
								case 49:
								case 52:
								case 56:
								case 57:
								case 58:
								case 59:
									continue;
								}
								goto Block_9;
							}
							break;
						}
					}
					Block_9:
					IL_F2:
					this.jj_la1[32] = this.jj_gen;
				}
				catch (System.Exception ex)
				{
					if (flag2)
					{
						this.nodeTree.ClearNodeScope(n2);
						flag2 = false;
					}
					else
					{
						this.nodeTree.PopNode();
					}
					if (ex is SystemException)
					{
						throw (SystemException)ex;
					}
					if (ex is ParseException)
					{
						throw (ParseException)ex;
					}
					throw (ApplicationException)ex;
				}
				finally
				{
					if (flag2)
					{
						this.nodeTree.CloseNodeScope(n2, true);
					}
				}
			}
			catch (System.Exception ex2)
			{
				if (flag)
				{
					this.nodeTree.ClearNodeScope(n);
					flag = false;
				}
				else
				{
					this.nodeTree.PopNode();
				}
				if (ex2 is SystemException)
				{
					throw (SystemException)ex2;
				}
				if (ex2 is ParseException)
				{
					throw (ParseException)ex2;
				}
				throw (ApplicationException)ex2;
			}
			finally
			{
				if (flag)
				{
					this.nodeTree.CloseNodeScope(n, true);
				}
			}
		}

		public void ElseIfStatement()
		{
			ASTElseIfStatement n = new ASTElseIfStatement(this, 20);
			bool flag = true;
			this.nodeTree.OpenNodeScope(n);
			try
			{
				this.ConsumeToken(45);
				int currentTokenKind = this.GetCurrentTokenKind();
				if (currentTokenKind != 23)
				{
					this.jj_la1[33] = this.jj_gen;
				}
				else
				{
					this.ConsumeToken(23);
				}
				this.ConsumeToken(5);
				this.Expression();
				this.ConsumeToken(6);
				ASTBlock n2 = new ASTBlock(this, 10);
				bool flag2 = true;
				this.nodeTree.OpenNodeScope(n2);
				try
				{
					while (true)
					{
						this.Statement();
						currentTokenKind = this.GetCurrentTokenKind();
						switch (currentTokenKind)
						{
						case 5:
						case 6:
						case 8:
						case 9:
						case 16:
						case 17:
						case 18:
						case 19:
						case 20:
						case 21:
						case 24:
							break;
						case 7:
						case 10:
						case 11:
						case 12:
						case 13:
						case 14:
						case 15:
						case 22:
						case 23:
							goto IL_135;
						default:
							if (currentTokenKind != 44)
							{
								switch (currentTokenKind)
								{
								case 47:
								case 49:
								case 52:
								case 56:
								case 57:
								case 58:
								case 59:
									continue;
								}
								goto Block_10;
							}
							break;
						}
					}
					Block_10:
					IL_135:
					this.jj_la1[34] = this.jj_gen;
				}
				catch (System.Exception ex)
				{
					if (flag2)
					{
						this.nodeTree.ClearNodeScope(n2);
						flag2 = false;
					}
					else
					{
						this.nodeTree.PopNode();
					}
					if (ex is SystemException)
					{
						throw (SystemException)ex;
					}
					if (ex is ParseException)
					{
						throw (ParseException)ex;
					}
					throw (ApplicationException)ex;
				}
				finally
				{
					if (flag2)
					{
						this.nodeTree.CloseNodeScope(n2, true);
					}
				}
			}
			catch (System.Exception ex2)
			{
				if (flag)
				{
					this.nodeTree.ClearNodeScope(n);
					flag = false;
				}
				else
				{
					this.nodeTree.PopNode();
				}
				if (ex2 is SystemException)
				{
					throw (SystemException)ex2;
				}
				if (ex2 is ParseException)
				{
					throw (ParseException)ex2;
				}
				throw (ApplicationException)ex2;
			}
			finally
			{
				if (flag)
				{
					this.nodeTree.CloseNodeScope(n, true);
				}
			}
		}

		public void SetDirective()
		{
			ASTSetDirective n = new ASTSetDirective(this, 21);
			bool flag = true;
			this.nodeTree.OpenNodeScope(n);
			try
			{
				this.ConsumeToken(9);
				if (this.jj_2_9(2))
				{
					this.ConsumeToken(23);
				}
				this.ConsumeToken(5);
				this.Expression();
				this.ConsumeToken(6);
				this.token_source.inSet = false;
				int currentTokenKind = this.GetCurrentTokenKind();
				if (currentTokenKind != 27)
				{
					this.jj_la1[35] = this.jj_gen;
				}
				else
				{
					this.ConsumeToken(27);
				}
			}
			catch (System.Exception ex)
			{
				if (flag)
				{
					this.nodeTree.ClearNodeScope(n);
					flag = false;
				}
				else
				{
					this.nodeTree.PopNode();
				}
				if (ex is SystemException)
				{
					throw (SystemException)ex;
				}
				if (ex is ParseException)
				{
					throw (ParseException)ex;
				}
				throw (ApplicationException)ex;
			}
			finally
			{
				if (flag)
				{
					this.nodeTree.CloseNodeScope(n, true);
				}
			}
		}

		public void StopStatement()
		{
			this.ConsumeToken(47);
		}

		public void Expression()
		{
			ASTExpression n = new ASTExpression(this, 22);
			bool flag = true;
			this.nodeTree.OpenNodeScope(n);
			try
			{
				if (!this.jj_2_10(2147483647))
				{
					int currentTokenKind = this.GetCurrentTokenKind();
					if (currentTokenKind <= 26)
					{
						if (currentTokenKind != 1 && currentTokenKind != 5)
						{
							switch (currentTokenKind)
							{
							case 23:
							case 24:
							case 25:
							case 26:
								break;
							default:
								goto IL_9C;
							}
						}
					}
					else if (currentTokenKind != 41 && currentTokenKind != 49)
					{
						switch (currentTokenKind)
						{
						case 56:
						case 58:
							break;
						case 57:
							goto IL_9C;
						default:
							goto IL_9C;
						}
					}
					this.ConditionalOrExpression();
					goto IL_BA;
					IL_9C:
					this.jj_la1[36] = this.jj_gen;
					this.ConsumeToken(-1);
					throw new ParseException();
				}
				this.Assignment();
				IL_BA:;
			}
			catch (System.Exception ex)
			{
				if (flag)
				{
					this.nodeTree.ClearNodeScope(n);
					flag = false;
				}
				else
				{
					this.nodeTree.PopNode();
				}
				if (ex is SystemException)
				{
					throw (SystemException)ex;
				}
				if (ex is ParseException)
				{
					throw (ParseException)ex;
				}
				throw (ApplicationException)ex;
			}
			finally
			{
				if (flag)
				{
					this.nodeTree.CloseNodeScope(n, true);
				}
			}
		}

		public void Assignment()
		{
			ASTAssignment n = new ASTAssignment(this, 23);
			bool flag = true;
			this.nodeTree.OpenNodeScope(n);
			try
			{
				this.PrimaryExpression();
				this.ConsumeToken(42);
				this.Expression();
			}
			catch (System.Exception ex)
			{
				if (flag)
				{
					this.nodeTree.ClearNodeScope(n);
					flag = false;
				}
				else
				{
					this.nodeTree.PopNode();
				}
				if (ex is SystemException)
				{
					throw (SystemException)ex;
				}
				if (ex is ParseException)
				{
					throw (ParseException)ex;
				}
				throw (ApplicationException)ex;
			}
			finally
			{
				if (flag)
				{
					this.nodeTree.CloseNodeScope(n, 2);
				}
			}
		}

		public void ConditionalOrExpression()
		{
			this.ConditionalAndExpression();
			while (true)
			{
				int currentTokenKind = this.GetCurrentTokenKind();
				if (currentTokenKind != 34)
				{
					break;
				}
				this.ConsumeToken(34);
				ASTOrNode n = new ASTOrNode(this, 24);
				bool flag = true;
				this.nodeTree.OpenNodeScope(n);
				try
				{
					this.ConditionalAndExpression();
				}
				catch (System.Exception ex)
				{
					if (flag)
					{
						this.nodeTree.ClearNodeScope(n);
						flag = false;
					}
					else
					{
						this.nodeTree.PopNode();
					}
					if (ex is SystemException)
					{
						throw (SystemException)ex;
					}
					if (ex is ParseException)
					{
						throw (ParseException)ex;
					}
					throw (ApplicationException)ex;
				}
				finally
				{
					if (flag)
					{
						this.nodeTree.CloseNodeScope(n, 2);
					}
				}
			}
			this.jj_la1[37] = this.jj_gen;
		}

		public void ConditionalAndExpression()
		{
			this.EqualityExpression();
			while (true)
			{
				int currentTokenKind = this.GetCurrentTokenKind();
				if (currentTokenKind != 33)
				{
					break;
				}
				this.ConsumeToken(33);
				ASTAndNode n = new ASTAndNode(this, 25);
				bool flag = true;
				this.nodeTree.OpenNodeScope(n);
				try
				{
					this.EqualityExpression();
				}
				catch (System.Exception ex)
				{
					if (flag)
					{
						this.nodeTree.ClearNodeScope(n);
						flag = false;
					}
					else
					{
						this.nodeTree.PopNode();
					}
					if (ex is SystemException)
					{
						throw (SystemException)ex;
					}
					if (ex is ParseException)
					{
						throw (ParseException)ex;
					}
					throw (ApplicationException)ex;
				}
				finally
				{
					if (flag)
					{
						this.nodeTree.CloseNodeScope(n, 2);
					}
				}
			}
			this.jj_la1[38] = this.jj_gen;
		}

		public void EqualityExpression()
		{
			this.RelationalExpression();
			while (true)
			{
				switch (this.GetCurrentTokenKind())
				{
				case 39:
				case 40:
					switch (this.GetCurrentTokenKind())
					{
					case 39:
					{
						this.ConsumeToken(39);
						ASTEQNode n = new ASTEQNode(this, 26);
						bool flag = true;
						this.nodeTree.OpenNodeScope(n);
						try
						{
							this.RelationalExpression();
						}
						catch (System.Exception ex)
						{
							if (flag)
							{
								this.nodeTree.ClearNodeScope(n);
								flag = false;
							}
							else
							{
								this.nodeTree.PopNode();
							}
							if (ex is SystemException)
							{
								throw (SystemException)ex;
							}
							if (ex is ParseException)
							{
								throw (ParseException)ex;
							}
							throw (ApplicationException)ex;
						}
						finally
						{
							if (flag)
							{
								this.nodeTree.CloseNodeScope(n, 2);
							}
						}
						continue;
					}
					case 40:
					{
						this.ConsumeToken(40);
						ASTNENode n2 = new ASTNENode(this, 27);
						bool flag2 = true;
						this.nodeTree.OpenNodeScope(n2);
						try
						{
							this.RelationalExpression();
						}
						catch (System.Exception ex2)
						{
							if (flag2)
							{
								this.nodeTree.ClearNodeScope(n2);
								flag2 = false;
							}
							else
							{
								this.nodeTree.PopNode();
							}
							if (ex2 is SystemException)
							{
								throw (SystemException)ex2;
							}
							if (ex2 is ParseException)
							{
								throw (ParseException)ex2;
							}
							throw (ApplicationException)ex2;
						}
						finally
						{
							if (flag2)
							{
								this.nodeTree.CloseNodeScope(n2, 2);
							}
						}
						continue;
					}
					}
					goto Block_2;
					//continue;
				}
				break;
			}
			this.jj_la1[39] = this.jj_gen;
			return;
			Block_2:
			this.jj_la1[40] = this.jj_gen;
			this.ConsumeToken(-1);
			throw new ParseException();
		}

		public void RelationalExpression()
		{
			this.AdditiveExpression();
			while (true)
			{
				switch (this.GetCurrentTokenKind())
				{
				case 35:
				case 36:
				case 37:
				case 38:
					switch (this.GetCurrentTokenKind())
					{
					case 35:
					{
						this.ConsumeToken(35);
						ASTLTNode n = new ASTLTNode(this, 28);
						bool flag = true;
						this.nodeTree.OpenNodeScope(n);
						try
						{
							this.AdditiveExpression();
						}
						catch (System.Exception ex)
						{
							if (flag)
							{
								this.nodeTree.ClearNodeScope(n);
								flag = false;
							}
							else
							{
								this.nodeTree.PopNode();
							}
							if (ex is SystemException)
							{
								throw (SystemException)ex;
							}
							if (ex is ParseException)
							{
								throw (ParseException)ex;
							}
							throw (ApplicationException)ex;
						}
						finally
						{
							if (flag)
							{
								this.nodeTree.CloseNodeScope(n, 2);
							}
						}
						continue;
					}
					case 36:
					{
						this.ConsumeToken(36);
						ASTLENode n2 = new ASTLENode(this, 30);
						bool flag2 = true;
						this.nodeTree.OpenNodeScope(n2);
						try
						{
							this.AdditiveExpression();
						}
						catch (System.Exception ex2)
						{
							if (flag2)
							{
								this.nodeTree.ClearNodeScope(n2);
								flag2 = false;
							}
							else
							{
								this.nodeTree.PopNode();
							}
							if (ex2 is SystemException)
							{
								throw (SystemException)ex2;
							}
							if (ex2 is ParseException)
							{
								throw (ParseException)ex2;
							}
							throw (ApplicationException)ex2;
						}
						finally
						{
							if (flag2)
							{
								this.nodeTree.CloseNodeScope(n2, 2);
							}
						}
						continue;
					}
					case 37:
					{
						this.ConsumeToken(37);
						ASTGTNode n3 = new ASTGTNode(this, 29);
						bool flag3 = true;
						this.nodeTree.OpenNodeScope(n3);
						try
						{
							this.AdditiveExpression();
						}
						catch (System.Exception ex3)
						{
							if (flag3)
							{
								this.nodeTree.ClearNodeScope(n3);
								flag3 = false;
							}
							else
							{
								this.nodeTree.PopNode();
							}
							if (ex3 is SystemException)
							{
								throw (SystemException)ex3;
							}
							if (ex3 is ParseException)
							{
								throw (ParseException)ex3;
							}
							throw (ApplicationException)ex3;
						}
						finally
						{
							if (flag3)
							{
								this.nodeTree.CloseNodeScope(n3, 2);
							}
						}
						continue;
					}
					case 38:
					{
						this.ConsumeToken(38);
						ASTGENode n4 = new ASTGENode(this, 31);
						bool flag4 = true;
						this.nodeTree.OpenNodeScope(n4);
						try
						{
							this.AdditiveExpression();
						}
						catch (System.Exception ex4)
						{
							if (flag4)
							{
								this.nodeTree.ClearNodeScope(n4);
								flag4 = false;
							}
							else
							{
								this.nodeTree.PopNode();
							}
							if (ex4 is SystemException)
							{
								throw (SystemException)ex4;
							}
							if (ex4 is ParseException)
							{
								throw (ParseException)ex4;
							}
							throw (ApplicationException)ex4;
						}
						finally
						{
							if (flag4)
							{
								this.nodeTree.CloseNodeScope(n4, 2);
							}
						}
						continue;
					}
					}
					goto Block_2;
					//continue;
				}
				break;
			}
			this.jj_la1[41] = this.jj_gen;
			return;
			Block_2:
			this.jj_la1[42] = this.jj_gen;
			this.ConsumeToken(-1);
			throw new ParseException();
		}

		public void AdditiveExpression()
		{
			this.MultiplicativeExpression();
			while (true)
			{
				switch (this.GetCurrentTokenKind())
				{
				case 28:
				case 29:
					switch (this.GetCurrentTokenKind())
					{
					case 28:
					{
						this.ConsumeToken(28);
						ASTSubtractNode n = new ASTSubtractNode(this, 33);
						bool flag = true;
						this.nodeTree.OpenNodeScope(n);
						try
						{
							this.MultiplicativeExpression();
						}
						catch (System.Exception ex)
						{
							if (flag)
							{
								this.nodeTree.ClearNodeScope(n);
								flag = false;
							}
							else
							{
								this.nodeTree.PopNode();
							}
							if (ex is SystemException)
							{
								throw (SystemException)ex;
							}
							if (ex is ParseException)
							{
								throw (ParseException)ex;
							}
							throw (ApplicationException)ex;
						}
						finally
						{
							if (flag)
							{
								this.nodeTree.CloseNodeScope(n, 2);
							}
						}
						continue;
					}
					case 29:
					{
						this.ConsumeToken(29);
						ASTAddNode n2 = new ASTAddNode(this, 32);
						bool flag2 = true;
						this.nodeTree.OpenNodeScope(n2);
						try
						{
							this.MultiplicativeExpression();
						}
						catch (System.Exception ex2)
						{
							if (flag2)
							{
								this.nodeTree.ClearNodeScope(n2);
								flag2 = false;
							}
							else
							{
								this.nodeTree.PopNode();
							}
							if (ex2 is SystemException)
							{
								throw (SystemException)ex2;
							}
							if (ex2 is ParseException)
							{
								throw (ParseException)ex2;
							}
							throw (ApplicationException)ex2;
						}
						finally
						{
							if (flag2)
							{
								this.nodeTree.CloseNodeScope(n2, 2);
							}
						}
						continue;
					}
					}
					goto Block_2;
					
				}
				break;
			}
			this.jj_la1[43] = this.jj_gen;
			return;
			Block_2:
			this.jj_la1[44] = this.jj_gen;
			this.ConsumeToken(-1);
			throw new ParseException();
		}

		public void MultiplicativeExpression()
		{
			this.UnaryExpression();
			while (true)
			{
				switch (this.GetCurrentTokenKind())
				{
				case 30:
				case 31:
				case 32:
					switch (this.GetCurrentTokenKind())
					{
					case 30:
					{
						this.ConsumeToken(30);
						ASTMulNode n = new ASTMulNode(this, 34);
						bool flag = true;
						this.nodeTree.OpenNodeScope(n);
						try
						{
							this.UnaryExpression();
						}
						catch (System.Exception ex)
						{
							if (flag)
							{
								this.nodeTree.ClearNodeScope(n);
								flag = false;
							}
							else
							{
								this.nodeTree.PopNode();
							}
							if (ex is SystemException)
							{
								throw (SystemException)ex;
							}
							if (ex is ParseException)
							{
								throw (ParseException)ex;
							}
							throw (ApplicationException)ex;
						}
						finally
						{
							if (flag)
							{
								this.nodeTree.CloseNodeScope(n, 2);
							}
						}
						continue;
					}
					case 31:
					{
						this.ConsumeToken(31);
						ASTDivNode n2 = new ASTDivNode(this, 35);
						bool flag2 = true;
						this.nodeTree.OpenNodeScope(n2);
						try
						{
							this.UnaryExpression();
						}
						catch (System.Exception ex2)
						{
							if (flag2)
							{
								this.nodeTree.ClearNodeScope(n2);
								flag2 = false;
							}
							else
							{
								this.nodeTree.PopNode();
							}
							if (ex2 is SystemException)
							{
								throw (SystemException)ex2;
							}
							if (ex2 is ParseException)
							{
								throw (ParseException)ex2;
							}
							throw (ApplicationException)ex2;
						}
						finally
						{
							if (flag2)
							{
								this.nodeTree.CloseNodeScope(n2, 2);
							}
						}
						continue;
					}
					case 32:
					{
						this.ConsumeToken(32);
						ASTModNode n3 = new ASTModNode(this, 36);
						bool flag3 = true;
						this.nodeTree.OpenNodeScope(n3);
						try
						{
							this.UnaryExpression();
						}
						catch (System.Exception ex3)
						{
							if (flag3)
							{
								this.nodeTree.ClearNodeScope(n3);
								flag3 = false;
							}
							else
							{
								this.nodeTree.PopNode();
							}
							if (ex3 is SystemException)
							{
								throw (SystemException)ex3;
							}
							if (ex3 is ParseException)
							{
								throw (ParseException)ex3;
							}
							throw (ApplicationException)ex3;
						}
						finally
						{
							if (flag3)
							{
								this.nodeTree.CloseNodeScope(n3, 2);
							}
						}
						continue;
					}
					}
					goto Block_2;
					
				}
				break;
			}
			this.jj_la1[45] = this.jj_gen;
			return;
			Block_2:
			this.jj_la1[46] = this.jj_gen;
			this.ConsumeToken(-1);
			throw new ParseException();
		}

		public void UnaryExpression()
		{
			int currentTokenKind;
			if (!this.jj_2_11(2))
			{
				currentTokenKind = this.GetCurrentTokenKind();
				if (currentTokenKind <= 5)
				{
					if (currentTokenKind != 1 && currentTokenKind != 5)
					{
						goto IL_153;
					}
				}
				else
				{
					switch (currentTokenKind)
					{
					case 23:
					case 24:
					case 25:
					case 26:
						break;
					default:
						if (currentTokenKind != 49)
						{
							switch (currentTokenKind)
							{
							case 56:
							case 58:
								break;
							case 57:
								goto IL_153;
							default:
								goto IL_153;
							}
						}
						break;
					}
				}
				this.PrimaryExpression();
				return;
				IL_153:
				this.jj_la1[48] = this.jj_gen;
				this.ConsumeToken(-1);
				throw new ParseException();
			}
			currentTokenKind = this.GetCurrentTokenKind();
			if (currentTokenKind != 23)
			{
				this.jj_la1[47] = this.jj_gen;
			}
			else
			{
				this.ConsumeToken(23);
			}
			this.ConsumeToken(41);
			ASTNotNode n = new ASTNotNode(this, 37);
			bool flag = true;
			this.nodeTree.OpenNodeScope(n);
			try
			{
				this.UnaryExpression();
			}
			catch (System.Exception ex)
			{
				if (flag)
				{
					this.nodeTree.ClearNodeScope(n);
					flag = false;
				}
				else
				{
					this.nodeTree.PopNode();
				}
				if (ex is SystemException)
				{
					throw (SystemException)ex;
				}
				if (ex is ParseException)
				{
					throw (ParseException)ex;
				}
				throw (ApplicationException)ex;
			}
			finally
			{
				if (flag)
				{
					this.nodeTree.CloseNodeScope(n, 1);
				}
			}
		}

		public void PrimaryExpression()
		{
			int currentTokenKind = this.GetCurrentTokenKind();
			if (currentTokenKind != 23)
			{
				this.jj_la1[49] = this.jj_gen;
			}
			else
			{
				this.ConsumeToken(23);
			}
			currentTokenKind = this.GetCurrentTokenKind();
			if (currentTokenKind != 24)
			{
				if (currentTokenKind != 49)
				{
					switch (currentTokenKind)
					{
					case 56:
					case 58:
						this.Reference();
						goto IL_11A;
					}
					this.jj_la1[50] = this.jj_gen;
					if (this.jj_2_12(2147483647))
					{
						this.IntegerRange();
					}
					else
					{
						currentTokenKind = this.GetCurrentTokenKind();
						if (currentTokenKind != 1)
						{
							if (currentTokenKind != 5)
							{
								switch (currentTokenKind)
								{
								case 25:
									this.True();
									break;
								case 26:
									this.False();
									break;
								default:
									this.jj_la1[51] = this.jj_gen;
									this.ConsumeToken(-1);
									throw new ParseException();
								}
							}
							else
							{
								this.ConsumeToken(5);
								this.Expression();
								this.ConsumeToken(6);
							}
						}
						else
						{
							this.ObjectArray();
						}
					}
				}
				else
				{
					this.NumberLiteral();
				}
			}
			else
			{
				this.StringLiteral();
			}
			IL_11A:
			currentTokenKind = this.GetCurrentTokenKind();
			if (currentTokenKind != 23)
			{
				this.jj_la1[52] = this.jj_gen;
			}
			else
			{
				this.ConsumeToken(23);
			}
		}

		private bool jj_2_1(int xla)
		{
			this.jj_la = xla;
			this.jj_lastpos = (this.jj_scanpos = this.token);
			bool result = !this.jj_3_1();
			this.Save(0, xla);
			return result;
		}

		private bool jj_2_2(int xla)
		{
			this.jj_la = xla;
			this.jj_lastpos = (this.jj_scanpos = this.token);
			bool result = !this.jj_3_2();
			this.Save(1, xla);
			return result;
		}

		private bool jj_2_3(int xla)
		{
			this.jj_la = xla;
			this.jj_lastpos = (this.jj_scanpos = this.token);
			bool result = !this.jj_3_3();
			this.Save(2, xla);
			return result;
		}

		private bool jj_2_4(int xla)
		{
			this.jj_la = xla;
			this.jj_lastpos = (this.jj_scanpos = this.token);
			bool result = !this.jj_3_4();
			this.Save(3, xla);
			return result;
		}

		private bool jj_2_5(int xla)
		{
			this.jj_la = xla;
			this.jj_lastpos = (this.jj_scanpos = this.token);
			bool result = !this.jj_3_5();
			this.Save(4, xla);
			return result;
		}

		private bool jj_2_6(int xla)
		{
			this.jj_la = xla;
			this.jj_lastpos = (this.jj_scanpos = this.token);
			bool result = !this.jj_3_6();
			this.Save(5, xla);
			return result;
		}

		private bool jj_2_7(int xla)
		{
			this.jj_la = xla;
			this.jj_lastpos = (this.jj_scanpos = this.token);
			bool result = !this.jj_3_7();
			this.Save(6, xla);
			return result;
		}

		private bool jj_2_8(int xla)
		{
			this.jj_la = xla;
			this.jj_lastpos = (this.jj_scanpos = this.token);
			bool result = !this.jj_3_8();
			this.Save(7, xla);
			return result;
		}

		private bool jj_2_9(int xla)
		{
			this.jj_la = xla;
			this.jj_lastpos = (this.jj_scanpos = this.token);
			bool result = !this.jj_3_9();
			this.Save(8, xla);
			return result;
		}

		private bool jj_2_10(int xla)
		{
			this.jj_la = xla;
			this.jj_lastpos = (this.jj_scanpos = this.token);
			bool result = !this.jj_3_10();
			this.Save(9, xla);
			return result;
		}

		private bool jj_2_11(int xla)
		{
			this.jj_la = xla;
			this.jj_lastpos = (this.jj_scanpos = this.token);
			bool result = !this.jj_3_11();
			this.Save(10, xla);
			return result;
		}

		private bool jj_2_12(int xla)
		{
			this.jj_la = xla;
			this.jj_lastpos = (this.jj_scanpos = this.token);
			bool result = !this.jj_3_12();
			this.Save(11, xla);
			return result;
		}

		private bool jj_3R_58()
		{
			return this.ScanToken(25) || (this.jj_la == 0 && this.jj_scanpos == this.jj_lastpos && false);
		}

		private bool jj_3_7()
		{
			bool result;
			if (this.ScanToken(57))
			{
				result = true;
			}
			else if (this.jj_la == 0 && this.jj_scanpos == this.jj_lastpos)
			{
				result = false;
			}
			else
			{
				Token token = this.jj_scanpos;
				if (this.jj_3_8())
				{
					this.jj_scanpos = token;
					if (this.jj_3R_30())
					{
						result = true;
						return result;
					}
					if (this.jj_la == 0 && this.jj_scanpos == this.jj_lastpos)
					{
						result = false;
						return result;
					}
				}
				else if (this.jj_la == 0 && this.jj_scanpos == this.jj_lastpos)
				{
					result = false;
					return result;
				}
				result = false;
			}
			return result;
		}

		private bool jj_3R_42()
		{
			bool result;
			if (this.jj_3R_54())
			{
				result = true;
			}
			else if (this.jj_la == 0 && this.jj_scanpos == this.jj_lastpos)
			{
				result = false;
			}
			else
			{
				Token token;
				while (true)
				{
					token = this.jj_scanpos;
					if (this.jj_3R_88())
					{
						break;
					}
					if (this.jj_la == 0 && this.jj_scanpos == this.jj_lastpos)
					{
						goto Block_6;
					}
				}
				this.jj_scanpos = token;
				result = false;
				return result;
				Block_6:
				result = false;
			}
			return result;
		}

		private bool jj_3_5()
		{
			bool result;
			if (this.ScanToken(57))
			{
				result = true;
			}
			else if (this.jj_la == 0 && this.jj_scanpos == this.jj_lastpos)
			{
				result = false;
			}
			else
			{
				Token token = this.jj_scanpos;
				if (this.jj_3_6())
				{
					this.jj_scanpos = token;
					if (this.jj_3R_28())
					{
						result = true;
						return result;
					}
					if (this.jj_la == 0 && this.jj_scanpos == this.jj_lastpos)
					{
						result = false;
						return result;
					}
				}
				else if (this.jj_la == 0 && this.jj_scanpos == this.jj_lastpos)
				{
					result = false;
					return result;
				}
				result = false;
			}
			return result;
		}

		private bool jj_3R_39()
		{
			bool result;
			if (this.ScanToken(58))
			{
				result = true;
			}
			else if (this.jj_la == 0 && this.jj_scanpos == this.jj_lastpos)
			{
				result = false;
			}
			else if (this.ScanToken(56))
			{
				result = true;
			}
			else if (this.jj_la == 0 && this.jj_scanpos == this.jj_lastpos)
			{
				result = false;
			}
			else
			{
				Token token;
				while (true)
				{
					token = this.jj_scanpos;
					if (this.jj_3_7())
					{
						break;
					}
					if (this.jj_la == 0 && this.jj_scanpos == this.jj_lastpos)
					{
						goto Block_9;
					}
				}
				this.jj_scanpos = token;
				result = (this.ScanToken(59) || (this.jj_la == 0 && this.jj_scanpos == this.jj_lastpos && false));
				return result;
				Block_9:
				result = false;
			}
			return result;
		}

		private bool jj_3_12()
		{
			bool result;
			if (this.ScanToken(1))
			{
				result = true;
			}
			else if (this.jj_la == 0 && this.jj_scanpos == this.jj_lastpos)
			{
				result = false;
			}
			else
			{
				Token token = this.jj_scanpos;
				if (this.jj_3R_34())
				{
					this.jj_scanpos = token;
				}
				else if (this.jj_la == 0 && this.jj_scanpos == this.jj_lastpos)
				{
					result = false;
					return result;
				}
				token = this.jj_scanpos;
				if (this.jj_3R_35())
				{
					this.jj_scanpos = token;
					if (this.jj_3R_36())
					{
						result = true;
						return result;
					}
					if (this.jj_la == 0 && this.jj_scanpos == this.jj_lastpos)
					{
						result = false;
						return result;
					}
				}
				else if (this.jj_la == 0 && this.jj_scanpos == this.jj_lastpos)
				{
					result = false;
					return result;
				}
				token = this.jj_scanpos;
				if (this.jj_3R_37())
				{
					this.jj_scanpos = token;
				}
				else if (this.jj_la == 0 && this.jj_scanpos == this.jj_lastpos)
				{
					result = false;
					return result;
				}
				result = (this.ScanToken(4) || (this.jj_la == 0 && this.jj_scanpos == this.jj_lastpos && false));
			}
			return result;
		}

		private bool jj_3R_24()
		{
			return this.ScanToken(23) || (this.jj_la == 0 && this.jj_scanpos == this.jj_lastpos && false);
		}

		private bool jj_3R_38()
		{
			bool result;
			if (this.ScanToken(56))
			{
				result = true;
			}
			else if (this.jj_la == 0 && this.jj_scanpos == this.jj_lastpos)
			{
				result = false;
			}
			else
			{
				Token token;
				while (true)
				{
					token = this.jj_scanpos;
					if (this.jj_3_5())
					{
						break;
					}
					if (this.jj_la == 0 && this.jj_scanpos == this.jj_lastpos)
					{
						goto Block_6;
					}
				}
				this.jj_scanpos = token;
				result = false;
				return result;
				Block_6:
				result = false;
			}
			return result;
		}

		private bool jj_3R_52()
		{
			return this.ScanToken(23) || (this.jj_la == 0 && this.jj_scanpos == this.jj_lastpos && false);
		}

		private bool jj_3R_51()
		{
			return this.ScanToken(5) || ((this.jj_la != 0 || this.jj_scanpos != this.jj_lastpos) && (this.jj_3R_60() || ((this.jj_la != 0 || this.jj_scanpos != this.jj_lastpos) && (this.ScanToken(6) || (this.jj_la == 0 && this.jj_scanpos == this.jj_lastpos && false)))));
		}

		private bool jj_3R_19()
		{
			Token token = this.jj_scanpos;
			bool result;
			if (this.jj_3R_38())
			{
				this.jj_scanpos = token;
				if (this.jj_3R_39())
				{
					result = true;
					return result;
				}
				if (this.jj_la == 0 && this.jj_scanpos == this.jj_lastpos)
				{
					result = false;
					return result;
				}
			}
			else if (this.jj_la == 0 && this.jj_scanpos == this.jj_lastpos)
			{
				result = false;
				return result;
			}
			result = false;
			return result;
		}

		private bool jj_3R_32()
		{
			return this.ScanToken(23) || (this.jj_la == 0 && this.jj_scanpos == this.jj_lastpos && false);
		}

		private bool jj_3R_50()
		{
			return this.jj_3R_59() || (this.jj_la == 0 && this.jj_scanpos == this.jj_lastpos && false);
		}

		private bool jj_3R_41()
		{
			return this.ScanToken(56) || (this.jj_la == 0 && this.jj_scanpos == this.jj_lastpos && false);
		}

		private bool jj_3R_49()
		{
			return this.jj_3R_58() || (this.jj_la == 0 && this.jj_scanpos == this.jj_lastpos && false);
		}

		private bool jj_3R_48()
		{
			return this.jj_3R_57() || (this.jj_la == 0 && this.jj_scanpos == this.jj_lastpos && false);
		}

		private bool jj_3R_47()
		{
			return this.jj_3R_56() || (this.jj_la == 0 && this.jj_scanpos == this.jj_lastpos && false);
		}

		private bool jj_3R_46()
		{
			return this.jj_3R_19() || (this.jj_la == 0 && this.jj_scanpos == this.jj_lastpos && false);
		}

		private bool jj_3R_45()
		{
			return this.jj_3R_40() || (this.jj_la == 0 && this.jj_scanpos == this.jj_lastpos && false);
		}

		private bool jj_3_4()
		{
			bool result;
			if (this.ScanToken(1))
			{
				result = true;
			}
			else if (this.jj_la == 0 && this.jj_scanpos == this.jj_lastpos)
			{
				result = false;
			}
			else
			{
				Token token = this.jj_scanpos;
				if (this.jj_3R_24())
				{
					this.jj_scanpos = token;
				}
				else if (this.jj_la == 0 && this.jj_scanpos == this.jj_lastpos)
				{
					result = false;
					return result;
				}
				token = this.jj_scanpos;
				if (this.jj_3R_25())
				{
					this.jj_scanpos = token;
					if (this.jj_3R_26())
					{
						result = true;
						return result;
					}
					if (this.jj_la == 0 && this.jj_scanpos == this.jj_lastpos)
					{
						result = false;
						return result;
					}
				}
				else if (this.jj_la == 0 && this.jj_scanpos == this.jj_lastpos)
				{
					result = false;
					return result;
				}
				token = this.jj_scanpos;
				if (this.jj_3R_27())
				{
					this.jj_scanpos = token;
				}
				else if (this.jj_la == 0 && this.jj_scanpos == this.jj_lastpos)
				{
					result = false;
					return result;
				}
				result = (this.ScanToken(4) || (this.jj_la == 0 && this.jj_scanpos == this.jj_lastpos && false));
			}
			return result;
		}

		private bool jj_3R_44()
		{
			return this.jj_3R_55() || (this.jj_la == 0 && this.jj_scanpos == this.jj_lastpos && false);
		}

		private bool jj_3R_29()
		{
			bool result;
			if (this.jj_3R_41())
			{
				result = true;
			}
			else if (this.jj_la == 0 && this.jj_scanpos == this.jj_lastpos)
			{
				result = false;
			}
			else if (this.ScanToken(5))
			{
				result = true;
			}
			else if (this.jj_la == 0 && this.jj_scanpos == this.jj_lastpos)
			{
				result = false;
			}
			else
			{
				Token token = this.jj_scanpos;
				if (this.jj_3R_42())
				{
					this.jj_scanpos = token;
				}
				else if (this.jj_la == 0 && this.jj_scanpos == this.jj_lastpos)
				{
					result = false;
					return result;
				}
				result = (this.ScanToken(7) || (this.jj_la == 0 && this.jj_scanpos == this.jj_lastpos && false));
			}
			return result;
		}

		private bool jj_3R_43()
		{
			return this.ScanToken(23) || (this.jj_la == 0 && this.jj_scanpos == this.jj_lastpos && false);
		}

		private bool jj_3R_31()
		{
			Token token = this.jj_scanpos;
			bool result;
			if (this.jj_3R_43())
			{
				this.jj_scanpos = token;
			}
			else if (this.jj_la == 0 && this.jj_scanpos == this.jj_lastpos)
			{
				result = false;
				return result;
			}
			token = this.jj_scanpos;
			if (this.jj_3R_44())
			{
				this.jj_scanpos = token;
				if (this.jj_3R_45())
				{
					this.jj_scanpos = token;
					if (this.jj_3R_46())
					{
						this.jj_scanpos = token;
						if (this.jj_3R_47())
						{
							this.jj_scanpos = token;
							if (this.jj_3R_48())
							{
								this.jj_scanpos = token;
								if (this.jj_3R_49())
								{
									this.jj_scanpos = token;
									if (this.jj_3R_50())
									{
										this.jj_scanpos = token;
										if (this.jj_3R_51())
										{
											result = true;
											return result;
										}
										if (this.jj_la == 0 && this.jj_scanpos == this.jj_lastpos)
										{
											result = false;
											return result;
										}
									}
									else if (this.jj_la == 0 && this.jj_scanpos == this.jj_lastpos)
									{
										result = false;
										return result;
									}
								}
								else if (this.jj_la == 0 && this.jj_scanpos == this.jj_lastpos)
								{
									result = false;
									return result;
								}
							}
							else if (this.jj_la == 0 && this.jj_scanpos == this.jj_lastpos)
							{
								result = false;
								return result;
							}
						}
						else if (this.jj_la == 0 && this.jj_scanpos == this.jj_lastpos)
						{
							result = false;
							return result;
						}
					}
					else if (this.jj_la == 0 && this.jj_scanpos == this.jj_lastpos)
					{
						result = false;
						return result;
					}
				}
				else if (this.jj_la == 0 && this.jj_scanpos == this.jj_lastpos)
				{
					result = false;
					return result;
				}
			}
			else if (this.jj_la == 0 && this.jj_scanpos == this.jj_lastpos)
			{
				result = false;
				return result;
			}
			token = this.jj_scanpos;
			if (this.jj_3R_52())
			{
				this.jj_scanpos = token;
			}
			else if (this.jj_la == 0 && this.jj_scanpos == this.jj_lastpos)
			{
				result = false;
				return result;
			}
			result = false;
			return result;
		}

		private bool jj_3R_73()
		{
			return this.ScanToken(23) || (this.jj_la == 0 && this.jj_scanpos == this.jj_lastpos && false);
		}

		private bool jj_3R_68()
		{
			return this.jj_3R_40() || (this.jj_la == 0 && this.jj_scanpos == this.jj_lastpos && false);
		}

		private bool jj_3R_106()
		{
			return this.ScanToken(32) || ((this.jj_la != 0 || this.jj_scanpos != this.jj_lastpos) && (this.jj_3R_33() || (this.jj_la == 0 && this.jj_scanpos == this.jj_lastpos && false)));
		}

		private bool jj_3R_67()
		{
			return this.jj_3R_19() || (this.jj_la == 0 && this.jj_scanpos == this.jj_lastpos && false);
		}

		private bool jj_3R_105()
		{
			return this.ScanToken(31) || ((this.jj_la != 0 || this.jj_scanpos != this.jj_lastpos) && (this.jj_3R_33() || (this.jj_la == 0 && this.jj_scanpos == this.jj_lastpos && false)));
		}

		private bool jj_3R_83()
		{
			return this.ScanToken(23) || (this.jj_la == 0 && this.jj_scanpos == this.jj_lastpos && false);
		}

		private bool jj_3R_66()
		{
			return this.jj_3R_59() || (this.jj_la == 0 && this.jj_scanpos == this.jj_lastpos && false);
		}

		private bool jj_3R_33()
		{
			Token token = this.jj_scanpos;
			bool result;
			if (this.jj_3_11())
			{
				this.jj_scanpos = token;
				if (this.jj_3R_53())
				{
					result = true;
					return result;
				}
				if (this.jj_la == 0 && this.jj_scanpos == this.jj_lastpos)
				{
					result = false;
					return result;
				}
			}
			else if (this.jj_la == 0 && this.jj_scanpos == this.jj_lastpos)
			{
				result = false;
				return result;
			}
			result = false;
			return result;
		}

		private bool jj_3_11()
		{
			Token token = this.jj_scanpos;
			bool result;
			if (this.jj_3R_32())
			{
				this.jj_scanpos = token;
			}
			else if (this.jj_la == 0 && this.jj_scanpos == this.jj_lastpos)
			{
				result = false;
				return result;
			}
			result = (this.ScanToken(41) || ((this.jj_la != 0 || this.jj_scanpos != this.jj_lastpos) && (this.jj_3R_33() || (this.jj_la == 0 && this.jj_scanpos == this.jj_lastpos && false))));
			return result;
		}

		private bool jj_3R_53()
		{
			return this.jj_3R_31() || (this.jj_la == 0 && this.jj_scanpos == this.jj_lastpos && false);
		}

		private bool jj_3R_104()
		{
			return this.ScanToken(30) || ((this.jj_la != 0 || this.jj_scanpos != this.jj_lastpos) && (this.jj_3R_33() || (this.jj_la == 0 && this.jj_scanpos == this.jj_lastpos && false)));
		}

		private bool jj_3R_101()
		{
			Token token = this.jj_scanpos;
			bool result;
			if (this.jj_3R_104())
			{
				this.jj_scanpos = token;
				if (this.jj_3R_105())
				{
					this.jj_scanpos = token;
					if (this.jj_3R_106())
					{
						result = true;
						return result;
					}
					if (this.jj_la == 0 && this.jj_scanpos == this.jj_lastpos)
					{
						result = false;
						return result;
					}
				}
				else if (this.jj_la == 0 && this.jj_scanpos == this.jj_lastpos)
				{
					result = false;
					return result;
				}
			}
			else if (this.jj_la == 0 && this.jj_scanpos == this.jj_lastpos)
			{
				result = false;
				return result;
			}
			result = false;
			return result;
		}

		private bool jj_3R_65()
		{
			return this.jj_3R_58() || (this.jj_la == 0 && this.jj_scanpos == this.jj_lastpos && false);
		}

		private bool jj_3R_55()
		{
			return this.ScanToken(24) || (this.jj_la == 0 && this.jj_scanpos == this.jj_lastpos && false);
		}

		private bool jj_3R_64()
		{
			return this.jj_3R_57() || (this.jj_la == 0 && this.jj_scanpos == this.jj_lastpos && false);
		}

		private bool jj_3R_63()
		{
			return this.jj_3R_56() || (this.jj_la == 0 && this.jj_scanpos == this.jj_lastpos && false);
		}

		private bool jj_3R_62()
		{
			return this.jj_3R_55() || (this.jj_la == 0 && this.jj_scanpos == this.jj_lastpos && false);
		}

		private bool jj_3R_40()
		{
			return this.ScanToken(49) || (this.jj_la == 0 && this.jj_scanpos == this.jj_lastpos && false);
		}

		private bool jj_3R_75()
		{
			return this.jj_3R_40() || (this.jj_la == 0 && this.jj_scanpos == this.jj_lastpos && false);
		}

		private bool jj_3R_80()
		{
			return this.ScanToken(3) || ((this.jj_la != 0 || this.jj_scanpos != this.jj_lastpos) && (this.jj_3R_54() || (this.jj_la == 0 && this.jj_scanpos == this.jj_lastpos && false)));
		}

		private bool jj_3R_71()
		{
			return this.jj_3R_40() || (this.jj_la == 0 && this.jj_scanpos == this.jj_lastpos && false);
		}

		private bool jj_3R_95()
		{
			bool result;
			if (this.jj_3R_33())
			{
				result = true;
			}
			else if (this.jj_la == 0 && this.jj_scanpos == this.jj_lastpos)
			{
				result = false;
			}
			else
			{
				Token token;
				while (true)
				{
					token = this.jj_scanpos;
					if (this.jj_3R_101())
					{
						break;
					}
					if (this.jj_la == 0 && this.jj_scanpos == this.jj_lastpos)
					{
						goto Block_6;
					}
				}
				this.jj_scanpos = token;
				result = false;
				return result;
				Block_6:
				result = false;
			}
			return result;
		}

		private bool jj_3R_61()
		{
			return this.ScanToken(23) || (this.jj_la == 0 && this.jj_scanpos == this.jj_lastpos && false);
		}

		private bool jj_3R_54()
		{
			Token token = this.jj_scanpos;
			bool result;
			if (this.jj_3R_61())
			{
				this.jj_scanpos = token;
			}
			else if (this.jj_la == 0 && this.jj_scanpos == this.jj_lastpos)
			{
				result = false;
				return result;
			}
			token = this.jj_scanpos;
			if (this.jj_3R_62())
			{
				this.jj_scanpos = token;
				if (this.jj_3R_63())
				{
					this.jj_scanpos = token;
					if (this.jj_3R_64())
					{
						this.jj_scanpos = token;
						if (this.jj_3R_65())
						{
							this.jj_scanpos = token;
							if (this.jj_3R_66())
							{
								this.jj_scanpos = token;
								if (this.jj_3R_67())
								{
									this.jj_scanpos = token;
									if (this.jj_3R_68())
									{
										result = true;
										return result;
									}
									if (this.jj_la == 0 && this.jj_scanpos == this.jj_lastpos)
									{
										result = false;
										return result;
									}
								}
								else if (this.jj_la == 0 && this.jj_scanpos == this.jj_lastpos)
								{
									result = false;
									return result;
								}
							}
							else if (this.jj_la == 0 && this.jj_scanpos == this.jj_lastpos)
							{
								result = false;
								return result;
							}
						}
						else if (this.jj_la == 0 && this.jj_scanpos == this.jj_lastpos)
						{
							result = false;
							return result;
						}
					}
					else if (this.jj_la == 0 && this.jj_scanpos == this.jj_lastpos)
					{
						result = false;
						return result;
					}
				}
				else if (this.jj_la == 0 && this.jj_scanpos == this.jj_lastpos)
				{
					result = false;
					return result;
				}
			}
			else if (this.jj_la == 0 && this.jj_scanpos == this.jj_lastpos)
			{
				result = false;
				return result;
			}
			token = this.jj_scanpos;
			if (this.jj_3R_83())
			{
				this.jj_scanpos = token;
			}
			else if (this.jj_la == 0 && this.jj_scanpos == this.jj_lastpos)
			{
				result = false;
				return result;
			}
			result = false;
			return result;
		}

		private bool jj_3R_103()
		{
			return this.ScanToken(28) || ((this.jj_la != 0 || this.jj_scanpos != this.jj_lastpos) && (this.jj_3R_95() || (this.jj_la == 0 && this.jj_scanpos == this.jj_lastpos && false)));
		}

		private bool jj_3R_102()
		{
			return this.ScanToken(29) || ((this.jj_la != 0 || this.jj_scanpos != this.jj_lastpos) && (this.jj_3R_95() || (this.jj_la == 0 && this.jj_scanpos == this.jj_lastpos && false)));
		}

		private bool jj_3R_96()
		{
			Token token = this.jj_scanpos;
			bool result;
			if (this.jj_3R_102())
			{
				this.jj_scanpos = token;
				if (this.jj_3R_103())
				{
					result = true;
					return result;
				}
				if (this.jj_la == 0 && this.jj_scanpos == this.jj_lastpos)
				{
					result = false;
					return result;
				}
			}
			else if (this.jj_la == 0 && this.jj_scanpos == this.jj_lastpos)
			{
				result = false;
				return result;
			}
			result = false;
			return result;
		}

		private bool jj_3R_69()
		{
			return this.ScanToken(23) || (this.jj_la == 0 && this.jj_scanpos == this.jj_lastpos && false);
		}

		private bool jj_3R_91()
		{
			bool result;
			if (this.jj_3R_95())
			{
				result = true;
			}
			else if (this.jj_la == 0 && this.jj_scanpos == this.jj_lastpos)
			{
				result = false;
			}
			else
			{
				Token token;
				while (true)
				{
					token = this.jj_scanpos;
					if (this.jj_3R_96())
					{
						break;
					}
					if (this.jj_la == 0 && this.jj_scanpos == this.jj_lastpos)
					{
						goto Block_6;
					}
				}
				this.jj_scanpos = token;
				result = false;
				return result;
				Block_6:
				result = false;
			}
			return result;
		}

		private bool jj_3R_100()
		{
			return this.ScanToken(38) || ((this.jj_la != 0 || this.jj_scanpos != this.jj_lastpos) && (this.jj_3R_91() || (this.jj_la == 0 && this.jj_scanpos == this.jj_lastpos && false)));
		}

		private bool jj_3R_76()
		{
			return this.ScanToken(23) || (this.jj_la == 0 && this.jj_scanpos == this.jj_lastpos && false);
		}

		private bool jj_3R_99()
		{
			return this.ScanToken(36) || ((this.jj_la != 0 || this.jj_scanpos != this.jj_lastpos) && (this.jj_3R_91() || (this.jj_la == 0 && this.jj_scanpos == this.jj_lastpos && false)));
		}

		private bool jj_3R_74()
		{
			return this.jj_3R_19() || (this.jj_la == 0 && this.jj_scanpos == this.jj_lastpos && false);
		}

		private bool jj_3R_98()
		{
			return this.ScanToken(37) || ((this.jj_la != 0 || this.jj_scanpos != this.jj_lastpos) && (this.jj_3R_91() || (this.jj_la == 0 && this.jj_scanpos == this.jj_lastpos && false)));
		}

		private bool jj_3R_70()
		{
			return this.jj_3R_19() || (this.jj_la == 0 && this.jj_scanpos == this.jj_lastpos && false);
		}

		private bool jj_3R_72()
		{
			return this.ScanToken(23) || (this.jj_la == 0 && this.jj_scanpos == this.jj_lastpos && false);
		}

		private bool jj_3R_77()
		{
			bool result;
			if (this.jj_3R_54())
			{
				result = true;
			}
			else if (this.jj_la == 0 && this.jj_scanpos == this.jj_lastpos)
			{
				result = false;
			}
			else
			{
				Token token;
				while (true)
				{
					token = this.jj_scanpos;
					if (this.jj_3R_80())
					{
						break;
					}
					if (this.jj_la == 0 && this.jj_scanpos == this.jj_lastpos)
					{
						goto Block_6;
					}
				}
				this.jj_scanpos = token;
				result = false;
				return result;
				Block_6:
				result = false;
			}
			return result;
		}

		private bool jj_3R_97()
		{
			return this.ScanToken(35) || ((this.jj_la != 0 || this.jj_scanpos != this.jj_lastpos) && (this.jj_3R_91() || (this.jj_la == 0 && this.jj_scanpos == this.jj_lastpos && false)));
		}

		private bool jj_3R_92()
		{
			Token token = this.jj_scanpos;
			bool result;
			if (this.jj_3R_97())
			{
				this.jj_scanpos = token;
				if (this.jj_3R_98())
				{
					this.jj_scanpos = token;
					if (this.jj_3R_99())
					{
						this.jj_scanpos = token;
						if (this.jj_3R_100())
						{
							result = true;
							return result;
						}
						if (this.jj_la == 0 && this.jj_scanpos == this.jj_lastpos)
						{
							result = false;
							return result;
						}
					}
					else if (this.jj_la == 0 && this.jj_scanpos == this.jj_lastpos)
					{
						result = false;
						return result;
					}
				}
				else if (this.jj_la == 0 && this.jj_scanpos == this.jj_lastpos)
				{
					result = false;
					return result;
				}
			}
			else if (this.jj_la == 0 && this.jj_scanpos == this.jj_lastpos)
			{
				result = false;
				return result;
			}
			result = false;
			return result;
		}

		private bool jj_3R_56()
		{
			bool result;
			if (this.ScanToken(1))
			{
				result = true;
			}
			else if (this.jj_la == 0 && this.jj_scanpos == this.jj_lastpos)
			{
				result = false;
			}
			else
			{
				Token token = this.jj_scanpos;
				if (this.jj_3R_69())
				{
					this.jj_scanpos = token;
				}
				else if (this.jj_la == 0 && this.jj_scanpos == this.jj_lastpos)
				{
					result = false;
					return result;
				}
				token = this.jj_scanpos;
				if (this.jj_3R_70())
				{
					this.jj_scanpos = token;
					if (this.jj_3R_71())
					{
						result = true;
						return result;
					}
					if (this.jj_la == 0 && this.jj_scanpos == this.jj_lastpos)
					{
						result = false;
						return result;
					}
				}
				else if (this.jj_la == 0 && this.jj_scanpos == this.jj_lastpos)
				{
					result = false;
					return result;
				}
				token = this.jj_scanpos;
				if (this.jj_3R_72())
				{
					this.jj_scanpos = token;
				}
				else if (this.jj_la == 0 && this.jj_scanpos == this.jj_lastpos)
				{
					result = false;
					return result;
				}
				if (this.ScanToken(4))
				{
					result = true;
				}
				else if (this.jj_la == 0 && this.jj_scanpos == this.jj_lastpos)
				{
					result = false;
				}
				else
				{
					token = this.jj_scanpos;
					if (this.jj_3R_73())
					{
						this.jj_scanpos = token;
					}
					else if (this.jj_la == 0 && this.jj_scanpos == this.jj_lastpos)
					{
						result = false;
						return result;
					}
					token = this.jj_scanpos;
					if (this.jj_3R_74())
					{
						this.jj_scanpos = token;
						if (this.jj_3R_75())
						{
							result = true;
							return result;
						}
						if (this.jj_la == 0 && this.jj_scanpos == this.jj_lastpos)
						{
							result = false;
							return result;
						}
					}
					else if (this.jj_la == 0 && this.jj_scanpos == this.jj_lastpos)
					{
						result = false;
						return result;
					}
					token = this.jj_scanpos;
					if (this.jj_3R_76())
					{
						this.jj_scanpos = token;
					}
					else if (this.jj_la == 0 && this.jj_scanpos == this.jj_lastpos)
					{
						result = false;
						return result;
					}
					result = (this.ScanToken(2) || (this.jj_la == 0 && this.jj_scanpos == this.jj_lastpos && false));
				}
			}
			return result;
		}

		private bool jj_3R_89()
		{
			bool result;
			if (this.jj_3R_91())
			{
				result = true;
			}
			else if (this.jj_la == 0 && this.jj_scanpos == this.jj_lastpos)
			{
				result = false;
			}
			else
			{
				Token token;
				while (true)
				{
					token = this.jj_scanpos;
					if (this.jj_3R_92())
					{
						break;
					}
					if (this.jj_la == 0 && this.jj_scanpos == this.jj_lastpos)
					{
						goto Block_6;
					}
				}
				this.jj_scanpos = token;
				result = false;
				return result;
				Block_6:
				result = false;
			}
			return result;
		}

		private bool jj_3R_94()
		{
			return this.ScanToken(40) || ((this.jj_la != 0 || this.jj_scanpos != this.jj_lastpos) && (this.jj_3R_89() || (this.jj_la == 0 && this.jj_scanpos == this.jj_lastpos && false)));
		}

		private bool jj_3R_93()
		{
			return this.ScanToken(39) || ((this.jj_la != 0 || this.jj_scanpos != this.jj_lastpos) && (this.jj_3R_89() || (this.jj_la == 0 && this.jj_scanpos == this.jj_lastpos && false)));
		}

		private bool jj_3R_90()
		{
			Token token = this.jj_scanpos;
			bool result;
			if (this.jj_3R_93())
			{
				this.jj_scanpos = token;
				if (this.jj_3R_94())
				{
					result = true;
					return result;
				}
				if (this.jj_la == 0 && this.jj_scanpos == this.jj_lastpos)
				{
					result = false;
					return result;
				}
			}
			else if (this.jj_la == 0 && this.jj_scanpos == this.jj_lastpos)
			{
				result = false;
				return result;
			}
			result = false;
			return result;
		}

		private bool jj_3R_57()
		{
			bool result;
			if (this.ScanToken(1))
			{
				result = true;
			}
			else if (this.jj_la == 0 && this.jj_scanpos == this.jj_lastpos)
			{
				result = false;
			}
			else
			{
				Token token = this.jj_scanpos;
				if (this.jj_3R_77())
				{
					this.jj_scanpos = token;
				}
				else if (this.jj_la == 0 && this.jj_scanpos == this.jj_lastpos)
				{
					result = false;
					return result;
				}
				result = (this.ScanToken(2) || (this.jj_la == 0 && this.jj_scanpos == this.jj_lastpos && false));
			}
			return result;
		}

		private bool jj_3R_86()
		{
			bool result;
			if (this.jj_3R_89())
			{
				result = true;
			}
			else if (this.jj_la == 0 && this.jj_scanpos == this.jj_lastpos)
			{
				result = false;
			}
			else
			{
				Token token;
				while (true)
				{
					token = this.jj_scanpos;
					if (this.jj_3R_90())
					{
						break;
					}
					if (this.jj_la == 0 && this.jj_scanpos == this.jj_lastpos)
					{
						goto Block_6;
					}
				}
				this.jj_scanpos = token;
				result = false;
				return result;
				Block_6:
				result = false;
			}
			return result;
		}

		private bool jj_3R_87()
		{
			return this.ScanToken(33) || ((this.jj_la != 0 || this.jj_scanpos != this.jj_lastpos) && (this.jj_3R_86() || (this.jj_la == 0 && this.jj_scanpos == this.jj_lastpos && false)));
		}

		private bool jj_3R_84()
		{
			bool result;
			if (this.jj_3R_86())
			{
				result = true;
			}
			else if (this.jj_la == 0 && this.jj_scanpos == this.jj_lastpos)
			{
				result = false;
			}
			else
			{
				Token token;
				while (true)
				{
					token = this.jj_scanpos;
					if (this.jj_3R_87())
					{
						break;
					}
					if (this.jj_la == 0 && this.jj_scanpos == this.jj_lastpos)
					{
						goto Block_6;
					}
				}
				this.jj_scanpos = token;
				result = false;
				return result;
				Block_6:
				result = false;
			}
			return result;
		}

		private bool jj_3R_85()
		{
			return this.ScanToken(34) || ((this.jj_la != 0 || this.jj_scanpos != this.jj_lastpos) && (this.jj_3R_84() || (this.jj_la == 0 && this.jj_scanpos == this.jj_lastpos && false)));
		}

		private bool jj_3_2()
		{
			return this.ScanToken(16) || (this.jj_la == 0 && this.jj_scanpos == this.jj_lastpos && false);
		}

		private bool jj_3_10()
		{
			return this.jj_3R_31() || ((this.jj_la != 0 || this.jj_scanpos != this.jj_lastpos) && (this.ScanToken(42) || (this.jj_la == 0 && this.jj_scanpos == this.jj_lastpos && false)));
		}

		private bool jj_3R_82()
		{
			bool result;
			if (this.jj_3R_84())
			{
				result = true;
			}
			else if (this.jj_la == 0 && this.jj_scanpos == this.jj_lastpos)
			{
				result = false;
			}
			else
			{
				Token token;
				while (true)
				{
					token = this.jj_scanpos;
					if (this.jj_3R_85())
					{
						break;
					}
					if (this.jj_la == 0 && this.jj_scanpos == this.jj_lastpos)
					{
						goto Block_6;
					}
				}
				this.jj_scanpos = token;
				result = false;
				return result;
				Block_6:
				result = false;
			}
			return result;
		}

		private bool jj_3R_81()
		{
			return this.jj_3R_31() || ((this.jj_la != 0 || this.jj_scanpos != this.jj_lastpos) && (this.ScanToken(42) || ((this.jj_la != 0 || this.jj_scanpos != this.jj_lastpos) && (this.jj_3R_60() || (this.jj_la == 0 && this.jj_scanpos == this.jj_lastpos && false)))));
		}

		private bool jj_3R_79()
		{
			return this.jj_3R_82() || (this.jj_la == 0 && this.jj_scanpos == this.jj_lastpos && false);
		}

		private bool jj_3R_78()
		{
			return this.jj_3R_81() || (this.jj_la == 0 && this.jj_scanpos == this.jj_lastpos && false);
		}

		private bool jj_3R_60()
		{
			Token token = this.jj_scanpos;
			bool result;
			if (this.jj_3R_78())
			{
				this.jj_scanpos = token;
				if (this.jj_3R_79())
				{
					result = true;
					return result;
				}
				if (this.jj_la == 0 && this.jj_scanpos == this.jj_lastpos)
				{
					result = false;
					return result;
				}
			}
			else if (this.jj_la == 0 && this.jj_scanpos == this.jj_lastpos)
			{
				result = false;
				return result;
			}
			result = false;
			return result;
		}

		private bool jj_3R_23()
		{
			return this.ScanToken(23) || (this.jj_la == 0 && this.jj_scanpos == this.jj_lastpos && false);
		}

		private bool jj_3_9()
		{
			return this.ScanToken(23) || (this.jj_la == 0 && this.jj_scanpos == this.jj_lastpos && false);
		}

		private bool jj_3_1()
		{
			return this.jj_3R_19() || (this.jj_la == 0 && this.jj_scanpos == this.jj_lastpos && false);
		}

		private bool jj_3R_22()
		{
			return this.jj_3R_40() || (this.jj_la == 0 && this.jj_scanpos == this.jj_lastpos && false);
		}

		private bool jj_3R_37()
		{
			return this.ScanToken(23) || (this.jj_la == 0 && this.jj_scanpos == this.jj_lastpos && false);
		}

		private bool jj_3R_21()
		{
			return this.jj_3R_19() || (this.jj_la == 0 && this.jj_scanpos == this.jj_lastpos && false);
		}

		private bool jj_3R_27()
		{
			return this.ScanToken(23) || (this.jj_la == 0 && this.jj_scanpos == this.jj_lastpos && false);
		}

		private bool jj_3R_30()
		{
			return this.jj_3R_41() || (this.jj_la == 0 && this.jj_scanpos == this.jj_lastpos && false);
		}

		private bool jj_3R_28()
		{
			return this.jj_3R_41() || (this.jj_la == 0 && this.jj_scanpos == this.jj_lastpos && false);
		}

		private bool jj_3R_36()
		{
			return this.jj_3R_40() || (this.jj_la == 0 && this.jj_scanpos == this.jj_lastpos && false);
		}

		private bool jj_3R_20()
		{
			return this.ScanToken(23) || (this.jj_la == 0 && this.jj_scanpos == this.jj_lastpos && false);
		}

		private bool jj_3R_35()
		{
			return this.jj_3R_19() || (this.jj_la == 0 && this.jj_scanpos == this.jj_lastpos && false);
		}

		private bool jj_3R_26()
		{
			return this.jj_3R_40() || (this.jj_la == 0 && this.jj_scanpos == this.jj_lastpos && false);
		}

		private bool jj_3_8()
		{
			return this.jj_3R_29() || (this.jj_la == 0 && this.jj_scanpos == this.jj_lastpos && false);
		}

		private bool jj_3_3()
		{
			bool result;
			if (this.ScanToken(1))
			{
				result = true;
			}
			else if (this.jj_la == 0 && this.jj_scanpos == this.jj_lastpos)
			{
				result = false;
			}
			else
			{
				Token token = this.jj_scanpos;
				if (this.jj_3R_20())
				{
					this.jj_scanpos = token;
				}
				else if (this.jj_la == 0 && this.jj_scanpos == this.jj_lastpos)
				{
					result = false;
					return result;
				}
				token = this.jj_scanpos;
				if (this.jj_3R_21())
				{
					this.jj_scanpos = token;
					if (this.jj_3R_22())
					{
						result = true;
						return result;
					}
					if (this.jj_la == 0 && this.jj_scanpos == this.jj_lastpos)
					{
						result = false;
						return result;
					}
				}
				else if (this.jj_la == 0 && this.jj_scanpos == this.jj_lastpos)
				{
					result = false;
					return result;
				}
				token = this.jj_scanpos;
				if (this.jj_3R_23())
				{
					this.jj_scanpos = token;
				}
				else if (this.jj_la == 0 && this.jj_scanpos == this.jj_lastpos)
				{
					result = false;
					return result;
				}
				result = (this.ScanToken(4) || (this.jj_la == 0 && this.jj_scanpos == this.jj_lastpos && false));
			}
			return result;
		}

		private bool jj_3R_88()
		{
			return this.ScanToken(3) || ((this.jj_la != 0 || this.jj_scanpos != this.jj_lastpos) && (this.jj_3R_54() || (this.jj_la == 0 && this.jj_scanpos == this.jj_lastpos && false)));
		}

		private bool jj_3_6()
		{
			return this.jj_3R_29() || (this.jj_la == 0 && this.jj_scanpos == this.jj_lastpos && false);
		}

		private bool jj_3R_25()
		{
			return this.jj_3R_19() || (this.jj_la == 0 && this.jj_scanpos == this.jj_lastpos && false);
		}

		private bool jj_3R_59()
		{
			return this.ScanToken(26) || (this.jj_la == 0 && this.jj_scanpos == this.jj_lastpos && false);
		}

		private bool jj_3R_34()
		{
			return this.ScanToken(23) || (this.jj_la == 0 && this.jj_scanpos == this.jj_lastpos && false);
		}

		public Parser(ICharStream stream)
		{
			this.currentTemplateName = "";
			this.velcharstream = null;
			this.rsvc = null;
			this.directiveStack = new Stack();
			this.lookingAhead = false;
			this.jj_la1_0 = new int[]
			{
				20906848,
				0,
				20906848,
				3670016,
				16777216,
				109051906,
				8388608,
				125829122,
				20906848,
				8,
				125829122,
				8388608,
				0,
				8388608,
				8388608,
				0,
				8388608,
				8388608,
				16777216,
				100663298,
				8388608,
				8,
				125829122,
				0,
				0,
				0,
				17170528,
				8388608,
				20906848,
				0,
				0,
				0,
				20906848,
				8388608,
				20906848,
				134217728,
				125829154,
				0,
				0,
				0,
				0,
				0,
				0,
				805306368,
				805306368,
				-1073741824,
				-1073741824,
				8388608,
				125829154,
				8388608,
				16777216,
				100663330,
				8388608
			};
			this.jj_la1_1 = new int[]
			{
				252874752,
				36864,
				236060672,
				0,
				85065728,
				0,
				0,
				85065728,
				252874752,
				0,
				84017152,
				0,
				84017152,
				0,
				0,
				84017152,
				0,
				0,
				0,
				84017152,
				0,
				0,
				84017152,
				16777216,
				16777216,
				83886080,
				235012096,
				0,
				252874752,
				8192,
				8192,
				16384,
				252874752,
				0,
				252874752,
				0,
				84017664,
				4,
				2,
				384,
				384,
				120,
				120,
				0,
				0,
				1,
				1,
				0,
				84017152,
				0,
				84017152,
				0,
				0
			};
			this.jj_rescan = false;
			this.jj_gc = 0;
			this.jj_kind = -1;
			this.InitBlock();
			this.token_source = new ParserTokenManager(stream);
			this.token = new Token();
			this.jj_ntk_Renamed_Field = -1;
			this.jj_gen = 0;
			for (int i = 0; i < 53; i++)
			{
				this.jj_la1[i] = -1;
			}
			for (int i = 0; i < this.jj_2_rtns.Length; i++)
			{
				this.jj_2_rtns[i] = new Parser.Calls();
			}
		}

		public void ReInit(ICharStream stream)
		{
			this.token_source.ReInit(stream);
			this.token = new Token();
			this.jj_ntk_Renamed_Field = -1;
			this.nodeTree.Reset();
			this.jj_gen = 0;
			for (int i = 0; i < 53; i++)
			{
				this.jj_la1[i] = -1;
			}
			for (int i = 0; i < this.jj_2_rtns.Length; i++)
			{
				this.jj_2_rtns[i] = new Parser.Calls();
			}
		}

		public Parser(ParserTokenManager tm)
		{
			this.currentTemplateName = "";
			this.velcharstream = null;
			this.rsvc = null;
			this.directiveStack = new Stack();
			this.lookingAhead = false;
			this.jj_la1_0 = new int[]
			{
				20906848,
				0,
				20906848,
				3670016,
				16777216,
				109051906,
				8388608,
				125829122,
				20906848,
				8,
				125829122,
				8388608,
				0,
				8388608,
				8388608,
				0,
				8388608,
				8388608,
				16777216,
				100663298,
				8388608,
				8,
				125829122,
				0,
				0,
				0,
				17170528,
				8388608,
				20906848,
				0,
				0,
				0,
				20906848,
				8388608,
				20906848,
				134217728,
				125829154,
				0,
				0,
				0,
				0,
				0,
				0,
				805306368,
				805306368,
				-1073741824,
				-1073741824,
				8388608,
				125829154,
				8388608,
				16777216,
				100663330,
				8388608
			};
			this.jj_la1_1 = new int[]
			{
				252874752,
				36864,
				236060672,
				0,
				85065728,
				0,
				0,
				85065728,
				252874752,
				0,
				84017152,
				0,
				84017152,
				0,
				0,
				84017152,
				0,
				0,
				0,
				84017152,
				0,
				0,
				84017152,
				16777216,
				16777216,
				83886080,
				235012096,
				0,
				252874752,
				8192,
				8192,
				16384,
				252874752,
				0,
				252874752,
				0,
				84017664,
				4,
				2,
				384,
				384,
				120,
				120,
				0,
				0,
				1,
				1,
				0,
				84017152,
				0,
				84017152,
				0,
				0
			};
			this.jj_rescan = false;
			this.jj_gc = 0;
			this.jj_kind = -1;
			this.InitBlock();
			this.token_source = tm;
			this.token = new Token();
			this.jj_ntk_Renamed_Field = -1;
			this.jj_gen = 0;
			for (int i = 0; i < 53; i++)
			{
				this.jj_la1[i] = -1;
			}
			for (int i = 0; i < this.jj_2_rtns.Length; i++)
			{
				this.jj_2_rtns[i] = new Parser.Calls();
			}
		}

		public void ReInit(ParserTokenManager tm)
		{
			this.token_source = tm;
			this.token = new Token();
			this.jj_ntk_Renamed_Field = -1;
			this.nodeTree.Reset();
			this.jj_gen = 0;
			for (int i = 0; i < 53; i++)
			{
				this.jj_la1[i] = -1;
			}
			for (int i = 0; i < this.jj_2_rtns.Length; i++)
			{
				this.jj_2_rtns[i] = new Parser.Calls();
			}
		}

		private Token ConsumeToken(int kind)
		{
			Token token = this.token;
			if (this.token.Next != null)
			{
				this.token = this.token.Next;
			}
			else
			{
				this.token = (this.token.Next = this.token_source.NextToken);
			}
			this.jj_ntk_Renamed_Field = -1;
			if (this.token.Kind == kind)
			{
				this.jj_gen++;
				if (++this.jj_gc > 100)
				{
					this.jj_gc = 0;
					for (int i = 0; i < this.jj_2_rtns.Length; i++)
					{
						for (Parser.Calls calls = this.jj_2_rtns[i]; calls != null; calls = calls.Next)
						{
							if (calls.Gen < this.jj_gen)
							{
								calls.First = null;
							}
						}
					}
				}
				return this.token;
			}
			this.token = token;
			this.jj_kind = kind;
			throw this.GenerateParseException();
		}

		private bool ScanToken(int kind)
		{
			if (this.jj_scanpos == this.jj_lastpos)
			{
				this.jj_la--;
				if (this.jj_scanpos.Next == null)
				{
					this.jj_lastpos = (this.jj_scanpos = (this.jj_scanpos.Next = this.token_source.NextToken));
				}
				else
				{
					this.jj_lastpos = (this.jj_scanpos = this.jj_scanpos.Next);
				}
			}
			else
			{
				this.jj_scanpos = this.jj_scanpos.Next;
			}
			if (this.jj_rescan)
			{
				int num = 0;
				Token next = this.token;
				while (next != null && next != this.jj_scanpos)
				{
					num++;
					next = next.Next;
				}
				if (next != null)
				{
					this.AddErrorToken(kind, num);
				}
			}
			return this.jj_scanpos.Kind != kind;
		}

		public Token GetToken(int index)
		{
			Token token = this.lookingAhead ? this.jj_scanpos : this.token;
			for (int i = 0; i < index; i++)
			{
				if (token.Next != null)
				{
					token = token.Next;
				}
				else
				{
					token = (token.Next = this.token_source.NextToken);
				}
			}
			return token;
		}

		private int jj_ntk()
		{
			int result;
			if ((this.jj_nt = this.token.Next) == null)
			{
				result = (this.jj_ntk_Renamed_Field = (this.token.Next = this.token_source.NextToken).Kind);
			}
			else
			{
				result = (this.jj_ntk_Renamed_Field = this.jj_nt.Kind);
			}
			return result;
		}

		private void AddErrorToken(int kind, int pos)
		{
			if (pos < 100)
			{
				if (pos == this.jj_endpos + 1)
				{
					this.jj_lasttokens[this.jj_endpos++] = kind;
				}
				else if (this.jj_endpos != 0)
				{
					this.jj_expentry = new int[this.jj_endpos];
					for (int i = 0; i < this.jj_endpos; i++)
					{
						this.jj_expentry[i] = this.jj_lasttokens[i];
					}
					bool flag = false;
					IEnumerator enumerator = this.jj_expentries.GetEnumerator();
					while (enumerator.MoveNext())
					{
						int[] array = (int[])enumerator.Current;
						if (array.Length == this.jj_expentry.Length)
						{
							flag = true;
							for (int i = 0; i < this.jj_expentry.Length; i++)
							{
								if (array[i] != this.jj_expentry[i])
								{
									flag = false;
									break;
								}
							}
							if (flag)
							{
								break;
							}
						}
					}
					if (!flag)
					{
						this.jj_expentries.Add(this.jj_expentry);
					}
					if (pos != 0)
					{
						int[] arg_14F_0 = this.jj_lasttokens;
						this.jj_endpos = pos;
						arg_14F_0[pos - 1] = kind;
					}
				}
			}
		}

		public ParseException GenerateParseException()
		{
			ArrayList arrayList = this.jj_expentries;
			arrayList.RemoveRange(0, arrayList.Count);
			bool[] array = new bool[62];
			for (int i = 0; i < 62; i++)
			{
				array[i] = false;
			}
			if (this.jj_kind >= 0)
			{
				array[this.jj_kind] = true;
				this.jj_kind = -1;
			}
			for (int i = 0; i < 53; i++)
			{
				if (this.jj_la1[i] == this.jj_gen)
				{
					for (int j = 0; j < 32; j++)
					{
						if ((this.jj_la1_0[i] & 1 << j) != 0)
						{
							array[j] = true;
						}
						if ((this.jj_la1_1[i] & 1 << j) != 0)
						{
							array[32 + j] = true;
						}
					}
				}
			}
			for (int i = 0; i < 62; i++)
			{
				if (array[i])
				{
					this.jj_expentry = new int[1];
					this.jj_expentry[0] = i;
					this.jj_expentries.Add(this.jj_expentry);
				}
			}
			this.jj_endpos = 0;
			this.RescanToken();
			this.AddErrorToken(0, 0);
			int[][] array2 = new int[this.jj_expentries.Count][];
			for (int i = 0; i < this.jj_expentries.Count; i++)
			{
				array2[i] = (int[])this.jj_expentries[i];
			}
			return new ParseException(this.token, array2, ParserConstants.TokenImage);
		}

		public void EnableTracing()
		{
		}

		public void DisableTracing()
		{
		}

		private void RescanToken()
		{
			this.jj_rescan = true;
			for (int i = 0; i < 12; i++)
			{
				Parser.Calls calls = this.jj_2_rtns[i];
				do
				{
					if (calls.Gen > this.jj_gen)
					{
						this.jj_la = calls.Arg;
						this.jj_lastpos = (this.jj_scanpos = calls.First);
						switch (i)
						{
						case 0:
							this.jj_3_1();
							break;
						case 1:
							this.jj_3_2();
							break;
						case 2:
							this.jj_3_3();
							break;
						case 3:
							this.jj_3_4();
							break;
						case 4:
							this.jj_3_5();
							break;
						case 5:
							this.jj_3_6();
							break;
						case 6:
							this.jj_3_7();
							break;
						case 7:
							this.jj_3_8();
							break;
						case 8:
							this.jj_3_9();
							break;
						case 9:
							this.jj_3_10();
							break;
						case 10:
							this.jj_3_11();
							break;
						case 11:
							this.jj_3_12();
							break;
						}
					}
					calls = calls.Next;
				}
				while (calls != null);
			}
			this.jj_rescan = false;
		}

		private void Save(int index, int xla)
		{
			Parser.Calls calls = this.jj_2_rtns[index];
			while (calls.Gen > this.jj_gen)
			{
				if (calls.Next == null)
				{
					calls = (calls.Next = new Parser.Calls());
					break;
				}
				calls = calls.Next;
			}
			calls.Gen = this.jj_gen + xla - this.jj_la;
			calls.First = this.token;
			calls.Arg = xla;
		}
	}
}
