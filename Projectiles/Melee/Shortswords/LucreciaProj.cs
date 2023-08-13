using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System;
using CalamityMod.Projectiles.BaseProjectiles;

namespace CalamityMod.Projectiles.Melee.Shortswords
{
    public class LucreciaProj: BaseShortswordProjectile
    {
        public const int OnHitIFrames = 5;
        public override string Texture => "CalamityMod/Items/Weapons/Melee/Lucrecia";

        public override void SetDefaults()
        {
            Projectile.Size = new Vector2(31);
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.scale = 1f;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.ownerHitCheck = true;
            Projectile.timeLeft = 360;
            Projectile.hide = true;
            Projectile.ownerHitCheck = true;
        }

        public override Action<Projectile> EffectBeforePullback => (proj) =>
        {
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity * 12f, ModContent.ProjectileType<DNA>(), Projectile.damage, Projectile.knockBack, Projectile.owner, 0f, 0f);
        };


        public override void SetVisualOffsets()
        {
            const int HalfSpriteWidth = 62 / 2;
            const int HalfSpriteHeight = 62 / 2;

            int HalfProjWidth = Projectile.width / 2;
            int HalfProjHeight = Projectile.height / 2;

            DrawOriginOffsetX = 0;
            DrawOffsetX = -(HalfSpriteWidth - HalfProjWidth);
            DrawOriginOffsetY = -(HalfSpriteHeight - HalfProjHeight);
        }

        public override void ExtraBehavior()
        {
            if (Main.rand.NextBool(5))
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.BoneTorch);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Main.player[Projectile.owner].GiveIFrames(OnHitIFrames, false);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            Player player = Main.player[Projectile.owner];
            bool isImmune = false;
            for (int j = 0; j < player.hurtCooldowns.Length; j++)
            {
                if (player.hurtCooldowns[j] > 0)
                    isImmune = true;
            }
            if (!isImmune)
            {
                Owner.GiveIFrames(OnHitIFrames, true);
            }
        }
    }
}
