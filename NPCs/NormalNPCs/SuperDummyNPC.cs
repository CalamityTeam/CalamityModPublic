using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using System;
using ReLogic.Utilities;
using Terraria;

namespace CalamityMod.NPCs.NormalNPCs
{
    public class SuperDummyNPC : ModNPC
    {
        public override void SetStaticDefaults()
        {
            this.HideFromBestiary();
            DisplayName.SetDefault("Super Dummy");
            Main.npcFrameCount[NPC.type] = 11;
            NPCID.Sets.CantTakeLunchMoney[Type] = true;
        }

        public override void SetDefaults()
        {
            NPC.width = 18;
            NPC.height = 48;
            NPC.damage = 0;
            NPC.defense = 0;
            NPC.lifeMax = 9999999;
            NPC.HitSound = SoundID.NPCHit15;
            NPC.DeathSound = SoundID.NPCDeath2;
            NPC.value = 0f;
            NPC.knockBackResist = 0f;
            NPC.netAlways = true;
            NPC.aiStyle = 0;
        }

        public override void UpdateLifeRegen(ref int damage)
        {
            NPC.lifeRegen += 2000000;
        }

        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            return false;
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            // Dummy AI, no way
            NPC.ai[0] = hitDirection;
        }

        public override void FindFrame(int frameHeight)
        {
            // Start animating when hit. Continue animating as long as the animation isn't finished
            if (NPC.justHit || NPC.frameCounter > 0 || (NPC.frame.Y != 0 && NPC.frame.Y != frameHeight * 4))
            {
                NPC.frameCounter += 1.0;
            }
            if (NPC.frameCounter > 6.0)
            {
                NPC.frameCounter = 0.0;
                NPC.frame.Y += frameHeight;
            }

            // Hit from behind
            if (NPC.ai[0] == -1)
            {
                if (NPC.justHit && NPC.frame.Y > frameHeight * 2)
                {
                    NPC.frame.Y = frameHeight;
                }    
                else if (NPC.frame.Y > frameHeight * 3)
                {
                    NPC.frame.Y = 0;
                }
            }
            // Hit from in front
            else
            {
                if (NPC.justHit && NPC.frame.Y > frameHeight * 7)
                {
                    NPC.frame.Y = frameHeight * 5;
                }
                else if (NPC.frame.Y > frameHeight * 10 || NPC.frame.Y < frameHeight * 4)
                {
                    NPC.frame.Y = frameHeight * 4;
                }
            }
        }

        public override bool CheckDead()
        {
            if (NPC.lifeRegen < 0)
            {
                NPC.life = NPC.lifeMax;
                return false;
            }
            return true;
        }
    }
}
