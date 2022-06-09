using Synapse.Api;
using Synapse.Api.Plugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using VT_Api.Core.Plugin.Updater;
using VT_Api.Reflexion;

namespace MapEditor
{
    class Updater : AbstractAutoUpdater<Plugin>
    {
        public override long GithubID => 477343426;

        public override bool Prerealase => false;
    }
}
