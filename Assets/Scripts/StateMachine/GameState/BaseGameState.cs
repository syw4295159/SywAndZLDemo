using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

    public abstract class BaseGameState
    {
        public abstract int ID { get; }
        public abstract void Enter();
        public abstract void Excute();
        public abstract void Stay();
        public abstract void Leave();
    }
