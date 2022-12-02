using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Ranged
{
    public class FlakKrakenGun : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/Ranged/FlakKraken";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Flak Kraken");
        }

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
            float num = 0f;
            Vector2 vector = player.RotatedRelativePoint(player.MountedCenter, true);
            if (Projectile.spriteDirection == -1)
            {
                num = 3.14159274f;
            }
            float num26 = 30f;
            if (Projectile.ai[0] > 90f)
            {
                num26 = 15f;
            }
            if (Projectile.ai[0] > 120f)
            {
                num26 = 5f;
            }
            Projectile.damage = player.ActiveItem() is null ? 0 : player.GetWeaponDamage(player.ActiveItem());
            Projectile.ai[0] += 1f;
            Projectile.ai[1] += 1f;
            int num27 = 10;
            bool flag10 = false;
            if (Projectile.ai[0] % num26 == 0f)
            {
                flag10 = true;
            }
            if (Projectile.ai[1] >= 1f)
            {
                Projectile.ai[1] = 0f;
                flag10 = true;
                if (Main.myPlayer == Projectile.owner)
                {
                    float scaleFactor5 = player.ActiveItem().shootSpeed * Projectile.scale;
                    Vector2 value12 = vector;
                    Vector2 value13 = Main.screenPosition + new Vector2((float)Main.mouseX, (float)Main.mouseY) - value12;
                    if (player.gravDir == -1f)
                    {
                        value13.Y = (float)(Main.screenHeight - Main.mouseY) + Main.screenPosition.Y - value12.Y;
                    }
                    Vector2 vector11 = Vector2.Normalize(value13);
                    if (float.IsNaN(vector11.X) || float.IsNaN(vector11.Y))
                    {
                        vector11 = -Vector2.UnitY;
                    }
                    vector11 = Vector2.Normalize(Vector2.Lerp(vector11, Vector2.Normalize(Projectile.velocity), 0.92f)); //0.92
                    vector11 *= scaleFactor5;
                    if (vector11.X != Projectile.velocity.X || vector11.Y != Projectile.velocity.Y)
                    {
                        Projectile.netUpdate = true;
                    }
                    Projectile.velocity = vector11;
                }
            }
            if (Projectile.soundDelay <= 0)
            {
                Projectile.soundDelay = num27;
                Projectile.soundDelay *= 2;
                if (Projectile.ai[0] != 1f && Projectile.ai[0] <= 500f)
                {
                    SoundEngine.PlaySound(SoundID.Item15, Projectile.position);
                }
            }
            if (flag10 && Main.myPlayer == Projectile.owner)
            {
                bool flag12 = player.channel && !player.noItems && !player.CCed;
                if (flag12)
                {
                    if (Projectile.ai[0] == 1f)
                    {
                        Vector2 vector2 = player.RotatedRelativePoint(player.MountedCenter, true);
                        float num78 = (float)Main.mouseX + Main.screenPosition.X - vector2.X;
                        float num79 = (float)Main.mouseY + Main.screenPosition.Y - vector2.Y;
                        if (player.gravDir == -1f)
                        {
                            num79 = Main.screenPosition.Y + (float)Main.screenHeight - (float)Main.mouseY - vector2.Y;
                        }
                        if ((float.IsNaN(num78) && float.IsNaN(num79)) || (num78 == 0f && num79 == 0f))
                        {
                            num78 = (float)player.direction;
                            num79 = 0f;
                        }
                        vector2 += new Vector2(num78, num79);
                        int num29 = Projectile.damage;
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), vector2.X, vector2.Y, 0f, 0f, ModContent.ProjectileType<FlakKrakenProj>(), num29, Projectile.knockBack, Projectile.owner, 0f, (float)Projectile.whoAmI);
                        Projectile.netUpdate = true;
                    }
                }
                else
                {
                    Projectile.Kill();
                }
            }
            Projectile.position = player.RotatedRelativePoint(player.MountedCenter, true) - Projectile.Size / 2f;
            Projectile.rotation = Projectile.velocity.ToRotation() + num;
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
