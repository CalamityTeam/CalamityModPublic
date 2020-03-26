using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.NPCs.Yharon
{
    public class DetonatingFlare2 : ModNPC
    {
        float speed = 0f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Detonating Flame");
            Main.npcFrameCount[npc.type] = 5;
        }

        public override void SetDefaults()
        {
            npc.aiStyle = -1;
            aiType = -1;
            npc.damage = 220;
            npc.width = 50;
            npc.height = 50;
            npc.defense = 75;
            npc.lifeMax = 13000;
            npc.knockBackResist = 0f;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.canGhostHeal = false;
            npc.HitSound = SoundID.NPCHit52;
            npc.DeathSound = SoundID.NPCDeath55;
            npc.alpha = 255;
        }

        public override void AI()
        {
            npc.alpha -= 3;
            bool revenge = CalamityWorld.revenge || CalamityWorld.bossRushActive;
			float randomSpeed = 10f;
            if (npc.localAI[3] == 0f)
            {
                switch (Main.rand.Next(6))
                {
                    case 0:
                        randomSpeed = 10f;
                        break;
                    case 1:
                        speed = 11.5f;
                        randomSpeed = 11.5f;
                        break;
                    case 2:
                        randomSpeed = 13f;
                        break;
                    case 3:
                        randomSpeed = 14.5f;
                        break;
                    case 4:
                        randomSpeed = 16f;
                        break;
                    case 5:
                        randomSpeed = 17.5f;
                        break;
                }
                npc.localAI[3] = 1f;
            }
            float speed = randomSpeed + (revenge ? 1f : 0f);
            CalamityAI.DungeonSpiritAI(npc, mod, speed, -MathHelper.PiOver2);
        }

        public override Color? GetAlpha(Color drawColor)
        {
            return new Color(255, Main.DiscoG, 53, 0);
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            if (CalamityWorld.revenge)
            {
                player.AddBuff(ModContent.BuffType<MarkedforDeath>(), 300);
            }
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            cooldownSlot = 1;
            return true;
        }

        public override void FindFrame(int frameHeight)
        {
            npc.frameCounter += 0.15f;
            npc.frameCounter %= Main.npcFrameCount[npc.type];
            int frame = (int)npc.frameCounter;
            npc.frame.Y = frame * frameHeight;
        }

        public override bool PreNPCLoot()
        {
            return false;
        }
    }
}
