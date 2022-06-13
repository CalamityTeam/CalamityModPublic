using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using CalamityMod.Particles;
using CalamityMod.Items.Weapons.Ranged;

namespace CalamityMod.NPCs
{
    public class CalamityPolarityNPC : GlobalNPC
    {
        private float curPolarity = 0;
        public bool isPolarized => curPolarity != 0;

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
            //Round the polarity to avoid ending up with a polarity that's got decimals which will cause it to forever alternate between positive and negative.
            CurPolarity = (float)Math.Round(update);    
        }

        public override GlobalNPC Clone(NPC npc, NPC npcClone)
        {
            CalamityPolarityNPC myClone = (CalamityPolarityNPC)base.Clone(npc, npcClone);

            myClone.curPolarity = CurPolarity;

            return myClone;
        }

        public override void PostAI(NPC npc)
        {
            if (!isPolarized)
                return;

            //Polarity decay
            if (curPolarity < 0)
                curPolarity++;
            else if (curPolarity > 0)
                curPolarity--;

            //Polarity particles
            if (Main.rand.NextBool(2))
            {
                Color sparkColor = AdamantiteParticleAccelerator.LightColors[CurPolarity < 0 ? 1 : 0];
                Particle spark = new ElectricSpark(npc.Center + Main.rand.NextVector2Circular(npc.width / 2f, npc.height / 2f), Main.rand.NextVector2CircularEdge(20f, 20f) * 0.4f, Color.Lerp(Color.White, sparkColor, 0.4f), sparkColor, Main.rand.NextFloat(0.5f, 1.2f), Main.rand.Next(20, 40), bloomScale: 2.5f);
                GeneralParticleHandler.SpawnParticle(spark);
            }
        }
    }
}
