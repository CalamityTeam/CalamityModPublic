using System;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using static CalamityMod.CalamityUtils;

namespace CalamityMod.Projectiles.Magic
{
    public class CauldronHoldout : ModProjectile, ILocalizedModType
    {
        public override LocalizedText DisplayName => GetItemName<TheCauldron>();

        public override string Texture => "CalamityMod/Items/Weapons/Magic/TheCauldron";

        public static int FireRate = 60;

        public override void SetDefaults()
        {
            Projectile.width = 46;
            Projectile.height = 46;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.ContinuouslyUpdateDamageStats = true;
            Projectile.ignoreWater = true;
        }
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            float rotoffset = MathHelper.PiOver4;
            Vector2 playerpos = player.RotatedRelativePoint(player.MountedCenter, true);
            bool shouldBeHeld = !player.CantUseHoldout();
            Projectile.damage = player.HeldItem is null ? 0 : player.GetWeaponDamage(player.HeldItem);
            if (Projectile.ai[0] > 0f)
            {
                Projectile.ai[0] -= 1f;
            }
            if (Main.myPlayer == Projectile.owner)
            {
                if (shouldBeHeld)
                {
                    float holdscale = player.HeldItem.shootSpeed * Projectile.scale;
                    Vector2 playerpos2 = playerpos;
                    Vector2 going = Main.screenPosition + new Vector2((float)Main.mouseX, (float)Main.mouseY) - Projectile.Center;
                    if (player.gravDir == -1f)
                    {
                        going.Y = (float)(Main.screenHeight - Main.mouseY) + Main.screenPosition.Y - Projectile.Center.Y;
                    }
                    Vector2 normalizedgoing = Vector2.Normalize(going);
                    if (float.IsNaN(normalizedgoing.X) || float.IsNaN(normalizedgoing.Y))
                    {
                        normalizedgoing = -Vector2.UnitY;
                    }
                    normalizedgoing *= holdscale;
                    if (normalizedgoing.X != Projectile.velocity.X || normalizedgoing.Y != Projectile.velocity.Y)
                    {
                        Projectile.netUpdate = true;
                    }
                    Projectile.velocity = normalizedgoing * 0.55f;

                    if (Projectile.ai[0] <= 0)
                    {
                        bool manaCostPaid = player.CheckMana(player.HeldItem, -1, true, false);
                        if (manaCostPaid)
                        {
                            SoundEngine.PlaySound(SoundID.DD2_BetsyFireballShot, Projectile.Center);
                            
                            int projType = ModContent.ProjectileType<CauldronProj>();
                            int projDamage = Projectile.damage;
                            float speedscale = 18f;
                            Vector2 shotSpeed = Projectile.velocity.SafeNormalize(-Vector2.UnitY) * speedscale;
                            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, shotSpeed, projType, projDamage, Projectile.knockBack, player.whoAmI);

                            for (int i = 0; i < 6; i++)
                            {
                                Vector2 burstSpeed = (Projectile.rotation - rotoffset).ToRotationVector2().RotatedByRandom(MathHelper.ToRadians(45f)) * Main.rand.NextFloat(8f, 14f);
                                SquishyLightParticle energy = new(Projectile.Center + Projectile.rotation.ToRotationVector2() * 5, burstSpeed, Main.rand.NextFloat(0.2f, 0.3f), Color.Orange, Main.rand.Next(6, 11), 3, 1.5f);
                                GeneralParticleHandler.SpawnParticle(energy);
                            }
                            Projectile.ai[0] = FireRate;
                        }
                        else
                        {
                            Projectile.Kill();
                        }
                    }
                }
                else
                {
                    Projectile.Kill();
                }
            }
            Projectile.Center = player.RotatedRelativePoint(player.MountedCenter, true) - new Vector2(player.direction == 1 ? 6 : 0, 30);
            Projectile.rotation = Projectile.velocity.ToRotation() + rotoffset;
            Projectile.spriteDirection = Projectile.direction;
            Projectile.timeLeft = 2;
            player.ChangeDir(Projectile.direction);
            player.heldProj = Projectile.whoAmI;
            player.itemTime = 2;
            player.itemAnimation = 2;
            player.itemRotation = (float)Math.Atan2((double)(Projectile.velocity.Y * (float)Projectile.direction), (double)(Projectile.velocity.X * (float)Projectile.direction));
            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, MathHelper.Pi);
            player.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Full, MathHelper.Pi);
        }

        public override bool? CanDamage() => false;

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = TextureAssets.Projectile[Type].Value;
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, tex.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(TheCauldron.Glow.Value, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, tex.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}
