using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Projectiles.Melee;
using CalamityMod.Projectiles.Typeless;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class StarfleetMK2Gun : ModProjectile
    {
        public override LocalizedText DisplayName => CalamityUtils.GetItemName<Starmada>();
        public override string Texture => "CalamityMod/Items/Weapons/Ranged/Starmada";

        public override void SetDefaults()
        {
            Projectile.width = 122;
            Projectile.height = 50;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.ignoreWater = true;
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, 32f / 255f, 208f / 255f, 255f / 255f);
            Player player = Main.player[Projectile.owner];
            Projectile.ai[1]--;
            if (Projectile.ai[0] >= 0f)
            {
                Projectile.ai[0] += 1f;
                switch ((int)Projectile.ai[0])
                {
                    case 90:
                    case 180:
                    case 270:
                    case 360:
                    case 450:
                        Projectile.localAI[0] += 1f;
                        break;
                    case 540:
                        Projectile.localAI[0] += 1f;
                        Projectile.ai[0] = -1f;
                        SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
                        int dustAmt = 36;
                        for (int d = 0; d < dustAmt; d++)
                        {
                            Vector2 source = Vector2.Normalize(Projectile.velocity) * 9f;
                            source = source.RotatedBy((d - (dustAmt / 2 - 1)) * MathHelper.TwoPi / dustAmt, default) + player.Center;
                            Vector2 dustVel = source - player.Center;
                            int index = Dust.NewDust(source + dustVel, 0, 0, 221, 0f, 0f, 0, default, 4f);
                            Main.dust[index].noGravity = true;
                            Main.dust[index].velocity = dustVel;
                        }
                        break;
                }
            }
            int baseUseTime = 27; //Ranges from 27 to 15 use time
            int modifier = 2;
            bool timeToFire = false;
            if (Projectile.ai[1] <= 0f)
            {
                Projectile.ai[1] = baseUseTime - modifier * Projectile.localAI[0];
                timeToFire = true;
            }
            bool canFire = player.channel && player.HasAmmo(player.ActiveItem()) && !player.noItems && !player.CCed;
            if (Projectile.soundDelay <= 0 && canFire)
            {
                Projectile.soundDelay = baseUseTime - modifier * (int)Projectile.localAI[0];
                if (Projectile.ai[0] != 1f)
                    SoundEngine.PlaySound(SoundID.Item92, Projectile.Center);
            }
            if (timeToFire && Main.myPlayer == Projectile.owner)
            {
                if (canFire)
                {
                    int type = ProjectileID.StarCannonStar;
                    float shootSpeed = 16f;
                    int damage = player.GetWeaponDamage(player.ActiveItem());
                    float knockBack = player.ActiveItem().knockBack;
                    player.PickAmmo(player.ActiveItem(), out type, out shootSpeed, out damage, out knockBack, out _);
                    for (int i = 0; i < 5; i++)
                    {
                        knockBack = player.GetWeaponKnockback(player.ActiveItem(), knockBack);
                        Vector2 playerPosition = player.RotatedRelativePoint(player.MountedCenter, true);
                        Projectile.velocity = Main.screenPosition - playerPosition;
                        Projectile.velocity.X += Main.mouseX;
                        Projectile.velocity.Y += Main.mouseY;
                        if (player.gravDir == -1f)
                            Projectile.velocity.Y = Main.screenHeight - Main.mouseY + Main.screenPosition.Y - playerPosition.Y;
                        Projectile.velocity.Normalize();
                        float variation = (1f + Projectile.localAI[0]) * 3f;
                        Vector2 position = playerPosition + Utils.RandomVector2(Main.rand, -variation, variation);
                        Vector2 speed = Projectile.velocity * shootSpeed * Main.rand.NextFloat(0.6f, 1.2f);
                        type = Utils.SelectRandom(Main.rand, new int[]
                        {
                            ModContent.ProjectileType<PlasmaBlast>(),
                            ModContent.ProjectileType<AstralStar>(),
                            ModContent.ProjectileType<GalacticaComet>(),
                            ProjectileID.StarCannonStar,
                            ProjectileID.Starfury
                        });
                        int star = Projectile.NewProjectile(Projectile.GetSource_FromThis(), position, speed, type, damage, knockBack, Projectile.owner);
                        if (star.WithinBounds(Main.maxProjectiles))
                        {
                            Main.projectile[star].penetrate = 1;
                            Main.projectile[star].timeLeft = 300;
                            Main.projectile[star].DamageType = DamageClass.Ranged;
                            Main.projectile[star].netUpdate = true;
                        }
                        Projectile.netUpdate = true;
                    }
                }
                else
                {
                    Projectile.Kill();
                }
            }
            Projectile.rotation = Projectile.velocity.ToRotation();
            Vector2 displayOffset = new Vector2(27f, -10f * Projectile.direction).RotatedBy(Projectile.rotation);
            Projectile.Center = player.RotatedRelativePoint(player.MountedCenter, true) + displayOffset;
            if (Projectile.spriteDirection == -1)
                Projectile.rotation += MathHelper.Pi;
            Projectile.spriteDirection = Projectile.direction;
            Projectile.timeLeft = 2;
            player.ChangeDir(Projectile.direction);
            player.heldProj = Projectile.whoAmI;
            player.itemTime = 2;
            player.itemAnimation = 2;
            player.itemRotation = (float)Math.Atan2(Projectile.velocity.Y * Projectile.direction, Projectile.velocity.X * Projectile.direction);
        }

        public override bool? CanDamage() => false;
    }
}
