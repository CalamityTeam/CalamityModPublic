using CalamityMod.Buffs.Summon;
using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon.Umbrella
{
    public class MagicHat : ModProjectile
    {
        public const float Range = 1500.0001f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Magic Hat");
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.netImportant = true;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.minionSlots = 5f;
            Projectile.timeLeft = 18000;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft *= 5;
            Projectile.minion = true;
            Projectile.DamageType = DamageClass.Summon;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();
            CalamityGlobalProjectile modProj = Projectile.Calamity();

            //set up minion buffs and bools
            bool hatExists = Projectile.type == ModContent.ProjectileType<MagicHat>();
            player.AddBuff(ModContent.BuffType<MagicHatBuff>(), 3600);
            if (hatExists)
            {
                if (player.dead)
                {
                    modPlayer.magicHat = false;
                }
                if (modPlayer.magicHat)
                {
                    Projectile.timeLeft = 2;
                }
            }

			if (Projectile.ai[0] == 1f)
			{
				List<Tuple<int, float>> Projectiles = new List<Tuple<int, float>>()
				{
					new Tuple<int, float>(ModContent.ProjectileType<MagicRifle>(), 1f),
					new Tuple<int, float>(ModContent.ProjectileType<MagicUmbrella>(), 1f),
					new Tuple<int, float>(ModContent.ProjectileType<MagicAxe>(), 1f),
					new Tuple<int, float>(ModContent.ProjectileType<MagicHammer>(), 3f),
				};
                float angleVariance = MathHelper.TwoPi / Projectiles.Count;
                float angle = 0f;
				for (int i = 0; i < Projectiles.Count; i++)
				{
					int p = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, Projectiles[i].Item1,(int)(Projectile.damage * Projectiles[i].Item2),
													 Projectile.knockBack * Projectiles[i].Item2, Projectile.owner, angle);
					if (Main.projectile.IndexInRange(p))
						Main.projectile[p].originalDamage = (int)(Projectile.originalDamage * Projectiles[i].Item2);
					angle += angleVariance;
				}
			}
			Projectile.ai[0]++;

            //projectile movement
            Projectile.Center = player.Center + Vector2.UnitY * (player.gfxOffY - 30f);
            if (player.gravDir == -1f)
            {
                Projectile.position.Y += 120f;
                Projectile.rotation = MathHelper.Pi;
            }
            else
            {
                Projectile.rotation = 0f;
            }
            Projectile.position.X = (int)Projectile.position.X;
            Projectile.position.Y = (int)Projectile.position.Y;

            //Change the summons scale size a little bit to make it pulse in and out
            float scalar = (float)Main.mouseTextColor / 200f - 0.35f;
            scalar *= 0.2f;
            Projectile.scale = scalar + 0.95f;

            //on summon dust and flexible damage
            if (Projectile.localAI[0] == 0f)
            {
                int dustAmt = 50;
                for (int dustIndex = 0; dustIndex < dustAmt; dustIndex++)
                {
                    int dustEffects = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y + 16f), Projectile.width, Projectile.height - 16, 234, 0f, 0f, 0, default, 1f);
                    Main.dust[dustEffects].velocity *= 2f;
                    Main.dust[dustEffects].scale *= 1.15f;
                }
                Projectile.localAI[0] += 1f;
            }

            //finding an enemy, then shooting projectiles if it's detected
            if (Projectile.owner == Main.myPlayer)
            {
				int targetIdx = -1;
				float maxHomingRange = Range;
				if (player.HasMinionAttackTargetNPC)
				{
					NPC npc = Main.npc[player.MinionAttackTargetNPC];
					if (npc.CanBeChasedBy(Projectile, false))
					{
						float dist = (Projectile.Center - npc.Center).Length();
						if (dist < maxHomingRange)
						{
							targetIdx = player.MinionAttackTargetNPC;
							maxHomingRange = dist;
						}
					}
				}
				if (targetIdx == -1)
				{
					for (int i = 0; i < Main.npc.Length; ++i)
					{
						NPC npc = Main.npc[i];
						if (npc is null || !npc.active)
							continue;

						if (npc.CanBeChasedBy(Projectile, false))
						{
							float dist = (Projectile.Center - npc.Center).Length();
							if (dist < maxHomingRange)
							{
								targetIdx = i;
								maxHomingRange = dist;
							}
						}
					}
				}
				//targetIdx = -1;
                if (targetIdx != -1)
                {
                    if (Projectile.ai[1]++ % 50f == 25f)
                    {
						int projType = Utils.SelectRandom(Main.rand, new int[]
						{
							ModContent.ProjectileType<MagicBunny>(),
							ModContent.ProjectileType<MagicBird>()
						});
						Vector2 velocity = CalamityUtils.GetProjectilePhysicsFiringVelocity(Projectile.Center, Main.npc[targetIdx].Center, 0.28f, 12f);
						Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.oldPosition.X + (float)(Projectile.width / 2), Projectile.oldPosition.Y + (float)(Projectile.height / 2), velocity.X, velocity.Y, projType, Projectile.damage, Projectile.knockBack, Projectile.owner);
                    }
                }
            }
        }

        // Glowmask effect
        public override Color? GetAlpha(Color lightColor) => new Color(200, 200, 200, 200);

        // No contact damage
        public override bool? CanDamage() => false;

		// Draw over players
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI) => overPlayers.Add(index);

        public override bool PreDraw(ref Color lightColor)
        {
			Player player = Main.player[Projectile.owner];

			Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
			
			float stealthPercent = player.Calamity().rogueStealth / player.Calamity().rogueStealthMax * 0.9f; //0 to 0.9
            bool hasStealth = player.Calamity().rogueStealth > 0f && stealthPercent > 0.45f && player.townNPCs < 3f && CalamityConfig.Instance.StealthInvisibility;
			if (player.ShouldNotDraw || hasStealth)
				texture = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Summon/Umbrella/MagicHatInvis").Value;
			Rectangle frame = texture.Frame(1, Main.projFrames[Type], 0, Projectile.frame);
			Vector2 origin = frame.Size() * 0.5f;
			Vector2 drawPosition = Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);
			SpriteEffects direction = Projectile.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            // Draw the hat.
			Main.spriteBatch.Draw(texture, drawPosition, frame, Projectile.GetAlpha(lightColor), Projectile.rotation, origin, Projectile.scale, direction, 0);
            return false;
        }
    }
}
