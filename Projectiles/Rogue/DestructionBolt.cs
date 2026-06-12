using CalamityMod.Enums;
using CalamityMod.Graphics.Renderers;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class DestructionBolt : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Rogue";
        public ref float time => ref Projectile.ai[0];
        public float CenterX;
        public float CenterY;
        public float MouseX;
        public float MouseY;
        public int timerOffset;
        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.extraUpdates = 3;
            Projectile.timeLeft = 600;
            Projectile.DamageType = RogueDamageClass.Instance;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 15;
        }

        public override void AI()
        {
            Player Owner = Main.player[Projectile.owner];
            Projectile.velocity *= 0.99f;
            if (time == 0)
            {
                MouseX = Projectile.Center.X;
                MouseY = Projectile.Center.Y;
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
                timerOffset = (int)(60 - Projectile.ai[1] * 15);
                time += timerOffset;
            }
            if (time == 180 + timerOffset)
            {
                Vector2 mouse = Owner.ClampedMouseWorld();
                MouseX = mouse.X;
                MouseY = mouse.Y;
            }
            else if (time < 180)
            {
                MouseX = Projectile.Center.X;
                MouseY = Projectile.Center.Y;
            }
            if (time >= (180 - timerOffset))
            {
                if (time == (180 + timerOffset))
                {
                    CenterX = Projectile.Center.X;
                    CenterY = Projectile.Center.Y;
                }
                if (time >= 180 + timerOffset)
                {
                    Projectile.velocity = Vector2.Zero;
                    Projectile.rotation = Projectile.rotation.AngleLerp((new Vector2(MouseX, MouseY) - Projectile.Center).SafeNormalize(Vector2.UnitX).ToRotation() + MathHelper.PiOver2, 0.2f);
                    Projectile.Center = new Vector2(MathHelper.Lerp(CenterX, MouseX, Utils.GetLerpValue(180 + timerOffset, 300, time, true)), MathHelper.Lerp(CenterY, MouseY, Utils.GetLerpValue(180 + timerOffset, 300, time, true)));
                }
            }
            else
            {
                Projectile.rotation = Projectile.rotation.AngleLerp(Projectile.velocity.ToRotation() + MathHelper.PiOver2, 0.08f);
            }

            if (Main.rand.NextBool(4))
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(15, 15), Main.rand.NextBool(6) ? 278 : 263, -Projectile.velocity);
                dust.scale = dust.type == 278 ? Main.rand.NextFloat(0.3f, 0.6f) : Main.rand.NextFloat(0.6f, 1.4f);
                dust.velocity = -Projectile.velocity.RotatedByRandom(0.3f) * Main.rand.NextFloat(0.1f, 0.7f);
                dust.noGravity = true;
                dust.color = Color.LightGreen;
            }

            time++;
            if (time >= 300)
                Projectile.Kill();
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.SourceDamage *= 0.02f;
        }
        public override void OnKill(int timeLeft)
        {
            if ((Projectile.ai[1] >= 4 && Projectile.ai[2] == 0) || (Projectile.ai[1] == 4 && Projectile.ai[2] == 15))
            {
                Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<DestructionStar>(), Projectile.damage, Projectile.knockBack * 5, Projectile.owner);
                if (Projectile.ai[2] >= 1)
                {
                    proj.Calamity().stealthStrike = true;
                    proj.timeLeft = 240;
                }

                for (int i = 0; i < 2; i++)
                {
                    Particle blastRing = new CustomPulse(Projectile.Center, Vector2.Zero, Color.LightGreen with { A = 0 }, "CalamityMod/Particles/BloomCircle", Vector2.One, Main.rand.NextFloat(-10, 10), Projectile.ai[1] * 0.5f, Projectile.ai[1] * 0.3f, 15, false);
                    GeneralParticleHandler.SpawnParticle(blastRing, false, GeneralDrawLayer.AfterEverything);
                }
            }
            else
            {
                for (int i = 0; i < 3; i++)
                {
                    Particle blastRing = new CustomPulse(Projectile.Center, Vector2.Zero, Color.Black, "CalamityMod/Particles/SmallBloom", Vector2.One, Main.rand.NextFloat(-10, 10), Projectile.ai[1] * 0.25f, Projectile.ai[1] * 0.2f, 10, false);
                    GeneralParticleHandler.SpawnParticle(blastRing);
                }
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = Terraria.GameContent.TextureAssets.Projectile[Type].Value;
            Asset<Texture2D> tex2 = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Rogue/DestructionBoltGhost");
            Asset<Texture2D> tex3 = ModContent.Request<Texture2D>("CalamityMod/Particles/BloomCircle");
            float fading = Utils.GetLerpValue(180, 90, time, true);
            float fading2 = Utils.GetLerpValue(240, 180, time, true);
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, lightColor * fading, Projectile.rotation, tex.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
            for (int i = 0; i < 2; i++)
            {
                Main.EntitySpriteDraw(tex2.Value, Projectile.Center - Main.screenPosition, null, Color.LightGreen with { A = 0 } * (1 - fading) * fading2, Projectile.rotation, tex2.Size() / 2f, Projectile.scale * fading2, SpriteEffects.None, 0);
                Main.EntitySpriteDraw(tex3.Value, Projectile.Center - Main.screenPosition, null, Color.LightGreen with { A = 0 } * (1 - fading2), Projectile.rotation, tex3.Size() / 2f, Projectile.scale / 3, SpriteEffects.None, 0);
            }
            return false;
        }
    }
}
