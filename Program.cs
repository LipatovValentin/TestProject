using System;
using System.Collections;
using System.Collections.Generic;

namespace TestProject
{
    public interface IQueue<T> : IEnumerable<T>
    {
        T Dequeue();
        void Enqueue(T item);
        int Count { get; }
    }
    public class Node<T>
    {
        public Node(T value)
        {
            Value = value;
        }
        public T Value { get; set; }
        public Node<T> Next { get; set; }
    }
    public class QueueWithNode<T> : IQueue<T>
    {
        private Node<T> _head;
        private Node<T> _tail;
        private int _count;
        private int _version;
        public Node<T> Head { get => _head; }
        public Node<T> Tail { get => _tail; }
        public int Count { get => _count; }
        public int Version { get => _version; }
        public QueueWithNode()
        {
            _head = null;
            _tail = null;
            _count = 0;
            _version = 0;
        }
        public void Enqueue(T item)
        {
            Node<T> tempNode = _tail;
            _tail = new Node<T>(item);
            if (_count == 0 || _head == null)
            {
                _head = _tail;
            }
            else
            {
                tempNode.Next = _tail;
            }
            _count = _count + 1;
            _version = _version + 1;
        }
        public T Dequeue()
        {
            if (_count == 0 || _head == null)
            {
                throw new InvalidOperationException();
            }
            T obj = _head.Value;
            _head = _head.Next;
            _count = _count - 1;
            _version = _version + 1;
            return obj;
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return new EnumeratorWithNode<T>(this);
        }
        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return new EnumeratorWithNode<T>(this);
        }
    }
    public struct EnumeratorWithNode<T> : IEnumerator<T>, IDisposable, IEnumerator
    {
        private QueueWithNode<T> _queue;
        private Node<T> _current;
        private int _version;
        internal EnumeratorWithNode(QueueWithNode<T> queue)
        {
            _queue = queue;
            _version = _queue.Version;
            _current = null;
        }
        public void Dispose()
        {
            _current = null;
        }
        public bool MoveNext()
        {
            if (_version != _queue.Version)
            {
                throw new InvalidOperationException();
            }
            if (_queue == null)
            {
                return false;
            }
            if (_queue.Head == null)
            {
                return false;
            }
            if (_current == null)
            {
                _current = _queue.Head;
                return true;
            }
            else
            {
                if (_current.Next != null)
                {
                    _current = _current.Next;
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        public T Current
        {
            get
            {
                if (_current == null)
                {
                    throw new InvalidOperationException();
                }
                return _current.Value;
            }
        }
        object IEnumerator.Current
        {
            get
            {
                if (_current == null)
                {
                    throw new InvalidOperationException();
                }
                return (object)_current.Value;
            }
        }
        void IEnumerator.Reset()
        {
            if (_version != _queue.Version)
            {
                throw new InvalidOperationException();
            }
            _current = null;
        }
    }
    public class Queue<T> : IQueue<T>
    {
        private T[] _array;
        private int _head;
        private int _tail;
        private int _size;
        private int _version;
        public int Head { get => _head; }
        public int Tail { get => _tail; }
        public int Count { get => _size; }
        public int Version { get => _version; }
        public Queue() : this(new T[0]) { }
        public Queue(IEnumerable<T> collection)
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }
            _array = new T[0];
            _size = 0;
            _version = 0;
            _tail = 0;
            _head = 0;
            foreach (T obj in collection)
            {
                Enqueue(obj);
            }
        }
        public T GetElement(int index)
        {
            if (_array == null)
            {
                throw new InvalidOperationException();
            }
            return _array[_head + index];
        }
        public void Enqueue(T item)
        {
            SetCapacity(_array.Length + 1);
            _array[_tail] = item;
            _tail = _tail + 1;
            _size = _size + 1;
            _version = _version + 1;
        }
        public T Dequeue()
        {
            if (_size == 0)
            {
                throw new InvalidOperationException();
            }
            T obj = _array[_head];
            _array[_head] = default(T);
            _head = _head + 1;
            _size = _size - 1;
            _version = _version + 1;
            return obj;
        }
        private void SetCapacity(int capacity)
        {
            T[] objArray = new T[capacity];
            if (_size > 0)
            {
                if (_head < _tail)
                {
                    Array.Copy(_array, _head, objArray, 0, _size);
                }
                else
                {
                    Array.Copy(_array, _head, objArray, 0, _array.Length - _head);
                    Array.Copy(_array, 0, objArray, _array.Length - _head, _tail);
                }
            }
            _array = objArray;
            _head = 0;
            _tail = _size == capacity ? 0 : _size;
            _version = _version + 1;
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return new Enumerator<T>(this);
        }
        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return new Enumerator<T>(this);
        }
    }
    public struct Enumerator<T> : IEnumerator<T>, IDisposable, IEnumerator
    {
        private Queue<T> _queue;
        private int _index;
        private T _current;
        private int _version;
        internal Enumerator(Queue<T> queue)
        {
            _queue = queue;
            _index = -1;
            _version = _queue.Version;
            _current = default(T);
        }
        public void Dispose()
        {
            _index = -2;
            _current = default(T);
        }
        public bool MoveNext()
        {
            if (_version != _queue.Version)
            {
                throw new InvalidOperationException();
            }
            if (_index == -2)
            {
                return false;
            }
            _index = _index + 1;
            if (_index == _queue.Count)
            {
                _index = -2;
                _current = default(T);
                return false;
            }
            _current = _queue.GetElement(_index);
            return true;
        }
        public T Current
        {
            get
            {
                if (_index < 0)
                {
                    throw new InvalidOperationException();
                }
                return _current;
            }
        }
        object IEnumerator.Current
        {
            get
            {
                if (_index < 0)
                {
                    throw new InvalidOperationException();
                }
                return (object)_current;
            }
        }
        void IEnumerator.Reset()
        {
            if (_version != _queue.Version)
            {
                throw new InvalidOperationException();
            }
            _index = -1;
            _current = default(T);
        }
    }
}
