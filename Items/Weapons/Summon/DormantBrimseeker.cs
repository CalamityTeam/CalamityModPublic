using Terraria.DataStructures;
using CalamityMod.Projectiles.Summon;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Summon
{
    public class DormantBrimseeker : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Dormant Brimseeker");
            Tooltip.SetDefault("You could've sworn that they turned even scarier when you looked at their reflections in a mirror\n" +
                               "Summons a brimseeker to keep you company\n" +
                               "Firing another brimseeker when all minion slots are filled summons a brimstone aura\n" +
                               "The aura empowers your brimseeker summons and produces damaging fireballs\n" +
                               "Only one aura can persist at a time");
        }

        public override void SetDefaults()
        {
            Item.damage = 42;
            Item.mana = 10;
            Item.width = Item.height = 32;
            Item.useTime = Item.useAnimation = 25;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.knockBack = 3f;
            Item.UseSound = SoundID.Item20;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<DormantBrimseekerSummoner>();
            Item.shootSpeed = 10f;
            Item.DamageType = DamageClass.Summon;

            Item.value = Item.buyPrice(0, 36, 0, 0);
            Item.rare = ItemRarityID.Pink;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            float totalSlots = 0f;
            for (int i = 0; i < Main.projectile.Length; i++)
            {
                if (Main.projectile[i].minion && Main.projectile[i].active && Main.projectile[i].owner == player.whoAmI)
                {
                    totalSlots += Main.projectile[i].minionSlots;
                }
            }
            if (totalSlots < player.maxMinions)
            {
                Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, 0f, 0f);
            }
            else if (player.ownedProjectileCounts[ModContent.ProjectileType<DormantBrimseekerAura>()] <= 0f)
            {
                Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<DormantBrimseekerAura>(), damage * 2, knockback, player.whoAmI, 0f, 0f);
            }
            return false;
        }
    }
}
