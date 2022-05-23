using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace CalamityMod.Items.Armor
{
    [AutoloadEquip(EquipType.Body)]
    public class VictideBreastplate : ModItem
    {

        public override void Load()
        {
            if (Main.netMode != NetmodeID.Server)
            {
                //register the faulds texture. This appears either when the leggings  or the chestplate is equipped (both works)
                EquipLoader.AddEquipTexture(Mod, "CalamityMod/Items/Armor/VictideFaulds_Waist", EquipType.Waist, name: "VictideFaulds");
            }
        }

        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            DisplayName.SetDefault("Victide Breastplate");
            Tooltip.SetDefault("5% increased damage reduction and critical strike chance\n" +
                "+5 defense and 10% increased damage reduction while submerged in liquid");

            if (Main.netMode != NetmodeID.Server)
            {
                int equipSlot = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Body);
                ArmorIDs.Body.Sets.HidesArms[equipSlot] = true;
                ArmorIDs.Body.Sets.HidesTopSkin[equipSlot] = true;
            }
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = Item.buyPrice(0, 1, 0, 0);
            Item.rare = ItemRarityID.Green;
            Item.defense = 5; //9
        }

        public override void UpdateEquip(Player player)
        {
            player.endurance += 0.05f;
            player.GetCritChance<GenericDamageClass>() += 5;
            if (Collision.DrownCollision(player.position, player.width, player.height, player.gravDir))
            {
                player.statDefense += 5;
                player.endurance += 0.1f;
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<VictideBar>(5).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
