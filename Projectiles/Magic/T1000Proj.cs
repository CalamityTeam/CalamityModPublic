using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Magic
{
    public class T1000Proj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("T1000");
            Main.projFrames[projectile.type] = 8;
        }

        public override void SetDefaults()
        {
            projectile.width = 80;
            projectile.height = 34;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.magic = true;
        }

        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            Vector2 playerPos = player.RotatedRelativePoint(player.MountedCenter, true);
            if (projectile.type == ModContent.ProjectileType<T1000Proj>())
            {
                projectile.ai[0] += 1f;
                int num2 = 0;
                if (projectile.ai[0] >= 20f)
                {
                    num2++;
                }
                if (projectile.ai[0] >= 40f)
                {
                    num2++;
                }
                if (projectile.ai[0] >= 60f)
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
                if (projectile.soundDelay <= 0)
                {
                    projectile.soundDelay = num3 - num4 * num2;
                    if (projectile.ai[0] != 1f)
                    {
                        Main.PlaySound(SoundID.Item91, projectile.position);
                    }
                    projectile.frame = (projectile.frame + 1) % Main.projFrames[projectile.type];
                }
                if (projectile.ai[1] == 1f && projectile.ai[0] != 1f)
                {
                    Vector2 vector2 = Vector2.UnitX * 24f;
                    vector2 = vector2.RotatedBy((double)(projectile.rotation - MathHelper.PiOver2), default);
                    Vector2 value = projectile.Center + vector2;
                    for (int i = 0; i < 2; i++)
                    {
                        int num5 = Dust.NewDust(value - Vector2.One * 8f, 16, 16, 66, projectile.velocity.X / 2f, projectile.velocity.Y / 2f, 100, new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB), 1f);
                        Main.dust[num5].velocity *= 0.66f;
                        Main.dust[num5].noGravity = true;
                        Main.dust[num5].scale = 1.4f;
                    }
                }
                if (flag && Main.myPlayer == projectile.owner)
                {
                    int weaponDamage2 = player.GetWeaponDamage(player.ActiveItem());
                    bool flag2 = player.channel && player.CheckMana(player.ActiveItem(), -1, true, false) && !player.noItems && !player.CCed;
                    if (flag2)
                    {
                        float speed = player.ActiveItem().shootSpeed * projectile.scale;
                        Vector2 spawnPos = playerPos + projectile.velocity * 0.75f;
                        Vector2 velocity = (Main.MouseWorld - projectile.Center).SafeNormalize(-Vector2.UnitY) * speed;
                        if (projectile.WithinRange(Main.MouseWorld, 50f))
                            velocity = projectile.velocity;
                        if (velocity.X != projectile.velocity.X || velocity.Y != projectile.velocity.Y)
                            projectile.netUpdate = true;

                        projectile.velocity = velocity;
                        int projType = ModContent.ProjectileType<T1000Laser>();
                        float velocityMult = 14f;
                        float randNum = 4f;
                        for (int j = 0; j < 4; j++)
                        {
							spawnPos += new Vector2(Main.rand.NextFloat(-randNum, randNum), Main.rand.NextFloat(-randNum, randNum));
                            Vector2 spinningpoint = Vector2.Normalize(projectile.velocity) * velocityMult;
							spinningpoint = spinningpoint.RotatedBy(Main.rand.NextDouble() * 0.2 - 0.1, default);
                            if (float.IsNaN(spinningpoint.X) || float.IsNaN(spinningpoint.Y))
                            {
                                spinningpoint = -Vector2.UnitY;
                            }
                            Projectile.NewProjectile(spawnPos, spinningpoint, projType, weaponDamage2, projectile.knockBack, projectile.owner, 0f, 0f);
                        }
                    }
                    else
                    {
                        projectile.Kill();
                    }
                }
            }
            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2;
            projectile.Center = player.RotatedRelativePoint(player.MountedCenter, true) + projectile.velocity.SafeNormalize(Vector2.UnitX * player.direction) * 6f;
            projectile.spriteDirection = projectile.direction;
            projectile.timeLeft = 2;
            player.ChangeDir(projectile.direction);
            player.heldProj = projectile.whoAmI;
            player.itemTime = 2;
            player.itemAnimation = 2;
            player.itemRotation = (projectile.velocity * projectile.direction).ToRotation();
        }

        public override bool CanDamage() => false;

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor) => projectile.ai[0] > 0f;
    }
}
