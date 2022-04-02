using CalamityMod.Buffs.StatDebuffs;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class TeardropCleaver : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Teardrop Cleaver");
            Tooltip.SetDefault("Makes your enemies cry");
        }

        public override void SetDefaults()
        {
            item.width = 56;
            item.damage = 24;
            item.melee = true;
            item.useAnimation = 24;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useTime = 24;
            item.useTurn = true;
            item.knockBack = 5.5f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.height = 66;
            item.value = Item.buyPrice(0, 2, 0, 0);
            item.rare = ItemRarityID.Green;
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<TemporalSadness>(), 60);
        }
    }
}
