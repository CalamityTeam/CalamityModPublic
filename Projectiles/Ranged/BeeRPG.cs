using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Ranged
{
    public class BeeRPG : ModProjectile
    {
        public static Item FalseLauncher = null;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bee RPG");
        }

        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 14;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 95;
            Projectile.DamageType = DamageClass.Ranged;
        }

        private static void DefineFalseLauncher()
        {
            int rocketID = ItemID.RocketLauncher;
            FalseLauncher = new Item();
            FalseLauncher.SetDefaults(rocketID, true);
        }

        public override void AI()
        {
            if (Math.Abs(Projectile.velocity.X) >= 8f || Math.Abs(Projectile.velocity.Y) >= 8f)
            {
                for (int num246 = 0; num246 < 2; num246++)
                {
                    float num247 = 0f;
                    float num248 = 0f;
                    if (num246 == 1)
                    {
                        num247 = Projectile.velocity.X * 0.5f;
                        num248 = Projectile.velocity.Y * 0.5f;
                    }
                    int num249 = Dust.NewDust(new Vector2(Projectile.position.X + 3f + num247, Projectile.position.Y + 3f + num248) - Projectile.velocity * 0.5f, Projectile.width - 8, Projectile.height - 8, 6, 0f, 0f, 100, default, 1f);
                    Main.dust[num249].scale *= 2f + (float)Main.rand.Next(10) * 0.1f;
                    Main.dust[num249].velocity *= 0.2f;
                    Main.dust[num249].noGravity = true;
                    num249 = Dust.NewDust(new Vector2(Projectile.position.X + 3f + num247, Projectile.position.Y + 3f + num248) - Projectile.velocity * 0.5f, Projectile.width - 8, Projectile.height - 8, 31, 0f, 0f, 100, default, 0.5f);
                    Main.dust[num249].fadeIn = 1f + (float)Main.rand.Next(5) * 0.1f;
                    Main.dust[num249].velocity *= 0.05f;
                }
            }
            if (Math.Abs(Projectile.velocity.X) < 15f && Math.Abs(Projectile.velocity.Y) < 15f)
            {
                Projectile.velocity *= 2f;
            }
            else if (Main.rand.NextBool(2))
            {
                int num252 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 31, 0f, 0f, 100, default, 1f);
                Main.dust[num252].scale = 0.1f + (float)Main.rand.Next(5) * 0.1f;
                Main.dust[num252].fadeIn = 1.5f + (float)Main.rand.Next(5) * 0.1f;
                Main.dust[num252].noGravity = true;
                Main.dust[num252].position = Projectile.Center + new Vector2(0f, (float)(-(float)Projectile.height / 2)).RotatedBy((double)Projectile.rotation, default) * 1.1f;
                Main.rand.Next(2);
                num252 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 6, 0f, 0f, 100, default, 1f);
                Main.dust[num252].scale = 1f + (float)Main.rand.Next(5) * 0.1f;
                Main.dust[num252].noGravity = true;
                Main.dust[num252].position = Projectile.Center + new Vector2(0f, (float)(-(float)Projectile.height / 2 - 6)).RotatedBy((double)Projectile.rotation, default) * 1.1f;
            }
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(90);

            // Construct a fake item to use with vanilla code for the sake of picking ammo.
            if (FalseLauncher is null)
                DefineFalseLauncher();
            Player player = Main.player[Projectile.owner];
            int projID = ProjectileID.RocketI;
            float shootSpeed = 0f;
            int damage = 0;
            float kb = 0f;
            player.PickAmmo(FalseLauncher, out projID, out shootSpeed, out damage, out kb, out _, true);
			if (projID == ProjectileID.DryRocket || projID == ProjectileID.WetRocket || projID == ProjectileID.LavaRocket || projID == ProjectileID.HoneyRocket)
			{
				if (Projectile.wet)
					Projectile.timeLeft = 1;
			}
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);
            SoundEngine.PlaySound(SoundID.Dig, Projectile.position);
            return true;
        }

        public override void Kill(int timeLeft)
        {
            Projectile.ExpandHitboxBy(32);
            Projectile.maxPenetrate = -1;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.Damage();
            if (Projectile.owner == Main.myPlayer)
            {
                int num516 = 12;
                for (int num517 = 0; num517 < num516; num517++)
                {
                    if (num517 % 2 != 1 || Main.rand.NextBool(3))
                    {
                        Vector2 value20 = Projectile.position;
                        Vector2 value21 = Projectile.oldVelocity;
                        value21.Normalize();
                        value21 *= 8f;
                        float num518 = (float)Main.rand.Next(-35, 36) * 0.01f;
                        float num519 = (float)Main.rand.Next(-35, 36) * 0.01f;
                        value20 -= value21 * (float)num517;
                        num518 += Projectile.oldVelocity.X / 6f;
                        num519 += Projectile.oldVelocity.Y / 6f;
                        int bee = Projectile.NewProjectile(Projectile.GetSource_FromThis(), value20.X, value20.Y, num518, num519, Main.player[Projectile.owner].beeType(), Main.player[Projectile.owner].beeDamage(Projectile.damage / 4), Main.player[Projectile.owner].beeKB(0f), Main.myPlayer);
                        if (bee.WithinBounds(Main.maxProjectiles))
                        {
                            Main.projectile[bee].penetrate = 2;
                            Main.projectile[bee].DamageType = DamageClass.Ranged;
                        }
                    }
                }
            }
            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
            for (int num621 = 0; num621 < 5; num621++)
            {
                int num622 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 31, 0f, 0f, 100, default, 2f);
                Main.dust[num622].velocity *= 3f;
                if (Main.rand.NextBool(2))
                {
                    Main.dust[num622].scale = 0.5f;
                    Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                }
            }
            for (int num623 = 0; num623 < 10; num623++)
            {
                int num624 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 6, 0f, 0f, 100, default, 3f);
                Main.dust[num624].noGravity = true;
                Main.dust[num624].velocity *= 5f;
                num624 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 6, 0f, 0f, 100, default, 2f);
                Main.dust[num624].velocity *= 2f;
            }

            if (Main.netMode != NetmodeID.Server)
            {
                Vector2 goreSource = Projectile.Center;
                int goreAmt = 3;
                Vector2 source = new Vector2(goreSource.X - 24f, goreSource.Y - 24f);
                for (int goreIndex = 0; goreIndex < goreAmt; goreIndex++)
                {
                    float velocityMult = 0.33f;
                    if (goreIndex < (goreAmt / 3))
                    {
                        velocityMult = 0.66f;
                    }
                    if (goreIndex >= (2 * goreAmt / 3))
                    {
                        velocityMult = 1f;
                    }
                    Mod mod = ModContent.GetInstance<CalamityMod>();
                    int type = Main.rand.Next(61, 64);
                    int smoke = Gore.NewGore(Projectile.GetSource_Death(), source, default, type, 1f);
                    Gore gore = Main.gore[smoke];
                    gore.velocity *= velocityMult;
                    gore.velocity.X += 1f;
                    gore.velocity.Y += 1f;
                    type = Main.rand.Next(61, 64);
                    smoke = Gore.NewGore(Projectile.GetSource_Death(), source, default, type, 1f);
                    gore = Main.gore[smoke];
                    gore.velocity *= velocityMult;
                    gore.velocity.X -= 1f;
                    gore.velocity.Y += 1f;
                    type = Main.rand.Next(61, 64);
                    smoke = Gore.NewGore(Projectile.GetSource_Death(), source, default, type, 1f);
                    gore = Main.gore[smoke];
                    gore.velocity *= velocityMult;
                    gore.velocity.X += 1f;
                    gore.velocity.Y -= 1f;
                    type = Main.rand.Next(61, 64);
                    smoke = Gore.NewGore(Projectile.GetSource_Death(), source, default, type, 1f);
                    gore = Main.gore[smoke];
                    gore.velocity *= velocityMult;
                    gore.velocity.X -= 1f;
                    gore.velocity.Y -= 1f;
                }
            }

            // Construct a fake item to use with vanilla code for the sake of picking ammo.
            if (FalseLauncher is null)
                DefineFalseLauncher();
            Player player = Main.player[Projectile.owner];
            int projID = ProjectileID.RocketI;
            float shootSpeed = 0f;
            int damage = 0;
            float kb = 0f;
            player.PickAmmo(FalseLauncher, out projID, out shootSpeed, out damage, out kb, out _, true);
            int blastRadius = 0;
            if (projID == ProjectileID.RocketII || projID == ProjectileID.ClusterRocketII) // Not adding the actual shrapnel tho
                blastRadius = 3;
            else if (projID == ProjectileID.RocketIV)
                blastRadius = 6;
            else if (projID == ProjectileID.MiniNukeRocketII)
                blastRadius = 9;

            Projectile.ExpandHitboxBy(14);

            if (Projectile.owner == Main.myPlayer && blastRadius > 0)
            {
                CalamityUtils.ExplodeandDestroyTiles(Projectile, blastRadius, true, new List<int>() { }, new List<int>() { });
            }
			if (Projectile.owner == Main.myPlayer)
			{
				Point center = Projectile.Center.ToTileCoordinates();
				DelegateMethods.v2_1 = center.ToVector2();
				DelegateMethods.f_1 = 3f;
				if (projID == ProjectileID.DryRocket)
				{
					DelegateMethods.f_1 = 3.5f;
					Utils.PlotTileArea(center.X, center.Y, DelegateMethods.SpreadDry);
				}
				else if (projID == ProjectileID.WetRocket)
				{
					Utils.PlotTileArea(center.X, center.Y, DelegateMethods.SpreadWater);
				}
				else if (projID == ProjectileID.LavaRocket)
				{
					Utils.PlotTileArea(center.X, center.Y, DelegateMethods.SpreadLava);
				}
				else if (projID == ProjectileID.HoneyRocket)
				{
					Utils.PlotTileArea(center.X, center.Y, DelegateMethods.SpreadHoney);
				}
			}
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<Plague>(), 180);
        }
    }
}
