using CalamityMod.Projectiles.BaseProjectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class StardustElementalBeam : BaseLaserbeamProjectile
    {
        public override float MaxScale => 0.85f;
        public override float MaxLaserLength => 1000f;
        public override float Lifetime => 30f;
        public override Color LightCastColor => Color.White;
        public override Texture2D LaserBeginTexture => ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Lasers/UltimaRayStart");
        public override Texture2D LaserMiddleTexture => ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Lasers/UltimaRayMid");
        public override Texture2D LaserEndTexture => ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Lasers/UltimaRayEnd");

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Beam");
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = 10;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 13;
            Projectile.tileCollide = false;
            Projectile.timeLeft = (int)Lifetime;
        }

        public override void ExtraBehavior()
        {
            // Generate 2 stars from the side.
            if (Main.myPlayer == Projectile.owner && Time == 5f)
            {
                int type = ModContent.ProjectileType<BeamStar>();

                int damage = (int)(Projectile.damage * 0.75);
                for (int i = 0; i < 2; i++)
                {
                    Vector2 starSpeed = Projectile.velocity.RotatedBy(MathHelper.PiOver2 * i) * 5f;
                    Projectile.NewProjectile(Projectile.Center, starSpeed, type, damage, Projectile.knockBack, Projectile.owner);
                }
            }
        }

        public override void DetermineScale() => Projectile.scale = Projectile.timeLeft / Lifetime * MaxScale;


        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            DrawBeamWithColor(spriteBatch, Color.Lerp(Color.CornflowerBlue * 1.1f, Color.Transparent, 0.3f), Projectile.scale);
            DrawBeamWithColor(spriteBatch, Color.Lerp(Color.Cyan, Color.Transparent, 0.3f), Projectile.scale * 0.5f);
            DrawBeamWithColor(spriteBatch, Color.Lerp(Color.White, Color.Transparent, 0.3f), Projectile.scale * 0.2f);
            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            // Inflict the celled debuff for 3 seconds.
            target.AddBuff(BuffID.StardustMinionBleed, 180);

            if (!Main.rand.NextBool(7))
                return;

            float spawnOffsetSpread = Main.rand.NextFloat(MathHelper.ToRadians(36f), MathHelper.ToRadians(64f));
            float baseOffsetAngle = Main.rand.NextFloat(-0.6f, 0.6f);
            int type = ModContent.ProjectileType<BeamStar>();
            damage = (int)(Projectile.damage * 0.7);
            for (int i = 0; i < 4; i++)
            {
                float spawnOffsetAngle = MathHelper.Lerp(spawnOffsetSpread * -0.5f, spawnOffsetSpread * 0.5f, i / 4f) + baseOffsetAngle;
                Vector2 spawnPosition = target.Top - Vector2.UnitY.RotatedBy(spawnOffsetAngle) * 65f;
                Projectile.NewProjectile(spawnPosition, -Vector2.UnitY.RotatedBy(spawnOffsetAngle) * 2f, type, damage, knockback, Projectile.owner);
            }
        }
    }
}
