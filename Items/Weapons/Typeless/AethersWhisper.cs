using CalamityMod.Items.Materials;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Projectiles.Hybrid;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Typeless
{
    public class AethersWhisper : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Aether's Whisper");
            Tooltip.SetDefault("Inflicts long-lasting shadowflame and splits on tile hits\n" +
                "Projectiles gain damage as they travel\n" +
                "Right click to change from magic to ranged damage\n" +
                "Right click consumes no mana");
        }

        public override void SetDefaults()
        {
            item.damage = 504;
            item.knockBack = 5.5f;
            item.useTime = item.useAnimation = 24;
            item.shootSpeed = 12f;
            item.shoot = ModContent.ProjectileType<AetherBeam>();
            item.mana = 30;
            item.magic = true;
            item.autoReuse = true;

            item.width = 134;
            item.height = 44;
            item.noMelee = true;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.UseSound = mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/LaserCannon");

            item.value = CalamityGlobalItem.Rarity12BuyPrice;
            item.Calamity().customRarity = CalamityRarity.Turquoise;
        }

        public override Vector2? HoldoutOffset() => new Vector2(-10, 0);

        public override bool AltFunctionUse(Player player) => true;

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                item.ranged = true;
                item.magic = false;
            }
            else
            {
                item.ranged = false;
                item.magic = true;
            }
            return base.CanUseItem(player);
        }

        public override bool OnPickup(Player player)
        {
            item.ranged = false;
            item.magic = true;
            return true;
        }

        public override void UpdateInventory(Player player)
        {
            // Reset to magic if not using an item to prevent sorting bugs.
            if (player.itemAnimation <= 0)
            {
                item.ranged = false;
                item.magic = true;
            }
        }

        public override void ModifyManaCost(Player player, ref float reduce, ref float mult)
        {
            if (player.altFunctionUse == 2)
                mult *= 0f;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            // This is coded weirdly because it defaults to using magic boosts without this.
            int dmg = player.GetWeaponDamage(player.ActiveItem());
            float ai0 = player.altFunctionUse == 2 ? 1f : 0f;
            Projectile.NewProjectile(position, new Vector2(speedX, speedY), type, dmg, knockBack, player.whoAmI, ai0, 0f);
            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<PlasmaRod>());
            recipe.AddIngredient(ModContent.ItemType<Lazhar>());
            recipe.AddIngredient(ModContent.ItemType<SpectreRifle>());
            recipe.AddIngredient(ModContent.ItemType<TwistingNether>(), 3);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
