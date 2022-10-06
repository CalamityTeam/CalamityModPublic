using CalamityMod.Buffs.Summon;
using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
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
				List<int> Projectiles = new List<int>()
				{
					ModContent.ProjectileType<MagicRifle>(),
					ModContent.ProjectileType<MagicUmbrella>(),
					ModContent.ProjectileType<MagicHammer>(),
				};
                float angleVariance = MathHelper.TwoPi / Projectiles.Count;
                float angle = 0f;
				foreach (int projType in Projectiles)
				{
					int p = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, projType, Projectile.damage, Projectile.knockBack, Projectile.owner, angle);
					if (Main.projectile.IndexInRange(p))
						Main.projectile[p].originalDamage = Projectile.originalDamage;
					angle += angleVariance;
				}
			}
			Projectile.ai[0]++;

            //projectile movement
            Projectile.Center = player.Center + Vector2.UnitY * (player.gfxOffY - 60f);
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
                float detectionRange = Range;
                bool enemyDetected = false;

                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC npc = Main.npc[i];
                    if (npc.CanBeChasedBy(Projectile, false))
                    {
                        float extraDistance = (npc.width / 2) + (npc.height / 2);

                        if (Vector2.Distance(npc.Center, Projectile.Center) < (detectionRange + extraDistance))
                        {
                            enemyDetected = true;
                            break;
                        }
                    }
                }
				//enemyDetected = false;
                if (enemyDetected)
                {
                    if (Projectile.ai[1]++ % 50f == 25f)
                    {
						int projType = Utils.SelectRandom(Main.rand, new int[]
						{
							ModContent.ProjectileType<MagicHammer>(),
							ModContent.ProjectileType<MagicAxe>(),
							ModContent.ProjectileType<MagicBird>()
						});
						projType = ModContent.ProjectileType<MagicBird>(); // Consider rabbits, we're the magician
						float velocityX = Main.rand.NextFloat(-10f, 10f);
						float velocityY = Main.rand.NextFloat(-15f, -8f);
						int p = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.oldPosition.X + (float)(Projectile.width / 2), Projectile.oldPosition.Y + (float)(Projectile.height / 2), velocityX, velocityY, projType, Projectile.damage, Projectile.knockBack, Projectile.owner);
						if (Main.projectile.IndexInRange(p))
							Main.projectile[p].originalDamage = Projectile.originalDamage;
                    }
                }
            }
        }

        //glowmask effect
        public override Color? GetAlpha(Color lightColor) => new Color(200, 200, 200, 200);

        //no contact damage
        public override bool? CanDamage() => false;
    }
}
