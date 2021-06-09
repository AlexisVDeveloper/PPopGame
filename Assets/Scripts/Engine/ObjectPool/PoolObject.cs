using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ObjectsPool
{
    public class PoolObject<T>
    {
        #region Attributes
        private bool _isActive;
        private Action<T> _init, _finalize;
        private T _obj;
        #endregion

        #region Public Methods
        public PoolObject(T obj, Action<T> init, Action<T> finalize) {
            _obj = obj;
            _init = init;
            _finalize = finalize;
            _isActive = false;
        }

        public T GetObj {
            get {
                return _obj;
            }
        }

        public bool isActive
        {
            get {
                return isActive;
            }

            set {
                _isActive = value;

                if (_isActive)
                    _init?.Invoke(_obj);
                else
                    _finalize?.Invoke(_obj);

            }
        }
        #endregion
    }
}
