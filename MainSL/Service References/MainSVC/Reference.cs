﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан программой.
//     Исполняемая версия:4.0.30319.34209
//
//     Изменения в этом файле могут привести к неправильной работе и будут потеряны в случае
//     повторной генерации кода.
// </auto-generated>
//------------------------------------------------------------------------------

// 
// This code was auto-generated by Microsoft.Silverlight.ServiceReference, version 5.0.61118.0
// 
namespace MainSL.MainSVC {
    using System.Runtime.Serialization;
    
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="User", Namespace="http://schemas.datacontract.org/2004/07/BlankJournal")]
    public partial class User : object, System.ComponentModel.INotifyPropertyChanged {
        
        private string UserLoginField;
        
        private string UserNameField;
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string UserLogin {
            get {
                return this.UserLoginField;
            }
            set {
                if ((object.ReferenceEquals(this.UserLoginField, value) != true)) {
                    this.UserLoginField = value;
                    this.RaisePropertyChanged("UserLogin");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string UserName {
            get {
                return this.UserNameField;
            }
            set {
                if ((object.ReferenceEquals(this.UserNameField, value) != true)) {
                    this.UserNameField = value;
                    this.RaisePropertyChanged("UserName");
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
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(Namespace="", ConfigurationName="MainSVC.MainService")]
    public interface MainService {
        
        [System.ServiceModel.OperationContractAttribute(AsyncPattern=true, Action="urn:MainService/DoWork", ReplyAction="urn:MainService/DoWorkResponse")]
        System.IAsyncResult BeginDoWork(System.AsyncCallback callback, object asyncState);
        
        void EndDoWork(System.IAsyncResult result);
        
        [System.ServiceModel.OperationContractAttribute(AsyncPattern=true, Action="urn:MainService/GetUser", ReplyAction="urn:MainService/GetUserResponse")]
        System.IAsyncResult BeginGetUser(System.AsyncCallback callback, object asyncState);
        
        MainSL.MainSVC.User EndGetUser(System.IAsyncResult result);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface MainServiceChannel : MainSL.MainSVC.MainService, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class GetUserCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        public GetUserCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        public MainSL.MainSVC.User Result {
            get {
                base.RaiseExceptionIfNecessary();
                return ((MainSL.MainSVC.User)(this.results[0]));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class MainServiceClient : System.ServiceModel.ClientBase<MainSL.MainSVC.MainService>, MainSL.MainSVC.MainService {
        
        private BeginOperationDelegate onBeginDoWorkDelegate;
        
        private EndOperationDelegate onEndDoWorkDelegate;
        
        private System.Threading.SendOrPostCallback onDoWorkCompletedDelegate;
        
        private BeginOperationDelegate onBeginGetUserDelegate;
        
        private EndOperationDelegate onEndGetUserDelegate;
        
        private System.Threading.SendOrPostCallback onGetUserCompletedDelegate;
        
        private BeginOperationDelegate onBeginOpenDelegate;
        
        private EndOperationDelegate onEndOpenDelegate;
        
        private System.Threading.SendOrPostCallback onOpenCompletedDelegate;
        
        private BeginOperationDelegate onBeginCloseDelegate;
        
        private EndOperationDelegate onEndCloseDelegate;
        
        private System.Threading.SendOrPostCallback onCloseCompletedDelegate;
        
        public MainServiceClient() {
        }
        
        public MainServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public MainServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public MainServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public MainServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public System.Net.CookieContainer CookieContainer {
            get {
                System.ServiceModel.Channels.IHttpCookieContainerManager httpCookieContainerManager = this.InnerChannel.GetProperty<System.ServiceModel.Channels.IHttpCookieContainerManager>();
                if ((httpCookieContainerManager != null)) {
                    return httpCookieContainerManager.CookieContainer;
                }
                else {
                    return null;
                }
            }
            set {
                System.ServiceModel.Channels.IHttpCookieContainerManager httpCookieContainerManager = this.InnerChannel.GetProperty<System.ServiceModel.Channels.IHttpCookieContainerManager>();
                if ((httpCookieContainerManager != null)) {
                    httpCookieContainerManager.CookieContainer = value;
                }
                else {
                    throw new System.InvalidOperationException("Не удалось установить CookieContainer. Убедитесь, что привязка содержит HttpCooki" +
                            "eContainerBindingElement.");
                }
            }
        }
        
        public event System.EventHandler<System.ComponentModel.AsyncCompletedEventArgs> DoWorkCompleted;
        
        public event System.EventHandler<GetUserCompletedEventArgs> GetUserCompleted;
        
        public event System.EventHandler<System.ComponentModel.AsyncCompletedEventArgs> OpenCompleted;
        
        public event System.EventHandler<System.ComponentModel.AsyncCompletedEventArgs> CloseCompleted;
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.IAsyncResult MainSL.MainSVC.MainService.BeginDoWork(System.AsyncCallback callback, object asyncState) {
            return base.Channel.BeginDoWork(callback, asyncState);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        void MainSL.MainSVC.MainService.EndDoWork(System.IAsyncResult result) {
            base.Channel.EndDoWork(result);
        }
        
        private System.IAsyncResult OnBeginDoWork(object[] inValues, System.AsyncCallback callback, object asyncState) {
            return ((MainSL.MainSVC.MainService)(this)).BeginDoWork(callback, asyncState);
        }
        
        private object[] OnEndDoWork(System.IAsyncResult result) {
            ((MainSL.MainSVC.MainService)(this)).EndDoWork(result);
            return null;
        }
        
        private void OnDoWorkCompleted(object state) {
            if ((this.DoWorkCompleted != null)) {
                InvokeAsyncCompletedEventArgs e = ((InvokeAsyncCompletedEventArgs)(state));
                this.DoWorkCompleted(this, new System.ComponentModel.AsyncCompletedEventArgs(e.Error, e.Cancelled, e.UserState));
            }
        }
        
        public void DoWorkAsync() {
            this.DoWorkAsync(null);
        }
        
        public void DoWorkAsync(object userState) {
            if ((this.onBeginDoWorkDelegate == null)) {
                this.onBeginDoWorkDelegate = new BeginOperationDelegate(this.OnBeginDoWork);
            }
            if ((this.onEndDoWorkDelegate == null)) {
                this.onEndDoWorkDelegate = new EndOperationDelegate(this.OnEndDoWork);
            }
            if ((this.onDoWorkCompletedDelegate == null)) {
                this.onDoWorkCompletedDelegate = new System.Threading.SendOrPostCallback(this.OnDoWorkCompleted);
            }
            base.InvokeAsync(this.onBeginDoWorkDelegate, null, this.onEndDoWorkDelegate, this.onDoWorkCompletedDelegate, userState);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.IAsyncResult MainSL.MainSVC.MainService.BeginGetUser(System.AsyncCallback callback, object asyncState) {
            return base.Channel.BeginGetUser(callback, asyncState);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        MainSL.MainSVC.User MainSL.MainSVC.MainService.EndGetUser(System.IAsyncResult result) {
            return base.Channel.EndGetUser(result);
        }
        
        private System.IAsyncResult OnBeginGetUser(object[] inValues, System.AsyncCallback callback, object asyncState) {
            return ((MainSL.MainSVC.MainService)(this)).BeginGetUser(callback, asyncState);
        }
        
        private object[] OnEndGetUser(System.IAsyncResult result) {
            MainSL.MainSVC.User retVal = ((MainSL.MainSVC.MainService)(this)).EndGetUser(result);
            return new object[] {
                    retVal};
        }
        
        private void OnGetUserCompleted(object state) {
            if ((this.GetUserCompleted != null)) {
                InvokeAsyncCompletedEventArgs e = ((InvokeAsyncCompletedEventArgs)(state));
                this.GetUserCompleted(this, new GetUserCompletedEventArgs(e.Results, e.Error, e.Cancelled, e.UserState));
            }
        }
        
        public void GetUserAsync() {
            this.GetUserAsync(null);
        }
        
        public void GetUserAsync(object userState) {
            if ((this.onBeginGetUserDelegate == null)) {
                this.onBeginGetUserDelegate = new BeginOperationDelegate(this.OnBeginGetUser);
            }
            if ((this.onEndGetUserDelegate == null)) {
                this.onEndGetUserDelegate = new EndOperationDelegate(this.OnEndGetUser);
            }
            if ((this.onGetUserCompletedDelegate == null)) {
                this.onGetUserCompletedDelegate = new System.Threading.SendOrPostCallback(this.OnGetUserCompleted);
            }
            base.InvokeAsync(this.onBeginGetUserDelegate, null, this.onEndGetUserDelegate, this.onGetUserCompletedDelegate, userState);
        }
        
        private System.IAsyncResult OnBeginOpen(object[] inValues, System.AsyncCallback callback, object asyncState) {
            return ((System.ServiceModel.ICommunicationObject)(this)).BeginOpen(callback, asyncState);
        }
        
        private object[] OnEndOpen(System.IAsyncResult result) {
            ((System.ServiceModel.ICommunicationObject)(this)).EndOpen(result);
            return null;
        }
        
        private void OnOpenCompleted(object state) {
            if ((this.OpenCompleted != null)) {
                InvokeAsyncCompletedEventArgs e = ((InvokeAsyncCompletedEventArgs)(state));
                this.OpenCompleted(this, new System.ComponentModel.AsyncCompletedEventArgs(e.Error, e.Cancelled, e.UserState));
            }
        }
        
        public void OpenAsync() {
            this.OpenAsync(null);
        }
        
        public void OpenAsync(object userState) {
            if ((this.onBeginOpenDelegate == null)) {
                this.onBeginOpenDelegate = new BeginOperationDelegate(this.OnBeginOpen);
            }
            if ((this.onEndOpenDelegate == null)) {
                this.onEndOpenDelegate = new EndOperationDelegate(this.OnEndOpen);
            }
            if ((this.onOpenCompletedDelegate == null)) {
                this.onOpenCompletedDelegate = new System.Threading.SendOrPostCallback(this.OnOpenCompleted);
            }
            base.InvokeAsync(this.onBeginOpenDelegate, null, this.onEndOpenDelegate, this.onOpenCompletedDelegate, userState);
        }
        
        private System.IAsyncResult OnBeginClose(object[] inValues, System.AsyncCallback callback, object asyncState) {
            return ((System.ServiceModel.ICommunicationObject)(this)).BeginClose(callback, asyncState);
        }
        
        private object[] OnEndClose(System.IAsyncResult result) {
            ((System.ServiceModel.ICommunicationObject)(this)).EndClose(result);
            return null;
        }
        
        private void OnCloseCompleted(object state) {
            if ((this.CloseCompleted != null)) {
                InvokeAsyncCompletedEventArgs e = ((InvokeAsyncCompletedEventArgs)(state));
                this.CloseCompleted(this, new System.ComponentModel.AsyncCompletedEventArgs(e.Error, e.Cancelled, e.UserState));
            }
        }
        
        public void CloseAsync() {
            this.CloseAsync(null);
        }
        
        public void CloseAsync(object userState) {
            if ((this.onBeginCloseDelegate == null)) {
                this.onBeginCloseDelegate = new BeginOperationDelegate(this.OnBeginClose);
            }
            if ((this.onEndCloseDelegate == null)) {
                this.onEndCloseDelegate = new EndOperationDelegate(this.OnEndClose);
            }
            if ((this.onCloseCompletedDelegate == null)) {
                this.onCloseCompletedDelegate = new System.Threading.SendOrPostCallback(this.OnCloseCompleted);
            }
            base.InvokeAsync(this.onBeginCloseDelegate, null, this.onEndCloseDelegate, this.onCloseCompletedDelegate, userState);
        }
        
        protected override MainSL.MainSVC.MainService CreateChannel() {
            return new MainServiceClientChannel(this);
        }
        
        private class MainServiceClientChannel : ChannelBase<MainSL.MainSVC.MainService>, MainSL.MainSVC.MainService {
            
            public MainServiceClientChannel(System.ServiceModel.ClientBase<MainSL.MainSVC.MainService> client) : 
                    base(client) {
            }
            
            public System.IAsyncResult BeginDoWork(System.AsyncCallback callback, object asyncState) {
                object[] _args = new object[0];
                System.IAsyncResult _result = base.BeginInvoke("DoWork", _args, callback, asyncState);
                return _result;
            }
            
            public void EndDoWork(System.IAsyncResult result) {
                object[] _args = new object[0];
                base.EndInvoke("DoWork", _args, result);
            }
            
            public System.IAsyncResult BeginGetUser(System.AsyncCallback callback, object asyncState) {
                object[] _args = new object[0];
                System.IAsyncResult _result = base.BeginInvoke("GetUser", _args, callback, asyncState);
                return _result;
            }
            
            public MainSL.MainSVC.User EndGetUser(System.IAsyncResult result) {
                object[] _args = new object[0];
                MainSL.MainSVC.User _result = ((MainSL.MainSVC.User)(base.EndInvoke("GetUser", _args, result)));
                return _result;
            }
        }
    }
}
