using CalamityMod.Buffs.Pets;
using CalamityMod.Projectiles.Pets;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Pets
{
    [LegacyName("SparksSummon")]
    public class EnchantedButterfly : ModItem, ILocalizedModType
    {
        public string LocalizationCategory => "Items.Pets";
        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.WispinaBottle);
            Item.shoot = ModContent.ProjectileType<Sparks>();
            Item.buffType = ModContent.BuffType<SparksBuff>();

            Item.value = Item.sellPrice(gold: 1);
            Item.rare = ItemRarityID.Pink;
            Item.Calamity().donorItem = true;
        }

        public override void UseStyle(Player player, Rectangle heldItemFrame)
        {
            if (player.whoAmI == Main.myPlayer && player.itemTime == 0)
            {
                player.AddBuff(Item.buffType, 3600, true);
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe(). //Oh my god this recipe is terrible no wonder no one knows this item exists
                AddIngredient(ItemID.GoldButterfly).
                AddIngredient(ItemID.MonarchButterfly).
                AddIngredient(ItemID.PurpleEmperorButterfly).
                AddIngredient(ItemID.RedAdmiralButterfly).
                AddIngredient(ItemID.UlyssesButterfly).
                AddIngredient(ItemID.SulphurButterfly).
                AddIngredient(ItemID.TreeNymphButterfly).
                AddIngredient(ItemID.ZebraSwallowtailButterfly).
                AddIngredient(ItemID.JuliaButterfly).
                AddIngredient(ItemID.HellButterfly).
                AddTile(TileID.CrystalBall).
                Register();
        }
    }
}
