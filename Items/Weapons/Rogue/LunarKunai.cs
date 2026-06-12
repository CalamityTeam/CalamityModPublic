using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Projectiles.Rogue;
using CalamityMod.Systems.Collections;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class LunarKunai : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            CalamityItemSets.ExtraDebuffTooltip_Enemy[Type] = [ModContent.BuffType<Nightwither>()];
        }
        public override void SetDefaults()
        {
            Item.width = 38;
            Item.height = 38;
            Item.damage = 102;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.useAnimation = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 10;
            Item.knockBack = 2f;
            Item.UseSound = SoundID.Item39;
            Item.autoReuse = true;
            Item.value = CalamityGlobalItem.RarityPurpleBuyPrice;
            Item.rare = ItemRarityID.Purple;
            Item.shoot = ModContent.ProjectileType<LunarKunaiProj>();
            Item.shootSpeed = 22f;
            Item.DamageType = RogueDamageClass.Instance;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int projAmt = player.Calamity().StealthStrikeAvailable() ? 10 : 3;
            for (int i = 0; i < projAmt; i++)
            {
                Vector2 spreadVel = velocity.RotatedByRandom(MathHelper.Pi / 20f);

                int stealth = Projectile.NewProjectile(source, position, spreadVel, ModContent.ProjectileType<LunarKunaiProj>(), damage, knockback, player.whoAmI);
                if (stealth.WithinBounds(Main.maxProjectiles) && player.Calamity().StealthStrikeAvailable())
                    Main.projectile[stealth].Calamity().stealthStrike = true;
            }
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.LunarBar, 5).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
