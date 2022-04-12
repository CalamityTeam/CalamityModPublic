using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using CalamityMod.Projectiles.Typeless;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Typeless
{
    public class LunicEye : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Lunic Eye");
            Tooltip.SetDefault("Fires lunic beams that reduce enemy protection\n" +
                "This weapon scales with all your damage stats at once");
        }

        public override void SetDefaults()
        {
            Item.width = 80;
            Item.damage = 9;
            Item.value = CalamityGlobalItem.Rarity4BuyPrice;
            Item.rare = ItemRarityID.LightRed;
            Item.useAnimation = 15;
            Item.useTime = 15;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 4.5f;
            Item.UseSound = SoundLoader.GetLegacySoundSlot(Mod, "Sounds/Item/LaserCannon");
            Item.autoReuse = true;
            Item.noMelee = true;
            Item.height = 50;
            Item.shoot = ModContent.ProjectileType<LunicBeam>();
            Item.shootSpeed = 12f;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-15, 0);
        }

        // Lunic Eye scales off of all damage types simultaneously (meaning it scales 5x from universal damage boosts).
        public override void ModifyWeaponDamage(Player player, ref StatModifier damage)
        {
            float formula = 5f * (player.GetDamage<GenericDamageClass>().Base - 1f);
            formula += player.GetDamage(DamageClass.Melee).Base - 1f;
            formula += player.GetDamage(DamageClass.Ranged).Base - 1f;
            formula += player.GetDamage(DamageClass.Magic).Base - 1f;
            formula += player.GetDamage(DamageClass.Summon).Base - 1f;
            formula += player.Calamity().throwingDamage - 1f;
            damage += formula;
        }

        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ModContent.ItemType<Stardust>(), 20).AddIngredient(ModContent.ItemType<SeaPrism>(), 15).AddIngredient(ModContent.ItemType<AerialiteBar>(), 15).AddIngredient(ItemID.SunplateBlock, 15).AddTile(TileID.Anvils).Register();
        }
    }
}
