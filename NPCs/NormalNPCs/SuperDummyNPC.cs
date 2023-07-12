using System.IO;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.NormalNPCs
{
    public class SuperDummyNPC : ModNPC
    {
        public int deathCounter = 0;
        public override void SetStaticDefaults()
        {
            this.HideFromBestiary();
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
            NPC.aiStyle = NPCAIStyleID.FaceClosestPlayer;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(deathCounter);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            deathCounter = reader.ReadInt32();
        }

        public override bool PreAI()
        {
            if (Main.zenithWorld)
            {
                deathCounter++;
                // If you don't attack the Dummy for a minute in gfb, it becomes sentient
                if (deathCounter >= 6000)
                {
                    NPC.damage = NPC.lifeMax;
                    CalamityGlobalAI.BuffedMimicAI(NPC, Mod);
                    return false;
                }
            }
            return true;
        }

        public override void UpdateLifeRegen(ref int damage)
        {
            NPC.lifeRegen += 2000000;
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => Main.zenithWorld;

        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position) => false;

        public override void HitEffect(NPC.HitInfo hit)
        {
            // Dummy AI, no way
            NPC.ai[0] = hit.HitDirection * -NPC.direction;
            // Reset hit timer if it isn't enraged
            if (deathCounter > 0 && deathCounter < 6000)
            {
                deathCounter = 0;
            }
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
            if (NPC.ai[0] == -1 || deathCounter > 6000)
            {
                if ((NPC.justHit || deathCounter > 6000) && NPC.frame.Y > frameHeight * 2)
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
