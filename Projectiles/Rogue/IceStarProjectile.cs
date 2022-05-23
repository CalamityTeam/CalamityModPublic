using CalamityMod.Projectiles.Typeless;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
namespace CalamityMod.Projectiles.Rogue
{
    public class IceStarProjectile : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/IceStar";

        private bool initStealth = false;
        private Vector2 initialVelocity;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Star");
        }

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.friendly = true;
            Projectile.penetrate = 3;
            Projectile.aiStyle = 2;
            Projectile.timeLeft = 280;
            AIType = ProjectileID.ThrowingKnife;
            Projectile.DamageType = RogueDamageClass.Instance;
            Projectile.coldDamage = true;
        }

        public override void AI()
        {
            if (!initStealth && Projectile.Calamity().stealthStrike)
            {
                Projectile.penetrate = -1;
                Projectile.usesLocalNPCImmunity = true;
                Projectile.localNPCHitCooldown = 9;
                Projectile.tileCollide = false;
                initialVelocity = Projectile.velocity;
                initStealth = true;
            }

            Projectile.rotation += 0.5f;
            if (Main.rand.NextBool(3))
            {
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, 67, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f);
            }
            Vector2 center = Projectile.Center;
            float maxDistance = Projectile.Calamity().stealthStrike ? 800f : 400f;
            bool homeIn = false;

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                if (Main.npc[i].CanBeChasedBy(Projectile, false))
                {
                    float extraDistance = (float)(Main.npc[i].width / 2) + (float)(Main.npc[i].height / 2);

                    if (Vector2.Distance(Main.npc[i].Center, Projectile.Center) < (maxDistance + extraDistance) && Collision.CanHit(Projectile.Center, 1, 1, Main.npc[i].Center, 1, 1))
                    {
                        center = Main.npc[i].Center;
                        homeIn = true;
                        break;
                    }
                }
            }

            if (homeIn)
            {
                Vector2 moveDirection = Projectile.SafeDirectionTo(center, Vector2.UnitY);
                Projectile.velocity = (Projectile.velocity * 20f + moveDirection * 14f) / (21f);
            }
            Projectile.velocity = initStealth && !homeIn ? initialVelocity : Projectile.velocity;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, tex.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }

        public override void Kill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item27, Projectile.position);
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, 67, Projectile.oldVelocity.X * 0.5f, Projectile.oldVelocity.Y * 0.5f);
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (initStealth)
            {
                if (Projectile.owner == Main.myPlayer && Main.player[Projectile.owner].ownedProjectileCounts[ModContent.ProjectileType<KelvinCatalystStar>()] < 15)
                {
                    float spread = 45f * 0.0174f;
                    double startAngle = Math.Atan2(Projectile.velocity.X, Projectile.velocity.Y) - spread / 2;
                    double deltaAngle = spread / 8f;
                    double offsetAngle;
                    int i;
                    for (i = 0; i < 4; i++)
                    {
                        offsetAngle = startAngle + deltaAngle * (i + i * i) / 2f + 32f * i;

                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X, Projectile.Center.Y, (float)(Math.Sin(offsetAngle) * 4f), (float)(Math.Cos(offsetAngle) * 4f),
                            ModContent.ProjectileType<KelvinCatalystStar>(), Projectile.damage / 8, Projectile.knockBack * 0.5f, Projectile.owner, 0f, 0f);

                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X, Projectile.Center.Y, (float)(-Math.Sin(offsetAngle) * 4f), (float)(-Math.Cos(offsetAngle) * 4f),
                            ModContent.ProjectileType<KelvinCatalystStar>(), Projectile.damage / 8, Projectile.knockBack * 0.5f, Projectile.owner, 0f, 0f);
                    }
                }
            }
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            if (initStealth)
            {
                if (Projectile.owner == Main.myPlayer && Main.player[Projectile.owner].ownedProjectileCounts[ModContent.ProjectileType<KelvinCatalystStar>()] < 15)
                {
                    float spread = 45f * 0.0174f;
                    double startAngle = Math.Atan2(Projectile.velocity.X, Projectile.velocity.Y) - spread / 2;
                    double deltaAngle = spread / 8f;
                    double offsetAngle;
                    int i;
                    for (i = 0; i < 4; i++)
                    {
                        offsetAngle = startAngle + deltaAngle * (i + i * i) / 2f + 32f * i;

                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X, Projectile.Center.Y, (float)(Math.Sin(offsetAngle) * 4f), (float)(Math.Cos(offsetAngle) * 4f),
                            ModContent.ProjectileType<KelvinCatalystStar>(), Projectile.damage / 8, Projectile.knockBack * 0.5f, Projectile.owner, 0f, 0f);

                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X, Projectile.Center.Y, (float)(-Math.Sin(offsetAngle) * 4f), (float)(-Math.Cos(offsetAngle) * 4f),
                            ModContent.ProjectileType<KelvinCatalystStar>(), Projectile.damage / 8, Projectile.knockBack * 0.5f, Projectile.owner, 0f, 0f);
                    }
                }
            }
        }
    }
}
