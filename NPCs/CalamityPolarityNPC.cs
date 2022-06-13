using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.NPCs
{
    public class CalamityPolarityNPC : GlobalNPC
    {
        private float curPolarity = 0;

        public override bool InstancePerEntity => true;

        public float CurPolarity
        {
            get
            {
                return curPolarity;
            }
            private set
            {
                curPolarity = value;
            }
        }
        public override void SetDefaults(NPC npc)
        {
            CurPolarity = 0;
        }

        //Applies a new polarity to the target
        public void applyPolarity(float update)
        {
            CurPolarity = update;    
        }

        public override GlobalNPC Clone(NPC npc, NPC npcClone)
        {
            CalamityPolarityNPC myClone = (CalamityPolarityNPC)base.Clone(npc, npcClone);

            myClone.curPolarity = CurPolarity;

            return myClone;
        }

        public override void PostAI(NPC npc)
        {
            //Polarity decay
            if (curPolarity < 0)
                curPolarity++;
            else if (curPolarity > 0)
                curPolarity--;
        }
    }
}
