using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Ranged
{
    public class ContagionBow : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/Ranged/Contagion";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Contagion");
        }

        public override void SetDefaults()
        {
            projectile.width = 42;
            projectile.height = 84;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.ranged = true;
        }

        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            Vector2 vector = player.RotatedRelativePoint(player.MountedCenter, true);
            if (projectile.type == ModContent.ProjectileType<ContagionBow>())
            {
                projectile.ai[0] += 1f;
                int num2 = 0;
                if (projectile.ai[0] >= 40f)
                {
                    num2++;
                }
                if (projectile.ai[0] >= 80f)
                {
                    num2++;
                }
                if (projectile.ai[0] >= 120f)
                {
                    num2++;
                }
                int num3 = 24;
                int num4 = 6;
                projectile.ai[1] += 1f;
                bool flag = false;
                if (projectile.ai[1] >= (float)(num3 - num4 * num2))
                {
                    projectile.ai[1] = 0f;
                    flag = true;
                }
                projectile.frameCounter += 1 + num2;
                if (projectile.frameCounter >= 4)
                {
                    projectile.frameCounter = 0;
                    projectile.frame++;
                    if (projectile.frame >= 3)
                    {
                        projectile.frame = 0;
                    }
                }
                if (projectile.soundDelay <= 0)
                {
                    projectile.soundDelay = num3 - num4 * num2;
                    if (projectile.ai[0] != 1f)
                    {
                        Main.PlaySound(SoundID.Item5, projectile.Center);
                    }
                }
                if (projectile.ai[1] == 1f && projectile.ai[0] != 1f)
                {
                    Vector2 vector2 = Vector2.UnitX * 24f;
                    vector2 = vector2.RotatedBy((double)(projectile.rotation - MathHelper.PiOver2), default);
                    Vector2 value = projectile.Center + vector2;
                    for (int i = 0; i < 2; i++)
                    {
                        int num5 = Dust.NewDust(value - Vector2.One * 8f, 16, 16, 44, projectile.velocity.X / 2f, projectile.velocity.Y / 2f, 100, default, 0.25f);
                        Main.dust[num5].velocity *= 0.66f;
                        Main.dust[num5].noGravity = true;
                        Main.dust[num5].scale = 1.4f;
                    }
                }
                if (flag && Main.myPlayer == projectile.owner)
                {
                    bool flag2 = player.channel && !player.noItems && !player.CCed;
                    if (flag2)
                    {
                        float speed = player.ActiveItem().shootSpeed * projectile.scale;
                        Vector2 spawnPos = vector;
                        Vector2 direction = Main.screenPosition + new Vector2((float)Main.mouseX, (float)Main.mouseY) - spawnPos;
                        if (player.gravDir == -1f)
                        {
                            direction.Y = (float)(Main.screenHeight - Main.mouseY) + Main.screenPosition.Y - spawnPos.Y;
                        }
                        Vector2 velocity = Vector2.Normalize(direction);
                        if (float.IsNaN(velocity.X) || float.IsNaN(velocity.Y))
                        {
                            velocity = -Vector2.UnitY;
                        }
                        velocity *= speed;
                        if (velocity.X != projectile.velocity.X || velocity.Y != projectile.velocity.Y)
                        {
                            projectile.netUpdate = true;
                        }
                        projectile.velocity = velocity;
                        int projType = ModContent.ProjectileType<ContagionArrow>();
                        float velocityMult = 14f;
                        float randNum = 7f;
                        spawnPos += new Vector2(Main.rand.NextFloat(-randNum, randNum), Main.rand.NextFloat(-randNum, randNum));
                        Vector2 spinningpoint = Vector2.Normalize(projectile.velocity) * velocityMult;
                        spinningpoint = spinningpoint.RotatedBy(Main.rand.NextDouble() * 0.2 - 0.1, default);
                        if (float.IsNaN(spinningpoint.X) || float.IsNaN(spinningpoint.Y))
                        {
                            spinningpoint = -Vector2.UnitY;
                        }
                        Projectile.NewProjectile(spawnPos, spinningpoint, projType, projectile.damage, projectile.knockBack, projectile.owner, 0f, 0f);
                    }
                    else
                    {
                        projectile.Kill();
                    }
                }
            }
            projectile.rotation = projectile.velocity.ToRotation();
            Vector2 displayOffset = new Vector2(5f, 0f).RotatedBy(projectile.rotation);
            projectile.Center = player.RotatedRelativePoint(player.MountedCenter, true) + displayOffset;
            if (projectile.spriteDirection == -1)
                projectile.rotation += MathHelper.Pi;
            projectile.spriteDirection = projectile.direction;
            projectile.timeLeft = 2;
            player.ChangeDir(projectile.direction);
            player.heldProj = projectile.whoAmI;
            player.itemTime = 2;
            player.itemAnimation = 2;
            player.itemRotation = (float)Math.Atan2((double)(projectile.velocity.Y * (float)projectile.direction), (double)(projectile.velocity.X * (float)projectile.direction));
        }

        public override bool CanDamage() => false;

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor) => projectile.ai[0] > 0f;
    }
}
