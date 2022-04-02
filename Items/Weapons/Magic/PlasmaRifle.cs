using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Magic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class PlasmaRifle : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Plasma Rifle");
            Tooltip.SetDefault("Fires a plasma blast that explodes\n" +
                "Right click to fire plasma bolts");
        }

        public override void SetDefaults()
        {
            item.damage = 160;
            item.mana = 40;
            item.magic = true;
            item.width = 72;
            item.height = 20;
            item.useTime = item.useAnimation = 8;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 4f;
            item.value = CalamityGlobalItem.Rarity12BuyPrice;
            item.rare = ItemRarityID.Purple;
            item.Calamity().customRarity = CalamityRarity.Turquoise;
            item.UseSound = mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/PlasmaBlast");
            item.autoReuse = true;
            item.shootSpeed = 12f;
            item.shoot = ModContent.ProjectileType<PlasmaShot>();
        }

        public override Vector2? HoldoutOffset() => new Vector2(-10, 0);

        public override bool AltFunctionUse(Player player) => true;

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                item.UseSound = mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/PlasmaBolt");
            }
            else
            {
                item.UseSound = mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/PlasmaBlast");
            }
            return base.CanUseItem(player);
        }

        public override void ModifyManaCost(Player player, ref float reduce, ref float mult)
        {
            if (player.altFunctionUse == 2)
                mult *= 0.25f;
        }

        public override float UseTimeMultiplier    (Player player)
        {
            if (player.altFunctionUse == 2)
                return 1f;
            return 0.2f;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.altFunctionUse == 2)
            {
                damage = (int)(damage * 1.15);
                Projectile.NewProjectile(position, new Vector2(speedX, speedY), ModContent.ProjectileType<PlasmaBolt>(), damage, knockBack, player.whoAmI);
            }
            else
                Projectile.NewProjectile(position, new Vector2(speedX, speedY), type, (int)(damage * 0.88), knockBack, player.whoAmI);
            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.ToxicFlask);
            recipe.AddIngredient(ItemID.Musket);
            recipe.AddIngredient(ModContent.ItemType<UeliaceBar>(), 7);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
            recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.ToxicFlask);
            recipe.AddIngredient(ItemID.TheUndertaker);
            recipe.AddIngredient(ModContent.ItemType<UeliaceBar>(), 7);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
