namespace Resta.UriTemplates
{
    using System.Collections.Generic;
    using System.Text;

    internal interface IUriTemplateComponent
    {
        void Resolve(StringBuilder builder, IDictionary<string, object> variables);
    }
}