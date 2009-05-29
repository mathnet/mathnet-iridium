//-----------------------------------------------------------------------
// <copyright file="SpecialFunctionsTest.cs" company="Math.NET Project">
//    Copyright (c) 2002-2009, Christoph Rüegg.
//    All Right Reserved.
// </copyright>
// <author>
//    Christoph Rüegg, http://christoph.ruegg.name
// </author>
// <product>
//    Math.NET Iridium, part of the Math.NET Project.
//    http://mathnet.opensourcedotnet.info
// </product>
// <license type="opensource" name="LGPL" version="2 or later">
//    This program is free software; you can redistribute it and/or modify
//    it under the terms of the GNU Lesser General Public License as published 
//    by the Free Software Foundation; either version 2 of the License, or
//    any later version.
//
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU Lesser General Public License for more details.
//
//    You should have received a copy of the GNU Lesser General Public 
//    License along with this program; if not, write to the Free Software
//    Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
// </license>
//-----------------------------------------------------------------------

using System;
using NUnit.Framework;

namespace Iridium.Test.SpecialFunctionsTests
{
    using MathNet.Numerics;

    [TestFixture]
    public class SpecialFunctionsTest
    {
        [Test]
        public void TestSpecialFunctions_IntLog2()
        {
            Assert.That(Fn.IntLog2(1), Is.EqualTo(0), "A");
            Assert.That(Fn.IntLog2(2), Is.EqualTo(1), "B");
            Assert.That(Fn.IntLog2(3), Is.EqualTo(2), "C");
            Assert.That(Fn.IntLog2(4), Is.EqualTo(2), "D");

            for(int i = 2; i < 31; i++)
            {
                int pow = (int)Math.Pow(2.0, i);
                Assert.That(Fn.IntLog2(pow), Is.EqualTo(i), pow.ToString());
                Assert.That(Fn.IntLog2(pow - 1), Is.EqualTo(i), pow.ToString() + "-1");
                Assert.That(Fn.IntLog2(pow + 1), Is.EqualTo(i + 1), pow.ToString() + "+1");
            }

            Assert.That(Fn.IntLog2(int.MaxValue), Is.EqualTo(31), "Y");
            Assert.That(Fn.IntLog2(int.MaxValue - 1), Is.EqualTo(31), "Z");
        }

        [Test]
        public void TestSpecialFunctions_FloorToPowerOf2()
        {
            Assert.That(Fn.FloorToPowerOf2(0), Is.EqualTo(0), "A");
            Assert.That(Fn.FloorToPowerOf2(1), Is.EqualTo(1), "B");
            Assert.That(Fn.FloorToPowerOf2(2), Is.EqualTo(2), "C");
            Assert.That(Fn.FloorToPowerOf2(3), Is.EqualTo(2), "D");
            Assert.That(Fn.FloorToPowerOf2(4), Is.EqualTo(4), "E");

            for(int i = 2; i < 31; i++)
            {
                int pow = (int)Math.Pow(2.0, i);
                Assert.That(Fn.FloorToPowerOf2(pow), Is.EqualTo(pow), pow.ToString());
                Assert.That(Fn.FloorToPowerOf2(pow - 1), Is.EqualTo(pow >> 1), pow.ToString() + "-1");
            }

            Assert.That(Fn.FloorToPowerOf2(int.MaxValue), Is.EqualTo((int.MaxValue >> 1) + 1), "Z");
        }

        [Test]
        public void TestSpecialFunctions_CeilingToPowerOf2()
        {
            Assert.That(Fn.CeilingToPowerOf2(0), Is.EqualTo(0), "A");
            Assert.That(Fn.CeilingToPowerOf2(1), Is.EqualTo(1), "B");
            Assert.That(Fn.CeilingToPowerOf2(2), Is.EqualTo(2), "C");
            Assert.That(Fn.CeilingToPowerOf2(3), Is.EqualTo(4), "D");
            Assert.That(Fn.CeilingToPowerOf2(4), Is.EqualTo(4), "E");

            for(int i = 2; i < 31; i++)
            {
                int pow = (int)Math.Pow(2.0, i);
                Assert.That(Fn.CeilingToPowerOf2(pow), Is.EqualTo(pow), pow.ToString());
                Assert.That(Fn.CeilingToPowerOf2(pow - 1), Is.EqualTo(pow), pow.ToString() + "-1");
            }

            Assert.That(Fn.CeilingToPowerOf2((int.MaxValue >> 1) + 1), Is.EqualTo((int.MaxValue >> 1) + 1), "Y");
            Assert.That(Fn.CeilingToPowerOf2((int.MaxValue >> 1)), Is.EqualTo((int.MaxValue >> 1) + 1), "Z");
        }

        [Test]
        public void TestSpecialFunctions_Gcd()
        {
            Assert.That(Fn.Gcd(0, 0), Is.EqualTo(0), "Gcd(0,0)");
            Assert.That(Fn.Gcd(-5, 0), Is.EqualTo(5), "Gcd(-5,0)");
            Assert.That(Fn.Gcd(0, 6), Is.EqualTo(6), "Gcd(0,6)");
            Assert.That(Fn.Gcd(0, Int32.MaxValue), Is.EqualTo(Int32.MaxValue), "Gcd(0,Int32Max)");
            Assert.That(Fn.Gcd(0, Int64.MaxValue), Is.EqualTo(Int64.MaxValue), "Gcd(0,Int64Max)");
            Assert.That(Fn.Gcd(Int32.MaxValue, Int64.MaxValue), Is.EqualTo(1), "Gcd(Int32Max,Int64Max)");
            Assert.That(Fn.Gcd(1 << 18, 1 << 20), Is.EqualTo(1 << 18), "Gcd(1>>18,1<<20)");
            Assert.That(Fn.Gcd(7, 13), Is.EqualTo(1), "Gcd(7,13)");
            Assert.That(Fn.Gcd(7, 14), Is.EqualTo(7), "Gcd(7,14)");
            Assert.That(Fn.Gcd(7, 15), Is.EqualTo(1), "Gcd(7,15)");
            Assert.That(Fn.Gcd(6, 15), Is.EqualTo(3), "Gcd(6,15)");
        }

        [Test]
        public void TestSpecialFunctions_GcdList()
        {
            Assert.That(Fn.Gcd(), Is.EqualTo(0), "Gcd()");
            Assert.That(Fn.Gcd(-100), Is.EqualTo(100), "Gcd(-100)");
            Assert.That(Fn.Gcd(-10, 6, -8), Is.EqualTo(2), "Gcd(-10,6,-8)");
            Assert.That(Fn.Gcd(-10, 6, -8, 5, 9, 13), Is.EqualTo(1), "Gcd(-10,6,-8,5,9,13)");
            Assert.That(Fn.Gcd(-10, 20, 120, 60, -15, 1000), Is.EqualTo(5), "Gcd(-10,20,120,60,-15,1000)");
            Assert.That(Fn.Gcd(Int64.MaxValue - 1, Int64.MaxValue - 4, Int64.MaxValue - 7), Is.EqualTo(3), "Gcd(Int64Max-1,Int64Max-4,Int64Max-7)");
            Assert.That(Fn.Gcd(492, -2 * 492, 492 / 4), Is.EqualTo(123), "Gcd(492, -984, 123)");
        }

        [Test]
        public void TestSpecialFunctions_Lcm()
        {
            Assert.That(Fn.Lcm(10, 10), Is.EqualTo(10), "Lcm(10,10)");
            Assert.That(Fn.Lcm(Int32.MaxValue, Int32.MaxValue), Is.EqualTo(Int32.MaxValue), "Lcm(Int32Max,Int32Max)");
            Assert.That(Fn.Lcm(Int64.MaxValue, Int64.MaxValue), Is.EqualTo(Int64.MaxValue), "Lcm(Int64Max,Int64Max)");
            Assert.That(Fn.Lcm(-Int64.MaxValue, -Int64.MaxValue), Is.EqualTo(Int64.MaxValue), "Lcm(-Int64Max,-Int64Max)");
            Assert.That(Fn.Lcm(-Int64.MaxValue, Int64.MaxValue), Is.EqualTo(Int64.MaxValue), "Lcm(-Int64Max,Int64Max)");

            Assert.That(Fn.Lcm(0, 10), Is.EqualTo(0), "Lcm(0,10)");
            Assert.That(Fn.Lcm(10, 0), Is.EqualTo(0), "Lcm(10,0)");

            Assert.That(Fn.Lcm(11, 7), Is.EqualTo(77), "Lcm(11,7)");
            Assert.That(Fn.Lcm(11, 33), Is.EqualTo(33), "Lcm(11,33)");
            Assert.That(Fn.Lcm(11, 34), Is.EqualTo(374), "Lcm(11,34)");
            Assert.That(Fn.Lcm(11, -32), Is.EqualTo(352), "Lcm(11,-32)");
        }

        [Test]
        public void TestSpecialFunctions_LcmList()
        {
            Assert.That(Fn.Lcm(), Is.EqualTo(1), "Lcm()");
            Assert.That(Fn.Lcm(-100), Is.EqualTo(100), "Lcm(-100)");
            Assert.That(Fn.Lcm(-10, 6, -8), Is.EqualTo(120), "Lcm(-10,6,-8)");
            Assert.That(Fn.Lcm(-10, 6, -8, 5, 9, 13), Is.EqualTo(4680), "Lcm(-10,6,-8,5,9,13)");
            Assert.That(Fn.Lcm(-10, 20, 120, 60, -15, 1000), Is.EqualTo(3000), "Lcm(-10,20,120,60,-15,1000)");
            Assert.That(Fn.Lcm(492, -2 * 492, 492 / 4), Is.EqualTo(984), "Lcm(492, -984, 123)");
            Assert.That(Fn.Lcm(32, 42, 36, 18), Is.EqualTo(2016), "Lcm(32,42,36,18)");
            Assert.That(Fn.Lcm(32, 42, 36, 18), Is.EqualTo(2016), "Lcm(32,42,36,18)");
        }

        [Test]
        public void TestSpecialFunctions_GammaLn_SpecialPoints()
        {
            // becomes negative in 1..2
            Assert.That(Fn.GammaLn(1), NumericIs.AlmostEqualTo(0.0), "A1");
            Assert.That(Fn.GammaLn(2), NumericIs.AlmostEqualTo(0.0), "A2");
            Assert.That(Fn.GammaLn(1.461632145), NumericIs.AlmostEqualTo(-.1214862905, 1e-9), "A3");
            Assert.That(Fn.GammaLn(1.2), NumericIs.AlmostEqualTo(-0.08537409000331584971970284), "A4");
            Assert.That(Fn.GammaLn(1.8), NumericIs.AlmostEqualTo(-0.07108387291437216698800249), "A5");

            // positive infinity at non-positive integers
            Assert.That(Fn.GammaLn(0.0), Is.EqualTo(double.PositiveInfinity), "A6");
            Assert.That(Fn.GammaLn(-1.0), Is.EqualTo(double.PositiveInfinity), "A7");
            Assert.That(Fn.GammaLn(-2.0), Is.EqualTo(double.PositiveInfinity), "A8");
            Assert.That(Fn.GammaLn(-10.0), Is.EqualTo(double.PositiveInfinity), "A9");
            Assert.That(Fn.GammaLn(-100.0), Is.EqualTo(double.PositiveInfinity), "A10");
            Assert.That(Fn.GammaLn(-100.1), Is.Not.EqualTo(double.PositiveInfinity), "A11");
            Assert.That(Fn.GammaLn(-99.9), Is.Not.EqualTo(double.PositiveInfinity), "A12");
            Assert.That(Fn.GammaLn(-100000), Is.EqualTo(double.PositiveInfinity), "A13");

            // continuous at branch points
            Assert.That(Fn.GammaLn(Number.Increment(13)), NumericIs.AlmostEqualTo(Fn.GammaLn(13)), "13+");
            Assert.That(Fn.GammaLn(Number.Decrement(13)), NumericIs.AlmostEqualTo(Fn.GammaLn(13)), "13-");
            Assert.That(Fn.GammaLn(Number.Increment(34)), NumericIs.AlmostEqualTo(Fn.GammaLn(34)), "34+");
            Assert.That(Fn.GammaLn(Number.Decrement(34)), NumericIs.AlmostEqualTo(Fn.GammaLn(34)), "34-");
        }

        [Test]
        public void TestSpecialFunctions_GammaLn_PositiveNumbers()
        {
            // Small Positive Numbers
            Assert.That(Fn.GammaLn(1e-1), NumericIs.AlmostEqualTo(2.252712651734205959869702), "A1");
            Assert.That(Fn.GammaLn(1e-2), NumericIs.AlmostEqualTo(4.599479878042021722513945), "A2");
            Assert.That(Fn.GammaLn(1e-3), NumericIs.AlmostEqualTo(6.907178885383853682512345), "A3");
            Assert.That(Fn.GammaLn(1e-4), NumericIs.AlmostEqualTo(9.210282658633962258448658), "A4");
            Assert.That(Fn.GammaLn(1e-5), NumericIs.AlmostEqualTo(11.51291969289582570742083), "A5");
            Assert.That(Fn.GammaLn(1e-6), NumericIs.AlmostEqualTo(13.81550998074943166920783), "A6");
            Assert.That(Fn.GammaLn(1e-7), NumericIs.AlmostEqualTo(16.11809559323676152264259), "A7");
            Assert.That(Fn.GammaLn(1e-8), NumericIs.AlmostEqualTo(18.42068073818020890537531), "A8");
            Assert.That(Fn.GammaLn(1e-10), NumericIs.AlmostEqualTo(23.02585092988273527369799), "A10");
            Assert.That(Fn.GammaLn(1e-15), NumericIs.AlmostEqualTo(34.53877639491068468305421), "A15");
            Assert.That(Fn.GammaLn(1e-20), NumericIs.AlmostEqualTo(46.05170185988091368035406), "A20");
            Assert.That(Fn.GammaLn(1e-50), NumericIs.AlmostEqualTo(115.1292546497022842008996), "A50");
            Assert.That(Fn.GammaLn(1e-100), NumericIs.AlmostEqualTo(230.2585092994045684017991), "A100");
            Assert.That(Fn.GammaLn(1e-150), NumericIs.AlmostEqualTo(345.3877639491068526026987), "A150");
            Assert.That(Fn.GammaLn(1e-200), NumericIs.AlmostEqualTo(460.5170185988091368035983), "A200");
            Assert.That(Fn.GammaLn(1e-300), NumericIs.AlmostEqualTo(690.7755278982137052053974), "A300");

            Assert.That(Fn.GammaLn(1.00001e-1), NumericIs.AlmostEqualTo(2.252702228029981864727227), "B1");
            Assert.That(Fn.GammaLn(1.00001e-2), NumericIs.AlmostEqualTo(4.599469822003483708382914), "B2");
            Assert.That(Fn.GammaLn(1.00001e-3), NumericIs.AlmostEqualTo(6.907168879678134113205166), "B3");
            Assert.That(Fn.GammaLn(1.00001e-5), NumericIs.AlmostEqualTo(11.51290969288810545253012), "B5");
            Assert.That(Fn.GammaLn(1.00001e-10), NumericIs.AlmostEqualTo(23.02584092993273436315149), "B10");
            Assert.That(Fn.GammaLn(1.00001e-20), NumericIs.AlmostEqualTo(46.05169185993091334702322), "B20");
            Assert.That(Fn.GammaLn(1.00001e-100), NumericIs.AlmostEqualTo(230.2584992994545680684683), "B100");
            Assert.That(Fn.GammaLn(1.00001e-300), NumericIs.AlmostEqualTo(690.7755178982637048720666), "B300");

            Assert.That(Fn.GammaLn(1.3e-1), NumericIs.AlmostEqualTo(1.978272496331759309624239), "C1");
            Assert.That(Fn.GammaLn(1.3e-2), NumericIs.AlmostEqualTo(4.335440242151057465250438), "C2");
            Assert.That(Fn.GammaLn(1.3e-3), NumericIs.AlmostEqualTo(6.644642023240026191960253), "C3");
            Assert.That(Fn.GammaLn(1.3e-5), NumericIs.AlmostEqualTo(11.25055369683808969647734), "C5");
            Assert.That(Fn.GammaLn(1.3e-10), NumericIs.AlmostEqualTo(22.76348666539792775172112), "C10");
            Assert.That(Fn.GammaLn(1.3e-20), NumericIs.AlmostEqualTo(45.78933759541342262831683), "C20");
            Assert.That(Fn.GammaLn(1.3e-100), NumericIs.AlmostEqualTo(229.9961450349370773497636), "C100");
            Assert.That(Fn.GammaLn(1.3e-300), NumericIs.AlmostEqualTo(690.5131636337462141533619), "C300");

            // Large Positive Numbers
            Assert.That(Fn.GammaLn(1e+1), NumericIs.AlmostEqualTo(12.80182748008146961120772), "D1");
            Assert.That(Fn.GammaLn(1e+2), NumericIs.AlmostEqualTo(359.1342053695753987760440), "D2");
            Assert.That(Fn.GammaLn(1e+3), NumericIs.AlmostEqualTo(5905.220423209181211826077), "D3");
            Assert.That(Fn.GammaLn(1e+4), NumericIs.AlmostEqualTo(82099.71749644237727264896), "D4");
            Assert.That(Fn.GammaLn(1e+5), NumericIs.AlmostEqualTo(1.051287708973656894900858e+6), "D5");
            Assert.That(Fn.GammaLn(1e+6), NumericIs.AlmostEqualTo(1.281550456914761165997697e+7), "D6");
            Assert.That(Fn.GammaLn(1e+7), NumericIs.AlmostEqualTo(1.511809493694739139401056e+8), "D7");
            Assert.That(Fn.GammaLn(1e+8), NumericIs.AlmostEqualTo(1.742068066103834709276216e+9), "D8");
            Assert.That(Fn.GammaLn(1e+10), NumericIs.AlmostEqualTo(2.202585092888105814700419e+11), "D10");
            Assert.That(Fn.GammaLn(1e+15), NumericIs.AlmostEqualTo(3.353877639491066890982021e+16), "D15");
            Assert.That(Fn.GammaLn(1e+20), NumericIs.AlmostEqualTo(4.505170185988091368013876e+21), "D20");
            Assert.That(Fn.GammaLn(1e+50), NumericIs.AlmostEqualTo(1.141292546497022842008996e+52), "D50");
            Assert.That(Fn.GammaLn(1e+100), NumericIs.AlmostEqualTo(2.292585092994045684017991e+102), "D100");
            Assert.That(Fn.GammaLn(1e+150), NumericIs.AlmostEqualTo(3.443877639491068526026987e+152), "D150");
            Assert.That(Fn.GammaLn(1e+200), NumericIs.AlmostEqualTo(4.595170185988091368035983e+202), "D200");
            Assert.That(Fn.GammaLn(1e+300), NumericIs.AlmostEqualTo(6.897755278982137052053974e+302), "D300");

            Assert.That(Fn.GammaLn(1.00001e+1), NumericIs.AlmostEqualTo(12.80205265586620612009742), "E1");
            Assert.That(Fn.GammaLn(1.00001e+2), NumericIs.AlmostEqualTo(359.1388055364532033610289), "E2");
            Assert.That(Fn.GammaLn(1.00001e+3), NumericIs.AlmostEqualTo(5905.289495811162541447347), "E3");
            Assert.That(Fn.GammaLn(1.00001e+5), NumericIs.AlmostEqualTo(1.051299221899121865129278e+6), "E5");
            Assert.That(Fn.GammaLn(1.00001e+10), NumericIs.AlmostEqualTo(2.202608118744035688490926e+11), "E10");
            Assert.That(Fn.GammaLn(1.00001e+20), NumericIs.AlmostEqualTo(4.505216237694951232260973e+21), "E20");
            Assert.That(Fn.GammaLn(1.00001e+100), NumericIs.AlmostEqualTo(2.292608118845475622808173e+102), "E100");
            Assert.That(Fn.GammaLn(1.00001e+300), NumericIs.AlmostEqualTo(6.897824356535426871757837e+302), "E300");

            Assert.That(Fn.GammaLn(1.3e+1), NumericIs.AlmostEqualTo(19.98721449566188614951736), "E1");
            Assert.That(Fn.GammaLn(1.3e+2), NumericIs.AlmostEqualTo(501.2652908915792927796609), "E2");
            Assert.That(Fn.GammaLn(1.3e+3), NumericIs.AlmostEqualTo(8018.489349348559232219987), "E3");
            Assert.That(Fn.GammaLn(1.3e+5), NumericIs.AlmostEqualTo(1.400782696121213042830339e+6), "E5");
            Assert.That(Fn.GammaLn(1.3e+10), NumericIs.AlmostEqualTo(2.897467975165781535348074e+11), "E10");
            Assert.That(Fn.GammaLn(1.3e+20), NumericIs.AlmostEqualTo(5.890828596165292615189154e+21), "E20");
            Assert.That(Fn.GammaLn(1.3e+100), NumericIs.AlmostEqualTo(2.983771356330336772899850e+102), "E100");
            Assert.That(Fn.GammaLn(1.3e+300), NumericIs.AlmostEqualTo(8.970492598114855551346628e+302), "E300");
        }

        [Test]
        public void TestSpecialFunctions_GammaLn_NegativeNumbers()
        {
            // Small Negative Numbers
            Assert.That(Fn.GammaLn(-1e-1), NumericIs.AlmostEqualTo(2.368961332728788655206708), "A1");
            Assert.That(Fn.GammaLn(-1e-2), NumericIs.AlmostEqualTo(4.611024992752801144215290), "A2");
            Assert.That(Fn.GammaLn(-1e-3), NumericIs.AlmostEqualTo(6.908333317515028431778011), "A3");
            Assert.That(Fn.GammaLn(-1e-4), NumericIs.AlmostEqualTo(9.210398101767743936293700), "A4");
            Assert.That(Fn.GammaLn(-1e-5), NumericIs.AlmostEqualTo(11.51293123720912453944932), "A5");
            Assert.That(Fn.GammaLn(-1e-6), NumericIs.AlmostEqualTo(13.81551113518076147307492), "A6");
            Assert.That(Fn.GammaLn(-1e-7), NumericIs.AlmostEqualTo(16.11809570867989450294996), "A7");
            Assert.That(Fn.GammaLn(-1e-8), NumericIs.AlmostEqualTo(18.42068074972452220340596), "A8");
            Assert.That(Fn.GammaLn(-1e-10), NumericIs.AlmostEqualTo(23.02585092999817840667829), "A10");
            Assert.That(Fn.GammaLn(-1e-15), NumericIs.AlmostEqualTo(34.53877639491068583748554), "A15");
            Assert.That(Fn.GammaLn(-1e-20), NumericIs.AlmostEqualTo(46.05170185988091368036560), "A20");
            Assert.That(Fn.GammaLn(-1e-50), NumericIs.AlmostEqualTo(115.1292546497022842008996), "A50");
            Assert.That(Fn.GammaLn(-1e-100), NumericIs.AlmostEqualTo(230.2585092994045684017991), "A100");
            Assert.That(Fn.GammaLn(-1e-150), NumericIs.AlmostEqualTo(345.3877639491068526026987), "A150");
            Assert.That(Fn.GammaLn(-1e-200), NumericIs.AlmostEqualTo(460.5170185988091368035983), "A200");
            Assert.That(Fn.GammaLn(-1e-300), NumericIs.AlmostEqualTo(690.7755278982137052053974), "A300");

            Assert.That(Fn.GammaLn(-1.00001e-1), NumericIs.AlmostEqualTo(2.368952087706699539440639), "B1");
            Assert.That(Fn.GammaLn(-1.00001e-2), NumericIs.AlmostEqualTo(4.611015052181439562964888), "B2");
            Assert.That(Fn.GammaLn(-1.00001e-3), NumericIs.AlmostEqualTo(6.908323323353646201900704), "B3");
            Assert.That(Fn.GammaLn(-1.00001e-5), NumericIs.AlmostEqualTo(11.51292123731684741756295), "B5");
            Assert.That(Fn.GammaLn(-1.00001e-10), NumericIs.AlmostEqualTo(23.02584093004817865056312), "B10");
            Assert.That(Fn.GammaLn(-1.00001e-20), NumericIs.AlmostEqualTo(46.05169185993091334703477), "B20");
            Assert.That(Fn.GammaLn(-1.00001e-100), NumericIs.AlmostEqualTo(230.2584992994545680684683), "B100");
            Assert.That(Fn.GammaLn(-1.00001e-300), NumericIs.AlmostEqualTo(690.7755178982637048720666), "B300");

            Assert.That(Fn.GammaLn(-1.3e-1), NumericIs.AlmostEqualTo(2.130124765217041905198028), "C1");
            Assert.That(Fn.GammaLn(-1.3e-2), NumericIs.AlmostEqualTo(4.350449610205194501037890), "C2");
            Assert.That(Fn.GammaLn(-1.3e-3), NumericIs.AlmostEqualTo(6.646142785729384394910125), "C3");
            Assert.That(Fn.GammaLn(-1.3e-5), NumericIs.AlmostEqualTo(11.25056870444537889694439), "C5");
            Assert.That(Fn.GammaLn(-1.3e-10), NumericIs.AlmostEqualTo(22.76348666554800382459552), "C10");
            Assert.That(Fn.GammaLn(-1.3e-20), NumericIs.AlmostEqualTo(45.78933759541342262833184), "C20");
            Assert.That(Fn.GammaLn(-1.3e-100), NumericIs.AlmostEqualTo(229.9961450349370773497636), "C100");
            Assert.That(Fn.GammaLn(-1.3e-300), NumericIs.AlmostEqualTo(690.5131636337462141533619), "C300");

            // Large Negative Numbers (no integers!)
            Assert.That(Fn.GammaLn(0.2 - 1e+1), NumericIs.AlmostEqualTo(-12.95985406357513604232074), "C1");
            Assert.That(Fn.GammaLn(0.2 - 1e+2), NumericIs.AlmostEqualTo(-361.1414188196566873672899), "C2");
            Assert.That(Fn.GammaLn(0.2 - 1e+3), NumericIs.AlmostEqualTo(-5909.070423939538431502831), "C3");
            Assert.That(Fn.GammaLn(0.2 - 1e+4), NumericIs.AlmostEqualTo(-82105.40963723920759885737), "C4");
            Assert.That(Fn.GammaLn(0.2 - 1e+5), NumericIs.AlmostEqualTo(-1.051295243189728041261483e+6), "C5");
            Assert.That(Fn.GammaLn(0.2 - 1e+6), NumericIs.AlmostEqualTo(-1.281551394543247720078214e+7), "C6");
            Assert.That(Fn.GammaLn(0.2 - 1e+7), NumericIs.AlmostEqualTo(-1.511809605878269258761394e+8), "C7");
            Assert.That(Fn.GammaLn(0.2 - 1e+8), NumericIs.AlmostEqualTo(-1.742068079164255802807487e+9), "C8");

            Assert.That(Fn.GammaLn(0.9999999 - 1e+1), NumericIs.AlmostEqualTo(3.316267945701607193755103, 1e-8), "D1");
            Assert.That(Fn.GammaLn(0.9999999 - 1e+2), NumericIs.AlmostEqualTo(-343.0161101786332478626370, 1e-9), "D2");
            Assert.That(Fn.GammaLn(0.9999999 - 1e+3), NumericIs.AlmostEqualTo(-5889.102328248948395158494, 1e-10), "D3");
            Assert.That(Fn.GammaLn(0.999 - 1e+4), NumericIs.AlmostEqualTo(-82092.81894980888167293454, 1e-14), "D4");
            Assert.That(Fn.GammaLn(0.999 - 1e+5), NumericIs.AlmostEqualTo(-1.051280812729653448117716e+6, 1e-14), "D5");
            Assert.That(Fn.GammaLn(0.999 - 1e+6), NumericIs.AlmostEqualTo(-1.281549767520619780169610e+7, 1e-14), "D6");
            Assert.That(Fn.GammaLn(0.999 - 1e+7), NumericIs.AlmostEqualTo(-1.511809424778350856243688e+8), "D7");
        }

        [Test]
        public void TestSpecialFunctions_Gamma()
        {
            // ensure poles return NaN
            Assert.That(Fn.Gamma(0.0), Is.NaN, "A1");
            Assert.That(Fn.Gamma(-1.0), Is.NaN, "A2");
            Assert.That(Fn.Gamma(-2.0), Is.NaN, "A3");
            Assert.That(Fn.Gamma(-20.0), Is.NaN, "A4");
            Assert.That(Fn.Gamma(-20.0000000001), Is.Not.NaN, "A4b");

            // Compare Gamma with Maple: "evalf(GAMMA(x),20);"
            Assert.That(Fn.Gamma(0.001), NumericIs.AlmostEqualTo(999.42377248459546611), "B1");
            Assert.That(Fn.Gamma(0.01), NumericIs.AlmostEqualTo(99.432585119150603714), "B2");
            Assert.That(Fn.Gamma(0.1), NumericIs.AlmostEqualTo(9.5135076986687318363), "B3");
            Assert.That(Fn.Gamma(0.2), NumericIs.AlmostEqualTo(4.5908437119988030532), "B4");
            Assert.That(Fn.Gamma(0.4), NumericIs.AlmostEqualTo(2.2181595437576882231), "B5");
            Assert.That(Fn.Gamma(0.6), NumericIs.AlmostEqualTo(1.4891922488128171024), "B6");
            Assert.That(Fn.Gamma(0.9), NumericIs.AlmostEqualTo(1.0686287021193193549), "B7");
            Assert.That(Fn.Gamma(0.999), NumericIs.AlmostEqualTo(1.0005782056293586480), "B8");
            Assert.That(Fn.Gamma(1.0), NumericIs.AlmostEqualTo(1.0), "B9");
            Assert.That(Fn.Gamma(1.001), NumericIs.AlmostEqualTo(.99942377248459546611), "B10");
            Assert.That(Fn.Gamma(1.5), NumericIs.AlmostEqualTo(.88622692545275801365), "B11");
            Assert.That(Fn.Gamma(1.9), NumericIs.AlmostEqualTo(.96176583190738741941), "B12");
            Assert.That(Fn.Gamma(2.0), NumericIs.AlmostEqualTo(1.0), "B13");
            Assert.That(Fn.Gamma(10.0), NumericIs.AlmostEqualTo(362880.0), "B14");
            Assert.That(Fn.Gamma(10.51), NumericIs.AlmostEqualTo(1159686.4489708177739), "B15");
            Assert.That(Fn.Gamma(100), NumericIs.AlmostEqualTo(.93326215443944152682e156), "B16");
            Assert.That(Fn.Gamma(-0.01), NumericIs.AlmostEqualTo(-100.58719796441077919, 1e-14), "B17");
            Assert.That(Fn.Gamma(-0.1), NumericIs.AlmostEqualTo(-10.686287021193193549), "B18");
            Assert.That(Fn.Gamma(-0.5), NumericIs.AlmostEqualTo(-3.5449077018110320546), "B19");
            Assert.That(Fn.Gamma(-1.2), NumericIs.AlmostEqualTo(4.8509571405220973902), "B20");
            Assert.That(Fn.Gamma(-2.01), NumericIs.AlmostEqualTo(-49.547903041431840399, 1e-13), "B21");
            Assert.That(Fn.Gamma(-100.01), NumericIs.AlmostEqualTo(-.10234011287149294961e-155, 1e-12), "B22");
        }

        [Test]
        public void TestSpecialFunctions_GammaRegularized()
        {
            /*
            Maple: P := (a,x) -> 1 - GAMMA(a,x)/GAMMA(a)
            Mathematica: GammaRegularized[a,0,x]
            */

            // Special Points
            Assert.That(Fn.GammaRegularized(0, 0), Is.NaN, "(0,0) -> NaN");

            // x axis (a=0)
            Assert.That(Fn.GammaRegularized(0, 1), NumericIs.AlmostEqualTo((double) 1), "(0,1) -> 1");
            Assert.That(Fn.GammaRegularized(0, 0.5), NumericIs.AlmostEqualTo((double) 1), "(0,1/2) -> 1");
            Assert.That(Fn.GammaRegularized(0, 0.001), NumericIs.AlmostEqualTo((double) 1), "(0,1/1000) -> 1");

            // a axis (x=0)
            Assert.That(Fn.GammaRegularized(1, 0), NumericIs.AlmostEqualTo((double) 0), "(1,0) -> 0");
            Assert.That(Fn.GammaRegularized(0.5, 0), NumericIs.AlmostEqualTo((double) 0), "(1/2,0) -> 0");
            Assert.That(Fn.GammaRegularized(0.001, 0), NumericIs.AlmostEqualTo((double) 0), "(1/1000,0) -> 0");

            // various points (some with known other representation)
            Assert.That(Fn.GammaRegularized(1, 1), NumericIs.AlmostEqualTo(0.63212055882855767840), "(1,1) -> 1-exp(-1)");
            Assert.That(Fn.GammaRegularized(1, Math.PI), NumericIs.AlmostEqualTo(0.95678608173622775023), "(1,pi) -> 1-exp(-pi)");
            Assert.That(Fn.GammaRegularized(0.5, 1), NumericIs.AlmostEqualTo(0.84270079294971486934), "(1/2,1) -> erf(1)");
            Assert.That(Fn.GammaRegularized(0.5, 0.2), NumericIs.AlmostEqualTo(0.47291074313446191487), "(1/2,1/5) -> erf(sqrt(5)/5)");
            Assert.That(Fn.GammaRegularized(0.5, 0.4), NumericIs.AlmostEqualTo(0.62890663047730242621), "(1/2,2/5) -> erf(sqrt(10)/5)");
            Assert.That(Fn.GammaRegularized(0.5, 0.8), NumericIs.AlmostEqualTo(0.79409678926793169113), "(1/2,4/5) -> erf(sqrt(20)/5)");
            Assert.That(Fn.GammaRegularized(0.25, 0.2), NumericIs.AlmostEqualTo(0.70985103173698245837), "(1/4,1/5)");
            Assert.That(Fn.GammaRegularized(0.5, 20d), NumericIs.AlmostEqualTo(0.99999999974603714105), "(1/2,20) -> erf(2*5^(1/2))");
        }

        [Test]
        public void TestSpecialFunctions_GammaRegularizedInverse()
        {
            /*
            Maple: PInv := (a,y) -> RootOf(1 - GAMMA(a,x)/GAMMA(a) - y);
            Mathematica: InverseGammaRegularized[a,0,x]
            */

            // Special Points (expected value debatable)
            Assert.That(Fn.GammaRegularizedInverse(0, 0), Is.NaN, "(0,0) -> NaN");

            // a axis (y=0)
            Assert.That(Fn.GammaRegularizedInverse(1, 0), NumericIs.AlmostEqualTo((double) 0), "(1,0) -> 0");
            Assert.That(Fn.GammaRegularizedInverse(0.5, 0), NumericIs.AlmostEqualTo((double) 0), "(1/2,0) -> 0");
            Assert.That(Fn.GammaRegularizedInverse(0.001, 0), NumericIs.AlmostEqualTo((double) 0), "(1/1000,0) -> 0");

            // shifted a axis (y=1)
            Assert.That(Fn.GammaRegularizedInverse(1, 1), Is.EqualTo(double.PositiveInfinity), "(1,1) -> +infty");
            Assert.That(Fn.GammaRegularizedInverse(0.5, 1), Is.EqualTo(double.PositiveInfinity), "(1/2,1) -> +infty");
            Assert.That(Fn.GammaRegularizedInverse(0.001, 1), Is.EqualTo(double.PositiveInfinity), "(1/1000,1) -> +infty");

            // shifted a axis (y=1.1)
            Assert.That(Fn.GammaRegularizedInverse(1, 1.1), Is.NaN, "(1,1) -> NaN");
            Assert.That(Fn.GammaRegularizedInverse(0.5, 1.1), Is.NaN, "(1/2,1) -> NaN");
            Assert.That(Fn.GammaRegularizedInverse(0.001, 1.1), Is.NaN, "(1/1000,1) -> NaN");

            // y axis (a=0)
            Assert.That(Fn.GammaRegularizedInverse(0, 1), Is.NaN, "(0,1) -> NaN");
            Assert.That(Fn.GammaRegularizedInverse(0, 0.001), Is.NaN, "(0,1/1000) -> NaN");

            // various points (some with known other representation)
            Assert.That(Fn.GammaRegularizedInverse(1, 0.63212055882855767840), NumericIs.AlmostEqualTo((double) 1), "(1,1-exp(-1)) -> 1");
            Assert.That(Fn.GammaRegularizedInverse(1, 0.95678608173622775023), NumericIs.AlmostEqualTo(Math.PI), "(1,1-exp(-pi)) -> pi");
            Assert.That(Fn.GammaRegularizedInverse(0.5, 0.84270079294971486934), NumericIs.AlmostEqualTo((double) 1), "(1/2,erf(1)) -> 1");
            Assert.That(Fn.GammaRegularizedInverse(0.5, 0.47291074313446191487), NumericIs.AlmostEqualTo(0.2), "(1/2,erf(sqrt(1/5))) -> 1/5");
            Assert.That(Fn.GammaRegularizedInverse(0.5, 0.62890663047730242621), NumericIs.AlmostEqualTo(0.4), "(1/2,erf(sqrt(2/5))) -> 2/5");
            Assert.That(Fn.GammaRegularizedInverse(0.5, 0.79409678926793169113), NumericIs.AlmostEqualTo(0.8), "(1/2,erf(sqrt(8/5))) -> 4/5");
            Assert.That(Fn.GammaRegularizedInverse(0.25, 0.70985103173698245837), NumericIs.AlmostEqualTo(0.2), "(1/4,?) -> 4/5");
            Assert.That(Fn.GammaRegularizedInverse(0.5, 0.99999225578356895592), NumericIs.AlmostEqualTo((double) 10, 1e-12), "(1/2,erf(sqrt(10))) -> 10");
            Assert.That(Fn.GammaRegularizedInverse(0.5, 0.99999999974603714105), NumericIs.AlmostEqualTo((double) 20, 1e-8), "(1/2,erf(sqrt(20))) -> 20");
            Assert.That(Fn.GammaRegularizedInverse(0.5, 0.999), NumericIs.AlmostEqualTo(5.4137830853313661466), "(1/2,0.999)");
            Assert.That(Fn.GammaRegularizedInverse(0.5, 0.8), NumericIs.AlmostEqualTo(0.82118720757490819339), "(1/2,0.8)");
            Assert.That(Fn.GammaRegularizedInverse(0.5, 0.6), NumericIs.AlmostEqualTo(0.35416315040039690443), "(1/2,0.6)");
            Assert.That(Fn.GammaRegularizedInverse(0.5, 0.2), NumericIs.AlmostEqualTo(0.032092377333650790123), "(1/2,0.2)");
        }

        [Test]
        public void TestSpecialFunctions_Digamma()
        {
            // ensure poles return NaN
            Assert.That(Fn.Digamma(0.0), Is.NaN, "A1");
            Assert.That(Fn.Digamma(-1.0), Is.NaN, "A2");
            Assert.That(Fn.Digamma(-2.0), Is.NaN, "A3");
            Assert.That(Fn.Digamma(-20.0), Is.NaN, "A4");
            Assert.That(Fn.Digamma(-20.0000000001), Is.Not.NaN, "A4b");

            // Compare Gamma with Maple: "evalf(Psi(x),20);"
            Assert.That(Fn.Digamma(0.001), NumericIs.AlmostEqualTo(-1000.5755719318103005), "B1");
            Assert.That(Fn.Digamma(0.01), NumericIs.AlmostEqualTo(-100.56088545786867450), "B2");
            Assert.That(Fn.Digamma(0.1), NumericIs.AlmostEqualTo(-10.423754940411076795), "B3");
            Assert.That(Fn.Digamma(0.2), NumericIs.AlmostEqualTo(-5.2890398965921882955), "B4");
            Assert.That(Fn.Digamma(0.4), NumericIs.AlmostEqualTo(-2.5613845445851161457), "B5");
            Assert.That(Fn.Digamma(0.6), NumericIs.AlmostEqualTo(-1.5406192138931904148), "B6");
            Assert.That(Fn.Digamma(0.9), NumericIs.AlmostEqualTo(-.75492694994705139189), "B7");
            Assert.That(Fn.Digamma(0.999), NumericIs.AlmostEqualTo(-.57886180210864542646), "B8");
            Assert.That(Fn.Digamma(1.0), NumericIs.AlmostEqualTo(-.57721566490153286061), "B9");
            Assert.That(Fn.Digamma(1.001), NumericIs.AlmostEqualTo(-.57557193181030047147, 1e-14), "B10");
            Assert.That(Fn.Digamma(1.5), NumericIs.AlmostEqualTo(.36489973978576520559e-1, 1e-14), "B11");
            Assert.That(Fn.Digamma(1.9), NumericIs.AlmostEqualTo(.35618416116405971922), "B12");
            Assert.That(Fn.Digamma(2.0), NumericIs.AlmostEqualTo(.42278433509846713939), "B13");
            Assert.That(Fn.Digamma(10.0), NumericIs.AlmostEqualTo(2.2517525890667211076), "B14");
            Assert.That(Fn.Digamma(10.51), NumericIs.AlmostEqualTo(2.3039997054324985520), "B15");
            Assert.That(Fn.Digamma(100), NumericIs.AlmostEqualTo(4.6001618527380874002), "B16");
            Assert.That(Fn.Digamma(-0.01), NumericIs.AlmostEqualTo(99.406213695944404856), "B17");
            Assert.That(Fn.Digamma(-0.1), NumericIs.AlmostEqualTo(9.2450730500529486081), "B18");
            Assert.That(Fn.Digamma(-0.5), NumericIs.AlmostEqualTo(.36489973978576520559e-1, 1e-14), "B19");
            Assert.That(Fn.Digamma(-1.2), NumericIs.AlmostEqualTo(4.8683247666271948739), "B20");
            Assert.That(Fn.Digamma(-2.01), NumericIs.AlmostEqualTo(100.89382514365634023, 1e-13), "B21");
            Assert.That(Fn.Digamma(-100.01), NumericIs.AlmostEqualTo(104.57736050326787844, 1e-12), "B22");
        }

        [Test]
        public void TestSpecialFunctions_Erf()
        {
            // Compare Erf with Maple: "evalf(erf(x),20);"
            Assert.That(Fn.Erf(0.0), NumericIs.AlmostEqualTo(.0), "A1");
            Assert.That(Fn.Erf(0.1), NumericIs.AlmostEqualTo(.11246291601828489220), "A2");
            Assert.That(Fn.Erf(0.2), NumericIs.AlmostEqualTo(.22270258921047845414), "A3");
            Assert.That(Fn.Erf(0.3), NumericIs.AlmostEqualTo(.32862675945912742764), "A4");
            Assert.That(Fn.Erf(0.4), NumericIs.AlmostEqualTo(.42839235504666845510), "A5");
            Assert.That(Fn.Erf(0.5), NumericIs.AlmostEqualTo(.52049987781304653768), "A6");
            Assert.That(Fn.Erf(0.6), NumericIs.AlmostEqualTo(.60385609084792592256), "A7");
            Assert.That(Fn.Erf(0.7), NumericIs.AlmostEqualTo(.67780119383741847298), "A8");
            Assert.That(Fn.Erf(0.8), NumericIs.AlmostEqualTo(.74210096470766048617), "A9");
            Assert.That(Fn.Erf(0.9), NumericIs.AlmostEqualTo(.79690821242283212852), "A10");
            Assert.That(Fn.Erf(1.0), NumericIs.AlmostEqualTo(.84270079294971486934), "A11");
            Assert.That(Fn.Erf(1.1), NumericIs.AlmostEqualTo(.88020506957408169977), "A12");
            Assert.That(Fn.Erf(1.2), NumericIs.AlmostEqualTo(.91031397822963538024), "A13");
            Assert.That(Fn.Erf(3.0), NumericIs.AlmostEqualTo(.99997790950300141456), "A14");
            Assert.That(Fn.Erf(9.0), NumericIs.AlmostEqualTo(1.0), "A15");
            Assert.That(Fn.Erf(100), NumericIs.AlmostEqualTo(1.0), "A16");
            Assert.That(Fn.Erf(-0.3), NumericIs.AlmostEqualTo(-.32862675945912742764), "A17");
            Assert.That(Fn.Erf(-0.8), NumericIs.AlmostEqualTo(-.74210096470766048617), "A18");

            // Compare ErvInverse with Maple: "erfinv := y -> RootOf(-erf(_Z)+y); evalf(erfinv(x),20);"
            Assert.That(Fn.ErfInverse(0.0), NumericIs.AlmostEqualTo(.0), "B1");
            Assert.That(Fn.ErfInverse(0.1), NumericIs.AlmostEqualTo(.88855990494257687016e-1, 1e-9), "B2");
            Assert.That(Fn.ErfInverse(0.2), NumericIs.AlmostEqualTo(.17914345462129167649, 1e-8), "B3");
            Assert.That(Fn.ErfInverse(0.3), NumericIs.AlmostEqualTo(.27246271472675435562, 1e-9), "B4");
            Assert.That(Fn.ErfInverse(0.4), NumericIs.AlmostEqualTo(.37080715859355792906, 1e-8), "B5");
            Assert.That(Fn.ErfInverse(0.5), NumericIs.AlmostEqualTo(.47693627620446987338, 1e-9), "B6");
            Assert.That(Fn.ErfInverse(0.6), NumericIs.AlmostEqualTo(.59511608144999485002, 1e-8), "B7");
            Assert.That(Fn.ErfInverse(0.7), NumericIs.AlmostEqualTo(.73286907795921685222, 1e-8), "B8");
            Assert.That(Fn.ErfInverse(0.8), NumericIs.AlmostEqualTo(.90619380243682322007, 1e-8), "B9");
            Assert.That(Fn.ErfInverse(0.9), NumericIs.AlmostEqualTo(1.1630871536766740867, 1e-8), "B10");
            Assert.That(Fn.ErfInverse(0.9999), NumericIs.AlmostEqualTo(2.7510639057120607961, 1e-8), "B11");
            Assert.That(Fn.ErfInverse(0.9999999), NumericIs.AlmostEqualTo(3.7665625815708380738, 1e-8), "B12");
            Assert.That(Fn.ErfInverse(-0.3), NumericIs.AlmostEqualTo(-.27246271472675435562, 1e-9), "B13");
            Assert.That(Fn.ErfInverse(-0.8), NumericIs.AlmostEqualTo(-.90619380243682322007, 1e-8), "B14");
            Assert.That(Fn.ErfInverse(0.001), NumericIs.AlmostEqualTo(.88622715746655210457e-3, 1e-8), "B15");
            Assert.That(Fn.ErfInverse(0.005), NumericIs.AlmostEqualTo(.44311636293707267099e-2, 1e-8), "B16");
        }

        [Test]
        public void TestSpecialFunctions_Beta()
        {
            // Symmetry:
            Assert.That(Fn.Beta(0.1, 1.0), NumericIs.AlmostEqualTo(Fn.Beta(1.0, 0.1)), "A1");
            Assert.That(Fn.Beta(0.1, 10.0), NumericIs.AlmostEqualTo(Fn.Beta(10.0, 0.1)), "A2");
            Assert.That(Fn.Beta(0.5, 1.0), NumericIs.AlmostEqualTo(Fn.Beta(1.0, 0.5)), "A3");
            Assert.That(Fn.Beta(0.5, 10.0), NumericIs.AlmostEqualTo(Fn.Beta(10.0, 0.5)), "A4");
            Assert.That(Fn.Beta(10.0, 100.0), NumericIs.AlmostEqualTo(Fn.Beta(100.0, 10.0)), "A1");

            // Compare with Maple: "evalf(Beta(0.1,x),20);", with relative accuracy
            Assert.That(Fn.Beta(0.1, 0.1), NumericIs.AlmostEqualTo(19.714639489050161663), "B1");
            Assert.That(Fn.Beta(0.1, 0.2), NumericIs.AlmostEqualTo(14.599371492764829943), "B2");
            Assert.That(Fn.Beta(0.1, 0.3), NumericIs.AlmostEqualTo(12.830598536321300437), "B3");
            Assert.That(Fn.Beta(0.1, 1.0), NumericIs.AlmostEqualTo(10.0), "B4");
            Assert.That(Fn.Beta(0.1, 2.0), NumericIs.AlmostEqualTo(9.0909090909090909091), "B5");
            Assert.That(Fn.Beta(0.1, 5.0), NumericIs.AlmostEqualTo(8.1743590791584497328), "B6");
            Assert.That(Fn.Beta(0.1, 10.0), NumericIs.AlmostEqualTo(7.5913800009109903433), "B7");
            Assert.That(Fn.Beta(0.1, 100.0), NumericIs.AlmostEqualTo(6.0053229390929389725, 1e-12), "B8");

            // Compare with Maple: "evalf(Beta(25.0,x),20);", with relative accuracy
            Assert.That(Fn.Beta(25.0, 0.1), NumericIs.AlmostEqualTo(6.9076854432998202098, 1e-13), "C1");
            Assert.That(Fn.Beta(25.0, 0.2), NumericIs.AlmostEqualTo(2.4193558279880311532, 1e-14), "C2");
            Assert.That(Fn.Beta(25.0, 0.3), NumericIs.AlmostEqualTo(1.1437887414566949564, 1e-14), "C3");
            Assert.That(Fn.Beta(25.0, 1.0), NumericIs.AlmostEqualTo(.40000000000000000000e-1, 1e-14), "C4");
            Assert.That(Fn.Beta(25.0, 2.0), NumericIs.AlmostEqualTo(.15384615384615384615e-2, 1e-14), "C5");
            Assert.That(Fn.Beta(25.0, 5.0), NumericIs.AlmostEqualTo(.16841396151740979327e-5, 1e-13), "C6");
            Assert.That(Fn.Beta(25.0, 10.0), NumericIs.AlmostEqualTo(.76261281522028757519e-9, 1e-13), "C7");
            Assert.That(Fn.Beta(25.0, 100.0), NumericIs.AlmostEqualTo(.38445319996184968535e-27, 1e-13), "C8");
        }

        [Test]
        public void TestSpecialFunctions_BetaRegularized()
        {
            // Maple: Ix := (x,a,b) -> int(t^(a-1)*(1-t)^(b-1),t=0..x)/Beta(a,b);

            // Compare with Maple: "evalf(Ix(x,0.2,0.2),20);", with relative accuracy
            Assert.That(Fn.BetaRegularized(0.2, 0.2, 0.0), NumericIs.AlmostEqualTo(0.0), "A1");
            Assert.That(Fn.BetaRegularized(0.2, 0.2, 0.2), NumericIs.AlmostEqualTo(.39272216435257082965), "A2");
            Assert.That(Fn.BetaRegularized(0.2, 0.2, 0.5), NumericIs.AlmostEqualTo(.50000000000000000000), "A3");
            Assert.That(Fn.BetaRegularized(0.2, 0.2, 0.8), NumericIs.AlmostEqualTo(.60727783564742917036), "A4");
            Assert.That(Fn.BetaRegularized(0.2, 0.2, 1.0), NumericIs.AlmostEqualTo(1.0000000000000000000), "A5");

            // Compare with Maple: "evalf(Ix(x,0.6,1.2),20);", with relative accuracy
            Assert.That(Fn.BetaRegularized(0.6, 1.2, 0.0), NumericIs.AlmostEqualTo(0.0), "B1");
            Assert.That(Fn.BetaRegularized(0.6, 1.2, 0.2), NumericIs.AlmostEqualTo(.42540331997033591754), "B2");
            Assert.That(Fn.BetaRegularized(0.6, 1.2, 0.5), NumericIs.AlmostEqualTo(.71641011564425207256), "B3");
            Assert.That(Fn.BetaRegularized(0.6, 1.2, 0.8), NumericIs.AlmostEqualTo(.91373194998181983314), "B4");
            Assert.That(Fn.BetaRegularized(0.6, 1.2, 1.0), NumericIs.AlmostEqualTo(1.0000000000000000000), "B5");

            // Compare with Maple: "evalf(Ix(x,7.0,1.2),20);", with relative accuracy
            Assert.That(Fn.BetaRegularized(7.0, 1.2, 0.0), NumericIs.AlmostEqualTo(0.0), "C1");
            Assert.That(Fn.BetaRegularized(7.0, 1.2, 0.2), NumericIs.AlmostEqualTo(.20126888449347947608e-4), "C2");
            Assert.That(Fn.BetaRegularized(7.0, 1.2, 0.5), NumericIs.AlmostEqualTo(.11371092280417448678e-1), "C3");
            Assert.That(Fn.BetaRegularized(7.0, 1.2, 0.7), NumericIs.AlmostEqualTo(.11102090346884848038, 1e-14), "C4");
            Assert.That(Fn.BetaRegularized(7.0, 1.2, 0.8), NumericIs.AlmostEqualTo(.26774648551269072265, 1e-14), "C5");
            Assert.That(Fn.BetaRegularized(7.0, 1.2, 0.9), NumericIs.AlmostEqualTo(.56477467605979107895), "C6");
            Assert.That(Fn.BetaRegularized(7.0, 1.2, 0.95), NumericIs.AlmostEqualTo(.77753405618146275868), "C7");
            Assert.That(Fn.BetaRegularized(7.0, 1.2, 1.0), NumericIs.AlmostEqualTo(1.0000000000000000000), "C8");
        }

        [Test]
        public void TestSpecialFunctions_Sinc()
        {
            // Test at integers:
            for(int i = -10; i < 10; i++)
            {
                Assert.That(Fn.Sinc(i), NumericIs.AlmostEqualTo((i == 0) ? 1.0 : 0.0), "sinc(" + i.ToString() + ")");
            }
        }

        [Test]
        public void TestSpecialFunctions_Factorial()
        {
            // exact
            double factorial = 1.0;
            for(int i = 1; i < 32; i++)
            {
                factorial *= i;
                Assert.That(Fn.Factorial(i), NumericIs.AlmostEqualTo(factorial), "Factorial: " + i.ToString());
            }

            // approximation
            for(int i = 32; i < 90; i++)
            {
                factorial *= i;
                Assert.That(Fn.Factorial(i), NumericIs.AlmostEqualTo(factorial, 1e-10), "Factorial: " + i.ToString());
            }
        }

        [Test]
        public void TestSpecialFunctions_HarmonicNumber()
        {
            // exact
            double sum = 0.0;
            for(int i = 1; i < 32; i++)
            {
                sum += 1.0 / i;
                Assert.That(Fn.HarmonicNumber(i), NumericIs.AlmostEqualTo(sum), "H" + i.ToString());
            }

            // approximation
            for(int i = 32; i < 90; i++)
            {
                sum += 1.0 / i;
                Assert.That(Fn.HarmonicNumber(i), NumericIs.AlmostEqualTo(sum, 1e-10), "H" + i.ToString());
            }

            // Compare with Maple: "evalf(sum(1/k,k=1..x),20)"
            Assert.That(Fn.HarmonicNumber(100000), NumericIs.AlmostEqualTo(12.090146129863427948, 1e-10), "H100000");
            Assert.That(Fn.HarmonicNumber(100000000), NumericIs.AlmostEqualTo(18.997896413853898325, 1e-10), "H100000000");
        }
    }
}
