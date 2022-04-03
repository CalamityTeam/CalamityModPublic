using Terraria.ModLoader;
using Terraria.ID;
using CalamityMod.Projectiles.Summon;
using Terraria;
using Microsoft.Xna.Framework;

namespace CalamityMod.Items.Weapons.Summon
{
    public class EyeOfNight : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Eye of Night");
            Tooltip.SetDefault("Summons a diseased eyeball that fires cells which attach to enemies and inflict cursed flames");
        }

        public override void SetDefaults()
        {
            Item.damage = 24;
            Item.mana = 10;
            Item.width = Item.height = 36;
            Item.useTime = Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.knockBack = 1f;
            Item.value = CalamityGlobalItem.Rarity3BuyPrice;
            Item.rare = ItemRarityID.Orange;

            Item.UseSound = SoundID.NPCHit1;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<EyeOfNightSummon>();
            Item.shootSpeed = 10f;
            Item.DamageType = DamageClass.Summon;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.altFunctionUse != 2)
                Projectile.NewProjectile(Main.MouseWorld, Vector2.UnitY * -3f, type, damage, knockBack, player.whoAmI);
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ModContent.ItemType<BelladonnaSpiritStaff>()).AddIngredient(ModContent.ItemType<StaffOfNecrosteocytes>()).AddIngredient(ModContent.ItemType<VileFeeder>()).AddIngredient(ItemID.ImpStaff).AddTile(TileID.DemonAltar).Register();
        }
    }
}
