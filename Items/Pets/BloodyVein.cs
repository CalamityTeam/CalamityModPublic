using CalamityMod.Buffs.Pets;
using CalamityMod.Projectiles.Pets;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Pets
{
    public class BloodyVein : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bloody Vein");
            Tooltip.SetDefault("Summons an amalgamated pile of flesh");
        }

        public override void SetDefaults()
        {
            item.damage = 0;
            item.useTime = 20;
            item.useAnimation = 20;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.noMelee = true;
            item.width = 24;
            item.height = 48;
            item.UseSound = SoundID.NPCHit9;
            item.shoot = ModContent.ProjectileType<PerforaMini>();
            item.buffType = ModContent.BuffType<BloodBound>();

            item.value = Item.buyPrice(gold: 4);
            item.rare = ItemRarityID.Orange;
            item.Calamity().donorItem = true;
        }

        public override void UseStyle(Player player)
        {
            if (player.whoAmI == Main.myPlayer && player.itemTime == 0)
            {
                player.AddBuff(item.buffType, 3600, true);
            }
        }
    }
}
