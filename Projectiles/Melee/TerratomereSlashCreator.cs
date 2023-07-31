using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Sounds;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee
{
    public class TerratomereSlashCreator : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        public NPC Target => Main.npc[(int)Projectile.ai[0]];
        
        public float SlashDirection
        {
            get
            {
                if (Projectile.ai[1] > MathHelper.Pi)
                    return Main.rand.NextFloatDirection();
                return Projectile.ai[1] + Main.rand.NextFloatDirection() * 0.2f;
            }
        }

        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = 2;
            Projectile.height = 2;
            Projectile.ignoreWater = false;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 45;
            Projectile.MaxUpdates = 2;
            Projectile.noEnchantmentVisuals = true;
        }

        public override void AI()
        {
            if (Projectile.timeLeft % Terratomere.SmallSlashCreationRate == 0)
            {
                SoundEngine.PlaySound(CommonCalamitySounds.SwiftSliceSound, Projectile.Center);
                if (Main.myPlayer == Projectile.owner)
                {
                    float maxOffset = Target.width * 0.4f;
                    if (maxOffset > 300f)
                        maxOffset = 300f;

                    Vector2 spawnOffset = SlashDirection.ToRotationVector2() * Main.rand.NextFloatDirection() * maxOffset;
                    Vector2 sliceVelocity = spawnOffset.SafeNormalize(Vector2.UnitY) * 0.1f;

                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Target.Center + spawnOffset, sliceVelocity, ModContent.ProjectileType<TerratomereSlash>(), (int)(Projectile.damage * Terratomere.SmallSlashDamageFactor), 0f, Projectile.owner);
                }
            }
        }

        public override bool? CanDamage() => false;
    }
}
