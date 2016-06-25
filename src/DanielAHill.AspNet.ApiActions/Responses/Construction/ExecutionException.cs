﻿#region Copyright
// Copyright (c) 2016 Daniel A Hill. All rights reserved.
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

using System;
using System.Collections.Generic;

namespace DanielAHill.AspNet.ApiActions.Responses.Construction
{
    internal class ExecutionException
    {
        public string Type { get; }
        public string Message { get; }
        public string StackTrace { get; }
        public KeyValuePair<object, object>[] Data { get; }

        internal ExecutionException(Exception ex)
        {
            Type = ex.GetType().FullName;
            Message = ex.Message;
            StackTrace = ex.StackTrace;

            Data = new KeyValuePair<object, object>[ex.Data.Count];
            var dataIndex = 0;
            foreach (var key in ex.Data.Keys)
            {
                Data[dataIndex] = new KeyValuePair<object, object>(key, ex.Data[key]);
                dataIndex++;
            }
        }
    }
}