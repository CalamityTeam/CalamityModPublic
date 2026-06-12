using CalamityMod.Buffs.Pets;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Pets;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Pets
{
    public class StarSwallowerUnit : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Pets";
        public override void SetDefaults()
        {
            Item.DefaultToVanitypet(ModContent.ProjectileType<StarSwallowerPet>(), ModContent.BuffType<StarSwallowerPetBuff>());
            Item.value = Item.sellPrice(gold: 1);
            Item.rare = ModContent.RarityType<DarkOrange>();
            Item.useStyle = ItemUseStyleID.HoldUp;
        }

        public override void UseStyle(Player player, Rectangle heldItemFrame)
        {
            if (player.whoAmI == Main.myPlayer && player.itemTime == 0)
            {
                player.AddBuff(Item.buffType, 15, true);
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.Frog).
                AddIngredient<MysteriousCircuitry>(8).
                AddIngredient<DubiousPlating>(4).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
