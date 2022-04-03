using CalamityMod.CalPlayer;
using CalamityMod.Projectiles.Summon;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.Summon
{
    class VoidConcentrationBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Concentrated Void");
            Description.SetDefault("The infinite void yearns for more...");
            Main.buffNoTimeDisplay[Type] = true;
            Main.buffNoSave[Type] = true;
            //Main.persistentBuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            CalamityPlayer mp = player.Calamity();
            int count = player.ownedProjectileCounts[ModContent.ProjectileType<VoidConcentrationAura>()];
            player.GetDamage(DamageClass.Summon) += 0.05f; //5%
            mp.voidConcentrationAura = true;
            if (!mp.voidAuraDamage && count == 0)
            {
                player.DelBuff(buffIndex);
                buffIndex--;
            }
            else
            {
                player.buffTime[buffIndex] = 18000;
            }
        }
    }
}
