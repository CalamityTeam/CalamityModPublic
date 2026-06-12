using System.Linq;
using CalamityMod.Buffs.Summon;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Summon;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Summon
{
    public class SunSpiritStaff : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Summon";
        public override void SetDefaults()
        {
            Item.width = 44;
            Item.height = 48;
            Item.damage = 25;
            Item.mana = 10;
            Item.useAnimation = Item.useTime = 36;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.knockBack = 1.15f;
            Item.value = CalamityGlobalItem.RarityBlueBuyPrice;
            Item.rare = ItemRarityID.Blue;
            Item.UseSound = SoundID.Item44;
            Item.buffType = ModContent.BuffType<SolarSpirit>();
            Item.shoot = ModContent.ProjectileType<SunSpiritMinion>();
            Item.DamageType = DamageClass.Summon;
        }

        public override bool CanUseItem(Player player)
        {
            float minionSlotsAvailable = player.maxMinions;
            foreach (var item in Main.ActiveProjectiles)
            {
                if (item.owner == player.whoAmI)
                    minionSlotsAvailable -= item.minionSlots;
            }
            return minionSlotsAvailable >= 1;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.ownedProjectileCounts[type] > 0)
            {
                var p = Main.projectile.First(x => x.active && x.type == type && x.owner == player.whoAmI);
                p.ai[0]++;
                p.netUpdate = true;
                return false;
            }
            player.AddBuff(Item.buffType, 2);
            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.Amber, 5).
                AddIngredient(ItemID.AntlionMandible, 3).
                AddIngredient(ItemID.PalmWood, 10).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
