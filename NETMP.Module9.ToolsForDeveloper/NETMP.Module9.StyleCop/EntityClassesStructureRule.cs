using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using StyleCop;
using StyleCop.CSharp;

namespace NETMP.Module9.StyleCop
{
    [SourceAnalyzer(typeof(CsParser))]
    public class EntityClassesStructureRule : SourceAnalyzer
    {
        private const string RuleName = "EntityClassesStructureRule";
        private const string TargetFolderName = "Entities";

        private readonly List<string> _requiredProperties;
        private readonly List<string> _requiredAttributes;

        private readonly string _attributeRegexString;
        private readonly string _propertyCheckTemplate;

        public EntityClassesStructureRule()
        {
            _requiredProperties = new List<string> { "Id", "Name" };
            _requiredAttributes = new List<string> { "DataContract" };

            _attributeRegexString = @"\[.+{0}(Attribute)?\]";
            _propertyCheckTemplate = "property {0}";
        }

        public override void AnalyzeDocument(CodeDocument currentCodeDocument)
        {
            var codeDocument = (CsDocument)currentCodeDocument;

            if (codeDocument.RootElement != null && !codeDocument.RootElement.Generated)
            {
                codeDocument.WalkDocument(CheckEntityClass, null, null);
            }
        }

        #region Private methods

        private bool CheckEntityClass(CsElement element, CsElement parentElement, object context)
        {
            if (element.ElementType == ElementType.Class)
            {
                var namespaceComponents = element.FullNamespaceName.Split('.');

                if (namespaceComponents.Contains(TargetFolderName))
                {
                    if (!IsPublic(element) || !HasValidAttributes(element) || !HasRequiredPublicProperties(element))
                    {
                        AddViolation(element, RuleName);
                        return false;
                    }
                }
            }

            return true;
        }

        private bool HasValidAttributes(CsElement element)
        {
            return _requiredAttributes.All(required => element.Attributes.Any(attr => Regex.IsMatch(attr.Text, string.Format(_attributeRegexString, attr.Text))));
        }

        private bool HasRequiredPublicProperties(CsElement element)
        {
            return _requiredProperties.All(required => element.ChildElements.Any(elem => elem.AccessModifier == AccessModifierType.Public && elem.Name == string.Format(_propertyCheckTemplate, required)));
        }

        private bool IsPublic(CsElement element)
        {
            return element.AccessModifier == AccessModifierType.Public;
        }

        #endregion
    }
}
