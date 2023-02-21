using Terraria.DataStructures;
using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Items.Weapons.Rogue
{
    public class MythrilKnife : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mythril Knife");
            Tooltip.SetDefault("Stealth strikes inflict are coated in deadly toxins, inflicting irradiated, poison, and acid venom");
            SacrificeTotal = 99;
        }

        public override void SetDefaults()
        {
            Item.width = 12;
            Item.damage = 80;
            Item.noMelee = true;
            Item.consumable = true;
            Item.noUseGraphic = true;
            Item.useAnimation = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 10;
            Item.knockBack = 1.75f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.height = 40;
            Item.maxStack = 999;
            Item.value = 1100;
            Item.rare = ItemRarityID.LightRed;
            Item.shoot = ModContent.ProjectileType<MythrilKnifeProjectile>();
            Item.shootSpeed = 12f;
            Item.DamageType = RogueDamageClass.Instance;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.Calamity().StealthStrikeAvailable())
            {
                int stealth = Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
                if (stealth.WithinBounds(Main.maxProjectiles))
                    Main.projectile[stealth].Calamity().stealthStrike = true;
                return false;
            }
            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe(150).
                AddIngredient(ItemID.MythrilBar).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
