using CalamityMod.Items;
using CalamityMod.Items.Weapons.DraedonsArsenal;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.DraedonsArsenal
{
    public class HydraulicVoltCrasherProjectile : ModProjectile
    {
        private int chargeCooldown = 0;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Hydraulic Volt Crasher");
            Main.projFrames[projectile.type] = 3;
        }

        public override void SetDefaults()
        {
            projectile.width = 56;
            projectile.height = 56;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.hide = true;
            projectile.ownerHitCheck = true;
            projectile.melee = true;
            projectile.scale = 1.75f;
            projectile.Calamity().trueMelee = true;
        }

        public override void AI()
        {
            projectile.timeLeft = 60;
            projectile.frameCounter++;
            if (projectile.frameCounter % 4f == 3f)
            {
                projectile.frame++;
                if (projectile.frame >= Main.projFrames[projectile.type])
                {
                    projectile.frame = 0;
                }
            }
            // Decrement charge cooldown
            if (chargeCooldown > 0)
                chargeCooldown = 0;
            // Play idle sounds every so often.
            if (projectile.soundDelay <= 0)
            {
                Main.PlaySound(SoundID.Item22, projectile.position);
                projectile.soundDelay = 30;
            }
            Player player = Main.player[projectile.owner];
            Vector2 center = player.RotatedRelativePoint(player.MountedCenter);

            if (Main.myPlayer == player.whoAmI)
            {
                // Get the projectile owner's held item. If it's not a modded item, stop now to prevent weird errors.
                Item heldItem = player.ActiveItem();
                if (heldItem.type < ItemID.Count)
                {
                    projectile.Kill();
                    return;
                }

                // Update the damage of the holdout projectile constantly so that it decreases as charge decreases, even while in use.
                projectile.damage = player.GetWeaponDamage(heldItem);

                // Check if the player's held item still has sufficient charge. If so, and they're still using it, take a tiny bit of charge from it.
                CalamityGlobalItem modItem = heldItem.Calamity();
                if (player.channel && modItem.Charge >= HydraulicVoltCrasher.HoldoutChargeUse)
                {
                    modItem.Charge -= HydraulicVoltCrasher.HoldoutChargeUse;

                    float speed = player.inventory[player.selectedItem].shootSpeed * projectile.scale;
                    Vector2 toPointTo = Main.MouseWorld;
                    if (player.gravDir == -1f)
                    {
                        toPointTo.Y = Main.screenHeight - toPointTo.Y;
                    }
                    toPointTo = Vector2.Normalize(toPointTo - center) * speed;
                    if (toPointTo != projectile.velocity)
                    {
                        projectile.netUpdate = true;
                    }
                    projectile.velocity = toPointTo;
                }
                else
                {
                    projectile.Kill();
                }
            }
            player.ChangeDir((projectile.velocity.X > 0).ToDirectionInt());
            player.heldProj = projectile.whoAmI;

            player.itemAnimation = 2;
            player.itemTime = 2; // Note: player.SetDummyItemTime(frame) exists in 1.4 which accomplishes this task

            player.itemRotation = (projectile.velocity * player.direction).ToRotation();
            projectile.direction = projectile.spriteDirection = player.direction;
            projectile.rotation = projectile.velocity.ToRotation() + (projectile.direction == -1).ToInt() * MathHelper.Pi;

            if (Main.rand.NextBool(5))
            {
                Vector2 spawnPosition = projectile.velocity;
                spawnPosition.Normalize();
                spawnPosition *= projectile.Size;
                spawnPosition += projectile.Center;
                Dust dust = Dust.NewDustPerfect(spawnPosition, 226);
                dust.velocity = projectile.velocity.SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(2f, 3.6f);
                dust.velocity += player.velocity * 0.4f;
            }
            projectile.position = center - projectile.Size * 0.5f;
            projectile.position -= projectile.velocity.ToRotation().ToRotationVector2() * 8f;
            projectile.velocity.X *= Main.rand.NextFloat(0.97f, 1.03f);
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/PlasmaBolt"), target.Center);
            if (chargeCooldown > 0)
                return;
            chargeCooldown = 60;
            TryToSuperchargeNPC(target);
            for (int i = 0; i < Main.npc.Length; i++)
            {
                if (i != target.whoAmI &&
                    Main.npc[i].CanBeChasedBy(projectile, false) &&
                    Main.npc[i].Distance(target.Center) < 240f)
                {
                    if (TryToSuperchargeNPC(Main.npc[i]))
                    {
                        for (float increment = 0f; increment <= 1f; increment += 0.05f)
                        {
                            Vector2 spawnPosition = Vector2.Lerp(target.Center, Main.npc[i].Center, increment);
                            Dust dust = Dust.NewDustPerfect(spawnPosition, 226);
                            dust.velocity = Vector2.Zero;
                            dust.scale = 1.6f;
                            dust.noGravity = true;
                        }
                    }
                }
            }
        }

        public bool TryToSuperchargeNPC(NPC npc)
        {
            // Prevent supercharging an enemy twice.
            for (int i = 0; i < Main.projectile.Length; i++)
            {
                if (Main.projectile[i].active &&
                    Main.projectile[i].type == ModContent.ProjectileType<VoltageStream>() &&
                    Main.projectile[i].ai[1] == i)
                {
                    return false;
                }
            }
            Projectile.NewProjectileDirect(npc.Center,
                                           Vector2.Zero,
                                           ModContent.ProjectileType<VoltageStream>(),
                                           (int)(projectile.damage * 0.8),
                                           0f,
                                           projectile.owner,
                                           0f,
                                           npc.whoAmI);
            return true;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture = Main.projectileTexture[projectile.type];
            int height = texture.Height / Main.projFrames[projectile.type];
            int frameHeight = height * projectile.frame;
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (projectile.spriteDirection == -1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            spriteBatch.Draw(texture, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, frameHeight, texture.Width, height)), lightColor, projectile.rotation, new Vector2(texture.Width / 2f, height / 2f), projectile.scale, spriteEffects, 0f);
            return false;
        }
    }
}
