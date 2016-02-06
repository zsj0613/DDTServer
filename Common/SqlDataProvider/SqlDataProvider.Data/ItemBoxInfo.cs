using System;
namespace SqlDataProvider.Data
{
	public class ItemBoxInfo : DataObject
	{
		public int Id
		{
			get;
			set;
		}
		public int DataId
		{
			get;
			set;
		}
		public int TemplateId
		{
			get;
			set;
		}
		public bool IsSelect
		{
			get;
			set;
		}
		public bool IsBind
		{
			get;
			set;
		}
		public int ItemValid
		{
			get;
			set;
		}
		public int ItemCount
		{
			get;
			set;
		}
		public int StrengthenLevel
		{
			get;
			set;
		}
		public int AttackCompose
		{
			get;
			set;
		}
		public int DefendCompose
		{
			get;
			set;
		}
		public int AgilityCompose
		{
			get;
			set;
		}
		public int LuckCompose
		{
			get;
			set;
		}
		public int Random
		{
			get;
			set;
		}
		public bool IsTips
		{
			get;
			set;
		}
		public bool IsLogs
		{
			get;
			set;
		}
	}
}
