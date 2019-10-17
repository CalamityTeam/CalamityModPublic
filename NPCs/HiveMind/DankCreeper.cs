using CalamityMod.World;
using Microsoft.Xna.Framework;
using System;
using Terraria; using CalamityMod.Projectiles; using Terraria.ModLoader;
using Terraria.ID;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles;  

namespace CalamityMod.NPCs
{
    public class DankCreeper : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Dank Creeper");
        }

        public override void SetDefaults()
        {
            npc.damage = 25;
            npc.width = 74;
            npc.height = 74;
            npc.defense = 6;
            npc.lifeMax = 45;
            if (CalamityWorld.bossRushActive)
            {
                npc.lifeMax = 10000;
            }
            npc.aiStyle = -1;
            aiType = -1;
            animationType = 10;
            npc.knockBackResist = CalamityWorld.bossRushActive ? 0f : 0.3f;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.canGhostHeal = false;
            npc.HitSound = SoundID.NPCHit1;
            npc.DeathSound = SoundID.NPCDeath1;
        }

        public override void AI()
        {
            bool revenge = CalamityWorld.revenge;
            npc.TargetClosest(true);
            float num1372 = revenge ? 12f : 11f;
            if (CalamityWorld.bossRushActive)
                num1372 = 18f;

            Vector2 vector167 = new Vector2(npc.Center.X + (float)(npc.direction * 20), npc.Center.Y + 6f);
            float num1373 = Main.player[npc.target].position.X + (float)Main.player[npc.target].width * 0.5f - vector167.X;
            float num1374 = Main.player[npc.target].Center.Y - vector167.Y;
            float num1375 = (float)Math.Sqrt((double)(num1373 * num1373 + num1374 * num1374));
            float num1376 = num1372 / num1375;
            num1373 *= num1376;
            num1374 *= num1376;
            npc.ai[0] -= 1f;
            if (num1375 < 200f || npc.ai[0] > 0f)
            {
                if (num1375 < 200f)
                {
                    npc.ai[0] = 20f;
                }
                if (npc.velocity.X < 0f)
                {
                    npc.direction = -1;
                }
                else
                {
                    npc.direction = 1;
                }
                return;
            }
            npc.velocity.X = (npc.velocity.X * 50f + num1373) / 51f;
            npc.velocity.Y = (npc.velocity.Y * 50f + num1374) / 51f;
            if (num1375 < 350f)
            {
                npc.velocity.X = (npc.velocity.X * 10f + num1373) / 11f;
                npc.velocity.Y = (npc.velocity.Y * 10f + num1374) / 11f;
            }
            if (num1375 < 300f)
            {
                npc.velocity.X = (npc.velocity.X * 7f + num1373) / 8f;
                npc.velocity.Y = (npc.velocity.Y * 7f + num1374) / 8f;
            }
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            if (CalamityWorld.revenge)
            {
                player.AddBuff(ModContent.BuffType<MarkedforDeath>(), 120);
            }
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, 13, hitDirection, -1f, 0, default, 1f);
            }
            if (npc.life <= 0)
            {
                for (int k = 0; k < 20; k++)
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, 13, hitDirection, -1f, 0, default, 1f);
                }
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/HiveMindGores/DankCreeperGore"), 1f);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/HiveMindGores/DankCreeperGore2"), 1f);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/HiveMindGores/DankCreeperGore3"), 1f);
            }
        }

        public override void NPCLoot()
        {
            if (Main.expertMode && Main.netMode != NetmodeID.MultiplayerClient)
            {
                Projectile.NewProjectile(npc.Center.X, npc.Center.Y, 0f, 0f, ModContent.ProjectileType<ShadeNimbusHostile>(), 14, 0f, Main.myPlayer, 0f, 0f);
            }
        }
    }
}
