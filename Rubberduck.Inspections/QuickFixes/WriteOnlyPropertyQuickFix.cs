using System;
using System.Linq;
using Rubberduck.Inspections.Abstract;
using Rubberduck.Inspections.Concrete;
using Rubberduck.Parsing.Grammar;
using Rubberduck.Parsing.Inspections.Abstract;
using Rubberduck.Parsing.Inspections.Resources;
using Rubberduck.Parsing.Symbols;
using Rubberduck.Parsing.VBA;

namespace Rubberduck.Inspections.QuickFixes
{
    public sealed class WriteOnlyPropertyQuickFix : QuickFixBase
    {
        private readonly RubberduckParserState _state;

        public WriteOnlyPropertyQuickFix(RubberduckParserState state)
            : base(typeof(WriteOnlyPropertyInspection))
        {
            _state = state;
        }

        public override void Fix(IInspectionResult result)
        {
            var parameters = ((IParameterizedDeclaration) result.Target).Parameters.Cast<ParameterDeclaration>().ToList();

            var signatureParams = parameters.Except(new[] {parameters.Last()}).Select(GetParamText);
            var propertyGet = "Public Property Get " + result.Target.IdentifierName + "(" + string.Join(", ", signatureParams) + ") As " +
                parameters.Last().AsTypeName + Environment.NewLine + "End Property" + Environment.NewLine + Environment.NewLine;

            var rewriter = _state.GetRewriter(result.Target);
            rewriter.InsertBefore(result.Target.Context.Start.TokenIndex, propertyGet);
        }

        public override string Description(IInspectionResult result)
        {
            return InspectionsUI.WriteOnlyPropertyQuickFix;
        }

        public override bool CanFixInProcedure => false;
        public override bool CanFixInModule => true;
        public override bool CanFixInProject => true;

        private string GetParamText(ParameterDeclaration param)
        {
            return (((VBAParser.ArgContext)param.Context).BYVAL() == null ? "ByRef " : "ByVal ") + param.IdentifierName + " As " + param.AsTypeName;
        }
    }
}
