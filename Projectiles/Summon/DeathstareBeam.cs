using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class DeathstareBeam : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Summon";
        public ref float OwnerUUID => ref Projectile.ai[0];
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.MinionShot[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 10;
            Projectile.DamageType = DamageClass.Summon;
        }
        public override void AI()
        {
            if (!Main.projectile.IndexInRange((int)OwnerUUID))
            {
                Projectile.Kill();
                return;
            }

            Projectile.Opacity = Utils.GetLerpValue(1f, 0f, 1f - Projectile.timeLeft / 10f, true);
            Projectile.Center = CalamityUtils.FindProjectileByIdentity((int)OwnerUUID, Projectile.owner).Center - Projectile.velocity;
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D beamTexture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawPosition = Projectile.Center + Vector2.UnitY * Projectile.gfxOffY - Main.screenPosition;
            Vector2 drawScale = new Vector2(0.55f, Projectile.velocity.Length() / beamTexture.Height * 20f);
            Color color = Color.White * 2.1f * Projectile.Opacity;

            if (Math.Abs(Projectile.rotation) > 0.008f)
                Main.spriteBatch.Draw(beamTexture, drawPosition, null, color, Projectile.rotation, beamTexture.Frame().Bottom(), drawScale, SpriteEffects.None, 0);
            return false;
        }
    }
}
