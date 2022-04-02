using CalamityMod.Buffs.Summon;
using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Summon
{
    public class TundraFlameBlossom : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Frost Blossom");
            ProjectileID.Sets.MinionSacrificable[projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = projectile.height = 30;
            projectile.netImportant = true;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.minionSlots = 1f;
            projectile.timeLeft = 18000;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.timeLeft *= 5;
            projectile.minion = true;
            projectile.coldDamage = true;
        }

        public override void AI()
        {
            bool isCorrectMinion = projectile.type == ModContent.ProjectileType<TundraFlameBlossom>();
            Player player = Main.player[projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();
            player.AddBuff(ModContent.BuffType<TundraFlameBlossomsBuff>(), 3600);
            if (isCorrectMinion)
            {
                if (player.dead)
                {
                    modPlayer.tundraFlameBlossom = false;
                }
                if (modPlayer.tundraFlameBlossom)
                {
                    projectile.timeLeft = 2;
                }
            }
            projectile.Center = player.Center + (projectile.ai[1] / 32f).ToRotationVector2() * 95f;
            projectile.scale = 1f + (float)Math.Sin(projectile.ai[1] / 40f) * 0.085f;
            projectile.Opacity = 1f - (float)Math.Sin(projectile.ai[1] / 45f) * 0.1f - 0.1f; // Range of 1f to 0.8f
            projectile.rotation += MathHelper.ToRadians(5f);
            if (projectile.localAI[0] == 0f)
            {
                projectile.Calamity().spawnedPlayerMinionDamageValue = player.MinionDamage();
                projectile.Calamity().spawnedPlayerMinionProjectileDamageValue = projectile.damage;
                for (int i = 0; i < 36; i++)
                {
                    Dust dust = Dust.NewDustPerfect(projectile.Center, 179);
                    dust.noGravity = true;
                    dust.velocity = Vector2.One.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(2f, 7f);
                }
                projectile.localAI[0] += 1f;
            }
            if (player.MinionDamage() != projectile.Calamity().spawnedPlayerMinionDamageValue)
            {
                int trueDamage = (int)((float)projectile.Calamity().spawnedPlayerMinionProjectileDamageValue /
                    projectile.Calamity().spawnedPlayerMinionDamageValue *
                    player.MinionDamage());
                projectile.damage = trueDamage;
            }
            int fireRate = 130;
            float fireSpeed = 6f;
            if (projectile.owner == Main.myPlayer)
            {
                NPC potentialTarget = projectile.Center.MinionHoming(850f, player);
                if (potentialTarget != null)
                {
                    fireRate = 42;
                    fireSpeed = 14f;
                    projectile.ai[0] = MathHelper.Lerp(projectile.ai[0], projectile.AngleTo(potentialTarget.Center) + MathHelper.PiOver2, 0.1f);
                }
                else projectile.ai[0] = MathHelper.Lerp(projectile.ai[0], 0f, 0.1f);
            }
            if (projectile.ai[1]++ % fireRate == fireRate - 1)
            {
                if (Main.myPlayer == projectile.owner)
                {
                    int count = projectile.ai[1] % (2 * fireRate) == (2 * fireRate - 1) ? 4 : 3;
                    for (int i = 0; i < count; i++)
                    {
                        Projectile.NewProjectile(projectile.Center, -Vector2.UnitY.RotatedBy(MathHelper.TwoPi / count * i + projectile.ai[0]) * fireSpeed,
                            ModContent.ProjectileType<TundraFlameBlossomsOrb>(), projectile.damage, projectile.knockBack, projectile.owner);
                    }
                }
            }
        }

        public override bool CanDamage() => false;
    }
}
