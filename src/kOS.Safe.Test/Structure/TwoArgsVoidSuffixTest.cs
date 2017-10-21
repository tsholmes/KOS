﻿using kOS.Safe.Encapsulation.Suffixes;
using kOS.Safe.Encapsulation;
using NSubstitute;
using NUnit.Framework;
using System;

namespace kOS.Safe.Test.Structure
{
    [TestFixture]
    public class TwoArgsVoidSuffixTest
    {
        [Test]
        public void CanCreate()
        {
            var suffix = new TwoArgsSuffix<kOS.Safe.Encapsulation.Structure, kOS.Safe.Encapsulation.Structure>((one, two) => { });
            Assert.IsNotNull(suffix.Get());
        }

        [Test]
        public void CanGetDelegate()
        {
            var suffix = new TwoArgsSuffix<kOS.Safe.Encapsulation.Structure, kOS.Safe.Encapsulation.Structure>((one, two) => { });
            var del = suffix.Get() as DelegateSuffixResult;
            Assert.IsNotNull(del);
            var delegateAsDelegate = del.Del;
            Assert.IsNotNull(delegateAsDelegate);
        }

        [Test]
        public void CanExecuteDelegate()
        {
            var mockDel = Substitute.For<TwoArgsSuffix<kOS.Safe.Encapsulation.Structure, kOS.Safe.Encapsulation.Structure>.Del>();

            var suffix = new TwoArgsSuffix<kOS.Safe.Encapsulation.Structure, kOS.Safe.Encapsulation.Structure>(mockDel);
            var del = suffix.Get() as DelegateSuffixResult;
            Assert.IsNotNull(del);
            var delegateAsDelegate = del.Del;
            Assert.IsNotNull(delegateAsDelegate);
            delegateAsDelegate.DynamicInvoke(new ScalarIntValue(0), new ScalarIntValue(0));

            mockDel.ReceivedWithAnyArgs(1);
        }
    }
}