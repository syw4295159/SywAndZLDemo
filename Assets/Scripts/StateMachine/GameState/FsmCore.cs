using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


    public class FsmCore
    {
        public BaseGameState curState;//当前状态此类是一个抽象类
        public BaseGameState prevState;
        public BaseGameState savePrevState;//上一个状态

        public void ChangeState(BaseGameState newState)
        {
            if (curState != null && curState.ID == newState.ID) return;
            if (prevState != null)
            {
                savePrevState = prevState;
                prevState.Leave();
            }
            curState = newState;
            curState.Enter();
            prevState = curState;
        }

        public void ChangePrevState()
        {
            if (savePrevState != null && curState.ID == savePrevState.ID) return;
            if (curState != null)
            {
                curState.Leave();
            }
            curState = savePrevState;
            curState.Enter();
            prevState = curState;
        }

        public void Update()
        {
            if (curState != null)
            {
                curState.Stay();
            }
        }

        public void Excute()//这个方法的作用是执行当前状态所对应的函数(行为)
        {
            if (curState != null)
            {
                curState.Excute();
            }
        }
    }

