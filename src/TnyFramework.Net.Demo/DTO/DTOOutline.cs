﻿//
//  文件名称：DTOOutline.cs
//  简   述：dto概述
//  创建标识：lrg 2021/7/5
//

using System;
using System.Reflection;
namespace TnyFramework.Net.Demo.DTO
{
    public static partial class DTOOutline
    {
        /// <summary>
        /// 注册所有DTO
        /// </summary>
        public static void RegisterDTOs()
        {
            var type = typeof(DTOOutline);
            var fields = type.GetFields(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
            foreach (var filed in fields)
            {
                if (filed.FieldType != typeof(Action) || !filed.Name.StartsWith("Register", StringComparison.Ordinal))
                    continue;
                var action = (Action)filed.GetValue(type);
                action?.Invoke();
            }
        }
    }
}