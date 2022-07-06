using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class HadalUrnJellyfish : ModProjectile
    {
        bool neartarget = false;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mirage Jelly");
            Main.projFrames[Projectile.type] = 5;
        }

        public override void SetDefaults()
        {
            Projectile.width = 38;
            Projectile.height = 38;
            Projectile.friendly = true;
            Projectile.timeLeft = 600;
            Projectile.penetrate = 3;
            Projectile.alpha = 255;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 16;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            Projectile.ai[0]++; //Timer for the jellyfish's hopping
            Projectile.ai[1]--; //Handles the existence of the aura and what happens afterwards
            Projectile.frameCounter++;
            if (Projectile.frameCounter > 8)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame >= Main.projFrames[Projectile.type])
            {
                Projectile.frame = 0;
            }

            if (Projectile.timeLeft > 60)
            {
                if (Projectile.alpha > 0)
                {
                    Projectile.alpha -= 25;
                }
                if (Projectile.alpha < 0)
                {
                    Projectile.alpha = 0;
                }
            }
            else
            {
                Projectile.alpha += 4;
                if (Projectile.alpha >= 255)
                {
                    Projectile.active = false;
                }
            }
            //Hop towards target every 1.5 seconds. Does not occur after aura
            if (Projectile.ai[0] % 90 == 0 && Projectile.ai[1] <= 0)
                CalamityUtils.HomeInOnNPC(Projectile, true, 1200f, 50f, 20f);
            else
                Projectile.velocity *= 0.985f;

            //Detect if any enemies are very close
            int maxDistance = 10;

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                if (Main.npc[i].CanBeChasedBy(Projectile, false))
                {
                    float extraDistance = (Main.npc[i].width / 2) + (Main.npc[i].height / 2);

                    bool canHit = true;
                    if (extraDistance < maxDistance)
                        canHit = Collision.CanHit(Projectile.Center, 1, 1, Main.npc[i].Center, 1, 1);

                    if (Projectile.WithinRange(Main.npc[i].Center, maxDistance + extraDistance) && canHit)
                    {
                        neartarget = true;
                    }
                }
            }
            //If an enemy is close, spawn an aura which also expands the hitbox of the jelly
            if (neartarget && Projectile.ai[1] <= 0)
            {
                Projectile.ExpandHitboxBy(Projectile.width * 2, Projectile.height * 2);
                Projectile.ai[1] = 120;
            }
            //While the aura is alive, pierce infinitely and stop in place
            if (Projectile.ai[1] > 80)
            {
                Projectile.velocity *= 0.7f;
                //Only spawn the aura once the jellies have fully stopped to prevent offcentered memes
                if (Projectile.ai[1] == 110)
                {
                    BloomRing blom = new BloomRing(Projectile.Center, Vector2.Zero, Color.Aqua, Projectile.scale / 2, 40);
                    GeneralParticleHandler.SpawnParticle(blom);
                    blom.Position = Projectile.Center;

                    Particle Bloom = new StrongBloom(Projectile.Center, Vector2.Zero, Color.Aqua * 0.6f, Projectile.scale * (1f + Main.rand.NextFloat(0f, 1.5f)) / 2, 40);
                    GeneralParticleHandler.SpawnParticle(Bloom);
                    Bloom.Position = Projectile.Center;

                    Projectile.penetrate = -1;
                    Projectile.localNPCHitCooldown = 6;
                }
            }
            //Once the aura dies, shrink back to normal size and disappear (it ran out of juice)
            if (Projectile.ai[1] == 80)
            {
                Projectile.ExpandHitboxBy(Projectile.width / 2, Projectile.height / 2);
                Projectile.localNPCHitCooldown = 16;
                Projectile.timeLeft = 60;
            }
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<CrushDepth>(), 240);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            int textureheight = tex.Height / Main.projFrames[Projectile.type];
            int y = textureheight * Projectile.frame;
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, y, tex.Width, textureheight)), Projectile.GetAlpha(lightColor), Projectile.rotation, new Vector2((float)tex.Width / 2f, (float)textureheight / 2f), Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}
