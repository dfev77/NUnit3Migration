using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;
using System.Linq;

namespace NUnit3Migration.Processors
{
    public class TestCaseAttributeProcessor : IProcessor
    {
        public void Process(DocumentEditor editor)
        {
            foreach (var node in editor.OriginalRoot.DescendantNodes()
                .OfType<AttributeSyntax>()
                .Where(attribute =>
                    attribute.Name is IdentifierNameSyntax &&
                    ((IdentifierNameSyntax)attribute.Name).Identifier.Text == "TestCase")
                    )
            {
                var resultArgument = node.ArgumentList.Arguments
                    .FirstOrDefault(a => a.NameEquals != null && a.NameEquals.Name.Identifier.Text == "Result");
                if (resultArgument != null)
                {
                    var identifierNameSyntax = resultArgument.NameEquals.Name;
                    var syntaxToken = SyntaxFactory.Identifier(
                        identifierNameSyntax.GetLeadingTrivia(),
                        "ExpectedResult",
                        identifierNameSyntax.GetTrailingTrivia());

                    var newNode = identifierNameSyntax.WithIdentifier(syntaxToken);
                    editor.ReplaceNode(identifierNameSyntax, newNode);
                }
            }
        }
    }
}
