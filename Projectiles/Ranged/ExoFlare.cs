using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    // Photoviscerator right click split projectile (attached flares to the flare cluster)
    public class ExoFlare : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public float OffsetSpeed
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }
        public float OffsetRotation
        {
            get => Projectile.ai[1];
            set => Projectile.ai[1] = value;
        }
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Exo Flare");
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 20;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 24;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 16;
            Projectile.timeLeft = 160;
        }
        public override void AI()
        {
            if (Projectile.localAI[0] == 0f)
            {
                OffsetRotation = Main.rand.NextFloat(MathHelper.TwoPi);
                OffsetSpeed = Main.rand.NextFloat(MathHelper.ToRadians(2.5f), MathHelper.ToRadians(4f)) * Main.rand.NextBool(2).ToDirectionInt();
                Projectile.localAI[0] = 1f;
            }

            // Ensure that the owner projectile index is a valid one.
            if (!Main.projectile.IndexInRange((int)Projectile.localAI[1]))
            {
                Projectile.Kill();
                return;
            }

            Projectile owner = Main.projectile[(int)Projectile.localAI[1]];

            // Ensure the owner is the correct projectile.
            if (owner.type != ModContent.ProjectileType<ExoFlareCluster>())
                Projectile.Kill();

            // Movement around the owner.
            Projectile.Center = owner.Center + OffsetRotation.ToRotationVector2() * (float)Math.Cos(OffsetRotation * 0.3f) * owner.Size * 0.5f;
            Projectile.rotation = (Projectile.position - Projectile.oldPos[1]).ToRotation();
            OffsetRotation += OffsetSpeed;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D lightTexture = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/PhotovisceratorLight").Value;
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                float colorInterpolation = (float)Math.Cos(Projectile.timeLeft / 16f + Main.GlobalTimeWrappedHourly / 20f + i / (float)Projectile.oldPos.Length * MathHelper.Pi) * 0.5f + 0.5f;
                Color color = Color.Lerp(Color.LightGreen, Color.LightPink, colorInterpolation) * 0.4f;
                color.A = 0;
                Vector2 drawPosition = Projectile.oldPos[i] + lightTexture.Size() * 0.5f - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);
                Color outerColor = color;
                Color innerColor = color * 0.5f;
                float intensity = 0.9f + 0.15f * (float)Math.Cos(Main.GlobalTimeWrappedHourly % 60f * MathHelper.TwoPi);

                // Become smaller the futher along the old positions we are.
                intensity *= MathHelper.Lerp(0.15f, 1f, 1f - i / (float)Projectile.oldPos.Length);

                Vector2 outerScale = new Vector2(1.65f) * intensity;
                Vector2 innerScale = new Vector2(1.65f) * intensity * 0.7f;
                outerColor *= intensity;
                innerColor *= intensity;
                Main.EntitySpriteDraw(lightTexture, drawPosition, null, outerColor, 0f, lightTexture.Size() * 0.5f, outerScale * 0.6f, SpriteEffects.None, 0);
                Main.EntitySpriteDraw(lightTexture, drawPosition, null, innerColor, 0f, lightTexture.Size() * 0.5f, innerScale * 0.6f, SpriteEffects.None, 0);
            }
            return false;
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit) => target.ExoDebuffs(2f);
        public override void OnHitPvp(Player target, int damage, bool crit) => target.ExoDebuffs(2f);
    }
}
