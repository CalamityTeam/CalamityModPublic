using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class AcidShrapnel : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Rogue/BarrelShrapnel";

        public bool HitTile = false;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Shrapnel");
            Main.projFrames[projectile.type] = 3;
        }

        public override void SetDefaults()
        {
            projectile.width = 12;
            projectile.height = 14;
            projectile.friendly = true;
            projectile.penetrate = 1;
            projectile.timeLeft = 160;
            projectile.tileCollide = true;
            projectile.Calamity().rogue = true;
            projectile.ignoreWater = true;
        }
        public override void AI()
        {
            projectile.ai[0] += 1f;
            if (HitTile)
            {
                projectile.velocity.X = 0f;
                projectile.rotation = MathHelper.Pi;
            }
            else
                projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2;

            projectile.velocity.Y += 0.2f;
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            HitTile = true;
            return false;
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<Irradiated>(), 180);
            projectile.Kill();
        }
        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<Irradiated>(), 180);
            projectile.Kill();
        }
        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 15; i++)
            {
                Dust dust = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, (int)CalamityDusts.SulfurousSeaAcid);
                dust.velocity = Vector2.UnitY.RotatedBy(i / 15f * MathHelper.TwoPi) * 3f * (float)Math.Cos(i / 15f * MathHelper.TwoPi);
                dust.scale = 2.5f;
                dust.noGravity = true;
            }
        }
    }
}
