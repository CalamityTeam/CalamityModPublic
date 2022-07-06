using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class HadalUrnHoldout : ModProjectile
    {
        public static readonly SoundStyle UrnSound = new("CalamityMod/Sounds/Item/HadalUrnClose");
        public int manatimer = 0;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Hadal Urn");
        }

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 24;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.ignoreWater = true;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            float rotoffset = MathHelper.PiOver2;
            Vector2 playerpos = player.RotatedRelativePoint(player.MountedCenter, true);
            bool shouldBeHeld = player.channel && !player.noItems && !player.CCed;
            Projectile.damage = (int)player.GetTotalDamage<MagicDamageClass>().ApplyTo(player.ActiveItem()?.damage ?? 0);
            if (Projectile.ai[0] > 0f)
            {
                Projectile.ai[0] -= 1f;
            }
            if (Main.myPlayer == Projectile.owner)
            {
                if (shouldBeHeld)
                {
                    float holdscale = player.ActiveItem().shootSpeed * Projectile.scale;
                    Vector2 playerpos2 = playerpos;
                    Vector2 going = Main.screenPosition + new Vector2((float)Main.mouseX, (float)Main.mouseY) - playerpos2;
                    if (player.gravDir == -1f)
                    {
                        going.Y = (float)(Main.screenHeight - Main.mouseY) + Main.screenPosition.Y - playerpos2.Y;
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
                    int projCount = 2;

                    if (Projectile.ai[0] <= 0)
                    {
                        bool manaCostPaid = player.CheckMana(player.ActiveItem(), -1, true, false);
                        if (manaCostPaid)
                        {
                            SoundEngine.PlaySound(SoundID.Item111, Projectile.position);
                            for (int i = 0; i < projCount; ++i)
                            {
                                int projType = Main.rand.Next(5);
                                int projDamage = Projectile.damage;
                                int spreadfactor = 35; //Spread of the storm
                                int ai = 0; //Solely used for the jellyfish to make it hop quicker, otherwise 0
                                float speedscale = 10f; //How fast the projectiles should move. This is modified for cerain projectile types

                                switch (projType)
                                {
                                    case 0:
                                    case 1:
                                        projType = ModContent.ProjectileType<HadalUrnLamprey>();
                                        break;
                                    case 2:
                                    case 3:
                                        projType = ModContent.ProjectileType<HadalUrnStarfish>();
                                        break;
                                    //Jellyfish clog up the screen in higher numbers, so they have a smaller chance of appearing
                                    //The chance is instead replaced with another abyssal isopod since they're arguably the weakest projectile
                                    case 4:
                                        projType = ModContent.ProjectileType<HadalUrnJellyfish>();
                                        speedscale = speedscale * 0.5f;
                                        ai = 30; //Initial "hop" is performed quicker than subsequent hops
                                        break;
                                    /*default:
                                        projType = ModContent.ProjectileType<HadalUrnIsopod>();
                                        speedscale = speedscale * 1.5f; //Donor requested these be faster than other projectiles
                                        break;*/
                                }
                                Vector2 shotSpeed = Vector2.Normalize(Projectile.velocity) * speedscale;
                                shotSpeed.X = shotSpeed.X + (float)Main.rand.Next(-spreadfactor, spreadfactor + 1) * 0.05f;
                                shotSpeed.Y = shotSpeed.Y + (float)Main.rand.Next(-spreadfactor, spreadfactor + 1) * 0.05f;
                                if (float.IsNaN(shotSpeed.X) || float.IsNaN(shotSpeed.Y))
                                {
                                    shotSpeed = -Vector2.UnitY;
                                }
                                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, shotSpeed, projType, projDamage, Projectile.knockBack, player.whoAmI, ai);
                            }
                            Projectile.ai[0] = 16f;
                        }
                        else
                        {
                            SoundEngine.PlaySound(UrnSound, Projectile.position);
                            Projectile.Kill();
                        }
                    }
                }
                else
                {
                    SoundEngine.PlaySound(UrnSound, Projectile.position);
                    Projectile.Kill();
                }
            }
            Projectile.position = player.RotatedRelativePoint(player.MountedCenter, true) - Projectile.Size / 2f;
            Projectile.rotation = Projectile.velocity.ToRotation() + rotoffset;
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
