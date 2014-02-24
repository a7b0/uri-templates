namespace Resta.UriTemplates
{
    using System.Collections.Generic;
    using System.Text;

    internal interface IUriTemplateComponent
    {
        void Resolve(StringBuilder builder, IDictionary<string, object> variables);

        IEnumerable<IUriTemplateComponent> ResolveTemplate(IDictionary<string, object> variables);
    }
}