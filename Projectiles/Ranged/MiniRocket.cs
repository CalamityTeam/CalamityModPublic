using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
namespace CalamityMod.Projectiles.Ranged
{
    public class MiniRocket : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Ranged";
        private ref float RocketType => ref Projectile.ai[0];

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 4;
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

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(90);

            //Animation
            Projectile.frameCounter++;
            if (Projectile.frameCounter > 7)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame >= Main.projFrames[Projectile.type])
            {
                Projectile.frame = 0;
            }

            if (Projectile.velocity.Length() >= 8f)
            {
                for (int i = 0; i < 2; i++)
                {
                    float halfX = 0f;
                    float halfY = 0f;
                    if (i == 1)
                    {
                        halfX = Projectile.velocity.X * 0.5f;
                        halfY = Projectile.velocity.Y * 0.5f;
                    }
                    int dusty = Dust.NewDust(new Vector2(Projectile.position.X + 3f + halfX, Projectile.position.Y + 3f + halfY) - Projectile.velocity * 0.5f, Projectile.width - 8, Projectile.height - 8, 6, 0f, 0f, 100, default, 1f);
                    Main.dust[dusty].scale *= 2f + (float)Main.rand.Next(10) * 0.1f;
                    Main.dust[dusty].velocity *= 0.2f;
                    Main.dust[dusty].noGravity = true;
                    dusty = Dust.NewDust(new Vector2(Projectile.position.X + 3f + halfX, Projectile.position.Y + 3f + halfY) - Projectile.velocity * 0.5f, Projectile.width - 8, Projectile.height - 8, 31, 0f, 0f, 100, default, 0.5f);
                    Main.dust[dusty].fadeIn = 1f + (float)Main.rand.Next(5) * 0.1f;
                    Main.dust[dusty].velocity *= 0.05f;
                }
            }
            CalamityUtils.HomeInOnNPC(Projectile, !Projectile.tileCollide, 200f, 12f, 20f);

			if (RocketType == ItemID.DryRocket || RocketType == ItemID.WetRocket || RocketType == ItemID.LavaRocket || RocketType == ItemID.HoneyRocket)
			{
				if (Projectile.wet)
					Projectile.timeLeft = 1;
			}
        }

        public override void OnKill(int timeLeft)
        {
            Projectile.ExpandHitboxBy(32);
            Projectile.maxPenetrate = -1;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.Damage();
            SoundEngine.PlaySound(SoundID.Item14, Projectile.position);
            for (int j = 0; j < 20; j++)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 31, 0f, 0f, 100, default, 2f);
                Main.dust[dust].velocity *= 3f;
                if (Main.rand.NextBool())
                {
                    Main.dust[dust].scale = 0.5f;
                    Main.dust[dust].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                }
            }
            for (int k = 0; k < 30; k++)
            {
                int dust2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 6, 0f, 0f, 100, default, 3f);
                Main.dust[dust2].noGravity = true;
                Main.dust[dust2].velocity *= 5f;
                dust2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 6, 0f, 0f, 100, default, 2f);
                Main.dust[dust2].velocity *= 2f;
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

            // Only do rocket effects for the owner client side
            if (Projectile.owner != Main.myPlayer)
                return;

            int blastRadius = 0;
            if (RocketType == ItemID.RocketII)
                blastRadius = 3;
            else if (RocketType == ItemID.RocketIV)
                blastRadius = 6;
            else if (RocketType == ItemID.MiniNukeII)
                blastRadius = 9;
            Projectile.ExpandHitboxBy(14);

            if (blastRadius > 0)
                Projectile.ExplodeandDestroyTiles(blastRadius, true, new List<int>(), new List<int>());

			Point center = Projectile.Center.ToTileCoordinates();
			DelegateMethods.v2_1 = center.ToVector2();
			DelegateMethods.f_1 = 3f;
			if (RocketType == ItemID.DryRocket)
			{
				DelegateMethods.f_1 = 3.5f;
				Utils.PlotTileArea(center.X, center.Y, DelegateMethods.SpreadDry);
			}
			else if (RocketType == ItemID.WetRocket)
			{
				Utils.PlotTileArea(center.X, center.Y, DelegateMethods.SpreadWater);
			}
			else if (RocketType == ItemID.LavaRocket)
			{
				Utils.PlotTileArea(center.X, center.Y, DelegateMethods.SpreadLava);
			}
			else if (RocketType == ItemID.HoneyRocket)
			{
				Utils.PlotTileArea(center.X, center.Y, DelegateMethods.SpreadHoney);
			}
        }
    }
}
