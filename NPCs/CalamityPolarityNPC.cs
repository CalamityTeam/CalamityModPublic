using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using CalamityMod.Particles;
using CalamityMod.Items.Weapons.Ranged;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace CalamityMod.NPCs
{
    public class CalamityPolarityNPC : GlobalNPC
    {
        private float curPolarity = 0;
        public List<Particle> pulses;
        private List<Particle> pulsesToClear;

        public bool isPolarized => curPolarity != 0;

        public override bool InstancePerEntity => true;

        public float CurPolarity
        {
            get
            {
                return curPolarity;
            }
            internal set
            {
                curPolarity = value;
            }
        }

        public override void SetDefaults(NPC npc)
        {
            CurPolarity = 0;
            pulses = new List<Particle>();
            pulsesToClear = new List<Particle>();

        }

        //Applies a new polarity to the target
        public void applyPolarity(float update, NPC npc)
        {
            //Round the polarity to avoid ending up with a polarity that's got decimals which will cause it to forever alternate between positive and negative.
            CurPolarity = (float)Math.Round(update);
            Color pulseColor = AdamantiteParticleAccelerator.LightColors[update < 0 ? 1 : 0];
            pulses.Add(new AuraPulseRing(pulseColor, new Vector2(Math.Max(npc.width / 156f * 1.1f, 0.25f) , 0.3f), new Vector2(Math.Max(npc.width / 156f * 1.5f, 0.4f), 0.01f), 40, npc));
        }

        public override GlobalNPC Clone(NPC npc, NPC npcClone)
        {
            CalamityPolarityNPC myClone = (CalamityPolarityNPC)base.Clone(npc, npcClone);

            myClone.curPolarity = CurPolarity;

            return myClone;
        }

        public override void PostAI(NPC npc)
        {
            HandlePulses();

            if (!isPolarized)
                return;

            //Polarity decay
            if (curPolarity < 0)
                curPolarity++;
            else if (curPolarity > 0)
                curPolarity--;

            //Polarity particles
            if (Main.rand.NextBool())
            {
                Color sparkColor = AdamantiteParticleAccelerator.LightColors[CurPolarity < 0 ? 1 : 0];
                Particle spark = new ElectricSpark(npc.Center + Main.rand.NextVector2Circular(npc.width / 2f, npc.height / 2f), Main.rand.NextVector2CircularEdge(20f, 20f) * 0.4f, Color.Lerp(Color.White, sparkColor, 0.4f), sparkColor, Main.rand.NextFloat(0.5f, 1.2f), Main.rand.Next(20, 40), bloomScale: 2.5f);
                GeneralParticleHandler.SpawnParticle(spark);
            }
        }

        public void HandlePulses()
        {
            foreach (Particle pulse in pulses)
            {
                pulse.Time++;
                pulse.Update();
                if (pulse.Time > pulse.Lifetime)
                    pulsesToClear.Add(pulse);
            }

            pulses.RemoveAll(n => pulsesToClear.Contains(n));
            pulsesToClear.Clear();
        }

        public override void PostDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            // I don't know who would be using this while also inflicting miracle blight, but in that rare case, do not draw these.
            if (npc.Calamity().miracleBlight > 0)
                return;

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

            foreach (Particle pulse in pulses)
            {
                pulse.CustomDraw(spriteBatch, npc.Center);
                pulse.CustomDraw(spriteBatch, npc.Center);
            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
        }
    }
}
