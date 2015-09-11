using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

namespace DbUp.Tests.TestInfrastructure
{
    internal class RecordingDataParameterCollection : IDataParameterCollection
    {
        private readonly Action<DatabaseAction> recordAction;
        private readonly List<object> backingList;

        public RecordingDataParameterCollection(Action<DatabaseAction> recordAction)
        {
            this.recordAction = recordAction;
            backingList = new List<object>();
        }

        public IEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        public int Count { get; private set; }
        public object SyncRoot { get; private set; }
        public bool IsSynchronized { get; private set; }
        public int Add(object value)
        {
            recordAction(DatabaseAction.AddParameterToCommand(value));
            backingList.Add(value);
            return backingList.Count - 1;
        }

        public bool Contains(object value)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public int IndexOf(object value)
        {
            throw new NotImplementedException();
        }

        public void Insert(int index, object value)
        {
            throw new NotImplementedException();
        }

        public void Remove(object value)
        {
            throw new NotImplementedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        object IList.this[int index]
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public bool IsReadOnly { get; private set; }
        public bool IsFixedSize { get; private set; }
        public bool Contains(string parameterName)
        {
            throw new NotImplementedException();
        }

        public int IndexOf(string parameterName)
        {
            throw new NotImplementedException();
        }

        public void RemoveAt(string parameterName)
        {
            throw new NotImplementedException();
        }

        object IDataParameterCollection.this[string parameterName]
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }
    }
}