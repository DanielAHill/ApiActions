// Copyright (c) 2018 Daniel A Hill. All rights reserved.
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

namespace ApiActions
{
    public class ApiActionConfiguration
    {
        public int AbstractModelMaxDepth { get; set; } = 25;
        public string DefaultRequestVersion { get; set; } = null;
        public string VersionRouteValueKey { get; set; } = "ApiActionVersion";
        public string QueryStringVerionKey { get; set; } = "Version";
        public string AcceptAllHeaderDefaultMediaType { get; set; } = "application/json";
        public string AcceptsHeaderNotMatchedDefaultMediaType { get; set; } = "application/json";

#if DEBUG
        public bool ReturnDetailedServerErrors { get; set; } = true;
#else
// TODO: Default to DNX version of web.config debug
        public bool ReturnDetailedServerErrors { get; set; } = false;
#endif
    }
}