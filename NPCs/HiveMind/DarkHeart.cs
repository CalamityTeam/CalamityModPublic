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
    public class DarkHeart : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Dark Heart");
            Main.npcFrameCount[npc.type] = 4;
        }

        public override void SetDefaults()
        {
            npc.damage = 0;
            npc.width = 32;
            npc.height = 32;
            npc.defense = 2;
            npc.lifeMax = 150;
            if (BossRushEvent.BossRushActive)
            {
                npc.lifeMax = 1800;
            }
            npc.aiStyle = -1;
            aiType = -1;
            npc.knockBackResist = BossRushEvent.BossRushActive ? 0f : 0.4f;
            npc.noGravity = true;
            npc.canGhostHeal = false;
            npc.chaseable = false;
            npc.HitSound = SoundID.NPCHit13;
            npc.DeathSound = SoundID.NPCDeath21;
			npc.Calamity().VulnerableToHeat = true;
			npc.Calamity().VulnerableToCold = true;
			npc.Calamity().VulnerableToSickness = true;
		}

        public override void FindFrame(int frameHeight)
        {
            npc.frameCounter += 0.15f;
            npc.frameCounter %= Main.npcFrameCount[npc.type];
            int frame = (int)npc.frameCounter;
            npc.frame.Y = frame * frameHeight;
        }

        public override void AI()
        {
            bool revenge = CalamityWorld.revenge || BossRushEvent.BossRushActive;
            npc.TargetClosest();
            float num1164 = revenge ? 4.5f : 4f;
            float num1165 = revenge ? 0.8f : 0.75f;
            if (BossRushEvent.BossRushActive || CalamityWorld.malice)
            {
                num1164 *= 2f;
                num1165 *= 2f;
            }

            Vector2 vector133 = new Vector2(npc.Center.X, npc.Center.Y);
            float num1166 = Main.player[npc.target].Center.X - vector133.X;
            float num1167 = Main.player[npc.target].Center.Y - vector133.Y - 400f;
            float num1168 = (float)Math.Sqrt(num1166 * num1166 + num1167 * num1167);
            if (num1168 < 20f)
            {
                num1166 = npc.velocity.X;
                num1167 = npc.velocity.Y;
            }
            else
            {
                num1168 = num1164 / num1168;
                num1166 *= num1168;
                num1167 *= num1168;
            }
            if (npc.velocity.X < num1166)
            {
                npc.velocity.X = npc.velocity.X + num1165;
                if (npc.velocity.X < 0f && num1166 > 0f)
                {
                    npc.velocity.X = npc.velocity.X + num1165 * 2f;
                }
            }
            else if (npc.velocity.X > num1166)
            {
                npc.velocity.X = npc.velocity.X - num1165;
                if (npc.velocity.X > 0f && num1166 < 0f)
                {
                    npc.velocity.X = npc.velocity.X - num1165 * 2f;
                }
            }
            if (npc.velocity.Y < num1167)
            {
                npc.velocity.Y = npc.velocity.Y + num1165;
                if (npc.velocity.Y < 0f && num1167 > 0f)
                {
                    npc.velocity.Y = npc.velocity.Y + num1165 * 2f;
                }
            }
            else if (npc.velocity.Y > num1167)
            {
                npc.velocity.Y = npc.velocity.Y - num1165;
                if (npc.velocity.Y > 0f && num1167 < 0f)
                {
                    npc.velocity.Y = npc.velocity.Y - num1165 * 2f;
                }
            }
            if (npc.position.X + npc.width > Main.player[npc.target].position.X && npc.position.X < Main.player[npc.target].position.X + Main.player[npc.target].width && npc.position.Y + npc.height < Main.player[npc.target].position.Y && Collision.CanHit(npc.position, npc.width, npc.height, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height) && Main.netMode != NetmodeID.MultiplayerClient)
            {
                npc.ai[0] += 1f;
                if (npc.ai[0] >= 30f)
                {
                    npc.ai[0] = 0f;
                    int num1169 = (int)(npc.position.X + 10f + Main.rand.Next(npc.width - 20));
                    int num1170 = (int)(npc.position.Y + npc.height + 4f);
					int type = ModContent.ProjectileType<ShaderainHostile>();
					int damage = npc.GetProjectileDamage(type);
					Projectile.NewProjectile(num1169, num1170, 0f, 4f, type, damage, 0f, Main.myPlayer, 0f, 0f);
                }
            }
        }

		public override void NPCLoot()
		{
			if (!CalamityWorld.revenge)
			{
				int closestPlayer = Player.FindClosest(npc.Center, 1, 1);
				if (Main.rand.Next(4) == 0 && Main.player[closestPlayer].statLife < Main.player[closestPlayer].statLifeMax2)
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.Heart);
			}
		}

		public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 3; k++)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, 14, hitDirection, -1f, 0, default, 1f);
            }
            if (npc.life <= 0)
            {
                for (int k = 0; k < 20; k++)
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, 14, hitDirection, -1f, 0, default, 1f);
                }
            }
        }
    }
}
