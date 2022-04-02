using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.DraedonsArsenal
{
    public class GaussFlux : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public float Time
        {
            get => projectile.ai[0];
            set => projectile.ai[0] = value;
        }
        public NPC Target
        {
            get => Main.npc[(int)projectile.ai[1]];
            set => projectile.ai[1] = value.whoAmI;
        }
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Gauss Flux");
        }

        public override void SetDefaults()
        {
            projectile.width = 16;
            projectile.height = 16;
            projectile.friendly = true;
            projectile.melee = true;
            projectile.penetrate = -1;
            projectile.timeLeft = 180;
            projectile.usesIDStaticNPCImmunity = true;
            projectile.idStaticNPCHitCooldown = 10;
        }

        public override void AI()
        {
            Lighting.AddLight(projectile.Center, Color.Lime.ToVector3());
            if (!Target.active)
            {
                projectile.Kill();
                return;
            }
            projectile.Center = Target.Center;
            if (!Main.dedServ)
            {
                if (Time == 0)
                {
                    for (int i = 0; i < 60; i++)
                    {
                        Dust dust = Dust.NewDustPerfect(Target.Center, 261);
                        dust.color = Utils.SelectRandom(Main.rand, Color.Yellow, Color.YellowGreen);
                        dust.velocity = Main.rand.NextVector2Circular(20f, 20f);
                        dust.scale = 2f;
                        dust.noGravity = true;
                    }
                }
                for (int i = 0; i < 7; i++)
                {
                    for (int arcIndex = 0; arcIndex < 6; arcIndex++)
                    {
                        float offsetAngle = MathHelper.ToRadians(1080f) * i / 18f;
                        offsetAngle += Time / 10f;
                        float scale = 1.4f + (float)Math.Cos(i / 7f * MathHelper.TwoPi + Time / 30f) * 0.3f;
                        scale *= MathHelper.Lerp(1f, 0.4f, arcIndex / 6f);
                        Vector2 offset = Target.Size.RotatedBy(offsetAngle) * 0.5f;
                        offset += (arcIndex * MathHelper.TwoPi / 6f + Time / 20f).ToRotationVector2() * 6f * arcIndex;

                        Dust dust = Dust.NewDustPerfect(Target.Center + offset, 261);
                        dust.color = Utils.SelectRandom(Main.rand, Color.Yellow, Color.YellowGreen);
                        dust.velocity = Vector2.Zero;
                        dust.scale = scale;
                        dust.noGravity = true;
                    }
                }
            }
            Time++;
        }
    }
}
