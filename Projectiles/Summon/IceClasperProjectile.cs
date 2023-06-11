using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class IceClasperProjectile : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Summon";

        public Player Owner => Main.player[Projectile.owner];

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.MinionShot[Projectile.type] = true;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.idStaticNPCHitCooldown = 10;
            Projectile.timeLeft = 300;

            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.minion = true;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

            int bootlegTexture = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 172, Projectile.velocity.X, Projectile.velocity.Y, 0, default, 1.5f);
            Main.dust[bootlegTexture].noGravity = true;

            Lighting.AddLight(Projectile.Center, Color.Cyan.ToVector3());
        }

        public override bool OnTileCollide(Vector2 oldVelocity) => false;

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(BuffID.Frostburn, 180);

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return true;
        }
    }
}
