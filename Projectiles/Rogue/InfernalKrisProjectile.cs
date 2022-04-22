using CalamityMod.Items.Weapons.Rogue;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Rogue
{
    public class InfernalKrisProjectile : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/InfernalKris";

        public static int spinTime = 280;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Infernal Kris");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.penetrate = 2;
            Projectile.timeLeft = 300;
            Projectile.Calamity().rogue = true;
        }

        public override void AI()
        {
            if (Projectile.timeLeft < spinTime)
            {
                Projectile.rotation += 0.4f * Projectile.direction;

                float minScale = 1.9f;
                float maxScale = 2.5f;
                int dust = Dust.NewDust(Projectile.position - new Vector2(10, 10), 30, 30, 6, Projectile.velocity.X, Projectile.velocity.Y, 0, default, Main.rand.NextFloat(minScale, maxScale));
                Main.dust[dust].noGravity = true;
            }
            else
            {
                Projectile.rotation = (float)Math.Atan2((double)Projectile.velocity.Y, (double)Projectile.velocity.X) + 0.785f;
            }

            Projectile.velocity.Y += 0.01f;
            if (Projectile.velocity.Y > 10f)
            {
                Projectile.velocity.Y = 10f;
            }
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (Projectile.timeLeft < spinTime)
            {
                damage = (int)(Projectile.damage * 1.75f) + Main.rand.Next(0, 6);
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            int debuffTime = 60 * (Projectile.Calamity().stealthStrike ? Main.rand.Next(4,8) : Main.rand.Next(3,6));
            target.AddBuff(BuffID.OnFire, debuffTime);

            if (Projectile.Calamity().stealthStrike)
            {
                int sparkCount = Main.rand.Next(3, 7);
                for (int i = 0; i < sparkCount; i++)
                {
                    int sparkScatter = 1;
                    Vector2 sparkVelocity = new Vector2(Main.rand.NextFloat(-sparkScatter, sparkScatter), Main.rand.NextFloat(-sparkScatter - 2, sparkScatter + 2));

                    sparkVelocity.Normalize();
                    sparkVelocity *= 3;

                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, sparkVelocity, ModContent.ProjectileType<InfernalKrisCinder>(), (int)(Projectile.damage * 0.4f), 0, Projectile.owner, 0, 0);
                }
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<InfernalKrisExplosion>(), (int)(Projectile.damage * 0.4f), 0, Projectile.owner, 0, 0);
                SoundEngine.PlaySound(SoundID.Item74, Projectile.position);
            }
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            int debuffTime = 60 * (Projectile.Calamity().stealthStrike ? Main.rand.Next(4,8) : Main.rand.Next(3,6));
            target.AddBuff(BuffID.OnFire, debuffTime);

            if (Projectile.Calamity().stealthStrike)
            {
                int sparkCount = Main.rand.Next(3, 7);
                for (int i = 0; i < sparkCount; i++)
                {
                    int sparkScatter = 1;
                    Vector2 sparkVelocity = new Vector2(Main.rand.NextFloat(-sparkScatter, sparkScatter), Main.rand.NextFloat(-sparkScatter - 2, sparkScatter + 2));

                    sparkVelocity.Normalize();
                    sparkVelocity *= 3;

                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, sparkVelocity, ModContent.ProjectileType<InfernalKrisCinder>(), (int)(Projectile.damage * 0.4f), 0, Projectile.owner, 0, 0);
                }
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<InfernalKrisExplosion>(), (int)(Projectile.damage * 0.4f), 0, Projectile.owner, 0, 0);
                SoundEngine.PlaySound(SoundID.Item74, Projectile.position);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.Calamity().stealthStrike)
            {
                // If this is a stealth strike, make the blade glow orange
                Color glowColour = new Color(255, 215, 100, 100);
                CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], glowColour, 1);

                float minScale = 1.9f;
                float maxScale = 2.5f;
                int dust = Dust.NewDust(Projectile.position, 10, 10, 6, Projectile.velocity.X, Projectile.velocity.Y, 0, default, Main.rand.NextFloat(minScale, maxScale));
                Main.dust[dust].noGravity = true;
            }
            else
            {
                CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            }
            return false;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Collision.HitTiles(Projectile.position + Projectile.velocity, Projectile.velocity, Projectile.width, Projectile.height);

            Projectile.ai[0] = 0;
            Projectile.ai[1] = 0;

            if (Projectile.velocity.X != oldVelocity.X)
            {
                if (oldVelocity.X < 0)
                {
                    Projectile.ai[0] = 1;
                }
                if (oldVelocity.X > 0)
                {
                    Projectile.ai[0] = -1;
                }
            }
            if (Projectile.velocity.Y != oldVelocity.Y)
            {
                if (oldVelocity.Y < 0)
                {
                    Projectile.ai[1] = 1;
                }
                if (oldVelocity.Y > 0)
                {
                    Projectile.ai[1] = -1;
                }
            }

            SoundEngine.PlaySound(SoundID.Item74, Projectile.position);
            Projectile.Kill();
            return false;
        }

        public override void Kill(int timeLeft)
        {
            if (Main.rand.Next(4) == 0)
            {
                Item.NewItem(Projectile.GetItemSource_DropAsItem(), (int)Projectile.position.X, (int)Projectile.position.Y, Projectile.width, Projectile.height, ModContent.ItemType<InfernalKris>());
            }

            if (Projectile.Calamity().stealthStrike)
            {
                int sparkCount = Main.rand.Next(3, 7);
                for (int i = 0; i < sparkCount; i++)
                {
                    int sparkScatter = 1;
                    Vector2 sparkVelocity = new Vector2(Main.rand.NextFloat(-sparkScatter, sparkScatter), Main.rand.NextFloat(-sparkScatter, sparkScatter));

                    if (Projectile.ai[0] != 0)
                    {
                        sparkVelocity.X *= -1;
                    }
                    if (Projectile.ai[1] != 0)
                    {
                        sparkVelocity.Y *= -1;
                    }

                    sparkVelocity.X += Projectile.ai[0];
                    sparkVelocity.Y += Projectile.ai[1];
                    sparkVelocity.Normalize();
                    sparkVelocity *= 3;

                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, sparkVelocity, ModContent.ProjectileType<InfernalKrisCinder>(), (int)(Projectile.damage * 0.4f), 0, Projectile.owner, 0, 0);
                }
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<InfernalKrisExplosion>(), (int)(Projectile.damage * 0.4f), 0, Projectile.owner, 0, 0);
                SoundEngine.PlaySound(SoundID.Item74, Projectile.position);
            }
        }
    }
}
