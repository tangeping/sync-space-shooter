
namespace KBEngine
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class Avatar : AvatarBase
    {
        public Avatar()
        {

        }

        public override void __init__()
        {
            component1.onAttached();

            if (isPlayer())
            {
                //Event.registerIn("relive", this, "relive");
                //Event.registerIn("reqFrameChange", this, "reqFrameChange");

                // 触发登陆成功事件
                
                Event.fireOut("onLoginSuccessfully", new object[] { KBEngineApp.app.entity_uuid, id, this });
            }

        }

        public override void onDestroy()
        {
            if (isPlayer())
            {
                KBEngine.Event.deregisterIn(this);
            }
        }

        public override void onEnterWorld()
        {
            base.onEnterWorld();

            if (isPlayer())
            {
                Event.fireOut("onAvatarEnterWorld", new object[] { KBEngineApp.app.entity_uuid, id, this });
            }
        }


        public override void onModelIDChanged(Byte old)
        {
            Debug.Log(className + "::set_modelID: " + old + " => " + modelID); 
            //Event.fireOut("set_modelID", new object[] { this, this.modelID });
        }

        public override void onNameChanged(string old)
        {
            Debug.Log(className + "::set_name: " + old + " => " + name); 
            //Event.fireOut("set_name", new object[] { this, this.name });
        }

    }

}

