using System;

namespace DbUp.Engine
{
    class DelegateDisposable : IDisposable
    {
        readonly Action dispose;

        public DelegateDisposable(Action dispose)
        {
            this.dispose = dispose;
        }

        public void Dispose() => dispose();
    }
}