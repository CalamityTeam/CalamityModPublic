using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class FinalDawnFireball : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Rogue";
        public const float DesiredSpeed = 30;
        public const float InterpolationTime = 10;
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 4;
        }
        public override void SetDefaults()
        {
            Projectile.width = 36;
            Projectile.height = 80;
            Projectile.DamageType = RogueDamageClass.Instance;
            Projectile.penetrate = 1;
            Projectile.hostile = false;
            Projectile.friendly = false;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.alpha = 50;
            Projectile.timeLeft = 180;
        }
        public override void AI()
        {
            NPC chargeAt = Main.npc[(int)Projectile.ai[1]];
            if(!chargeAt.active)
                Projectile.Kill();

            int idx = Dust.NewDust(Projectile.position, Projectile.width , Projectile.height, ModContent.DustType<FinalFlame>(), 0f, 0f, 0, default, 1.0f);
            Main.dust[idx].velocity = Projectile.velocity * 0.5f;
            Main.dust[idx].noGravity = true;
            Main.dust[idx].noLight = true;

            Projectile.ai[0]++;
            if (Projectile.ai[0] >= 20)
            {
                Projectile.friendly = true;
                NPC npc = Main.npc[(int)Projectile.ai[1]];
                Vector2 desiredVelocity = Projectile.SafeDirectionTo(npc.Center) * DesiredSpeed;
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, desiredVelocity, 1f / InterpolationTime);
                if(!npc.active)
                    Projectile.Kill();
            }

            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 5)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
                if (Projectile.frame >= Main.projFrames[Projectile.type])
                    Projectile.frame = 0;
            }
        }
        public override void OnKill(int timeLeft)
        {
            if (Main.myPlayer == Projectile.owner)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X, Projectile.Center.Y, 0f, 0f, ModContent.ProjectileType<FinalDawnReticle>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D glowmask = ModContent.Request<Texture2D>(Texture).Value;
            int height = ModContent.Request<Texture2D>(Texture).Value.Height / Main.projFrames[Projectile.type];
            int yStart = height * Projectile.frame;
            Main.spriteBatch.Draw(glowmask,
                                  Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY),
                                  new Rectangle?(new Rectangle(0, yStart, glowmask.Width, height)),
                                  Projectile.GetAlpha(Color.White), Projectile.rotation,
                                  new Vector2(glowmask.Width / 2f, height / 2f), Projectile.scale,
                                  Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally,
                                  0f);
            return false;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<Dragonfire>(), 240);
        }
    }
}
