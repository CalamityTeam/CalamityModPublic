using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Boss
{
    public class PhantomShot : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Boss/PhantomHookShot";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Phantom Shot");
        }

        public override void SetDefaults()
        {
            projectile.width = 14;
            projectile.height = 14;
            projectile.hostile = true;
            projectile.ignoreWater = true;
            projectile.alpha = 255;
            projectile.penetrate = -1;
            cooldownSlot = 1;
            projectile.Calamity().affectedByMaliceModeVelocityMultiplier = true;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(projectile.localAI[0]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            projectile.localAI[0] = reader.ReadSingle();
        }

        public override void AI()
        {
            if (projectile.ai[1] == 0f)
            {
                projectile.ai[1] = 1f;
                Main.PlaySound(SoundID.Item20, projectile.position);
            }
            projectile.rotation = (float)Math.Atan2(projectile.velocity.Y, projectile.velocity.X) + 1.57f;
            projectile.localAI[0] += 1f;
            if (projectile.localAI[0] > 9f)
            {
                projectile.alpha -= 5;
                if (projectile.alpha < 30)
                    projectile.alpha = 30;
            }
            if (projectile.localAI[0] > 180f && projectile.localAI[0] < 240f && Main.expertMode)
            {
                if (projectile.ai[0] == 0f)
                    projectile.ai[0] = projectile.velocity.Length() * 3f;

                int num189 = Player.FindClosest(projectile.Center, 1, 1);
                Vector2 vector20 = Main.player[num189].Center - projectile.Center;
                vector20.Normalize();
                vector20 *= projectile.ai[0];
                int num190 = 80;
                projectile.velocity = (projectile.velocity * (num190 - 1) + vector20) / num190;
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(100, 250, 250, projectile.alpha);
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<Nightwither>(), 120);
        }

        public override void ModifyHitPlayer(Player target, ref int damage, ref bool crit)
        {
            target.Calamity().lastProjectileHit = projectile;
        }
    }
}
