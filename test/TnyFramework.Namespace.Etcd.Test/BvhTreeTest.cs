// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

// using System;
// using GF.Base;
// using NUnit.Framework;
// using TnyFramework.Navmesh.Navmesh.Collection;
//
// namespace TnyFramework.Namespace.Etcd.Test
// {
//
//     public class BvhTreeTest
//     {
//         [Test]
//         public void TestBvhTree()
//         {
//             var min = FmVector2.ZERO;
//             var max = FmVector2.Create(100);
//             var elements = new IBvhElement<int>[4];
//             elements[0] = new BvhElement<int>((Fix64) 20, (Fix64) 20, (Fix64) 40, (Fix64) 40, 1);
//             elements[1] = new BvhElement<int>((Fix64) 20, (Fix64) 20, (Fix64) 30, (Fix64) 30, 2);
//             elements[2] = new BvhElement<int>((Fix64) 80.5, (Fix64) 80.5, (Fix64) 91.5, (Fix64) 91.5, 3);
//             elements[3] = new BvhElement<int>((Fix64) 60, (Fix64) 10, (Fix64) 70, (Fix64) 30, 4);
//             
//             var tree = new BvhTree2<int>(min, max, Fix64.One);
//             tree.Create(elements, new BvhElementVisitor<int>());
//             
//             var result = tree.Query(FmVector2.Create(23, 23), FmVector2.Create(23, 23));
//             Console.WriteLine(string.Join(",", result));
//             
//             result = tree.Query(FmVector2.Create(81, 81), FmVector2.Create(82, 82));
//             Console.WriteLine(string.Join(",", result));
//             result = tree.Query(FmVector2.Create(80, 80), FmVector2.Create(80, 80));
//             Console.WriteLine(string.Join(",", result));
//             result = tree.Query(FmVector2.Create(90, 90), FmVector2.Create(90, 90));
//             Console.WriteLine(string.Join(",", result));
//             result = tree.Query(FmVector2.Create(78, 78), FmVector2.Create(78, 78));
//             Console.WriteLine(string.Join(",", result));
//             result = tree.Query(FmVector2.Create(92, 92), FmVector2.Create(92, 92));
//             Console.WriteLine(string.Join(",", result));
//             result = tree.Query(FmVector2.Create(62, 20), FmVector2.Create(65, 25));
//             Console.WriteLine(string.Join(",", result));
//         }
//     }
//
// }


