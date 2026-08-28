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
        private static InvoiceDescriptor CreateBalancedInvoice(decimal lineAllowance = 0m, decimal lineCharge = 0m)
        {
            InvoiceDescriptor descriptor = InvoiceDescriptor.CreateInvoice("RE-4711", new DateTime(2026, 1, 15), CurrencyCodes.EUR);
            TradeLineItem lineItem = descriptor.AddTradeLineItem(
                name: "Test item",
                netUnitPrice: 100m,
                unitCode: QuantityCodes.H87,
                billedQuantity: 2m,
                lineTotalAmount: 200m,
                taxType: TaxTypes.VAT,
                categoryCode: TaxCategoryCodes.S,
                taxPercent: 19m);

            if (lineAllowance != 0m)
            {
                lineItem.AddSpecifiedTradeAllowance(CurrencyCodes.EUR, 200m, lineAllowance, "Quantity discount");
            }
            if (lineCharge != 0m)
            {
                lineItem.AddSpecifiedTradeCharge(CurrencyCodes.EUR, 200m, lineCharge, "Line charge");
            }

            decimal lineTotal = 200m - lineAllowance + lineCharge;
            decimal taxTotal = lineTotal * 19m / 100m;
            lineItem.LineTotalAmount = lineTotal;
            descriptor.AddApplicableTradeTax(lineTotal, 19m, taxTotal, TaxTypes.VAT, TaxCategoryCodes.S);
            descriptor.SetTotals(
                lineTotalAmount: lineTotal,
                chargeTotalAmount: 0m,
                allowanceTotalAmount: 0m,
                taxBasisAmount: lineTotal,
                taxTotalAmount: taxTotal,
                grandTotalAmount: lineTotal + taxTotal,
                totalPrepaidAmount: 0m,
                duePayableAmount: lineTotal + taxTotal);
            return descriptor;
        }


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


        [TestMethod]
        public void ValidInvoiceWithLineAllowanceIsAccepted()
        {
            InvoiceDescriptor descriptor = CreateBalancedInvoice(lineAllowance: 10m);

            ValidationResult result = InvoiceValidator.Validate(descriptor, ZUGFeRDVersion.Version23);

            Assert.IsTrue(result.IsValid, string.Join(Environment.NewLine, result.Messages));
        }


        [TestMethod]
        public void ValidInvoiceWithLineChargeIsAccepted()
        {
            InvoiceDescriptor descriptor = CreateBalancedInvoice(lineCharge: 10m);

            ValidationResult result = InvoiceValidator.Validate(descriptor, ZUGFeRDVersion.Version23);

            Assert.IsTrue(result.IsValid, string.Join(Environment.NewLine, result.Messages));
        }


        [TestMethod]
        public void PriceAllowanceIsNotSubtractedTwice()
        {
            InvoiceDescriptor descriptor = CreateBalancedInvoice();
            descriptor.TradeLineItems[0].AddTradeAllowance(CurrencyCodes.EUR, 110m, 10m, "Price allowance");

            ValidationResult result = InvoiceValidator.Validate(descriptor, ZUGFeRDVersion.Version23);

            Assert.IsTrue(result.IsValid, string.Join(Environment.NewLine, result.Messages));
        }
    }
}
