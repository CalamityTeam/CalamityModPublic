using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Particles;
using CalamityMod.Items.Weapons.Melee;
using Terraria.Graphics.Shaders;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
using static CalamityMod.CalamityUtils;

namespace CalamityMod.Projectiles.Melee
{
    public class AriesWrathConstellation : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";
        public Player Owner => Main.player[projectile.owner];
        public float Timer => 20 - projectile.timeLeft;

        public Vector2 SizeVector;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Constellation");
        }
        public override void SetDefaults()
        {
            projectile.melee = true;
            projectile.width = projectile.height = 8;
            projectile.tileCollide = false;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 30;
            projectile.timeLeft = 20;
        }


        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float collisionPoint = 0f;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), projectile.Center, projectile.Center + SizeVector, 30f, ref collisionPoint);
        }

        public override void AI()
        {
            if (projectile.ai[0] == 0)
            {
                projectile.ai[0] = 1;
                Vector2 previousStar = projectile.Center;
                Vector2 offset;
                Particle Line;
                Particle Star = new GenericSparkle(previousStar, Vector2.Zero, Color.White, Color.Plum, Main.rand.NextFloat(1f, 1.5f), 20, 0f, 3f);
                GeneralParticleHandler.SpawnParticle(Star);


                for (float i = 0 + Main.rand.NextFloat(0.2f, 0.5f); i < 1; i += Main.rand.NextFloat(0.2f, 0.5f))
                {
                    offset = Main.rand.NextFloat(-50f, 50f) * Utils.SafeNormalize(SizeVector.RotatedBy(MathHelper.PiOver2), Vector2.Zero);
                    Star = new GenericSparkle(projectile.Center + SizeVector * i + offset, Vector2.Zero, Color.White, Color.Plum, Main.rand.NextFloat(1f, 1.5f), 20, 0f, 3f);
                    GeneralParticleHandler.SpawnParticle(Star);

                    Line = new BloomLineVFX(previousStar, projectile.Center + SizeVector * i + offset - previousStar, 0.8f, Color.MediumVioletRed * 0.75f, 20, true, true);
                    GeneralParticleHandler.SpawnParticle(Line);

                    if (Main.rand.Next(3) == 0)
                    {
                        offset = Main.rand.NextFloat(-50f, 50f) * Utils.SafeNormalize(SizeVector.RotatedBy(MathHelper.PiOver2), Vector2.Zero);
                        Star = new GenericSparkle(projectile.Center + SizeVector * i + offset, Vector2.Zero, Color.White, Color.Plum, Main.rand.NextFloat(1f, 1.5f), 20, 0f, 3f);
                        GeneralParticleHandler.SpawnParticle(Star);

                        Line = new BloomLineVFX(previousStar, projectile.Center + SizeVector * i + offset - previousStar, 0.8f, Color.MediumVioletRed * 0.75f, 20, true, true);
                        GeneralParticleHandler.SpawnParticle(Line);
                    }

                    previousStar = projectile.Center + SizeVector * i + offset;
                }

                Star = new GenericSparkle(projectile.Center + SizeVector, Vector2.Zero, Color.White, Color.Plum, Main.rand.NextFloat(1f, 1.5f), 20, 0f, 3f);
                GeneralParticleHandler.SpawnParticle(Star);

                Line = new BloomLineVFX(previousStar, projectile.Center + SizeVector - previousStar, 0.8f, Color.MediumVioletRed * 0.75f, 20, true);
                GeneralParticleHandler.SpawnParticle(Line);
            }
        }


        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.WriteVector2(SizeVector);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            SizeVector = reader.ReadVector2();
        }

    }
}