using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Rogue
{
    public class DeepSeaDumbbell3 : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Rogue";
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/DeepSeaDumbbell";

        public override void SetDefaults()
        {
            Projectile.width = 26;
            Projectile.height = 26;
            Projectile.friendly = true;
            Projectile.DamageType = RogueDamageClass.Instance;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 1;
        }

        public override void AI()
        {
            if (Projectile.ai[0] < 60f)
                Projectile.ai[0] += 1f;
            else
            {
                CalamityUtils.HomeInOnNPC(Projectile, !Projectile.tileCollide, 300f, 12f, 20f);
            }

            Projectile.rotation += Math.Abs(Projectile.velocity.X) * 0.01f * (float)Projectile.direction;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            SoundEngine.PlaySound(SoundID.NPCDeath43 with { Volume = SoundID.NPCDeath43.Volume * 0.35f }, Projectile.position);

            if (Projectile.owner == Main.myPlayer)
            {
                float randVel = (float)Main.rand.Next(-35, 36) * 0.01f;
                float randVel2 = (float)Main.rand.Next(-35, 36) * 0.01f;
                int inc;
                for (int i = 0; i < 2; i = inc + 1)
                {
                    if (i == 1)
                    {
                        randVel *= 10f;
                        randVel2 *= 10f;
                    }
                    else
                    {
                        randVel *= -10f;
                        randVel2 *= -10f;
                    }

                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position.X, Projectile.position.Y, randVel, randVel2, ModContent.ProjectileType<DeepSeaDumbbellWeight>(),
                        (int)((double)Projectile.damage * 0.25), Projectile.knockBack * 0.25f, Main.myPlayer, 0f, 0f);

                    inc = i;
                }
            }

            Projectile.Kill();

            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<CrushDepth>(), 120);

            SoundEngine.PlaySound(SoundID.NPCDeath43 with { Volume = SoundID.NPCDeath43.Volume * 0.35f }, Projectile.position);

            if (Projectile.owner == Main.myPlayer)
            {
                float randVel = (float)Main.rand.Next(-35, 36) * 0.01f;
                float randVel2 = (float)Main.rand.Next(-35, 36) * 0.01f;
                int inc;
                for (int i = 0; i < 2; i = inc + 1)
                {
                    if (i == 1)
                    {
                        randVel *= 10f;
                        randVel2 *= 10f;
                    }
                    else
                    {
                        randVel *= -10f;
                        randVel2 *= -10f;
                    }

                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position.X, Projectile.position.Y, randVel, randVel2, ModContent.ProjectileType<DeepSeaDumbbellWeight>(),
                        (int)((double)Projectile.damage * 0.25), Projectile.knockBack * 0.25f, Main.myPlayer, 0f, 0f);

                    inc = i;
                }
            }

            Projectile.Kill();
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(ModContent.BuffType<CrushDepth>(), 120);

            SoundEngine.PlaySound(SoundID.NPCDeath43 with { Volume = SoundID.NPCDeath43.Volume * 0.35f }, Projectile.position);

            if (Projectile.owner == Main.myPlayer)
            {
                float randVel = (float)Main.rand.Next(-35, 36) * 0.01f;
                float randVel2 = (float)Main.rand.Next(-35, 36) * 0.01f;
                int inc;
                for (int i = 0; i < 2; i = inc + 1)
                {
                    if (i == 1)
                    {
                        randVel *= 10f;
                        randVel2 *= 10f;
                    }
                    else
                    {
                        randVel *= -10f;
                        randVel2 *= -10f;
                    }

                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position.X, Projectile.position.Y, randVel, randVel2, ModContent.ProjectileType<DeepSeaDumbbellWeight>(),
                        (int)((double)Projectile.damage * 0.25), Projectile.knockBack * 0.25f, Main.myPlayer, 0f, 0f);

                    inc = i;
                }
            }

            Projectile.Kill();
        }
    }
}
