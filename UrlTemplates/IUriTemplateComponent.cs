namespace Resta.UriTemplates
{
    using System.Collections.Generic;
    using System.Text;

    internal interface IUriTemplateComponent
    {
        void Resolve(UriTemplateBuilder builder, IDictionary<string, object> variables, bool keepUnresolved);
    }
}