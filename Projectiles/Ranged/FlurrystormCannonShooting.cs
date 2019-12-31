using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class FlurrystormCannonShooting : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Flurrystorm Cannon");
            Main.projFrames[projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            projectile.width = 58;
            projectile.height = 34;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.ranged = true;
            projectile.ignoreWater = true;
			projectile.coldDamage = true;
        }

        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            float num = 0f;
            Vector2 vector = player.RotatedRelativePoint(player.MountedCenter, true);
            if (projectile.spriteDirection == -1)
            {
                num = 3.14159274f;
            }
            projectile.frameCounter++;
            if (projectile.frameCounter > 4)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
            }
            if (projectile.frame >= 2)
            {
                projectile.frame = 0;
            }
            projectile.ai[0] += 1f;
            int num39 = 0;
            float spreadMult = 0.15f;
            if (projectile.ai[0] >= 60f)
            {
                num39++;
            }
            if (projectile.ai[0] >= 120f)
            {
                num39++;
            }
            if (projectile.ai[0] >= 180f)
            {
                num39++;
            }
            if (projectile.ai[0] >= 240f)
            {
                num39++;
            }
            if (projectile.ai[0] >= 300f)
            {
                num39++;
            }
            if (projectile.ai[0] >= 360f)
            {
                num39++;
            }
            if (projectile.ai[0] >= 420f)
            {
                num39++;
            }
            if (projectile.ai[0] >= 480f) //full speed after 8 seconds
            {
                num39++;
            }
            int num40 = 20;
            int num41 = 2;
            projectile.ai[1] -= 1f;
            bool flag15 = false;
            if (projectile.ai[1] <= 0f)
            {
                projectile.ai[1] = (float)(num40 - num41 * num39);
                flag15 = true;
            }
            bool flag16 = player.channel && player.HasAmmo(player.inventory[player.selectedItem], true) && !player.noItems && !player.CCed;
            if (projectile.localAI[0] > 0f)
            {
                projectile.localAI[0] -= 1f;
            }
            if (projectile.soundDelay <= 0 && flag16)
            {
                projectile.soundDelay = num40 - num41 * num39;
                if (projectile.ai[0] != 1f)
                {
                    Main.PlaySound(SoundID.Item11, projectile.position);
                }
                projectile.localAI[0] = 12f;
            }
            if (flag15 && Main.myPlayer == projectile.owner)
            {
                int num42 = 14;
                float scaleFactor11 = 14f;
                int weaponDamage2 = player.GetWeaponDamage(player.inventory[player.selectedItem]);
                float weaponKnockback2 = player.inventory[player.selectedItem].knockBack;
                if (flag16)
                {
                    player.PickAmmo(player.inventory[player.selectedItem], ref num42, ref scaleFactor11, ref flag16, ref weaponDamage2, ref weaponKnockback2, false);
                    weaponKnockback2 = player.GetWeaponKnockback(player.inventory[player.selectedItem], weaponKnockback2);
                    float scaleFactor12 = player.inventory[player.selectedItem].shootSpeed * projectile.scale;
                    Vector2 vector19 = vector;
                    Vector2 value18 = Main.screenPosition + new Vector2((float)Main.mouseX, (float)Main.mouseY) - vector19;
                    if (player.gravDir == -1f)
                    {
                        value18.Y = (float)(Main.screenHeight - Main.mouseY) + Main.screenPosition.Y - vector19.Y;
                    }
                    Vector2 value19 = Vector2.Normalize(value18);
                    if (float.IsNaN(value19.X) || float.IsNaN(value19.Y))
                    {
                        value19 = -Vector2.UnitY;
                    }
                    value19 *= scaleFactor12;
                    if (value19.X != projectile.velocity.X || value19.Y != projectile.velocity.Y)
                    {
                        projectile.netUpdate = true;
                    }
                    projectile.velocity = value19 * 0.55f;

					Vector2 vector20 = Vector2.Normalize(projectile.velocity) * scaleFactor11 * (0.6f + Main.rand.NextFloat() * spreadMult);
					if (float.IsNaN(vector20.X) || float.IsNaN(vector20.Y))
					{
						vector20 = -Vector2.UnitY;
					}
					Vector2 vector21 = vector19 + Utils.RandomVector2(Main.rand, -5f, 5f);
					vector20.X += (float)Main.rand.Next(-15, 16) * spreadMult;
					vector20.Y += (float)Main.rand.Next(-15, 16) * spreadMult;
					int snowball = Projectile.NewProjectile(vector21.X, vector21.Y, vector20.X, vector20.Y, num42, weaponDamage2, weaponKnockback2, projectile.owner, 0f, 0f);
					Main.projectile[snowball].noDropItem = true;
					Main.projectile[snowball].Calamity().forceRanged = true;
					Main.projectile[snowball].thrown = false;
					Main.projectile[snowball].extraUpdates += Main.rand.Next(0,2);

					if (Main.rand.NextBool(5)) //ice chunk
					{
						Vector2 vector2 = Vector2.Normalize(projectile.velocity) * scaleFactor11 * (0.6f + Main.rand.NextFloat() * 0.8f);
						if (float.IsNaN(vector2.X) || float.IsNaN(vector2.Y))
						{
							vector2 = -Vector2.UnitY;
						}
						float speedY = vector2.Y;
						Vector2 vector3 = vector19 + Utils.RandomVector2(Main.rand, -15f, 15f);
						int iceChunk = Projectile.NewProjectile(vector3.X, vector3.Y, vector2.X, vector2.Y, ModContent.ProjectileType<FlurrystormIceChunk>(), (int)((double)weaponDamage2 * 1.5), (int)((double)weaponKnockback2 * 1.5), projectile.owner, 0.0f, speedY);
						Main.projectile[iceChunk].extraUpdates += num39 / 2; //0 to 2
					}
                }
                else
                {
                    projectile.Kill();
                }
            }
            projectile.position = player.RotatedRelativePoint(player.MountedCenter, true) - projectile.Size / 2f;
            projectile.rotation = projectile.velocity.ToRotation() + num;
            projectile.spriteDirection = projectile.direction;
            projectile.timeLeft = 2;
            player.ChangeDir(projectile.direction);
            player.heldProj = projectile.whoAmI;
            player.itemTime = 2;
            player.itemAnimation = 2;
            player.itemRotation = (float)Math.Atan2((double)(projectile.velocity.Y * (float)projectile.direction), (double)(projectile.velocity.X * (float)projectile.direction));
        }

        public override bool CanDamage()
        {
            return false;
        }
    }
}
