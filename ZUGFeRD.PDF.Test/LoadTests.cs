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
using s2industries.ZUGFeRD;
using s2industries.ZUGFeRD.PDF;

namespace s2industries.ZUGFeRD.PDF.Test
{
    [TestClass]
    public sealed class LoadTests : TestBase
    {
        [TestMethod]
        public async Task BasicLoadExampleFile()
        {
            string path = @"..\..\..\..\documentation\zugferd23en\Examples\4. EXTENDED\EXTENDED_Warenrechnung\EXTENDED_Warenrechnung.pdf";
            path = _makeSurePathIsCrossPlatformCompatible(path);
            InvoiceDescriptor desc = await InvoicePdfProcessor.LoadFromPdfAsync(path);

            Assert.IsNotNull(desc);
            Assert.AreEqual("R87654321012345", desc.InvoiceNo);
        } // !BasicLoadExampleFile()


        [TestMethod]
        public async Task BasicLoadNonExistingFile()
        {
            string path = @"doesnotexist.pdf";
            path = _makeSurePathIsCrossPlatformCompatible(path);
            await Assert.ThrowsExceptionAsync<FileNotFoundException>(() => InvoicePdfProcessor.LoadFromPdfAsync(path));
        } // !BasicLoadNonExistingFile()
    }
}
