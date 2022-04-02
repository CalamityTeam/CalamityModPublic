using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using System.IO;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    class VoidConcentrationAura : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";
        public int timer = 0;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Concentrated Void Aura");
        }

        public override void SetDefaults()
        {
            projectile.width = 46;
            projectile.height = 80;
            projectile.netImportant = true;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.minionSlots = 3f;
            projectile.timeLeft = 18000;
            projectile.penetrate = -1;
            
            projectile.tileCollide = false;
            projectile.timeLeft *= 5;
            projectile.minion = true;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(timer);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            timer = reader.ReadInt32();
        }

        public override bool? CanCutTiles()
        {
            return true;
        }

        public void HandleRightClick()
        {
            Vector2 velocity = Main.MouseWorld - Main.player[projectile.owner].Center;
            velocity.Normalize();
            velocity *= 2f;
            Projectile.NewProjectile(Main.player[projectile.owner].Center, velocity, ModContent.ProjectileType<VoidConcentrationBlackhole>(), (int)(projectile.damage * 5f), 0f, projectile.owner);
            projectile.Kill();
        }

        public override void AI()
        {
            Player owner = Main.player[projectile.owner];
            CalamityPlayer mp = owner.Calamity();
            projectile.Center = owner.Center;
            mp.voidAuraDamage = true;
            if (owner.dead)
                mp.voidAuraDamage = false;
            if (!mp.voidAuraDamage || !mp.voidConcentrationAura && projectile.ai[0] == 1f)
            {
                mp.voidAura = false;
                projectile.Kill();
            }
            if (owner.whoAmI == Main.myPlayer && owner.ownedProjectileCounts[projectile.type] <= 25 && timer > 0 && timer % 4 == 0)
            {
                NPC target = CalamityUtils.MinionHoming(projectile.Center, 1800f, owner);
                if (target != null)
                {
                    Vector2 correctedVelocity = target.Center - owner.Center;
                    correctedVelocity.Normalize();
                    correctedVelocity *= 3f;
                    int perturbificator9000 = Main.rand.Next(-1, 2);
                    Vector2 perturbedspeed = new Vector2(correctedVelocity.X + perturbificator9000, correctedVelocity.Y + perturbificator9000).RotatedBy(MathHelper.ToRadians(Main.rand.Next(1, 3)));
                    Projectile.NewProjectile(projectile.Center, perturbedspeed, ModContent.ProjectileType<VoidConcentrationOrb>(), (int)(projectile.damage * 0.75f), 0f, owner.whoAmI);
                }
                timer = -1;
            }
            projectile.ai[0] = 1f;
            if (timer > 50 && timer % 4 == 0)
                return;
            timer++;
        }
    }
}
