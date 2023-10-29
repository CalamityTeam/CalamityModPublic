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
    public class CosmicDischargeFlail : BaseWhipProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.alpha = 255;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.MeleeNoSpeed;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 6;
            Projectile.coldDamage = true;
            Projectile.extraUpdates = 1;
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

        // All of this wouldnt be here if depthLayer fucking worked, i wanna hit someone - Shucks
        // the hell you mean, depthlayer is a useless fucking variable bro - Iban 2022
        // Not necessarily. Terraria is just made in such a way that we're forced to use sprite batch states that ignore it - Dominic 2022
        public override void PostDraw(Color lightColor)
        {
            Vector2 mountedCenter = Main.player[Projectile.owner].MountedCenter;
            Color colorAtCenter = Lighting.GetColor((int)(Projectile.position.X + Projectile.width * 0.5) / 16,
                (int)((Projectile.position.Y + Projectile.height * 0.5) / 16.0));
            if (Projectile.hide && !ProjectileID.Sets.DontAttachHideToAlpha[Projectile.type])
            {
                colorAtCenter = Lighting.GetColor((int)mountedCenter.X / 16, (int)(mountedCenter.Y / 16f));
            }
            Color drawColor = Projectile.GetAlpha(colorAtCenter);
            float speed = Projectile.velocity.Length() + 16f - 40f * Projectile.scale;
            Vector2 normalizedVelocity = Vector2.Normalize(Projectile.velocity);
            Rectangle type1BodyFrame = new Rectangle(0, BodyType1StartY, FlailTexture.Width, BodyType1SectionHeight);
            Vector2 bodyDrawPosition = Projectile.Center.Floor();
            bodyDrawPosition += normalizedVelocity * Projectile.scale * 33f;

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
                                          bodyDrawPosition - Main.screenPosition + Vector2.UnitY * Main.player[Projectile.owner].gfxOffY,
                                          new Microsoft.Xna.Framework.Rectangle?(type1BodyFrame),
                                          drawColor,
                                          Projectile.rotation + MathHelper.Pi,
                                          new Vector2((float)(type1BodyFrame.Width / 2), 0f),
                                          Projectile.scale,
                                          SpriteEffects.None,
                                          0.6f);
                    counter += type1BodyFrame.Height * Projectile.scale;
                    bodyDrawPosition += normalizedVelocity * (float)type1BodyFrame.Height * Projectile.scale;
                }
            }
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (projHitbox.Intersects(targetHitbox))
            {
                return true;
            }
            float zero = 0f;
            if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, Projectile.Center + Projectile.velocity, 16f * Projectile.scale, ref zero))
            {
                return true;
            }
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Player player = Main.player[Projectile.owner];
            target.AddBuff(BuffID.Frostburn, 180);
            target.AddBuff(ModContent.BuffType<Nightwither>(), 180);
            target.AddBuff(ModContent.BuffType<GlacialState>(), 60);
            if (Projectile.localAI[1] <= 0f && Projectile.owner == Main.myPlayer)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.Center.X, target.Center.Y, 0f, 0f, ModContent.ProjectileType<CosmicIceBurst>(), Projectile.damage, 10f, Projectile.owner, 0f, 0.85f + Main.rand.NextFloat() * 1.15f);
            }
            Projectile.localAI[1] = 4f;
            player.AddBuff(ModContent.BuffType<CosmicFreeze>(), 300);
        }
    }
}
