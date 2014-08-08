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

namespace Microsoft.Data.OData.VerboseJson
{
    #region Namespaces
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Text;
#if ODATALIB_ASYNC
    using System.Threading.Tasks;
#endif
    using Microsoft.Data.Edm;
    using Microsoft.Data.OData.Metadata;

    #endregion Namespaces

    /// <summary>
    /// ODataCollectionWriter for the Verbose JSON format.
    /// </summary>
    internal sealed class ODataVerboseJsonCollectionWriter : ODataCollectionWriterCore
    {
        /// <summary>
        /// The output context to write to.
        /// </summary>
        private readonly ODataVerboseJsonOutputContext verboseJsonOutputContext;

        /// <summary>
        /// The Verbose JSON collection serializer to use.
        /// </summary>
        private readonly ODataVerboseJsonCollectionSerializer verboseJsonCollectionSerializer;

        /// <summary>
        /// Constructor for creating a collection writer to use when writing operation result payloads.
        /// </summary>
        /// <param name="verboseJsonOutputContext">The output context to write to.</param>
        /// <param name="itemTypeReference">The item type of the collection being written or null if no metadata is available.</param>
        internal ODataVerboseJsonCollectionWriter(ODataVerboseJsonOutputContext verboseJsonOutputContext, IEdmTypeReference itemTypeReference)
            : base(verboseJsonOutputContext, itemTypeReference)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(verboseJsonOutputContext != null, "verboseJsonOutputContext != null");

            this.verboseJsonOutputContext = verboseJsonOutputContext;
            this.verboseJsonCollectionSerializer = new ODataVerboseJsonCollectionSerializer(this.verboseJsonOutputContext);
        }

        /// <summary>
        /// Constructor for creating a collection writer to use when writing parameter payloads.
        /// </summary>
        /// <param name="verboseJsonOutputContext">The output context to write to.</param>
        /// <param name="expectedItemType">The type reference of the expected item type or null if no expected item type exists.</param>
        /// <param name="listener">If not null, the writer will notify the implementer of the interface of relevant state changes in the writer.</param>
        internal ODataVerboseJsonCollectionWriter(ODataVerboseJsonOutputContext verboseJsonOutputContext, IEdmTypeReference expectedItemType, IODataReaderWriterListener listener)
            : base(verboseJsonOutputContext, expectedItemType, listener)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(verboseJsonOutputContext != null, "verboseJsonOutputContext != null");

            this.verboseJsonOutputContext = verboseJsonOutputContext;
            this.verboseJsonCollectionSerializer = new ODataVerboseJsonCollectionSerializer(this.verboseJsonOutputContext);
        }

        /// <summary>
        /// Check if the object has been disposed; called from all public API methods. Throws an ObjectDisposedException if the object
        /// has already been disposed.
        /// </summary>
        protected override void VerifyNotDisposed()
        {
            this.verboseJsonOutputContext.VerifyNotDisposed();
        }

        /// <summary>
        /// Flush the output.
        /// </summary>
        protected override void FlushSynchronously()
        {
            this.verboseJsonOutputContext.Flush();
        }

#if ODATALIB_ASYNC
        /// <summary>
        /// Flush the output.
        /// </summary>
        /// <returns>Task representing the pending flush operation.</returns>
        protected override Task FlushAsynchronously()
        {
            return this.verboseJsonOutputContext.FlushAsync();
        }
#endif

        /// <summary>
        /// Start writing an OData payload.
        /// </summary>
        protected override void StartPayload()
        {
            this.verboseJsonCollectionSerializer.WritePayloadStart();
        }

        /// <summary>
        /// Finish writing an OData payload.
        /// </summary>
        protected override void EndPayload()
        {
            this.verboseJsonCollectionSerializer.WritePayloadEnd();
        }

        /// <summary>
        /// Start writing a collection.
        /// </summary>
        /// <param name="collectionStart">The <see cref="ODataCollectionStart"/> representing the collection.</param>
        protected override void StartCollection(ODataCollectionStart collectionStart)
        {
            this.verboseJsonCollectionSerializer.WriteCollectionStart();
        }

        /// <summary>
        /// Finish writing a collection.
        /// </summary>
        protected override void EndCollection()
        {
            this.verboseJsonCollectionSerializer.WriteCollectionEnd();
        }

        /// <summary>
        /// Writes a collection item (either primitive or complex)
        /// </summary>
        /// <param name="item">The collection item to write.</param>
        /// <param name="expectedItemType">The expected type of the collection item or null if no expected item type exists.</param>
        protected override void WriteCollectionItem(object item, IEdmTypeReference expectedItemType)
        {
            if (item == null)
            {
                ValidationUtils.ValidateNullCollectionItem(expectedItemType, this.verboseJsonOutputContext.MessageWriterSettings.WriterBehavior);
                this.verboseJsonOutputContext.JsonWriter.WriteValue(null);
            }
            else
            {
                ODataComplexValue complexValue = item as ODataComplexValue;
                if (complexValue != null)
                {
                    this.verboseJsonCollectionSerializer.AssertRecursionDepthIsZero();
                    this.verboseJsonCollectionSerializer.WriteComplexValue(
                        complexValue,
                        expectedItemType,
                        false,
                        this.DuplicatePropertyNamesChecker,
                        this.CollectionValidator);
                    this.verboseJsonCollectionSerializer.AssertRecursionDepthIsZero();
                    this.DuplicatePropertyNamesChecker.Clear();
                }
                else
                {
                    Debug.Assert(!(item is ODataCollectionValue), "!(item is ODataCollectionValue)");
                    Debug.Assert(!(item is ODataStreamReferenceValue), "!(item is ODataStreamReferenceValue)");
                    this.verboseJsonCollectionSerializer.WritePrimitiveValue(item, this.CollectionValidator, expectedItemType);
                }
            }
        }
    }
}