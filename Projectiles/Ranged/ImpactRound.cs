using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Ranged
{
    public class ImpactRound : ModProjectile
    {
		private bool initialized = false;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Impact Round");
        }

        public override void SetDefaults()
        {
            projectile.width = 4;
            projectile.height = 4;
            projectile.light = 0.5f;
            projectile.alpha = 255;
            projectile.extraUpdates = 7;
            projectile.scale = 1.18f;
            projectile.friendly = true;
            projectile.ranged = true;
            projectile.ignoreWater = true;
            projectile.aiStyle = 1;
            aiType = ProjectileID.BulletHighVelocity;
            projectile.penetrate = -1;
            projectile.timeLeft = 600;
			projectile.tileCollide = false;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
        }

		public override void AI()
		{
			if (!initialized)
			{
				initialized = true;
                Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/LargeWeaponFire"), (int)projectile.position.X, (int)projectile.position.Y);
			}
		}

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor) => projectile.timeLeft < 600;
    }
}
