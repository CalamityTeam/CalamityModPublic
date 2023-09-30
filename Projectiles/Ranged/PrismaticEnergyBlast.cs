using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Projectiles.BaseProjectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class PrismaticEnergyBlast : BaseLaserbeamProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Ranged";
        public bool ExplodedYet
        {
            get => Projectile.ai[1] == 1f;
            set => Projectile.ai[1] = value.ToInt();
        }
        public override string Texture => "CalamityMod/ExtraTextures/Lasers/PrismLaserStart";
        public override float MaxScale => 1f;
        public override float MaxLaserLength => 2700f;
        public override float Lifetime => 50f;
        public override Color LaserOverlayColor => Color.White;
        public override Color LightCastColor => LaserOverlayColor;
        public override Texture2D LaserBeginTexture => ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Lasers/PrismLaserStart", AssetRequestMode.ImmediateLoad).Value;
        public override Texture2D LaserMiddleTexture => ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Lasers/PrismLaserMid", AssetRequestMode.ImmediateLoad).Value;
        public override Texture2D LaserEndTexture => ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Lasers/PrismLaserEnd", AssetRequestMode.ImmediateLoad).Value;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 5;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 14;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.tileCollide = false;
            Projectile.penetrate = 100;
            Projectile.extraUpdates = 1;
            Projectile.timeLeft = 450;
        }

        public override bool PreAI()
        {
            Projectile.frameCounter++;
            if (Projectile.frameCounter % 5 == 4)
                Projectile.frame++;

            Vector2 laserEnd = Projectile.position + Projectile.velocity * LaserLength;
            if (Collision.SolidCollision(laserEnd, 84, 84))
                CreateExplosion(laserEnd + Projectile.Size * 0.5f);
            return true;
        }

        public void CreateExplosion(Vector2 laserEnd)
        {
            if (Main.myPlayer != Projectile.owner || ExplodedYet || LaserLength <= 60f)
                return;

            Projectile.NewProjectile(Projectile.GetSource_FromThis(), laserEnd, Vector2.Zero, ModContent.ProjectileType<PrismExplosionLarge>(), Projectile.damage / 2, 0f, Projectile.owner);

            if (!ExplodedYet)
            {
                ExplodedYet = true;
                Projectile.netUpdate = true;
            }
        }

        public override float DetermineLaserLength()
        {
            if (Projectile.penetrate == 100)
                return DetermineLaserLength_CollideWithTiles(8);
            return LaserLength;
        }

        public override bool? CanDamage() => Projectile.penetrate == 100 ? null : false;

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            LaserLength = Projectile.Distance(target.Center);
            CreateExplosion(target.Center);
            target.AddBuff(ModContent.BuffType<MiracleBlight>(), 300);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info) => target.AddBuff(ModContent.BuffType<MiracleBlight>(), 300);

        public override void OnKill(int timeLeft)
        {
            Vector2 laserEnd = Projectile.Center + Projectile.velocity * LaserLength;
            CreateExplosion(laserEnd);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            // Start texture drawing.
            Rectangle beginFrame = LaserBeginTexture.Frame(1, Main.projFrames[Projectile.type], 0, Projectile.frame);
            Main.EntitySpriteDraw(LaserBeginTexture,
                             Projectile.Center - Main.screenPosition,
                             beginFrame,
                             Color.White,
                             Projectile.rotation,
                             beginFrame.Size() / 2f,
                             Projectile.scale,
                             SpriteEffects.None,
                             0);

            // Prepare things for body drawing.
            float laserBodyLength = LaserLength;
            laserBodyLength -= (LaserBeginTexture.Height * 0.5f + LaserEndTexture.Height) * Projectile.scale / Main.projFrames[Projectile.type];
            Vector2 centerOnLaser = Projectile.Center;

            // Body drawing.
            Rectangle middleFrame = LaserMiddleTexture.Frame(1, Main.projFrames[Projectile.type], 0, Projectile.frame);
            if (laserBodyLength > 30f)
            {
                float laserOffset = (LaserMiddleTexture.Height - 10f) * Projectile.scale / Main.projFrames[Projectile.type];
                float incrementalBodyLength = 0f;
                while (incrementalBodyLength + 1f < laserBodyLength)
                {
                    Main.EntitySpriteDraw(LaserMiddleTexture,
                                     centerOnLaser - Main.screenPosition,
                                     middleFrame,
                                     Color.White,
                                     Projectile.rotation,
                                     middleFrame.Width * 0.5f * Vector2.UnitX,
                                     Projectile.scale,
                                     SpriteEffects.None,
                                     0);
                    incrementalBodyLength += laserOffset;
                    centerOnLaser += Projectile.velocity * laserOffset;
                }
            }

            // End texture drawing.
            Rectangle endFrame = LaserEndTexture.Frame(1, Main.projFrames[Projectile.type], 0, Projectile.frame);
            Vector2 laserEndCenter = centerOnLaser - Main.screenPosition;
            Main.EntitySpriteDraw(LaserEndTexture,
                             laserEndCenter,
                             endFrame,
                             Color.White,
                             Projectile.rotation,
                             endFrame.Size() * new Vector2(0.5f, 0f),
                             Projectile.scale,
                             SpriteEffects.None,
                             0);
            return false;
        }
    }
}
