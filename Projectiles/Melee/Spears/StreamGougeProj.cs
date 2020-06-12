using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;
using CalamityMod.Projectiles.BaseProjectiles;
namespace CalamityMod.Projectiles.Melee.Spears
{
    public class StreamGougeProj : BaseSpearProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Gouge");
        }

        public override void SetDefaults()
        {
            projectile.width = 40;
            projectile.aiStyle = 19;
            projectile.melee = true;
            projectile.timeLeft = 90;
            projectile.height = 40;
            projectile.friendly = true;
            projectile.hostile = false;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.penetrate = -1;
            projectile.ownerHitCheck = true;
            projectile.hide = true;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 1;
            //projectile.Calamity().trueMelee = true;
        }

        public override float InitialSpeed => 3f;
        public override float ForwardSpeed => 0.95f;
        public override float ReelbackSpeed => 2.4f;

        public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            spriteBatch.Draw(ModContent.GetTexture("CalamityMod/Projectiles/Melee/Spears/StreamGougeGlow"),
                             projectile.Center - Main.screenPosition,
                             null,
                             Color.White,
                             projectile.rotation,
                             Vector2.Zero,
                             1f,
                             SpriteEffects.None,
                             0f);
        }
        public override Action<Projectile> EffectBeforeReelback => (proj) =>
        {
            Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, projectile.velocity.X, projectile.velocity.Y,
                ModContent.ProjectileType<EssenceBeam>(), projectile.damage * 4, projectile.knockBack, projectile.owner, 0f, 0f);
        };

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<GodSlayerInferno>(), 300);
        }
    }
}
