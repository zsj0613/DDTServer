using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
namespace Bussiness.CenterService
{
	[GeneratedCode("System.Runtime.Serialization", "3.0.0.0"), DebuggerStepThrough, DataContract(Name = "ServerData", Namespace = "http://schemas.datacontract.org/2004/07/Center.Server")]
	[Serializable]
	public class ServerData : IExtensibleDataObject, INotifyPropertyChanged
	{
		[NonSerialized]
		private ExtensionDataObject extensionDataField;
		[OptionalField]
		private int IdField;
		[OptionalField]
		private string IpField;
		[OptionalField]
		private int LowestLevelField;
		[OptionalField]
		private int MustLevelField;
		[OptionalField]
		private string NameField;
		[OptionalField]
		private int OnlineField;
		[OptionalField]
		private int PortField;
		[OptionalField]
		private int StateField;
		public event PropertyChangedEventHandler PropertyChanged;
        //{
        //    [MethodImpl(MethodImplOptions.Synchronized)]
        //    add
        //    {
        //        this.PropertyChanged = (PropertyChangedEventHandler)Delegate.Combine(this.PropertyChanged, value);
        //    }
        //    [MethodImpl(MethodImplOptions.Synchronized)]
        //    remove
        //    {
        //        this.PropertyChanged = (PropertyChangedEventHandler)Delegate.Remove(this.PropertyChanged, value);
        //    }
        //}
		[Browsable(false)]
		public ExtensionDataObject ExtensionData
		{
			get
			{
				return this.extensionDataField;
			}
			set
			{
				this.extensionDataField = value;
			}
		}
		[DataMember]
		public int Id
		{
			get
			{
				return this.IdField;
			}
			set
			{
				if (!this.IdField.Equals(value))
				{
					this.IdField = value;
					this.RaisePropertyChanged("Id");
				}
			}
		}
		[DataMember]
		public string Ip
		{
			get
			{
				return this.IpField;
			}
			set
			{
				if (!object.ReferenceEquals(this.IpField, value))
				{
					this.IpField = value;
					this.RaisePropertyChanged("Ip");
				}
			}
		}
		[DataMember]
		public int LowestLevel
		{
			get
			{
				return this.LowestLevelField;
			}
			set
			{
				if (!this.LowestLevelField.Equals(value))
				{
					this.LowestLevelField = value;
					this.RaisePropertyChanged("LowestLevel");
				}
			}
		}
		[DataMember]
		public int MustLevel
		{
			get
			{
				return this.MustLevelField;
			}
			set
			{
				if (!this.MustLevelField.Equals(value))
				{
					this.MustLevelField = value;
					this.RaisePropertyChanged("MustLevel");
				}
			}
		}
		[DataMember]
		public string Name
		{
			get
			{
				return this.NameField;
			}
			set
			{
				if (!object.ReferenceEquals(this.NameField, value))
				{
					this.NameField = value;
					this.RaisePropertyChanged("Name");
				}
			}
		}
		[DataMember]
		public int Online
		{
			get
			{
				return this.OnlineField;
			}
			set
			{
				if (!this.OnlineField.Equals(value))
				{
					this.OnlineField = value;
					this.RaisePropertyChanged("Online");
				}
			}
		}
		[DataMember]
		public int Port
		{
			get
			{
				return this.PortField;
			}
			set
			{
				if (!this.PortField.Equals(value))
				{
					this.PortField = value;
					this.RaisePropertyChanged("Port");
				}
			}
		}
		[DataMember]
		public int State
		{
			get
			{
				return this.StateField;
			}
			set
			{
				if (!this.StateField.Equals(value))
				{
					this.StateField = value;
					this.RaisePropertyChanged("State");
				}
			}
		}
		protected void RaisePropertyChanged(string propertyName)
		{
			PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
			if (propertyChanged != null)
			{
				propertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
}
