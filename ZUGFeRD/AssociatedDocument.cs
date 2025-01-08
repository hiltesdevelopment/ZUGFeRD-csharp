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
using System.Threading.Tasks;

namespace s2industries.ZUGFeRD
{
    /// <summary>
    /// Representation for general information on item level
    /// </summary>
    public class AssociatedDocument
    {
        /// <summary>
        ///  Detailed information in free text form
        /// </summary>
        public List<Note> Notes { get; set; } = new List<Note>();

        /// <summary>
        /// identifier of the invoice line item
        /// </summary>
        public string LineID { get; internal set; }

        /// <summary>
        /// Refers to the superior line in a hierarchical structure. 
        /// This property is used to map a hierarchy tree of invoice items, allowing child items to reference their parent line.
        /// BT-X-304
        /// 
        /// Example usage:
        /// <code>
        /// var tradeLineItem = new TradeLineItem();
        /// tradeLineItem.SetParentLineId("1");
        /// </code>
        /// </summary>
        public string ParentLineID { get; internal set; }


        /// <summary>
        /// Initializes a new associated document object, optionally passing a certain lineID
        /// </summary>
        /// <param name="lineID"></param>
        internal AssociatedDocument(string lineID)
        {
            this.LineID = lineID;
        }

        /// <summary>
        /// Type of the invoice line item
        /// 
        /// If the LineStatusCode element is used, the LineStatusReasonCode must be filled.
        /// 
        /// BT-X-7
        /// </summary>
        public LineStatusCodes? LineStatusCode { get; internal set; } = null;

        /// <summary>
        /// Subtype of the invoice line item
        /// 
        /// BT-X-8
        /// </summary>
        public LineStatusReasonCodes? LineStatusReasonCode { get; internal set; } = null;
    }
}
