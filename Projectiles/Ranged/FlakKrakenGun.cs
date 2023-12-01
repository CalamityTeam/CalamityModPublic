using System;
using CalamityMod.Items.Weapons.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class FlakKrakenGun : ModProjectile
    {
        public override LocalizedText DisplayName => CalamityUtils.GetItemName<FlakKraken>();
        public override string Texture => "CalamityMod/Items/Weapons/Ranged/FlakKraken";

        public override void SetDefaults()
        {
            Projectile.width = 152;
            Projectile.height = 58;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.ignoreWater = true;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            float pi = 0f;
            Vector2 playerRotation = player.RotatedRelativePoint(player.MountedCenter, true);
            if (Projectile.spriteDirection == -1)
            {
                pi = 3.14159274f;
            }
            float hitRate = 30f;
            if (Projectile.ai[0] > 90f)
            {
                hitRate = 15f;
            }
            if (Projectile.ai[0] > 120f)
            {
                hitRate = 5f;
            }
            Projectile.damage = player.ActiveItem() is null ? 0 : player.GetWeaponDamage(player.ActiveItem());
            Projectile.ai[0] += 1f;
            Projectile.ai[1] += 1f;
            bool isUsing = false;
            if (Projectile.ai[0] % hitRate == 0f)
            {
                isUsing = true;
            }
            if (Projectile.ai[1] >= 1f)
            {
                Projectile.ai[1] = 0f;
                isUsing = true;
                if (Main.myPlayer == Projectile.owner)
                {
                    float scaleFactor5 = player.ActiveItem().shootSpeed * Projectile.scale;
                    Vector2 shootDirection = Main.screenPosition + new Vector2((float)Main.mouseX, (float)Main.mouseY) - playerRotation;
                    if (player.gravDir == -1f)
                    {
                        shootDirection.Y = (float)(Main.screenHeight - Main.mouseY) + Main.screenPosition.Y - playerRotation.Y;
                    }
                    Vector2 normalizeShoot = Vector2.Normalize(shootDirection);
                    if (float.IsNaN(normalizeShoot.X) || float.IsNaN(normalizeShoot.Y))
                    {
                        normalizeShoot = -Vector2.UnitY;
                    }
                    normalizeShoot = Vector2.Normalize(Vector2.Lerp(normalizeShoot, Vector2.Normalize(Projectile.velocity), 0.92f)); //0.92
                    normalizeShoot *= scaleFactor5;
                    if (normalizeShoot.X != Projectile.velocity.X || normalizeShoot.Y != Projectile.velocity.Y)
                    {
                        Projectile.netUpdate = true;
                    }
                    Projectile.velocity = normalizeShoot;
                }
            }
            if (Projectile.soundDelay <= 0)
            {
                Projectile.soundDelay = 10;
                Projectile.soundDelay *= 2;
                if (Projectile.ai[0] != 1f && Projectile.ai[0] <= 500f)
                {
                    SoundEngine.PlaySound(SoundID.Item15, Projectile.position);
                }
            }
            if (isUsing && Main.myPlayer == Projectile.owner)
            {
                bool canUseItem = player.channel && !player.noItems && !player.CCed;
                if (canUseItem)
                {
                    if (Projectile.ai[0] == 1f)
                    {
                        Vector2 projPos = player.RotatedRelativePoint(player.MountedCenter, true);
                        float projX = (float)Main.mouseX + Main.screenPosition.X - projPos.X;
                        float projY = (float)Main.mouseY + Main.screenPosition.Y - projPos.Y;
                        if (player.gravDir == -1f)
                        {
                            projY = Main.screenPosition.Y + (float)Main.screenHeight - (float)Main.mouseY - projPos.Y;
                        }
                        if ((float.IsNaN(projX) && float.IsNaN(projY)) || (projX == 0f && projY == 0f))
                        {
                            projX = (float)player.direction;
                            projY = 0f;
                        }
                        projPos += new Vector2(projX, projY);
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), projPos.X, projPos.Y, 0f, 0f, ModContent.ProjectileType<FlakKrakenProj>(), Projectile.damage, Projectile.knockBack, Projectile.owner, 0f, (float)Projectile.whoAmI);
                        Projectile.netUpdate = true;
                    }
                }
                else
                {
                    Projectile.Kill();
                }
            }
            Projectile.position = player.RotatedRelativePoint(player.MountedCenter, true) - Projectile.Size / 2f;
            Projectile.rotation = Projectile.velocity.ToRotation() + pi;
            Projectile.spriteDirection = Projectile.direction;
            Projectile.timeLeft = 2;
            player.ChangeDir(Projectile.direction);
            player.heldProj = Projectile.whoAmI;
            player.itemTime = 2;
            player.itemAnimation = 2;
            player.itemRotation = (float)Math.Atan2((double)(Projectile.velocity.Y * (float)Projectile.direction), (double)(Projectile.velocity.X * (float)Projectile.direction));
        }

        public override bool? CanDamage() => false;
    }
}
