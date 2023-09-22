using System;
using System.Diagnostics;

namespace PensionCoach.Tools.CommonUtils
{
    public sealed class MeasureTime : IDisposable
    {
        private readonly Action<long> _action;
        private readonly Stopwatch _sw;

        public MeasureTime(Action<long> action)
        {
            _action = action;
            _sw = new Stopwatch();
            _sw.Start();
        }

        public void Dispose()
        {
            _sw.Stop();
            _action.Invoke(_sw.ElapsedMilliseconds);
        }
    }
}
