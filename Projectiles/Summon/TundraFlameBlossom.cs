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
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 30;
            Projectile.netImportant = true;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.minionSlots = 1f;
            Projectile.timeLeft = 18000;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft *= 5;
            Projectile.minion = true;
            Projectile.coldDamage = true;
        }

        public override void AI()
        {
            bool isCorrectMinion = Projectile.type == ModContent.ProjectileType<TundraFlameBlossom>();
            Player player = Main.player[Projectile.owner];
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
                    Projectile.timeLeft = 2;
                }
            }
            Projectile.Center = player.Center + (Projectile.ai[1] / 32f).ToRotationVector2() * 95f;
            Projectile.scale = 1f + (float)Math.Sin(Projectile.ai[1] / 40f) * 0.085f;
            Projectile.Opacity = 1f - (float)Math.Sin(Projectile.ai[1] / 45f) * 0.1f - 0.1f; // Range of 1f to 0.8f
            Projectile.rotation += MathHelper.ToRadians(5f);
            if (Projectile.localAI[0] == 0f)
            {
                for (int i = 0; i < 36; i++)
                {
                    Dust dust = Dust.NewDustPerfect(Projectile.Center, 179);
                    dust.noGravity = true;
                    dust.velocity = Vector2.One.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(2f, 7f);
                }
                Projectile.localAI[0] += 1f;
            }
            int fireRate = 130;
            float fireSpeed = 6f;
            if (Projectile.owner == Main.myPlayer)
            {
                NPC potentialTarget = Projectile.Center.MinionHoming(850f, player);
                if (potentialTarget != null)
                {
                    fireRate = 42;
                    fireSpeed = 14f;
                    Projectile.ai[0] = MathHelper.Lerp(Projectile.ai[0], Projectile.AngleTo(potentialTarget.Center) + MathHelper.PiOver2, 0.1f);
                }
                else Projectile.ai[0] = MathHelper.Lerp(Projectile.ai[0], 0f, 0.1f);
            }
            if (Projectile.ai[1]++ % fireRate == fireRate - 1)
            {
                if (Main.myPlayer == Projectile.owner)
                {
                    int count = Projectile.ai[1] % (2 * fireRate) == (2 * fireRate - 1) ? 4 : 3;
                    for (int i = 0; i < count; i++)
                    {
                        int orb = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, -Vector2.UnitY.RotatedBy(MathHelper.TwoPi / count * i + Projectile.ai[0]) * fireSpeed,
                            ModContent.ProjectileType<TundraFlameBlossomsOrb>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                        Main.projectile[orb].originalDamage = Projectile.originalDamage;
                    }
                }
            }
        }

        public override bool? CanDamage() => false;
    }
}
