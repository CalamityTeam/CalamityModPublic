using CalamityMod.Buffs.Summon;
using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class CrimslimeMinion : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Summon";
        public float dust = 0f;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 6;
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 26;
            Projectile.height = 26;
            Projectile.netImportant = true;
            Projectile.friendly = true;
            Projectile.minionSlots = 1;
            Projectile.alpha = 75;
            Projectile.aiStyle = ProjAIStyleID.Pet;
            Projectile.timeLeft = 18000;
            Projectile.penetrate = -1;
            Projectile.timeLeft *= 5;
            Projectile.minion = true;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 23;
            AIType = ProjectileID.BabySlime;
            Projectile.DamageType = DamageClass.Summon;
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            fallThrough = false;
            return true;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();
            if (dust == 0f)
            {
                int constant = 16;
                for (int i = 0; i < constant; i++)
                {
                    Vector2 rotate = Vector2.Normalize(Projectile.velocity) * new Vector2((float)Projectile.width / 2f, (float)Projectile.height) * 0.75f;
                    rotate = rotate.RotatedBy((double)((float)(i - (constant / 2 - 1)) * 6.28318548f / (float)constant), default) + Projectile.Center;
                    Vector2 faceDirection = rotate - Projectile.Center;
                    int dust = Dust.NewDust(rotate + faceDirection, 0, 0, 5, faceDirection.X * 1f, faceDirection.Y * 1f, 100, default, 1.1f);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].noLight = true;
                    Main.dust[dust].velocity = faceDirection;
                }
                dust += 1f;
            }
            bool isMinion = Projectile.type == ModContent.ProjectileType<CrimslimeMinion>();
            player.AddBuff(ModContent.BuffType<Crimslime>(), 3600);
            if (isMinion)
            {
                if (player.dead)
                {
                    modPlayer.cSlime2 = false;
                }
                if (modPlayer.cSlime2)
                {
                    Projectile.timeLeft = 2;
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = ModContent.Request<Texture2D>(Texture).Value;
            int framing = texture2D13.Height / Main.projFrames[Projectile.type];
            int y6 = framing * Projectile.frame;
            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, y6, texture2D13.Width, framing)), Projectile.GetAlpha(lightColor), Projectile.rotation, new Vector2((float)texture2D13.Width / 2f, (float)framing / 2f), Projectile.scale, SpriteEffects.None, 0);
            return false;
        }

        public override bool OnTileCollide(Vector2 oldVelocity) => false;
    }
}
