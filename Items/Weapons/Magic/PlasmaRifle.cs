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
            Item.damage = 160;
            Item.mana = 40;
            Item.DamageType = DamageClass.Magic;
            Item.width = 72;
            Item.height = 20;
            Item.useTime = Item.useAnimation = 8;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 4f;
            Item.value = CalamityGlobalItem.Rarity12BuyPrice;
            Item.rare = ItemRarityID.Purple;
            Item.Calamity().customRarity = CalamityRarity.Turquoise;
            Item.UseSound = Mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/PlasmaBlast");
            Item.autoReuse = true;
            Item.shootSpeed = 12f;
            Item.shoot = ModContent.ProjectileType<PlasmaShot>();
        }

        public override Vector2? HoldoutOffset() => new Vector2(-10, 0);

        public override bool AltFunctionUse(Player player) => true;

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                Item.UseSound = Mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/PlasmaBolt");
            }
            else
            {
                Item.UseSound = Mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/PlasmaBlast");
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
            CreateRecipe(1).AddIngredient(ItemID.ToxicFlask).AddIngredient(ItemID.Musket).AddIngredient(ModContent.ItemType<UeliaceBar>(), 7).AddTile(TileID.LunarCraftingStation).Register();
            CreateRecipe(1).AddIngredient(ItemID.ToxicFlask).AddIngredient(ItemID.TheUndertaker).AddIngredient(ModContent.ItemType<UeliaceBar>(), 7).AddTile(TileID.LunarCraftingStation).Register();
        }
    }
}
