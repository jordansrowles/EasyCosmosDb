using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyCosmosDb
{
    public interface IBaseCosmosModel
    {
        IDictionary<string, JToken> CatchAll { get; set; }
    }

    /// <summary>
    ///     Base model for business layer objects.
    ///     Partial class so it can be extended with more properties
    /// </summary>
    public partial class BaseCosmosModel : IBaseCosmosModel
    {
        [JsonExtensionData]
        public IDictionary<string, JToken> CatchAll { get; set; }
    }
}
