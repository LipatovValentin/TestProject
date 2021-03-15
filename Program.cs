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
        public T Value { get; set; }
        public Node<T> Next { get; set; }
        public Node(T value)
        {
            Value = value;
        }
    }
    public class QueueWithNode<T> : IQueue<T>
    {
        private Node<T> _head;
        private int _count;
        private int _version;
        public Node<T> Head 
        {
            get
            {
                return _head;
            }
        } 
        public int Count 
        { 
            get 
            { 
                return _count;
            }
        }
        public int Version
        {
            get
            {
               return _version;
            }
        }
        public QueueWithNode()
        {
            _head = null;
            _count = 0;
            _version = 0;
        }
        public void Enqueue(T item)
        {
            if (_head == null)
            {
                _head = new Node<T>(item);
            }
            else
            {
                GetLast(_head).Next = new Node<T>(item);
            }
            _count = _count + 1;
            _version = _version + 1;
        }
        public T Dequeue()
        {
            if (_head == null)
            {
                throw new InvalidOperationException();
            }
            T obj = _head.Value;
            _head = _head.Next;
            _count = _count - 1;
            _version = _version + 1;
            return obj;
        }
        private Node<T> GetLast(Node<T> node)
        {
            if (node == null)
            {
                throw new InvalidOperationException();
            }
            if (node.Next == null)
            {
                return node;
            }
            else
            {
                return GetLast(node.Next);
            }
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
            if (queue == null)
            {
                throw new ArgumentNullException();
            }
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
        private int _count;
        private int _version;
        public int Head 
        { 
            get
            {
                return _head; 
            }
        }
        public int Tail 
        { 
            get 
            {
                return _tail;
            }
        }
        public int Count 
        { 
            get
            {
                return _count; 
            }
        }
        public int Version 
        { 
            get 
            {
                return _version; 
            }
        }
        public Queue()
        {
            _array = new T[0];
            _count = 0;
            _version = 0;
            _tail = 0;
            _head = 0;
        }
        public T GetElement(int index)
        {
            if (_array == null)
            {
                throw new InvalidOperationException();
            }
            return _array[(_head + index) % _array.Length];
        }
        public void Enqueue(T item)
        {
            if (_array == null || _array.Length == 0)
            {
                SetCapacity(2);
            }
            if (_count == _array.Length)
            {
                SetCapacity(_array.Length * 2);
            }
            _array[_tail] = item;
            _tail = (_tail + 1) % _array.Length;
            _count = _count + 1;
            _version = _version + 1;
        }
        public T Dequeue()
        {
            if (_count == 0 || _array == null || _array.Length == 0)
            {
                throw new InvalidOperationException();
            }
            T obj = this._array[this._head];
            _array[_head] = default(T);
            _head = (_head + 1) % _array.Length;
            _count = _count - 1;
            _version = _version + 1;
            return obj;
        }
        private void SetCapacity(int capacity)
        {
            T[] objArray = new T[capacity];
            if (_count > 0)
            {
                if (_head < _tail)
                {
                    Array.Copy(_array, _head, objArray, 0, _count);
                }
                else
                {
                    Array.Copy(_array, _head, objArray, 0, _array.Length - _head);
                    Array.Copy(_array, 0, objArray, _array.Length - _head, _tail);
                }
            }
            _array = objArray;
            _head = 0;
            _tail = _count == capacity ? 0 : _count;
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
