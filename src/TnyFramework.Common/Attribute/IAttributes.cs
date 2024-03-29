// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using System.Collections.Generic;

namespace TnyFramework.Common.Attribute
{

    public interface IAttributes
    {
        /// <summary>
        /// 获取指定key和类型的属性
        /// </summary>
        /// <param name="key">取指定key</param>
        /// <typeparam name="T">有该属性返回该属性 否则返回null</typeparam>
        /// <returns></returns>
        T Get<T>(AttrKey<T> key);

        /// <summary>
        /// 获取指定key和类型的属性, 如果 value 为 null 返回默认值
        /// </summary>
        /// <param name="key">取指定key</param>
        /// <param name="defaultValue">默认值</param>
        /// <typeparam name="T">有该属性返回值 否则返回默认值</typeparam>
        /// <returns></returns>
        T Get<T>(AttrKey<T> key, T defaultValue);

        /// <summary>
        /// 插入指定的 key 与 value, 如果key在则返回 false, 不存在则插入value并返回true
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <typeparam name="T"></typeparam>
        /// <returns> 是否插入</returns>
        bool TryAdd<T>(AttrKey<T> key, T value);

        /// <summary>
        /// 插入指定的 key 与 value, 如果key在则返回 false, 不存在则插入value并返回true
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="supplier">值提供器</param>
        /// <typeparam name="T"></typeparam>
        /// <returns> 是否插入</returns>
        bool TryAdd<T>(AttrKey<T> key, Func<T> supplier);

        /// <summary>
        /// 插入指定的 key 与 value, 如果key在则返回 false, 不存在则插入value并返回true
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="supplier">值提供器</param>
        /// <typeparam name="T"></typeparam>
        /// <returns> 是否插入</returns>
        T Load<T>(AttrKey<T> key, Func<T> supplier);

        /// <summary>
        /// 删除指定key的属性
        /// </summary>
        /// <param name="key">指定的Key</param>
        /// <typeparam name="T"></typeparam>
        /// <returns>返回删除的值</returns>
        T Remove<T>(AttrKey<T> key);

        /// <summary>
        /// 设置key和属性
        /// </summary>
        /// <param name="key">设置的key</param>
        /// <param name="value">设置的属性</param>
        /// <typeparam name="T"></typeparam>
        void Set<T>(AttrKey<T> key, T value);

        /// <summary>
        /// 设置key和属性
        /// </summary>
        /// <param name="pair">键值对</param>
        /// <typeparam name="T"></typeparam>
        void Set<T>(IAttrPair<T> pair);

        /// <summary>
        /// 批量设置属性
        /// </summary>
        /// <param name="pairs">键值对列表</param>
        void SetAll(ICollection<IAttrPair> pairs);

        /// <summary>
        /// 批量设置属性
        /// </summary>
        /// <param name="pairs">键值对列表</param>
        void SetAll(params IAttrPair[] pairs);

        /// <summary>
        /// 删除指定key集合的属性
        /// </summary>
        /// <param name="keys">指定key集合的属性</param>
        void RemoveAll(ICollection<IAttrKey> keys);

        /// <summary>
        /// 获取所有的属性键值对
        /// </summary>
        /// <returns></returns>
        IDictionary<IAttrKey, object> AttributeMap();

        /// <summary>
        /// 删除所有的属性键值对
        /// </summary>
        void Clear();

        /// <summary>
        /// 是否为空
        /// </summary>
        bool Empty { get; }
    }

}
