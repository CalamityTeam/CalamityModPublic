using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class ImpactRound : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Ranged/AMRShot";

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
            projectile.Calamity().pointBlankShotDuration = CalamityGlobalProjectile.basePointBlankShotDuration;
        }

        public override void AI()
        {
            if (!initialized && projectile.ranged) //Ranged check prevents quiver splits triggering the sound
            {
                initialized = true;

                if (Main.netMode != NetmodeID.Server)
                {
                    var sound = mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/LargeWeaponFire");
                    sound = sound.WithVolume(sound.Volume * 0.45f);
                    Main.PlaySound(sound, projectile.Center);
                }
            }
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            double damageMult = 1D;
            if (crit)
                damageMult += 0.25;
            damage = (int)(damage * damageMult);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor) => projectile.timeLeft < 600;
    }
}
