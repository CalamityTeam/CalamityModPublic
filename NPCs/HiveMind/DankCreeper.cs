using CalamityMod.Events;
using CalamityMod.Projectiles.Boss;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.HiveMind
{
    public class DankCreeper : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Dank Creeper");
        }

        public override void SetDefaults()
        {
            npc.Calamity().canBreakPlayerDefense = true;
            npc.GetNPCDamage();
            npc.width = 70;
            npc.height = 70;
            npc.defense = 6;
            npc.lifeMax = 90;
            if (BossRushEvent.BossRushActive)
            {
                npc.lifeMax = 2000;
            }
            npc.aiStyle = -1;
            aiType = -1;
            npc.knockBackResist = BossRushEvent.BossRushActive ? 0f : 0.3f;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.canGhostHeal = false;
            npc.HitSound = SoundID.NPCHit1;
            npc.DeathSound = SoundID.NPCDeath1;
            npc.Calamity().VulnerableToHeat = true;
            npc.Calamity().VulnerableToCold = true;
            npc.Calamity().VulnerableToSickness = true;
        }

        public override void AI()
        {
            npc.TargetClosest();
            bool revenge = CalamityWorld.revenge;
            float speed = revenge ? 12f : 11f;
            if (BossRushEvent.BossRushActive || CalamityWorld.malice)
                speed = 18f;

            if (npc.ai[1] < 90f)
                npc.ai[1] += 1f;
            speed = MathHelper.Lerp(3f, speed, npc.ai[1] / 90f);

            Vector2 vector167 = new Vector2(npc.Center.X + (npc.direction * 20), npc.Center.Y + 6f);
            float num1373 = Main.player[npc.target].position.X + Main.player[npc.target].width * 0.5f - vector167.X;
            float num1374 = Main.player[npc.target].Center.Y - vector167.Y;
            float num1375 = (float)Math.Sqrt(num1373 * num1373 + num1374 * num1374);
            float num1376 = speed / num1375;
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

        public override bool PreNPCLoot()
        {
            if (!CalamityWorld.revenge)
            {
                int closestPlayer = Player.FindClosest(npc.Center, 1, 1);
                if (Main.rand.Next(4) == 0 && Main.player[closestPlayer].statLife < Main.player[closestPlayer].statLifeMax2)
                    Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.Heart);
            }

            if ((Main.expertMode || BossRushEvent.BossRushActive) && Main.netMode != NetmodeID.MultiplayerClient)
            {
                int type = ModContent.ProjectileType<ShadeNimbusHostile>();
                int damage = npc.GetProjectileDamage(type);
                Projectile.NewProjectile(npc.Center, Vector2.Zero, type, damage, 0f, Main.myPlayer);
            }

            return false;
        }
    }
}
