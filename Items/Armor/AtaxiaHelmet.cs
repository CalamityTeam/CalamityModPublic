using CalamityMod.CalPlayer;
using Terraria; using CalamityMod.Projectiles; using Terraria.ModLoader;
using Terraria.ID;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Items
{
    [AutoloadEquip(EquipType.Head)]
    public class AtaxiaHelmet : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ataxia Helmet");
            Tooltip.SetDefault("5% increased minion damage and increased minion knockback\n" +
                "+2 max minions\n" +
                "Temporary immunity to lava and immunity to fire damage");
        }

        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
            item.value = Item.buyPrice(0, 30, 0, 0);
            item.rare = 8;
            item.defense = 6; //40
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<AtaxiaArmor>() && legs.type == ModContent.ItemType<AtaxiaSubligar>();
        }

        public override void ArmorSetShadows(Player player)
        {
            player.armorEffectDrawOutlines = true;
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = "40% increased minion damage\n" +
                "Inferno effect when below 50% life\n" +
                "Summons a chaos spirit to protect you\n" +
                "You have a 20% chance to emit a blazing explosion when you are hit";
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.ataxiaBlaze = true;
            modPlayer.chaosSpirit = true;
            if (player.whoAmI == Main.myPlayer)
            {
                if (player.FindBuffIndex(ModContent.BuffType<Buffs.ChaosSpirit>()) == -1)
                {
                    player.AddBuff(ModContent.BuffType<Buffs.ChaosSpirit>(), 3600, true);
                }
                if (player.ownedProjectileCounts[ModContent.ProjectileType<Projectiles.ChaosSpirit>()] < 1)
                {
                    Projectile.NewProjectile(player.Center.X, player.Center.Y, 0f, -1f, ModContent.ProjectileType<Projectiles.ChaosSpirit>(), (int)(190f * player.minionDamage), 0f, Main.myPlayer, 0f, 0f);
                }
            }
            player.minionDamage += 0.4f;
        }

        public override void UpdateEquip(Player player)
        {
            player.minionDamage += 0.05f;
            player.minionKB += 1.5f;
            player.maxMinions += 2;
            player.lavaMax += 240;
            player.buffImmune[BuffID.OnFire] = true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "CruptixBar", 7);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
