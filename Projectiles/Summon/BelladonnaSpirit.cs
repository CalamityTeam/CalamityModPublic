using CalamityMod.Buffs.Summon;
using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Summon
{
    public class BelladonnaSpirit : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Belladonna Spirit");
            Main.projFrames[projectile.type] = 4;
            ProjectileID.Sets.MinionSacrificable[projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 38;
            projectile.height = 36;
            projectile.netImportant = true;
            projectile.friendly = true;
            projectile.minionSlots = 1;
            projectile.timeLeft = 18000;
            projectile.penetrate = -1;
            projectile.timeLeft *= 5;
            projectile.minion = true;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
        }

        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();
            if (projectile.localAI[0] == 0f)
            {
                projectile.Calamity().spawnedPlayerMinionDamageValue = (player.allDamage + player.minionDamage - 1f);
                projectile.Calamity().spawnedPlayerMinionProjectileDamageValue = projectile.damage;
                for (int i = 0; i < 45; i++)
                {
                    float angle = MathHelper.TwoPi / 45f * i;
                    Vector2 velocity = angle.ToRotationVector2() * 4f;
                    Dust dust = Dust.NewDustPerfect(projectile.Center + velocity * 2.75f, 39, velocity);
                    dust.noGravity = true;
                }
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
            if ((player.allDamage + player.minionDamage - 1f) != projectile.Calamity().spawnedPlayerMinionDamageValue)
            {
                int trueDamage = (int)((float)projectile.Calamity().spawnedPlayerMinionProjectileDamageValue /
                    projectile.Calamity().spawnedPlayerMinionDamageValue *
                    (player.allDamage + player.minionDamage - 1f));
                projectile.damage = trueDamage;
            }
            bool isCorrectProjectile = projectile.type == ModContent.ProjectileType<BelladonnaSpirit>();
            player.AddBuff(ModContent.BuffType<BelladonnaSpiritBuff>(), 3600);
            if (isCorrectProjectile)
            {
                if (player.dead)
                {
                    modPlayer.belladonaSpirit = false;
                }
                if (modPlayer.belladonaSpirit)
                {
                    projectile.timeLeft = 2;
                }
            }
            if (projectile.velocity.X > 0.25f)
            {
                projectile.spriteDirection = 1;
            }
            else if (projectile.velocity.X < -0.25f)
            {
                projectile.spriteDirection = -1;
            }
            NPC potentialTarget = projectile.Center.MinionHoming(1200f, player);
            Vector2 targetPosition = player.Bottom;
            if (potentialTarget == null)
            {
                projectile.velocity.X = (player.Center.X + player.direction * 75f - projectile.Center.X) / 110f;
                if (projectile.Distance(player.Center) > 2500f ||
                    targetPosition.Y - projectile.Top.Y > 360f)
                {
                    projectile.Center = player.Center;
                    projectile.netUpdate = true;
                }
                else if (targetPosition.Y - projectile.Top.Y < -550f)
                {
                    projectile.velocity.Y += Math.Sign(targetPosition.Y - targetPosition.Y) * 0.05f;
                }
                else
                {
                    projectile.velocity.Y = (targetPosition.Y - projectile.Center.Y) / 110f;
                }
            }
            else
            {
                targetPosition = potentialTarget.Center;
                if (Math.Abs(targetPosition.X - projectile.Center.X) < 180f)
                {
                    projectile.velocity.X *= 0.95f;
                    projectile.ai[0]++;
                    if (Main.myPlayer == projectile.owner && projectile.ai[0] % 20f == 19f)
                    {
                        for (int i = -1; i <= 1; i++)
                        {
                            float angle = Main.rand.NextFloat(-0.1f, 0.1f) + i * 0.05f;
                            Projectile.NewProjectile(projectile.Center + new Vector2(0f, -6f),
                                projectile.DirectionTo(potentialTarget.Center).RotatedBy(angle) * 7.5f, ModContent.ProjectileType<BelladonnaPetal>(),
                                projectile.damage, projectile.knockBack, projectile.owner);
                        }
                    }
                    if (Main.myPlayer == projectile.owner && projectile.ai[0] % 180f == 179f)
                    {
                        for (int i = 0; i <= 4; i++)
                        {
                            float angle = MathHelper.Lerp(MathHelper.ToRadians(-Main.rand.NextFloat(30f, 36f)), MathHelper.ToRadians(Main.rand.NextFloat(30f, 36f)), i / 4f);
                            Projectile.NewProjectile(projectile.Center + new Vector2(0f, -6f),
                                new Vector2(0f, -9f).RotatedBy(angle), ModContent.ProjectileType<BelladonnaPetal>(),
                                projectile.damage, projectile.knockBack, projectile.owner);
                        }
                    }
                }
                else
                {
                    projectile.velocity.X += (targetPosition.X - projectile.Center.X + potentialTarget.spriteDirection * 75f > 0).ToDirectionInt() * 0.5f;
                    projectile.velocity.X = MathHelper.Clamp(projectile.velocity.X, -12f, 12f);
                }
                projectile.velocity.Y = (targetPosition.Y - projectile.Center.Y + potentialTarget.spriteDirection * 75f) / 90f;
            }
            float SAImovement = 0.05f;
            for (int index = 0; index < Main.projectile.Length; index++)
            {
                Projectile proj = Main.projectile[index];
                if (index != projectile.whoAmI &&
                    proj.active && proj.owner == projectile.owner &&
                    proj.type == projectile.type && Math.Abs(projectile.position.X - proj.position.X) + Math.Abs(projectile.position.Y - proj.position.Y) < projectile.width)
                {
                    if (projectile.position.X < proj.position.X)
                    {
                        projectile.velocity.X -= SAImovement;
                    }
                    else
                    {
                        projectile.velocity.X += SAImovement;
                    }
                    if (projectile.position.Y < proj.position.Y)
                    {
                        projectile.velocity.Y -= SAImovement;
                    }
                    else
                    {
                        projectile.velocity.Y += SAImovement;
                    }
                }
            }
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (projectile.penetrate == 0)
            {
                projectile.Kill();
            }
            return false;
        }
    }
}
