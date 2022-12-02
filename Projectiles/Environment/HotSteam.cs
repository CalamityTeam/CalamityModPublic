using CalamityMod.DataStructures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Environment
{
    public class HotSteam : ModProjectile, IAdditiveDrawer
    {
        public const int Lifetime = 180;

        public override string Texture => "CalamityMod/Projectiles/Summon/SmallAresArms/MinionPlasmaGas";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Steam");

            // This prevents the water from creating a universal distortion wherever it lands, thus making the geysers look weird.
            ProjectileID.Sets.NoLiquidDistortion[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 184;
            Projectile.penetrate = -1;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = Lifetime;
            Projectile.hide = true;
            Projectile.localAI[0] = 1f;
        }

        public override void AI()
        {
            Projectile.localAI[0] = 0f;
            Projectile.scale = Utils.Remap(Projectile.timeLeft, Lifetime, 0.4f, 0.02f, 1.4f);
            Projectile.Opacity = Utils.Remap(Projectile.timeLeft, 160f, 50f, 1.15f, 0.01f);
            Projectile.velocity.Y *= 0.97f;
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            if (damage <= 0)
                return;

            target.AddBuff(BuffID.Poisoned, 180);
        }

        public override bool? CanDamage() => Projectile.Opacity > 0.6f ? null : false;

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return CalamityUtils.CircularHitboxCollision(Projectile.Center, Projectile.scale * 92f, targetHitbox);
        }

        public void AdditiveDraw(SpriteBatch spriteBatch)
        {
            if (Projectile.localAI[0] == 1f)
                return;

            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 origin = texture.Size() * 0.5f;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            float opacity = Projectile.Opacity * 0.3f;
            Color drawColor = new Color(159, 207, 181) * opacity;
            Vector2 scale = Projectile.Size / texture.Size() * Projectile.scale * 1.35f;
            spriteBatch.Draw(texture, drawPosition, null, drawColor, Projectile.rotation, origin, scale, 0, 0f);
        }
    }
}
