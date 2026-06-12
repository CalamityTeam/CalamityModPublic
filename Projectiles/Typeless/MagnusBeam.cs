using System;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Dusts;
using CalamityMod.Graphics.Primitives;
using CalamityMod.Items.Weapons.Typeless;
using CalamityMod.Particles;
using CalamityMod.Projectiles.Healing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Typeless
{
    public class MagnusBeam : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Typeless";

        public ref float ProximityFactor => ref Projectile.ai[1];

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.CultistIsResistantTo[Type] = true;
            ProjectileID.Sets.TrailCacheLength[Type] = 30;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 1;
            Projectile.MaxUpdates = 4;
            Projectile.timeLeft = 120 * Projectile.MaxUpdates;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            if (Projectile.ai[0] == 0f)
            {
                for (int i = 0; i < 6; i++)
                {
                    Vector2 velocity = (MathHelper.TwoPi * i / 6f + Projectile.rotation + MathHelper.ToRadians(30f)).ToRotationVector2() * 0.8f;
                    Color crossColor = i % 2 == 1 ? Color.MidnightBlue : Color.CornflowerBlue;
                    Particle cross = new GlowSparkParticle(Projectile.Center, velocity, false, 6, 0.015f, crossColor, Vector2.One, true);
                    GeneralParticleHandler.SpawnParticle(cross);
                }
                Projectile.ai[0] = 1f;
            }

            // Find the closest NPC targetable
            Color trailColor = Color.CornflowerBlue;
            float range = 320f;
            int targetNPC = -1;
            foreach (NPC target in Main.ActiveNPCs)
            {
                if (!target.CanBeChasedBy(Projectile))
                    continue;

                float distance = Vector2.Distance(target.Center, Projectile.Center);
                if (distance < range && Collision.CanHit(Projectile, target))
                {
                    range = distance;
                    targetNPC = target.whoAmI;
                }
            }
            if (targetNPC > -1)
            {
                NPC target = Main.npc[targetNPC];
                Vector2 idealVelocity = Projectile.SafeDirectionTo(target.Center) * 12f;
                Projectile.velocity = (Projectile.velocity * 29f + idealVelocity) / 30f;
                Projectile.velocity = Projectile.velocity.MoveTowards(idealVelocity, 1f);
                ProximityFactor = Utils.GetLerpValue(320f, 0f, Vector2.Distance(Projectile.Center, target.Center), true);
            }
            trailColor = Color.Lerp(Color.CornflowerBlue, Color.Magenta, ProximityFactor);
            Lighting.AddLight(Projectile.Center, trailColor.ToVector3() * 0.5f);

            Dust trail = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<LightDust>(), Projectile.velocity * 0.05f);
            trail.noGravity = true;
            trail.scale = Main.rand.NextFloat(0.8f, 1f);
            trail.color = trailColor;

            Vector2 sinOffset = (Vector2.UnitY * MathF.Sin(Projectile.timeLeft * MathHelper.Pi * 0.05f) * 24f).RotatedBy(Projectile.rotation);
            Dust offTrail = Dust.NewDustPerfect(Projectile.Center + sinOffset, DustID.SpectreStaff, Main.rand.NextVector2Circular(0.2f, 0.2f));
            offTrail.noGravity = true;
            offTrail.scale = Main.rand.NextFloat(1.2f, 1.8f);
            offTrail.alpha = Main.rand.Next(120, 180 + 1);
        }

        internal float WidthFunction(float completionRatio, Vector2 vertexPos) => Projectile.scale * 24f;
        internal Color ColorFunction(float completionRatio, Vector2 vertexPos)
        {
            Vector3 trailColor = Main.rgbToHsl(Color.Lerp(Color.CornflowerBlue, Color.Magenta, ProximityFactor));
            Vector3 endColor = trailColor + new Vector3(0.1f + MathF.Sin(Main.GlobalTimeWrappedHourly * 5f) * 0.05f, 0f, 0.1f);
            return Main.hslToRgb(Vector3.Lerp(trailColor, endColor, Utils.GetLerpValue(0f, 0.72f, completionRatio, true))) * Utils.GetLerpValue(0.8f, 0.54f, completionRatio, true) * Projectile.Opacity;
        }

        public override void PostDraw(Color lightColor)
        {
            GameShaders.Misc["CalamityMod:ImpFlameTrail"].SetShaderTexture(ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Trails/ScarletDevilStreak"));
            PrimitiveRenderer.RenderTrail(Projectile.oldPos, new(WidthFunction, ColorFunction, (_,_) => Projectile.Size * 0.5f, shader: GameShaders.Misc["CalamityMod:ImpFlameTrail"]), 30);
            Texture2D glow = TextureAssets.Projectile[Type].Value;
            Main.EntitySpriteDraw(glow, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, glow.Size() * 0.5f, Projectile.scale, SpriteEffects.None);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<MarkedforDeath>(), 480);

            Player player = Main.player[Projectile.owner];
            Main.player[Projectile.owner].SpawnLifeStealProjectile(target, Projectile, ModContent.ProjectileType<RoyalHeal>(), (int)Math.Round(hit.Damage * 0.1), 0.75f);
            int manaHeal = Math.Clamp(player.statManaMax2 - player.statMana, 0, 20);
            if (manaHeal > 0)
            {
                player.statMana += manaHeal;
                player.ManaEffect(manaHeal);
            }
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(EyeofMagnus.ImpactSound, Projectile.Center);

            if (Projectile.owner == Main.myPlayer)
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<MagnusBoom>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
        }
    }
}
