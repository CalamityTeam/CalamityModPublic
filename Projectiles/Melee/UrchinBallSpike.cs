using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee
{
    public class UrchinBallSpike : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Spike");
        }

        public override void SetDefaults()
        {
            projectile.width = 6;
            projectile.height = 6;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.melee = true;
            projectile.penetrate = -1;
            aiType = ProjectileID.BoneJavelin;
        }

        public override void AI()
        {
            if (projectile.ai[0] == 0f)
            {
                projectile.velocity.X = projectile.velocity.X * 0.99f;
                projectile.velocity.Y = projectile.velocity.Y + 0.15f;
                projectile.rotation += 0.4f * (float)projectile.direction;
                projectile.spriteDirection = projectile.direction;
            }
            //Sticky Behaviour
            projectile.StickyProjAI(6);
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

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item10, projectile.position);
            projectile.position.X = projectile.position.X + (float)(projectile.width / 2);
            projectile.position.Y = projectile.position.Y + (float)(projectile.height / 2);
            projectile.width = 13;
            projectile.height = 20;
            projectile.position.X = projectile.position.X - (float)(projectile.width / 2);
            projectile.position.Y = projectile.position.Y - (float)(projectile.height / 2);
            for (int num194 = 0; num194 < 20; num194++)
            {
                int num195 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 14, 0f, 0f, 0, new Color(0, 255, 255), 1f);
                Main.dust[num195].noGravity = true;
                Main.dust[num195].velocity *= 2f;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (target.type == NPCID.KingSlime || target.type == NPCID.WallofFlesh || target.type == NPCID.WallofFleshEye ||
                target.type == NPCID.SkeletronHead || target.type == NPCID.SkeletronHand)
            {
                target.buffImmune[BuffID.Venom] = false;
            }
            target.AddBuff(BuffID.Venom, 120);
        }
    }
}
