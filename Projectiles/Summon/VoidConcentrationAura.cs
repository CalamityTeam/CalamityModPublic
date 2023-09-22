using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using System.IO;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class VoidConcentrationAura : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Summon";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";
        public int timer = 0;

        public override void SetDefaults()
        {
            Projectile.width = 46;
            Projectile.height = 80;
            Projectile.netImportant = true;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.minionSlots = 3f;
            Projectile.timeLeft = 18000;
            Projectile.penetrate = -1;

            Projectile.tileCollide = false;
            Projectile.timeLeft *= 5;
            Projectile.minion = true;
            Projectile.DamageType = DamageClass.Summon;
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
            Vector2 velocity = Main.MouseWorld - Main.player[Projectile.owner].Center;
            velocity.Normalize();
            velocity *= 2f;
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Main.player[Projectile.owner].Center, velocity, ModContent.ProjectileType<VoidConcentrationBlackhole>(), (int)(Projectile.damage * 5f), 0f, Projectile.owner);

            Projectile.Kill();
        }

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            CalamityPlayer mp = owner.Calamity();
            Projectile.Center = owner.Center;
            mp.voidAuraDamage = true;
            if (owner.dead)
                mp.voidAuraDamage = false;
            if (!mp.voidAuraDamage || !mp.voidConcentrationAura && Projectile.ai[0] == 1f)
            {
                mp.voidAura = false;
                Projectile.Kill();
            }
            if (owner.whoAmI == Main.myPlayer && owner.ownedProjectileCounts[Projectile.type] <= 25 && timer > 0 && timer % 4 == 0)
            {
                NPC target = CalamityUtils.MinionHoming(Projectile.Center, 1800f, owner);
                if (target != null)
                {
                    Vector2 correctedVelocity = target.Center - owner.Center;
                    correctedVelocity.Normalize();
                    correctedVelocity *= 3f;
                    int perturbificator9000 = Main.rand.Next(-1, 2);
                    Vector2 perturbedspeed = new Vector2(correctedVelocity.X + perturbificator9000, correctedVelocity.Y + perturbificator9000).RotatedBy(MathHelper.ToRadians(Main.rand.Next(1, 3)));
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, perturbedspeed, ModContent.ProjectileType<VoidConcentrationOrb>(), (int)(Projectile.damage * 0.75f), 0f, owner.whoAmI);
                }
                timer = -1;
            }
            Projectile.ai[0] = 1f;
            if (timer > 50 && timer % 4 == 0)
                return;
            timer++;
        }
    }
}
