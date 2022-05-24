using Terraria.DataStructures;
using CalamityMod.Projectiles.Summon;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.NPCs.SlimeGod

namespace CalamityMod.Items.Weapons.Summon
{
    public class SlimePuppetStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Slime Puppet Staff");
            Tooltip.SetDefault("Summons a slime ball that follows you\n" +
                                "The ball flies toward nearby enemies and explodes into slime on enemy hits\n" +
                                "Does not consume minion slots"); // In other words, bootleg mage :TaxEvasion:
            Item.staff[Item.type] = true;
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 26;
            Item.mana = 10;
            Item.width = 48;
            Item.height = 56;
            Item.useTime = Item.useAnimation = 29;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 3.6f;
            Item.value = CalamityGlobalItem.Rarity4BuyPrice;
            Item.rare = ItemRarityID.LightRed;
            Item.UseSound = SlimeGodCore.PossessionSound;
            Item.shoot = ModContent.ProjectileType<SlimePuppet>();
            Item.shootSpeed = 10f;
            Item.DamageType = DamageClass.Summon;
        }

        public override Vector2? HoldoutOrigin() => new Vector2(12f);

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse != 2)
            {
                int p = Projectile.NewProjectile(source, Main.MouseWorld, Vector2.Zero, type, damage, knockback, Main.myPlayer);
                if (Main.projectile.IndexInRange(p))
                    Main.projectile[p].originalDamage = Item.damage;
            }
            return false;
        }
    }
}
