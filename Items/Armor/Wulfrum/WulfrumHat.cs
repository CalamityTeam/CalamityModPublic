using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;

namespace CalamityMod.Items.Armor.Wulfrum
{
    [AutoloadEquip(EquipType.Head)]
    [LegacyName("WulfrumHelmet")]
    [LegacyName("WulfrumHeadSummon")]
    public class WulfrumHat : ModItem, IExtendedHat
    {
        public string ExtensionTexture => "CalamityMod/Items/Armor/Wulfrum/WulfrumHat_HeadExtension";
        public Vector2 ExtensionSpriteOffset(PlayerDrawSet drawInfo) => -Vector2.UnitY * 2f;
        public string EquipSlotName(Player drawPlayer) => drawPlayer.Male ? Name : "WulfrumHatFemale";

        public override void Load()
        {
            if (Main.netMode != NetmodeID.Server)
            {
                EquipLoader.AddEquipTexture(Mod, "CalamityMod/Items/Armor/Wulfrum/WulfrumHat_FemaleHead", EquipType.Head, name : "WulfrumHatFemale");
            }
        }

        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("Wulfrum Hat & Goggles");
            Tooltip.SetDefault("10% increased minion damage\n"+
                "Comes equipped with hair extensions"
                );
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = Item.buyPrice(0, 0, 75, 0);
            Item.rare = ItemRarityID.Blue;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<WulfrumJacket>() && legs.type == ModContent.ItemType<WulfrumOveralls>();
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = "+3 defense and +1 max minion\n" +
                "+5 defense when below 50% life";
            player.statDefense += 3; //8
            player.maxMinions++;
            if (player.statLife <= (int)(player.statLifeMax2 * 0.5))
            {
                player.statDefense += 5; //13
            }

            player.Calamity().wulfrumSet = true;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage<SummonDamageClass>() += 0.1f;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<WulfrumMetalScrap>(5).
                AddIngredient<EnergyCore>().
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
