using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Projectiles.BaseProjectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class StardustElementalBeam : BaseLaserbeamProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Magic";
        public override float MaxScale => 0.85f;
        public override float MaxLaserLength => 1000f;
        public override float Lifetime => 30f;
        public override Color LightCastColor => Color.White;
        public override Texture2D LaserBeginTexture => ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Lasers/UltimaRayStart", AssetRequestMode.ImmediateLoad).Value;
        public override Texture2D LaserMiddleTexture => ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Lasers/UltimaRayMid", AssetRequestMode.ImmediateLoad).Value;
        public override Texture2D LaserEndTexture => ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Lasers/UltimaRayEnd", AssetRequestMode.ImmediateLoad).Value;

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
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, starSpeed, type, damage, Projectile.knockBack, Projectile.owner);
                }
            }
        }

        public override void DetermineScale() => Projectile.scale = Projectile.timeLeft / Lifetime * MaxScale;


        public override bool PreDraw(ref Color lightColor)
        {
            DrawBeamWithColor(Color.Lerp(Color.CornflowerBlue * 1.1f, Color.Transparent, 0.3f), Projectile.scale);
            DrawBeamWithColor(Color.Lerp(Color.Cyan, Color.Transparent, 0.3f), Projectile.scale * 0.5f);
            DrawBeamWithColor(Color.Lerp(Color.White, Color.Transparent, 0.3f), Projectile.scale * 0.2f);
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<ElementalMix>(), 30);
            if (!Main.rand.NextBool(7))
                return;

            float spawnOffsetSpread = Main.rand.NextFloat(MathHelper.ToRadians(36f), MathHelper.ToRadians(64f));
            float baseOffsetAngle = Main.rand.NextFloat(-0.6f, 0.6f);
            int type = ModContent.ProjectileType<BeamStar>();
            hit.Damage = (int)(Projectile.damage * 0.7);
            for (int i = 0; i < 4; i++)
            {
                float spawnOffsetAngle = MathHelper.Lerp(spawnOffsetSpread * -0.5f, spawnOffsetSpread * 0.5f, i / 4f) + baseOffsetAngle;
                Vector2 spawnPosition = target.Top - Vector2.UnitY.RotatedBy(spawnOffsetAngle) * 65f;
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), spawnPosition, -Vector2.UnitY.RotatedBy(spawnOffsetAngle) * 2f, type, hit.Damage, hit.Knockback, Projectile.owner);
            }
        }
    }
}
