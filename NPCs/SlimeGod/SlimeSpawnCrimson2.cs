using CalamityMod.CalPlayer;
using CalamityMod.Events;
using CalamityMod.Projectiles.Enemy;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.SlimeGod
{
    public class SlimeSpawnCrimson2 : ModNPC
    {
        public float spikeTimer = 60f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Crimson Slime Spawn");
            Main.npcFrameCount[npc.type] = 5;
        }

        public override void SetDefaults()
        {
            npc.aiStyle = 1;
			npc.GetNPCDamage();
			npc.width = 40;
            npc.height = 30;
            npc.defense = 6;
            npc.lifeMax = 130;
            if (BossRushEvent.BossRushActive)
            {
                npc.lifeMax = 12000;
            }
            npc.knockBackResist = 0f;
            npc.alpha = 55;
            npc.lavaImmune = false;
            npc.noGravity = false;
            npc.noTileCollide = false;
            npc.canGhostHeal = false;
            npc.HitSound = SoundID.NPCHit1;
            npc.DeathSound = SoundID.NPCDeath1;
			npc.Calamity().VulnerableToHeat = true;
			npc.Calamity().VulnerableToSickness = false;
		}

        public override void FindFrame(int frameHeight)
        {
			int frameY = 1;
			if (!Main.dedServ)
			{
				if (!Main.NPCLoaded[npc.type] || Main.npcTexture[npc.type] is null)
					return;
				frameY = Main.npcTexture[npc.type].Height / Main.npcFrameCount[npc.type];
			}
			int aiState = 0;
			if (npc.aiAction == 0)
				aiState = npc.velocity.Y >= 0f ? (npc.velocity.Y <= 0f ? (npc.velocity.X == 0f ? 0 : 1) : 3) : 2;
			else if (npc.aiAction == 1)
				aiState = 4;

			npc.frameCounter++;
			if (aiState > 0)
				npc.frameCounter++;
			if (aiState == 4)
				npc.frameCounter++;
			if (npc.frameCounter >= 8f)
			{
				npc.frame.Y += frameY;
				npc.frameCounter = 0f;
			}
			if (npc.frame.Y >= frameY * Main.npcFrameCount[npc.type])
				npc.frame.Y = 0;
        }

        public override void AI()
        {
            if (spikeTimer > 0f)
                spikeTimer -= 1f;

			int type = ModContent.ProjectileType<CrimsonSpike>();
			int damage = npc.GetProjectileDamage(type);
			if (!npc.wet)
            {
                Vector2 vector3 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
                float num14 = Main.player[npc.target].position.X + (float)Main.player[npc.target].width * 0.5f - vector3.X;
                float num15 = Main.player[npc.target].position.Y - vector3.Y;
                float num16 = (float)Math.Sqrt((double)(num14 * num14 + num15 * num15));
                if (Main.expertMode && num16 < 120f && Collision.CanHit(npc.position, npc.width, npc.height, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height) && npc.velocity.Y == 0f)
                {
                    npc.ai[0] = -40f;
                    if (npc.velocity.Y == 0f)
                    {
                        npc.velocity.X = npc.velocity.X * 0.9f;
                    }
                    if (Main.netMode != NetmodeID.MultiplayerClient && spikeTimer == 0f)
                    {
                        for (int n = 0; n < 5; n++)
                        {
                            Vector2 vector4 = new Vector2((float)(n - 2), -4f);
                            vector4.X *= 1f + (float)Main.rand.Next(-50, 51) * 0.005f;
                            vector4.Y *= 1f + (float)Main.rand.Next(-50, 51) * 0.005f;
                            vector4.Normalize();
                            vector4 *= 4f + (float)Main.rand.Next(-50, 51) * 0.01f;
                            Projectile.NewProjectile(vector3.X, vector3.Y, vector4.X, vector4.Y, type, damage, 0f, Main.myPlayer, 0f, 0f);
                            spikeTimer = 30f;
                        }
                    }
                }
                else if (num16 < 360f && Collision.CanHit(npc.position, npc.width, npc.height, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height) && npc.velocity.Y == 0f)
                {
                    npc.ai[0] = -40f;
                    if (npc.velocity.Y == 0f)
                    {
                        npc.velocity.X = npc.velocity.X * 0.9f;
                    }
                    if (Main.netMode != NetmodeID.MultiplayerClient && spikeTimer == 0f)
                    {
                        num15 = Main.player[npc.target].position.Y - vector3.Y - (float)Main.rand.Next(0, 200);
                        num16 = (float)Math.Sqrt((double)(num14 * num14 + num15 * num15));
                        num16 = 6.5f / num16;
                        num14 *= num16;
                        num15 *= num16;
                        spikeTimer = 50f;
                        Projectile.NewProjectile(vector3.X, vector3.Y, num14, num15, type, damage, 0f, Main.myPlayer, 0f, 0f);
                    }
                }
            }
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, 4, hitDirection, -1f, 0, default, 1f);
            }
            if (npc.life <= 0)
            {
                for (int k = 0; k < 20; k++)
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, 4, hitDirection, -1f, 0, default, 1f);
                }
            }
        }

		public override bool PreNPCLoot()
		{
			if (!CalamityWorld.revenge)
			{
				int closestPlayer = Player.FindClosest(npc.Center, 1, 1);
				if (Main.rand.Next(8) == 0 && Main.player[closestPlayer].statLife < Main.player[closestPlayer].statLifeMax2)
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.Heart);
			}

			DropHelper.DropItemChance(npc, ItemID.Blindfold, Main.expertMode ? 50 : 100);

			return false;
		}

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
			player.AddBuff(BuffID.Darkness, 90, true);
		}
    }
}
