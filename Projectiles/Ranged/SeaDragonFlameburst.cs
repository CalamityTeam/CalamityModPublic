using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.Audio;


namespace CalamityMod.Projectiles.Ranged
{
    public class SeaDragonFlameburst : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Ranged";
        public int frameX = 0;
        public int frameY = 0;
        public int currentFrame => frameY + frameX * 4;

        public override void SetDefaults() //Mostly Copied from ArcherfishRing
        {
            Projectile.width = 322;
            Projectile.height = 81;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.alpha = 225;
            Projectile.timeLeft = 20;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.Calamity().pointBlankShotDuration = CalamityGlobalProjectile.DefaultPointBlankDuration;
        }

        public override void AI() //Mostly Copied from SubductionFlameburst and ArcherfishRing
        {
            //2-6
            Projectile.frameCounter += 1;
            if (Projectile.frameCounter % 7 == 6)
            {
                frameY += 1;
                if (frameY >= 4)
                {
                    frameX += 1;
                    frameY = 0;
                }
                if (frameX >= 3)
                {
                    Projectile.Kill();
                }
            }
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
        }



        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            Rectangle frame = new Rectangle(frameX * Projectile.width, frameY * Projectile.height, Projectile.width, Projectile.height);
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, frame, Color.White, Projectile.rotation, Projectile.Size / 2, 1f, SpriteEffects.None, 0);
            return false;
        }
        
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            //Audio feedback for hitting the blast
            Player user = Main.player[Projectile.owner];
            SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode, Projectile.position);

            target.AddBuff(BuffID.OnFire3, 240);
            target.AddBuff(BuffID.Daybreak, 420);
        }
    }
}
