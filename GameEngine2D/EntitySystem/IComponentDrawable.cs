﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;

namespace GameEngine2D.EntitySystem
{
    interface IComponentDrawable
    {
        void Draw(Matrix worldViewProjMatrix);
    }
}
