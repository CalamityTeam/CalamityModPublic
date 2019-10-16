using Terraria; using CalamityMod.Projectiles; using Terraria.ModLoader;
using Terraria.ID;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Items
{
    public class PhantasmalFury : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Phantasmal Fury");
            Tooltip.SetDefault("Casts a phantasmal bolt that explodes into more bolts");
            Item.staff[item.type] = true;
        }

        public override void SetDefaults()
        {
            item.damage = 320;
            item.magic = true;
            item.mana = 20;
            item.width = 62;
            item.height = 60;
            item.useTime = 20;
            item.useAnimation = 20;
            item.useStyle = 5;
            item.noMelee = true;
            item.knockBack = 7.5f;
            item.value = Item.buyPrice(1, 40, 0, 0);
            item.rare = 10;
            item.UseSound = SoundID.Item43;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<PhantasmalFury>();
            item.shootSpeed = 12f;
            item.Calamity().postMoonLordRarity = 13;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.SpectreStaff);
            recipe.AddIngredient(null, "RuinousSoul", 2);
            recipe.AddIngredient(null, "DarkPlasma");
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
