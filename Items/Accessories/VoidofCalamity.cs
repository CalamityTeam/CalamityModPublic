using CalamityMod.CalPlayer;
using CalamityMod.Projectiles.Typeless;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.GameContent.Creative;

namespace CalamityMod.Items.Accessories
{
    public class VoidofCalamity : ModItem
    {
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            DisplayName.SetDefault("Void of Calamity");
            Tooltip.SetDefault("12% increase to all damage\n" +
            "Brimstone fire rains down after getting hit");
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 22;
            Item.value = CalamityGlobalItem.Rarity7BuyPrice;
            Item.rare = ItemRarityID.Lime;
            Item.accessory = true;
        }

        public override bool CanEquipAccessory(Player player, int slot, bool modded) => !player.Calamity().voidOfCalamity;

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.voidOfCalamity = true;
            player.GetDamage<GenericDamageClass>() += 0.12f;
            if (player.whoAmI == Main.myPlayer)
            {
                var source = player.GetSource_Accessory(Item);
                if (player.immune)
                {
                    if (player.miscCounter % 10 == 0)
                    {
                        CalamityUtils.ProjectileRain(source, player.Center, 400f, 100f, 500f, 800f, 22f, ModContent.ProjectileType<StandingFire>(), (int)(30 * player.AverageDamage()), 5f, player.whoAmI);
                    }
                }
            }
        }
    }
}
