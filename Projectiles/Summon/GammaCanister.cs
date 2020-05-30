using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Summon
{
    public class GammaCanister : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Gamma Canister");
            ProjectileID.Sets.MinionShot[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 20;
            projectile.height = 28;
            projectile.friendly = projectile.hostile = false;
            projectile.minion = true;
            projectile.minionSlots = 0f;
            projectile.timeLeft = 300;
            projectile.tileCollide = true;
            projectile.ignoreWater = true;
        }
        public override void AI()
        {
            if (!Main.player[projectile.owner].Calamity().GammaCanisters.Contains(projectile.whoAmI))
            {
                Main.player[projectile.owner].Calamity().GammaCanisters.Add(projectile.whoAmI);
            }
            projectile.ai[1]++;
            if (projectile.ai[1] == 80f)
            {
                for (int i = 0; i < 36; i++)
                {
                    Vector2 velocity = Vector2.One.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(3f, 8f);
                    Dust dust = Dust.NewDustPerfect(projectile.Center, (int)CalamityDusts.SulfurousSeaAcid);
                    dust.velocity = velocity;
                    dust.noGravity = true;
                    dust.scale = Main.rand.NextFloat(1.3f, 1.7f);
                }
            }
            projectile.velocity = Vector2.UnitY * (float)Math.Sin(projectile.ai[1] / 40f * MathHelper.TwoPi) * 0.5f;
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            const float totalRotations = 3f;
            const float outwardPositionDeltaStart = 8f;
            float angle = MathHelper.TwoPi * projectile.ai[1] / totalRotations / 80f;
            float outwardPositionDelta = outwardPositionDeltaStart;
            if (projectile.ai[1] < 80f && projectile.ai[1] > 55f)
            {
                outwardPositionDelta = MathHelper.Lerp(outwardPositionDeltaStart, 0f, (projectile.ai[1] - 55f) / 35f);
            }
            if (projectile.ai[1] >= 80f)
            {
                outwardPositionDelta = 0f;
            }
            for (int i = 0; i < 3; i++)
            {
                angle += MathHelper.TwoPi / 3f * i;
                spriteBatch.Draw(ModContent.GetTexture(Texture),
                                 projectile.Center + angle.ToRotationVector2() * outwardPositionDelta + Vector2.UnitY * (float)Math.Cos(angle) * 3f - Main.screenPosition,
                                 null,
                                 Color.White,
                                 projectile.rotation,
                                 projectile.Size * 0.5f,
                                 projectile.scale,
                                 SpriteEffects.None,
                                 0f);
            }
            return false;
        }
        public override void Kill(int timeLeft)
        {
            Main.player[projectile.owner].Calamity().GammaCanisters.Clear();
            for (int i = 0; i < 12; i++)
            {
                int idx = Dust.NewDust(projectile.position, 8, 8, (int)CalamityDusts.SulfurousSeaAcid, 0, 0, 0, default, 0.75f);
                Main.dust[idx].noGravity = true;
                Main.dust[idx].velocity *= 3f;
                Main.dust[idx].scale = 1.8f;
                idx = Dust.NewDust(projectile.position, 8, 8, (int)CalamityDusts.SulfurousSeaAcid, 0, 0, 0, default, 0.75f);
                Main.dust[idx].noGravity = true;
                Main.dust[idx].velocity *= 3f;
                Main.dust[idx].scale = 1.8f;
            }
        }
    }
}
