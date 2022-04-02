using CalamityMod.Projectiles.Magic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class ApoctosisArray : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Apoctosis Array");
            Tooltip.SetDefault("Fires ion blasts that speed up and then explode\n" +
                "Damage scales with how full your mana is\n" +
                "Using Astral Injection reduces the effectiveness of the mana boost");
        }

        public override void SetDefaults()
        {
            item.damage = 99;
            item.width = 98;
            item.height = 34;
            item.magic = true;
            item.mana = 12;
            item.useAnimation = 7;
            item.useTime = 7;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.knockBack = 6.75f;
            item.UseSound = SoundID.Item91;
            item.autoReuse = true;
            item.noMelee = true;
            item.value = CalamityGlobalItem.Rarity10BuyPrice;
            item.rare = ItemRarityID.Red;
            item.shoot = ModContent.ProjectileType<IonBlast>();
            item.shootSpeed = 8f;
        }

        public override Vector2? HoldoutOffset() => new Vector2(-25, 0);

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            float manaRatio = (float)player.statMana / player.statManaMax2;
            bool injectionNerf = player.Calamity().astralInjection;
            if (injectionNerf)
                manaRatio = MathHelper.Min(manaRatio, 0.65f);

            // 20% to 160% damage. Astral Injection caps it at 111% damage.
            float damageRatio = 0.2f + 1.4f * manaRatio;
            int finalDamage = (int)(damage * damageRatio);

            Projectile proj = Projectile.NewProjectileDirect(position, new Vector2(speedX, speedY), type, finalDamage, knockBack, player.whoAmI);
            proj.scale = 0.75f + 0.75f * manaRatio;
            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<IonBlaster>());
            recipe.AddIngredient(ItemID.LunarBar, 5);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
