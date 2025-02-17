/*
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
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace s2industries.ZUGFeRD
{
    /// <summary>
    /// http://www.unece.org/trade/untdid/d00a/tred/tred5153.htm
    /// </summary>
    public enum AdditionalReferencedDocumentTypeCode
    {
        /// <summary>
        /// simple reference document
        /// </summary>
        [EnumStringValue("916")]
        ReferenceDocument = 916,

        /// <summary>
        /// an invoice which could contain items
        /// </summary>
        [EnumStringValue("130")]
        InvoiceDataSheet = 130,

        /// <summary>
        /// price and sales catalog
        /// </summary>
        [EnumStringValue("50")]
        PriceSalesCatalogueResponse = 50,
    }
}
