using CalamityMod.Projectiles.Melee;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class LionfishProj : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/Lionfish";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Lionfish");
        }

        public override void SetDefaults()
        {
            projectile.width = 30;
            projectile.height = 30;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.penetrate = -1;
            projectile.alpha = 255;
            projectile.Calamity().rogue = true;
            projectile.timeLeft = CalamityUtils.SecondsToFrames(20f);
        }

        public override void AI()
        {
            if (projectile.alpha > 0)
            {
                projectile.alpha -= 25;
            }
            if (projectile.alpha < 0)
            {
                projectile.alpha = 0;
            }
            if (projectile.ai[0] == 0f)
            {
                projectile.spriteDirection = projectile.direction = (projectile.velocity.X > 0).ToDirectionInt();
                projectile.rotation = projectile.velocity.ToRotation() + (projectile.spriteDirection == 1 ? 0f : MathHelper.Pi);
                projectile.ai[1] += 1f;
                if (projectile.ai[1] >= 45f)
                {
                    float horizontalMult = 0.98f;
                    float fallSpeed = 0.35f;
                    projectile.velocity.X *= horizontalMult;
                    projectile.velocity.Y += fallSpeed;
                }
                if (projectile.Calamity().stealthStrike)
                {
                    if (projectile.timeLeft % 8 == 0 && projectile.owner == Main.myPlayer)
                    {
                        Vector2 velocity = projectile.DirectionFrom(Main.player[projectile.owner].Center);
                        velocity *= Main.rand.NextFloat(4.5f, 6.5f);
                        velocity = velocity.RotatedBy((Main.rand.NextDouble() - 0.5) * Math.PI * 0.5, default);
                        int spike = Projectile.NewProjectile(projectile.Center, velocity, ModContent.ProjectileType<UrchinSpikeFugu>(), (int)(projectile.damage * 0.5), projectile.knockBack * 0.5f, projectile.owner, -10f, 0f);
                        if (spike.WithinBounds(Main.maxProjectiles))
                            Main.projectile[spike].Calamity().forceRogue = true;
                    }
                }
            }
            //Sticky Behaviour
            projectile.StickyProjAI(15);
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            projectile.ModifyHitNPCSticky(6, false);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (targetHitbox.Width > 8 && targetHitbox.Height > 8)
            {
                targetHitbox.Inflate(-targetHitbox.Width / 8, -targetHitbox.Height / 8);
            }
            return null;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D tex = Main.projectileTexture[projectile.type];
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (projectile.spriteDirection == -1)
                spriteEffects = SpriteEffects.FlipHorizontally;
            spriteBatch.Draw(tex, projectile.Center - Main.screenPosition, null, projectile.GetAlpha(lightColor), projectile.rotation, tex.Size() / 2f, projectile.scale, spriteEffects, 0f);
            return false;
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Dig, projectile.position);
            CalamityGlobalProjectile.ExpandHitboxBy(projectile, 72);
            for (int d = 0; d < 3; d++)
            {
                Dust.NewDust(projectile.position, projectile.width, projectile.height, 14, 0f, 0f, 100, new Color(0, 255, 255), 1.5f);
            }
            for (int d = 0; d < 30; d++)
            {
                int idx = Dust.NewDust(projectile.position, projectile.width, projectile.height, 14, 0f, 0f, 0, new Color(0, 255, 255), 2.5f);
                Main.dust[idx].noGravity = true;
                Main.dust[idx].velocity *= 3f;
                idx = Dust.NewDust(projectile.position, projectile.width, projectile.height, 14, 0f, 0f, 100, new Color(0, 255, 255), 1.5f);
                Main.dust[idx].velocity *= 2f;
                Main.dust[idx].noGravity = true;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (target.type == NPCID.KingSlime || target.type == NPCID.WallofFlesh || target.type == NPCID.WallofFleshEye ||
                target.type == NPCID.SkeletronHead || target.type == NPCID.SkeletronHand)
            {
                target.buffImmune[BuffID.Venom] = false;
            }
            target.AddBuff(BuffID.Venom, 240);
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.Venom, 240);
        }
    }
}
