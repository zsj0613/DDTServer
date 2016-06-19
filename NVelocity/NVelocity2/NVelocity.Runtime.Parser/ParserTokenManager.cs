using System;
using System.Collections;
using System.IO;
using System.Text;

namespace NVelocity.Runtime.Parser
{
	public class ParserTokenManager : ParserConstants
	{
		private int fileDepth = 0;

		private int lparen = 0;

		private int rparen = 0;

		private Stack stateStack = new Stack();

		public bool debugPrint = false;

		private bool inReference;

		public bool inDirective;

		private bool inComment;

		public bool inSet;

		public TextWriter debugStream = Console.Out;

		private static ulong[] jjbitVec0 = new ulong[]
		{
			18446744073709551614uL,
			18446744073709551615uL,
			18446744073709551615uL,
			18446744073709551615uL
		};

		private static ulong[] jjbitVec2 = new ulong[]
		{
			0uL,
			0uL,
			18446744073709551615uL,
			18446744073709551615uL
		};

		private static int[] jjnextStates = new int[]
		{
			22,
			23,
			26,
			11,
			12,
			13,
			1,
			2,
			4,
			11,
			16,
			12,
			13,
			19,
			20,
			24,
			25,
			35,
			36,
			37,
			38,
			14,
			15,
			17,
			19,
			20,
			39,
			40,
			5,
			6,
			7,
			8,
			9,
			10,
			24,
			25,
			27,
			18,
			19,
			21,
			9,
			10,
			11,
			12,
			22,
			29,
			13,
			14,
			7,
			8,
			16,
			17,
			18,
			19,
			20,
			21,
			8,
			9,
			10,
			11,
			12,
			13,
			17,
			18,
			21,
			6,
			7,
			8,
			6,
			11,
			7,
			8,
			14,
			15,
			29,
			30,
			31,
			32,
			9,
			10,
			12,
			14,
			15,
			33,
			34
		};

		public static string[] jjstrLiteralImages;

		public static string[] lexStateNames;

		public static int[] jjnewLexState;

		private static long[] jjtoToken;

		private static long[] jjtoSkip;

		private static long[] jjtoSpecial;

		private static long[] jjtoMore;

		private ICharStream input_stream;

		private uint[] jjrounds = new uint[42];

		private uint[] jjstateSet = new uint[84];

		private StringBuilder image;

		private int jjimageLen;

		private int lengthOfMatch;

		protected char curChar;

		private int curLexState = 3;

		private int defaultLexState = 3;

		private int jjnewStateCnt;

		private uint jjround;

		private int jjmatchedPos;

		private int jjmatchedKind;

		public Token NextToken
		{
			get
			{
				Token token = null;
				int num = 0;
				Token token2;
				Token result;
				while (true)
				{
					try
					{
						this.curChar = this.input_stream.BeginToken();
					}
					catch (IOException var_4_20)
					{
						this.jjmatchedKind = 0;
						token2 = this.jjFillToken();
						token2.SpecialToken = token;
						result = token2;
						return result;
					}
					this.image = null;
					this.jjimageLen = 0;
					while (true)
					{
						switch (this.curLexState)
						{
						case 0:
							this.jjmatchedKind = 2147483647;
							this.jjmatchedPos = 0;
							num = this.jjMoveStringLiteralDfa0_0();
							break;
						case 1:
							this.jjmatchedKind = 2147483647;
							this.jjmatchedPos = 0;
							num = this.jjMoveStringLiteralDfa0_1();
							if (this.jjmatchedPos == 0 && this.jjmatchedKind > 60)
							{
								this.jjmatchedKind = 60;
							}
							break;
						case 2:
							this.jjmatchedKind = 2147483647;
							this.jjmatchedPos = 0;
							num = this.jjMoveStringLiteralDfa0_2();
							if (this.jjmatchedPos == 0 && this.jjmatchedKind > 60)
							{
								this.jjmatchedKind = 60;
							}
							break;
						case 3:
							this.jjmatchedKind = 2147483647;
							this.jjmatchedPos = 0;
							num = this.jjMoveStringLiteralDfa0_3();
							break;
						case 4:
							this.jjmatchedKind = 2147483647;
							this.jjmatchedPos = 0;
							num = this.jjMoveStringLiteralDfa0_4();
							if (this.jjmatchedPos == 0 && this.jjmatchedKind > 61)
							{
								this.jjmatchedKind = 61;
							}
							break;
						case 5:
							this.jjmatchedKind = 2147483647;
							this.jjmatchedPos = 0;
							num = this.jjMoveStringLiteralDfa0_5();
							if (this.jjmatchedPos == 0 && this.jjmatchedKind > 60)
							{
								this.jjmatchedKind = 60;
							}
							break;
						case 6:
							this.jjmatchedKind = 2147483647;
							this.jjmatchedPos = 0;
							num = this.jjMoveStringLiteralDfa0_6();
							if (this.jjmatchedPos == 0 && this.jjmatchedKind > 22)
							{
								this.jjmatchedKind = 22;
							}
							break;
						case 7:
							this.jjmatchedKind = 2147483647;
							this.jjmatchedPos = 0;
							num = this.jjMoveStringLiteralDfa0_7();
							if (this.jjmatchedPos == 0 && this.jjmatchedKind > 22)
							{
								this.jjmatchedKind = 22;
							}
							break;
						case 8:
							this.jjmatchedKind = 2147483647;
							this.jjmatchedPos = 0;
							num = this.jjMoveStringLiteralDfa0_8();
							if (this.jjmatchedPos == 0 && this.jjmatchedKind > 22)
							{
								this.jjmatchedKind = 22;
							}
							break;
						}
						if (this.jjmatchedKind == 2147483647)
						{
							goto IL_478;
						}
						if (this.jjmatchedPos + 1 < num)
						{
							this.input_stream.Backup(num - this.jjmatchedPos - 1);
						}
						if ((ParserTokenManager.jjtoToken[this.jjmatchedKind >> 6] & 1L << this.jjmatchedKind) != 0L)
						{
							goto Block_19;
						}
						if ((ParserTokenManager.jjtoSkip[this.jjmatchedKind >> 6] & 1L << this.jjmatchedKind) != 0L)
						{
							break;
						}
						this.MoreLexicalActions();
						if (ParserTokenManager.jjnewLexState[this.jjmatchedKind] != -1)
						{
							this.curLexState = ParserTokenManager.jjnewLexState[this.jjmatchedKind];
						}
						num = 0;
						this.jjmatchedKind = 2147483647;
						try
						{
							this.curChar = this.input_stream.ReadChar();
							continue;
						}
						catch (IOException var_5_470)
						{
						}
						goto IL_476;
					}
					if ((ParserTokenManager.jjtoSpecial[this.jjmatchedKind >> 6] & 1L << this.jjmatchedKind) != 0L)
					{
						token2 = this.jjFillToken();
						if (token == null)
						{
							token = token2;
						}
						else
						{
							token2.SpecialToken = token;
							token = (token.Next = token2);
						}
						this.SkipLexicalActions(token2);
					}
					else
					{
						this.SkipLexicalActions(null);
					}
					if (ParserTokenManager.jjnewLexState[this.jjmatchedKind] != -1)
					{
						this.curLexState = ParserTokenManager.jjnewLexState[this.jjmatchedKind];
					}
				}
				Block_19:
				token2 = this.jjFillToken();
				token2.SpecialToken = token;
				this.TokenLexicalActions(token2);
				if (ParserTokenManager.jjnewLexState[this.jjmatchedKind] != -1)
				{
					this.curLexState = ParserTokenManager.jjnewLexState[this.jjmatchedKind];
				}
				result = token2;
				return result;
				IL_476:
				IL_478:
				int num2 = this.input_stream.EndLine;
				int num3 = this.input_stream.EndColumn;
				string errorAfter = null;
				bool flag = false;
				try
				{
					this.input_stream.ReadChar();
					this.input_stream.Backup(1);
				}
				catch (IOException var_5_470)
				{
					flag = true;
					errorAfter = ((num <= 1) ? "" : this.input_stream.GetImage());
					if (this.curChar == '\n' || this.curChar == '\r')
					{
						num2++;
						num3 = 0;
					}
					else
					{
						num3++;
					}
				}
				if (!flag)
				{
					this.input_stream.Backup(1);
					errorAfter = ((num <= 1) ? "" : this.input_stream.GetImage());
				}
				throw new TokenMgrError(flag, this.curLexState, num2, num3, errorAfter, this.curChar, 0);
			}
		}

		public bool StateStackPop()
		{
			Hashtable hashtable;
			bool result;
			try
			{
				hashtable = (Hashtable)this.stateStack.Pop();
			}
			catch (InvalidOperationException var_1_16)
			{
				this.lparen = 0;
				this.SwitchTo(3);
				result = false;
				return result;
			}
			if (this.debugPrint)
			{
				Console.Out.WriteLine(string.Concat(new object[]
				{
					" stack pop (",
					this.stateStack.Count,
					") : lparen=",
					(int)hashtable["lparen"],
					" newstate=",
					(int)hashtable["lexstate"]
				}));
			}
			this.lparen = (int)hashtable["lparen"];
			this.rparen = (int)hashtable["rparen"];
			this.SwitchTo((int)hashtable["lexstate"]);
			result = true;
			return result;
		}

		public bool StateStackPush()
		{
			if (this.debugPrint)
			{
				Console.Out.WriteLine(string.Concat(new object[]
				{
					" (",
					this.stateStack.Count,
					") pushing cur state : ",
					this.curLexState
				}));
			}
			Hashtable hashtable = new Hashtable();
			hashtable.Add("lexstate", this.curLexState);
			hashtable.Add("lparen", this.lparen);
			hashtable.Add("rparen", this.rparen);
			this.lparen = 0;
			this.stateStack.Push(hashtable);
			return true;
		}

		public void ClearStateVars()
		{
			this.stateStack.Clear();
			this.lparen = 0;
			this.rparen = 0;
			this.inReference = false;
			this.inDirective = false;
			this.inComment = false;
			this.inSet = false;
		}

		private void RPARENHandler()
		{
			bool flag = false;
			if (this.inComment)
			{
				flag = true;
			}
			while (!flag)
			{
				if (this.lparen > 0)
				{
					if (this.lparen == this.rparen + 1)
					{
						this.StateStackPop();
					}
					else
					{
						this.rparen++;
					}
					flag = true;
				}
				else if (!this.StateStackPop())
				{
					break;
				}
			}
		}

		public void SetDebugStream(StreamWriter ds)
		{
			this.debugStream = ds;
		}

		private int jjStopStringLiteralDfa_0(int pos, long active0)
		{
			int result;
			switch (pos)
			{
			case 0:
				if ((active0 & 100663296L) != 0L)
				{
					this.jjmatchedKind = 52;
					result = 33;
				}
				else if ((active0 & 268435456L) != 0L)
				{
					result = 31;
				}
				else if ((active0 & 53248L) != 0L)
				{
					result = 7;
				}
				else
				{
					result = -1;
				}
				break;
			case 1:
				if ((active0 & 100663296L) != 0L)
				{
					this.jjmatchedKind = 52;
					this.jjmatchedPos = 1;
					result = 33;
				}
				else if ((active0 & 16384L) != 0L)
				{
					result = 5;
				}
				else
				{
					result = -1;
				}
				break;
			case 2:
				if ((active0 & 100663296L) != 0L)
				{
					this.jjmatchedKind = 52;
					this.jjmatchedPos = 2;
					result = 33;
				}
				else
				{
					result = -1;
				}
				break;
			case 3:
				if ((active0 & 67108864L) != 0L)
				{
					this.jjmatchedKind = 52;
					this.jjmatchedPos = 3;
					result = 33;
				}
				else if ((active0 & 33554432L) != 0L)
				{
					result = 33;
				}
				else
				{
					result = -1;
				}
				break;
			default:
				result = -1;
				break;
			}
			return result;
		}

		private int jjStartNfa_0(int pos, long active0)
		{
			return this.jjMoveNfa_0(this.jjStopStringLiteralDfa_0(pos, active0), pos + 1);
		}

		private int jjStopAtPos(int pos, int kind)
		{
			this.jjmatchedKind = kind;
			this.jjmatchedPos = pos;
			return pos + 1;
		}

		private int jjStartNfaWithStates_0(int pos, int kind, int state)
		{
			this.jjmatchedKind = kind;
			this.jjmatchedPos = pos;
			int result;
			try
			{
				this.curChar = this.input_stream.ReadChar();
			}
			catch (IOException var_0_24)
			{
				result = pos + 1;
				return result;
			}
			result = this.jjMoveNfa_0(state, pos + 1);
			return result;
		}

		private int jjMoveStringLiteralDfa0_0()
		{
			char c = this.curChar;
			int result;
			if (c <= ']')
			{
				switch (c)
				{
				case '!':
					this.jjmatchedKind = 41;
					result = this.jjMoveStringLiteralDfa1_0(1099511627776L);
					return result;
				case '"':
				case '$':
				case '\'':
				case ')':
					break;
				case '#':
					this.jjmatchedKind = 15;
					result = this.jjMoveStringLiteralDfa1_0(20480L);
					return result;
				case '%':
					result = this.jjStopAtPos(0, 32);
					return result;
				case '&':
					result = this.jjMoveStringLiteralDfa1_0(8589934592L);
					return result;
				case '(':
					result = this.jjStopAtPos(0, 5);
					return result;
				case '*':
					result = this.jjStopAtPos(0, 30);
					return result;
				case '+':
					result = this.jjStopAtPos(0, 29);
					return result;
				case ',':
					result = this.jjStopAtPos(0, 3);
					return result;
				case '-':
					result = this.jjStartNfaWithStates_0(0, 28, 31);
					return result;
				case '.':
					result = this.jjMoveStringLiteralDfa1_0(16L);
					return result;
				case '/':
					result = this.jjStopAtPos(0, 31);
					return result;
				default:
					switch (c)
					{
					case '<':
						this.jjmatchedKind = 35;
						result = this.jjMoveStringLiteralDfa1_0(68719476736L);
						return result;
					case '=':
						this.jjmatchedKind = 42;
						result = this.jjMoveStringLiteralDfa1_0(549755813888L);
						return result;
					case '>':
						this.jjmatchedKind = 37;
						result = this.jjMoveStringLiteralDfa1_0(274877906944L);
						return result;
					default:
						switch (c)
						{
						case '[':
							result = this.jjStopAtPos(0, 1);
							return result;
						case ']':
							result = this.jjStopAtPos(0, 2);
							return result;
						}
						break;
					}
					break;
				}
			}
			else
			{
				if (c == 'f')
				{
					result = this.jjMoveStringLiteralDfa1_0(67108864L);
					return result;
				}
				if (c == 't')
				{
					result = this.jjMoveStringLiteralDfa1_0(33554432L);
					return result;
				}
				if (c == '|')
				{
					result = this.jjMoveStringLiteralDfa1_0(17179869184L);
					return result;
				}
			}
			result = this.jjMoveNfa_0(0, 0);
			return result;
		}

		private int jjMoveStringLiteralDfa1_0(long active0)
		{
			int result;
			try
			{
				this.curChar = this.input_stream.ReadChar();
			}
			catch (IOException var_0_16)
			{
				this.jjStopStringLiteralDfa_0(0, active0);
				result = 1;
				return result;
			}
			char c = this.curChar;
			if (c <= '.')
			{
				if (c <= '&')
				{
					if (c != '#')
					{
						if (c == '&')
						{
							if ((active0 & 8589934592L) != 0L)
							{
								result = this.jjStopAtPos(1, 33);
								return result;
							}
						}
					}
					else if ((active0 & 4096L) != 0L)
					{
						result = this.jjStopAtPos(1, 12);
						return result;
					}
				}
				else if (c != '*')
				{
					if (c == '.')
					{
						if ((active0 & 16L) != 0L)
						{
							result = this.jjStopAtPos(1, 4);
							return result;
						}
					}
				}
				else if ((active0 & 16384L) != 0L)
				{
					result = this.jjStartNfaWithStates_0(1, 14, 5);
					return result;
				}
			}
			else if (c <= 'a')
			{
				if (c != '=')
				{
					if (c == 'a')
					{
						result = this.jjMoveStringLiteralDfa2_0(active0, 67108864L);
						return result;
					}
				}
				else
				{
					if ((active0 & 68719476736L) != 0L)
					{
						result = this.jjStopAtPos(1, 36);
						return result;
					}
					if ((active0 & 274877906944L) != 0L)
					{
						result = this.jjStopAtPos(1, 38);
						return result;
					}
					if ((active0 & 549755813888L) != 0L)
					{
						result = this.jjStopAtPos(1, 39);
						return result;
					}
					if ((active0 & 1099511627776L) != 0L)
					{
						result = this.jjStopAtPos(1, 40);
						return result;
					}
				}
			}
			else
			{
				if (c == 'r')
				{
					result = this.jjMoveStringLiteralDfa2_0(active0, 33554432L);
					return result;
				}
				if (c == '|')
				{
					if ((active0 & 17179869184L) != 0L)
					{
						result = this.jjStopAtPos(1, 34);
						return result;
					}
				}
			}
			result = this.jjStartNfa_0(0, active0);
			return result;
		}

		private int jjMoveStringLiteralDfa2_0(long old0, long active0)
		{
			int result;
			if ((active0 &= old0) == 0L)
			{
				result = this.jjStartNfa_0(0, old0);
			}
			else
			{
				try
				{
					this.curChar = this.input_stream.ReadChar();
				}
				catch (IOException var_0_32)
				{
					this.jjStopStringLiteralDfa_0(1, active0);
					result = 2;
					return result;
				}
				char c = this.curChar;
				if (c != 'l')
				{
					if (c != 'u')
					{
						result = this.jjStartNfa_0(1, active0);
					}
					else
					{
						result = this.jjMoveStringLiteralDfa3_0(active0, 33554432L);
					}
				}
				else
				{
					result = this.jjMoveStringLiteralDfa3_0(active0, 67108864L);
				}
			}
			return result;
		}

		private int jjMoveStringLiteralDfa3_0(long old0, long active0)
		{
			int result;
			if ((active0 &= old0) == 0L)
			{
				result = this.jjStartNfa_0(1, old0);
			}
			else
			{
				try
				{
					this.curChar = this.input_stream.ReadChar();
				}
				catch (IOException var_0_32)
				{
					this.jjStopStringLiteralDfa_0(2, active0);
					result = 3;
					return result;
				}
				char c = this.curChar;
				if (c != 'e')
				{
					if (c == 's')
					{
						result = this.jjMoveStringLiteralDfa4_0(active0, 67108864L);
						return result;
					}
				}
				else if ((active0 & 33554432L) != 0L)
				{
					result = this.jjStartNfaWithStates_0(3, 25, 33);
					return result;
				}
				result = this.jjStartNfa_0(2, active0);
			}
			return result;
		}

		private int jjMoveStringLiteralDfa4_0(long old0, long active0)
		{
			int result;
			if ((active0 &= old0) == 0L)
			{
				result = this.jjStartNfa_0(2, old0);
			}
			else
			{
				try
				{
					this.curChar = this.input_stream.ReadChar();
				}
				catch (IOException var_0_32)
				{
					this.jjStopStringLiteralDfa_0(3, active0);
					result = 4;
					return result;
				}
				char c = this.curChar;
				if (c == 'e')
				{
					if ((active0 & 67108864L) != 0L)
					{
						result = this.jjStartNfaWithStates_0(4, 26, 33);
						return result;
					}
				}
				result = this.jjStartNfa_0(3, active0);
			}
			return result;
		}

		private void jjCheckNAdd(int state)
		{
			if (this.jjrounds[state] != this.jjround)
			{
				this.jjstateSet[this.jjnewStateCnt++] = (uint)state;
				this.jjrounds[state] = this.jjround;
			}
		}

		private void jjAddStates(int start, int end)
		{
			do
			{
				this.jjstateSet[this.jjnewStateCnt++] = (uint)ParserTokenManager.jjnextStates[start];
			}
			while (start++ != end);
		}

		private void jjCheckNAddTwoStates(int state1, int state2)
		{
			this.jjCheckNAdd(state1);
			this.jjCheckNAdd(state2);
		}

		private void jjCheckNAddStates(int start, int end)
		{
			do
			{
				this.jjCheckNAdd(ParserTokenManager.jjnextStates[start]);
			}
			while (start++ != end);
		}

		private void jjCheckNAddStates(int start)
		{
			this.jjCheckNAdd(ParserTokenManager.jjnextStates[start]);
			this.jjCheckNAdd(ParserTokenManager.jjnextStates[start + 1]);
		}

		private int jjMoveNfa_0(int startState, int curPos)
		{
			int num = 0;
			this.jjnewStateCnt = 42;
			int num2 = 1;
			this.jjstateSet[0] = (uint)startState;
			int num3 = 2147483647;
			int result;
			while (true)
			{
				if ((this.jjround += 1u) == 2147483647u)
				{
					this.ReInitRounds();
				}
				if (this.curChar < '@')
				{
					long num4 = 1L << (int)this.curChar;
					while (true)
					{
						switch (this.jjstateSet[--num2])
						{
						case 0u:
							if ((287948901175001088L & num4) != 0L)
							{
								if (num3 > 49)
								{
									num3 = 49;
								}
								this.jjCheckNAdd(31);
							}
							else if ((9216L & num4) != 0L)
							{
								if (num3 > 27)
								{
									num3 = 27;
								}
							}
							else if ((4294967808L & num4) != 0L)
							{
								if (num3 > 23)
								{
									num3 = 23;
								}
								this.jjCheckNAdd(9);
							}
							else if (this.curChar == '$')
							{
								if (num3 > 10)
								{
									num3 = 10;
								}
								this.jjCheckNAddTwoStates(39, 40);
							}
							else if (this.curChar == '-')
							{
								this.jjCheckNAdd(31);
							}
							else if (this.curChar == '\'')
							{
								this.jjCheckNAddStates(0, 2);
							}
							else if (this.curChar == '"')
							{
								this.jjCheckNAddStates(3, 5);
							}
							else if (this.curChar == '#')
							{
								this.jjstateSet[this.jjnewStateCnt++] = 7u;
							}
							else if (this.curChar == ')')
							{
								if (num3 > 6)
								{
									num3 = 6;
								}
								this.jjCheckNAddStates(6, 8);
							}
							if (this.curChar == '\r')
							{
								this.jjstateSet[this.jjnewStateCnt++] = 28u;
							}
							break;
						case 1u:
							if ((4294967808L & num4) != 0L)
							{
								this.jjCheckNAddStates(6, 8);
							}
							break;
						case 2u:
							if ((9216L & num4) != 0L && num3 > 6)
							{
								num3 = 6;
							}
							break;
						case 3u:
							if (this.curChar == '\n' && num3 > 6)
							{
								num3 = 6;
							}
							break;
						case 4u:
							if (this.curChar == '\r')
							{
								this.jjstateSet[this.jjnewStateCnt++] = 3u;
							}
							break;
						case 5u:
							if (this.curChar == '*')
							{
								this.jjstateSet[this.jjnewStateCnt++] = 6u;
							}
							break;
						case 6u:
							if ((-34359738369L & num4) != 0L && num3 > 13)
							{
								num3 = 13;
							}
							break;
						case 7u:
							if (this.curChar == '*')
							{
								this.jjstateSet[this.jjnewStateCnt++] = 5u;
							}
							break;
						case 8u:
							if (this.curChar == '#')
							{
								this.jjstateSet[this.jjnewStateCnt++] = 7u;
							}
							break;
						case 9u:
							if ((4294967808L & num4) != 0L)
							{
								if (num3 > 23)
								{
									num3 = 23;
								}
								this.jjCheckNAdd(9);
							}
							break;
						case 10u:
							if (this.curChar == '"')
							{
								this.jjCheckNAddStates(3, 5);
							}
							break;
						case 11u:
							if ((-17179878401L & num4) != 0L)
							{
								this.jjCheckNAddStates(3, 5);
							}
							break;
						case 12u:
							if (this.curChar == '"' && num3 > 24)
							{
								num3 = 24;
							}
							break;
						case 14u:
							if ((566935683072L & num4) != 0L)
							{
								this.jjCheckNAddStates(3, 5);
							}
							break;
						case 15u:
							if ((71776119061217280L & num4) != 0L)
							{
								this.jjCheckNAddStates(9, 12);
							}
							break;
						case 16u:
							if ((71776119061217280L & num4) != 0L)
							{
								this.jjCheckNAddStates(3, 5);
							}
							break;
						case 17u:
							if ((4222124650659840L & num4) != 0L)
							{
								this.jjstateSet[this.jjnewStateCnt++] = 18u;
							}
							break;
						case 18u:
							if ((71776119061217280L & num4) != 0L)
							{
								this.jjCheckNAdd(16);
							}
							break;
						case 19u:
							if (this.curChar == ' ')
							{
								this.jjAddStates(13, 14);
							}
							break;
						case 20u:
							if (this.curChar == '\n')
							{
								this.jjCheckNAddStates(3, 5);
							}
							break;
						case 21u:
							if (this.curChar == '\'')
							{
								this.jjCheckNAddStates(0, 2);
							}
							break;
						case 22u:
							if ((-549755823105L & num4) != 0L)
							{
								this.jjCheckNAddStates(0, 2);
							}
							break;
						case 24u:
							if (this.curChar == ' ')
							{
								this.jjAddStates(15, 16);
							}
							break;
						case 25u:
							if (this.curChar == '\n')
							{
								this.jjCheckNAddStates(0, 2);
							}
							break;
						case 26u:
							if (this.curChar == '\'' && num3 > 24)
							{
								num3 = 24;
							}
							break;
						case 27u:
							if ((9216L & num4) != 0L && num3 > 27)
							{
								num3 = 27;
							}
							break;
						case 28u:
							if (this.curChar == '\n' && num3 > 27)
							{
								num3 = 27;
							}
							break;
						case 29u:
							if (this.curChar == '\r')
							{
								this.jjstateSet[this.jjnewStateCnt++] = 28u;
							}
							break;
						case 30u:
							if (this.curChar == '-')
							{
								this.jjCheckNAdd(31);
							}
							break;
						case 31u:
							if ((287948901175001088L & num4) != 0L)
							{
								if (num3 > 49)
								{
									num3 = 49;
								}
								this.jjCheckNAdd(31);
							}
							break;
						case 33u:
							if ((287948901175001088L & num4) != 0L)
							{
								if (num3 > 52)
								{
									num3 = 52;
								}
								this.jjstateSet[this.jjnewStateCnt++] = 33u;
							}
							break;
						case 36u:
							if (this.curChar == '$' && num3 > 10)
							{
								num3 = 10;
							}
							break;
						case 38u:
							if (this.curChar == '$')
							{
								this.jjCheckNAddTwoStates(39, 40);
							}
							break;
						case 40u:
							if (this.curChar == '!' && num3 > 11)
							{
								num3 = 11;
							}
							break;
						case 41u:
							if (this.curChar == '$')
							{
								if (num3 > 10)
								{
									num3 = 10;
								}
								this.jjCheckNAddTwoStates(39, 40);
							}
							break;
						}
						//IL_88F:
						if (num2 == num)
						{
							break;
						}
						continue;
						//goto IL_88F;
					}
				}
				else if (this.curChar < '\u0080')
				{
					long num4 = 1L << (int)this.curChar;
					while (true)
					{
						uint num5 = this.jjstateSet[--num2];
						if (num5 <= 6u)
						{
							if (num5 != 0u)
							{
								if (num5 == 6u)
								{
									if (num3 > 13)
									{
										num3 = 13;
									}
								}
							}
							else if ((576460745995190270L & num4) != 0L)
							{
								if (num3 > 52)
								{
									num3 = 52;
								}
								this.jjCheckNAdd(33);
							}
							else if (this.curChar == '\\')
							{
								this.jjCheckNAddStates(17, 20);
							}
						}
						else
						{
							switch (num5)
							{
							case 11u:
								if ((-268435457L & num4) != 0L)
								{
									this.jjCheckNAddStates(3, 5);
								}
								break;
							case 12u:
								break;
							case 13u:
								if (this.curChar == '\\')
								{
									this.jjAddStates(21, 25);
								}
								break;
							case 14u:
								if ((5700160604602368L & num4) != 0L)
								{
									this.jjCheckNAddStates(3, 5);
								}
								break;
							default:
								switch (num5)
								{
								case 22u:
									this.jjAddStates(0, 2);
									break;
								case 23u:
									if (this.curChar == '\\')
									{
										this.jjAddStates(15, 16);
									}
									break;
								default:
									switch (num5)
									{
									case 32u:
									case 33u:
										if ((576460745995190270L & num4) != 0L)
										{
											if (num3 > 52)
											{
												num3 = 52;
											}
											this.jjCheckNAdd(33);
										}
										break;
									case 34u:
										if (this.curChar == '\\')
										{
											this.jjCheckNAddStates(17, 20);
										}
										break;
									case 35u:
										if (this.curChar == '\\')
										{
											this.jjCheckNAddTwoStates(35, 36);
										}
										break;
									case 37u:
										if (this.curChar == '\\')
										{
											this.jjCheckNAddTwoStates(37, 38);
										}
										break;
									case 39u:
										if (this.curChar == '\\')
										{
											this.jjAddStates(26, 27);
										}
										break;
									}
									break;
								}
								break;
							}
						}
						//IL_B1D:
						if (num2 == num)
						{
							break;
						}
						continue;
						//goto IL_B1D;
					}
				}
				else
				{
					int num6 = (int)(this.curChar >> 8);
					int i = num6 >> 6;
					long l = 1L << num6;
					int i2 = (int)((this.curChar & 'ÿ') >> 6);
					long l2 = 1L << (int)this.curChar;
					do
					{
						uint num5 = this.jjstateSet[--num2];
						if (num5 != 6u)
						{
							if (num5 != 11u)
							{
								if (num5 == 22u)
								{
									if (ParserTokenManager.jjCanMove_0(num6, i, i2, l, l2))
									{
										this.jjAddStates(0, 2);
									}
								}
							}
							else if (ParserTokenManager.jjCanMove_0(num6, i, i2, l, l2))
							{
								this.jjAddStates(3, 5);
							}
						}
						else if (ParserTokenManager.jjCanMove_0(num6, i, i2, l, l2) && num3 > 13)
						{
							num3 = 13;
						}
					}
					while (num2 != num);
				}
				if (num3 != 2147483647)
				{
					this.jjmatchedKind = num3;
					this.jjmatchedPos = curPos;
					num3 = 2147483647;
				}
				curPos++;
				if ((num2 = this.jjnewStateCnt) == (num = 42 - (this.jjnewStateCnt = num)))
				{
					break;
				}
				try
				{
					this.curChar = this.input_stream.ReadChar();
				}
				catch (IOException var_11_C82)
				{
					result = curPos;
					return result;
				}
			}
			result = curPos;
			return result;
		}

		private int jjStopStringLiteralDfa_6(int pos, long active0)
		{
			int result;
			if (pos != 0)
			{
				result = -1;
			}
			else if ((active0 & 53248L) != 0L)
			{
				result = 2;
			}
			else
			{
				result = -1;
			}
			return result;
		}

		private int jjStartNfa_6(int pos, long active0)
		{
			return this.jjMoveNfa_6(this.jjStopStringLiteralDfa_6(pos, active0), pos + 1);
		}

		private int jjStartNfaWithStates_6(int pos, int kind, int state)
		{
			this.jjmatchedKind = kind;
			this.jjmatchedPos = pos;
			int result;
			try
			{
				this.curChar = this.input_stream.ReadChar();
			}
			catch (IOException var_0_24)
			{
				result = pos + 1;
				return result;
			}
			result = this.jjMoveNfa_6(state, pos + 1);
			return result;
		}

		private int jjMoveStringLiteralDfa0_6()
		{
			char c = this.curChar;
			int result;
			if (c != '#')
			{
				if (c != '*')
				{
					result = this.jjMoveNfa_6(3, 0);
				}
				else
				{
					result = this.jjMoveStringLiteralDfa1_6(2097152L);
				}
			}
			else
			{
				this.jjmatchedKind = 15;
				result = this.jjMoveStringLiteralDfa1_6(20480L);
			}
			return result;
		}

		private int jjMoveStringLiteralDfa1_6(long active0)
		{
			int result;
			try
			{
				this.curChar = this.input_stream.ReadChar();
			}
			catch (IOException var_0_16)
			{
				this.jjStopStringLiteralDfa_6(0, active0);
				result = 1;
				return result;
			}
			char c = this.curChar;
			if (c != '#')
			{
				if (c == '*')
				{
					if ((active0 & 16384L) != 0L)
					{
						result = this.jjStartNfaWithStates_6(1, 14, 0);
						return result;
					}
				}
			}
			else
			{
				if ((active0 & 4096L) != 0L)
				{
					result = this.jjStopAtPos(1, 12);
					return result;
				}
				if ((active0 & 2097152L) != 0L)
				{
					result = this.jjStopAtPos(1, 21);
					return result;
				}
			}
			result = this.jjStartNfa_6(0, active0);
			return result;
		}

		private int jjMoveNfa_6(int startState, int curPos)
		{
			int num = 0;
			this.jjnewStateCnt = 12;
			int num2 = 1;
			this.jjstateSet[0] = (uint)startState;
			int num3 = 2147483647;
			int result;
			while (true)
			{
				if ((this.jjround += 1u) == 2147483647u)
				{
					this.ReInitRounds();
				}
				if (this.curChar < '@')
				{
					long num4 = 1L << (int)this.curChar;
					while (true)
					{
						switch (this.jjstateSet[--num2])
						{
						case 0u:
							if (this.curChar == '*')
							{
								this.jjstateSet[this.jjnewStateCnt++] = 1u;
							}
							break;
						case 1u:
							if ((-34359738369L & num4) != 0L && num3 > 13)
							{
								num3 = 13;
							}
							break;
						case 2u:
							if (this.curChar == '*')
							{
								this.jjstateSet[this.jjnewStateCnt++] = 0u;
							}
							break;
						case 3u:
							if (this.curChar == '$')
							{
								if (num3 > 10)
								{
									num3 = 10;
								}
								this.jjCheckNAddTwoStates(9, 10);
							}
							else if (this.curChar == '#')
							{
								this.jjstateSet[this.jjnewStateCnt++] = 2u;
							}
							break;
						case 6u:
							if (this.curChar == '$' && num3 > 10)
							{
								num3 = 10;
							}
							break;
						case 8u:
							if (this.curChar == '$')
							{
								this.jjCheckNAddTwoStates(9, 10);
							}
							break;
						case 10u:
							if (this.curChar == '!' && num3 > 11)
							{
								num3 = 11;
							}
							break;
						case 11u:
							if (this.curChar == '$')
							{
								if (num3 > 10)
								{
									num3 = 10;
								}
								this.jjCheckNAddTwoStates(9, 10);
							}
							break;
						}
						//IL_24E:
						if (num2 == num)
						{
							break;
						}
						continue;
						//goto IL_24E;
					}
				}
				else if (this.curChar < '\u0080')
				{
					long num4 = 1L << (int)this.curChar;
					while (true)
					{
						switch (this.jjstateSet[--num2])
						{
						case 1u:
							if (num3 > 13)
							{
								num3 = 13;
							}
							break;
						case 3u:
							if (this.curChar == '\\')
							{
								this.jjCheckNAddStates(28, 31);
							}
							break;
						case 5u:
							if (this.curChar == '\\')
							{
								this.jjCheckNAddTwoStates(5, 6);
							}
							break;
						case 7u:
							if (this.curChar == '\\')
							{
								this.jjCheckNAddTwoStates(7, 8);
							}
							break;
						case 9u:
							if (this.curChar == '\\')
							{
								this.jjAddStates(32, 33);
							}
							break;
						}
						//IL_364:
						if (num2 == num)
						{
							break;
						}
						continue;
						//goto IL_364;
					}
				}
				else
				{
					int num5 = (int)(this.curChar >> 8);
					int i = num5 >> 6;
					long l = 1L << num5;
					int i2 = (int)((this.curChar & 'ÿ') >> 6);
					long l2 = 1L << (int)this.curChar;
					do
					{
						uint num6 = this.jjstateSet[--num2];
						if (num6 == 1u)
						{
							if (ParserTokenManager.jjCanMove_0(num5, i, i2, l, l2) && num3 > 13)
							{
								num3 = 13;
							}
						}
					}
					while (num2 != num);
				}
				if (num3 != 2147483647)
				{
					this.jjmatchedKind = num3;
					this.jjmatchedPos = curPos;
					num3 = 2147483647;
				}
				curPos++;
				if ((num2 = this.jjnewStateCnt) == (num = 12 - (this.jjnewStateCnt = num)))
				{
					break;
				}
				try
				{
					this.curChar = this.input_stream.ReadChar();
				}
				catch (IOException var_11_474)
				{
					result = curPos;
					return result;
				}
			}
			result = curPos;
			return result;
		}

		private int jjStopStringLiteralDfa_4(int pos, long active0)
		{
			int result;
			switch (pos)
			{
			case 0:
				if ((active0 & 53248L) != 0L)
				{
					result = 2;
				}
				else if ((active0 & 35184372088832L) != 0L)
				{
					this.jjmatchedKind = 52;
					result = 22;
				}
				else if ((active0 & 158329674399744L) != 0L)
				{
					this.jjmatchedKind = 52;
					result = 7;
				}
				else
				{
					result = -1;
				}
				break;
			case 1:
				if ((active0 & 16384L) != 0L)
				{
					result = 0;
				}
				else if ((active0 & 35184372088832L) != 0L)
				{
					this.jjmatchedKind = 52;
					this.jjmatchedPos = 1;
					result = 28;
				}
				else if ((active0 & 140737488355328L) != 0L)
				{
					this.jjmatchedKind = 52;
					this.jjmatchedPos = 1;
					result = 7;
				}
				else if ((active0 & 17592186044416L) != 0L)
				{
					result = 7;
				}
				else
				{
					result = -1;
				}
				break;
			case 2:
				if ((active0 & 35184372088832L) != 0L)
				{
					this.jjmatchedKind = 52;
					this.jjmatchedPos = 2;
					result = 23;
				}
				else if ((active0 & 140737488355328L) != 0L)
				{
					this.jjmatchedKind = 52;
					this.jjmatchedPos = 2;
					result = 7;
				}
				else
				{
					result = -1;
				}
				break;
			case 3:
				if ((active0 & 140737488355328L) != 0L)
				{
					result = 7;
				}
				else if ((active0 & 35184372088832L) != 0L)
				{
					this.jjmatchedKind = 46;
					this.jjmatchedPos = 3;
					result = 30;
				}
				else
				{
					result = -1;
				}
				break;
			case 4:
				if ((active0 & 35184372088832L) != 0L)
				{
					this.jjmatchedKind = 52;
					this.jjmatchedPos = 4;
					result = 7;
				}
				else
				{
					result = -1;
				}
				break;
			default:
				result = -1;
				break;
			}
			return result;
		}

		private int jjStartNfa_4(int pos, long active0)
		{
			return this.jjMoveNfa_4(this.jjStopStringLiteralDfa_4(pos, active0), pos + 1);
		}

		private int jjStartNfaWithStates_4(int pos, int kind, int state)
		{
			this.jjmatchedKind = kind;
			this.jjmatchedPos = pos;
			int result;
			try
			{
				this.curChar = this.input_stream.ReadChar();
			}
			catch (IOException var_0_24)
			{
				result = pos + 1;
				return result;
			}
			result = this.jjMoveNfa_4(state, pos + 1);
			return result;
		}

		private int jjMoveStringLiteralDfa0_4()
		{
			char c = this.curChar;
			int result;
			if (c <= 'e')
			{
				if (c == '#')
				{
					this.jjmatchedKind = 15;
					result = this.jjMoveStringLiteralDfa1_4(20480L);
					return result;
				}
				if (c == 'e')
				{
					result = this.jjMoveStringLiteralDfa1_4(35184372088832L);
					return result;
				}
			}
			else
			{
				if (c == 'i')
				{
					result = this.jjMoveStringLiteralDfa1_4(17592186044416L);
					return result;
				}
				if (c == 's')
				{
					result = this.jjMoveStringLiteralDfa1_4(140737488355328L);
					return result;
				}
			}
			result = this.jjMoveNfa_4(3, 0);
			return result;
		}

		private int jjMoveStringLiteralDfa1_4(long active0)
		{
			int result;
			try
			{
				this.curChar = this.input_stream.ReadChar();
			}
			catch (IOException var_0_16)
			{
				this.jjStopStringLiteralDfa_4(0, active0);
				result = 1;
				return result;
			}
			char c = this.curChar;
			if (c <= '*')
			{
				if (c != '#')
				{
					if (c == '*')
					{
						if ((active0 & 16384L) != 0L)
						{
							result = this.jjStartNfaWithStates_4(1, 14, 0);
							return result;
						}
					}
				}
				else if ((active0 & 4096L) != 0L)
				{
					result = this.jjStopAtPos(1, 12);
					return result;
				}
			}
			else if (c != 'f')
			{
				if (c == 'l')
				{
					result = this.jjMoveStringLiteralDfa2_4(active0, 35184372088832L);
					return result;
				}
				if (c == 't')
				{
					result = this.jjMoveStringLiteralDfa2_4(active0, 140737488355328L);
					return result;
				}
			}
			else if ((active0 & 17592186044416L) != 0L)
			{
				result = this.jjStartNfaWithStates_4(1, 44, 7);
				return result;
			}
			result = this.jjStartNfa_4(0, active0);
			return result;
		}

		private int jjMoveStringLiteralDfa2_4(long old0, long active0)
		{
			int result;
			if ((active0 &= old0) == 0L)
			{
				result = this.jjStartNfa_4(0, old0);
			}
			else
			{
				try
				{
					this.curChar = this.input_stream.ReadChar();
				}
				catch (IOException var_0_32)
				{
					this.jjStopStringLiteralDfa_4(1, active0);
					result = 2;
					return result;
				}
				char c = this.curChar;
				if (c != 'o')
				{
					if (c != 's')
					{
						result = this.jjStartNfa_4(1, active0);
					}
					else
					{
						result = this.jjMoveStringLiteralDfa3_4(active0, 35184372088832L);
					}
				}
				else
				{
					result = this.jjMoveStringLiteralDfa3_4(active0, 140737488355328L);
				}
			}
			return result;
		}

		private int jjMoveStringLiteralDfa3_4(long old0, long active0)
		{
			int result;
			if ((active0 &= old0) == 0L)
			{
				result = this.jjStartNfa_4(1, old0);
			}
			else
			{
				try
				{
					this.curChar = this.input_stream.ReadChar();
				}
				catch (IOException var_0_32)
				{
					this.jjStopStringLiteralDfa_4(2, active0);
					result = 3;
					return result;
				}
				char c = this.curChar;
				if (c != 'e')
				{
					if (c == 'p')
					{
						if ((active0 & 140737488355328L) != 0L)
						{
							result = this.jjStartNfaWithStates_4(3, 47, 7);
							return result;
						}
					}
					result = this.jjStartNfa_4(2, active0);
				}
				else
				{
					result = this.jjMoveStringLiteralDfa4_4(active0, 35184372088832L);
				}
			}
			return result;
		}

		private int jjMoveStringLiteralDfa4_4(long old0, long active0)
		{
			int result;
			if ((active0 &= old0) == 0L)
			{
				result = this.jjStartNfa_4(2, old0);
			}
			else
			{
				try
				{
					this.curChar = this.input_stream.ReadChar();
				}
				catch (IOException var_0_32)
				{
					this.jjStopStringLiteralDfa_4(3, active0);
					result = 4;
					return result;
				}
				char c = this.curChar;
				if (c != 'i')
				{
					result = this.jjStartNfa_4(3, active0);
				}
				else
				{
					result = this.jjMoveStringLiteralDfa5_4(active0, 35184372088832L);
				}
			}
			return result;
		}

		private int jjMoveStringLiteralDfa5_4(long old0, long active0)
		{
			int result;
			if ((active0 &= old0) == 0L)
			{
				result = this.jjStartNfa_4(3, old0);
			}
			else
			{
				try
				{
					this.curChar = this.input_stream.ReadChar();
				}
				catch (IOException var_0_32)
				{
					this.jjStopStringLiteralDfa_4(4, active0);
					result = 5;
					return result;
				}
				char c = this.curChar;
				if (c == 'f')
				{
					if ((active0 & 35184372088832L) != 0L)
					{
						result = this.jjStartNfaWithStates_4(5, 45, 7);
						return result;
					}
				}
				result = this.jjStartNfa_4(4, active0);
			}
			return result;
		}

		private int jjMoveNfa_4(int startState, int curPos)
		{
			int num = 0;
			this.jjnewStateCnt = 30;
			int num2 = 1;
			this.jjstateSet[0] = (uint)startState;
			int num3 = 2147483647;
			int result;
			while (true)
			{
				if ((this.jjround += 1u) == 2147483647u)
				{
					this.ReInitRounds();
				}
				if (this.curChar < '@')
				{
					long num4 = 1L << (int)this.curChar;
					while (true)
					{
						switch (this.jjstateSet[--num2])
						{
						case 0u:
							if (this.curChar == '*')
							{
								this.jjstateSet[this.jjnewStateCnt++] = 1u;
							}
							break;
						case 1u:
							if ((-34359738369L & num4) != 0L && num3 > 13)
							{
								num3 = 13;
							}
							break;
						case 2u:
							if (this.curChar == '*')
							{
								this.jjstateSet[this.jjnewStateCnt++] = 0u;
							}
							break;
						case 3u:
							if ((287948901175001088L & num4) != 0L)
							{
								if (num3 > 49)
								{
									num3 = 49;
								}
								this.jjCheckNAdd(5);
							}
							else if (this.curChar == '$')
							{
								if (num3 > 10)
								{
									num3 = 10;
								}
								this.jjCheckNAddTwoStates(13, 14);
							}
							else if (this.curChar == '-')
							{
								this.jjCheckNAdd(5);
							}
							else if (this.curChar == '#')
							{
								this.jjstateSet[this.jjnewStateCnt++] = 2u;
							}
							break;
						case 4u:
							if (this.curChar == '-')
							{
								this.jjCheckNAdd(5);
							}
							break;
						case 5u:
							if ((287948901175001088L & num4) != 0L)
							{
								if (num3 > 49)
								{
									num3 = 49;
								}
								this.jjCheckNAdd(5);
							}
							break;
						case 7u:
						case 22u:
							if ((287948901175001088L & num4) != 0L)
							{
								if (num3 > 52)
								{
									num3 = 52;
								}
								this.jjCheckNAdd(7);
							}
							break;
						case 10u:
							if (this.curChar == '$' && num3 > 10)
							{
								num3 = 10;
							}
							break;
						case 12u:
							if (this.curChar == '$')
							{
								this.jjCheckNAddTwoStates(13, 14);
							}
							break;
						case 14u:
							if (this.curChar == '!' && num3 > 11)
							{
								num3 = 11;
							}
							break;
						case 15u:
							if (this.curChar == '$')
							{
								if (num3 > 10)
								{
									num3 = 10;
								}
								this.jjCheckNAddTwoStates(13, 14);
							}
							break;
						case 18u:
							if ((4294967808L & num4) != 0L)
							{
								this.jjAddStates(37, 39);
							}
							break;
						case 19u:
							if ((9216L & num4) != 0L && num3 > 43)
							{
								num3 = 43;
							}
							break;
						case 20u:
							if (this.curChar == '\n' && num3 > 43)
							{
								num3 = 43;
							}
							break;
						case 21u:
							if (this.curChar == '\r')
							{
								this.jjstateSet[this.jjnewStateCnt++] = 20u;
							}
							break;
						case 23u:
							if ((287948901175001088L & num4) != 0L)
							{
								if (num3 > 52)
								{
									num3 = 52;
								}
								this.jjCheckNAdd(7);
							}
							break;
						case 24u:
							if ((4294967808L & num4) != 0L)
							{
								this.jjCheckNAddStates(34, 36);
							}
							break;
						case 25u:
							if ((9216L & num4) != 0L && num3 > 46)
							{
								num3 = 46;
							}
							break;
						case 26u:
							if (this.curChar == '\n' && num3 > 46)
							{
								num3 = 46;
							}
							break;
						case 27u:
							if (this.curChar == '\r')
							{
								this.jjstateSet[this.jjnewStateCnt++] = 26u;
							}
							break;
						case 28u:
							if ((287948901175001088L & num4) != 0L)
							{
								if (num3 > 52)
								{
									num3 = 52;
								}
								this.jjCheckNAdd(7);
							}
							break;
						case 30u:
							if ((287948901175001088L & num4) != 0L)
							{
								if (num3 > 52)
								{
									num3 = 52;
								}
								this.jjCheckNAdd(7);
							}
							else if ((9216L & num4) != 0L)
							{
								if (num3 > 46)
								{
									num3 = 46;
								}
							}
							else if ((4294967808L & num4) != 0L)
							{
								this.jjCheckNAddStates(34, 36);
							}
							if (this.curChar == '\r')
							{
								this.jjstateSet[this.jjnewStateCnt++] = 26u;
							}
							break;
						}
						IL_60C:
						if (num2 == num)
						{
							break;
						}
						continue;
						goto IL_60C;
					}
				}
				else if (this.curChar < '\u0080')
				{
					long num4 = 1L << (int)this.curChar;
					while (true)
					{
						uint num5 = this.jjstateSet[--num2];
						switch (num5)
						{
						case 1u:
							if (num3 > 13)
							{
								num3 = 13;
							}
							break;
						case 2u:
						case 4u:
						case 5u:
						case 10u:
						case 12u:
						case 14u:
						case 15u:
						case 18u:
						case 19u:
						case 20u:
						case 21u:
							break;
						case 3u:
							if ((576460745995190270L & num4) != 0L)
							{
								if (num3 > 52)
								{
									num3 = 52;
								}
								this.jjCheckNAdd(7);
							}
							else if (this.curChar == '\\')
							{
								this.jjCheckNAddStates(40, 43);
							}
							if (this.curChar == 'e')
							{
								this.jjAddStates(44, 45);
							}
							break;
						case 6u:
							if ((576460745995190270L & num4) != 0L)
							{
								if (num3 > 52)
								{
									num3 = 52;
								}
								this.jjCheckNAdd(7);
							}
							break;
						case 7u:
							goto IL_753;
						case 8u:
							if (this.curChar == '\\')
							{
								this.jjCheckNAddStates(40, 43);
							}
							break;
						case 9u:
							if (this.curChar == '\\')
							{
								this.jjCheckNAddTwoStates(9, 10);
							}
							break;
						case 11u:
							if (this.curChar == '\\')
							{
								this.jjCheckNAddTwoStates(11, 12);
							}
							break;
						case 13u:
							if (this.curChar == '\\')
							{
								this.jjAddStates(46, 47);
							}
							break;
						case 16u:
							if (this.curChar == 'e')
							{
								this.jjAddStates(44, 45);
							}
							break;
						case 17u:
							if (this.curChar == 'd')
							{
								if (num3 > 43)
								{
									num3 = 43;
								}
								this.jjAddStates(37, 39);
							}
							break;
						case 22u:
							if ((576460745995190270L & num4) != 0L)
							{
								if (num3 > 52)
								{
									num3 = 52;
								}
								this.jjCheckNAdd(7);
							}
							if (this.curChar == 'l')
							{
								this.jjstateSet[this.jjnewStateCnt++] = 28u;
							}
							else if (this.curChar == 'n')
							{
								this.jjstateSet[this.jjnewStateCnt++] = 17u;
							}
							break;
						case 23u:
							if ((576460745995190270L & num4) != 0L)
							{
								if (num3 > 52)
								{
									num3 = 52;
								}
								this.jjCheckNAdd(7);
							}
							if (this.curChar == 'e')
							{
								if (num3 > 46)
								{
									num3 = 46;
								}
								this.jjAddStates(34, 36);
							}
							break;
						default:
							switch (num5)
							{
							case 28u:
								if ((576460745995190270L & num4) != 0L)
								{
									if (num3 > 52)
									{
										num3 = 52;
									}
									this.jjCheckNAdd(7);
								}
								if (this.curChar == 's')
								{
									this.jjstateSet[this.jjnewStateCnt++] = 23u;
								}
								break;
							case 29u:
								if (this.curChar == 'l')
								{
									this.jjstateSet[this.jjnewStateCnt++] = 28u;
								}
								break;
							case 30u:
								goto IL_753;
							}
							break;
						}
						IL_A62:
						if (num2 == num)
						{
							break;
						}
						continue;
						IL_753:
						if ((576460745995190270L & num4) != 0L)
						{
							if (num3 > 52)
							{
								num3 = 52;
							}
							this.jjCheckNAdd(7);
						}
						goto IL_A62;
					}
				}
				else
				{
					int num6 = (int)(this.curChar >> 8);
					int i = num6 >> 6;
					long l = 1L << num6;
					int i2 = (int)((this.curChar & 'ÿ') >> 6);
					long l2 = 1L << (int)this.curChar;
					do
					{
						uint num5 = this.jjstateSet[--num2];
						if (num5 == 1u)
						{
							if (ParserTokenManager.jjCanMove_0(num6, i, i2, l, l2) && num3 > 13)
							{
								num3 = 13;
							}
						}
					}
					while (num2 != num);
				}
				if (num3 != 2147483647)
				{
					this.jjmatchedKind = num3;
					this.jjmatchedPos = curPos;
					num3 = 2147483647;
				}
				curPos++;
				if ((num2 = this.jjnewStateCnt) == (num = 30 - (this.jjnewStateCnt = num)))
				{
					break;
				}
				try
				{
					this.curChar = this.input_stream.ReadChar();
				}
				catch (IOException var_11_B72)
				{
					result = curPos;
					return result;
				}
			}
			result = curPos;
			return result;
		}

		private int jjStopStringLiteralDfa_3(int pos, long active0)
		{
			int result;
			if (pos != 0)
			{
				result = -1;
			}
			else if ((active0 & 196608L) != 0L)
			{
				result = 7;
			}
			else if ((active0 & 53248L) != 0L)
			{
				result = 14;
			}
			else
			{
				result = -1;
			}
			return result;
		}

		private int jjStartNfa_3(int pos, long active0)
		{
			return this.jjMoveNfa_3(this.jjStopStringLiteralDfa_3(pos, active0), pos + 1);
		}

		private int jjStartNfaWithStates_3(int pos, int kind, int state)
		{
			this.jjmatchedKind = kind;
			this.jjmatchedPos = pos;
			int result;
			try
			{
				this.curChar = this.input_stream.ReadChar();
			}
			catch (IOException var_0_24)
			{
				result = pos + 1;
				return result;
			}
			result = this.jjMoveNfa_3(state, pos + 1);
			return result;
		}

		private int jjMoveStringLiteralDfa0_3()
		{
			char c = this.curChar;
			int result;
			if (c != '#')
			{
				if (c != '\\')
				{
					result = this.jjMoveNfa_3(11, 0);
				}
				else
				{
					this.jjmatchedKind = 17;
					result = this.jjMoveStringLiteralDfa1_3(65536L);
				}
			}
			else
			{
				this.jjmatchedKind = 15;
				result = this.jjMoveStringLiteralDfa1_3(20480L);
			}
			return result;
		}

		private int jjMoveStringLiteralDfa1_3(long active0)
		{
			int result;
			try
			{
				this.curChar = this.input_stream.ReadChar();
			}
			catch (IOException var_0_16)
			{
				this.jjStopStringLiteralDfa_3(0, active0);
				result = 1;
				return result;
			}
			char c = this.curChar;
			if (c != '#')
			{
				if (c != '*')
				{
					if (c == '\\')
					{
						if ((active0 & 65536L) != 0L)
						{
							result = this.jjStartNfaWithStates_3(1, 16, 23);
							return result;
						}
					}
				}
				else if ((active0 & 16384L) != 0L)
				{
					result = this.jjStartNfaWithStates_3(1, 14, 12);
					return result;
				}
			}
			else if ((active0 & 4096L) != 0L)
			{
				result = this.jjStopAtPos(1, 12);
				return result;
			}
			result = this.jjStartNfa_3(0, active0);
			return result;
		}

		private int jjMoveNfa_3(int startState, int curPos)
		{
			int num = 0;
			this.jjnewStateCnt = 23;
			int num2 = 1;
			this.jjstateSet[0] = (uint)startState;
			int num3 = 2147483647;
			int result;
			while (true)
			{
				if ((this.jjround += 1u) == 2147483647u)
				{
					this.ReInitRounds();
				}
				if (this.curChar < '@')
				{
					long num4 = 1L << (int)this.curChar;
					while (true)
					{
						switch (this.jjstateSet[--num2])
						{
						case 0u:
							if ((4294967808L & num4) != 0L)
							{
								this.jjCheckNAddTwoStates(0, 4);
							}
							break;
						case 4u:
							if (this.curChar == '#')
							{
								this.jjCheckNAdd(3);
							}
							break;
						case 5u:
							if ((-103079215105L & num4) != 0L)
							{
								if (num3 > 18)
								{
									num3 = 18;
								}
								this.jjCheckNAdd(5);
							}
							break;
						case 7u:
							if (this.curChar == '$')
							{
								this.jjCheckNAddTwoStates(20, 21);
							}
							else if (this.curChar == '#')
							{
								this.jjstateSet[this.jjnewStateCnt++] = 9u;
							}
							if (this.curChar == '$')
							{
								if (num3 > 10)
								{
									num3 = 10;
								}
							}
							break;
						case 8u:
							if (this.curChar == '#')
							{
								this.jjstateSet[this.jjnewStateCnt++] = 9u;
							}
							break;
						case 10u:
							if ((287948901175001088L & num4) != 0L)
							{
								if (num3 > 8)
								{
									num3 = 8;
								}
								this.jjstateSet[this.jjnewStateCnt++] = 10u;
							}
							break;
						case 11u:
							if ((-103079215105L & num4) != 0L)
							{
								if (num3 > 18)
								{
									num3 = 18;
								}
								this.jjCheckNAdd(5);
							}
							else if (this.curChar == '$')
							{
								if (num3 > 10)
								{
									num3 = 10;
								}
								this.jjCheckNAddTwoStates(20, 21);
							}
							else if (this.curChar == '#')
							{
								this.jjCheckNAddTwoStates(3, 14);
							}
							if ((4294967808L & num4) != 0L)
							{
								this.jjCheckNAddTwoStates(0, 4);
							}
							break;
						case 12u:
							if (this.curChar == '*')
							{
								this.jjstateSet[this.jjnewStateCnt++] = 13u;
							}
							break;
						case 13u:
							if ((-34359738369L & num4) != 0L && num3 > 13)
							{
								num3 = 13;
							}
							break;
						case 14u:
							if (this.curChar == '*')
							{
								this.jjstateSet[this.jjnewStateCnt++] = 12u;
							}
							break;
						case 17u:
							if (this.curChar == '$' && num3 > 10)
							{
								num3 = 10;
							}
							break;
						case 19u:
							if (this.curChar == '$')
							{
								this.jjCheckNAddTwoStates(20, 21);
							}
							break;
						case 21u:
							if (this.curChar == '!' && num3 > 11)
							{
								num3 = 11;
							}
							break;
						case 22u:
							if (this.curChar == '$')
							{
								if (num3 > 10)
								{
									num3 = 10;
								}
								this.jjCheckNAddTwoStates(20, 21);
							}
							break;
						case 23u:
							if (this.curChar == '$')
							{
								this.jjCheckNAddTwoStates(20, 21);
							}
							if (this.curChar == '$')
							{
								if (num3 > 10)
								{
									num3 = 10;
								}
							}
							break;
						}
						IL_490:
						if (num2 == num)
						{
							break;
						}
						continue;
						goto IL_490;
					}
				}
				else if (this.curChar < '\u0080')
				{
					long num4 = 1L << (int)this.curChar;
					while (true)
					{
						switch (this.jjstateSet[--num2])
						{
						case 1u:
							if (this.curChar == 't' && num3 > 9)
							{
								num3 = 9;
							}
							break;
						case 2u:
							if (this.curChar == 'e')
							{
								this.jjstateSet[this.jjnewStateCnt++] = 1u;
							}
							break;
						case 3u:
						case 14u:
							if (this.curChar == 's')
							{
								this.jjstateSet[this.jjnewStateCnt++] = 2u;
							}
							break;
						case 5u:
							if ((-268435457L & num4) != 0L)
							{
								if (num3 > 18)
								{
									num3 = 18;
								}
								this.jjCheckNAdd(5);
							}
							break;
						case 6u:
							if (this.curChar == '\\')
							{
								this.jjAddStates(48, 49);
							}
							break;
						case 7u:
							if (this.curChar == '\\')
							{
								this.jjCheckNAddTwoStates(18, 19);
							}
							if (this.curChar == '\\')
							{
								this.jjCheckNAddTwoStates(16, 17);
							}
							if (this.curChar == '\\')
							{
								this.jjstateSet[this.jjnewStateCnt++] = 6u;
							}
							break;
						case 9u:
						case 10u:
							if ((576460745995190270L & num4) != 0L)
							{
								if (num3 > 8)
								{
									num3 = 8;
								}
								this.jjCheckNAdd(10);
							}
							break;
						case 11u:
							if ((-268435457L & num4) != 0L)
							{
								if (num3 > 18)
								{
									num3 = 18;
								}
								this.jjCheckNAdd(5);
							}
							else if (this.curChar == '\\')
							{
								this.jjCheckNAddStates(50, 53);
							}
							if (this.curChar == '\\')
							{
								this.jjAddStates(48, 49);
							}
							break;
						case 13u:
							if (num3 > 13)
							{
								num3 = 13;
							}
							break;
						case 15u:
							if (this.curChar == '\\')
							{
								this.jjCheckNAddStates(50, 53);
							}
							break;
						case 16u:
							if (this.curChar == '\\')
							{
								this.jjCheckNAddTwoStates(16, 17);
							}
							break;
						case 18u:
							if (this.curChar == '\\')
							{
								this.jjCheckNAddTwoStates(18, 19);
							}
							break;
						case 20u:
							if (this.curChar == '\\')
							{
								this.jjAddStates(54, 55);
							}
							break;
						case 23u:
							if (this.curChar == '\\')
							{
								this.jjAddStates(48, 49);
							}
							if (this.curChar == '\\')
							{
								this.jjCheckNAddTwoStates(18, 19);
							}
							if (this.curChar == '\\')
							{
								this.jjCheckNAddTwoStates(16, 17);
							}
							break;
						}
						IL_84C:
						if (num2 == num)
						{
							break;
						}
						continue;
						goto IL_84C;
					}
				}
				else
				{
					int num5 = (int)(this.curChar >> 8);
					int i = num5 >> 6;
					long l = 1L << num5;
					int i2 = (int)((this.curChar & 'ÿ') >> 6);
					long l2 = 1L << (int)this.curChar;
					while (true)
					{
						uint num6 = this.jjstateSet[--num2];
						if (num6 == 5u)
						{
							goto IL_8CE;
						}
						switch (num6)
						{
						case 11u:
							goto IL_8CE;
						case 13u:
							if (ParserTokenManager.jjCanMove_0(num5, i, i2, l, l2) && num3 > 13)
							{
								num3 = 13;
							}
							break;
						}
						IL_92D:
						if (num2 == num)
						{
							break;
						}
						continue;
						IL_8CE:
						if (!ParserTokenManager.jjCanMove_0(num5, i, i2, l, l2))
						{
							goto IL_92D;
						}
						if (num3 > 18)
						{
							num3 = 18;
						}
						this.jjCheckNAdd(5);
						goto IL_92D;
					}
				}
				if (num3 != 2147483647)
				{
					this.jjmatchedKind = num3;
					this.jjmatchedPos = curPos;
					num3 = 2147483647;
				}
				curPos++;
				if ((num2 = this.jjnewStateCnt) == (num = 23 - (this.jjnewStateCnt = num)))
				{
					break;
				}
				try
				{
					this.curChar = this.input_stream.ReadChar();
				}
				catch (IOException var_11_9A9)
				{
					result = curPos;
					return result;
				}
			}
			result = curPos;
			return result;
		}

		private int jjStopStringLiteralDfa_7(int pos, long active0)
		{
			int result;
			if (pos != 0)
			{
				result = -1;
			}
			else if ((active0 & 53248L) != 0L)
			{
				result = 2;
			}
			else
			{
				result = -1;
			}
			return result;
		}

		private int jjStartNfa_7(int pos, long active0)
		{
			return this.jjMoveNfa_7(this.jjStopStringLiteralDfa_7(pos, active0), pos + 1);
		}

		private int jjStartNfaWithStates_7(int pos, int kind, int state)
		{
			this.jjmatchedKind = kind;
			this.jjmatchedPos = pos;
			int result;
			try
			{
				this.curChar = this.input_stream.ReadChar();
			}
			catch (IOException var_0_24)
			{
				result = pos + 1;
				return result;
			}
			result = this.jjMoveNfa_7(state, pos + 1);
			return result;
		}

		private int jjMoveStringLiteralDfa0_7()
		{
			char c = this.curChar;
			int result;
			if (c != '#')
			{
				if (c != '*')
				{
					result = this.jjMoveNfa_7(3, 0);
				}
				else
				{
					result = this.jjMoveStringLiteralDfa1_7(1048576L);
				}
			}
			else
			{
				this.jjmatchedKind = 15;
				result = this.jjMoveStringLiteralDfa1_7(20480L);
			}
			return result;
		}

		private int jjMoveStringLiteralDfa1_7(long active0)
		{
			int result;
			try
			{
				this.curChar = this.input_stream.ReadChar();
			}
			catch (IOException var_0_16)
			{
				this.jjStopStringLiteralDfa_7(0, active0);
				result = 1;
				return result;
			}
			char c = this.curChar;
			if (c != '#')
			{
				if (c == '*')
				{
					if ((active0 & 16384L) != 0L)
					{
						result = this.jjStartNfaWithStates_7(1, 14, 0);
						return result;
					}
				}
			}
			else
			{
				if ((active0 & 4096L) != 0L)
				{
					result = this.jjStopAtPos(1, 12);
					return result;
				}
				if ((active0 & 1048576L) != 0L)
				{
					result = this.jjStopAtPos(1, 20);
					return result;
				}
			}
			result = this.jjStartNfa_7(0, active0);
			return result;
		}

		private int jjMoveNfa_7(int startState, int curPos)
		{
			int num = 0;
			this.jjnewStateCnt = 12;
			int num2 = 1;
			this.jjstateSet[0] = (uint)startState;
			int num3 = 2147483647;
			int result;
			while (true)
			{
				if ((this.jjround += 1u) == 2147483647u)
				{
					this.ReInitRounds();
				}
				if (this.curChar < '@')
				{
					long num4 = 1L << (int)this.curChar;
					while (true)
					{
						switch (this.jjstateSet[--num2])
						{
						case 0u:
							if (this.curChar == '*')
							{
								this.jjstateSet[this.jjnewStateCnt++] = 1u;
							}
							break;
						case 1u:
							if ((-34359738369L & num4) != 0L && num3 > 13)
							{
								num3 = 13;
							}
							break;
						case 2u:
							if (this.curChar == '*')
							{
								this.jjstateSet[this.jjnewStateCnt++] = 0u;
							}
							break;
						case 3u:
							if (this.curChar == '$')
							{
								if (num3 > 10)
								{
									num3 = 10;
								}
								this.jjCheckNAddTwoStates(9, 10);
							}
							else if (this.curChar == '#')
							{
								this.jjstateSet[this.jjnewStateCnt++] = 2u;
							}
							break;
						case 6u:
							if (this.curChar == '$' && num3 > 10)
							{
								num3 = 10;
							}
							break;
						case 8u:
							if (this.curChar == '$')
							{
								this.jjCheckNAddTwoStates(9, 10);
							}
							break;
						case 10u:
							if (this.curChar == '!' && num3 > 11)
							{
								num3 = 11;
							}
							break;
						case 11u:
							if (this.curChar == '$')
							{
								if (num3 > 10)
								{
									num3 = 10;
								}
								this.jjCheckNAddTwoStates(9, 10);
							}
							break;
						}
						IL_24E:
						if (num2 == num)
						{
							break;
						}
						continue;
						goto IL_24E;
					}
				}
				else if (this.curChar < '\u0080')
				{
					long num4 = 1L << (int)this.curChar;
					while (true)
					{
						switch (this.jjstateSet[--num2])
						{
						case 1u:
							if (num3 > 13)
							{
								num3 = 13;
							}
							break;
						case 3u:
							if (this.curChar == '\\')
							{
								this.jjCheckNAddStates(28, 31);
							}
							break;
						case 5u:
							if (this.curChar == '\\')
							{
								this.jjCheckNAddTwoStates(5, 6);
							}
							break;
						case 7u:
							if (this.curChar == '\\')
							{
								this.jjCheckNAddTwoStates(7, 8);
							}
							break;
						case 9u:
							if (this.curChar == '\\')
							{
								this.jjAddStates(32, 33);
							}
							break;
						}
						IL_364:
						if (num2 == num)
						{
							break;
						}
						continue;
						goto IL_364;
					}
				}
				else
				{
					int num5 = (int)(this.curChar >> 8);
					int i = num5 >> 6;
					long l = 1L << num5;
					int i2 = (int)((this.curChar & 'ÿ') >> 6);
					long l2 = 1L << (int)this.curChar;
					do
					{
						uint num6 = this.jjstateSet[--num2];
						if (num6 == 1u)
						{
							if (ParserTokenManager.jjCanMove_0(num5, i, i2, l, l2) && num3 > 13)
							{
								num3 = 13;
							}
						}
					}
					while (num2 != num);
				}
				if (num3 != 2147483647)
				{
					this.jjmatchedKind = num3;
					this.jjmatchedPos = curPos;
					num3 = 2147483647;
				}
				curPos++;
				if ((num2 = this.jjnewStateCnt) == (num = 12 - (this.jjnewStateCnt = num)))
				{
					break;
				}
				try
				{
					this.curChar = this.input_stream.ReadChar();
				}
				catch (IOException var_11_474)
				{
					result = curPos;
					return result;
				}
			}
			result = curPos;
			return result;
		}

		private int jjStopStringLiteralDfa_8(int pos, long active0)
		{
			int result;
			if (pos != 0)
			{
				result = -1;
			}
			else if ((active0 & 53248L) != 0L)
			{
				result = 2;
			}
			else
			{
				result = -1;
			}
			return result;
		}

		private int jjStartNfa_8(int pos, long active0)
		{
			return this.jjMoveNfa_8(this.jjStopStringLiteralDfa_8(pos, active0), pos + 1);
		}

		private int jjStartNfaWithStates_8(int pos, int kind, int state)
		{
			this.jjmatchedKind = kind;
			this.jjmatchedPos = pos;
			int result;
			try
			{
				this.curChar = this.input_stream.ReadChar();
			}
			catch (IOException var_0_24)
			{
				result = pos + 1;
				return result;
			}
			result = this.jjMoveNfa_8(state, pos + 1);
			return result;
		}

		private int jjMoveStringLiteralDfa0_8()
		{
			char c = this.curChar;
			int result;
			if (c != '#')
			{
				result = this.jjMoveNfa_8(3, 0);
			}
			else
			{
				this.jjmatchedKind = 15;
				result = this.jjMoveStringLiteralDfa1_8(20480L);
			}
			return result;
		}

		private int jjMoveStringLiteralDfa1_8(long active0)
		{
			int result;
			try
			{
				this.curChar = this.input_stream.ReadChar();
			}
			catch (IOException var_0_16)
			{
				this.jjStopStringLiteralDfa_8(0, active0);
				result = 1;
				return result;
			}
			char c = this.curChar;
			if (c != '#')
			{
				if (c == '*')
				{
					if ((active0 & 16384L) != 0L)
					{
						result = this.jjStartNfaWithStates_8(1, 14, 0);
						return result;
					}
				}
			}
			else if ((active0 & 4096L) != 0L)
			{
				result = this.jjStopAtPos(1, 12);
				return result;
			}
			result = this.jjStartNfa_8(0, active0);
			return result;
		}

		private int jjMoveNfa_8(int startState, int curPos)
		{
			int num = 0;
			this.jjnewStateCnt = 15;
			int num2 = 1;
			this.jjstateSet[0] = (uint)startState;
			int num3 = 2147483647;
			int result;
			while (true)
			{
				if ((this.jjround += 1u) == 2147483647u)
				{
					this.ReInitRounds();
				}
				if (this.curChar < '@')
				{
					long num4 = 1L << (int)this.curChar;
					while (true)
					{
						switch (this.jjstateSet[--num2])
						{
						case 0u:
							if (this.curChar == '*')
							{
								this.jjstateSet[this.jjnewStateCnt++] = 1u;
							}
							break;
						case 1u:
							if ((-34359738369L & num4) != 0L && num3 > 13)
							{
								num3 = 13;
							}
							break;
						case 2u:
							if (this.curChar == '*')
							{
								this.jjstateSet[this.jjnewStateCnt++] = 0u;
							}
							break;
						case 3u:
							if ((9216L & num4) != 0L)
							{
								if (num3 > 19)
								{
									num3 = 19;
								}
							}
							else if (this.curChar == '$')
							{
								if (num3 > 10)
								{
									num3 = 10;
								}
								this.jjCheckNAddTwoStates(12, 13);
							}
							else if (this.curChar == '#')
							{
								this.jjstateSet[this.jjnewStateCnt++] = 2u;
							}
							if (this.curChar == '\r')
							{
								this.jjstateSet[this.jjnewStateCnt++] = 5u;
							}
							break;
						case 4u:
							if ((9216L & num4) != 0L && num3 > 19)
							{
								num3 = 19;
							}
							break;
						case 5u:
							if (this.curChar == '\n' && num3 > 19)
							{
								num3 = 19;
							}
							break;
						case 6u:
							if (this.curChar == '\r')
							{
								this.jjstateSet[this.jjnewStateCnt++] = 5u;
							}
							break;
						case 9u:
							if (this.curChar == '$' && num3 > 10)
							{
								num3 = 10;
							}
							break;
						case 11u:
							if (this.curChar == '$')
							{
								this.jjCheckNAddTwoStates(12, 13);
							}
							break;
						case 13u:
							if (this.curChar == '!' && num3 > 11)
							{
								num3 = 11;
							}
							break;
						case 14u:
							if (this.curChar == '$')
							{
								if (num3 > 10)
								{
									num3 = 10;
								}
								this.jjCheckNAddTwoStates(12, 13);
							}
							break;
						}
						IL_332:
						if (num2 == num)
						{
							break;
						}
						continue;
						goto IL_332;
					}
				}
				else if (this.curChar < '\u0080')
				{
					long num4 = 1L << (int)this.curChar;
					while (true)
					{
						uint num5 = this.jjstateSet[--num2];
						switch (num5)
						{
						case 1u:
							if (num3 > 13)
							{
								num3 = 13;
							}
							break;
						case 2u:
							break;
						case 3u:
							if (this.curChar == '\\')
							{
								this.jjCheckNAddStates(56, 59);
							}
							break;
						default:
							switch (num5)
							{
							case 8u:
								if (this.curChar == '\\')
								{
									this.jjCheckNAddTwoStates(8, 9);
								}
								break;
							case 10u:
								if (this.curChar == '\\')
								{
									this.jjCheckNAddTwoStates(10, 11);
								}
								break;
							case 12u:
								if (this.curChar == '\\')
								{
									this.jjAddStates(60, 61);
								}
								break;
							}
							break;
						}
						IL_450:
						if (num2 == num)
						{
							break;
						}
						continue;
						goto IL_450;
					}
				}
				else
				{
					int num6 = (int)(this.curChar >> 8);
					int i = num6 >> 6;
					long l = 1L << num6;
					int i2 = (int)((this.curChar & 'ÿ') >> 6);
					long l2 = 1L << (int)this.curChar;
					do
					{
						uint num5 = this.jjstateSet[--num2];
						if (num5 == 1u)
						{
							if (ParserTokenManager.jjCanMove_0(num6, i, i2, l, l2) && num3 > 13)
							{
								num3 = 13;
							}
						}
					}
					while (num2 != num);
				}
				if (num3 != 2147483647)
				{
					this.jjmatchedKind = num3;
					this.jjmatchedPos = curPos;
					num3 = 2147483647;
				}
				curPos++;
				if ((num2 = this.jjnewStateCnt) == (num = 15 - (this.jjnewStateCnt = num)))
				{
					break;
				}
				try
				{
					this.curChar = this.input_stream.ReadChar();
				}
				catch (IOException var_11_560)
				{
					result = curPos;
					return result;
				}
			}
			result = curPos;
			return result;
		}

		private int jjStopStringLiteralDfa_5(int pos, long active0)
		{
			int result;
			switch (pos)
			{
			case 0:
				if ((active0 & 53248L) != 0L)
				{
					result = 2;
				}
				else if ((active0 & 100663296L) != 0L)
				{
					this.jjmatchedKind = 56;
					result = 5;
				}
				else
				{
					result = -1;
				}
				break;
			case 1:
				if ((active0 & 16384L) != 0L)
				{
					result = 0;
				}
				else if ((active0 & 100663296L) != 0L)
				{
					this.jjmatchedKind = 56;
					this.jjmatchedPos = 1;
					result = 5;
				}
				else
				{
					result = -1;
				}
				break;
			case 2:
				if ((active0 & 100663296L) != 0L)
				{
					this.jjmatchedKind = 56;
					this.jjmatchedPos = 2;
					result = 5;
				}
				else
				{
					result = -1;
				}
				break;
			case 3:
				if ((active0 & 67108864L) != 0L)
				{
					this.jjmatchedKind = 56;
					this.jjmatchedPos = 3;
					result = 5;
				}
				else if ((active0 & 33554432L) != 0L)
				{
					result = 5;
				}
				else
				{
					result = -1;
				}
				break;
			default:
				result = -1;
				break;
			}
			return result;
		}

		private int jjStartNfa_5(int pos, long active0)
		{
			return this.jjMoveNfa_5(this.jjStopStringLiteralDfa_5(pos, active0), pos + 1);
		}

		private int jjStartNfaWithStates_5(int pos, int kind, int state)
		{
			this.jjmatchedKind = kind;
			this.jjmatchedPos = pos;
			int result;
			try
			{
				this.curChar = this.input_stream.ReadChar();
			}
			catch (IOException var_0_24)
			{
				result = pos + 1;
				return result;
			}
			result = this.jjMoveNfa_5(state, pos + 1);
			return result;
		}

		private int jjMoveStringLiteralDfa0_5()
		{
			char c = this.curChar;
			int result;
			if (c <= 'f')
			{
				if (c == '#')
				{
					this.jjmatchedKind = 15;
					result = this.jjMoveStringLiteralDfa1_5(20480L);
					return result;
				}
				if (c == 'f')
				{
					result = this.jjMoveStringLiteralDfa1_5(67108864L);
					return result;
				}
			}
			else
			{
				if (c == 't')
				{
					result = this.jjMoveStringLiteralDfa1_5(33554432L);
					return result;
				}
				switch (c)
				{
				case '{':
					result = this.jjStopAtPos(0, 58);
					return result;
				case '}':
					result = this.jjStopAtPos(0, 59);
					return result;
				}
			}
			result = this.jjMoveNfa_5(3, 0);
			return result;
		}

		private int jjMoveStringLiteralDfa1_5(long active0)
		{
			int result;
			try
			{
				this.curChar = this.input_stream.ReadChar();
			}
			catch (IOException var_0_16)
			{
				this.jjStopStringLiteralDfa_5(0, active0);
				result = 1;
				return result;
			}
			char c = this.curChar;
			if (c <= '*')
			{
				if (c != '#')
				{
					if (c == '*')
					{
						if ((active0 & 16384L) != 0L)
						{
							result = this.jjStartNfaWithStates_5(1, 14, 0);
							return result;
						}
					}
				}
				else if ((active0 & 4096L) != 0L)
				{
					result = this.jjStopAtPos(1, 12);
					return result;
				}
			}
			else
			{
				if (c == 'a')
				{
					result = this.jjMoveStringLiteralDfa2_5(active0, 67108864L);
					return result;
				}
				if (c == 'r')
				{
					result = this.jjMoveStringLiteralDfa2_5(active0, 33554432L);
					return result;
				}
			}
			result = this.jjStartNfa_5(0, active0);
			return result;
		}

		private int jjMoveStringLiteralDfa2_5(long old0, long active0)
		{
			int result;
			if ((active0 &= old0) == 0L)
			{
				result = this.jjStartNfa_5(0, old0);
			}
			else
			{
				try
				{
					this.curChar = this.input_stream.ReadChar();
				}
				catch (IOException var_0_32)
				{
					this.jjStopStringLiteralDfa_5(1, active0);
					result = 2;
					return result;
				}
				char c = this.curChar;
				if (c != 'l')
				{
					if (c != 'u')
					{
						result = this.jjStartNfa_5(1, active0);
					}
					else
					{
						result = this.jjMoveStringLiteralDfa3_5(active0, 33554432L);
					}
				}
				else
				{
					result = this.jjMoveStringLiteralDfa3_5(active0, 67108864L);
				}
			}
			return result;
		}

		private int jjMoveStringLiteralDfa3_5(long old0, long active0)
		{
			int result;
			if ((active0 &= old0) == 0L)
			{
				result = this.jjStartNfa_5(1, old0);
			}
			else
			{
				try
				{
					this.curChar = this.input_stream.ReadChar();
				}
				catch (IOException var_0_32)
				{
					this.jjStopStringLiteralDfa_5(2, active0);
					result = 3;
					return result;
				}
				char c = this.curChar;
				if (c != 'e')
				{
					if (c == 's')
					{
						result = this.jjMoveStringLiteralDfa4_5(active0, 67108864L);
						return result;
					}
				}
				else if ((active0 & 33554432L) != 0L)
				{
					result = this.jjStartNfaWithStates_5(3, 25, 5);
					return result;
				}
				result = this.jjStartNfa_5(2, active0);
			}
			return result;
		}

		private int jjMoveStringLiteralDfa4_5(long old0, long active0)
		{
			int result;
			if ((active0 &= old0) == 0L)
			{
				result = this.jjStartNfa_5(2, old0);
			}
			else
			{
				try
				{
					this.curChar = this.input_stream.ReadChar();
				}
				catch (IOException var_0_32)
				{
					this.jjStopStringLiteralDfa_5(3, active0);
					result = 4;
					return result;
				}
				char c = this.curChar;
				if (c == 'e')
				{
					if ((active0 & 67108864L) != 0L)
					{
						result = this.jjStartNfaWithStates_5(4, 26, 5);
						return result;
					}
				}
				result = this.jjStartNfa_5(3, active0);
			}
			return result;
		}

		private int jjMoveNfa_5(int startState, int curPos)
		{
			int num = 0;
			this.jjnewStateCnt = 16;
			int num2 = 1;
			this.jjstateSet[0] = (uint)startState;
			int num3 = 2147483647;
			int result;
			while (true)
			{
				if ((this.jjround += 1u) == 2147483647u)
				{
					this.ReInitRounds();
				}
				if (this.curChar < '@')
				{
					long num4 = 1L << (int)this.curChar;
					while (true)
					{
						switch (this.jjstateSet[--num2])
						{
						case 0u:
							if (this.curChar == '*')
							{
								this.jjstateSet[this.jjnewStateCnt++] = 1u;
							}
							break;
						case 1u:
							if ((-34359738369L & num4) != 0L && num3 > 13)
							{
								num3 = 13;
							}
							break;
						case 2u:
							if (this.curChar == '*')
							{
								this.jjstateSet[this.jjnewStateCnt++] = 0u;
							}
							break;
						case 3u:
							if (this.curChar == '$')
							{
								if (num3 > 10)
								{
									num3 = 10;
								}
								this.jjCheckNAddTwoStates(13, 14);
							}
							else if (this.curChar == '.')
							{
								this.jjstateSet[this.jjnewStateCnt++] = 7u;
							}
							else if (this.curChar == '#')
							{
								this.jjstateSet[this.jjnewStateCnt++] = 2u;
							}
							break;
						case 5u:
							if ((287984085547089920L & num4) != 0L)
							{
								if (num3 > 56)
								{
									num3 = 56;
								}
								this.jjstateSet[this.jjnewStateCnt++] = 5u;
							}
							break;
						case 6u:
							if (this.curChar == '.')
							{
								this.jjstateSet[this.jjnewStateCnt++] = 7u;
							}
							break;
						case 10u:
							if (this.curChar == '$' && num3 > 10)
							{
								num3 = 10;
							}
							break;
						case 12u:
							if (this.curChar == '$')
							{
								this.jjCheckNAddTwoStates(13, 14);
							}
							break;
						case 14u:
							if (this.curChar == '!' && num3 > 11)
							{
								num3 = 11;
							}
							break;
						case 15u:
							if (this.curChar == '$')
							{
								if (num3 > 10)
								{
									num3 = 10;
								}
								this.jjCheckNAddTwoStates(13, 14);
							}
							break;
						}
						IL_312:
						if (num2 == num)
						{
							break;
						}
						continue;
						goto IL_312;
					}
				}
				else if (this.curChar < '\u0080')
				{
					long num4 = 1L << (int)this.curChar;
					while (true)
					{
						switch (this.jjstateSet[--num2])
						{
						case 1u:
							if (num3 > 13)
							{
								num3 = 13;
							}
							break;
						case 3u:
							if ((576460745995190270L & num4) != 0L)
							{
								if (num3 > 56)
								{
									num3 = 56;
								}
								this.jjCheckNAdd(5);
							}
							else if (this.curChar == '\\')
							{
								this.jjCheckNAddStates(40, 43);
							}
							break;
						case 4u:
						case 5u:
							if ((576460745995190270L & num4) != 0L)
							{
								if (num3 > 56)
								{
									num3 = 56;
								}
								this.jjCheckNAdd(5);
							}
							break;
						case 7u:
							if ((576460743847706622L & num4) != 0L && num3 > 57)
							{
								num3 = 57;
							}
							break;
						case 8u:
							if (this.curChar == '\\')
							{
								this.jjCheckNAddStates(40, 43);
							}
							break;
						case 9u:
							if (this.curChar == '\\')
							{
								this.jjCheckNAddTwoStates(9, 10);
							}
							break;
						case 11u:
							if (this.curChar == '\\')
							{
								this.jjCheckNAddTwoStates(11, 12);
							}
							break;
						case 13u:
							if (this.curChar == '\\')
							{
								this.jjAddStates(46, 47);
							}
							break;
						}
						IL_500:
						if (num2 == num)
						{
							break;
						}
						continue;
						goto IL_500;
					}
				}
				else
				{
					int num5 = (int)(this.curChar >> 8);
					int i = num5 >> 6;
					long l = 1L << num5;
					int i2 = (int)((this.curChar & 'ÿ') >> 6);
					long l2 = 1L << (int)this.curChar;
					do
					{
						uint num6 = this.jjstateSet[--num2];
						if (num6 == 1u)
						{
							if (ParserTokenManager.jjCanMove_0(num5, i, i2, l, l2) && num3 > 13)
							{
								num3 = 13;
							}
						}
					}
					while (num2 != num);
				}
				if (num3 != 2147483647)
				{
					this.jjmatchedKind = num3;
					this.jjmatchedPos = curPos;
					num3 = 2147483647;
				}
				curPos++;
				if ((num2 = this.jjnewStateCnt) == (num = 16 - (this.jjnewStateCnt = num)))
				{
					break;
				}
				try
				{
					this.curChar = this.input_stream.ReadChar();
				}
				catch (IOException var_11_610)
				{
					result = curPos;
					return result;
				}
			}
			result = curPos;
			return result;
		}

		private int jjStopStringLiteralDfa_1(int pos, long active0)
		{
			int result;
			switch (pos)
			{
			case 0:
				if ((active0 & 53248L) != 0L)
				{
					result = 2;
				}
				else if ((active0 & 100663296L) != 0L)
				{
					this.jjmatchedKind = 56;
					result = 25;
				}
				else if ((active0 & 16L) != 0L)
				{
					result = 27;
				}
				else
				{
					result = -1;
				}
				break;
			case 1:
				if ((active0 & 16384L) != 0L)
				{
					result = 0;
				}
				else if ((active0 & 100663296L) != 0L)
				{
					this.jjmatchedKind = 56;
					this.jjmatchedPos = 1;
					result = 25;
				}
				else
				{
					result = -1;
				}
				break;
			case 2:
				if ((active0 & 100663296L) != 0L)
				{
					this.jjmatchedKind = 56;
					this.jjmatchedPos = 2;
					result = 25;
				}
				else
				{
					result = -1;
				}
				break;
			case 3:
				if ((active0 & 67108864L) != 0L)
				{
					this.jjmatchedKind = 56;
					this.jjmatchedPos = 3;
					result = 25;
				}
				else if ((active0 & 33554432L) != 0L)
				{
					result = 25;
				}
				else
				{
					result = -1;
				}
				break;
			default:
				result = -1;
				break;
			}
			return result;
		}

		private int jjStartNfa_1(int pos, long active0)
		{
			return this.jjMoveNfa_1(this.jjStopStringLiteralDfa_1(pos, active0), pos + 1);
		}

		private int jjStartNfaWithStates_1(int pos, int kind, int state)
		{
			this.jjmatchedKind = kind;
			this.jjmatchedPos = pos;
			int result;
			try
			{
				this.curChar = this.input_stream.ReadChar();
			}
			catch (IOException var_0_24)
			{
				result = pos + 1;
				return result;
			}
			result = this.jjMoveNfa_1(state, pos + 1);
			return result;
		}

		private int jjMoveStringLiteralDfa0_1()
		{
			char c = this.curChar;
			int result;
			if (c <= '.')
			{
				if (c == '#')
				{
					this.jjmatchedKind = 15;
					result = this.jjMoveStringLiteralDfa1_1(20480L);
					return result;
				}
				if (c == ')')
				{
					result = this.jjStopAtPos(0, 7);
					return result;
				}
				switch (c)
				{
				case ',':
					result = this.jjStopAtPos(0, 3);
					return result;
				case '.':
					result = this.jjMoveStringLiteralDfa1_1(16L);
					return result;
				}
			}
			else if (c <= 'f')
			{
				switch (c)
				{
				case '[':
					result = this.jjStopAtPos(0, 1);
					return result;
				case '\\':
					break;
				case ']':
					result = this.jjStopAtPos(0, 2);
					return result;
				default:
					if (c == 'f')
					{
						result = this.jjMoveStringLiteralDfa1_1(67108864L);
						return result;
					}
					break;
				}
			}
			else
			{
				if (c == 't')
				{
					result = this.jjMoveStringLiteralDfa1_1(33554432L);
					return result;
				}
				switch (c)
				{
				case '{':
					result = this.jjStopAtPos(0, 58);
					return result;
				case '}':
					result = this.jjStopAtPos(0, 59);
					return result;
				}
			}
			result = this.jjMoveNfa_1(3, 0);
			return result;
		}

		private int jjMoveStringLiteralDfa1_1(long active0)
		{
			int result;
			try
			{
				this.curChar = this.input_stream.ReadChar();
			}
			catch (IOException var_0_16)
			{
				this.jjStopStringLiteralDfa_1(0, active0);
				result = 1;
				return result;
			}
			char c = this.curChar;
			if (c <= '*')
			{
				if (c != '#')
				{
					if (c == '*')
					{
						if ((active0 & 16384L) != 0L)
						{
							result = this.jjStartNfaWithStates_1(1, 14, 0);
							return result;
						}
					}
				}
				else if ((active0 & 4096L) != 0L)
				{
					result = this.jjStopAtPos(1, 12);
					return result;
				}
			}
			else if (c != '.')
			{
				if (c == 'a')
				{
					result = this.jjMoveStringLiteralDfa2_1(active0, 67108864L);
					return result;
				}
				if (c == 'r')
				{
					result = this.jjMoveStringLiteralDfa2_1(active0, 33554432L);
					return result;
				}
			}
			else if ((active0 & 16L) != 0L)
			{
				result = this.jjStopAtPos(1, 4);
				return result;
			}
			result = this.jjStartNfa_1(0, active0);
			return result;
		}

		private int jjMoveStringLiteralDfa2_1(long old0, long active0)
		{
			int result;
			if ((active0 &= old0) == 0L)
			{
				result = this.jjStartNfa_1(0, old0);
			}
			else
			{
				try
				{
					this.curChar = this.input_stream.ReadChar();
				}
				catch (IOException var_0_32)
				{
					this.jjStopStringLiteralDfa_1(1, active0);
					result = 2;
					return result;
				}
				char c = this.curChar;
				if (c != 'l')
				{
					if (c != 'u')
					{
						result = this.jjStartNfa_1(1, active0);
					}
					else
					{
						result = this.jjMoveStringLiteralDfa3_1(active0, 33554432L);
					}
				}
				else
				{
					result = this.jjMoveStringLiteralDfa3_1(active0, 67108864L);
				}
			}
			return result;
		}

		private int jjMoveStringLiteralDfa3_1(long old0, long active0)
		{
			int result;
			if ((active0 &= old0) == 0L)
			{
				result = this.jjStartNfa_1(1, old0);
			}
			else
			{
				try
				{
					this.curChar = this.input_stream.ReadChar();
				}
				catch (IOException var_0_32)
				{
					this.jjStopStringLiteralDfa_1(2, active0);
					result = 3;
					return result;
				}
				char c = this.curChar;
				if (c != 'e')
				{
					if (c == 's')
					{
						result = this.jjMoveStringLiteralDfa4_1(active0, 67108864L);
						return result;
					}
				}
				else if ((active0 & 33554432L) != 0L)
				{
					result = this.jjStartNfaWithStates_1(3, 25, 25);
					return result;
				}
				result = this.jjStartNfa_1(2, active0);
			}
			return result;
		}

		private int jjMoveStringLiteralDfa4_1(long old0, long active0)
		{
			int result;
			if ((active0 &= old0) == 0L)
			{
				result = this.jjStartNfa_1(2, old0);
			}
			else
			{
				try
				{
					this.curChar = this.input_stream.ReadChar();
				}
				catch (IOException var_0_32)
				{
					this.jjStopStringLiteralDfa_1(3, active0);
					result = 4;
					return result;
				}
				char c = this.curChar;
				if (c == 'e')
				{
					if ((active0 & 67108864L) != 0L)
					{
						result = this.jjStartNfaWithStates_1(4, 26, 25);
						return result;
					}
				}
				result = this.jjStartNfa_1(3, active0);
			}
			return result;
		}

		private int jjMoveNfa_1(int startState, int curPos)
		{
			int num = 0;
			this.jjnewStateCnt = 36;
			int num2 = 1;
			this.jjstateSet[0] = (uint)startState;
			int num3 = 2147483647;
			int result;
			while (true)
			{
				if ((this.jjround += 1u) == 2147483647u)
				{
					this.ReInitRounds();
				}
				if (this.curChar < '@')
				{
					long num4 = 1L << (int)this.curChar;
					while (true)
					{
						switch (this.jjstateSet[--num2])
						{
						case 0u:
							if (this.curChar == '*')
							{
								this.jjstateSet[this.jjnewStateCnt++] = 1u;
							}
							break;
						case 1u:
							if ((-34359738369L & num4) != 0L && num3 > 13)
							{
								num3 = 13;
							}
							break;
						case 2u:
							if (this.curChar == '*')
							{
								this.jjstateSet[this.jjnewStateCnt++] = 0u;
							}
							break;
						case 3u:
							if ((287948901175001088L & num4) != 0L)
							{
								if (num3 > 49)
								{
									num3 = 49;
								}
								this.jjCheckNAdd(23);
							}
							else if ((4294967808L & num4) != 0L)
							{
								if (num3 > 23)
								{
									num3 = 23;
								}
								this.jjCheckNAdd(4);
							}
							else if (this.curChar == '$')
							{
								if (num3 > 10)
								{
									num3 = 10;
								}
								this.jjCheckNAddTwoStates(33, 34);
							}
							else if (this.curChar == '.')
							{
								this.jjstateSet[this.jjnewStateCnt++] = 27u;
							}
							else if (this.curChar == '-')
							{
								this.jjCheckNAdd(23);
							}
							else if (this.curChar == '\'')
							{
								this.jjCheckNAddStates(62, 64);
							}
							else if (this.curChar == '"')
							{
								this.jjCheckNAddStates(65, 67);
							}
							else if (this.curChar == '#')
							{
								this.jjstateSet[this.jjnewStateCnt++] = 2u;
							}
							break;
						case 4u:
							if ((4294967808L & num4) != 0L)
							{
								if (num3 > 23)
								{
									num3 = 23;
								}
								this.jjCheckNAdd(4);
							}
							break;
						case 5u:
							if (this.curChar == '"')
							{
								this.jjCheckNAddStates(65, 67);
							}
							break;
						case 6u:
							if ((-17179878401L & num4) != 0L)
							{
								this.jjCheckNAddStates(65, 67);
							}
							break;
						case 7u:
							if (this.curChar == '"' && num3 > 24)
							{
								num3 = 24;
							}
							break;
						case 9u:
							if ((566935683072L & num4) != 0L)
							{
								this.jjCheckNAddStates(65, 67);
							}
							break;
						case 10u:
							if ((71776119061217280L & num4) != 0L)
							{
								this.jjCheckNAddStates(68, 71);
							}
							break;
						case 11u:
							if ((71776119061217280L & num4) != 0L)
							{
								this.jjCheckNAddStates(65, 67);
							}
							break;
						case 12u:
							if ((4222124650659840L & num4) != 0L)
							{
								this.jjstateSet[this.jjnewStateCnt++] = 13u;
							}
							break;
						case 13u:
							if ((71776119061217280L & num4) != 0L)
							{
								this.jjCheckNAdd(11);
							}
							break;
						case 14u:
							if (this.curChar == ' ')
							{
								this.jjAddStates(72, 73);
							}
							break;
						case 15u:
							if (this.curChar == '\n')
							{
								this.jjCheckNAddStates(65, 67);
							}
							break;
						case 16u:
							if (this.curChar == '\'')
							{
								this.jjCheckNAddStates(62, 64);
							}
							break;
						case 17u:
							if ((-549755823105L & num4) != 0L)
							{
								this.jjCheckNAddStates(62, 64);
							}
							break;
						case 19u:
							if (this.curChar == ' ')
							{
								this.jjAddStates(13, 14);
							}
							break;
						case 20u:
							if (this.curChar == '\n')
							{
								this.jjCheckNAddStates(62, 64);
							}
							break;
						case 21u:
							if (this.curChar == '\'' && num3 > 24)
							{
								num3 = 24;
							}
							break;
						case 22u:
							if (this.curChar == '-')
							{
								this.jjCheckNAdd(23);
							}
							break;
						case 23u:
							if ((287948901175001088L & num4) != 0L)
							{
								if (num3 > 49)
								{
									num3 = 49;
								}
								this.jjCheckNAdd(23);
							}
							break;
						case 25u:
							if ((287984085547089920L & num4) != 0L)
							{
								if (num3 > 56)
								{
									num3 = 56;
								}
								this.jjstateSet[this.jjnewStateCnt++] = 25u;
							}
							break;
						case 26u:
							if (this.curChar == '.')
							{
								this.jjstateSet[this.jjnewStateCnt++] = 27u;
							}
							break;
						case 30u:
							if (this.curChar == '$' && num3 > 10)
							{
								num3 = 10;
							}
							break;
						case 32u:
							if (this.curChar == '$')
							{
								this.jjCheckNAddTwoStates(33, 34);
							}
							break;
						case 34u:
							if (this.curChar == '!' && num3 > 11)
							{
								num3 = 11;
							}
							break;
						case 35u:
							if (this.curChar == '$')
							{
								if (num3 > 10)
								{
									num3 = 10;
								}
								this.jjCheckNAddTwoStates(33, 34);
							}
							break;
						}
						IL_70D:
						if (num2 == num)
						{
							break;
						}
						continue;
						goto IL_70D;
					}
				}
				else if (this.curChar < '\u0080')
				{
					long num4 = 1L << (int)this.curChar;
					while (true)
					{
						uint num5 = this.jjstateSet[--num2];
						switch (num5)
						{
						case 1u:
							if (num3 > 13)
							{
								num3 = 13;
							}
							break;
						case 2u:
						case 4u:
						case 5u:
						case 7u:
							break;
						case 3u:
							if ((576460745995190270L & num4) != 0L)
							{
								if (num3 > 56)
								{
									num3 = 56;
								}
								this.jjCheckNAdd(25);
							}
							else if (this.curChar == '\\')
							{
								this.jjCheckNAddStates(74, 77);
							}
							break;
						case 6u:
							if ((-268435457L & num4) != 0L)
							{
								this.jjCheckNAddStates(65, 67);
							}
							break;
						case 8u:
							if (this.curChar == '\\')
							{
								this.jjAddStates(78, 82);
							}
							break;
						case 9u:
							if ((5700160604602368L & num4) != 0L)
							{
								this.jjCheckNAddStates(65, 67);
							}
							break;
						default:
							switch (num5)
							{
							case 17u:
								this.jjAddStates(62, 64);
								break;
							case 18u:
								if (this.curChar == '\\')
								{
									this.jjAddStates(13, 14);
								}
								break;
							case 24u:
							case 25u:
								if ((576460745995190270L & num4) != 0L)
								{
									if (num3 > 56)
									{
										num3 = 56;
									}
									this.jjCheckNAdd(25);
								}
								break;
							case 27u:
								if ((576460743847706622L & num4) != 0L && num3 > 57)
								{
									num3 = 57;
								}
								break;
							case 28u:
								if (this.curChar == '\\')
								{
									this.jjCheckNAddStates(74, 77);
								}
								break;
							case 29u:
								if (this.curChar == '\\')
								{
									this.jjCheckNAddTwoStates(29, 30);
								}
								break;
							case 31u:
								if (this.curChar == '\\')
								{
									this.jjCheckNAddTwoStates(31, 32);
								}
								break;
							case 33u:
								if (this.curChar == '\\')
								{
									this.jjAddStates(83, 84);
								}
								break;
							}
							break;
						}
						IL_9DA:
						if (num2 == num)
						{
							break;
						}
						continue;
						goto IL_9DA;
					}
				}
				else
				{
					int num6 = (int)(this.curChar >> 8);
					int i = num6 >> 6;
					long l = 1L << num6;
					int i2 = (int)((this.curChar & 'ÿ') >> 6);
					long l2 = 1L << (int)this.curChar;
					do
					{
						uint num5 = this.jjstateSet[--num2];
						if (num5 != 1u)
						{
							if (num5 != 6u)
							{
								if (num5 == 17u)
								{
									if (ParserTokenManager.jjCanMove_0(num6, i, i2, l, l2))
									{
										this.jjAddStates(62, 64);
									}
								}
							}
							else if (ParserTokenManager.jjCanMove_0(num6, i, i2, l, l2))
							{
								this.jjAddStates(65, 67);
							}
						}
						else if (ParserTokenManager.jjCanMove_0(num6, i, i2, l, l2) && num3 > 13)
						{
							num3 = 13;
						}
					}
					while (num2 != num);
				}
				if (num3 != 2147483647)
				{
					this.jjmatchedKind = num3;
					this.jjmatchedPos = curPos;
					num3 = 2147483647;
				}
				curPos++;
				if ((num2 = this.jjnewStateCnt) == (num = 36 - (this.jjnewStateCnt = num)))
				{
					break;
				}
				try
				{
					this.curChar = this.input_stream.ReadChar();
				}
				catch (IOException var_11_B42)
				{
					result = curPos;
					return result;
				}
			}
			result = curPos;
			return result;
		}

		private int jjStopStringLiteralDfa_2(int pos, long active0)
		{
			int result;
			switch (pos)
			{
			case 0:
				if ((active0 & 53248L) != 0L)
				{
					result = 2;
				}
				else if ((active0 & 100663296L) != 0L)
				{
					this.jjmatchedKind = 56;
					result = 5;
				}
				else
				{
					result = -1;
				}
				break;
			case 1:
				if ((active0 & 16384L) != 0L)
				{
					result = 0;
				}
				else if ((active0 & 100663296L) != 0L)
				{
					this.jjmatchedKind = 56;
					this.jjmatchedPos = 1;
					result = 5;
				}
				else
				{
					result = -1;
				}
				break;
			case 2:
				if ((active0 & 100663296L) != 0L)
				{
					this.jjmatchedKind = 56;
					this.jjmatchedPos = 2;
					result = 5;
				}
				else
				{
					result = -1;
				}
				break;
			case 3:
				if ((active0 & 67108864L) != 0L)
				{
					this.jjmatchedKind = 56;
					this.jjmatchedPos = 3;
					result = 5;
				}
				else if ((active0 & 33554432L) != 0L)
				{
					result = 5;
				}
				else
				{
					result = -1;
				}
				break;
			default:
				result = -1;
				break;
			}
			return result;
		}

		private int jjStartNfa_2(int pos, long active0)
		{
			return this.jjMoveNfa_2(this.jjStopStringLiteralDfa_2(pos, active0), pos + 1);
		}

		private int jjStartNfaWithStates_2(int pos, int kind, int state)
		{
			this.jjmatchedKind = kind;
			this.jjmatchedPos = pos;
			int result;
			try
			{
				this.curChar = this.input_stream.ReadChar();
			}
			catch (IOException var_0_24)
			{
				result = pos + 1;
				return result;
			}
			result = this.jjMoveNfa_2(state, pos + 1);
			return result;
		}

		private int jjMoveStringLiteralDfa0_2()
		{
			char c = this.curChar;
			int result;
			if (c <= '(')
			{
				if (c == '#')
				{
					this.jjmatchedKind = 15;
					result = this.jjMoveStringLiteralDfa1_2(20480L);
					return result;
				}
				if (c == '(')
				{
					result = this.jjStopAtPos(0, 5);
					return result;
				}
			}
			else
			{
				if (c == 'f')
				{
					result = this.jjMoveStringLiteralDfa1_2(67108864L);
					return result;
				}
				if (c == 't')
				{
					result = this.jjMoveStringLiteralDfa1_2(33554432L);
					return result;
				}
				switch (c)
				{
				case '{':
					result = this.jjStopAtPos(0, 58);
					return result;
				case '}':
					result = this.jjStopAtPos(0, 59);
					return result;
				}
			}
			result = this.jjMoveNfa_2(3, 0);
			return result;
		}

		private int jjMoveStringLiteralDfa1_2(long active0)
		{
			int result;
			try
			{
				this.curChar = this.input_stream.ReadChar();
			}
			catch (IOException var_0_16)
			{
				this.jjStopStringLiteralDfa_2(0, active0);
				result = 1;
				return result;
			}
			char c = this.curChar;
			if (c <= '*')
			{
				if (c != '#')
				{
					if (c == '*')
					{
						if ((active0 & 16384L) != 0L)
						{
							result = this.jjStartNfaWithStates_2(1, 14, 0);
							return result;
						}
					}
				}
				else if ((active0 & 4096L) != 0L)
				{
					result = this.jjStopAtPos(1, 12);
					return result;
				}
			}
			else
			{
				if (c == 'a')
				{
					result = this.jjMoveStringLiteralDfa2_2(active0, 67108864L);
					return result;
				}
				if (c == 'r')
				{
					result = this.jjMoveStringLiteralDfa2_2(active0, 33554432L);
					return result;
				}
			}
			result = this.jjStartNfa_2(0, active0);
			return result;
		}

		private int jjMoveStringLiteralDfa2_2(long old0, long active0)
		{
			int result;
			if ((active0 &= old0) == 0L)
			{
				result = this.jjStartNfa_2(0, old0);
			}
			else
			{
				try
				{
					this.curChar = this.input_stream.ReadChar();
				}
				catch (IOException var_0_32)
				{
					this.jjStopStringLiteralDfa_2(1, active0);
					result = 2;
					return result;
				}
				char c = this.curChar;
				if (c != 'l')
				{
					if (c != 'u')
					{
						result = this.jjStartNfa_2(1, active0);
					}
					else
					{
						result = this.jjMoveStringLiteralDfa3_2(active0, 33554432L);
					}
				}
				else
				{
					result = this.jjMoveStringLiteralDfa3_2(active0, 67108864L);
				}
			}
			return result;
		}

		private int jjMoveStringLiteralDfa3_2(long old0, long active0)
		{
			int result;
			if ((active0 &= old0) == 0L)
			{
				result = this.jjStartNfa_2(1, old0);
			}
			else
			{
				try
				{
					this.curChar = this.input_stream.ReadChar();
				}
				catch (IOException var_0_32)
				{
					this.jjStopStringLiteralDfa_2(2, active0);
					result = 3;
					return result;
				}
				char c = this.curChar;
				if (c != 'e')
				{
					if (c == 's')
					{
						result = this.jjMoveStringLiteralDfa4_2(active0, 67108864L);
						return result;
					}
				}
				else if ((active0 & 33554432L) != 0L)
				{
					result = this.jjStartNfaWithStates_2(3, 25, 5);
					return result;
				}
				result = this.jjStartNfa_2(2, active0);
			}
			return result;
		}

		private int jjMoveStringLiteralDfa4_2(long old0, long active0)
		{
			int result;
			if ((active0 &= old0) == 0L)
			{
				result = this.jjStartNfa_2(2, old0);
			}
			else
			{
				try
				{
					this.curChar = this.input_stream.ReadChar();
				}
				catch (IOException var_0_32)
				{
					this.jjStopStringLiteralDfa_2(3, active0);
					result = 4;
					return result;
				}
				char c = this.curChar;
				if (c == 'e')
				{
					if ((active0 & 67108864L) != 0L)
					{
						result = this.jjStartNfaWithStates_2(4, 26, 5);
						return result;
					}
				}
				result = this.jjStartNfa_2(3, active0);
			}
			return result;
		}

		private int jjMoveNfa_2(int startState, int curPos)
		{
			int num = 0;
			this.jjnewStateCnt = 16;
			int num2 = 1;
			this.jjstateSet[0] = (uint)startState;
			int num3 = 2147483647;
			int result;
			while (true)
			{
				if ((this.jjround += 1u) == 2147483647u)
				{
					this.ReInitRounds();
				}
				if (this.curChar < '@')
				{
					long num4 = 1L << (int)this.curChar;
					while (true)
					{
						switch (this.jjstateSet[--num2])
						{
						case 0u:
							if (this.curChar == '*')
							{
								this.jjstateSet[this.jjnewStateCnt++] = 1u;
							}
							break;
						case 1u:
							if ((-34359738369L & num4) != 0L && num3 > 13)
							{
								num3 = 13;
							}
							break;
						case 2u:
							if (this.curChar == '*')
							{
								this.jjstateSet[this.jjnewStateCnt++] = 0u;
							}
							break;
						case 3u:
							if (this.curChar == '$')
							{
								if (num3 > 10)
								{
									num3 = 10;
								}
								this.jjCheckNAddTwoStates(13, 14);
							}
							else if (this.curChar == '.')
							{
								this.jjstateSet[this.jjnewStateCnt++] = 7u;
							}
							else if (this.curChar == '#')
							{
								this.jjstateSet[this.jjnewStateCnt++] = 2u;
							}
							break;
						case 5u:
							if ((287984085547089920L & num4) != 0L)
							{
								if (num3 > 56)
								{
									num3 = 56;
								}
								this.jjstateSet[this.jjnewStateCnt++] = 5u;
							}
							break;
						case 6u:
							if (this.curChar == '.')
							{
								this.jjstateSet[this.jjnewStateCnt++] = 7u;
							}
							break;
						case 10u:
							if (this.curChar == '$' && num3 > 10)
							{
								num3 = 10;
							}
							break;
						case 12u:
							if (this.curChar == '$')
							{
								this.jjCheckNAddTwoStates(13, 14);
							}
							break;
						case 14u:
							if (this.curChar == '!' && num3 > 11)
							{
								num3 = 11;
							}
							break;
						case 15u:
							if (this.curChar == '$')
							{
								if (num3 > 10)
								{
									num3 = 10;
								}
								this.jjCheckNAddTwoStates(13, 14);
							}
							break;
						}
						IL_312:
						if (num2 == num)
						{
							break;
						}
						continue;
						goto IL_312;
					}
				}
				else if (this.curChar < '\u0080')
				{
					long num4 = 1L << (int)this.curChar;
					while (true)
					{
						switch (this.jjstateSet[--num2])
						{
						case 1u:
							if (num3 > 13)
							{
								num3 = 13;
							}
							break;
						case 3u:
							if ((576460745995190270L & num4) != 0L)
							{
								if (num3 > 56)
								{
									num3 = 56;
								}
								this.jjCheckNAdd(5);
							}
							else if (this.curChar == '\\')
							{
								this.jjCheckNAddStates(40, 43);
							}
							break;
						case 4u:
						case 5u:
							if ((576460745995190270L & num4) != 0L)
							{
								if (num3 > 56)
								{
									num3 = 56;
								}
								this.jjCheckNAdd(5);
							}
							break;
						case 7u:
							if ((576460743847706622L & num4) != 0L && num3 > 57)
							{
								num3 = 57;
							}
							break;
						case 8u:
							if (this.curChar == '\\')
							{
								this.jjCheckNAddStates(40, 43);
							}
							break;
						case 9u:
							if (this.curChar == '\\')
							{
								this.jjCheckNAddTwoStates(9, 10);
							}
							break;
						case 11u:
							if (this.curChar == '\\')
							{
								this.jjCheckNAddTwoStates(11, 12);
							}
							break;
						case 13u:
							if (this.curChar == '\\')
							{
								this.jjAddStates(46, 47);
							}
							break;
						}
						IL_500:
						if (num2 == num)
						{
							break;
						}
						continue;
						goto IL_500;
					}
				}
				else
				{
					int num5 = (int)(this.curChar >> 8);
					int i = num5 >> 6;
					long l = 1L << num5;
					int i2 = (int)((this.curChar & 'ÿ') >> 6);
					long l2 = 1L << (int)this.curChar;
					do
					{
						uint num6 = this.jjstateSet[--num2];
						if (num6 == 1u)
						{
							if (ParserTokenManager.jjCanMove_0(num5, i, i2, l, l2) && num3 > 13)
							{
								num3 = 13;
							}
						}
					}
					while (num2 != num);
				}
				if (num3 != 2147483647)
				{
					this.jjmatchedKind = num3;
					this.jjmatchedPos = curPos;
					num3 = 2147483647;
				}
				curPos++;
				if ((num2 = this.jjnewStateCnt) == (num = 16 - (this.jjnewStateCnt = num)))
				{
					break;
				}
				try
				{
					this.curChar = this.input_stream.ReadChar();
				}
				catch (IOException var_11_610)
				{
					result = curPos;
					return result;
				}
			}
			result = curPos;
			return result;
		}

		private static bool jjCanMove_0(int hiByte, int i1, int i2, long l1, long l2)
		{
			bool result;
			if (hiByte != 0)
			{
				result = ((ParserTokenManager.jjbitVec0[i1] & (ulong)l1) != 0uL);
			}
			else
			{
				result = ((ParserTokenManager.jjbitVec2[i2] & (ulong)l2) != 0uL);
			}
			return result;
		}

		public ParserTokenManager(ICharStream stream)
		{
			this.input_stream = stream;
		}

		public ParserTokenManager(ICharStream stream, int lexState) : this(stream)
		{
			this.SwitchTo(lexState);
		}

		public void ReInit(ICharStream stream)
		{
			this.jjmatchedPos = (this.jjnewStateCnt = 0);
			this.curLexState = this.defaultLexState;
			this.input_stream = stream;
			this.ReInitRounds();
		}

		private void ReInitRounds()
		{
			this.jjround = 2147483649u;
			int num = 42;
			while (num-- > 0)
			{
				this.jjrounds[num] = 2147483648u;
			}
		}

		public void ReInit(ICharStream stream, int lexState)
		{
			this.ReInit(stream);
			this.SwitchTo(lexState);
		}

		public void SwitchTo(int lexState)
		{
			if (lexState >= 9 || lexState < 0)
			{
				throw new TokenMgrError("Error: Ignoring invalid lexical state : " + lexState + ". State unchanged.", 2);
			}
			this.curLexState = lexState;
		}

		private Token jjFillToken()
		{
			Token token = Token.NewToken(this.jjmatchedKind);
			token.Kind = this.jjmatchedKind;
			string text = ParserTokenManager.jjstrLiteralImages[this.jjmatchedKind];
			token.Image = ((text == null) ? this.input_stream.GetImage() : text);
			token.BeginLine = this.input_stream.BeginLine;
			token.BeginColumn = this.input_stream.BeginColumn;
			token.EndLine = this.input_stream.EndLine;
			token.EndColumn = this.input_stream.EndColumn;
			return token;
		}

		private void SkipLexicalActions(Token matchedToken)
		{
			switch (this.jjmatchedKind)
			{
			case 60:
				if (this.image == null)
				{
					this.image = new StringBuilder(new string(this.input_stream.GetSuffix(this.jjimageLen + (this.lengthOfMatch = this.jjmatchedPos + 1))));
				}
				else
				{
					this.image.Append(this.input_stream.GetSuffix(this.jjimageLen + (this.lengthOfMatch = this.jjmatchedPos + 1)));
				}
				this.input_stream.Backup(1);
				this.inReference = false;
				if (this.debugPrint)
				{
					Console.Out.Write("REF_TERM :");
				}
				this.StateStackPop();
				break;
			case 61:
				if (this.image == null)
				{
					this.image = new StringBuilder(new string(this.input_stream.GetSuffix(this.jjimageLen + (this.lengthOfMatch = this.jjmatchedPos + 1))));
				}
				else
				{
					this.image.Append(this.input_stream.GetSuffix(this.jjimageLen + (this.lengthOfMatch = this.jjmatchedPos + 1)));
				}
				if (this.debugPrint)
				{
					Console.Out.Write("DIRECTIVE_TERM :");
				}
				this.input_stream.Backup(1);
				this.inDirective = false;
				this.StateStackPop();
				break;
			}
		}

		private void MoreLexicalActions()
		{
			this.jjimageLen += (this.lengthOfMatch = this.jjmatchedPos + 1);
			switch (this.jjmatchedKind)
			{
			case 10:
				if (this.image == null)
				{
					this.image = new StringBuilder(new string(this.input_stream.GetSuffix(this.jjimageLen)));
				}
				else
				{
					this.image.Append(this.input_stream.GetSuffix(this.jjimageLen));
				}
				this.jjimageLen = 0;
				if (!this.inComment)
				{
					if (this.curLexState == 5)
					{
						this.inReference = false;
						this.StateStackPop();
					}
					this.inReference = true;
					if (this.debugPrint)
					{
						Console.Out.Write("$  : going to " + 5);
					}
					this.StateStackPush();
					this.SwitchTo(5);
				}
				break;
			case 11:
				if (this.image == null)
				{
					this.image = new StringBuilder(new string(this.input_stream.GetSuffix(this.jjimageLen)));
				}
				else
				{
					this.image.Append(this.input_stream.GetSuffix(this.jjimageLen));
				}
				this.jjimageLen = 0;
				if (!this.inComment)
				{
					if (this.curLexState == 5)
					{
						this.inReference = false;
						this.StateStackPop();
					}
					this.inReference = true;
					if (this.debugPrint)
					{
						Console.Out.Write("$!  : going to " + 5);
					}
					this.StateStackPush();
					this.SwitchTo(5);
				}
				break;
			case 12:
				if (this.image == null)
				{
					this.image = new StringBuilder(new string(this.input_stream.GetSuffix(this.jjimageLen)));
				}
				else
				{
					this.image.Append(this.input_stream.GetSuffix(this.jjimageLen));
				}
				this.jjimageLen = 0;
				if (!this.inComment)
				{
					if (this.curLexState == 5)
					{
						this.inReference = false;
						this.StateStackPop();
					}
					this.inComment = true;
					this.StateStackPush();
					this.SwitchTo(8);
				}
				break;
			case 13:
				if (this.image == null)
				{
					this.image = new StringBuilder(new string(this.input_stream.GetSuffix(this.jjimageLen)));
				}
				else
				{
					this.image.Append(this.input_stream.GetSuffix(this.jjimageLen));
				}
				this.jjimageLen = 0;
				this.input_stream.Backup(1);
				this.inComment = true;
				this.StateStackPush();
				this.SwitchTo(7);
				break;
			case 14:
				if (this.image == null)
				{
					this.image = new StringBuilder(new string(this.input_stream.GetSuffix(this.jjimageLen)));
				}
				else
				{
					this.image.Append(this.input_stream.GetSuffix(this.jjimageLen));
				}
				this.jjimageLen = 0;
				this.inComment = true;
				this.StateStackPush();
				this.SwitchTo(6);
				break;
			case 15:
				if (this.image == null)
				{
					this.image = new StringBuilder(new string(this.input_stream.GetSuffix(this.jjimageLen)));
				}
				else
				{
					this.image.Append(this.input_stream.GetSuffix(this.jjimageLen));
				}
				this.jjimageLen = 0;
				if (!this.inComment)
				{
					if (this.curLexState == 5 || this.curLexState == 2)
					{
						this.inReference = false;
						this.StateStackPop();
					}
					this.inDirective = true;
					if (this.debugPrint)
					{
						Console.Out.Write("# :  going to " + 0);
					}
					this.StateStackPush();
					this.SwitchTo(4);
				}
				break;
			}
		}

		private void TokenLexicalActions(Token matchedToken)
		{
			int num = this.jjmatchedKind;
			if (num <= 27)
			{
				switch (num)
				{
				case 5:
					if (this.image == null)
					{
						this.image = new StringBuilder(new string(this.input_stream.GetSuffix(this.jjimageLen + (this.lengthOfMatch = this.jjmatchedPos + 1))));
					}
					else
					{
						this.image.Append(this.input_stream.GetSuffix(this.jjimageLen + (this.lengthOfMatch = this.jjmatchedPos + 1)));
					}
					if (!this.inComment)
					{
						this.lparen++;
					}
					if (this.curLexState == 2)
					{
						this.SwitchTo(1);
					}
					break;
				case 6:
					if (this.image == null)
					{
						this.image = new StringBuilder(new string(this.input_stream.GetSuffix(this.jjimageLen + (this.lengthOfMatch = this.jjmatchedPos + 1))));
					}
					else
					{
						this.image.Append(this.input_stream.GetSuffix(this.jjimageLen + (this.lengthOfMatch = this.jjmatchedPos + 1)));
					}
					this.RPARENHandler();
					break;
				case 7:
					if (this.image == null)
					{
						this.image = new StringBuilder(new string(this.input_stream.GetSuffix(this.jjimageLen + (this.lengthOfMatch = this.jjmatchedPos + 1))));
					}
					else
					{
						this.image.Append(this.input_stream.GetSuffix(this.jjimageLen + (this.lengthOfMatch = this.jjmatchedPos + 1)));
					}
					this.SwitchTo(5);
					break;
				case 8:
					break;
				case 9:
					if (this.image == null)
					{
						this.image = new StringBuilder(new string(this.input_stream.GetSuffix(this.jjimageLen + (this.lengthOfMatch = this.jjmatchedPos + 1))));
					}
					else
					{
						this.image.Append(this.input_stream.GetSuffix(this.jjimageLen + (this.lengthOfMatch = this.jjmatchedPos + 1)));
					}
					if (!this.inComment)
					{
						this.inDirective = true;
						if (this.debugPrint)
						{
							Console.Out.Write("#set :  going to " + 0);
						}
						this.StateStackPush();
						this.inSet = true;
						this.SwitchTo(0);
					}
					break;
				default:
					switch (num)
					{
					case 19:
						if (this.image == null)
						{
							this.image = new StringBuilder(new string(this.input_stream.GetSuffix(this.jjimageLen + (this.lengthOfMatch = this.jjmatchedPos + 1))));
						}
						else
						{
							this.image.Append(this.input_stream.GetSuffix(this.jjimageLen + (this.lengthOfMatch = this.jjmatchedPos + 1)));
						}
						this.inComment = false;
						this.StateStackPop();
						break;
					case 20:
						if (this.image == null)
						{
							this.image = new StringBuilder(new string(this.input_stream.GetSuffix(this.jjimageLen + (this.lengthOfMatch = this.jjmatchedPos + 1))));
						}
						else
						{
							this.image.Append(this.input_stream.GetSuffix(this.jjimageLen + (this.lengthOfMatch = this.jjmatchedPos + 1)));
						}
						this.inComment = false;
						this.StateStackPop();
						break;
					case 21:
						if (this.image == null)
						{
							this.image = new StringBuilder(new string(this.input_stream.GetSuffix(this.jjimageLen + (this.lengthOfMatch = this.jjmatchedPos + 1))));
						}
						else
						{
							this.image.Append(this.input_stream.GetSuffix(this.jjimageLen + (this.lengthOfMatch = this.jjmatchedPos + 1)));
						}
						this.inComment = false;
						this.StateStackPop();
						break;
					case 24:
						if (this.image == null)
						{
							this.image = new StringBuilder(new string(this.input_stream.GetSuffix(this.jjimageLen + (this.lengthOfMatch = this.jjmatchedPos + 1))));
						}
						else
						{
							this.image.Append(this.input_stream.GetSuffix(this.jjimageLen + (this.lengthOfMatch = this.jjmatchedPos + 1)));
						}
						if (this.curLexState == 0 && !this.inSet && this.lparen == 0)
						{
							this.StateStackPop();
						}
						break;
					case 27:
						if (this.image == null)
						{
							this.image = new StringBuilder(new string(this.input_stream.GetSuffix(this.jjimageLen + (this.lengthOfMatch = this.jjmatchedPos + 1))));
						}
						else
						{
							this.image.Append(this.input_stream.GetSuffix(this.jjimageLen + (this.lengthOfMatch = this.jjmatchedPos + 1)));
						}
						if (this.debugPrint)
						{
							Console.Out.WriteLine(" NEWLINE :");
						}
						this.StateStackPop();
						if (this.inSet)
						{
							this.inSet = false;
						}
						if (this.inDirective)
						{
							this.inDirective = false;
						}
						break;
					}
					break;
				}
			}
			else
			{
				switch (num)
				{
				case 43:
					if (this.image == null)
					{
						this.image = new StringBuilder(new string(this.input_stream.GetSuffix(this.jjimageLen + (this.lengthOfMatch = this.jjmatchedPos + 1))));
					}
					else
					{
						this.image.Append(this.input_stream.GetSuffix(this.jjimageLen + (this.lengthOfMatch = this.jjmatchedPos + 1)));
					}
					this.inDirective = false;
					this.StateStackPop();
					break;
				case 44:
					if (this.image == null)
					{
						this.image = new StringBuilder(new string(this.input_stream.GetSuffix(this.jjimageLen + (this.lengthOfMatch = this.jjmatchedPos + 1))));
					}
					else
					{
						this.image.Append(this.input_stream.GetSuffix(this.jjimageLen + (this.lengthOfMatch = this.jjmatchedPos + 1)));
					}
					this.SwitchTo(0);
					break;
				case 45:
					if (this.image == null)
					{
						this.image = new StringBuilder(new string(this.input_stream.GetSuffix(this.jjimageLen + (this.lengthOfMatch = this.jjmatchedPos + 1))));
					}
					else
					{
						this.image.Append(this.input_stream.GetSuffix(this.jjimageLen + (this.lengthOfMatch = this.jjmatchedPos + 1)));
					}
					this.SwitchTo(0);
					break;
				case 46:
					if (this.image == null)
					{
						this.image = new StringBuilder(new string(this.input_stream.GetSuffix(this.jjimageLen + (this.lengthOfMatch = this.jjmatchedPos + 1))));
					}
					else
					{
						this.image.Append(this.input_stream.GetSuffix(this.jjimageLen + (this.lengthOfMatch = this.jjmatchedPos + 1)));
					}
					this.inDirective = false;
					this.StateStackPop();
					break;
				case 47:
					if (this.image == null)
					{
						this.image = new StringBuilder(new string(this.input_stream.GetSuffix(this.jjimageLen + (this.lengthOfMatch = this.jjmatchedPos + 1))));
					}
					else
					{
						this.image.Append(this.input_stream.GetSuffix(this.jjimageLen + (this.lengthOfMatch = this.jjmatchedPos + 1)));
					}
					matchedToken.Kind = 0;
					this.fileDepth = 0;
					break;
				case 48:
					break;
				case 49:
					if (this.image == null)
					{
						this.image = new StringBuilder(new string(this.input_stream.GetSuffix(this.jjimageLen + (this.lengthOfMatch = this.jjmatchedPos + 1))));
					}
					else
					{
						this.image.Append(this.input_stream.GetSuffix(this.jjimageLen + (this.lengthOfMatch = this.jjmatchedPos + 1)));
					}
					if (this.lparen == 0 && !this.inSet && this.curLexState != 1)
					{
						this.StateStackPop();
					}
					break;
				default:
					switch (num)
					{
					case 57:
						if (this.image == null)
						{
							this.image = new StringBuilder(new string(this.input_stream.GetSuffix(this.jjimageLen + (this.lengthOfMatch = this.jjmatchedPos + 1))));
						}
						else
						{
							this.image.Append(this.input_stream.GetSuffix(this.jjimageLen + (this.lengthOfMatch = this.jjmatchedPos + 1)));
						}
						this.input_stream.Backup(1);
						matchedToken.Image = ".";
						if (this.debugPrint)
						{
							Console.Out.Write("DOT : switching to " + 2);
						}
						this.SwitchTo(2);
						break;
					case 59:
						if (this.image == null)
						{
							this.image = new StringBuilder(new string(this.input_stream.GetSuffix(this.jjimageLen + (this.lengthOfMatch = this.jjmatchedPos + 1))));
						}
						else
						{
							this.image.Append(this.input_stream.GetSuffix(this.jjimageLen + (this.lengthOfMatch = this.jjmatchedPos + 1)));
						}
						this.StateStackPop();
						break;
					}
					break;
				}
			}
		}

		static ParserTokenManager()
		{
			// 注意: 此类型已标记为 'beforefieldinit'.
			string[] array = new string[62];
			ParserTokenManager.jjstrLiteralImages = array;
			ParserTokenManager.lexStateNames = new string[]
			{
				"DIRECTIVE",
				"REFMOD2",
				"REFMODIFIER",
				"DEFAULT",
				"PRE_DIRECTIVE",
				"REFERENCE",
				"IN_MULTI_LINE_COMMENT",
				"IN_FORMAL_COMMENT",
				"IN_SINGLE_LINE_COMMENT"
			};
			ParserTokenManager.jjnewLexState = new int[]
			{
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1
			};
			ParserTokenManager.jjtoToken = new long[]
			{
				1086211935122162687L
			};
			ParserTokenManager.jjtoSkip = new long[]
			{
				3458764513820540928L
			};
			ParserTokenManager.jjtoSpecial = new long[]
			{
				3458764513820540928L
			};
			ParserTokenManager.jjtoMore = new long[]
			{
				4258816L
			};
		}
	}
}
