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

namespace System.Data.Services.Providers
{
#region Namespaces
    using System.Linq;
#endregion

    /// <summary>
    /// This interface declares the methods required to support the $expand and $select
    /// query options for a WCF Data Service. This interface is a superset
    /// of <see cref="IExpandProvider"/> interface and adds support for paging
    /// in the expanded results and projections.
    /// </summary>
    internal interface IProjectionProvider
    {
        /// <summary>Applies expansions and projections to the specified <paramref name="source"/>.</summary>
        /// <param name="source"><see cref="IQueryable"/> object to expand and apply projections to.</param>
        /// <param name="rootProjectionNode">The root node of the tree which describes
        ///     the projections and expansions to be applied to the <paramref name="source"/>.</param>
        /// <param name="epmRelevantForCurrentFormat">Determine if the current Response format needs epm values.</param>
        /// <returns>
        /// An <see cref="IQueryable"/> object, with the results including 
        /// the expansions and projections specified in <paramref name="rootProjectionNode"/>. 
        /// </returns>
        /// <remarks>
        /// The returned <see cref="IQueryable"/> may implement the <see cref="IExpandedResult"/> interface 
        /// to provide enumerable objects for the expansions; otherwise, the expanded
        /// information is expected to be found directly in the enumerated objects. If paging is 
        /// requested by providing a non-empty list in <paramref name="rootProjectionNode"/>.OrderingInfo then
        /// it is expected that the topmost <see cref="IExpandedResult"/> would have a $skiptoken property 
        /// which will be an <see cref="IExpandedResult"/> in itself and each of it's sub-properties will
        /// be named SkipTokenPropertyXX where XX represents numbers in increasing order starting from 0. Each of 
        /// SkipTokenPropertyXX properties will be used to generated the $skiptoken to support paging.
        /// If projections are required, the provider may choose to return <see cref="IQueryable"/>
        /// which returns instances of <see cref="IProjectedResult"/>. In that case property values are determined
        /// by calling the <see cref="IProjectedResult.GetProjectedPropertyValue"/> method instead of
        /// accessing properties of the returned object directly.
        /// If both expansion and projections are required, the provider may choose to return <see cref="IQueryable"/>
        /// of <see cref="IExpandedResult"/> which in turn returns <see cref="IProjectedResult"/> from its
        /// <see cref="IExpandedResult.ExpandedElement"/> property.
        /// </remarks>
        IQueryable ApplyProjections(IQueryable source, RootProjectionNode rootProjectionNode, bool epmRelevantForCurrentFormat);
    }
}