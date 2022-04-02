using CalamityMod.Items.Weapons.Rogue;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Rogue
{
    public class GelDartProjectile : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/GelDart";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Dart");
        }

        public override void SetDefaults()
        {
            projectile.width = 12;
            projectile.height = 12;
            projectile.friendly = true;
            projectile.penetrate = 4;
            projectile.aiStyle = 2;
            projectile.timeLeft = 600;
            aiType = ProjectileID.ThrowingKnife;
            projectile.Calamity().rogue = true;
        }

        public override void AI()
        {
            if (projectile.owner == Main.myPlayer && projectile.Calamity().stealthStrike)
            {
                projectile.rotation += (Math.Abs(projectile.velocity.X) + Math.Abs(projectile.velocity.Y)) * 0.01f * (float)projectile.direction;
                if (projectile.timeLeft % 8 == 0)
                {
                    Vector2 velocity = new Vector2(Main.rand.NextFloat(-7f, 7f), Main.rand.NextFloat(-7f, 7f));
                    int slime = Projectile.NewProjectile(projectile.Center, velocity, ProjectileID.SlimeGun, (int)(projectile.damage * 0.5), projectile.knockBack * 0.5f, projectile.owner);
                    if (slime.WithinBounds(Main.maxProjectiles))
                    {
                        Main.projectile[slime].Calamity().forceRogue = true;
                        Main.projectile[slime].usesLocalNPCImmunity = true;
                        Main.projectile[slime].localNPCHitCooldown = 10;
                    }
                }
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            projectile.penetrate--;
            if (projectile.penetrate <= 0)
            {
                projectile.Kill();
            }
            else
            {
                if (projectile.Calamity().stealthStrike)
                {
                    if (projectile.velocity != oldVelocity)
                    {
                        projectile.velocity = Main.rand.NextFloat(-1.15f, -0.85f) * oldVelocity * 1.35f;
                        Main.PlaySound(SoundID.Item56, projectile.position); // Minecart bumper sound
                    }
                }
                else
                {
                    if (projectile.velocity.X != oldVelocity.X)
                    {
                        projectile.velocity.X = -oldVelocity.X;
                    }
                    if (projectile.velocity.Y != oldVelocity.Y)
                    {
                        projectile.velocity.Y = -oldVelocity.Y;
                    }
                }
            }
            return false;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D tex = Main.projectileTexture[projectile.type];
            spriteBatch.Draw(tex, projectile.Center - Main.screenPosition, null, projectile.GetAlpha(lightColor), projectile.rotation, tex.Size() / 2f, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }

        public override void Kill(int timeLeft)
        {
            if (Main.rand.NextBool(2))
            {
                Item.NewItem((int)projectile.position.X, (int)projectile.position.Y, projectile.width, projectile.height, ModContent.ItemType<GelDart>());
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Slimed, 120);
            if (projectile.Calamity().stealthStrike)
                target.AddBuff(BuffID.Slow, 120);
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.Slimed, 120);
            if (projectile.Calamity().stealthStrike)
                target.AddBuff(BuffID.Slow, 120);
        }
    }
}
