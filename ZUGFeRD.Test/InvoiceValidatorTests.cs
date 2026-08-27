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

namespace s2industries.ZUGFeRD.Test
{
    [TestClass]
    public class InvoiceValidatorTests
    {
        [TestMethod]
        public void ValidTaxBasisAmountIsAccepted()
        {
            InvoiceDescriptor descriptor = new InvoiceProvider().CreateInvoice();

            ValidationResult result = InvoiceValidator.Validate(descriptor, ZUGFeRDVersion.Version23);

            Assert.IsTrue(result.IsValid, string.Join(Environment.NewLine, result.Messages));
        }


        [TestMethod]
        public void MissingTaxBasisAmountIsReported()
        {
            InvoiceDescriptor descriptor = new InvoiceProvider().CreateInvoice();
            descriptor.TaxBasisAmount = null;

            ValidationResult result = InvoiceValidator.Validate(descriptor, ZUGFeRDVersion.Version23);

            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.Messages.Any(message => message.Contains("Kein TaxBasisAmount vorhanden", StringComparison.Ordinal)));
        }


        [TestMethod]
        public void InvalidTaxBasisAmountIsReported()
        {
            InvoiceDescriptor descriptor = new InvoiceProvider().CreateInvoice();
            descriptor.TaxBasisAmount = 472m;

            ValidationResult result = InvoiceValidator.Validate(descriptor, ZUGFeRDVersion.Version23);

            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.Messages.Any(message => message.Contains("taxBasisTotal", StringComparison.Ordinal)));
        }
    }
}
