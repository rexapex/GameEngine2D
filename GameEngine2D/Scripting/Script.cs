using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameEngine2D.EntitySystem;

namespace GameEngine2D.Scripting
{
    // Script component class
    class Script : Component, IComponentInitializable, IComponentUpdatable
    {
        public string SourceFile { get; set; }
        public string ClassName { get; set;  }
        public IScript ScriptObj { get; private set; }

        public Script(Entity parent) : base(parent) {}

        public void OnScriptLoad(IScript scriptObj)
        {
            ScriptObj = scriptObj;
            if(ScriptObj != null)
            {
                ScriptObj.Initialize(parent);
            }
        }

        public void Initialize()
        {
            if(ScriptObj != null)
            {
                ScriptObj.Start();
            }
        }

        public void Update()
        {
            if(ScriptObj != null)
            {
                ScriptObj.Update();
            }
        }
    }
}
