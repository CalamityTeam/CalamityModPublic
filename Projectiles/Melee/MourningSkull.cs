using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee
{
    public class MourningSkull : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.CultistIsResistantTo[Type] = true;
            ProjectileID.Sets.TrailCacheLength[Type] = 10;
            ProjectileID.Sets.TrailingMode[Type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 26;
            Projectile.height = 26;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = 5;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.timeLeft = 600;
            Projectile.alpha = 255;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.extraUpdates = 2;
        }

        public override void AI()
        {
            if (Projectile.ai[0] < 0f)
            {
                Projectile.alpha = 0;
            }
            if (Projectile.alpha > 0)
            {
                Projectile.alpha -= 50;
            }
            if (Projectile.alpha < 0)
            {
                Projectile.alpha = 0;
            }

            if (Projectile.velocity.X < 0f)
            {
                Projectile.spriteDirection = -1;
                Projectile.rotation = (float)Math.Atan2((double)-(double)Projectile.velocity.Y, (double)-(double)Projectile.velocity.X);
            }
            else
            {
                Projectile.spriteDirection = 1;
                Projectile.rotation = Projectile.velocity.ToRotation();
            }

            if (Projectile.ai[0] >= 0f && Projectile.ai[0] < 200f)
            {
                NPC potentialTarget = null;
                if (Main.npc[(int)Projectile.ai[0]].active && Projectile.localNPCImmunity[(int)Projectile.ai[0]] == 0)
                    potentialTarget = Main.npc[(int)Projectile.ai[0]];
                else
                {
                    float range = 1600f;
                    foreach (NPC target in Main.ActiveNPCs)
                    {
                        if (target.CanBeChasedBy(Projectile) && Projectile.localNPCImmunity[target.whoAmI] == 0)
                        {
                            float distance = Vector2.Distance(target.Center, Projectile.Center);
                            if (distance < range)
                            {
                                range = distance;
                                potentialTarget = target;
                            }
                        }
                    }
                }

                if (potentialTarget != null)
                {
                    Projectile.alpha = 0;
                    Vector2 idealVelocity = Projectile.SafeDirectionTo(potentialTarget.Center) * 18f;
                    Projectile.velocity = (Projectile.velocity * 14f + idealVelocity) / 15f;
                }

                if (Projectile.velocity.X < 0f)
                {
                    Projectile.spriteDirection = -1;
                    Projectile.rotation = (float)Math.Atan2((double)-(double)Projectile.velocity.Y, (double)-(double)Projectile.velocity.X);
                }
                else
                {
                    Projectile.spriteDirection = 1;
                    Projectile.rotation = Projectile.velocity.ToRotation();
                }

                int eightConst = 8;
                int mourningDust = Dust.NewDust(new Vector2(Projectile.position.X + (float)eightConst, Projectile.position.Y + (float)eightConst), Projectile.width - eightConst * 2, Projectile.height - eightConst * 2, Main.rand.NextBool() ? 5 : 6, 0f, 0f, 0, default, 1f);
                Dust dust = Main.dust[mourningDust];
                dust.velocity *= 0.5f;
                dust = Main.dust[mourningDust];
                dust.velocity += Projectile.velocity * 0.5f;
                Main.dust[mourningDust].noGravity = true;
                Main.dust[mourningDust].noLight = true;
                Main.dust[mourningDust].scale = 1.4f;
                return;
            }

            Projectile.Kill();
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(200, 200, 200, Main.rand.Next(0, 128));
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Type], lightColor, 1);
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item10, Projectile.position);
            for (int i = 0; i < 5; i++)
            {
                int bloody = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Blood, 0f, 0f, 100, default, 2f);
                Main.dust[bloody].velocity *= 3f;
                if (Main.rand.NextBool())
                {
                    Main.dust[bloody].scale = 0.5f;
                    Main.dust[bloody].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                }
            }
            for (int j = 0; j < 10; j++)
            {
                int fiery = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, 0f, 0f, 100, default, 3f);
                Main.dust[fiery].noGravity = true;
                Main.dust[fiery].velocity *= 5f;
                fiery = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, 0f, 0f, 100, default, 2f);
                Main.dust[fiery].velocity *= 2f;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Daybreak, 300);
            if (Projectile.owner == Main.myPlayer && Projectile.numHits == 0)
            {
                for (int k = 0; k < 2; k++)
                {
                    Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.InfernoFork, Projectile.oldVelocity.X * 0.5f, Projectile.oldVelocity.Y * 0.5f);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position.X, Projectile.position.Y, Main.rand.Next(-35, 36) * 0.2f, Main.rand.Next(-35, 36) * 0.2f, ModContent.ProjectileType<TinyFlare>(),
                     (int)(Projectile.damage * 0.35), Projectile.knockBack * 0.35f, Main.myPlayer, 0f, 0f);
                }
            }
        }
    }
}
