using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatDebuffs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Rogue
{
    public class ApoctolithProj : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Rogue";
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/Apoctolith";

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.friendly = true;
            Projectile.DamageType = RogueDamageClass.Instance;
            Projectile.ignoreWater = true;
        }

        public override void AI()
        {
            //Constant rotation and gravity
            Projectile.rotation += 0.4f * Projectile.direction;
            Projectile.velocity.Y += 0.3f;
            if (Projectile.velocity.Y > 16f)
            {
                Projectile.velocity.Y = 16f;
            }
            //Dust trail
            if (Main.rand.NextBool(13))
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 180, Projectile.velocity.X * 0.25f, Projectile.velocity.Y * 0.25f, 150, default, 0.9f);
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<CrushDepth>(), 240);

            if (hit.Crit)
                target.Calamity().miscDefenseLoss = Math.Min(target.defense, 15);

            if (Projectile.Calamity().stealthStrike)
                target.AddBuff(ModContent.BuffType<Eutrophication>(), 120);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(ModContent.BuffType<CrushDepth>(), 240);
            if (Projectile.Calamity().stealthStrike)
            {
                target.AddBuff(ModContent.BuffType<Eutrophication>(), 120);
            }
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Tink, Projectile.position);
            //Dust on impact
            int dust_splash = 0;
            while (dust_splash < 9)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 180, -Projectile.velocity.X * 0.15f, -Projectile.velocity.Y * 0.15f, 120, default, 1.5f);
                dust_splash += 1;
            }
            // This only triggers if stealth is full
            if (Projectile.Calamity().stealthStrike)
            {
                int split = 0;
                while (split < 5)
                {
                    //Calculate the velocity of the projectile
                    float shardspeedX = -Projectile.velocity.X * Main.rand.NextFloat(.5f, .7f) + Main.rand.NextFloat(-3f, 3f);
                    float shardspeedY = -Projectile.velocity.Y * Main.rand.Next(50, 70) * 0.01f + Main.rand.Next(-8, 9) * 0.2f;
                    //Prevents the projectile speed from being too low
                    if (shardspeedX < 2f && shardspeedX > -2f)
                    {
                        shardspeedX += -Projectile.velocity.X;
                    }
                    if (shardspeedY > 2f && shardspeedY < 2f)
                    {
                        shardspeedY += -Projectile.velocity.Y;
                    }

                    //Spawn the projectile
                    int shard = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position.X + shardspeedX, Projectile.position.Y + shardspeedY, shardspeedX, shardspeedY, ModContent.ProjectileType<ApoctolithShard>(), (int)(Projectile.damage * 0.5), Projectile.knockBack / 2f, Projectile.owner);
                    Main.projectile[shard].frame = Main.rand.Next(3);
                    split += 1;
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 origin = new Vector2(32f, 33f);
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }

        public override void PostDraw(Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>("CalamityMod/Items/Weapons/Rogue/ApoctolithGlow").Value;
            Vector2 origin = new Vector2(32f, 33f);
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);
        }
    }
}
