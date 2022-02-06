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
    public class HiveBlob : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Hive Blob");
        }

        public override void SetDefaults()
        {
            npc.npcSlots = 0.1f;
            npc.aiStyle = -1;
            npc.damage = 0;
            npc.width = 25;
            npc.height = 25;
            npc.lifeMax = 100;
            if (BossRushEvent.BossRushActive)
            {
                npc.lifeMax = 1300;
            }
			npc.knockBackResist = 0f;
			aiType = -1;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.canGhostHeal = false;
            npc.chaseable = false;
            npc.HitSound = SoundID.NPCHit1;
            npc.DeathSound = SoundID.NPCDeath1;
			npc.Calamity().VulnerableToHeat = true;
			npc.Calamity().VulnerableToCold = true;
			npc.Calamity().VulnerableToSickness = true;
		}

        public override void AI()
        {
            bool expertMode = Main.expertMode || BossRushEvent.BossRushActive;
            bool revenge = CalamityWorld.revenge || BossRushEvent.BossRushActive;
			bool death = CalamityWorld.death || BossRushEvent.BossRushActive;

			int num750 = CalamityGlobalNPC.hiveMind;
			if (num750 < 0 || !Main.npc[num750].active)
			{
				npc.active = false;
				npc.netUpdate = true;
				return;
			}

			if (npc.ai[3] > 0f)
                num750 = (int)npc.ai[3] - 1;

            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
				npc.localAI[0] -= 1f;
				if (npc.localAI[0] <= 0f)
                {
                    npc.localAI[0] = Main.rand.Next(180, 361);
                    npc.ai[0] = Main.rand.Next(-100, 101);
                    npc.ai[1] = Main.rand.Next(-100, 101);
                    npc.netUpdate = true;
                }
            }

            npc.TargetClosest(true);

            float num751 = death ? 0.8f : revenge ? 0.7f : expertMode ? 0.6f : 0.5f;
            float num752 = 128f;
            Vector2 vector22 = new Vector2(npc.ai[0] * 16f + 8f, npc.ai[1] * 16f + 8f);
            float num189 = Main.player[npc.target].position.X + (Main.player[npc.target].width / 2) - (npc.width / 2) - vector22.X;
            float num190 = Main.player[npc.target].position.Y + (Main.player[npc.target].height / 2) - (npc.height / 2) - vector22.Y;
            float num191 = (float)Math.Sqrt(num189 * num189 + num190 * num190);
            float num754 = Main.npc[num750].position.X + (Main.npc[num750].width / 2);
            float num755 = Main.npc[num750].position.Y + (Main.npc[num750].height / 2);
            Vector2 vector93 = new Vector2(num754, num755);
            float num756 = num754 + npc.ai[0];
            float num757 = num755 + npc.ai[1];
            float num758 = num756 - vector93.X;
            float num759 = num757 - vector93.Y;
            float num760 = (float)Math.Sqrt(num758 * num758 + num759 * num759);
            num760 = num752 / num760;
            num758 *= num760;
            num759 *= num760;
            if (npc.position.X < num754 + num758)
            {
                npc.velocity.X = npc.velocity.X + num751;
                if (npc.velocity.X < 0f && num758 > 0f)
                    npc.velocity.X = npc.velocity.X * 0.8f;
            }
            else if (npc.position.X > num754 + num758)
            {
                npc.velocity.X = npc.velocity.X - num751;
                if (npc.velocity.X > 0f && num758 < 0f)
                    npc.velocity.X = npc.velocity.X * 0.8f;
            }
            if (npc.position.Y < num755 + num759)
            {
                npc.velocity.Y = npc.velocity.Y + num751;
                if (npc.velocity.Y < 0f && num759 > 0f)
                    npc.velocity.Y = npc.velocity.Y * 0.8f;
            }
            else if (npc.position.Y > num755 + num759)
            {
                npc.velocity.Y = npc.velocity.Y - num751;
                if (npc.velocity.Y > 0f && num759 < 0f)
                    npc.velocity.Y = npc.velocity.Y * 0.8f;
            }

			float velocityLimit = 8f;
			if (npc.velocity.X > velocityLimit)
				npc.velocity.X = velocityLimit;
			if (npc.velocity.X < -velocityLimit)
				npc.velocity.X = -velocityLimit;
			if (npc.velocity.Y > velocityLimit)
				npc.velocity.Y = velocityLimit;
			if (npc.velocity.Y < -velocityLimit)
				npc.velocity.Y = -velocityLimit;

			if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (!Collision.CanHit(npc.position, npc.width, npc.height, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height))
                    npc.localAI[1] = 180f;

                npc.localAI[1] += 1f;
                if (npc.localAI[1] >= 360f && Vector2.Distance(Main.player[npc.target].Center, npc.Center) > 80f)
                {
                    npc.localAI[1] = 0f;
                    npc.TargetClosest(true);
                    if (Collision.CanHit(npc.position, npc.width, npc.height, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height))
                    {
                        float num941 = death ? 5f : revenge ? 4.5f : expertMode ? 4f : 3.5f;
                        Vector2 vector104 = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + (npc.height / 2));
                        float num942 = Main.player[npc.target].position.X + Main.player[npc.target].width * 0.5f - vector104.X;
                        float num943 = Main.player[npc.target].position.Y + Main.player[npc.target].height * 0.5f - vector104.Y;
                        float num944 = (float)Math.Sqrt(num942 * num942 + num943 * num943);
                        num944 = num941 / num944;
                        num942 *= num944;
                        num943 *= num944;
						int type = ModContent.ProjectileType<VileClot>();
						int damage = npc.GetProjectileDamage(type);
						Projectile.NewProjectile(vector104.X, vector104.Y, num942, num943, type, damage, 0f, Main.myPlayer, 0f, 0f);
                        npc.netUpdate = true;
                    }
                }
            }
        }

		public override void NPCLoot()
		{
			if (!CalamityWorld.revenge)
			{
				int closestPlayer = Player.FindClosest(npc.Center, 1, 1);
				if (Main.rand.Next(8) == 0 && Main.player[closestPlayer].statLife < Main.player[closestPlayer].statLifeMax2)
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.Heart);
			}
		}

		public override bool CheckActive()
        {
            return false;
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, 14, hitDirection, -1f, 0, default, 1f);
            }
            if (npc.life <= 0)
            {
                for (int k = 0; k < 10; k++)
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, 14, hitDirection, -1f, 0, default, 1f);
                }
            }
        }
    }
}
