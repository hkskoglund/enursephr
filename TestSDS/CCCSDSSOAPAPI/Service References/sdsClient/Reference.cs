﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.3053
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CCCBrowser.SoapAPI.sdsClient {
    using System.Runtime.Serialization;
    using System;
    
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "3.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="Scope", Namespace="http://schemas.microsoft.com/sitka/2008/03/")]
    [System.SerializableAttribute()]
    public partial struct Scope : System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        private string AuthorityIdField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string ContainerIdField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string EntityIdField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private CCCBrowser.SoapAPI.sdsClient.VersionMatch VersionMatchField;
        
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true)]
        public string AuthorityId {
            get {
                return this.AuthorityIdField;
            }
            set {
                if ((object.ReferenceEquals(this.AuthorityIdField, value) != true)) {
                    this.AuthorityIdField = value;
                    this.RaisePropertyChanged("AuthorityId");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string ContainerId {
            get {
                return this.ContainerIdField;
            }
            set {
                if ((object.ReferenceEquals(this.ContainerIdField, value) != true)) {
                    this.ContainerIdField = value;
                    this.RaisePropertyChanged("ContainerId");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string EntityId {
            get {
                return this.EntityIdField;
            }
            set {
                if ((object.ReferenceEquals(this.EntityIdField, value) != true)) {
                    this.EntityIdField = value;
                    this.RaisePropertyChanged("EntityId");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false)]
        public CCCBrowser.SoapAPI.sdsClient.VersionMatch VersionMatch {
            get {
                return this.VersionMatchField;
            }
            set {
                if ((object.ReferenceEquals(this.VersionMatchField, value) != true)) {
                    this.VersionMatchField = value;
                    this.RaisePropertyChanged("VersionMatch");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "3.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="VersionMatch", Namespace="http://schemas.microsoft.com/sitka/2008/03/")]
    [System.SerializableAttribute()]
    public partial class VersionMatch : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private CCCBrowser.SoapAPI.sdsClient.VersionMatchType MatchTypeField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private long VersionField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public CCCBrowser.SoapAPI.sdsClient.VersionMatchType MatchType {
            get {
                return this.MatchTypeField;
            }
            set {
                if ((this.MatchTypeField.Equals(value) != true)) {
                    this.MatchTypeField = value;
                    this.RaisePropertyChanged("MatchType");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public long Version {
            get {
                return this.VersionField;
            }
            set {
                if ((this.VersionField.Equals(value) != true)) {
                    this.VersionField = value;
                    this.RaisePropertyChanged("Version");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "3.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="VersionMatchType", Namespace="http://schemas.microsoft.com/sitka/2008/03/")]
    public enum VersionMatchType : int {
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        Ignore = 0,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        Match = 1,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        NotMatch = 2,
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "3.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="Entity", Namespace="http://schemas.microsoft.com/sitka/2008/03/")]
    [System.SerializableAttribute()]
    [System.Runtime.Serialization.KnownTypeAttribute(typeof(CCCBrowser.SoapAPI.sdsClient.Authority))]
    [System.Runtime.Serialization.KnownTypeAttribute(typeof(CCCBrowser.SoapAPI.sdsClient.Container))]
    public partial class Entity : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string IdField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string KindField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private System.Collections.Generic.Dictionary<string, object> PropertiesField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private long VersionField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Id {
            get {
                return this.IdField;
            }
            set {
                if ((object.ReferenceEquals(this.IdField, value) != true)) {
                    this.IdField = value;
                    this.RaisePropertyChanged("Id");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Kind {
            get {
                return this.KindField;
            }
            set {
                if ((object.ReferenceEquals(this.KindField, value) != true)) {
                    this.KindField = value;
                    this.RaisePropertyChanged("Kind");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.Collections.Generic.Dictionary<string, object> Properties {
            get {
                return this.PropertiesField;
            }
            set {
                if ((object.ReferenceEquals(this.PropertiesField, value) != true)) {
                    this.PropertiesField = value;
                    this.RaisePropertyChanged("Properties");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public long Version {
            get {
                return this.VersionField;
            }
            set {
                if ((this.VersionField.Equals(value) != true)) {
                    this.VersionField = value;
                    this.RaisePropertyChanged("Version");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "3.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="Authority", Namespace="http://schemas.microsoft.com/sitka/2008/03/")]
    [System.SerializableAttribute()]
    public partial class Authority : CCCBrowser.SoapAPI.sdsClient.Entity {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "3.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="Container", Namespace="http://schemas.microsoft.com/sitka/2008/03/")]
    [System.SerializableAttribute()]
    public partial class Container : CCCBrowser.SoapAPI.sdsClient.Entity {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "3.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="Error", Namespace="http://schemas.microsoft.com/sitka/2008/03/")]
    [System.SerializableAttribute()]
    public partial class Error : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string MessageField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private CCCBrowser.SoapAPI.sdsClient.ErrorCodes StatusCodeField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Message {
            get {
                return this.MessageField;
            }
            set {
                if ((object.ReferenceEquals(this.MessageField, value) != true)) {
                    this.MessageField = value;
                    this.RaisePropertyChanged("Message");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public CCCBrowser.SoapAPI.sdsClient.ErrorCodes StatusCode {
            get {
                return this.StatusCodeField;
            }
            set {
                if ((this.StatusCodeField.Equals(value) != true)) {
                    this.StatusCodeField = value;
                    this.RaisePropertyChanged("StatusCode");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "3.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="ErrorCodes", Namespace="http://schemas.microsoft.com/sitka/2008/03/")]
    public enum ErrorCodes : int {
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        Success = 0,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        EntityNotFound = 1001,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        EntityExists = 1002,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        InvalidEntity = 1003,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        EntityNotModified = 1004,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        InvalidContainerSpecification = 2001,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        UnableToCreateDnsRecord = 3001,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        UnableToRemoveDnsRecord = 3002,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        NoAuthoritySpecified = 3003,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        InvalidContainerQueryRequest = 4001,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        InvalidQueryDefinition = 4002,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        ServiceNotAvailable = 5001,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        OperationTimeout = 5002,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        InvalidRequest = 6001,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        InvalidScope = 6002,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        PreconditionFailed = 6003,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        ContentMatchFailed = 6004,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        SecurityError = 7001,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        InvalidUserId = 7002,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        UnknownError = 8001,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        UnexpectedError = 8002,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        ConcurrencyConflictError = 8003,
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "3.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(Namespace="http://schemas.microsoft.com/sitka/2008/03/", ConfigurationName="sdsClient.ISitkaSoapService")]
    public interface ISitkaSoapService {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://schemas.microsoft.com/sitka/2008/03/ISitkaSoapService/Create", ReplyAction="http://schemas.microsoft.com/sitka/2008/03/ISitkaSoapService/CreateResponse")]
        [System.ServiceModel.FaultContractAttribute(typeof(CCCBrowser.SoapAPI.sdsClient.Error), Action="http://schemas.microsoft.com/sitka/2008/03/ISitkaSoapService/CreateErrorFault", Name="Error")]
        CCCBrowser.SoapAPI.sdsClient.Scope Create(CCCBrowser.SoapAPI.sdsClient.Scope scope, CCCBrowser.SoapAPI.sdsClient.Entity entity);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://schemas.microsoft.com/sitka/2008/03/ISitkaSoapService/Get", ReplyAction="http://schemas.microsoft.com/sitka/2008/03/ISitkaSoapService/GetResponse")]
        [System.ServiceModel.FaultContractAttribute(typeof(CCCBrowser.SoapAPI.sdsClient.Error), Action="http://schemas.microsoft.com/sitka/2008/03/ISitkaSoapService/GetErrorFault", Name="Error")]
        CCCBrowser.SoapAPI.sdsClient.Entity Get(CCCBrowser.SoapAPI.sdsClient.Scope scope);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://schemas.microsoft.com/sitka/2008/03/ISitkaSoapService/Update", ReplyAction="http://schemas.microsoft.com/sitka/2008/03/ISitkaSoapService/UpdateResponse")]
        [System.ServiceModel.FaultContractAttribute(typeof(CCCBrowser.SoapAPI.sdsClient.Error), Action="http://schemas.microsoft.com/sitka/2008/03/ISitkaSoapService/UpdateErrorFault", Name="Error")]
        CCCBrowser.SoapAPI.sdsClient.Scope Update(CCCBrowser.SoapAPI.sdsClient.Scope scope, CCCBrowser.SoapAPI.sdsClient.Entity entity);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://schemas.microsoft.com/sitka/2008/03/ISitkaSoapService/Delete", ReplyAction="http://schemas.microsoft.com/sitka/2008/03/ISitkaSoapService/DeleteResponse")]
        [System.ServiceModel.FaultContractAttribute(typeof(CCCBrowser.SoapAPI.sdsClient.Error), Action="http://schemas.microsoft.com/sitka/2008/03/ISitkaSoapService/DeleteErrorFault", Name="Error")]
        void Delete(CCCBrowser.SoapAPI.sdsClient.Scope scope);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://schemas.microsoft.com/sitka/2008/03/ISitkaSoapService/Query", ReplyAction="http://schemas.microsoft.com/sitka/2008/03/ISitkaSoapService/QueryResponse")]
        [System.ServiceModel.FaultContractAttribute(typeof(CCCBrowser.SoapAPI.sdsClient.Error), Action="http://schemas.microsoft.com/sitka/2008/03/ISitkaSoapService/QueryErrorFault", Name="Error")]
        System.Collections.Generic.List<CCCBrowser.SoapAPI.sdsClient.Entity> Query(CCCBrowser.SoapAPI.sdsClient.Scope scope, [System.ServiceModel.MessageParameterAttribute(Name="query")] string query1);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "3.0.0.0")]
    public interface ISitkaSoapServiceChannel : CCCBrowser.SoapAPI.sdsClient.ISitkaSoapService, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "3.0.0.0")]
    public partial class SitkaSoapServiceClient : System.ServiceModel.ClientBase<CCCBrowser.SoapAPI.sdsClient.ISitkaSoapService>, CCCBrowser.SoapAPI.sdsClient.ISitkaSoapService {
        
        public SitkaSoapServiceClient() {
        }
        
        public SitkaSoapServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public SitkaSoapServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public SitkaSoapServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public SitkaSoapServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public CCCBrowser.SoapAPI.sdsClient.Scope Create(CCCBrowser.SoapAPI.sdsClient.Scope scope, CCCBrowser.SoapAPI.sdsClient.Entity entity) {
            return base.Channel.Create(scope, entity);
        }
        
        public CCCBrowser.SoapAPI.sdsClient.Entity Get(CCCBrowser.SoapAPI.sdsClient.Scope scope) {
            return base.Channel.Get(scope);
        }
        
        public CCCBrowser.SoapAPI.sdsClient.Scope Update(CCCBrowser.SoapAPI.sdsClient.Scope scope, CCCBrowser.SoapAPI.sdsClient.Entity entity) {
            return base.Channel.Update(scope, entity);
        }
        
        public void Delete(CCCBrowser.SoapAPI.sdsClient.Scope scope) {
            base.Channel.Delete(scope);
        }
        
        public System.Collections.Generic.List<CCCBrowser.SoapAPI.sdsClient.Entity> Query(CCCBrowser.SoapAPI.sdsClient.Scope scope, string query1) {
            return base.Channel.Query(scope, query1);
        }
    }
}
