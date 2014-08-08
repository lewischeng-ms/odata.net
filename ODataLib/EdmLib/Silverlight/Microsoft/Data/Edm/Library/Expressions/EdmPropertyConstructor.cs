//   OData .NET Libraries ver. 5.6.2
//   Copyright (c) Microsoft Corporation. All rights reserved.
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

using Microsoft.Data.Edm.Expressions;

namespace Microsoft.Data.Edm.Library.Expressions
{
    /// <summary>
    /// Represents an EDM property constructor specified as part of a EDM record construction expression.
    /// </summary>
    public class EdmPropertyConstructor : EdmElement, IEdmPropertyConstructor
    {
        private readonly string name;
        private readonly IEdmExpression value;

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmPropertyConstructor"/> class.
        /// </summary>
        /// <param name="name">Property name.</param>
        /// <param name="value">Property value.</param>
        public EdmPropertyConstructor(string name, IEdmExpression value)
        {
            EdmUtil.CheckArgumentNull(name, "name");
            EdmUtil.CheckArgumentNull(value, "value");

            this.name = name;
            this.value = value;
        }

        /// <summary>
        /// Gets the name of the property.
        /// </summary>
        public string Name
        {
            get { return this.name; }
        }

        /// <summary>
        /// Gets the expression for the value of the property.
        /// </summary>
        public IEdmExpression Value
        {
            get { return this.value; }
        }
    }
}