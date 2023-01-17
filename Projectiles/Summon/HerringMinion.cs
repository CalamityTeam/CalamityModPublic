using CalamityMod.Buffs.Summon;
using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class HerringMinion : ModProjectile
    {
        public Player Owner => Main.player[Projectile.owner];

        public CalamityPlayer moddedOwner => Owner.Calamity();
        
        public int dustEffect = 3;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Herring");
            Main.projFrames[Projectile.type] = 8;
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 60;
            Projectile.height = 24;
            Projectile.netImportant = true;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.minionSlots = 0.5f;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.minion = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.usesIDStaticNPCImmunity= true;
            Projectile.idStaticNPCHitCooldown = 8;
        }

        public override void AI()
        {
            CheckMinionExistance(); // Checks if the minion can still exist.
            SpawnEffect(); // Does a dust effectw where it spawns.
            DoAnimation(); // Does the animation of the minion.
            PointInRightDirection(); // Points at where it's going.

            Projectile.rotation = Projectile.velocity.X * 0.05f; // Does a little movement effect with it's rotation when it's moving.

            Projectile.ChargingMinionAI(1200f, 1500f, 2200f, 150f, 0, 24f, 15f, 4f, new Vector2(0f, -60f), 12f, 12f, true, true, 1);
        }

        public void CheckMinionExistance()
        {
            Owner.AddBuff(ModContent.BuffType<Herring>(), 1);
            if (Projectile.type == ModContent.ProjectileType<HerringMinion>())
            {
                if (Owner.dead)
                    moddedOwner.herring = false;
                if (moddedOwner.herring)
                    Projectile.timeLeft = 2;
            }
        }

        public void SpawnEffect()
        {
            if (dustEffect > 0)
            {
                int num226 = 36;
                for (int num227 = 0; num227 < num226; num227++)
                {
                    Vector2 vector6 = Vector2.Normalize(Projectile.velocity) * new Vector2(Projectile.width / 2f, Projectile.height) * 0.75f;
                    vector6 = vector6.RotatedBy((double)((num227 - (num226 / 2 - 1)) * 6.28318548f / num226), default) + Projectile.Center;
                    Vector2 vector7 = vector6 - Projectile.Center;
                    int num228 = Dust.NewDust(vector6 + vector7, 0, 0, DustID.Water, vector7.X * 1.75f, vector7.Y * 1.75f, 100, default, 1.1f);
                    Main.dust[num228].noGravity = true;
                    Main.dust[num228].velocity = vector7;
                }
                dustEffect--;
            }
        }

        public void DoAnimation()
        {
            Projectile.frameCounter++;
            Projectile.frame = Projectile.frameCounter / 8 % Main.projFrames[Projectile.type];
        }

        public void PointInRightDirection()
        {
            if (Math.Abs(Projectile.velocity.X) > 0.1f)
                Projectile.spriteDirection = Math.Sign(Projectile.velocity.X);
        }

        public override bool OnTileCollide(Vector2 oldVelocity) => false;
    }
}
