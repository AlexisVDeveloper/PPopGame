using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ObjectsPool
{
    public class Pool<T>
    {
        private List<PoolObject<T>> _allObjects;
        private const int _maxCount = 100;
        private Func<T> _factoryMethod;
        private Action<T> _initObject, _finalizeObject;

        
        public Pool(Func<T> factoryMethod, Action<T> init, Action<T> finalize)
        {
            _allObjects = new List<PoolObject<T>>();
            _factoryMethod = factoryMethod;
            _initObject = init;
            _finalizeObject = finalize;
        }

        public PoolObject<T> GetObject(int count)
        {
            for(var i = 0; i < count; i++) {
                if(!_allObjects[i].isActive) {
                    _allObjects[i].isActive = true;
                    return _allObjects[i];
                }
            }

            if(_allObjects.Count < _maxCount) {
                var obj = new PoolObject<T>(_factoryMethod(), _initObject, _finalizeObject);
                obj.isActive = true;
                _allObjects.Add(obj);
                return obj;
            }

            return null;
        }

        public void DesactivePoolObject(T obj) {
            foreach(var pObj in _allObjects) {
                if(pObj.GetObj.Equals(obj)) {
                    pObj.isActive = false;
                    return;
                }
            }
        }
    }
}

