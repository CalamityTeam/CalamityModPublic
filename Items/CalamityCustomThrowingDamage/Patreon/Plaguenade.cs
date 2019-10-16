using Terraria;
using Terraria.ID;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Items
{
    public class Plaguenade : CalamityDamageItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Plaguenade");
            Tooltip.SetDefault("Releases a swarm of angry plague bees");
        }

        public override void SafeSetDefaults()
        {
            item.width = 20;
            item.damage = 60;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.consumable = true;
            item.useAnimation = 15;
            item.useStyle = 1;
            item.useTime = 15;
            item.knockBack = 1.5f;
            item.maxStack = 999;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.height = 28;
            item.value = Item.buyPrice(0, 1, 0, 0);
            item.rare = 8;
            item.shoot = ModContent.ProjectileType<Plaguenade>();
            item.shootSpeed = 12f;
            item.Calamity().rogue = true;
            item.Calamity().postMoonLordRarity = 21;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.Beenade, 15);
            recipe.AddIngredient(null, "PlagueCellCluster", 3);
            recipe.AddIngredient(ItemID.Obsidian, 2);
            recipe.AddIngredient(ItemID.Stinger);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this, 30);
            recipe.AddRecipe();
        }
    }
}
