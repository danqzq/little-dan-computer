using Danqzq.Models;
using UnityEngine;

namespace Danqzq
{
    public sealed class Promise
    {
        public bool IsResolved { get; private set; }
        
        private System.Action<string> _onComplete, _onError;
        
        public Promise(System.Action<object> onComplete = null)
        {
            _onComplete = onComplete;
        }
        
        public Promise Then(System.Action<string> action)
        {
            _onComplete += action;
            return this;
        }
        
        public Promise Then<T>(System.Action<T> action)
        {
            _onComplete += x => action(JsonUtility.FromJson<T>(x));
            return this;
        }
        
        public void Catch(System.Action<string> action)
        {
            _onError = action;
        }
        
        public void Resolve(string responseObject, bool success = true)
        {
            IsResolved = true;
            if (success)
                _onComplete?.Invoke(responseObject);
            else
                _onError?.Invoke(responseObject);
        }
        
        public Promise<T> Cast<T>() where T : IServerObject
        {
            return new Promise<T>(this);
        }
    }
    
    public sealed class Promise<T> where T : IServerObject
    {
        public bool IsResolved => _promise.IsResolved;
        
        private Promise _promise;
        
        public Promise(Promise promise)
        {
            _promise = promise;
        }
        
        public Promise<T> Then(System.Action<T> action)
        {
            _promise.Then(action);
            return this;
        }
        
        public void Catch(System.Action<string> action)
        {
            _promise.Catch(action);
        }
        
        public void Feed(string responseObject, bool success = true)
        {
            _promise.Resolve(responseObject, success);
        }
    }
}