using CalamityMod.Buffs.Summon;
using CalamityMod.Dusts;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Summon
{
    public class TacticalPlagueEngineSummon : ModProjectile
    {
        public static Item FalseGun = null;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Tactical Plague Jet");
            Main.projFrames[projectile.type] = 3;
            ProjectileID.Sets.MinionSacrificable[projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 52;
            projectile.height = 32;
            projectile.netImportant = true;
            projectile.friendly = true;
            projectile.minionSlots = 1;
            projectile.timeLeft = 18000;
            projectile.penetrate = -1;
            projectile.timeLeft *= 5;
            projectile.minion = true;
            projectile.tileCollide = false;
			projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();
            if (projectile.localAI[0] == 0f)
            {
                projectile.Calamity().spawnedPlayerMinionDamageValue = player.MinionDamage();
                projectile.Calamity().spawnedPlayerMinionProjectileDamageValue = projectile.damage;
                for (int i = 0; i < 45; i++)
                {
                    float angle = MathHelper.TwoPi / 45f * i;
                    Vector2 velocity = angle.ToRotationVector2() * 4f;
                    Dust dust = Dust.NewDustPerfect(projectile.Center + velocity * 2f, (int)CalamityDusts.Plague, velocity);
                    dust.noGravity = true;
                }
                FalseGun = ItemLoader.GetItem(ModContent.ItemType<P90>()).item;
                projectile.localAI[0] = 1f;
            }
            if (projectile.frameCounter++ > 6f)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
            }
            if (projectile.frame >= Main.projFrames[projectile.type])
            {
                projectile.frame = 0;
            }
            if (player.MinionDamage() != projectile.Calamity().spawnedPlayerMinionDamageValue)
            {
                int trueDamage = (int)((float)projectile.Calamity().spawnedPlayerMinionProjectileDamageValue /
                    projectile.Calamity().spawnedPlayerMinionDamageValue *
                    player.MinionDamage());
                projectile.damage = trueDamage;
            }
            bool isCorrectProjectile = projectile.type == ModContent.ProjectileType<TacticalPlagueEngineSummon>();
            player.AddBuff(ModContent.BuffType<TacticalPlagueEngineBuff>(), 3600);
            if (isCorrectProjectile)
            {
				if (player.dead)
				{
					modPlayer.plagueEngine = false;
				}
				if (modPlayer.plagueEngine)
                {
                    projectile.timeLeft = 2;
                }
            }
            NPC potentialTarget = projectile.Center.MinionHoming(1560f, player);

            if (potentialTarget is null || !player.HasAmmo(FalseGun, false))
            {
                float acceleration = 0.1f;
                Vector2 distanceVector = player.Center - projectile.Center;
                if (distanceVector.Length() < 200f)
                {
                    acceleration = 0.07f;
                }
                if (distanceVector.Length() < 140f)
                {
                    acceleration = 0.035f;
                }
                if (distanceVector.Length() > 100f)
                {
                    if (Math.Abs(player.Center.X - projectile.Center.X) > 20f)
                    {
                        projectile.velocity.X += acceleration * Math.Sign(player.Center.X - projectile.Center.X);
                    }
                    if (Math.Abs(player.Center.Y - projectile.Center.Y) > 10f)
                    {
                        projectile.velocity.Y += acceleration * Math.Sign(player.Center.Y - projectile.Center.Y);
                    }
                }
                else if (projectile.velocity.Length() > 4f)
                {
                    projectile.velocity *= 0.95f;
                }
                if (Math.Abs(projectile.velocity.Y) < 2f)
                {
                    projectile.velocity.Y += 0.1f * Math.Sign(player.Center.Y - projectile.Center.Y);
                }
                if (projectile.velocity.Length() > 9f)
                {
                    projectile.velocity = Vector2.Normalize(projectile.velocity) * 9f;
                }

                if (projectile.velocity.X > 0.25f)
                {
                    projectile.spriteDirection = 1;
                }
                else if (projectile.velocity.X < -0.25f)
                {
                    projectile.spriteDirection = -1;
                }
                projectile.rotation = projectile.rotation.AngleTowards(0f, 0.2f);

                if (distanceVector.Length() > 2700f)
                {
                    projectile.Center = player.Center;
                    projectile.netUpdate = true;
                }
            }
            else
            {
                projectile.spriteDirection = 1;
                Vector2 idealVelocity = projectile.DirectionTo(potentialTarget.Center - Vector2.UnitY * 195f) * 17f;
                projectile.velocity = Vector2.Lerp(projectile.velocity, idealVelocity, 0.035f);
                projectile.rotation = projectile.rotation.AngleTowards(projectile.AngleTo(potentialTarget.Center), 0.1f);

                if (projectile.ai[0]++ % 75f == 24f)
                {
                    int damage = projectile.damage;
                    if (projectile.ai[1]++ % 20f == 0f)
                    {
                        int idx = Projectile.NewProjectile(projectile.Center, projectile.DirectionTo(potentialTarget.Center) * 18f, ModContent.ProjectileType<MK2RocketHoming>(),
                            (int)(damage * 1.5), 5f, projectile.owner);
                        Main.projectile[idx].Calamity().forceMinion = true;
                    }
                    else
                    {
                        int shoot = 0;
                        float shootSpeed = 0f;
                        bool canShoot = true;
                        float knockBack = projectile.knockBack * 0.5f;
                        player.PickAmmo(FalseGun, ref shoot, ref shootSpeed, ref canShoot, ref damage, ref knockBack);
                        int idx = Projectile.NewProjectile(projectile.Center, projectile.DirectionTo(potentialTarget.Center) * shootSpeed, shoot,
                            damage, knockBack, projectile.owner);
                        // There's airway for a small bug in here, but the potential alterative (that has indeed been appearing), where
                        // the projectile simply cannot exist, is far worse than this. If you have another solution, let me know.
                        if (idx >= 0 && idx < Main.projectile.Length)
                        {
                            Main.projectile[idx].Calamity().forceMinion = true;
                        }
                    }
                }
				projectile.MinionAntiClump(0.25f);
            }
        }

        public override bool CanDamage() => false;
    }
}
