using CalamityMod.Projectiles.Melee;
using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class Bonebreaker : ModItem
    {
        public const int BaseDamage = 60;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bonebreaker");
            Tooltip.SetDefault("Fires javelins that stick to enemies before bursting into shrapnel");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useAnimation = 15;
            Item.useTime = 15;
            Item.width = 32;
            Item.height = 32;
            Item.damage = BaseDamage;
            Item.DamageType = DamageClass.MeleeNoSpeed;
            Item.knockBack = 7f;
            Item.UseSound = SoundID.Item1;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.value = Item.buyPrice(0, 36, 0, 0);
            Item.rare = ItemRarityID.Pink;
            Item.shoot = ModContent.ProjectileType<BonebreakerProjectile>();
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.shootSpeed = 12f;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.BoneJavelin, 150).
                AddIngredient<CorrodedFossil>(15).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
