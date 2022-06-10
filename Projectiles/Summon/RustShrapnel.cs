using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class RustShrapnel : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Rogue/BarrelShrapnel";

        public bool HitTile = false;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Shrapnel");
            Main.projFrames[Projectile.type] = 3;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 14;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 180;
            Projectile.tileCollide = true;
            Projectile.minionSlots = 0f;
            Projectile.minion = true;
        }
        public override void AI()
        {
            Projectile.ai[0] += 1f;
            if (HitTile)
            {
                Projectile.velocity.X = 0f;
                Projectile.rotation = MathHelper.Pi;
            }
            else
            {
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            }
            Projectile.velocity.Y += 0.4f;
            Projectile.velocity.X *= 0.96f;
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            HitTile = true;
            return false;
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<Irradiated>(), 180);
            Projectile.Kill();
        }
        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<Irradiated>(), 180);
            Projectile.Kill();
        }
        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 15; i++)
            {
                Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, (int)CalamityDusts.SulfurousSeaAcid);
                dust.velocity = Vector2.UnitY.RotatedBy(i / 15f * MathHelper.TwoPi) * 3f * (float)Math.Cos(i / 15f * MathHelper.TwoPi);
                dust.scale = 2.5f;
                dust.noGravity = true;
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }
    }
}
