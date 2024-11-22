using System;
using System.Collections.Generic;
using System.Collections;
using static UnityEngine.ProBuilder.AutoUnwrapSettings;

namespace Adapt
{ 
    class SlidingBuffer<T> : IEnumerable<T>
    {
        private readonly Queue<T> _queue;
        private readonly int _maxCount;
        private readonly int _currCount;

        public SlidingBuffer(int maxCount)
        {
            _maxCount = maxCount;
            _queue = new Queue<T>(maxCount);
        }

        public void Add(T item)
        {
            if (_queue.Count == _maxCount)
                _queue.Dequeue();
            _queue.Enqueue(item);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _queue.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    class MathUtil
    {
        public static double LerpDouble(double start, double end, double t)
        {
            return start + (end - start) * t;
        }
    }

}

