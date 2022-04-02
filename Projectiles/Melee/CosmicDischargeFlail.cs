using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatBuffs;
using CalamityMod.Buffs.StatDebuffs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Projectiles.BaseProjectiles;

namespace CalamityMod.Projectiles.Melee
{
    public class CosmicDischargeFlail : BaseWhipProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cosmic Discharge");
        }

        public override void SetDefaults()
        {
            projectile.width = 16;
            projectile.height = 16;
            projectile.friendly = true;
            projectile.alpha = 255;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.melee = true;
            projectile.ignoreWater = true;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 6;
            projectile.coldDamage = true;
            projectile.extraUpdates = 1;
        }

        public override Color SpecialDrawColor => new Color(150, 255, 255);
        public override int ExudeDustType => 67;
        public override int WhipDustType => 187;
        public override int HandleHeight => 62;
        public override int BodyType1StartY => 64;
        public override int BodyType1SectionHeight => 28;
        public override int BodyType2StartY => 94;
        public override int BodyType2SectionHeight => 18;
        public override int TailStartY => 114;
        public override int TailHeight => 84;

        //All of this wouldnt be here if depthLayer fucking worked, i wanna hit someone - Shucks
        public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Vector2 mountedCenter = Main.player[projectile.owner].MountedCenter;
            Color colorAtCenter = Lighting.GetColor((int)(projectile.position.X + projectile.width * 0.5) / 16,
                (int)((projectile.position.Y + projectile.height * 0.5) / 16.0));
            if (projectile.hide && !ProjectileID.Sets.DontAttachHideToAlpha[projectile.type])
            {
                colorAtCenter = Lighting.GetColor((int)mountedCenter.X / 16, (int)(mountedCenter.Y / 16f));
            }
            Color drawColor = projectile.GetAlpha(colorAtCenter);
            float speed = projectile.velocity.Length() + 16f - 40f * projectile.scale;
            Vector2 normalizedVelocity = Vector2.Normalize(projectile.velocity);
            Rectangle type1BodyFrame = new Rectangle(0, BodyType1StartY, FlailTexture.Width, BodyType1SectionHeight);
            Vector2 bodyDrawPosition = projectile.Center.Floor();
            bodyDrawPosition += normalizedVelocity * projectile.scale * 33f;

            if (speed > 0f)
            {
                float counter = 0f;
                while (counter + 1f < speed)
                {
                    if (speed - counter < (float)type1BodyFrame.Height)
                    {
                        type1BodyFrame.Height = (int)(speed - counter);
                    }
                    Main.spriteBatch.Draw(FlailTexture,
                                          bodyDrawPosition - Main.screenPosition + Vector2.UnitY * Main.player[projectile.owner].gfxOffY,
                                          new Microsoft.Xna.Framework.Rectangle?(type1BodyFrame),
                                          drawColor,
                                          projectile.rotation + MathHelper.Pi,
                                          new Vector2((float)(type1BodyFrame.Width / 2), 0f),
                                          projectile.scale,
                                          SpriteEffects.None,
                                          0.6f);
                    counter += type1BodyFrame.Height * projectile.scale;
                    bodyDrawPosition += normalizedVelocity * (float)type1BodyFrame.Height * projectile.scale;
                }
            }
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (projHitbox.Intersects(targetHitbox))
            {
                return true;
            }
            float num8 = 0f;
            if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), projectile.Center, projectile.Center + projectile.velocity, 16f * projectile.scale, ref num8))
            {
                return true;
            }
            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            Player player = Main.player[projectile.owner];
            target.AddBuff(BuffID.Frostburn, 180);
            target.AddBuff(ModContent.BuffType<Nightwither>(), 180);
            target.AddBuff(ModContent.BuffType<GlacialState>(), 60);
            if (projectile.localAI[1] <= 0f && projectile.owner == Main.myPlayer)
            {
                Projectile.NewProjectile(target.Center.X, target.Center.Y, 0f, 0f, ModContent.ProjectileType<CosmicIceBurst>(), projectile.damage, 10f, projectile.owner, 0f, 0.85f + Main.rand.NextFloat() * 1.15f);
            }
            projectile.localAI[1] = 4f;
            player.AddBuff(ModContent.BuffType<CosmicFreeze>(), 300);
        }
    }
}
