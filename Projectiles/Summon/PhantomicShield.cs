using Microsoft.Xna.Framework;
using System.IO;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class PhantomicShield : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Summon";
        public const float floatDist = 50f;

        public int deathTimer = 240;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 4;
        }

        public override void SetDefaults()
        {
            Projectile.width = 38;
            Projectile.height = 56;
            Projectile.netImportant = true;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.minionSlots = 0f;
            Projectile.timeLeft = 18000;
            Projectile.penetrate = 1;
            Projectile.tileCollide = false;
            Projectile.timeLeft *= 5;
            Projectile.minion = true;
            Projectile.DamageType = DamageClass.Summon;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(deathTimer);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            deathTimer = reader.ReadInt32();
        }

        public override void OnKill(int timeLeft)
        {
            for (int d = 0; d < 6; d++)
            {
                int shadow = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 27, 0f, 0f, 100, new Color(0, 0, 0), 2f);
                Main.dust[shadow].velocity *= 3f;
                if (Main.rand.NextBool())
                {
                    Main.dust[shadow].scale = 0.5f;
                    Main.dust[shadow].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                }
            }
            for (int d = 0; d < 18; d++)
            {
                int shadow = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 27, 0f, 0f, 100, new Color(0, 0, 0), 3f);
                Main.dust[shadow].noGravity = true;
                Main.dust[shadow].velocity *= 5f;
                shadow = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 27, 0f, 0f, 100, new Color(0, 0, 0), 2f);
                Main.dust[shadow].velocity *= 2f;
            }
        }

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            NPC target = CalamityUtils.MinionHoming(owner.position, 1500f, owner);
            if (target != null)
            {
                Vector2 pos1 = owner.position;
                Vector2 pos2 = target.position;
                Projectile.ai[0] = (pos2 - pos1).ToRotation();
            }
            else
            {
                Projectile.ai[0] -= MathHelper.ToRadians(2f);
            }
            if (target == null)
            {
                deathTimer--;
            }
            else
            {
                deathTimer = 240;
            }
            if (owner.dead || deathTimer <= 0)
                Projectile.Kill();
            Projectile.Center = owner.Center + Projectile.ai[0].ToRotationVector2() * floatDist;
            Projectile.rotation = (owner.Center - Projectile.Center).ToRotation();

            Projectile.frameCounter++;
            if (Projectile.frameCounter > 5)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
                if (Projectile.frame % 2 == 0)
                    Projectile.netUpdate = true;
            }
            if (Projectile.frame > 3)
            {
                Projectile.frame = 0;
            }
        }
    }
}
