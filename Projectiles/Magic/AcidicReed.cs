using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Dusts;
using Terraria;
using Terraria.ModLoader;
using Terraria.Audio;
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
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.timeLeft = 600;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = false;
            Projectile.penetrate = 1;
        }

        public override void AI()
        {
            if (Projectile.ai[0] == 1f)
            {
                Terraria.Audio.LegacySoundStyle saxSound = Utils.SelectRandom(Main.rand, new Terraria.Audio.LegacySoundStyle[]
                {
                    Mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/Saxophone/Sax1"),
                    Mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/Saxophone/Sax2"),
                    Mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/Saxophone/Sax3"),
                    Mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/Saxophone/Sax4"),
                    Mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/Saxophone/Sax5"),
                    Mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/Saxophone/Sax6")
                });
                SoundEngine.PlaySound(saxSound, Projectile.position);
                Projectile.ai[0] = 0f;
            }
            if (Projectile.velocity.Y < 10f)
                Projectile.velocity.Y += 0.25f;
            Projectile.rotation = Projectile.velocity.ToRotation();
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
                int idx = Dust.NewDust(Projectile.position, 8, 8, (int)CalamityDusts.SulfurousSeaAcid, 0, 0, 0, default, 0.75f);
                Main.dust[idx].noGravity = true;
                Main.dust[idx].velocity *= 3f;
                idx = Dust.NewDust(Projectile.position, 8, 8, (int)CalamityDusts.SulfurousSeaAcid, 0, 0, 0, default, 0.75f);
                Main.dust[idx].noGravity = true;
                Main.dust[idx].velocity *= 3f;
            }
        }
    }
}
