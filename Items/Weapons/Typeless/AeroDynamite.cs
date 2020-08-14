using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Typeless;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Typeless
{
    public class AeroDynamite : ModItem
    {
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/Turbulance";
        public static int Damage = 100;
        public static float Knockback = 10f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Aero Dynamite");
            Tooltip.SetDefault("Throws a floaty explosive that defies gravity");
        }

        public override void SetDefaults()
        {
            item.useTime = item.useAnimation = 40;
			item.maxStack = 999;
			item.consumable = true;
			item.shootSpeed = 6f;
			item.shoot = ModContent.ProjectileType<AeroExplosive>();

			item.width = 8;
			item.height = 28;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.noMelee = true;
			item.noUseGraphic = true;
            item.UseSound = SoundID.Item1;
            item.value = Item.buyPrice(0, 0, 50, 0); //Craft 8 at a time
            item.rare = 3;
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
            recipe.AddIngredient(ModContent.ItemType<AerialiteBar>(), 4);
            recipe.AddIngredient(ItemID.Cloud, 20);
            recipe.AddIngredient(ItemID.Dynamite, 8);
            recipe.AddTile(TileID.SkyMill);
            recipe.SetResult(this, 8);
            recipe.AddRecipe();
        }
    }
}
