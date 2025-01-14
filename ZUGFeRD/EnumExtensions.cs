﻿/*
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at
 *
 *   http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */

using System;
using System.ComponentModel;
using System.Reflection;


namespace s2industries.ZUGFeRD
{
    internal static class EnumExtensions
    {
        internal static string EnumToString<T>(this T value) where T : Enum
        {
            return value.ToString("g");
        } // !EnumToString()


        internal static T IntToEnum<T>(this int value) where T : Enum
        {
            if (Enum.IsDefined(typeof(T), value))
            {
                return (T)Enum.ToObject(typeof(T), value);
            }
            else
            {
                return default;
            }
        } // !IntToEnum()


        internal static T StringToEnum<T>(this string value) where T : Enum
        {
            try
            {
                return (T)Enum.Parse(typeof(T), value);
            }
            catch
            {
                return default;
            }
        } // !IntToEnum()


        internal static int EnumToInt<T>(this T value) where T : Enum
        {
            return (int)(object)value;
        } // !EnumToInt()


        internal static string GetDescriptionAttribute<T>(this T value) where T : Enum
        {
            FieldInfo field = value.GetType().GetField(value.ToString());
            if (field == null)
            {
                return null;
            }
            DescriptionAttribute attribute = field.GetCustomAttribute<DescriptionAttribute>();
            return attribute?.Description;
        } // !GetDescriptionAttribute()


        internal static T FromDescription<T>(string code) where T : Enum
        {
            if (string.IsNullOrEmpty(code))
            {
                return default;
            }
            foreach (T value in Enum.GetValues(typeof(T)))
            {
                var description = value.GetDescriptionAttribute();
                if (description != null && description.Equals(code, StringComparison.OrdinalIgnoreCase))
                {
                    return value;
                }
            }
            return default;
        } // !FromDescription()
    }
}
