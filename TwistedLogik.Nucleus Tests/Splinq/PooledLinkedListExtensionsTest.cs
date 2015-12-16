﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TwistedLogik.Nucleus.Collections;
using TwistedLogik.Nucleus.Splinq;
using TwistedLogik.Nucleus.Testing;

namespace TwistedLogik.NucleusTests.Splinq
{
    [TestClass]
    public class PooledLinkedListExtensionsTest : NucleusTestFramework
    {
        [TestMethod]
        public void PooledLinkedListExtensions_Any_ReturnsTrueIfPooledLinkedListContainsItems()
        {
            var list = new PooledLinkedList<Int32>();
            list.AddLast(1);

            var result = list.Any();

            TheResultingValue(result).ShouldBe(true);
        }

        [TestMethod]
        public void PooledLinkedListExtensions_Any_ReturnsFalseIfPooledLinkedListDoesNotContainItems()
        {
            var list = new PooledLinkedList<Int32>();

            var result = list.Any();

            TheResultingValue(result).ShouldBe(false);
        }

        [TestMethod]
        public void PooledLinkedListExtensions_AnyWithPredicate_ReturnsTrueIfPooledLinkedListContainsMatchingItems()
        {
            var list = new PooledLinkedList<Int32>();
            list.AddLast(1);
            list.AddLast(2);
            list.AddLast(3);

            var result = list.Any(x => x % 2 == 0);

            TheResultingValue(result).ShouldBe(true);
        }

        [TestMethod]
        public void PooledLinkedListExtensions_AnyWithPredicate_ReturnsFalseIfPooledLinkedListDoesNotContainMatchingItems()
        {
            var list = new PooledLinkedList<Int32>();
            list.AddLast(1);
            list.AddLast(3);

            var result = list.Any(x => x % 2 == 0);

            TheResultingValue(result).ShouldBe(false);
        }

        [TestMethod]
        public void PooledLinkedListExtensions_All_ReturnsTrueIfAllItemsMatchPredicate()
        {
            var list = new PooledLinkedList<Int32>();
            list.AddLast(2);
            list.AddLast(4);
            list.AddLast(6);

            var result = list.All(x => x % 2 == 0);

            TheResultingValue(result).ShouldBe(true);
        }

        [TestMethod]
        public void PooledLinkedListExtensions_All_ReturnsTrueIfPooledLinkedListIsEmpty()
        {
            var list = new PooledLinkedList<Int32>();

            var result = list.All(x => x % 2 == 0);

            TheResultingValue(result).ShouldBe(true);
        }

        [TestMethod]
        public void PooledLinkedListExtensions_All_ReturnsFalseIfOneItemDoesNotMatchPredicate()
        {
            var list = new PooledLinkedList<Int32>();
            list.AddLast(1);
            list.AddLast(2);
            list.AddLast(4);
            list.AddLast(6);

            var result = list.All(x => x % 2 == 0);

            TheResultingValue(result).ShouldBe(false);
        }

        [TestMethod]
        public void PooledLinkedListExtensions_Count_ReturnsCorrectSize()
        {
            var list = new PooledLinkedList<Int32>();
            list.AddLast(1);
            list.AddLast(2);
            list.AddLast(3);

            var result = list.Count();

            TheResultingValue(result).ShouldBe(3);
        }

        [TestMethod]
        public void PooledLinkedListExtensions_CountWithPredicate_ReturnsCorrectSize()
        {
            var list = new PooledLinkedList<Int32>();
            list.AddLast(1);
            list.AddLast(2);
            list.AddLast(3);

            var result = list.Count(x => x % 2 == 0);

            TheResultingValue(result).ShouldBe(1);
        }

        [TestMethod]
        public void PooledLinkedListExtensions_First_ReturnsFirstItemInPooledLinkedList()
        {
            var list = new PooledLinkedList<Int32>();
            list.AddLast(1);
            list.AddLast(2);
            list.AddLast(3);

            var result = list.First();

            TheResultingValue(result).ShouldBe(1);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void PooledLinkedListExtensions_First_ThrowsExceptionIfPooledLinkedListIsEmpty()
        {
            var list = new PooledLinkedList<Int32>();

            list.First();
        }

        [TestMethod]
        public void PooledLinkedListExtensions_Last_ReturnsLastItemInPooledLinkedList()
        {
            var list = new PooledLinkedList<Int32>();
            list.AddLast(1);
            list.AddLast(2);
            list.AddLast(3);

            var result = list.Last();

            TheResultingValue(result).ShouldBe(3);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void PooledLinkedListExtensions_Last_ThrowsExceptionIfPooledLinkedListIsEmpty()
        {
            var list = new PooledLinkedList<Int32>();

            list.Last();
        }

        [TestMethod]
        public void PooledLinkedListExtensions_Single_ReturnsSingleItemInPooledLinkedList()
        {
            var list = new PooledLinkedList<Int32>();
            list.AddLast(4);

            var result = list.Single();

            TheResultingValue(result).ShouldBe(4);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void PooledLinkedListExtensions_Single_ThrowsExceptionIfPooledLinkedListIsEmpty()
        {
            var list = new PooledLinkedList<Int32>();

            list.Single();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void PooledLinkedListExtensions_Single_ThrowsExceptionIfPooledLinkedListHasMultipleItems()
        {
            var list = new PooledLinkedList<Int32>();
            list.AddLast(1);
            list.AddLast(2);

            list.Single();
        }

        [TestMethod]
        public void PooledLinkedListExtensions_SingleOrDefault_ReturnsSingleItemInPooledLinkedList()
        {
            var list = new PooledLinkedList<Int32>();
            list.AddLast(4);

            var result = list.SingleOrDefault();

            TheResultingValue(result).ShouldBe(4);
        }

        [TestMethod]
        public void PooledLinkedListExtensions_SingleOrDefault_ReturnsDefaultValueIfPooledLinkedListIsEmpty()
        {
            var list = new PooledLinkedList<Int32>();

            var result = list.SingleOrDefault();

            TheResultingValue(result).ShouldBe(default(Int32));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void PooledLinkedListExtensions_SingleOrDefault_ThrowsExceptionIfPooledLinkedListHasMultipleItems()
        {
            var list = new PooledLinkedList<Int32>();
            list.AddLast(1);
            list.AddLast(2);

            list.SingleOrDefault();
        }

        [TestMethod]
        public void PooledLinkedListExtensions_Max_ReturnsMaxValue()
        {
            var list = new PooledLinkedList<Int32>();
            list.AddLast(4);
            list.AddLast(5);
            list.AddLast(6);
            list.AddLast(99);
            list.AddLast(10);
            list.AddLast(1);
            list.AddLast(12);
            list.AddLast(45);

            var result = list.Max();

            TheResultingValue(result).ShouldBe(99);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void PooledLinkedListExtensions_Max_ThrowsExceptionIfPooledLinkedListIsEmpty()
        {
            var list = new PooledLinkedList<Int32>();

            list.Max();
        }

        [TestMethod]
        public void PooledLinkedListExtensions_Min_ReturnsMinValue()
        {
            var list = new PooledLinkedList<Int32>();
            list.AddLast(4);
            list.AddLast(5);
            list.AddLast(6);
            list.AddLast(99);
            list.AddLast(10);
            list.AddLast(1);
            list.AddLast(12);
            list.AddLast(45);

            var result = list.Min();

            TheResultingValue(result).ShouldBe(1);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void PooledLinkedListExtensions_Min_ThrowsExceptionIfPooledLinkedListIsEmpty()
        {
            var list = new PooledLinkedList<Int32>();

            list.Min();
        }
    }
}
