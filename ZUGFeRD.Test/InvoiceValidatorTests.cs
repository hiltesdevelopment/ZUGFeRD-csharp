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
        private static InvoiceDescriptor CreateBalancedInvoice(decimal lineAllowance = 0m, decimal lineCharge = 0m,
            decimal prepaidAmount = 0m, decimal roundingAmount = 0m)
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
            decimal grandTotal = lineTotal + taxTotal;
            lineItem.LineTotalAmount = lineTotal;
            descriptor.AddApplicableTradeTax(lineTotal, 19m, taxTotal, TaxTypes.VAT, TaxCategoryCodes.S);
            descriptor.SetTotals(
                lineTotalAmount: lineTotal,
                chargeTotalAmount: 0m,
                allowanceTotalAmount: 0m,
                taxBasisAmount: lineTotal,
                taxTotalAmount: taxTotal,
                grandTotalAmount: grandTotal,
                totalPrepaidAmount: prepaidAmount,
                duePayableAmount: grandTotal - prepaidAmount + roundingAmount,
                roundingAmount: roundingAmount);
            return descriptor;
        }


        private static void AddTaxGroup(InvoiceDescriptor descriptor, string name, decimal basisAmount,
            decimal taxPercent, decimal taxAmount, TaxCategoryCodes categoryCode)
        {
            descriptor.AddTradeLineItem(
                name: name,
                netUnitPrice: basisAmount,
                unitCode: QuantityCodes.H87,
                billedQuantity: 1m,
                lineTotalAmount: basisAmount,
                taxType: TaxTypes.VAT,
                categoryCode: categoryCode,
                taxPercent: taxPercent);
            descriptor.AddApplicableTradeTax(basisAmount, taxPercent, taxAmount, TaxTypes.VAT, categoryCode);
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


        [TestMethod]
        public void ValidInvoiceWithPrepaidAmountIsAccepted()
        {
            InvoiceDescriptor descriptor = CreateBalancedInvoice(prepaidAmount: 50m);

            ValidationResult result = InvoiceValidator.Validate(descriptor, ZUGFeRDVersion.Version23);

            Assert.IsTrue(result.IsValid, string.Join(Environment.NewLine, result.Messages));
        }


        [TestMethod]
        public void ValidInvoiceWithRoundingAmountIsAccepted()
        {
            InvoiceDescriptor descriptor = CreateBalancedInvoice(roundingAmount: 0.05m);

            ValidationResult result = InvoiceValidator.Validate(descriptor, ZUGFeRDVersion.Version23);

            Assert.IsTrue(result.IsValid, string.Join(Environment.NewLine, result.Messages));
        }


        [TestMethod]
        public void MissingDuePayableAmountIsReported()
        {
            InvoiceDescriptor descriptor = CreateBalancedInvoice();
            descriptor.DuePayableAmount = null;

            ValidationResult result = InvoiceValidator.Validate(descriptor, ZUGFeRDVersion.Version23);

            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.Messages.Any(message => message.Contains("Kein DuePayableAmount vorhanden", StringComparison.Ordinal)));
        }


        [TestMethod]
        public void InvalidDuePayableAmountIsReported()
        {
            InvoiceDescriptor descriptor = CreateBalancedInvoice(prepaidAmount: 50m, roundingAmount: 0.05m);
            descriptor.DuePayableAmount = descriptor.GrandTotalAmount.Value;

            ValidationResult result = InvoiceValidator.Validate(descriptor, ZUGFeRDVersion.Version23);

            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.Messages.Any(message => message.Contains("duePayable", StringComparison.Ordinal)));
        }


        [TestMethod]
        public void DuePayableUsesDeclaredGrandTotalAmount()
        {
            InvoiceDescriptor descriptor = CreateBalancedInvoice(prepaidAmount: 50m);
            descriptor.GrandTotalAmount += 1m;

            ValidationResult result = InvoiceValidator.Validate(descriptor, ZUGFeRDVersion.Version23);

            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.Messages.Any(message => message.Contains(
                "monetarySummation.duePayable Message: Berechneter Wert ist[", StringComparison.Ordinal)));
        }


        [TestMethod]
        public void TaxAmountsAreRoundedPerTaxGroup()
        {
            InvoiceDescriptor descriptor = InvoiceDescriptor.CreateInvoice("RE-ROUND-GROUPS", new DateTime(2026, 1, 15), CurrencyCodes.EUR);
            AddTaxGroup(descriptor, "Group 19", 0.03m, 19m, 0.01m, TaxCategoryCodes.S);
            AddTaxGroup(descriptor, "Group 7", 0.08m, 7m, 0.01m, TaxCategoryCodes.S);
            AddTaxGroup(descriptor, "Group 5", 0.11m, 5m, 0.01m, TaxCategoryCodes.S);
            descriptor.SetTotals(0.22m, 0m, 0m, 0.22m, 0.03m, 0.25m, 0m, 0.25m);

            ValidationResult result = InvoiceValidator.Validate(descriptor, ZUGFeRDVersion.Version23);

            Assert.IsTrue(result.IsValid, string.Join(Environment.NewLine, result.Messages));
        }


        [TestMethod]
        public void UnroundedTaxAmountIsReported()
        {
            InvoiceDescriptor descriptor = InvoiceDescriptor.CreateInvoice("RE-ROUND-INVALID", new DateTime(2026, 1, 15), CurrencyCodes.EUR);
            AddTaxGroup(descriptor, "Unrounded group", 0.03m, 19m, 0.0057m, TaxCategoryCodes.S);
            descriptor.SetTotals(0.03m, 0m, 0m, 0.03m, 0.0057m, 0.0357m, 0m, 0.0357m);

            ValidationResult result = InvoiceValidator.Validate(descriptor, ZUGFeRDVersion.Version23);

            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.Messages.Any(message => message.Contains("BR-CO-17", StringComparison.Ordinal)));
        }


        [TestMethod]
        public void NegativeMidpointTaxAmountIsRoundedAwayFromZero()
        {
            InvoiceDescriptor descriptor = InvoiceDescriptor.CreateInvoice("RE-ROUND-NEGATIVE", new DateTime(2026, 1, 15), CurrencyCodes.EUR);
            AddTaxGroup(descriptor, "Negative midpoint", -0.025m, 20m, -0.01m, TaxCategoryCodes.S);
            descriptor.SetTotals(-0.025m, 0m, 0m, -0.025m, -0.01m, -0.035m, 0m, -0.035m);

            ValidationResult result = InvoiceValidator.Validate(descriptor, ZUGFeRDVersion.Version23);

            Assert.IsTrue(result.IsValid, string.Join(Environment.NewLine, result.Messages));
        }


        [TestMethod]
        public void NonVatTaxDoesNotAffectVatTotals()
        {
            InvoiceDescriptor descriptor = CreateBalancedInvoice();
            descriptor.AddApplicableTradeTax(200m, 5m, 10m, TaxTypes.AAA, TaxCategoryCodes.S);

            ValidationResult result = InvoiceValidator.Validate(descriptor, ZUGFeRDVersion.Version23);

            Assert.IsTrue(result.IsValid, string.Join(Environment.NewLine, result.Messages));
        }


        [TestMethod]
        public void MissingTaxTypeIsReported()
        {
            InvoiceDescriptor descriptor = CreateBalancedInvoice();
            descriptor.Taxes[0].TypeCode = null;

            ValidationResult result = InvoiceValidator.Validate(descriptor, ZUGFeRDVersion.Version23);

            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.Messages.Any(message => message.Contains("Tax type code is required", StringComparison.Ordinal)));
        }


        [TestMethod]
        public void TaxAmountDeviationsWithinSameRateAreReported()
        {
            InvoiceDescriptor descriptor = InvoiceDescriptor.CreateInvoice("RE-ROUND-CATEGORY", new DateTime(2026, 1, 15), CurrencyCodes.EUR);
            AddTaxGroup(descriptor, "Standard rate", 1m, 7m, 0.08m, TaxCategoryCodes.S);
            AddTaxGroup(descriptor, "Lower rate", 1m, 7m, 0.06m, TaxCategoryCodes.AA);
            descriptor.SetTotals(2m, 0m, 0m, 2m, 0.14m, 2.14m, 0m, 2.14m);

            ValidationResult result = InvoiceValidator.Validate(descriptor, ZUGFeRDVersion.Version23);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(2, result.Messages.Count(message => message.Contains("BR-CO-17", StringComparison.Ordinal)));
        }
    }
}
