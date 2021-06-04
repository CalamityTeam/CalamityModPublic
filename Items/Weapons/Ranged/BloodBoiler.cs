using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class BloodBoiler : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Blood Boiler");
            Tooltip.SetDefault("Fires a stream of lifestealing bloodfire\n" +
                "Uses your health as ammo\n" + "25% chance to not consume ammo");
        }

        public override void SetDefaults()
        {
            item.damage = 250;
            item.ranged = true;
            item.width = 60;
            item.height = 30;
            item.useTime = 5;
            item.useAnimation = 15;
            item.autoReuse = true;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 4f;
            item.value = CalamityGlobalItem.Rarity12BuyPrice;
            item.rare = ItemRarityID.Purple;
            item.Calamity().customRarity = CalamityRarity.Turquoise;
            item.shootSpeed = 12f;
            item.shoot = ModContent.ProjectileType<BloodBoilerFire>();
        }

        public override Vector2? HoldoutOffset() => new Vector2(-5, 0);

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (Main.rand.NextFloat() > 0.75f)
                --player.statLife;
            if (player.statLife <= 0)
            {
                PlayerDeathReason pdr = PlayerDeathReason.ByCustomReason(Main.rand.NextBool(2) ? player.name + " suffered from severe anemia." : player.name + " was unable to obtain a blood transfusion.");
                player.KillMe(pdr, 1000.0, 0, false);
                return false;
            }
            return true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<BloodstoneCore>(), 6);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
