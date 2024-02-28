using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Graphics.Primitives;
using CalamityMod.NPCs.AcidRain;
using CalamityMod.Projectiles.BaseProjectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Enemy
{
    public class GammaRayBurst : BaseLaserbeamProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Enemy";

        public int OwnerIndex
        {
            get => (int)Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        public override float MaxScale => 10f;
        public override float MaxLaserLength => 4800f;
        public override float Lifetime => 300;
        public override Color LaserOverlayColor => new Color(0, 200, 50, 100);
        public override Color LightCastColor => Color.White;
        public override Texture2D LaserBeginTexture => ModContent.Request<Texture2D>(Texture).Value;
        public override Texture2D LaserMiddleTexture => ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Lasers/AresLaserBeamMiddle", AssetRequestMode.ImmediateLoad).Value;
        public override Texture2D LaserEndTexture => ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Lasers/AresLaserBeamEnd", AssetRequestMode.ImmediateLoad).Value;
        public override string Texture => "CalamityMod/Projectiles/Boss/AresLaserBeamStart";

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 5;
            ProjectileID.Sets.DrawScreenCheckFluff[Projectile.type] = 10000;
        }

        public override void SetDefaults()
        {
            Projectile.Calamity().DealsDefenseDamage = true;
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.hostile = true;
            Projectile.alpha = 255;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 630;
            CooldownSlot = ImmunityCooldownID.Bosses;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(Projectile.localAI[0]);
            writer.Write(Projectile.localAI[1]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Projectile.localAI[0] = reader.ReadSingle();
            Projectile.localAI[1] = reader.ReadSingle();
        }

        public override void AttachToSomething()
        {
            if (Main.npc[OwnerIndex].active && Main.npc[OwnerIndex].type == ModContent.NPCType<NuclearTerror>())
            {
                Projectile.Center = Main.npc[OwnerIndex].Center;
            }

            // Die of the owner is invalid in some way.
            else
            {
                Projectile.Kill();
                return;
            }
        }

        public override void UpdateLaserMotion()
        {
            Projectile.rotation += Main.npc[OwnerIndex].spriteDirection * 0.005f;
            Projectile.velocity = (Projectile.rotation - MathHelper.PiOver2).ToRotationVector2();
        }

        public override void PostAI()
        {
            // Determine frames.
            Projectile.frameCounter++;
            if (Projectile.frameCounter % 5f == 0f)
                Projectile.frame = (Projectile.frame + 1) % Main.projFrames[Projectile.type];
        }

        public float LaserWidthFunction(float _) => Projectile.scale * Projectile.width + 180;

        public static Color LaserColorFunction(float completionRatio)
        {
            float colorInterpolant = (float)Math.Sin(Main.GlobalTimeWrappedHourly * -3.2f + completionRatio * 23f) * 0.5f + 0.5f;
            return Color.Lerp(Color.LightGreen, Color.Lime, colorInterpolant * 0.67f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            // This should never happen, but just in case.
            if (Projectile.velocity == Vector2.Zero)
                return false;

            Vector2 laserEnd = Projectile.Center + Projectile.velocity.SafeNormalize(Vector2.UnitY) * LaserLength;
            Vector2[] baseDrawPoints = new Vector2[8];
            for (int i = 0; i < baseDrawPoints.Length; i++)
                baseDrawPoints[i] = Vector2.Lerp(Projectile.Center, laserEnd, i / (float)(baseDrawPoints.Length - 1f));

            // Select textures to pass to the shader, along with the electricity color.
            GameShaders.Misc["CalamityMod:ArtemisLaser"].UseColor(Color.Cyan);
            GameShaders.Misc["CalamityMod:ArtemisLaser"].UseImage1("Images/Extra_189");
            GameShaders.Misc["CalamityMod:ArtemisLaser"].UseImage2("Images/Misc/Perlin");

            PrimitiveSet.Prepare(baseDrawPoints, new(LaserWidthFunction, LaserColorFunction, shader: GameShaders.Misc["CalamityMod:ArtemisLaser"]), 64);
            return false;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (info.Damage <= 0)
                return;

            target.AddBuff(ModContent.BuffType<SulphuricPoisoning>(), 300);
        }

        public override bool CanHitPlayer(Player target) => Projectile.scale >= 0.5f;
    }
}
