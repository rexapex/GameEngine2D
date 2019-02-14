using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameEngine2D.AssetManagement
{
    class ProjectInfo
    {
        public string Name { get; private set; }
        public string EngineVersion { get; private set; }
        public string GameVersion { get; private set; }
        public string DateCreated { get; private set; }
        public string DateModified { get; private set; }

        public ProjectInfo(string name, string engineVersion, string gameVersion, string dateCreated, string dateModified)
        {
            Name = name;
            EngineVersion = engineVersion;
            GameVersion = gameVersion;
            DateCreated = dateCreated;
            DateModified = dateModified;
        }
    }
}
