using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class IchorSpearProj : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/IchorSpear";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Spear");
        }

        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 2;
            Projectile.aiStyle = 113;
            Projectile.timeLeft = 600;
            aiType = ProjectileID.BoneJavelin;
            Projectile.Calamity().rogue = true;
            Projectile.localNPCHitCooldown = 10;
        }

        public override void AI()
        {
            if (Main.rand.NextBool(4))
            {
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, 246, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f);
            }
            Projectile.rotation = (float)Math.Atan2((double)Projectile.velocity.Y, (double)Projectile.velocity.X) + 0.785f;
            if (Projectile.spriteDirection == -1)
            {
                Projectile.rotation -= 1.57f;
            }

            if (Projectile.Calamity().stealthStrike)
            {
                if (Projectile.timeLeft % 8 == 0)
                {
                    if (Projectile.owner == Main.myPlayer)
                    {
                        Vector2 velocity = new Vector2(Main.rand.NextFloat(-14f, 14f), Main.rand.NextFloat(-14f, 14f));
                        int ichor = Projectile.NewProjectile(Projectile.Center, velocity, Main.rand.NextBool(2) ? ProjectileID.GoldenShowerFriendly : ProjectileID.IchorSplash, (int)(Projectile.damage * 0.5), Projectile.knockBack * 0.5f, Projectile.owner);
                        if (ichor.WithinBounds(Main.maxProjectiles))
                        {
                            Main.projectile[ichor].Calamity().forceRogue = true;
                            Main.projectile[ichor].usesLocalNPCImmunity = true;
                            Main.projectile[ichor].localNPCHitCooldown = 10;
                            Main.projectile[ichor].extraUpdates = 2;
                        }
                    }
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, tex.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }

        public override void Kill(int timeLeft)
        {
            for (int i = 0; i <= 10; i++)
            {
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, 246, Projectile.oldVelocity.X * 0.5f, Projectile.oldVelocity.Y * 0.5f);
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Ichor, Projectile.Calamity().stealthStrike ? 600 : 120);
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.Ichor, Projectile.Calamity().stealthStrike ? 600 : 120);
        }
    }
}
