using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace TestProject
{
    public struct TestStruct
    {
        public int Value { get; set; }
    }

    public class TestClass
    {
        public int Value { get; set; }
    }

    public class UnitTest1
    {

        private IQueue<T> InitQueue<T>() => new Queue<T>(); //Add queue 


        [Fact]
        [Trait("DisplayName", "Struct Dequeue empty")]
        public void StructQueue_Dequeue_Empty()
        {
            var q = InitQueue<TestStruct>();
            var ex = Record.Exception(() => q.Dequeue());
            ex.Should().NotBeNull().And.BeOfType<InvalidOperationException>();
        }

        [Fact]
        [Trait("DisplayName", "Class Dequeue empty")]
        public void ClassQueue_Dequeue_Empty()
        {
            var q = InitQueue<TestClass>();
            var ex = Record.Exception(() => q.Dequeue());
            ex.Should().NotBeNull().And.BeOfType<InvalidOperationException>();
        }


        [Fact]
        [Trait("DisplayName", "Struct Dequeue more than enque")]
        public void StructQueue_Dequeue_MoreThanExists()
        {
            var q = InitQueue<TestStruct>();

            foreach (var i in Enumerable.Range(0, 5))
                q.Enqueue(new TestStruct { Value = i });
            foreach (var _ in Enumerable.Range(0, 5))
                q.Dequeue();

            var ex = Record.Exception(() => q.Dequeue());
            ex.Should().NotBeNull().And.BeOfType<InvalidOperationException>();
        }

        [Fact]
        [Trait("DisplayName", "Class Dequeue more than enque")]
        public void ClassQueue_Dequeue_MoreThanExists()
        {
            var q = InitQueue<TestClass>();

            foreach (var i in Enumerable.Range(0, 5))
                q.Enqueue(new TestClass { Value = i });
            foreach (var _ in Enumerable.Range(0, 5))
                q.Dequeue();

            var ex = Record.Exception(() => q.Dequeue());
            ex.Should().NotBeNull().And.BeOfType<InvalidOperationException>();
        }

        [Fact]
        [Trait("DisplayName", "Struct Count Empty")]
        public void StructQueue_Count_Empty()
        {
            var q = InitQueue<TestStruct>();
            q.Count.Should().Be(0);
        }

        [Fact]
        [Trait("DisplayName", "Class Count Empty")]
        public void ClassQueue_Count_Empty()
        {
            var q = InitQueue<TestClass>();
            q.Count.Should().Be(0);
        }

        [Fact]
        [Trait("DisplayName", "Struct Count Not empty")]
        public void StructQueue_Count_NotEmpty()
        {
            var q = InitQueue<TestStruct>();

            foreach (var i in Enumerable.Range(0, 10))
                q.Enqueue(new TestStruct { Value = i });
            foreach (var _ in Enumerable.Range(0, 7))
                q.Dequeue();
            q.Count.Should().Be(3);
        }

        [Fact]
        [Trait("DisplayName", "Class Count Not empty")]
        public void ClassQueue_Count_NotEmpty()
        {
            var q = InitQueue<TestClass>();

            foreach (var i in Enumerable.Range(0, 10))
                q.Enqueue(new TestClass { Value = i });
            foreach (var _ in Enumerable.Range(0, 7))
                q.Dequeue();
            q.Count.Should().Be(3);
        }


        [Fact]
        [Trait("DisplayName", "Struct Dequeue order")]
        public void StructQueue_Dequeue_Order()
        {
            var q = InitQueue<TestStruct>();
            foreach (var i in Enumerable.Range(0, 10))
                q.Enqueue(new TestStruct { Value = i });
            foreach (var i in Enumerable.Range(0, 10))
                q.Dequeue().Should().Be(new TestStruct { Value = i });
        }

        [Fact]
        [Trait("DisplayName", "Class Dequeue order")]
        public void ClassQueue_Dequeue_Order()
        {
            var testItem1 = new TestClass { Value = 1 };
            var testItem2 = new TestClass { Value = 2 };
            var testItem3 = new TestClass { Value = 3 };
            var testItem4 = new TestClass { Value = 4 };

            var q = InitQueue<TestClass>();

            q.Enqueue(testItem1);
            q.Enqueue(testItem2);
            q.Enqueue(testItem3);
            q.Enqueue(testItem4);

            var r1 = q.Dequeue();
            var r2 = q.Dequeue();
            var r3 = q.Dequeue();
            var r4 = q.Dequeue();

            r1.Should().BeSameAs(testItem1);
            r2.Should().BeSameAs(testItem2);
            r3.Should().BeSameAs(testItem3);
            r4.Should().BeSameAs(testItem4);
        }

        [Fact]
        [Trait("DisplayName", "Struct Foreach simple")]
        public void StructQueue_Foreach_Simple()
        {
            var q = InitQueue<TestStruct>();
            foreach (var i in Enumerable.Range(0, 10))
                q.Enqueue(new TestStruct { Value = i });

            int k = 0;
            foreach (var item in q)
                item.Should().Be(new TestStruct { Value = k++ });

        }

        [Fact]
        [Trait("DisplayName", "Class Foreach simple")]
        public void ClassQueue_Foreach_Simple()
        {
            var testItem1 = new TestClass { Value = 1 };
            var testItem2 = new TestClass { Value = 2 };
            var testItem3 = new TestClass { Value = 3 };
            var testItem4 = new TestClass { Value = 4 };

            var q = InitQueue<TestClass>();

            q.Enqueue(testItem1);
            q.Enqueue(testItem2);
            q.Enqueue(testItem3);
            q.Enqueue(testItem4);

            var arr = new[]
            {
                testItem1,testItem2,testItem3,testItem4
            };

            int k = 0;
            foreach (var item in q)
                item.Should().BeSameAs(arr[k++]);

        }

        [Fact]
        [Trait("DisplayName", "Struct Foreach invalidation Dequeue")]
        public void StructQueue_Foreach_InvalidationDequeue()
        {
            var q = InitQueue<TestStruct>();
            foreach (var i in Enumerable.Range(0, 10))
                q.Enqueue(new TestStruct { Value = i });
            void a()
            {
                foreach (var item in q)
                    q.Dequeue();
            }
            var ex = Record.Exception(a);
            ex.Should().NotBeNull().And.BeOfType<InvalidOperationException>();
        }

        [Fact]
        [Trait("DisplayName", "Class Foreach invalidation Dequeue")]
        public void ClassQueue_Foreach_InvalidationDequeue()
        {
            var q = InitQueue<TestClass>();
            foreach (var i in Enumerable.Range(0, 10))
                q.Enqueue(new TestClass { Value = i });
            void a()
            {
                foreach (var item in q)
                    q.Dequeue();
            }
            var ex = Record.Exception(a);
            ex.Should().NotBeNull().And.BeOfType<InvalidOperationException>();
        }

        [Fact]
        [Trait("DisplayName", "Struct Foreach invalidation Enqueue")]
        public void StructQueue_Foreach_InvalidationEnqueue()
        {
            var q = InitQueue<TestStruct>();
            foreach (var i in Enumerable.Range(0, 10))
                q.Enqueue(new TestStruct { Value = i });
            void a()
            {
                foreach (var item in q)
                    q.Enqueue(default);
            }
            var ex = Record.Exception(a);
            ex.Should().NotBeNull().And.BeOfType<InvalidOperationException>();
        }

        [Fact]
        [Trait("DisplayName", "Class Foreach invalidation Enqueue")]
        public void ClassQueue_Foreach_InvalidationEnqueue()
        {
            var q = InitQueue<TestClass>();
            foreach (var i in Enumerable.Range(0, 10))
                q.Enqueue(new TestClass { Value = i });
            void a()
            {
                foreach (var item in q)
                    q.Enqueue(default);
            }
            var ex = Record.Exception(a);
            ex.Should().NotBeNull().And.BeOfType<InvalidOperationException>();
        }

        [Fact]
        [Trait("DisplayName", "Struct Foreach Reset")]
        public void StructQueue_Iterator_Reset()
        {
            var q = InitQueue<TestStruct>();
            foreach (var i in Enumerable.Range(0, 10))
                q.Enqueue(new TestStruct { Value = i });
            int k = 0;
            var iterator = q.GetEnumerator();
            while (iterator.MoveNext())
                iterator.Current.Should().Be(new TestStruct { Value = k++ });
            k.Should().Be(10);
            k = 0;
            iterator.Reset();
            while (iterator.MoveNext())
                iterator.Current.Should().Be(new TestStruct { Value = k++ });
            k.Should().Be(10);
        }

        [Fact]
        [Trait("DisplayName", "Class Foreach Reset")]
        public void ClassQueue_Iterator_Reset()
        {
            var q = InitQueue<TestClass>();

            var arr = new[]
            {
                new TestClass{ Value = 1 },
                new TestClass{ Value = 2 },
                new TestClass{ Value = 3 },
            };

            foreach (var i in Enumerable.Range(0, 3))
                q.Enqueue(arr[i]);
            int k = 0;
            var iterator = q.GetEnumerator();
            while (iterator.MoveNext())
                iterator.Current.Should().BeSameAs(arr[k++]);
            k.Should().Be(3);
            k = 0;
            iterator.Reset();
            while (iterator.MoveNext())
                iterator.Current.Should().BeSameAs(arr[k++]);
            k.Should().Be(3);
        }
    }
}