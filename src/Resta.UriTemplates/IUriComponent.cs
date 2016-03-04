using System.Collections.Generic;
using System.Text;

namespace Resta.UriTemplates
{
    internal interface IUriComponent
    {
        void Resolve(StringBuilder builder, IDictionary<string, object> variables);

        IEnumerable<IUriComponent> ResolveTemplate(IDictionary<string, object> variables);
    }
}