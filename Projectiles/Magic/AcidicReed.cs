using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Dusts;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Magic
{
    public class AcidicReed : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Reed");
        }

        public override void SetDefaults()
        {
            projectile.width = 10;
            projectile.height = 10;
            projectile.friendly = true;
            projectile.magic = true;
            projectile.timeLeft = 600;
            projectile.tileCollide = true;
            projectile.ignoreWater = false;
            projectile.penetrate = 1;
        }

        public override void AI()
        {
            if (projectile.ai[0] == 1f)
            {
                Terraria.Audio.LegacySoundStyle saxSound = Utils.SelectRandom(Main.rand, new Terraria.Audio.LegacySoundStyle[]
                {
                    mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/Saxophone/Sax1"),
                    mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/Saxophone/Sax2"),
                    mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/Saxophone/Sax3"),
                    mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/Saxophone/Sax4"),
                    mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/Saxophone/Sax5"),
                    mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/Saxophone/Sax6")
                });
                Main.PlaySound(saxSound, projectile.position);
                projectile.ai[0] = 0f;
            }
            if (projectile.velocity.Y < 10f)
                projectile.velocity.Y += 0.25f;
            projectile.rotation = projectile.velocity.ToRotation();
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<Irradiated>(), 180);
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<Irradiated>(), 180);
        }

        public override void Kill(int timeLeft)
        {
            for (int i = 0; i <= 2; i++)
            {
                int idx = Dust.NewDust(projectile.position, 8, 8, (int)CalamityDusts.SulfurousSeaAcid, 0, 0, 0, default, 0.75f);
                Main.dust[idx].noGravity = true;
                Main.dust[idx].velocity *= 3f;
                idx = Dust.NewDust(projectile.position, 8, 8, (int)CalamityDusts.SulfurousSeaAcid, 0, 0, 0, default, 0.75f);
                Main.dust[idx].noGravity = true;
                Main.dust[idx].velocity *= 3f;
            }
        }
    }
}
