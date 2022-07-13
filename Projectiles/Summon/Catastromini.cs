using CalamityMod.CalPlayer;
using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class Catastromini : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Catastromini");
            Main.projFrames[Projectile.type] = 3;
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 36;
            Projectile.netImportant = true;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.minionSlots = 0.5f;
            Projectile.timeLeft = 18000;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft *= 5;
            Projectile.minion = true;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 8;
            Projectile.DamageType = DamageClass.Summon;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();
            if (Projectile.localAI[0] == 0f)
            {
                int dustAmt = 36;
                for (int d = 0; d < dustAmt; d++)
                {
                    Vector2 source = Vector2.Normalize(Projectile.velocity) * new Vector2((float)Projectile.width / 2f, (float)Projectile.height) * 0.75f;
                    source = source.RotatedBy((double)((float)(d - (dustAmt / 2 - 1)) * MathHelper.TwoPi / (float)dustAmt), default) + Projectile.Center;
                    Vector2 dustVel = source - Projectile.Center;
                    int brim = Dust.NewDust(source + dustVel, 0, 0, (int)CalamityDusts.Brimstone, dustVel.X * 1.75f, dustVel.Y * 1.75f, 100, default, 1.1f);
                    Main.dust[brim].noGravity = true;
                    Main.dust[brim].velocity = dustVel;
                }
                Projectile.localAI[0] += 1f;
            }
            bool correctMinion = Projectile.type == ModContent.ProjectileType<Catastromini>();
            if (correctMinion)
            {
                if (player.dead)
                {
                    modPlayer.cEyes = false;
                }
                if (modPlayer.cEyes)
                {
                    Projectile.timeLeft = 2;
                }
            }

            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.Pi;
            Projectile.frameCounter++;
            if (Projectile.frameCounter > (Projectile.ai[0] == 2f ? 1 : 3))
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame > 2)
            {
                Projectile.frame = 0;
            }

            Projectile.ChargingMinionAI(Calamitamini.Range, Calamitamini.SeparationAnxietyMin, Calamitamini.SeparationAnxietyMax, Calamitamini.SafeDist, 0, 40f, 8f, 4f, new Vector2(0f, -60f), 40f, 8f, false, true);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            SpriteEffects spriteEffects = Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            int frameHeight = texture.Height / Main.projFrames[Projectile.type];
            int yStart = frameHeight * Projectile.frame;
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, yStart, texture.Width, frameHeight)), Projectile.GetAlpha(lightColor), Projectile.rotation, new Vector2(texture.Width / 2f, frameHeight / 2f), Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}
