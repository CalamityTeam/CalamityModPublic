using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Typeless;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Typeless
{
	public class AeroDynamite : ModItem
    {
        public static int Damage = 250;
        public static float Knockback = 10f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Skynamite");
            Tooltip.SetDefault("You don't need an aerodynamics major to use this\n" +
			"Throws a floaty explosive that defies gravity");
        }

        public override void SetDefaults()
        {
            item.useTime = item.useAnimation = 40;
			item.maxStack = 999;
			item.consumable = true;
			item.shootSpeed = 5f;
			item.shoot = ModContent.ProjectileType<AeroExplosive>();

			item.width = 8;
			item.height = 28;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.noMelee = true;
			item.noUseGraphic = true;
            item.UseSound = SoundID.Item1;
            item.value = Item.buyPrice(0, 0, 40, 0); // Crafted 10 at a time
            item.rare = ItemRarityID.Orange;
        }

        public override void UpdateInventory(Player player)
        {
			//Get the Demolitionist to spawn. Also check for Merchant because he does that.
			if (NPC.FindFirstNPC(NPCID.Merchant) != -1 && NPC.FindFirstNPC(NPCID.Demolitionist) == -1)
				Main.townNPCCanSpawn[NPCID.Demolitionist] = true;
		}

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.Dynamite, 10);
            recipe.AddIngredient(ModContent.ItemType<AerialiteBar>(), 1);
            recipe.AddTile(TileID.SkyMill);
            recipe.SetResult(this, 10);
            recipe.AddRecipe();
        }
    }
}
