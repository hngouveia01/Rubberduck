﻿using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rubberduck.Inspections.Concrete;
using Rubberduck.Inspections.QuickFixes;
using RubberduckTests.Mocks;

namespace RubberduckTests.QuickFixes
{
    [TestClass]
    public class RemoveUnassignedIdentifierQuickFixTests
    {
        [TestMethod]
        [TestCategory("QuickFixes")]
        public void UnassignedVariable_QuickFixWorks()
        {
            const string inputCode =
@"Sub Foo()
Dim var1 as Integer
End Sub";

            const string expectedCode =
@"Sub Foo()
End Sub";

            var vbe = MockVbeBuilder.BuildFromSingleStandardModule(inputCode, out var component);
            var state = MockParser.CreateAndParse(vbe.Object);

            var inspection = new VariableNotAssignedInspection(state);
            new RemoveUnassignedIdentifierQuickFix(state).Fix(inspection.GetInspectionResults().First());

            Assert.AreEqual(expectedCode, state.GetRewriter(component).GetText());
        }

        [TestMethod]
        [TestCategory("QuickFixes")]
        public void UnassignedVariable_VariableOnMultipleLines_QuickFixWorks()
        {
            const string inputCode =
@"Sub Foo()
Dim _
var1 _
as _
Integer
End Sub";

            const string expectedCode =
@"Sub Foo()
End Sub";

            var vbe = MockVbeBuilder.BuildFromSingleStandardModule(inputCode, out var component);
            var state = MockParser.CreateAndParse(vbe.Object);

            var inspection = new VariableNotAssignedInspection(state);
            new RemoveUnassignedIdentifierQuickFix(state).Fix(inspection.GetInspectionResults().First());

            Assert.AreEqual(expectedCode, state.GetRewriter(component).GetText());
        }

        [TestMethod]
        [TestCategory("QuickFixes")]
        public void UnassignedVariable_MultipleVariablesOnSingleLine_QuickFixWorks()
        {
            const string inputCode =
@"Sub Foo()
Dim var1 As Integer, var2 As Boolean
End Sub";

            // note the extra space after "Integer"--the VBE will remove it
            const string expectedCode =
@"Sub Foo()
Dim var1 As Integer
End Sub";

            var vbe = MockVbeBuilder.BuildFromSingleStandardModule(inputCode, out var component);
            var state = MockParser.CreateAndParse(vbe.Object);

            var inspection = new VariableNotAssignedInspection(state);
            new RemoveUnassignedIdentifierQuickFix(state).Fix(
                inspection.GetInspectionResults().Single(s => s.Target.IdentifierName == "var2"));

            Assert.AreEqual(expectedCode, state.GetRewriter(component).GetText());
        }

        [TestMethod]
        [TestCategory("QuickFixes")]
        public void UnassignedVariable_MultipleVariablesOnMultipleLines_QuickFixWorks()
        {
            const string inputCode =
@"Sub Foo()
Dim var1 As Integer, _
var2 As Boolean
End Sub";

            const string expectedCode =
@"Sub Foo()
Dim var1 As Integer
End Sub";

            var vbe = MockVbeBuilder.BuildFromSingleStandardModule(inputCode, out var component);
            var state = MockParser.CreateAndParse(vbe.Object);

            var inspection = new VariableNotAssignedInspection(state);
            new RemoveUnassignedIdentifierQuickFix(state).Fix(
                inspection.GetInspectionResults().Single(s => s.Target.IdentifierName == "var2"));

            Assert.AreEqual(expectedCode, state.GetRewriter(component).GetText());
        }
    }
}
